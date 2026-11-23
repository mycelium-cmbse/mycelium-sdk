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
    using System.Collections.Specialized;
    using System.Linq;
    using System.Web;

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
        /// and an unknown parameter is ignored; names are matched ignoring case
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="query"/> is <c>null</c>
        /// </exception>
        /// <exception cref="FormatException">
        /// Thrown when a known parameter is repeated, has no value or has a value other than <c>true</c> or <c>false</c>,
        /// compared ignoring case and surrounding white space
        /// </exception>
        public static QueryParameter FromString(string query)
        {
            var parameters = HttpUtility.ParseQueryString((query ?? throw new ArgumentNullException(nameof(query))).Trim());

            return new QueryParameter
            {
                IncludeReferenced = ReadFlag(parameters, IncludeReferencedParameterName),
                IncludeComposed = ReadFlag(parameters, IncludeComposedParameterName)
            };
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
        /// Reads one boolean query parameter from the parsed query string
        /// </summary>
        /// <param name="parameters">
        /// The query string parsed by <see cref="HttpUtility.ParseQueryString(string)"/>, which files a name written
        /// without <c>=</c> as a value under the <c>null</c> key
        /// </param>
        /// <param name="name">
        /// The name of the query parameter to read
        /// </param>
        /// <returns>
        /// The parsed value, or <c>false</c> when the parameter is absent
        /// </returns>
        /// <exception cref="FormatException">
        /// Thrown when the parameter is repeated, has no value or has a value other than <c>true</c> or <c>false</c>
        /// </exception>
        private static bool ReadFlag(NameValueCollection parameters, string name)
        {
            var values = parameters.GetValues(name) ?? [];

            var valuelessCount = (parameters.GetValues(null) ?? []).Count(valueless => string.Equals(valueless, name, StringComparison.OrdinalIgnoreCase));

            if (values.Length + valuelessCount > 1)
            {
                throw new FormatException($"The '{name}' query parameter is repeated.");
            }

            if (valuelessCount == 1 || (values.Length == 1 && string.IsNullOrEmpty(values[0])))
            {
                throw new FormatException($"The '{name}' query parameter has no value.");
            }

            if (values.Length == 0)
            {
                return false;
            }

            if (!bool.TryParse(values[0], out var flag))
            {
                throw new FormatException($"The '{name}' query parameter must be 'true' or 'false' (case-insensitive), but was '{values[0]}'.");
            }

            return flag;
        }
    }
}
