// ------------------------------------------------------------------------------------------------
//  <copyright file="ProjectLifecycleKindProvider.cs" company="Starion Group S.A.">
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
    /// Provides XMI-literal conversions for <see cref="ProjectLifecycleKind" />.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public static partial class ProjectLifecycleKindProvider
    {
        /// <summary>
        /// Parses an XMI literal using ordinal, case-insensitive matching.
        /// </summary>
        /// <param name="value">
        /// The XMI literal to parse. Letter casing is ignored.
        /// </param>
        /// <returns>
        /// The corresponding <see cref="ProjectLifecycleKind" /> value.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="value" /> does not match a defined XMI literal.
        /// </exception>
        public static ProjectLifecycleKind Parse(ReadOnlySpan<char> value)
        {
            if (value.Equals("Preparation".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                return ProjectLifecycleKind.Preparation;
            }

            if (value.Equals("Open".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                return ProjectLifecycleKind.Open;
            }

            if (value.Equals("Review".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                return ProjectLifecycleKind.Review;
            }

            if (value.Equals("Archived".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                return ProjectLifecycleKind.Archived;
            }

            throw new ArgumentException(
            $"'{new string(value)}' is not a valid ProjectLifecycleKind literal.",
            nameof(value));
        }

        /// <summary>
        /// Tries to parse an XMI literal using ordinal, case-insensitive matching.
        /// </summary>
        /// <param name="value">
        /// The XMI literal to parse. Letter casing is ignored.
        /// </param>
        /// <param name="result">
        /// The parsed value, or the default enumeration value when parsing fails.
        /// </param>
        /// <returns>
        /// <see langword="true" /> when parsing succeeds; otherwise, <see langword="false" />.
        /// </returns>
        public static bool TryParse(ReadOnlySpan<char> value, out ProjectLifecycleKind result)
        {
            if (value.Equals("Preparation".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                result = ProjectLifecycleKind.Preparation;
                return true;
            }

            if (value.Equals("Open".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                result = ProjectLifecycleKind.Open;
                return true;
            }

            if (value.Equals("Review".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                result = ProjectLifecycleKind.Review;
                return true;
            }

            if (value.Equals("Archived".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                result = ProjectLifecycleKind.Archived;
                return true;
            }

            result = default;
            return false;
        }

        /// <summary>
        /// Formats a <see cref="ProjectLifecycleKind" /> value using its exact XMI literal.
        /// </summary>
        /// <param name="value">
        /// The enumeration value to format.
        /// </param>
        /// <returns>
        /// The exact XMI literal spelling and casing.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="value" /> is not a defined enumeration value.
        /// </exception>
        public static string Format(ProjectLifecycleKind value)
        {
            return value switch
            {
                ProjectLifecycleKind.Preparation => "Preparation",
                ProjectLifecycleKind.Open => "Open",
                ProjectLifecycleKind.Review => "Review",
                ProjectLifecycleKind.Archived => "Archived",
                _ => throw new ArgumentOutOfRangeException(
                nameof(value),
                value,
                $"'{value}' is not a defined ProjectLifecycleKind value.")
            };
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
