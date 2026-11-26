// ------------------------------------------------------------------------------------------------
//  <copyright file="ISessionService.cs" company="Starion Group S.A.">
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
    using System.Threading;
    using System.Threading.Tasks;

    using ErrorOr;

    using SysML2.NET.PSM;

    using SySML2.NET.REST;

    using Project = SysML2.NET.PSM.DTO.Project;

    /// <summary>
    /// The connection of a client to a Systems Modeling API server, from its opening to its closing. The session of the
    /// SysML2.NET REST client stays inside the service, so that nothing reaches the server without an open connection; the
    /// engineering data is read through <see cref="ExecuteAsync" /> and <see cref="ReadAllAsync{T}" />. Every failure is
    /// returned as an error, never thrown.
    /// </summary>
    public interface ISessionService
    {
        /// <summary>
        /// Gets the address of the server of the open connection, or <see langword="null" /> when no connection is open.
        /// </summary>
        Uri ServerAddress { get; }

        /// <summary>
        /// Opens a connection to a server, replacing any open one, and reads every page of its projects. A failed or cancelled
        /// attempt leaves no connection.
        /// </summary>
        /// <param name="serverAddress">The absolute address of the server.</param>
        /// <param name="cancellationToken">The token that cancels the request.</param>
        /// <returns>The projects of the server, or the error that tells why the connection is not open.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="serverAddress" /> is <see langword="null" />.</exception>
        Task<ErrorOr<IReadOnlyList<Project>>> OpenAsync(Uri serverAddress, CancellationToken cancellationToken = default);

        /// <summary>
        /// Reads every page of the projects of the open connection. A read that fails keeps the connection open.
        /// </summary>
        /// <param name="queryParameters">The query parameters of the first page, such as its size, or <see langword="null" />; they are not changed.</param>
        /// <param name="cancellationToken">The token that cancels the request.</param>
        /// <returns>The projects, or the error that tells why they cannot be read.</returns>
        Task<ErrorOr<IReadOnlyList<Project>>> GetProjectsAsync(QueryParameters queryParameters = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Reads one project of the open connection from the server.
        /// </summary>
        /// <param name="projectId">The identifier of the project.</param>
        /// <param name="cancellationToken">The token that cancels the request.</param>
        /// <returns>The project, <see cref="SessionErrors.ProjectNotFound" /> when the server does not know it, or the error that tells why it cannot be read.</returns>
        Task<ErrorOr<Project>> GetProjectAsync(Guid projectId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Runs an operation on the session of the open connection, such as loading a commit.
        /// </summary>
        /// <param name="operation">The operation, which receives the session and the token that cancels it.</param>
        /// <param name="cancellationToken">The token that cancels the operation.</param>
        /// <returns>Success, or the error that tells why the operation failed.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="operation" /> is <see langword="null" />.</exception>
        Task<ErrorOr<Success>> ExecuteAsync(Func<ISession, CancellationToken, Task> operation, CancellationToken cancellationToken = default);

        /// <summary>
        /// Reads every page of a list of the open connection, following the cursor of each page.
        /// </summary>
        /// <typeparam name="T">The type of the records of the list.</typeparam>
        /// <param name="page">The request of one page, which receives the client, the query parameters of the page and the token that cancels it.</param>
        /// <param name="cancellationToken">The token that cancels the request.</param>
        /// <returns>The records of every page, or the error that tells why they cannot be read.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="page" /> is <see langword="null" />.</exception>
        Task<ErrorOr<IReadOnlyList<T>>> ReadAllAsync<T>(Func<IPsmApiClient, QueryParameters, CancellationToken, Task<PagedResult<T>>> page, CancellationToken cancellationToken = default);

        /// <summary>
        /// Closes the open connection, if any, which drops the model assembled by the session.
        /// </summary>
        void Close();
    }
}
