// ------------------------------------------------------------------------------------------------
//  <copyright file="SessionErrors.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.Dal
{
    using ErrorOr;

    /// <summary>
    /// The errors that <see cref="ISessionService" /> returns, so that every client tells them apart by the same code.
    /// </summary>
    public static class SessionErrors
    {
        /// <summary>
        /// The error returned when the server cannot be reached, does not answer within the time limit, or does not answer as the
        /// API expects.
        /// </summary>
        public static readonly Error Unavailable = Error.Failure("Session.Unavailable", "Cannot connect to this server");

        /// <summary>
        /// The error returned when the server of the open connection has no project with the requested identifier.
        /// </summary>
        public static readonly Error ProjectNotFound = Error.NotFound("Session.ProjectNotFound", "The project does not exist on this server.");
    }
}
