// ------------------------------------------------------------------------------------------------
//  <copyright file="SessionService.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.Dal
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

    using SysML2.NET.PSM;
    using SysML2.NET.REST.Exceptions;

    using SySML2.NET.REST;

    using Project = SysML2.NET.PSM.DTO.Project;

    /// <summary>
    /// Holds the connection to a server through the session of the SysML2.NET REST client, so that a server that cannot be
    /// reached, answers late or not as the API expects gives <see cref="SessionErrors.Unavailable" /> instead of an exception.
    /// </summary>
    /// <remarks>
    /// The service opens the address it is given: a host that lets users type the address decides which servers it may reach.
    /// </remarks>
    public sealed partial class SessionService : ISessionService
    {
        /// <summary>
        /// The number of pages of projects read at most, so that a server that keeps sending a next page cannot hold the client.
        /// </summary>
        private const int MaximumPageCount = 1000;

        /// <summary>
        /// The time a server has to answer a request before the attempt fails, so that a server that never answers does not hold
        /// the client.
        /// </summary>
        private static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(10);

        /// <summary>
        /// The logger that receives a failed request.
        /// </summary>
        private readonly ILogger<SessionService> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionService" /> class.
        /// </summary>
        /// <param name="session">The session of the SysML2.NET REST client.</param>
        /// <param name="logger">The logger that receives a failed request.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="session" /> or <paramref name="logger" /> is <see langword="null" />.</exception>
        public SessionService(ISession session, ILogger<SessionService> logger)
        {
            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }

            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            this.Session = session;
            this.logger = logger;
        }

        /// <summary>
        /// Gets the session of the SysML2.NET REST client, through which the engineering data of the open connection is queried.
        /// </summary>
        public ISession Session { get; }

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
        /// <returns>The projects of the server, or <see cref="SessionErrors.Unavailable" /> when the server does not answer as the API expects.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="serverAddress" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="serverAddress" /> is not absolute.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the request is cancelled.</exception>
        public async Task<ErrorOr<IReadOnlyList<Project>>> OpenAsync(Uri serverAddress, CancellationToken cancellationToken)
        {
            if (serverAddress == null)
            {
                throw new ArgumentNullException(nameof(serverAddress));
            }

            this.ServerAddress = null;

            try
            {
                var read = await this.ReadProjectsAsync(token => this.Session.Open(serverAddress, token), cancellationToken);

                if (read.IsError)
                {
                    this.Session.Close();
                    return read.Errors;
                }

                this.ServerAddress = serverAddress;
                return read;
            }
            catch
            {
                this.Session.Close();
                throw;
            }
        }

        /// <summary>
        /// Reads every page of the projects of the open connection. A read that fails keeps the connection open.
        /// </summary>
        /// <param name="cancellationToken">The token that cancels the request.</param>
        /// <returns>The projects, or <see cref="SessionErrors.Unavailable" /> when no connection is open or the server does not answer as the API expects.</returns>
        /// <exception cref="OperationCanceledException">Thrown when the request is cancelled.</exception>
        public async Task<ErrorOr<IReadOnlyList<Project>>> GetProjectsAsync(CancellationToken cancellationToken)
        {
            if (this.ServerAddress == null || this.Session.Client is not { } client)
            {
                return SessionErrors.Unavailable;
            }

            return await this.ReadProjectsAsync(token => client.GetProjectsAsync(new QueryParameters(), token), cancellationToken);
        }

        /// <summary>
        /// Reads one project of the open connection from the server.
        /// </summary>
        /// <param name="projectId">The identifier of the project.</param>
        /// <param name="cancellationToken">The token that cancels the request.</param>
        /// <returns>The project, <see cref="SessionErrors.ProjectNotFound" /> when the server does not know it, or <see cref="SessionErrors.Unavailable" /> when no connection is open or the server does not answer as the API expects.</returns>
        /// <exception cref="OperationCanceledException">Thrown when the request is cancelled.</exception>
        public async Task<ErrorOr<Project>> GetProjectAsync(Guid projectId, CancellationToken cancellationToken)
        {
            if (this.ServerAddress == null || this.Session.Client is not { } client)
            {
                return SessionErrors.Unavailable;
            }

            var read = await this.RequestAsync(token => ReadProjectAsync(client, projectId, token), cancellationToken);

            if (read.IsError)
            {
                return read.Errors;
            }

            if (read.Value.Length == 0)
            {
                return SessionErrors.ProjectNotFound;
            }

            return read.Value[0];
        }

        /// <summary>
        /// Closes the open connection, if any, which drops the model assembled by the session.
        /// </summary>
        public void Close()
        {
            this.Session.Close();
            this.ServerAddress = null;
        }

        /// <summary>
        /// Reads one project, as a list that is empty when the server answers that it does not know it.
        /// </summary>
        /// <param name="client">The client of the open connection.</param>
        /// <param name="projectId">The identifier of the project.</param>
        /// <param name="cancellationToken">The token that cancels the request.</param>
        /// <returns>The project, or no project when the server does not know it.</returns>
        private static async Task<Project[]> ReadProjectAsync(IPsmApiClient client, Guid projectId, CancellationToken cancellationToken)
        {
            try
            {
                return [await client.GetProjectByIdAsync(projectId, cancellationToken)];
            }
            catch (PsmApiException exception) when (exception.StatusCode == HttpStatusCode.NotFound)
            {
                return [];
            }
        }

        /// <summary>
        /// Reads every page of projects: the first one from the given request, the next ones by following the cursor of the page
        /// before. Pages that repeat a cursor, or more than a thousand of them, give <see cref="SessionErrors.Unavailable" />.
        /// </summary>
        /// <param name="firstPage">The request of the first page, which receives the token that cancels it.</param>
        /// <param name="cancellationToken">The token that cancels the request.</param>
        /// <returns>The projects of every page, or <see cref="SessionErrors.Unavailable" />.</returns>
        /// <exception cref="OperationCanceledException">Thrown when the request is cancelled.</exception>
        private async Task<ErrorOr<IReadOnlyList<Project>>> ReadProjectsAsync(Func<CancellationToken, Task<PagedResult<Project>>> firstPage, CancellationToken cancellationToken)
        {
            var page = await this.RequestAsync(firstPage, cancellationToken);
            var read = new List<Project>();
            var readCursors = new HashSet<string>(StringComparer.Ordinal);

            while (!page.IsError)
            {
                read.AddRange(page.Value.Records);

                if (page.Value.Next is not { } cursor)
                {
                    return read;
                }

                if (!readCursors.Add(cursor.Token) || readCursors.Count >= MaximumPageCount)
                {
                    return SessionErrors.Unavailable;
                }

                var parameters = new QueryParameters { PageAfter = cursor };
                page = await this.RequestAsync(token => this.Session.Client.GetProjectsAsync(parameters, token), cancellationToken);
            }

            return page.Errors;
        }

        /// <summary>
        /// Sends one request to the server with its own time limit, and turns a server that cannot be reached or does not answer
        /// as the API expects into <see cref="SessionErrors.Unavailable" />.
        /// </summary>
        /// <typeparam name="T">The type of the answer.</typeparam>
        /// <param name="request">The request, which receives the token that cancels it.</param>
        /// <param name="cancellationToken">The token that cancels the request.</param>
        /// <returns>The answer, or <see cref="SessionErrors.Unavailable" />.</returns>
        /// <exception cref="OperationCanceledException">Thrown when the request is cancelled.</exception>
        private async Task<ErrorOr<T>> RequestAsync<T>(Func<CancellationToken, Task<T>> request, CancellationToken cancellationToken)
        {
            using (var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            {
                timeout.CancelAfter(RequestTimeout);

                try
                {
                    return await request(timeout.Token);
                }
                catch (Exception exception) when (IsServerFailure(exception, cancellationToken))
                {
                    this.LogRequestFailure(exception);
                    return SessionErrors.Unavailable;
                }
            }
        }

        /// <summary>
        /// Tells whether a request failed because of the server: unreachable, too late, or not answering as the API expects. An
        /// unsupported scheme, such as <c>ftp</c>, reaches no server either.
        /// </summary>
        /// <param name="exception">The failure.</param>
        /// <param name="cancellationToken">The token of the caller, whose own cancellation is not a failure of the server.</param>
        /// <returns><see langword="true" /> when the failure is the server's.</returns>
        private static bool IsServerFailure(Exception exception, CancellationToken cancellationToken) => exception switch
        {
            HttpRequestException or PsmApiException or JsonException or NotSupportedException => true,
            OperationCanceledException => !cancellationToken.IsCancellationRequested,
            _ => false
        };

        /// <summary>
        /// Logs a request that failed. The message template holds no address, but the exception, kept for diagnosis, may name the
        /// host or carry the text of the server.
        /// </summary>
        /// <param name="exception">The failure.</param>
        [LoggerMessage(Level = LogLevel.Warning, Message = "A request to the model server failed.")]
        private partial void LogRequestFailure(Exception exception);
    }
}
