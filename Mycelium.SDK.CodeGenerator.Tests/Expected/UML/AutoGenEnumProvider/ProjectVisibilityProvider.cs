// ------------------------------------------------------------------------------------------------
//  <copyright file="ProjectVisibilityProvider.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.Extensions
{
    using System;
    using System.CodeDom.Compiler;

    using Mycelium.SDK;

    /// <summary>
    /// Provides exact XMI-literal conversions for <see cref="ProjectVisibility" />.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public static partial class ProjectVisibilityProvider
    {
        /// <summary>
        /// Parses an exact XMI literal as a <see cref="ProjectVisibility" /> value.
        /// </summary>
        /// <param name="value">
        /// The case-sensitive XMI literal to parse.
        /// </param>
        /// <returns>
        /// The corresponding <see cref="ProjectVisibility" /> value.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="value" /> is not an exact XMI literal.
        /// </exception>
        public static ProjectVisibility Parse(ReadOnlySpan<char> value)
        {
            if (value.Equals("Private".AsSpan(), StringComparison.Ordinal))
            {
                return ProjectVisibility.Private;
            }

            if (value.Equals("Organization".AsSpan(), StringComparison.Ordinal))
            {
                return ProjectVisibility.Organization;
            }

            if (value.Equals("Public".AsSpan(), StringComparison.Ordinal))
            {
                return ProjectVisibility.Public;
            }

            throw new ArgumentException(
            $"'{new string(value)}' is not a valid ProjectVisibility literal.",
            nameof(value));
        }

        /// <summary>
        /// Tries to parse an exact XMI literal as a <see cref="ProjectVisibility" /> value.
        /// </summary>
        /// <param name="value">
        /// The case-sensitive XMI literal to parse.
        /// </param>
        /// <param name="result">
        /// The parsed value, or the default enumeration value when parsing fails.
        /// </param>
        /// <returns>
        /// <see langword="true" /> when parsing succeeds; otherwise, <see langword="false" />.
        /// </returns>
        public static bool TryParse(ReadOnlySpan<char> value, out ProjectVisibility result)
        {
            if (value.Equals("Private".AsSpan(), StringComparison.Ordinal))
            {
                result = ProjectVisibility.Private;
                return true;
            }

            if (value.Equals("Organization".AsSpan(), StringComparison.Ordinal))
            {
                result = ProjectVisibility.Organization;
                return true;
            }

            if (value.Equals("Public".AsSpan(), StringComparison.Ordinal))
            {
                result = ProjectVisibility.Public;
                return true;
            }

            result = default;
            return false;
        }

        /// <summary>
        /// Formats a <see cref="ProjectVisibility" /> value as its exact XMI literal.
        /// </summary>
        /// <param name="value">
        /// The enumeration value to format.
        /// </param>
        /// <returns>
        /// The exact case-sensitive XMI literal.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="value" /> is not a defined enumeration value.
        /// </exception>
        public static string Format(ProjectVisibility value)
        {
            return value switch
            {
                ProjectVisibility.Private => "Private",
                ProjectVisibility.Organization => "Organization",
                ProjectVisibility.Public => "Public",
                _ => throw new ArgumentOutOfRangeException(
                nameof(value),
                value,
                $"'{value}' is not a defined ProjectVisibility value.")
            };
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
