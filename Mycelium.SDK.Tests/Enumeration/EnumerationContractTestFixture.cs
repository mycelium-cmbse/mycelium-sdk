// ------------------------------------------------------------------------------------------------
//  <copyright file="EnumerationContractTestFixture.cs" company="Starion Group S.A.">
// 
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
// 
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.Tests.Enumeration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Mycelium.SDK;

    /// <summary>
    /// Verifies the complete public runtime contract of the generated enumerations.
    /// </summary>
    [TestFixture]
    public class EnumerationContractTestFixture
    {
        private const string EnumerationNamespace = "Mycelium.SDK";

        private static readonly IReadOnlyDictionary<Type, string[]>
            LiteralNamesByEnumeration =
                new Dictionary<Type, string[]>
                {
                    [typeof(ActivationStatus)] =
                    [
                        "Active",
                        "Pending",
                        "Suspended",
                        "Archived",
                        "Deleted"
                    ],
                    [typeof(CommentStatus)] =
                    [
                        "Open",
                        "Resolved"
                    ],
                    [typeof(OrganizationMembershipRole)] =
                    [
                        "Administrator",
                        "Member",
                        "Owner"
                    ],
                    [typeof(ProjectLifecycleKind)] =
                    [
                        "Preparation",
                        "Open",
                        "Review",
                        "Archived"
                    ],
                    [typeof(ProjectMemberRole)] =
                    [
                        "Administrator",
                        "Participant",
                        "Viewer"
                    ],
                    [typeof(ProjectMode)] =
                    [
                        "Regular",
                        "Concurrent"
                    ],
                    [typeof(ProjectVisibility)] =
                    [
                        "Private",
                        "Organization",
                        "Public"
                    ],
                    [typeof(ReviewStatus)] =
                    [
                        "Draft",
                        "Ready",
                        "Approved",
                        "ChangesRequested",
                        "Closed"
                    ]
                };

        [Test]
        public void Verify_that_public_enum_inventory_matches_the_reviewed_contract()
        {
            var expectedTypes = LiteralNamesByEnumeration.Keys
                .OrderBy(type => type.Name, StringComparer.Ordinal)
                .ToArray();

            var actualTypes = typeof(ActivationStatus).Assembly
                .GetExportedTypes()
                .Where(
                    type => type.IsEnum
                            && type.Namespace == EnumerationNamespace)
                .OrderBy(type => type.Name, StringComparer.Ordinal)
                .ToArray();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(expectedTypes, Has.Length.EqualTo(8));
                Assert.That(actualTypes, Is.EqualTo(expectedTypes), "The public enum set contains missing or extra types.");
            }
        }

        [Test]
        public void Verify_that_every_enum_exposes_the_exact_reviewed_literals()
        {
            using (Assert.EnterMultipleScope())
            {
                foreach (var contract in LiteralNamesByEnumeration)
                {
                    var actualLiteralNames = Enum.GetNames(contract.Key);

                    Assert.That(
                        actualLiteralNames,
                        Is.EqualTo(contract.Value),
                        $"Enum '{contract.Key.Name}' does not expose the reviewed literal spelling, casing, and order.");
                }
            }
        }
    }
}
