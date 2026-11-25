// ------------------------------------------------------------------------------------------------
//  <copyright file="ProjectServerHandler.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.Dal.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Answers the requests of a real SysML2.NET session as a Systems Modeling API server would, from one answer per request
    /// path and query, so that the session service is tested without a mocking library.
    /// </summary>
    internal sealed class ProjectServerHandler : HttpMessageHandler
    {
        /// <summary>
        /// The answers, keyed by the unescaped path and query of the request.
        /// </summary>
        private readonly Dictionary<string, Func<HttpResponseMessage>> answers = new Dictionary<string, Func<HttpResponseMessage>>(StringComparer.Ordinal);

        /// <summary>
        /// Gets the unescaped path and query of every request received, in order.
        /// </summary>
        public List<string> Requests { get; } = [];

        /// <summary>
        /// Gets or sets the answer to a request that has no answer of its own, "not found" unless a test sets another.
        /// </summary>
        public Func<HttpRequestMessage, HttpResponseMessage> Fallback { get; set; } = _ => new HttpResponseMessage(HttpStatusCode.NotFound);

        /// <summary>
        /// Sets the answer to a request.
        /// </summary>
        /// <param name="pathAndQuery">The unescaped path and query of the request.</param>
        /// <param name="answer">The answer.</param>
        public void Answer(string pathAndQuery, Func<HttpResponseMessage> answer)
        {
            this.answers[pathAndQuery] = answer;
        }

        /// <summary>
        /// Answers a request from its path and query, or from <see cref="Fallback" />.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The token that cancels the request.</param>
        /// <returns>The answer.</returns>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var pathAndQuery = Uri.UnescapeDataString(request.RequestUri.PathAndQuery);
            this.Requests.Add(pathAndQuery);

            return Task.FromResult(this.answers.TryGetValue(pathAndQuery, out var answer) ? answer() : this.Fallback(request));
        }
    }
}
