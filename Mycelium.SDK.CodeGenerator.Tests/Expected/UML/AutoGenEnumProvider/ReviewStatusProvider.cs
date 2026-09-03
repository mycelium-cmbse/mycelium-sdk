// ------------------------------------------------------------------------------------------------
//  <copyright file="ReviewStatusProvider.cs" company="Starion Group S.A.">
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
    /// Provides XMI-literal conversions for <see cref="ReviewStatus" />.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public static partial class ReviewStatusProvider
    {
        /// <summary>
        /// Parses an XMI literal using ordinal, case-insensitive matching.
        /// </summary>
        /// <param name="value">
        /// The XMI literal to parse. Letter casing is ignored.
        /// </param>
        /// <returns>
        /// The corresponding <see cref="ReviewStatus" /> value.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="value" /> does not match a defined XMI literal.
        /// </exception>
        public static ReviewStatus Parse(ReadOnlySpan<char> value)
        {
            if (value.Equals("Draft".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                return ReviewStatus.Draft;
            }

            if (value.Equals("Ready".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                return ReviewStatus.Ready;
            }

            if (value.Equals("Approved".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                return ReviewStatus.Approved;
            }

            if (value.Equals("ChangesRequested".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                return ReviewStatus.ChangesRequested;
            }

            if (value.Equals("Closed".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                return ReviewStatus.Closed;
            }

            throw new ArgumentException(
            $"'{new string(value)}' is not a valid ReviewStatus literal.",
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
        public static bool TryParse(ReadOnlySpan<char> value, out ReviewStatus result)
        {
            if (value.Equals("Draft".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                result = ReviewStatus.Draft;
                return true;
            }

            if (value.Equals("Ready".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                result = ReviewStatus.Ready;
                return true;
            }

            if (value.Equals("Approved".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                result = ReviewStatus.Approved;
                return true;
            }

            if (value.Equals("ChangesRequested".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                result = ReviewStatus.ChangesRequested;
                return true;
            }

            if (value.Equals("Closed".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                result = ReviewStatus.Closed;
                return true;
            }

            result = default;
            return false;
        }

        /// <summary>
        /// Formats a <see cref="ReviewStatus" /> value using its exact XMI literal.
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
        public static string Format(ReviewStatus value)
        {
            return value switch
            {
                ReviewStatus.Draft => "Draft",
                ReviewStatus.Ready => "Ready",
                ReviewStatus.Approved => "Approved",
                ReviewStatus.ChangesRequested => "ChangesRequested",
                ReviewStatus.Closed => "Closed",
                _ => throw new ArgumentOutOfRangeException(
                nameof(value),
                value,
                $"'{value}' is not a defined ReviewStatus value.")
            };
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
