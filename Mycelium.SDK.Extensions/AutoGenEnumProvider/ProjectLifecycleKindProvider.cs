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
    /// Provides exact XMI-literal conversions for <see cref="ProjectLifecycleKind" />.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public static partial class ProjectLifecycleKindProvider
    {
        /// <summary>
        /// Parses an exact XMI literal as a <see cref="ProjectLifecycleKind" /> value.
        /// </summary>
        /// <param name="value">
        /// The case-sensitive XMI literal to parse.
        /// </param>
        /// <returns>
        /// The corresponding <see cref="ProjectLifecycleKind" /> value.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="value" /> is not an exact XMI literal.
        /// </exception>
        public static ProjectLifecycleKind Parse(ReadOnlySpan<char> value)
        {
            if (value.Equals("Preparation".AsSpan(), StringComparison.Ordinal))
            {
                return ProjectLifecycleKind.Preparation;
            }

            if (value.Equals("Open".AsSpan(), StringComparison.Ordinal))
            {
                return ProjectLifecycleKind.Open;
            }

            if (value.Equals("Review".AsSpan(), StringComparison.Ordinal))
            {
                return ProjectLifecycleKind.Review;
            }

            if (value.Equals("Archived".AsSpan(), StringComparison.Ordinal))
            {
                return ProjectLifecycleKind.Archived;
            }

            throw new ArgumentException(
            $"'{new string(value)}' is not a valid ProjectLifecycleKind literal.",
            nameof(value));
        }

        /// <summary>
        /// Tries to parse an exact XMI literal as a <see cref="ProjectLifecycleKind" /> value.
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
        public static bool TryParse(ReadOnlySpan<char> value, out ProjectLifecycleKind result)
        {
            if (value.Equals("Preparation".AsSpan(), StringComparison.Ordinal))
            {
                result = ProjectLifecycleKind.Preparation;
                return true;
            }

            if (value.Equals("Open".AsSpan(), StringComparison.Ordinal))
            {
                result = ProjectLifecycleKind.Open;
                return true;
            }

            if (value.Equals("Review".AsSpan(), StringComparison.Ordinal))
            {
                result = ProjectLifecycleKind.Review;
                return true;
            }

            if (value.Equals("Archived".AsSpan(), StringComparison.Ordinal))
            {
                result = ProjectLifecycleKind.Archived;
                return true;
            }

            result = default;
            return false;
        }

        /// <summary>
        /// Formats a <see cref="ProjectLifecycleKind" /> value as its exact XMI literal.
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