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
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Mycelium.SDK.DTO;

    /// <summary>
    /// Verifies representative runtime behavior of the generated FunctionalData DTOs.
    /// </summary>
    [TestFixture]
    public class DtoContractTestFixture
    {
        private const string DtoNamespace = "Mycelium.SDK.DTO";

        [Test]
        public void Verify_that_all_DTO_properties_have_public_getters_and_setters()
        {
            var interfaceProperties = QueryDtoInterfaceTypes()
                .SelectMany(
                    interfaceType =>
                        interfaceType
                            .GetProperties(
                                BindingFlags.Public
                                | BindingFlags.Instance
                                | BindingFlags.DeclaredOnly)
                            .Select(
                                property =>
                                    (DeclaringType: interfaceType, Property: property)));

            var concreteProperties = QueryConcreteDtoTypes()
                .SelectMany(
                    concreteType =>
                        concreteType
                            .GetProperties(
                                BindingFlags.Public
                                | BindingFlags.Instance
                                | BindingFlags.DeclaredOnly)
                            .Select(
                                property =>
                                    (DeclaringType: concreteType, Property: property)));

            using (Assert.EnterMultipleScope())
            {
                foreach (var contractProperty in interfaceProperties.Concat(concreteProperties))
                {
                    var displayName =
                        $"{contractProperty.DeclaringType.Name}.{contractProperty.Property.Name}";

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

        [Test]
        public void Verify_that_collection_properties_are_initialized_and_empty()
        {
            var collectionProperties = QueryConcreteDtoTypes()
                .SelectMany(
                    concreteType =>
                        concreteType
                            .GetProperties(
                                BindingFlags.Public
                                | BindingFlags.Instance
                                | BindingFlags.DeclaredOnly)
                            .Where(
                                property =>
                                    IsSupportedCollection(property.PropertyType))
                            .Select(
                                property =>
                                    (DeclaringType: concreteType, Property: property)))
                .OrderBy(
                    item => $"{item.DeclaringType.Name}.{item.Property.Name}",
                    StringComparer.Ordinal);

            using (Assert.EnterMultipleScope())
            {
                foreach (var collectionProperty in collectionProperties)
                {
                    var displayName =
                        $"{collectionProperty.DeclaringType.Name}.{collectionProperty.Property.Name}";

                    var instance =
                        Activator.CreateInstance(collectionProperty.DeclaringType);

                    Assert.That(
                        instance,
                        Is.Not.Null,
                        $"'{collectionProperty.DeclaringType.Name}' could not be constructed.");

                    if (instance is null)
                    {
                        continue;
                    }

                    var value =
                        collectionProperty.Property.GetValue(instance);

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
        public void Verify_that_generated_DTO_types_are_public_and_constructible()
        {
            using (Assert.EnterMultipleScope())
            {
                foreach (var interfaceType in QueryDtoInterfaceTypes())
                {
                    Assert.That(
                        interfaceType.IsPublic,
                        Is.True,
                        $"DTO interface '{interfaceType.Name}' must be public.");
                }

                foreach (var concreteType in QueryConcreteDtoTypes())
                {
                    Assert.That(
                        concreteType.IsPublic,
                        Is.True,
                        $"DTO implementation '{concreteType.Name}' must be public.");

                    Assert.That(
                        concreteType.GetConstructor(Type.EmptyTypes),
                        Is.Not.Null,
                        $"'{concreteType.Name}' must provide a public parameterless constructor.");
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

        private static Type[] QueryConcreteDtoTypes()
        {
            return typeof(IThing).Assembly
                .GetTypes()
                .Where(
                    type => type.Namespace == DtoNamespace
                            && type.IsClass
                            && !type.IsAbstract)
                .OrderBy(type => type.Name, StringComparer.Ordinal)
                .ToArray();
        }

        private static Type[] QueryDtoInterfaceTypes()
        {
            return typeof(IThing).Assembly
                .GetTypes()
                .Where(
                    type => type.Namespace == DtoNamespace
                            && type.IsInterface)
                .OrderBy(type => type.Name, StringComparer.Ordinal)
                .ToArray();
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
        }

        private static bool IsSupportedCollection(Type type)
        {
            if (!type.IsGenericType)
            {
                return false;
            }

            var genericType = type.GetGenericTypeDefinition();

            return genericType == typeof(List<>)
                   || genericType == typeof(Dictionary<,>);
        }
    }
}
