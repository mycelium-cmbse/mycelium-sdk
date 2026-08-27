// ------------------------------------------------------------------------------------------------
//  <copyright file="OrganizationMembershipRoleProvider.cs" company="Starion Group S.A.">
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
    /// Provides exact XMI-literal conversions for <see cref="OrganizationMembershipRole" />.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public static partial class OrganizationMembershipRoleProvider
    {
        /// <summary>
        /// Parses an exact XMI literal as a <see cref="OrganizationMembershipRole" /> value.
        /// </summary>
        /// <param name="value">
        /// The case-sensitive XMI literal to parse.
        /// </param>
        /// <returns>
        /// The corresponding <see cref="OrganizationMembershipRole" /> value.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="value" /> is not an exact XMI literal.
        /// </exception>
        public static OrganizationMembershipRole Parse(ReadOnlySpan<char> value)
        {
            if (value.Equals("Administrator".AsSpan(), StringComparison.Ordinal))
            {
                return OrganizationMembershipRole.Administrator;
            }

            if (value.Equals("Member".AsSpan(), StringComparison.Ordinal))
            {
                return OrganizationMembershipRole.Member;
            }

            if (value.Equals("Owner".AsSpan(), StringComparison.Ordinal))
            {
                return OrganizationMembershipRole.Owner;
            }

            throw new ArgumentException(
            $"'{new string(value)}' is not a valid OrganizationMembershipRole literal.",
            nameof(value));
        }

        /// <summary>
        /// Tries to parse an exact XMI literal as a <see cref="OrganizationMembershipRole" /> value.
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
        public static bool TryParse(ReadOnlySpan<char> value, out OrganizationMembershipRole result)
        {
            if (value.Equals("Administrator".AsSpan(), StringComparison.Ordinal))
            {
                result = OrganizationMembershipRole.Administrator;
                return true;
            }

            if (value.Equals("Member".AsSpan(), StringComparison.Ordinal))
            {
                result = OrganizationMembershipRole.Member;
                return true;
            }

            if (value.Equals("Owner".AsSpan(), StringComparison.Ordinal))
            {
                result = OrganizationMembershipRole.Owner;
                return true;
            }

            result = default;
            return false;
        }

        /// <summary>
        /// Formats a <see cref="OrganizationMembershipRole" /> value as its exact XMI literal.
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
        public static string Format(OrganizationMembershipRole value)
        {
            return value switch
            {
                OrganizationMembershipRole.Administrator => "Administrator",
                OrganizationMembershipRole.Member => "Member",
                OrganizationMembershipRole.Owner => "Owner",
                _ => throw new ArgumentOutOfRangeException(
                nameof(value),
                value,
                $"'{value}' is not a defined OrganizationMembershipRole value.")
            };
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
