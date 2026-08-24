// ------------------------------------------------------------------------------------------------
//  <copyright file="OpenApiOperationExtensions.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;

    using Humanizer;

    using Microsoft.OpenApi;

    /// <summary>
    /// Provides Mycelium-specific queries for the OpenAPI operations that back a Carter route.
    /// </summary>
    public static class OpenApiOperationExtensions
    {
        /// <summary>
        /// The order in which HTTP methods are emitted for a single path.
        /// </summary>
        /// <remarks>
        /// The walker visits paths in document order, which the specification does not guarantee across
        /// exports. Emission order is therefore imposed here so that generated output is deterministic.
        /// </remarks>
        private static readonly string[] HttpMethodOrder = ["GET", "POST", "PUT", "DELETE", "PATCH", "HEAD", "OPTIONS", "TRACE"];

        extension(SearchResult searchResult)
        {
            /// <summary>
            /// Queries the deterministic sort key of an operation.
            /// </summary>
            /// <returns>
            /// The zero-based rank of the operation's HTTP method.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the operation is <see langword="null" />.
            /// </exception>
            /// <exception cref="InvalidOperationException">
            /// Thrown when the operation uses an HTTP method that has no defined emission order.
            /// </exception>
            public int QueryHttpMethodRank()
            {
                ArgumentNullException.ThrowIfNull(searchResult);

                var httpMethod = QueryHttpMethod(searchResult).Method;
                var rank = Array.IndexOf(HttpMethodOrder, httpMethod);

                if (rank < 0)
                {
                    throw new InvalidOperationException(
                        $"Operation '{searchResult.Describe()}' uses HTTP method '{httpMethod}', which has no defined emission order.");
                }

                return rank;
            }

            /// <summary>
            /// Queries the path of an operation as it appears in the OpenAPI document.
            /// </summary>
            /// <returns>
            /// The templated path of the operation.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the operation is <see langword="null" />.
            /// </exception>
            /// <exception cref="InvalidOperationException">
            /// Thrown when the operation has no path.
            /// </exception>
            public string QueryPath()
            {
                ArgumentNullException.ThrowIfNull(searchResult);

                var path = searchResult.CurrentKeys?.Path;

                if (string.IsNullOrWhiteSpace(path))
                {
                    throw new InvalidOperationException(
                        $"Operation '{searchResult.Operation?.OperationId}' has no path.");
                }

                return path;
            }

            /// <summary>
            /// Queries the single OpenAPI tag that owns an operation.
            /// </summary>
            /// <returns>
            /// The name of the tag that owns the operation.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the operation is <see langword="null" />.
            /// </exception>
            /// <exception cref="InvalidOperationException">
            /// Thrown when the operation carries no tag, because the generated module for it would have
            /// no name.
            /// </exception>
            public string QueryTag()
            {
                ArgumentNullException.ThrowIfNull(searchResult);

                var tag = searchResult.Operation?.Tags?
                    .Select(operationTag => operationTag.Name)
                    .FirstOrDefault(name => !string.IsNullOrWhiteSpace(name));

                if (string.IsNullOrWhiteSpace(tag))
                {
                    throw new InvalidOperationException(
                        $"Operation '{searchResult.Describe()}' carries no tag, so it cannot be assigned to a Carter module.");
                }

                return tag;
            }

            /// <summary>
            /// Queries the name of the Carter mapping method for an operation.
            /// </summary>
            /// <returns>
            /// The mapping method name without its <c>Map</c> prefix - for example <c>Get</c>.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the operation is <see langword="null" />.
            /// </exception>
            public string QueryHttpMethodName()
            {
                ArgumentNullException.ThrowIfNull(searchResult);

                return QueryHttpMethod(searchResult).Method.ToLowerInvariant().Pascalize();
            }

            /// <summary>
            /// Queries the ASP.NET Core route template of an operation.
            /// </summary>
            /// <returns>
            /// The route template, with a <c>:guid</c> constraint on every path parameter that the
            /// specification types as a UUID.
            /// </returns>
            /// <remarks>
            /// Constraining a UUID placeholder makes a malformed identifier a routing miss rather than a
            /// binding fault deeper in the pipeline. Path parameters of any other type are left
            /// unconstrained; note that the specification types <c>datatypeId</c> as a URI, which cannot
            /// match a single path segment when it contains a slash.
            /// </remarks>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the operation is <see langword="null" />.
            /// </exception>
            public string QueryRouteTemplate()
            {
                ArgumentNullException.ThrowIfNull(searchResult);

                var routeTemplate = searchResult.QueryPath();

                foreach (var parameter in QueryUuidPathParameterNames(searchResult))
                {
                    routeTemplate = routeTemplate.Replace($"{{{parameter}}}", $"{{{parameter}:guid}}", StringComparison.Ordinal);
                }

                return routeTemplate;
            }

            /// <summary>
            /// Queries the name of the hand-written handler that an operation is mapped to.
            /// </summary>
            /// <returns>
            /// The handler name - for example <c>GetProjects</c> for the <c>getProjects</c> operation.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the operation is <see langword="null" />.
            /// </exception>
            /// <exception cref="InvalidOperationException">
            /// Thrown when the operation has no identifier.
            /// </exception>
            /// <exception cref="ArgumentException">
            /// Thrown when the operation identifier cannot be represented as a legal C# identifier.
            /// </exception>
            public string QueryHandlerName()
            {
                ArgumentNullException.ThrowIfNull(searchResult);

                var operationId = searchResult.Operation?.OperationId;

                if (string.IsNullOrWhiteSpace(operationId))
                {
                    throw new InvalidOperationException(
                        $"Operation '{searchResult.Describe()}' has no operationId, so it has no handler name.");
                }

                return ReservedCSharpNameMapper.Map(operationId.Pascalize());
            }

            /// <summary>
            /// Returns a readable description of an operation for use in error messages.
            /// </summary>
            /// <returns>
            /// The HTTP method and path of the operation.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the operation is <see langword="null" />.
            /// </exception>
            public string Describe()
            {
                ArgumentNullException.ThrowIfNull(searchResult);

                return $"{searchResult.CurrentKeys?.Operation?.Method} {searchResult.CurrentKeys?.Path}";
            }
        }

        /// <summary>
        /// Queries the HTTP method of an operation.
        /// </summary>
        /// <param name="searchResult">
        /// The operation whose HTTP method is queried.
        /// </param>
        /// <returns>
        /// The HTTP method of the operation.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the operation has no HTTP method.
        /// </exception>
        private static HttpMethod QueryHttpMethod(SearchResult searchResult)
        {
            return searchResult.CurrentKeys?.Operation
                   ?? throw new InvalidOperationException(
                       $"Operation '{searchResult.Operation?.OperationId}' has no HTTP method.");
        }

        /// <summary>
        /// Queries the names of the path parameters that the specification types as a UUID.
        /// </summary>
        /// <param name="searchResult">
        /// The operation whose path parameters are queried.
        /// </param>
        /// <returns>
        /// The names of the UUID-typed path parameters.
        /// </returns>
        private static IEnumerable<string> QueryUuidPathParameterNames(SearchResult searchResult)
        {
            return (searchResult.Operation?.Parameters ?? [])
                .Where(parameter => parameter.In == ParameterLocation.Path)
                .Where(parameter => string.Equals(parameter.Schema?.Format, "uuid", StringComparison.Ordinal))
                .Select(parameter => parameter.Name)
                .Where(name => !string.IsNullOrWhiteSpace(name));
        }

    }
}
