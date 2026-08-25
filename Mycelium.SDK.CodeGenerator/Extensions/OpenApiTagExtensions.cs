// ------------------------------------------------------------------------------------------------
//  <copyright file="OpenApiTagExtensions.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Extensions
{
    using System;
    using System.Linq;

    using Humanizer;

    /// <summary>
    /// Provides Mycelium-specific queries for OpenAPI tags.
    /// </summary>
    public static class OpenApiTagExtensions
    {
        /// <summary>
        /// The suffix appended to every generated Carter module name.
        /// </summary>
        private const string ModuleNameSuffix = "Api";

        extension(string tag)
        {
            /// <summary>
            /// Queries the type name of the Carter module that carries the routes of an OpenAPI tag.
            /// </summary>
            /// <returns>
            /// The type name of the Carter module - for example <c>DiffMergeApi</c> for the
            /// <c>Diff &amp; Merge</c> tag.
            /// </returns>
            /// <remarks>
            /// The name is derived rather than looked up in a table, so a tag added or renamed by a
            /// future specification export needs no change here. Characters that cannot appear in a C#
            /// identifier are replaced by a space, <see cref="InflectorExtensions.Pascalize" /> then
            /// removes the whitespace and capitalizes each word, and
            /// <see cref="ReservedCSharpNameMapper.Map" /> remains the final gate on the result.
            /// </remarks>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the tag is <see langword="null" />.
            /// </exception>
            /// <exception cref="ArgumentException">
            /// Thrown when the tag is empty, or when the derived name cannot be represented as a legal
            /// C# identifier.
            /// </exception>
            public string QueryModuleName()
            {
                ArgumentException.ThrowIfNullOrEmpty(tag);

                var identifierCharacters = tag
                    .Select(character => char.IsLetterOrDigit(character) ? character : ' ')
                    .ToArray();

                var moduleName = $"{new string(identifierCharacters).Pascalize()}{ModuleNameSuffix}";

                return ReservedCSharpNameMapper.Map(moduleName);
            }
        }
    }
}
