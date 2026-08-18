// ------------------------------------------------------------------------------------------------
//  <copyright file="PocoInheritanceTestFixture.cs" company="Starion Group S.A.">
// 
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
// 
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.Tests.Poco
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Mycelium.SDK.POCO;

    /// <summary>
    /// Verifies inheritance and complete implementation of the generated
    /// FunctionalData POCO contracts.
    /// </summary>
    [TestFixture]
    public class PocoInheritanceTestFixture
    {
        private static readonly IReadOnlyDictionary<Type, Type[]> DirectGeneralizations =
            new Dictionary<Type, Type[]>
            {
                [typeof(IThing)] = [],
                [typeof(IAuditableThing)] = [typeof(IThing)],
                [typeof(IBranchProtectionRule)] = [typeof(IAuditableThing)],
                [typeof(IComment)] = [typeof(IAuditableThing)],
                [typeof(IFunctionalProject)] = [typeof(IAuditableThing)],
                [typeof(IFunctionalProjectPolicy)] = [typeof(IAuditableThing)],
                [typeof(IOrganization)] = [typeof(IAuditableThing)],
                [typeof(IOrganizationMember)] = [typeof(IAuditableThing)],
                [typeof(IOrganizationPolicy)] = [typeof(IAuditableThing)],
                [typeof(IOwnership)] = [typeof(IAuditableThing)],
                [typeof(IProjectMember)] = [typeof(IAuditableThing)],
                [typeof(IReview)] = [typeof(IAuditableThing)],
                [typeof(IUser)] = [typeof(IAuditableThing)]
            };

        private static readonly IReadOnlyDictionary<Type, Type> ConcreteImplementations =
            new Dictionary<Type, Type>
            {
                [typeof(IBranchProtectionRule)] = typeof(BranchProtectionRule),
                [typeof(IComment)] = typeof(Comment),
                [typeof(IFunctionalProject)] = typeof(FunctionalProject),
                [typeof(IFunctionalProjectPolicy)] = typeof(FunctionalProjectPolicy),
                [typeof(IOrganization)] = typeof(Organization),
                [typeof(IOrganizationMember)] = typeof(OrganizationMember),
                [typeof(IOrganizationPolicy)] = typeof(OrganizationPolicy),
                [typeof(IOwnership)] = typeof(Ownership),
                [typeof(IProjectMember)] = typeof(ProjectMember),
                [typeof(IReview)] = typeof(Review),
                [typeof(IUser)] = typeof(User)
            };

        [Test]
        public void Verify_that_POCO_interface_inheritance_matches_UML_generalization()
        {
            using (Assert.EnterMultipleScope())
            {
                foreach (var expectedGeneralization in DirectGeneralizations)
                {
                    var actualGeneralizations = QueryDirectInterfaces(expectedGeneralization.Key);

                    var expectedGeneralizations = expectedGeneralization.Value
                        .OrderBy(type => type.Name, StringComparer.Ordinal)
                        .ToArray();

                    Assert.That(
                        actualGeneralizations,
                        Is.EqualTo(expectedGeneralizations),
                        $"Interface '{expectedGeneralization.Key.Name}' has an unexpected direct base interface.");
                }
            }
        }

        [Test]
        public void Verify_that_each_concrete_POCO_implements_its_complete_inherited_contract()
        {
            using (Assert.EnterMultipleScope())
            {
                foreach (var implementation in ConcreteImplementations)
                {
                    var interfaceType = implementation.Key;
                    var concreteType = implementation.Value;

                    Assert.That(
                        interfaceType.IsAssignableFrom(concreteType),
                        Is.True,
                        $"'{concreteType.Name}' does not implement '{interfaceType.Name}'.");

                    var expectedProperties =
                        QueryCompleteInterfaceProperties(interfaceType)
                            .Select(QueryPropertySignature)
                            .OrderBy(
                                signature => signature,
                                StringComparer.Ordinal)
                            .ToArray();

                    var actualProperties = concreteType
                        .GetProperties(
                            BindingFlags.Public
                            | BindingFlags.Instance
                            | BindingFlags.DeclaredOnly)
                        .Select(QueryPropertySignature)
                        .OrderBy(
                            signature => signature,
                            StringComparer.Ordinal)
                        .ToArray();

                    Assert.That(
                        actualProperties,
                        Is.EqualTo(expectedProperties),
                        $"'{concreteType.Name}' does not implement the complete inherited interface contract.");
                }
            }
        }

        private static IEnumerable<PropertyInfo>
            QueryCompleteInterfaceProperties(Type interfaceType)
        {
            return interfaceType
                .GetInterfaces()
                .Append(interfaceType)
                .SelectMany(type =>
                    type.GetProperties(
                        BindingFlags.Public
                        | BindingFlags.Instance
                        | BindingFlags.DeclaredOnly));
        }

        private static Type[] QueryDirectInterfaces(Type interfaceType)
        {
            var allInterfaces = interfaceType.GetInterfaces();
            var indirectInterfaces = allInterfaces
                .SelectMany(candidate => candidate.GetInterfaces())
                .ToHashSet();

            return allInterfaces
                .Where(candidate => !indirectInterfaces.Contains(candidate))
                .OrderBy(
                    candidate => candidate.Name,
                    StringComparer.Ordinal)
                .ToArray();
        }

        private static string QueryPropertySignature(PropertyInfo property)
        {
            return $"{property.Name}:{property.PropertyType}";
        }
    }
}
