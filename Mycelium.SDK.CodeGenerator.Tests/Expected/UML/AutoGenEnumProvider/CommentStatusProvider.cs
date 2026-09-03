// ------------------------------------------------------------------------------------------------
//  <copyright file="CommentStatusProvider.cs" company="Starion Group S.A.">
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
    /// Provides XMI-literal conversions for <see cref="CommentStatus" />.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public static partial class CommentStatusProvider
    {
        /// <summary>
        /// Parses an XMI literal using ordinal, case-insensitive matching.
        /// </summary>
        /// <param name="value">
        /// The XMI literal to parse. Letter casing is ignored.
        /// </param>
        /// <returns>
        /// The corresponding <see cref="CommentStatus" /> value.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="value" /> does not match a defined XMI literal.
        /// </exception>
        public static CommentStatus Parse(ReadOnlySpan<char> value)
        {
            if (value.Equals("Open".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                return CommentStatus.Open;
            }

            if (value.Equals("Resolved".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                return CommentStatus.Resolved;
            }

            throw new ArgumentException(
            $"'{new string(value)}' is not a valid CommentStatus literal.",
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
        public static bool TryParse(ReadOnlySpan<char> value, out CommentStatus result)
        {
            if (value.Equals("Open".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                result = CommentStatus.Open;
                return true;
            }

            if (value.Equals("Resolved".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                result = CommentStatus.Resolved;
                return true;
            }

            result = default;
            return false;
        }

        /// <summary>
        /// Formats a <see cref="CommentStatus" /> value using its exact XMI literal.
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
        public static string Format(CommentStatus value)
        {
            return value switch
            {
                CommentStatus.Open => "Open",
                CommentStatus.Resolved => "Resolved",
                _ => throw new ArgumentOutOfRangeException(
                nameof(value),
                value,
                $"'{value}' is not a defined CommentStatus value.")
            };
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
