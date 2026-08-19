// ------------------------------------------------------------------------------------------------
//  <copyright file="ExpectedEnumerations.cs" company="Starion Group S.A.">
// 
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
// 
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Tests.Expected
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Provides the reviewed UML enumeration names and exact ordered literal names.
    /// </summary>
    public class ExpectedEnumerations : IEnumerable<string>
    {
        private static readonly IReadOnlyDictionary<string, string[]> LiteralNamesByEnumeration =
            new Dictionary<string, string[]>(StringComparer.Ordinal)
            {
                ["ActivationStatus"] =
                [
                    "Active",
                    "Pending",
                    "Suspended",
                    "Archived",
                    "Deleted"
                ],
                ["CommentStatus"] =
                [
                    "Open",
                    "Resolved"
                ],
                ["OrganizationMembershipRole"] =
                [
                    "Administrator",
                    "Member",
                    "Owner"
                ],
                ["ProjectLifecycleKind"] =
                [
                    "Preparation",
                    "Open",
                    "Review",
                    "Archived"
                ],
                ["ProjectMemberRole"] =
                [
                    "Administrator",
                    "Participant",
                    "Viewer"
                ],
                ["ProjectMode"] =
                [
                    "Regular",
                    "Concurrent"
                ],
                ["ProjectVisibility"] =
                [
                    "Private",
                    "Organization",
                    "Public"
                ],
                ["ReviewStatus"] =
                [
                    "Draft",
                    "Ready",
                    "Approved",
                    "ChangesRequested",
                    "Closed"
                ]
            };

        /// <summary>
        /// Returns the exact reviewed literal names for an enumeration.
        /// </summary>
        /// <param name="enumerationName">
        /// The reviewed enumeration name.
        /// </param>
        /// <returns>
        /// The literal names in modeled ordinal order.
        /// </returns>
        public static IReadOnlyList<string> QueryLiteralNames(string enumerationName)
        {
            ArgumentException.ThrowIfNullOrEmpty(enumerationName);

            if (LiteralNamesByEnumeration.TryGetValue(enumerationName, out var literalNames))
            {
                return literalNames;
            }

            throw new KeyNotFoundException($"Enumeration '{enumerationName}' is not present in the reviewed inventory.");
        }

        /// <summary>
        /// Returns the expected UML enumeration names in ordinal order.
        /// </summary>
        /// <returns>
        /// The expected UML enumeration names.
        /// </returns>
        public IEnumerator<string> GetEnumerator()
        {
            return LiteralNamesByEnumeration.Keys
                .OrderBy(name => name, StringComparer.Ordinal)
                .GetEnumerator();
        }

        /// <summary>
        /// Returns the expected UML enumeration names.
        /// </summary>
        /// <returns>
        /// The expected UML enumeration names.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
