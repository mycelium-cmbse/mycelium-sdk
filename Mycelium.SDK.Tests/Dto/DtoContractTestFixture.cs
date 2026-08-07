// ------------------------------------------------------------------------------------------------
//  <copyright file="DtoContractTestFixture.cs" company="Starion Group S.A.">
// 
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
// 
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.Tests.Dto
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Mycelium.SDK.DTO;

    /// <summary>
    /// Verifies the public runtime contract of the generated FunctionalData DTOs.
    /// </summary>
    [TestFixture]
    public class DtoContractTestFixture
    {
        private const string DtoNamespace = "Mycelium.SDK.DTO";

        private static readonly Type[] DtoInterfaceTypes =
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

        private static readonly Type[] ConcreteDtoTypes =
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
            "Organization.InvolvedUser",
            "Organization.Projects",
            "ProjectMember.Owns",
            "Review.Comments",
            "Review.Reviewers",
            "User.IsPartOfOrganizations",
            "User.IsPartOfProjects"
        ];

        [Test]
        public void Verify_that_DTO_interface_coverage_matches_the_reviewed_contract()
        {
            var expectedInterfaceTypes = DtoInterfaceTypes
                .OrderBy(type => type.Name, StringComparer.Ordinal)
                .ToArray();

            var actualInterfaceTypes = typeof(IThing).Assembly
                .GetExportedTypes()
                .Where(type => type.Namespace == DtoNamespace && type.IsInterface)
                .OrderBy(type => type.Name, StringComparer.Ordinal)
                .ToArray();

            Assert.Multiple(() =>
            {
                Assert.That(expectedInterfaceTypes, Has.Length.EqualTo(13));
                Assert.That(
                    actualInterfaceTypes,
                    Is.EqualTo(expectedInterfaceTypes),
                    "The public DTO interface set contains missing or extra interfaces.");
            });
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

        [Test]
        public void Verify_that_concrete_DTO_coverage_matches_the_reviewed_contract()
        {
            var expectedConcreteTypes = ConcreteDtoTypes
                .OrderBy(type => type.Name, StringComparer.Ordinal)
                .ToArray();

            var actualConcreteTypes = typeof(IThing).Assembly
                .GetExportedTypes()
                .Where(type => type.Namespace == DtoNamespace && type.IsClass && !type.IsAbstract)
                .OrderBy(type => type.Name, StringComparer.Ordinal)
                .ToArray();

            Assert.Multiple(() =>
            {
                Assert.That(expectedConcreteTypes, Has.Length.EqualTo(11));

                Assert.That(
                    actualConcreteTypes,
                    Is.EqualTo(expectedConcreteTypes),
                    "The public concrete DTO set contains missing or extra classes.");

                foreach (var concreteType in ConcreteDtoTypes)
                {
                    Assert.That(
                        concreteType.GetConstructor(Type.EmptyTypes),
                        Is.Not.Null,
                        $"'{concreteType.Name}' must provide a public parameterless constructor.");
                }
            });
        }

        [Test]
        public void Verify_that_only_derived_properties_are_getter_only()
        {
            var interfaceProperties = DtoInterfaceTypes
                .SelectMany(interfaceType =>
                    interfaceType
                        .GetProperties(
                            BindingFlags.Public
                            | BindingFlags.Instance
                            | BindingFlags.DeclaredOnly)
                        .Select(property => (DeclaringType: interfaceType, Property: property)));

            var concreteProperties = ConcreteDtoTypes
                .SelectMany(concreteType =>
                    concreteType
                        .GetProperties(
                            BindingFlags.Public
                            | BindingFlags.Instance
                            | BindingFlags.DeclaredOnly)
                        .Select(property => (DeclaringType: concreteType, Property: property)));

            Assert.Multiple(() =>
            {
                foreach (var contractProperty in interfaceProperties.Concat(concreteProperties))
                {
                    var displayName = $"{contractProperty.DeclaringType.Name}.{contractProperty.Property.Name}";

                    Assert.That(
                        contractProperty.Property.GetGetMethod(nonPublic: true)?.IsPublic,
                        Is.True,
                        $"'{displayName}' must have a public getter.");

                    var setter = contractProperty.Property.GetSetMethod(nonPublic: true);

                    if (IsDerivedProperty(contractProperty.DeclaringType, contractProperty.Property))
                    {
                        Assert.That(
                            setter,
                            Is.Null,
                            $"Derived property '{displayName}' must be getter-only.");
                    }
                    else
                    {
                        Assert.That(
                            setter?.IsPublic,
                            Is.True,
                            $"Writable property '{displayName}' must have a public setter.");
                    }
                }
            });
        }

        [Test]
        public void Verify_that_collection_properties_are_initialized_and_empty()
        {
            var collectionProperties = ConcreteDtoTypes
                .SelectMany(concreteType =>
                    concreteType
                        .GetProperties(
                            BindingFlags.Public
                            | BindingFlags.Instance
                            | BindingFlags.DeclaredOnly)
                        .Where(property => IsList(property.PropertyType))
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
                "The DTO collection-property set is incomplete or contains unexpected properties.");

            Assert.Multiple(() =>
            {
                foreach (var collectionProperty in collectionProperties)
                {
                    var displayName =
                        $"{collectionProperty.DeclaringType.Name}.{collectionProperty.Property.Name}";

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
                        Is.InstanceOf<System.Collections.ICollection>(),
                        $"Collection property '{displayName}' does not expose a supported collection.");

                    if (value is System.Collections.ICollection collection)
                    {
                        Assert.That(
                            collection.Count,
                            Is.Zero,
                            $"Collection property '{displayName}' must initially be empty.");
                    }
                }
            });
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
                typeof(Guid));

            yield return new TestCaseData(
                typeof(IProjectMember),
                nameof(IProjectMember.ActiveOwnership),
                typeof(Guid?));

            yield return new TestCaseData(
                typeof(IProjectMember),
                nameof(IProjectMember.Owns),
                typeof(List<Guid>));

            yield return new TestCaseData(
                typeof(IBranchProtectionRule),
                nameof(IBranchProtectionRule.MergeAllowedFor),
                typeof(List<ProjectMemberRole>));

            yield return new TestCaseData(
                typeof(IProjectMember),
                nameof(IProjectMember.IsOutsideCollaborator),
                typeof(bool));
        }

        private static bool IsList(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
        }

        private static bool IsDerivedProperty(
            Type declaringType,
            PropertyInfo property)
        {
            return property.Name == nameof(IProjectMember.IsOutsideCollaborator)
                   && (declaringType == typeof(IProjectMember)
                       || declaringType == typeof(ProjectMember));
        }
    }
}
