// ------------------------------------------------------------------------------------------------
//  <copyright file="ProjectMemberTestFixture.cs" company="Starion Group S.A.">
// 
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
// 
//  </copyright>
//  ------------------------------------------------------------------------------------------------
// test
namespace Mycelium.SDK.Tests.Poco
{
    using System;
    using System.Collections.Generic;

    using Mycelium.SDK.POCO;

    /// <summary>
    /// Verifies the handwritten runtime behavior of the
    /// <see cref="ProjectMember" /> POCO.
    /// </summary>
    [TestFixture]
    public class ProjectMemberTestFixture
    {
        [Test]
        public void Verify_that_IsOutsideCollaborator_is_false_when_any_membership_matches_the_owning_organization()
        {
            var owningOrganizationId =
                Guid.Parse("11111111-1111-1111-1111-111111111111");

            var outsideOrganizationId =
                Guid.Parse("22222222-2222-2222-2222-222222222222");

            var user = new User
            {
                IsPartOfOrganizations =
                [
                    new OrganizationMember
                    {
                        Organization = new Organization
                        {
                            Id = outsideOrganizationId
                        }
                    },
                    new OrganizationMember
                    {
                        Organization = new Organization
                        {
                            Id = owningOrganizationId
                        }
                    }
                ]
            };

            var projectMember = new ProjectMember
            {
                User = user,
                IsPartOf = new FunctionalProject
                {
                    BelongsTo = new Organization
                    {
                        Id = owningOrganizationId
                    }
                }
            };

            Assert.That(
                projectMember.IsOutsideCollaborator,
                Is.False);
        }

        [Test]
        public void Verify_that_IsOutsideCollaborator_is_true_when_no_membership_matches_the_owning_organization()
        {
            var user = new User
            {
                IsPartOfOrganizations =
                [
                    new OrganizationMember
                    {
                        Organization = new Organization
                        {
                            Id = Guid.Parse(
                                "11111111-1111-1111-1111-111111111111")
                        }
                    },
                    new OrganizationMember
                    {
                        Organization = new Organization
                        {
                            Id = Guid.Parse(
                                "22222222-2222-2222-2222-222222222222")
                        }
                    }
                ]
            };

            var projectMember = new ProjectMember
            {
                User = user,
                IsPartOf = new FunctionalProject
                {
                    BelongsTo = new Organization
                    {
                        Id = Guid.Parse(
                            "33333333-3333-3333-3333-333333333333")
                    }
                }
            };

            Assert.That(
                projectMember.IsOutsideCollaborator,
                Is.True);
        }

        [Test]
        public void Verify_that_IsOutsideCollaborator_is_true_when_the_user_has_no_organization_memberships()
        {
            var projectMember = new ProjectMember
            {
                User = new User
                {
                    IsPartOfOrganizations = []
                },
                IsPartOf = new FunctionalProject
                {
                    BelongsTo = new Organization
                    {
                        Id = Guid.Parse(
                            "11111111-1111-1111-1111-111111111111")
                    }
                }
            };

            Assert.That(
                projectMember.IsOutsideCollaborator,
                Is.True);
        }

        [TestCaseSource(nameof(ProjectMembersWithMissingRequiredGraphData))]
        public void Verify_that_IsOutsideCollaborator_throws_when_required_graph_data_is_missing(
            ProjectMember projectMember)
        {
            Assert.That(
                () => { _ = projectMember.IsOutsideCollaborator; },
                Throws.TypeOf<InvalidOperationException>());
        }

        private static IEnumerable<TestCaseData> ProjectMembersWithMissingRequiredGraphData()
        {
            yield return new TestCaseData(
                    new ProjectMember
                    {
                        IsPartOf = new FunctionalProject
                        {
                            BelongsTo = new Organization()
                        }
                    })
                .SetName("Missing user");

            yield return new TestCaseData(
                    new ProjectMember
                    {
                        User = new User
                        {
                            IsPartOfOrganizations = []
                        }
                    })
                .SetName("Missing functional project");

            yield return new TestCaseData(
                    new ProjectMember
                    {
                        User = new User
                        {
                            IsPartOfOrganizations = []
                        },
                        IsPartOf = new FunctionalProject()
                    })
                .SetName("Missing owning organization");

            yield return new TestCaseData(
                    new ProjectMember
                    {
                        User = new User
                        {
                            IsPartOfOrganizations = null
                        },
                        IsPartOf = new FunctionalProject
                        {
                            BelongsTo = new Organization()
                        }
                    })
                .SetName("Missing organization-membership collection");
        }
    }
}
