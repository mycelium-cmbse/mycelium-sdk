// ------------------------------------------------------------------------------------------------
//  <copyright file="SessionErrors.cs" company="Starion Group S.A.">
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

    using ErrorOr;

    /// <summary>
    /// The errors that the <see cref="SessionService" /> returns instead of throwing. An error caused by an exception carries
    /// it in its metadata under <see cref="ExceptionKey" />, so that the client can log or inspect it.
    /// </summary>
    public static class SessionErrors
    {
        /// <summary>
        /// The metadata key of the exception that caused an error.
        /// </summary>
        public const string ExceptionKey = "exception";

        /// <summary>
        /// The code of the error returned when no connection is open.
        /// </summary>
        public const string NotOpenCode = "Session.NotOpen";

        /// <summary>
        /// The code of the error returned when the server cannot be reached or does not answer as the API expects.
        /// </summary>
        public const string UnavailableCode = "Session.Unavailable";

        /// <summary>
        /// The code of the error returned when the server does not know the requested project.
        /// </summary>
        public const string ProjectNotFoundCode = "Session.ProjectNotFound";

        /// <summary>
        /// The code of the error returned when the caller cancelled the request.
        /// </summary>
        public const string CancelledCode = "Session.Cancelled";

        /// <summary>
        /// The code of the error returned when the request failed for a reason that is not the server's.
        /// </summary>
        public const string FailedCode = "Session.Failed";

        /// <summary>
        /// Gets the error returned when an operation needs an open connection and none is open.
        /// </summary>
        public static Error NotOpen => Error.Failure(NotOpenCode, "No connection to a server is open.");

        /// <summary>
        /// Creates the error returned when the server cannot be reached, answers too late, sends a page that fails, or sends pages
        /// that repeat or never end.
        /// </summary>
        /// <param name="exception">The exception that caused it, or <see langword="null" /> when the server answered but not as the API expects.</param>
        /// <returns>The error.</returns>
        public static Error Unavailable(Exception exception = null) => Error.Failure(UnavailableCode, "Cannot connect to this server", Metadata(exception));

        /// <summary>
        /// Creates the error returned when the server answers that it does not know the requested project.
        /// </summary>
        /// <param name="exception">The exception that carried the answer of the server.</param>
        /// <returns>The error.</returns>
        public static Error ProjectNotFound(Exception exception = null) => Error.NotFound(ProjectNotFoundCode, "The project does not exist on this server.", Metadata(exception));

        /// <summary>
        /// Creates the error returned when the caller cancelled the request.
        /// </summary>
        /// <param name="exception">The cancellation.</param>
        /// <returns>The error.</returns>
        public static Error Cancelled(Exception exception) => Error.Failure(CancelledCode, "The request was cancelled.", Metadata(exception));

        /// <summary>
        /// Creates the error returned when the request failed for a reason that is not the server's.
        /// </summary>
        /// <param name="exception">The failure.</param>
        /// <returns>The error.</returns>
        public static Error Failed(Exception exception) => Error.Unexpected(FailedCode, "The request failed.", Metadata(exception));

        /// <summary>
        /// Puts the exception that caused an error in its metadata.
        /// </summary>
        /// <param name="exception">The exception, or <see langword="null" />.</param>
        /// <returns>The metadata, empty without an exception.</returns>
        private static Dictionary<string, object> Metadata(Exception exception) => exception == null ? [] : new Dictionary<string, object> { [ExceptionKey] = exception };
    }
}
