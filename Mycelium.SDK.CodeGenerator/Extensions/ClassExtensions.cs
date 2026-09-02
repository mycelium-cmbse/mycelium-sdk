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
        /// Queries the properties declared directly by the class.
        /// </summary>
        /// <param name="umlClass">
        /// The UML class whose directly owned properties are queried.
        /// </param>
        /// <returns>
        /// The directly owned properties, deduplicated and deterministically ordered.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="umlClass" /> is <see langword="null" />.
        /// </exception>
        public static IReadOnlyList<IProperty> QueryDtoInterfaceProperties(this IClass umlClass)
        {
            ArgumentNullException.ThrowIfNull(umlClass);

            return OrderProperties(
                    umlClass.OwnedAttribute.Where(IsDtoProperty))
                .ToArray();
        }

        /// <summary>
        /// Queries the properties declared directly by the class for the POCO interface contract.
        /// </summary>
        /// <param name="umlClass">
        /// The UML class whose directly owned POCO properties are queried.
        /// </param>
        /// <returns>
        /// The directly owned POCO properties, deduplicated and deterministically ordered.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="umlClass" /> is <see langword="null" />.
        /// </exception>
        public static IReadOnlyList<IProperty> QueryPocoInterfaceProperties(this IClass umlClass)
        {
            ArgumentNullException.ThrowIfNull(umlClass);

            return OrderProperties(umlClass.OwnedAttribute).ToArray();
        }

        /// <summary>
        /// Queries every property that a concrete DTO implementation must
        /// implement, including inherited properties.
        /// </summary>
        /// <param name="umlClass">
        /// The UML class whose implementation properties are queried.
        /// </param>
        /// <returns>
        /// The direct and inherited properties, deduplicated and deterministically ordered.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="umlClass" /> is <see langword="null" />.
        /// </exception>
        public static IReadOnlyList<IProperty> QueryDtoImplementationProperties(this IClass umlClass)
        {
            ArgumentNullException.ThrowIfNull(umlClass);

            return OrderProperties(
                    umlClass.QueryAllProperties().Where(IsDtoProperty))
                .ToArray();
        }

        /// <summary>
        /// Queries every property that a concrete POCO implementation must implement, including inherited
        /// properties.
        /// </summary>
        /// <param name="umlClass">
        /// The UML class whose POCO implementation properties are queried.
        /// </param>
        /// <returns>
        /// The complete direct and inherited POCO property contract, deduplicated and deterministically
        /// ordered.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="umlClass" /> is <see langword="null" />.
        /// </exception>
        public static IReadOnlyList<IProperty> QueryPocoImplementationProperties(this IClass umlClass)
        {
            ArgumentNullException.ThrowIfNull(umlClass);

            return OrderProperties(umlClass.QueryAllProperties()).ToArray();
        }

        /// <summary>
        /// Queries the direct UML generalizations used for generated interface inheritance.
        /// </summary>
        /// <param name="umlClass">
        /// The UML class whose direct generalizations are queried.
        /// </param>
        /// <returns>
        /// The distinct direct generalizations in deterministic order.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="umlClass" /> is <see langword="null" />.
        /// </exception>
        public static IReadOnlyList<IClass> QueryGeneralizations(this IClass umlClass)
        {
            ArgumentNullException.ThrowIfNull(umlClass);

            return QueryDirectGeneralizations(umlClass);
        }

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
        private static bool IsDtoProperty(IProperty property)
        {
            return !property.IsDerived && !property.IsDerivedUnion;
        }

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
            var distinctProperties = new List<IProperty>();
            var propertyIds = new HashSet<string>(StringComparer.Ordinal);

            foreach (var property in properties)
            {
                if (propertyIds.Add(property.XmiId))
                {
                    distinctProperties.Add(property);
                }
            }

            return distinctProperties
                .OrderBy(property => string.Equals(property.Name, "id", StringComparison.OrdinalIgnoreCase) ? 0 : 1)
                .ThenBy(property => property.Name, StringComparer.OrdinalIgnoreCase)
                .ThenBy(property => property.Name, StringComparer.Ordinal)
                .ThenBy(property => property.XmiId, StringComparer.Ordinal);
        }
    }
}
