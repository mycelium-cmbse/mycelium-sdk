// ------------------------------------------------------------------------------------------------
//  <copyright file="PocoContractTestFixture.cs" company="Starion Group S.A.">
// 
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
// 
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.Tests.Poco
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Mycelium.SDK.POCO;

    /// <summary>
    /// Verifies the public runtime contract of the generated FunctionalData POCOs.
    /// </summary>
    [TestFixture]
    public class PocoContractTestFixture
    {
        private const string PocoNamespace = "Mycelium.SDK.POCO";

        private static readonly Type[] PocoInterfaceTypes =
        [
            typeof(IThing),
            typeof(IAuditableThing),
            typeof(IBranchProtectionRule),
            typeof(IComment),
            typeof(IFunctionalProject),
            typeof(IFunctionalProjectPolicy),
            typeof(IOrganization),
            typeof(IOrganizationMember),
            typeof(IOrganizationPolicy),
            typeof(IOwnership),
            typeof(IProjectMember),
            typeof(IReview),
            typeof(IUser)
        ];

        private static readonly Type[] ConcretePocoTypes =
        [
            typeof(BranchProtectionRule),
            typeof(Comment),
            typeof(FunctionalProject),
            typeof(FunctionalProjectPolicy),
            typeof(Organization),
            typeof(OrganizationMember),
            typeof(OrganizationPolicy),
            typeof(Ownership),
            typeof(ProjectMember),
            typeof(Review),
            typeof(User)
        ];

        private static readonly string[] InitializedCollectionProperties =
        [
            "BranchProtectionRule.DefaultReviewers",
            "BranchProtectionRule.MergeAllowedFor",
            "Comment.Replies",
            "FunctionalProject.BranchRules",
            "FunctionalProject.Defines",
            "FunctionalProject.Involves",
            "FunctionalProject.Reviews",
            "FunctionalProject.SharedPreferences",
            "Organization.InvolvedUser",
            "Organization.Projects",
            "ProjectMember.Owns",
            "Review.Comments",
            "Review.Reviewers",
            "User.IsPartOfOrganizations",
            "User.IsPartOfProjects",
            "User.UserPreferences"
        ];

        [Test]
        public void Verify_that_IsOutsideCollaborator_is_false_for_a_member_of_the_owning_organization()
        {
            var organizationId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var user = new User();

            user.IsPartOfOrganizations.Add(new OrganizationMember
            {
                Organization = new Organization
                {
                    Id = organizationId
                }
            });

            var projectMember = new ProjectMember
            {
                User = user,
                IsPartOf = new FunctionalProject
                {
                    BelongsTo = new Organization
                    {
                        Id = organizationId
                    }
                }
            };

            Assert.That(projectMember.IsOutsideCollaborator, Is.False);
        }

        [Test]
        public void Verify_that_IsOutsideCollaborator_is_getter_only()
        {
            var interfaceProperty = typeof(IProjectMember).GetProperty(
                nameof(IProjectMember.IsOutsideCollaborator),
                BindingFlags.Public
                | BindingFlags.Instance
                | BindingFlags.DeclaredOnly);

            var implementationProperty = typeof(ProjectMember).GetProperty(
                nameof(ProjectMember.IsOutsideCollaborator),
                BindingFlags.Public
                | BindingFlags.Instance
                | BindingFlags.DeclaredOnly);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(
                    interfaceProperty,
                    Is.Not.Null,
                    "'IProjectMember.IsOutsideCollaborator' was not found.");

                Assert.That(
                    interfaceProperty!.PropertyType,
                    Is.EqualTo(typeof(bool)));

                Assert.That(
                    interfaceProperty.GetGetMethod(true)?.IsPublic,
                    Is.True,
                    "'IProjectMember.IsOutsideCollaborator' must have a public getter.");

                Assert.That(
                    interfaceProperty.GetSetMethod(true),
                    Is.Null,
                    "'IProjectMember.IsOutsideCollaborator' must not have a setter.");

                Assert.That(
                    implementationProperty,
                    Is.Not.Null,
                    "'ProjectMember.IsOutsideCollaborator' was not found.");

                Assert.That(
                    implementationProperty!.PropertyType,
                    Is.EqualTo(typeof(bool)));

                Assert.That(
                    implementationProperty.GetGetMethod(true)?.IsPublic,
                    Is.True,
                    "'ProjectMember.IsOutsideCollaborator' must have a public getter.");

                Assert.That(
                    implementationProperty.GetSetMethod(true),
                    Is.Null,
                    "'ProjectMember.IsOutsideCollaborator' must not have a setter.");
            }
        }

        [Test]
        public void Verify_that_IsOutsideCollaborator_is_true_for_a_member_outside_the_owning_organization()
        {
            var user = new User();

            user.IsPartOfOrganizations.Add(new OrganizationMember
            {
                Organization = new Organization
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111")
                }
            });

            var projectMember = new ProjectMember
            {
                User = user,
                IsPartOf = new FunctionalProject
                {
                    BelongsTo = new Organization
                    {
                        Id = Guid.Parse("22222222-2222-2222-2222-222222222222")
                    }
                }
            };

            Assert.That(projectMember.IsOutsideCollaborator, Is.True);
        }

        [Test]
        public void Verify_that_POCO_interface_coverage_matches_the_reviewed_contract()
        {
            var expectedInterfaceTypes = PocoInterfaceTypes
                .OrderBy(type => type.Name, StringComparer.Ordinal)
                .ToArray();

            var actualInterfaceTypes = typeof(IThing).Assembly
                .GetExportedTypes()
                .Where(type => type.Namespace == PocoNamespace && type.IsInterface)
                .OrderBy(type => type.Name, StringComparer.Ordinal)
                .ToArray();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(expectedInterfaceTypes, Has.Length.EqualTo(13));

                Assert.That(
                    actualInterfaceTypes,
                    Is.EqualTo(expectedInterfaceTypes),
                    "The public POCO interface set contains missing or extra interfaces.");
            }
        }

        [Test]
        public void Verify_that_abstract_UML_classes_have_no_POCO_implementation()
        {
            var assembly = typeof(IThing).Assembly;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(
                    assembly.GetType($"{PocoNamespace}.Thing"),
                    Is.Null,
                    "Abstract UML class 'Thing' must not have a POCO implementation.");

                Assert.That(
                    assembly.GetType($"{PocoNamespace}.AuditableThing"),
                    Is.Null,
                    "Abstract UML class 'AuditableThing' must not have a POCO implementation.");
            }
        }

        [Test]
        public void Verify_that_collection_properties_are_initialized_and_empty()
        {
            var collectionProperties = ConcretePocoTypes
                .SelectMany(concreteType =>
                    concreteType
                        .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                        .Where(property => IsSupportedCollection(property.PropertyType))
                        .Select(property => (DeclaringType: concreteType, Property: property)))
                .OrderBy(
                    item => $"{item.DeclaringType.Name}.{item.Property.Name}",
                    StringComparer.Ordinal)
                .ToArray();

            var actualCollectionPropertyNames = collectionProperties
                .Select(item => $"{item.DeclaringType.Name}.{item.Property.Name}")
                .ToArray();

            Assert.That(
                actualCollectionPropertyNames,
                Is.EqualTo(InitializedCollectionProperties),
                "The POCO collection-property set is incomplete or contains unexpected properties.");

            using (Assert.EnterMultipleScope())
            {
                foreach (var collectionProperty in collectionProperties)
                {
                    var displayName = $"{collectionProperty.DeclaringType.Name}.{collectionProperty.Property.Name}";
                    var instance = Activator.CreateInstance(collectionProperty.DeclaringType);

                    Assert.That(
                        instance,
                        Is.Not.Null,
                        $"'{collectionProperty.DeclaringType.Name}' could not be constructed.");

                    if (instance is null)
                    {
                        continue;
                    }

                    var value = collectionProperty.Property.GetValue(instance);

                    Assert.That(
                        value,
                        Is.Not.Null,
                        $"Collection property '{displayName}' was not initialized.");

                    Assert.That(
                        value,
                        Is.InstanceOf<ICollection>(),
                        $"Collection property '{displayName}' does not expose a supported collection.");

                    if (value is ICollection collection)
                    {
                        Assert.That(
                            collection.Count,
                            Is.Zero,
                            $"Collection property '{displayName}' must initially be empty.");
                    }
                }
            }
        }

        [Test]
        public void Verify_that_concrete_POCO_coverage_matches_the_reviewed_contract()
        {
            var expectedConcreteTypes = ConcretePocoTypes
                .OrderBy(type => type.Name, StringComparer.Ordinal)
                .ToArray();

            var actualConcreteTypes = typeof(IThing).Assembly
                .GetExportedTypes()
                .Where(type => type.Namespace == PocoNamespace && type.IsClass && !type.IsAbstract)
                .OrderBy(type => type.Name, StringComparer.Ordinal)
                .ToArray();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(expectedConcreteTypes, Has.Length.EqualTo(11));

                Assert.That(
                    actualConcreteTypes,
                    Is.EqualTo(expectedConcreteTypes),
                    "The public concrete POCO set contains missing or extra classes.");

                foreach (var concreteType in ConcretePocoTypes)
                {
                    Assert.That(
                        concreteType.GetConstructor(Type.EmptyTypes),
                        Is.Not.Null,
                        $"'{concreteType.Name}' must provide a public parameterless constructor.");
                }
            }
        }

        [Test]
        public void Verify_that_non_derived_POCO_properties_have_public_getters_and_setters()
        {
            var interfaceProperties = PocoInterfaceTypes
                .SelectMany(interfaceType =>
                    interfaceType
                        .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                        .Select(property => (DeclaringType: interfaceType, Property: property)));

            var concreteProperties = ConcretePocoTypes
                .SelectMany(concreteType =>
                    concreteType
                        .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                        .Select(property => (DeclaringType: concreteType, Property: property)));

            var nonDerivedProperties = interfaceProperties
                .Concat(concreteProperties)
                .Where(contractProperty => contractProperty.Property.Name != nameof(IProjectMember.IsOutsideCollaborator));

            using (Assert.EnterMultipleScope())
            {
                foreach (var contractProperty in nonDerivedProperties)
                {
                    var displayName = $"{contractProperty.DeclaringType.Name}.{contractProperty.Property.Name}";

                    Assert.That(
                        contractProperty.Property.GetGetMethod(true)?.IsPublic,
                        Is.True,
                        $"'{displayName}' must have a public getter.");

                    Assert.That(
                        contractProperty.Property.GetSetMethod(true)?.IsPublic,
                        Is.True,
                        $"'{displayName}' must have a public setter.");
                }
            }
        }

        [TestCaseSource(nameof(RepresentativeInterfacePropertyContracts))]
        public void Verify_that_representative_interface_property_shape_matches_the_contract(
            Type interfaceType,
            string propertyName,
            Type expectedPropertyType)
        {
            var property = interfaceType.GetProperty(
                propertyName,
                BindingFlags.Public
                | BindingFlags.Instance
                | BindingFlags.DeclaredOnly);

            Assert.That(
                property,
                Is.Not.Null,
                $"Interface '{interfaceType.Name}' does not declare property '{propertyName}'.");

            Assert.That(
                property!.PropertyType,
                Is.EqualTo(expectedPropertyType),
                $"Property '{interfaceType.Name}.{propertyName}' has an unexpected type.");
        }

        [TestCaseSource(nameof(ProjectMembersWithMissingRequiredGraphData))]
        public void Verify_that_IsOutsideCollaborator_throws_when_required_graph_data_is_missing(ProjectMember projectMember)
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
                        User = new User()
                    })
                .SetName("Missing functional project");

            yield return new TestCaseData(
                    new ProjectMember
                    {
                        User = new User(),
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
                .SetName("Missing organization memberships");
        }

        private static IEnumerable<TestCaseData> RepresentativeInterfacePropertyContracts()
        {
            yield return new TestCaseData(
                typeof(IThing),
                nameof(IThing.Id),
                typeof(Guid));

            yield return new TestCaseData(
                typeof(IAuditableThing),
                nameof(IAuditableThing.CreatedOn),
                typeof(DateTime));

            yield return new TestCaseData(
                typeof(IBranchProtectionRule),
                nameof(IBranchProtectionRule.MinimumRequiredApproval),
                typeof(int));

            yield return new TestCaseData(
                typeof(IBranchProtectionRule),
                nameof(IBranchProtectionRule.ReviewRequired),
                typeof(bool));

            yield return new TestCaseData(
                typeof(IComment),
                nameof(IComment.Content),
                typeof(string));

            yield return new TestCaseData(
                typeof(IFunctionalProject),
                nameof(IFunctionalProject.CurrentMode),
                typeof(ProjectMode));

            yield return new TestCaseData(
                typeof(IFunctionalProject),
                nameof(IFunctionalProject.BelongsTo),
                typeof(IOrganization));

            yield return new TestCaseData(
                typeof(IProjectMember),
                nameof(IProjectMember.ActiveOwnership),
                typeof(IOwnership));

            yield return new TestCaseData(
                typeof(IProjectMember),
                nameof(IProjectMember.Owns),
                typeof(List<IOwnership>));

            yield return new TestCaseData(
                typeof(IBranchProtectionRule),
                nameof(IBranchProtectionRule.DefaultReviewers),
                typeof(List<IUser>));

            yield return new TestCaseData(
                typeof(IBranchProtectionRule),
                nameof(IBranchProtectionRule.MergeAllowedFor),
                typeof(List<ProjectMemberRole>));
        }

        private static bool IsSupportedCollection(Type type)
        {
            if (!type.IsGenericType)
            {
                return false;
            }

            var genericType = type.GetGenericTypeDefinition();

            return genericType == typeof(List<>) || genericType == typeof(Dictionary<,>);
        }
    }
}
