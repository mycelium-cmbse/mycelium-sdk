// ------------------------------------------------------------------------------------------------
//  <copyright file="SessionServiceOptions.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.REST
{
    using System;

    /// <summary>
    /// The limits that the <see cref="SessionService" /> puts on a server, set by the client that registers the service.
    /// </summary>
    public sealed class SessionServiceOptions
    {
        /// <summary>
        /// Gets or sets the number of pages read at most for one list, so that a server that keeps sending a next page cannot
        /// hold the client. It must be greater than zero.
        /// </summary>
        public int MaximumPageCount { get; set; } = 1000;

        /// <summary>
        /// Gets or sets the time a server has to answer one request, so that a server that never answers cannot hold the client.
        /// It must be greater than zero, or <see cref="System.Threading.Timeout.InfiniteTimeSpan" /> for no limit. It does not apply
        /// to an operation run with <see cref="ISessionService.ExecuteAsync" />, which may load a whole commit.
        /// </summary>
        public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromSeconds(10);
    }
}
