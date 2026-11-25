// ------------------------------------------------------------------------------------------------
//  <copyright file="ProjectServerClientFactory.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.Dal.Tests
{
    using System.Net.Http;

    /// <summary>
    /// Creates the HTTP clients of a real SysML2.NET session over a <see cref="ProjectServerHandler" />.
    /// </summary>
    internal sealed class ProjectServerClientFactory : IHttpClientFactory
    {
        /// <summary>
        /// The handler that answers every client.
        /// </summary>
        private readonly ProjectServerHandler handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectServerClientFactory" /> class.
        /// </summary>
        /// <param name="handler">The handler that answers every client.</param>
        public ProjectServerClientFactory(ProjectServerHandler handler)
        {
            this.handler = handler;
        }

        /// <summary>
        /// Creates a client that the handler answers and that leaves the handler alive when it is disposed.
        /// </summary>
        /// <param name="name">The name of the client.</param>
        /// <returns>The client.</returns>
        public HttpClient CreateClient(string name)
        {
            return new HttpClient(this.handler, false);
        }
    }
}
