// ------------------------------------------------------------------------------------------------
//  <copyright file="ISessionService.cs" company="Starion Group S.A.">
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
    using System.Threading;
    using System.Threading.Tasks;

    using ErrorOr;

    using SySML2.NET.REST;

    using Project = SysML2.NET.PSM.DTO.Project;

    /// <summary>
    /// The connection of a client to a server that implements the OMG Systems Modeling API and Services, from its opening until
    /// it closes, so that every Mycelium client handles its session the same way. The service keeps no record of its own: what
    /// it returns is read from the server, and the SysML2.NET session assembles the elements of a commit.
    /// </summary>
    public interface ISessionService
    {
        /// <summary>
        /// Gets the session of the SysML2.NET REST client, through which the engineering data of the open connection is queried.
        /// The connection is opened and closed through this service only, which keeps <see cref="ServerAddress" /> in step
        /// with it.
        /// </summary>
        ISession Session { get; }

        /// <summary>
        /// Gets the address of the server of the open connection, or <see langword="null" /> when no connection is open.
        /// </summary>
        Uri ServerAddress { get; }

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
        Task<ErrorOr<IReadOnlyList<Project>>> OpenAsync(Uri serverAddress, CancellationToken cancellationToken);

        /// <summary>
        /// Reads every page of the projects of the open connection. A read that fails keeps the connection open.
        /// </summary>
        /// <param name="cancellationToken">The token that cancels the request.</param>
        /// <returns>The projects, or <see cref="SessionErrors.Unavailable" /> when no connection is open or the server does not answer as the API expects.</returns>
        /// <exception cref="OperationCanceledException">Thrown when the request is cancelled.</exception>
        Task<ErrorOr<IReadOnlyList<Project>>> GetProjectsAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Reads one project of the open connection from the server.
        /// </summary>
        /// <param name="projectId">The identifier of the project.</param>
        /// <param name="cancellationToken">The token that cancels the request.</param>
        /// <returns>The project, <see cref="SessionErrors.ProjectNotFound" /> when the server does not know it, or <see cref="SessionErrors.Unavailable" /> when no connection is open or the server does not answer as the API expects.</returns>
        /// <exception cref="OperationCanceledException">Thrown when the request is cancelled.</exception>
        Task<ErrorOr<Project>> GetProjectAsync(Guid projectId, CancellationToken cancellationToken);

        /// <summary>
        /// Closes the open connection, if any, which drops the model assembled by the session.
        /// </summary>
        void Close();
    }
}
