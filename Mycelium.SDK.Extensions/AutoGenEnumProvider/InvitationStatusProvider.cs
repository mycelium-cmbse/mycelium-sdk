// ------------------------------------------------------------------------------------------------
//  <copyright file="InvitationStatusProvider.cs" company="Starion Group S.A.">
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
    /// Provides XMI-literal conversions for <see cref="InvitationStatus" />.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public static partial class InvitationStatusProvider
    {
        /// <summary>
        /// Parses an XMI literal using ordinal, case-insensitive matching.
        /// </summary>
        /// <param name="value">
        /// The XMI literal to parse. Letter casing is ignored.
        /// </param>
        /// <returns>
        /// The corresponding <see cref="InvitationStatus" /> value.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="value" /> does not match a defined XMI literal.
        /// </exception>
        public static InvitationStatus Parse(ReadOnlySpan<char> value)
        {
            if (value.Equals("Pending".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                return InvitationStatus.Pending;
            }

            if (value.Equals("Accepted".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                return InvitationStatus.Accepted;
            }

            if (value.Equals("Declined".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                return InvitationStatus.Declined;
            }

            if (value.Equals("Revoked".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                return InvitationStatus.Revoked;
            }

            throw new ArgumentException($"'{new string(value)}' is not a valid InvitationStatus literal.", nameof(value));
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
        public static bool TryParse(ReadOnlySpan<char> value, out InvitationStatus result)
        {
            if (value.Equals("Pending".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                result = InvitationStatus.Pending;
                return true;
            }

            if (value.Equals("Accepted".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                result = InvitationStatus.Accepted;
                return true;
            }

            if (value.Equals("Declined".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                result = InvitationStatus.Declined;
                return true;
            }

            if (value.Equals("Revoked".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                result = InvitationStatus.Revoked;
                return true;
            }

            result = default;
            return false;
        }

        /// <summary>
        /// Formats a <see cref="InvitationStatus" /> value using its exact XMI literal.
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
        public static string Format(InvitationStatus value)
        {
            return value switch
            {
                InvitationStatus.Pending => "Pending",
                InvitationStatus.Accepted => "Accepted",
                InvitationStatus.Declined => "Declined",
                InvitationStatus.Revoked => "Revoked",
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, $"'{value}' is not a defined InvitationStatus value.")
            };
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
