// ------------------------------------------------------------------------------------------------
//  <copyright file="QueryParameter.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.Http
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Holds the query parameters defined by the OpenAPI file of the Functional Data service, and writes them to and
    /// parses them from the query string of an HTTP request
    /// </summary>
    public sealed class QueryParameter
    {
        /// <summary>
        /// The name of the <c>includeReferenced</c> query parameter, as spelled in the OpenAPI file
        /// </summary>
        private const string IncludeReferencedParameterName = "includeReferenced";

        /// <summary>
        /// The name of the <c>includeComposed</c> query parameter, as spelled in the OpenAPI file
        /// </summary>
        private const string IncludeComposedParameterName = "includeComposed";

        /// <summary>
        /// Gets or sets a value indicating whether a read adds every Thing referenced, other than by composition, by a returned Thing
        /// </summary>
        public bool IncludeReferenced { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a read adds every Thing composed, directly or transitively, into a returned Thing
        /// </summary>
        public bool IncludeComposed { get; set; }

        /// <summary>
        /// Parses the query part of an HTTP request into a <see cref="QueryParameter"/>
        /// </summary>
        /// <param name="query">
        /// The query part, with or without a leading <c>?</c>; a full URL is not supported
        /// </param>
        /// <returns>
        /// The <see cref="QueryParameter"/> carrying the parsed values; an absent parameter keeps its default <c>false</c>
        /// and an unknown parameter is ignored
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="query"/> is <c>null</c>
        /// </exception>
        /// <exception cref="FormatException">
        /// Thrown when a known parameter is repeated, has no value or has a value other than <c>true</c> or <c>false</c>
        /// </exception>
        public static QueryParameter FromString(string query)
        {
            var text = (query ?? throw new ArgumentNullException(nameof(query))).Trim();

            if (text.StartsWith('?'))
            {
                text = text.Substring(1);
            }

            var queryParameter = new QueryParameter();

            var readParameterNames = new HashSet<string>(StringComparer.Ordinal);

            foreach (var (name, value) in text.Split(['&'], StringSplitOptions.RemoveEmptyEntries).Select(SplitPair).Where(pair => IsKnownParameterName(pair.Name)))
            {
                if (!readParameterNames.Add(name))
                {
                    throw new FormatException($"The '{name}' query parameter is repeated.");
                }

                if (string.IsNullOrEmpty(value))
                {
                    throw new FormatException($"The '{name}' query parameter has no value.");
                }

                if (!bool.TryParse(value, out var flag))
                {
                    throw new FormatException($"The '{name}' query parameter must be 'true' or 'false', but was '{value}'.");
                }

                if (name == IncludeReferencedParameterName)
                {
                    queryParameter.IncludeReferenced = flag;
                }
                else
                {
                    queryParameter.IncludeComposed = flag;
                }
            }

            return queryParameter;
        }

        /// <summary>
        /// Returns the query string of this <see cref="QueryParameter"/>, writing only the parameters set to <c>true</c>
        /// </summary>
        /// <returns>
        /// The query string with its leading <c>?</c>, for example <c>?includeReferenced=true&amp;includeComposed=true</c>,
        /// or an empty string when every parameter is <c>false</c>
        /// </returns>
        public override string ToString()
        {
            List<string> parameters = [];

            if (this.IncludeReferenced)
            {
                parameters.Add($"{IncludeReferencedParameterName}=true");
            }

            if (this.IncludeComposed)
            {
                parameters.Add($"{IncludeComposedParameterName}=true");
            }

            return parameters.Count == 0 ? string.Empty : $"?{string.Join("&", parameters)}";
        }

        /// <summary>
        /// Splits one <c>name=value</c> pair of a query string at its first <c>=</c> and URL-decodes both parts
        /// </summary>
        /// <param name="pair">
        /// The pair to split
        /// </param>
        /// <returns>
        /// The decoded name, and the decoded value or <c>null</c> when the pair has no <c>=</c>
        /// </returns>
        private static (string Name, string Value) SplitPair(string pair)
        {
            var parts = pair.Split('=', 2);

            return (Uri.UnescapeDataString(parts[0]), parts.Length == 1 ? null : Uri.UnescapeDataString(parts[1]));
        }

        /// <summary>
        /// Determines whether a name is one of the query parameters defined by the OpenAPI file, using ordinal comparison
        /// </summary>
        /// <param name="name">
        /// The decoded parameter name
        /// </param>
        /// <returns>
        /// <c>true</c> when <paramref name="name"/> is a known query parameter; otherwise <c>false</c>
        /// </returns>
        private static bool IsKnownParameterName(string name)
        {
            return name == IncludeReferencedParameterName || name == IncludeComposedParameterName;
        }
    }
}
