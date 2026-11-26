// ------------------------------------------------------------------------------------------------
//  <copyright file="SessionService.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.REST
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    using ErrorOr;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    using SysML2.NET.PSM;
    using SysML2.NET.REST.Exceptions;

    using SySML2.NET.REST;

    using Project = SysML2.NET.PSM.DTO.Project;

    /// <summary>
    /// Holds the connection to a server through the session of the SysML2.NET REST client. A server that cannot be reached,
    /// answers late or not as the API expects, a cancelled request and any other failure come back as a
    /// <see cref="SessionErrors" /> error that carries the exception, instead of being thrown.
    /// </summary>
    /// <remarks>
    /// The service opens the address it is given: a host that lets users type the address decides which servers it may reach.
    /// It is not thread-safe: a client keeps one service per user connection and does not open, close and read concurrently.
    /// </remarks>
    public sealed partial class SessionService : ISessionService
    {
        /// <summary>
        /// The session of the SysML2.NET REST client, which only this service opens and closes.
        /// </summary>
        private readonly ISession session;

        /// <summary>
        /// The limits put on the server.
        /// </summary>
        private readonly SessionServiceOptions options;

        /// <summary>
        /// The logger that receives a failed request.
        /// </summary>
        private readonly ILogger<SessionService> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionService" /> class.
        /// </summary>
        /// <param name="session">The session of the SysML2.NET REST client.</param>
        /// <param name="options">The limits put on the server.</param>
        /// <param name="logger">The logger that receives a failed request.</param>
        /// <exception cref="ArgumentNullException">Thrown when an argument or the value of <paramref name="options" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when a limit of <paramref name="options" /> is not greater than zero.</exception>
        public SessionService(ISession session, IOptions<SessionServiceOptions> options, ILogger<SessionService> logger)
        {
            this.session = session ?? throw new ArgumentNullException(nameof(session));
            this.options = (options ?? throw new ArgumentNullException(nameof(options))).Value ?? throw new ArgumentNullException(nameof(options));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            if (this.options.MaximumPageCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(options), this.options.MaximumPageCount, "The maximum page count must be greater than zero.");
            }

            if (this.options.RequestTimeout != Timeout.InfiniteTimeSpan && (this.options.RequestTimeout <= TimeSpan.Zero || this.options.RequestTimeout.TotalMilliseconds > int.MaxValue))
            {
                throw new ArgumentOutOfRangeException(nameof(options), this.options.RequestTimeout, "The request timeout must be greater than zero and at most int.MaxValue milliseconds, or infinite.");
            }
        }

        /// <summary>
        /// Gets the address of the server of the open connection, or <see langword="null" /> when no connection is open.
        /// </summary>
        public Uri ServerAddress { get; private set; }

        /// <summary>
        /// Opens a connection to a server, replacing any open one, and reads every page of its projects. The open connection is
        /// dropped as soon as the attempt starts, so a failed or cancelled attempt leaves none.
        /// </summary>
        /// <param name="serverAddress">The absolute address of the server.</param>
        /// <param name="cancellationToken">The token that cancels the request.</param>
        /// <returns>The projects of the server, or the error that tells why the connection is not open.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="serverAddress" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="serverAddress" /> is not absolute.</exception>
        public async Task<ErrorOr<IReadOnlyList<Project>>> OpenAsync(Uri serverAddress, CancellationToken cancellationToken = default)
        {
            var address = serverAddress ?? throw new ArgumentNullException(nameof(serverAddress));

            if (!address.IsAbsoluteUri)
            {
                throw new ArgumentException("The address of the server must be absolute.", nameof(serverAddress));
            }

            this.ServerAddress = null;

            var read = await this.ReadPagesAsync(token => this.session.Open(address, token), (parameters, token) => this.session.Client.GetProjectsAsync(parameters, token), null, cancellationToken);

            if (read.IsError)
            {
                this.session.Close();
                return read.Errors;
            }

            this.ServerAddress = address;
            return read;
        }

        /// <summary>
        /// Reads every page of the projects of the open connection. A read that fails keeps the connection open.
        /// </summary>
        /// <param name="queryParameters">The query parameters of the first page, such as its size, or <see langword="null" />; they are not changed.</param>
        /// <param name="cancellationToken">The token that cancels the request.</param>
        /// <returns>The projects, <see cref="SessionErrors.NotOpen" /> without an open connection, or the error that tells why they cannot be read.</returns>
        public Task<ErrorOr<IReadOnlyList<Project>>> GetProjectsAsync(QueryParameters queryParameters = null, CancellationToken cancellationToken = default)
        {
            if (!this.IsOpen)
            {
                return Task.FromResult<ErrorOr<IReadOnlyList<Project>>>(SessionErrors.NotOpen);
            }

            return this.ReadPagesAsync(token => this.session.Client.GetProjectsAsync(queryParameters, token), (parameters, token) => this.session.Client.GetProjectsAsync(parameters, token), queryParameters, cancellationToken);
        }

        /// <summary>
        /// Reads one project of the open connection from the server.
        /// </summary>
        /// <param name="projectId">The identifier of the project.</param>
        /// <param name="cancellationToken">The token that cancels the request.</param>
        /// <returns>The project, <see cref="SessionErrors.ProjectNotFound" /> when the server does not know it, <see cref="SessionErrors.NotOpen" /> without an open connection, or the error that tells why it cannot be read.</returns>
        public async Task<ErrorOr<Project>> GetProjectAsync(Guid projectId, CancellationToken cancellationToken = default)
        {
            if (!this.IsOpen)
            {
                return SessionErrors.NotOpen;
            }

            var read = await this.RequestAsync(token => this.session.Client.GetProjectByIdAsync(projectId, token), cancellationToken, this.options.RequestTimeout);

            if (read.IsError && read.FirstError.Metadata?.TryGetValue(SessionErrors.ExceptionKey, out var cause) == true && cause is PsmApiException { StatusCode: HttpStatusCode.NotFound } notFound)
            {
                return SessionErrors.ProjectNotFound(notFound);
            }

            return read;
        }

        /// <summary>
        /// Runs an operation on the session of the open connection, such as loading a commit. The request timeout does not
        /// apply, because an operation may load a whole model: the caller cancels it with its token.
        /// </summary>
        /// <param name="operation">The operation, which receives the session and the token that cancels it.</param>
        /// <param name="cancellationToken">The token that cancels the operation.</param>
        /// <returns>Success, <see cref="SessionErrors.NotOpen" /> without an open connection, or the error that tells why the operation failed.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="operation" /> is <see langword="null" />.</exception>
        public async Task<ErrorOr<Success>> ExecuteAsync(Func<ISession, CancellationToken, Task> operation, CancellationToken cancellationToken = default)
        {
            var run = operation ?? throw new ArgumentNullException(nameof(operation));

            if (!this.IsOpen)
            {
                return SessionErrors.NotOpen;
            }

            return await this.RequestAsync(
                async token =>
                {
                    await run(this.session, token);
                    return Result.Success;
                },
                cancellationToken,
                Timeout.InfiniteTimeSpan);
        }

        /// <summary>
        /// Reads every page of a list of the open connection, following the cursor of each page.
        /// </summary>
        /// <typeparam name="T">The type of the records of the list.</typeparam>
        /// <param name="page">The request of one page, which receives the client, the query parameters of the page and the token that cancels it.</param>
        /// <param name="cancellationToken">The token that cancels the request.</param>
        /// <returns>The records of every page, <see cref="SessionErrors.NotOpen" /> without an open connection, or the error that tells why they cannot be read.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="page" /> is <see langword="null" />.</exception>
        public Task<ErrorOr<IReadOnlyList<T>>> ReadAllAsync<T>(Func<IPsmApiClient, QueryParameters, CancellationToken, Task<PagedResult<T>>> page, CancellationToken cancellationToken = default)
        {
            var request = page ?? throw new ArgumentNullException(nameof(page));

            if (!this.IsOpen)
            {
                return Task.FromResult<ErrorOr<IReadOnlyList<T>>>(SessionErrors.NotOpen);
            }

            return this.ReadPagesAsync(token => request(this.session.Client, new QueryParameters(), token), (parameters, token) => request(this.session.Client, parameters, token), null, cancellationToken);
        }

        /// <summary>
        /// Closes the open connection, if any, which drops the model assembled by the session.
        /// </summary>
        public void Close()
        {
            this.session.Close();
            this.ServerAddress = null;
        }

        /// <summary>
        /// Gets a value indicating whether a connection is open and its client is still there.
        /// </summary>
        private bool IsOpen => this.ServerAddress != null && this.session.Client != null;

        /// <summary>
        /// Reads the first page, then the next ones by following the cursor of the page before, with the page size the caller
        /// asked for. A cursor that repeats, or more pages than <see cref="SessionServiceOptions.MaximumPageCount" />, give
        /// <see cref="SessionErrors.Unavailable" />.
        /// </summary>
        /// <typeparam name="T">The type of the records.</typeparam>
        /// <param name="firstPage">The request of the first page.</param>
        /// <param name="nextPage">The request of a next page, which receives its query parameters.</param>
        /// <param name="queryParameters">The query parameters of the caller, read and never changed, or <see langword="null" />.</param>
        /// <param name="cancellationToken">The token that cancels the request.</param>
        /// <returns>The records of every page, or the error of the first page that failed.</returns>
        private async Task<ErrorOr<IReadOnlyList<T>>> ReadPagesAsync<T>(Func<CancellationToken, Task<PagedResult<T>>> firstPage, Func<QueryParameters, CancellationToken, Task<PagedResult<T>>> nextPage, QueryParameters queryParameters, CancellationToken cancellationToken)
        {
            var page = await this.RequestAsync(firstPage, cancellationToken, this.options.RequestTimeout);
            var records = new List<T>();
            var readCursors = new HashSet<string>(StringComparer.Ordinal);

            while (!page.IsError)
            {
                records.AddRange(page.Value.Records ?? []);

                if (page.Value.Next is not { } cursor)
                {
                    return records;
                }

                if (!readCursors.Add(cursor.Token) || readCursors.Count >= this.options.MaximumPageCount)
                {
                    return SessionErrors.Unavailable();
                }

                var parameters = new QueryParameters { PageSize = queryParameters?.PageSize ?? 0, PageAfter = cursor };
                page = await this.RequestAsync(token => nextPage(parameters, token), cancellationToken, this.options.RequestTimeout);
            }

            return page.Errors;
        }

        /// <summary>
        /// Sends one request with its own time limit, and turns every failure into an error that carries the exception.
        /// </summary>
        /// <typeparam name="T">The type of the answer.</typeparam>
        /// <param name="request">The request, which receives the token that cancels it.</param>
        /// <param name="cancellationToken">The token of the caller.</param>
        /// <param name="timeout">The time the server has to answer, or <see cref="Timeout.InfiniteTimeSpan" />.</param>
        /// <returns>The answer, or the error.</returns>
        private async Task<ErrorOr<T>> RequestAsync<T>(Func<CancellationToken, Task<T>> request, CancellationToken cancellationToken, TimeSpan timeout)
        {
            using (var limit = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            {
                limit.CancelAfter(timeout);

                try
                {
                    return await request(limit.Token);
                }
                catch (Exception exception)
                {
                    return this.ToError(exception, cancellationToken);
                }
            }
        }

        /// <summary>
        /// Turns a failed request into its error: the caller's cancellation, a server that cannot be reached, answers too late or
        /// not as the API expects, or another failure. Only the failures that are not the caller's own cancellation are logged.
        /// </summary>
        /// <param name="exception">The failure.</param>
        /// <param name="cancellationToken">The token of the caller.</param>
        /// <returns>The error, carrying the exception.</returns>
        private Error ToError(Exception exception, CancellationToken cancellationToken)
        {
            if (exception is OperationCanceledException && cancellationToken.IsCancellationRequested)
            {
                return SessionErrors.Cancelled(exception);
            }

            this.LogRequestFailure(exception);

            return exception switch
            {
                HttpRequestException or PsmApiException or JsonException or NotSupportedException or OperationCanceledException => SessionErrors.Unavailable(exception),
                _ => SessionErrors.Failed(exception)
            };
        }

        /// <summary>
        /// Logs a request that failed. The message template holds no address, but the exception, kept for diagnosis, may name the
        /// host or carry the text of the server.
        /// </summary>
        /// <param name="exception">The failure.</param>
        [LoggerMessage(Level = LogLevel.Warning, Message = "A request to the model server failed.")]
        private partial void LogRequestFailure(Exception exception);
    }
}
