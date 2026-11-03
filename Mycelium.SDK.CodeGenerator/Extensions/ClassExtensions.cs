// ------------------------------------------------------------------------------------------------
//  <copyright file="ClassExtensions.cs" company="Starion Group S.A.">
// 
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
// 
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Extensions
{
    using uml4net.Classification;
    using uml4net.Extensions;
    using uml4net.StructuredClassifiers;

    /// <summary>
    /// Provides artifact-specific queries for UML classes.
    /// </summary>
    public static class ClassExtensions
    {
        /// <summary>
        /// Deduplicates and deterministically orders resolved direct UML generalizations.
        /// </summary>
        /// <param name="umlClass">
        /// The UML class whose direct generalizations are queried.
        /// </param>
        /// <returns>
        /// The distinct direct UML classes in deterministic order.
        /// </returns>
        private static IClass[] QueryDirectGeneralizations(IClass umlClass)
        {
            return umlClass.SuperClass
                .GroupBy(generalClass => generalClass.XmiId, StringComparer.Ordinal)
                .Select(group => group.First())
                .OrderBy(generalClass => generalClass.Name, StringComparer.OrdinalIgnoreCase)
                .ThenBy(generalClass => generalClass.Name, StringComparer.Ordinal)
                .ThenBy(generalClass => generalClass.XmiId, StringComparer.Ordinal)
                .ToArray();
        }

        /// <summary>
        /// Determines whether a property belongs to the generated DTO contract.
        /// </summary>
        /// <param name="property">
        /// The UML property to evaluate.
        /// </param>
        /// <returns>
        /// <see langword="true" /> when the property is neither derived nor a
        /// derived union; otherwise, <see langword="false" />.
        /// </returns>
        private static bool IsDtoProperty(IProperty property) => !property.IsDerived && !property.IsDerivedUnion;

        /// <summary>
        /// Removes duplicate properties and orders them deterministically for generation.
        /// </summary>
        /// <param name="properties">
        /// The properties to deduplicate and order.
        /// </param>
        /// <returns>
        /// The deterministically ordered properties.
        /// </returns>
        private static IEnumerable<IProperty> OrderProperties(IEnumerable<IProperty> properties)
        {
            var propertyIds = new HashSet<string>(StringComparer.Ordinal);

            var distinctProperties = properties
                .Where(property => propertyIds.Add(property.XmiId))
                .ToArray();

            return distinctProperties
                .OrderBy(property => string.Equals(property.Name, "id", StringComparison.OrdinalIgnoreCase) ? 0 : 1)
                .ThenBy(property => property.Name, StringComparer.OrdinalIgnoreCase)
                .ThenBy(property => property.Name, StringComparer.Ordinal)
                .ThenBy(property => property.XmiId, StringComparer.Ordinal);
        }

        extension(IClass umlClass)
        {
            /// <summary>
            /// Queries the properties declared directly by the class.
            /// </summary>
            /// <returns>
            /// The directly owned properties, deduplicated and deterministically ordered.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the class is <see langword="null" />.
            /// </exception>
            public IReadOnlyList<IProperty> QueryDtoInterfaceProperties()
            {
                ArgumentNullException.ThrowIfNull(umlClass);

                return OrderProperties(umlClass.OwnedAttribute.Where(IsDtoProperty))
                    .ToArray();
            }

            /// <summary>
            /// Queries the properties declared directly by the class for the POCO interface contract.
            /// </summary>
            /// <returns>
            /// The directly owned POCO properties, deduplicated and deterministically ordered.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the class is <see langword="null" />.
            /// </exception>
            public IReadOnlyList<IProperty> QueryPocoInterfaceProperties()
            {
                ArgumentNullException.ThrowIfNull(umlClass);

                return OrderProperties(umlClass.OwnedAttribute)
                    .ToArray();
            }

            /// <summary>
            /// Queries every property that a concrete DTO implementation must
            /// implement, including inherited properties.
            /// </summary>
            /// <returns>
            /// The direct and inherited properties, deduplicated and deterministically ordered.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the class is <see langword="null" />.
            /// </exception>
            public IReadOnlyList<IProperty> QueryDtoImplementationProperties()
            {
                ArgumentNullException.ThrowIfNull(umlClass);

                return OrderProperties(umlClass.QueryAllProperties()
                    .Where(IsDtoProperty))
                    .ToArray();
            }

            /// <summary>
            /// Queries every property that a concrete POCO implementation must implement, including inherited
            /// properties.
            /// </summary>
            /// <returns>
            /// The complete direct and inherited POCO property contract, deduplicated and deterministically
            /// ordered.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the class is <see langword="null" />.
            /// </exception>
            public IReadOnlyList<IProperty> QueryPocoImplementationProperties()
            {
                ArgumentNullException.ThrowIfNull(umlClass);

                return OrderProperties(umlClass.QueryAllProperties())
                    .ToArray();
            }

            /// <summary>
            /// Queries the direct UML generalizations used for generated interface inheritance.
            /// </summary>
            /// <returns>
            /// The distinct direct generalizations in deterministic order.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the class is <see langword="null" />.
            /// </exception>
            public IReadOnlyList<IClass> QueryGeneralizations()
            {
                ArgumentNullException.ThrowIfNull(umlClass);

                return QueryDirectGeneralizations(umlClass);
            }

            /// <summary>
            /// Queries the non-derived attributes of a class and of its generalizations.
            /// </summary>
            /// <returns>
            /// The attributes in declaration order, own attributes before inherited ones.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the class is <see langword="null" />.
            /// </exception>
            public IEnumerable<IProperty> QueryNonDerivedAttributes()
            {
                ArgumentNullException.ThrowIfNull(umlClass);

                return umlClass.OwnedAttribute
                    .Where(property => !property.IsDerived)
                    .Concat(umlClass.Generalization
                        .Select(generalization => generalization.General)
                        .OfType<IClass>()
                        .SelectMany(general => general.QueryNonDerivedAttributes()));
            }

            /// <summary>
            /// Queries a non-derived attribute of a class or of its generalizations, the most specific declaration first.
            /// </summary>
            /// <param name="attributeName">
            /// The attribute name.
            /// </param>
            /// <returns>
            /// The attribute, or <see langword="null" /> when the class declares none by that name.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the class is <see langword="null" />.
            /// </exception>
            public IProperty QueryNonDerivedAttribute(string attributeName)
            {
                ArgumentNullException.ThrowIfNull(umlClass);

                return umlClass.QueryNonDerivedAttributes().FirstOrDefault(property => string.Equals(property.Name, attributeName, StringComparison.Ordinal));
            }
        }
    }
}
