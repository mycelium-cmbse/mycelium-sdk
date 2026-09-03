// ------------------------------------------------------------------------------------------------
//  <copyright file="ActivationStatusProvider.cs" company="Starion Group S.A.">
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
    /// Provides XMI-literal conversions for <see cref="ActivationStatus" />.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public static partial class ActivationStatusProvider
    {
        /// <summary>
        /// Parses an XMI literal using ordinal, case-insensitive matching.
        /// </summary>
        /// <param name="value">
        /// The XMI literal to parse. Letter casing is ignored.
        /// </param>
        /// <returns>
        /// The corresponding <see cref="ActivationStatus" /> value.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="value" /> does not match a defined XMI literal.
        /// </exception>
        public static ActivationStatus Parse(ReadOnlySpan<char> value)
        {
            if (value.Equals("Active".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                return ActivationStatus.Active;
            }

            if (value.Equals("Pending".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                return ActivationStatus.Pending;
            }

            if (value.Equals("Suspended".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                return ActivationStatus.Suspended;
            }

            if (value.Equals("Archived".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                return ActivationStatus.Archived;
            }

            if (value.Equals("Deleted".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                return ActivationStatus.Deleted;
            }

            throw new ArgumentException(
            $"'{new string(value)}' is not a valid ActivationStatus literal.",
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
        public static bool TryParse(ReadOnlySpan<char> value, out ActivationStatus result)
        {
            if (value.Equals("Active".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                result = ActivationStatus.Active;
                return true;
            }

            if (value.Equals("Pending".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                result = ActivationStatus.Pending;
                return true;
            }

            if (value.Equals("Suspended".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                result = ActivationStatus.Suspended;
                return true;
            }

            if (value.Equals("Archived".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                result = ActivationStatus.Archived;
                return true;
            }

            if (value.Equals("Deleted".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                result = ActivationStatus.Deleted;
                return true;
            }

            result = default;
            return false;
        }

        /// <summary>
        /// Formats a <see cref="ActivationStatus" /> value using its exact XMI literal.
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
        public static string Format(ActivationStatus value)
        {
            return value switch
            {
                ActivationStatus.Active => "Active",
                ActivationStatus.Pending => "Pending",
                ActivationStatus.Suspended => "Suspended",
                ActivationStatus.Archived => "Archived",
                ActivationStatus.Deleted => "Deleted",
                _ => throw new ArgumentOutOfRangeException(
                nameof(value),
                value,
                $"'{value}' is not a defined ActivationStatus value.")
            };
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
