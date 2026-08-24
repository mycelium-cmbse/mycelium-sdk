// ------------------------------------------------------------------------------------------------
//  <copyright file="OpenApiDocumentExtensions.cs" company="Starion Group S.A.">
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

    using Microsoft.OpenApi;

    /// <summary>
    /// Provides Mycelium-specific queries for an OpenAPI document.
    /// </summary>
    public static class OpenApiDocumentExtensions
    {
        extension(OpenApiDocument document)
        {
            /// <summary>
            /// Queries the operations of a document, grouped by the tag that owns them.
            /// </summary>
            /// <returns>
            /// One group per tag, ordered by tag name, each containing the operations of that tag
            /// ordered by path and then by HTTP method.
            /// </returns>
            /// <remarks>
            /// <see cref="OpenApiWalker" /> visits paths in document order, which the specification does
            /// not guarantee across exports. The operations are therefore sorted before they are grouped
            /// - grouping preserves the order of the source sequence within each group - so that the
            /// generated output is byte-for-byte deterministic.
            /// </remarks>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the document is <see langword="null" />.
            /// </exception>
            /// <exception cref="InvalidOperationException">
            /// Thrown when an operation carries no tag, has no path, or uses an HTTP method that has no
            /// defined emission order.
            /// </exception>
            public IReadOnlyList<IGrouping<string, SearchResult>> QueryOperationGroups()
            {
                ArgumentNullException.ThrowIfNull(document);

                var operationSearch = new OperationSearch((_, _, _) => true);

                new OpenApiWalker(operationSearch).Walk(document);

                return operationSearch.SearchResults
                    .OrderBy(searchResult => searchResult.QueryPath(), StringComparer.Ordinal)
                    .ThenBy(searchResult => searchResult.QueryHttpMethodRank())
                    .GroupBy(searchResult => searchResult.QueryTag(), StringComparer.Ordinal)
                    .OrderBy(group => group.Key, StringComparer.Ordinal)
                    .ToArray();
            }
        }
    }
}
