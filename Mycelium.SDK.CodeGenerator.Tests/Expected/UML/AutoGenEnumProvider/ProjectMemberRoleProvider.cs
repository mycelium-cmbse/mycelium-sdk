// ------------------------------------------------------------------------------------------------
//  <copyright file="ProjectMemberRoleProvider.cs" company="Starion Group S.A.">
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
    /// Provides XMI-literal conversions for <see cref="ProjectMemberRole" />.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public static partial class ProjectMemberRoleProvider
    {
        /// <summary>
        /// Parses an XMI literal using ordinal, case-insensitive matching.
        /// </summary>
        /// <param name="value">
        /// The XMI literal to parse. Letter casing is ignored.
        /// </param>
        /// <returns>
        /// The corresponding <see cref="ProjectMemberRole" /> value.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="value" /> does not match a defined XMI literal.
        /// </exception>
        public static ProjectMemberRole Parse(ReadOnlySpan<char> value)
        {
            if (value.Equals("Administrator".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                return ProjectMemberRole.Administrator;
            }

            if (value.Equals("Participant".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                return ProjectMemberRole.Participant;
            }

            if (value.Equals("Viewer".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                return ProjectMemberRole.Viewer;
            }

            throw new ArgumentException(
            $"'{new string(value)}' is not a valid ProjectMemberRole literal.",
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
        public static bool TryParse(ReadOnlySpan<char> value, out ProjectMemberRole result)
        {
            if (value.Equals("Administrator".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                result = ProjectMemberRole.Administrator;
                return true;
            }

            if (value.Equals("Participant".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                result = ProjectMemberRole.Participant;
                return true;
            }

            if (value.Equals("Viewer".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                result = ProjectMemberRole.Viewer;
                return true;
            }

            result = default;
            return false;
        }

        /// <summary>
        /// Formats a <see cref="ProjectMemberRole" /> value using its exact XMI literal.
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
        public static string Format(ProjectMemberRole value)
        {
            return value switch
            {
                ProjectMemberRole.Administrator => "Administrator",
                ProjectMemberRole.Participant => "Participant",
                ProjectMemberRole.Viewer => "Viewer",
                _ => throw new ArgumentOutOfRangeException(
                nameof(value),
                value,
                $"'{value}' is not a defined ProjectMemberRole value.")
            };
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
