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
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Mycelium.SDK.POCO;

    /// <summary>
    /// Verifies representative POCO inheritance and complete generated implementations.
    /// </summary>
    [TestFixture]
    public class PocoInheritanceTestFixture
    {
        [TestCase(typeof(IAuditableThing), typeof(IThing))]
        [TestCase(typeof(IBranchProtectionRule), typeof(IAuditableThing))]
        public void Verify_that_representative_POCO_inheritance_matches_UML_generalization(
            Type interfaceType,
            Type expectedDirectInterface)
        {
            var actualDirectInterfaces =
                QueryDirectInterfaces(interfaceType);

            Assert.That(
                actualDirectInterfaces,
                Is.EqualTo([expectedDirectInterface]),
                $"Interface '{interfaceType.Name}' has an unexpected direct base interface.");
        }

        [Test]
        public void Verify_that_each_generated_POCO_implements_its_complete_inherited_contract()
        {
            using (Assert.EnterMultipleScope())
            {
                foreach (var concreteType in QueryConcretePocoTypes())
                {
                    var interfaceType = concreteType.Assembly.GetType(
                        $"{concreteType.Namespace}.I{concreteType.Name}");

                    Assert.That(
                        interfaceType,
                        Is.Not.Null,
                        $"No generated interface was found for '{concreteType.Name}'.");

                    if (interfaceType is null)
                    {
                        continue;
                    }

                    Assert.That(
                        interfaceType.IsAssignableFrom(concreteType),
                        Is.True,
                        $"'{concreteType.Name}' does not implement '{interfaceType.Name}'.");

                    var expectedProperties = QueryCompleteInterfaceProperties(interfaceType)
                        .Select(QueryPropertySignature)
                        .OrderBy(signature => signature, StringComparer.Ordinal)
                        .ToArray();

                    var actualProperties = concreteType
                        .GetProperties(
                            BindingFlags.Public
                            | BindingFlags.Instance
                            | BindingFlags.DeclaredOnly)
                        .Select(QueryPropertySignature)
                        .OrderBy(signature => signature, StringComparer.Ordinal)
                        .ToArray();

                    Assert.That(
                        actualProperties,
                        Is.EqualTo(expectedProperties),
                        $"'{concreteType.Name}' does not implement the complete inherited interface contract.");
                }
            }
        }

        private static IEnumerable<PropertyInfo> QueryCompleteInterfaceProperties(
            Type interfaceType)
        {
            return interfaceType
                .GetInterfaces()
                .Append(interfaceType)
                .SelectMany(
                    type =>
                        type.GetProperties(
                            BindingFlags.Public
                            | BindingFlags.Instance
                            | BindingFlags.DeclaredOnly));
        }

        private static Type[] QueryConcretePocoTypes()
        {
            return typeof(IThing).Assembly
                .GetTypes()
                .Where(
                    type => type.Namespace == typeof(IThing).Namespace
                            && type.IsClass
                            && !type.IsAbstract
                            && type.GetCustomAttribute<GeneratedCodeAttribute>() is not null)
                .OrderBy(type => type.Name, StringComparer.Ordinal)
                .ToArray();
        }

        private static Type[] QueryDirectInterfaces(Type interfaceType)
        {
            var allInterfaces = interfaceType.GetInterfaces();

            var indirectInterfaces = allInterfaces
                .SelectMany(candidate => candidate.GetInterfaces())
                .ToHashSet();

            return allInterfaces
                .Where(candidate => !indirectInterfaces.Contains(candidate))
                .OrderBy(candidate => candidate.Name, StringComparer.Ordinal)
                .ToArray();
        }

        private static string QueryPropertySignature(PropertyInfo property)
        {
            return $"{property.Name}:{property.PropertyType}";
        }
    }
}
