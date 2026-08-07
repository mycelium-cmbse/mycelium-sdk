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
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using uml4net.Classification;
    using uml4net.Extensions;
    using uml4net.StructuredClassifiers;

    /// <summary>
    /// Provides DTO-specific queries for UML classes.
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
        public static IReadOnlyList<IProperty> QueryDtoInterfaceProperties(this IClass umlClass)
        {
            ArgumentNullException.ThrowIfNull(umlClass);

            return OrderDtoProperties(umlClass.OwnedAttribute).ToArray();
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
        public static IReadOnlyList<IProperty> QueryDtoImplementationProperties(this IClass umlClass)
        {
            ArgumentNullException.ThrowIfNull(umlClass);

            return OrderDtoProperties(umlClass.QueryAllProperties()).ToArray();
        }

        /// <summary>
        /// Queries the direct UML generalizations in deterministic order.
        /// </summary>
        /// <param name="umlClass">
        /// The UML class whose direct generalizations are queried.
        /// </param>
        /// <returns>
        /// The distinct direct generalizations in deterministic order.
        /// </returns>
        public static IReadOnlyList<IClass> QueryDtoGeneralizations(this IClass umlClass)
        {
            ArgumentNullException.ThrowIfNull(umlClass);

            var generalClasses = umlClass.SuperClass;

            if (generalClasses.Count != umlClass.Generalization.Count)
            {
                throw new InvalidOperationException(
                    $"Class '{umlClass.Describe()}' has an unresolved or non-class generalization.");
            }

            return generalClasses
                .GroupBy(generalClass => QueryRequiredXmiId(generalClass), StringComparer.Ordinal)
                .Select(group => group.First())
                .OrderBy(generalClass => generalClass.Name, StringComparer.OrdinalIgnoreCase)
                .ThenBy(generalClass => generalClass.Name, StringComparer.Ordinal)
                .ThenBy(generalClass => generalClass.XmiId, StringComparer.Ordinal)
                .ToArray();
        }

        /// <summary>
        /// Removes duplicate properties and orders them deterministically for generation.
        /// </summary>
        /// <param name="properties">
        /// The properties to validate, deduplicate, and order.
        /// </param>
        /// <returns>
        /// The deterministically ordered DTO properties.
        /// </returns>
        private static IEnumerable<IProperty> OrderDtoProperties(IEnumerable<IProperty> properties)
        {
            var distinctProperties = new List<IProperty>();
            var propertyIds = new HashSet<string>(StringComparer.Ordinal);

            foreach (var property in properties)
            {
                if (string.IsNullOrWhiteSpace(property.XmiId))
                {
                    throw new InvalidOperationException("A DTO property has no XMI identifier.");
                }

                if (string.IsNullOrWhiteSpace(property.Name))
                {
                    throw new InvalidOperationException($"Property '{property.XmiId}' has no name.");
                }

                if (propertyIds.Add(property.XmiId))
                {
                    distinctProperties.Add(property);
                }
            }

            return distinctProperties
                .OrderBy(property => string.Equals(property.Name, "id", StringComparison.OrdinalIgnoreCase)
                            ? 0
                            : 1)
                .ThenBy(property => property.Name, StringComparer.OrdinalIgnoreCase)
                .ThenBy(property => property.Name, StringComparer.Ordinal)
                .ThenBy(property => property.XmiId, StringComparer.Ordinal);
        }

        /// <summary>
        /// Returns the required XMI identifier of a UML class.
        /// </summary>
        /// <param name="umlClass">
        /// The UML class.
        /// </param>
        /// <returns>
        /// The class XMI identifier.
        /// </returns>
        private static string QueryRequiredXmiId(IClass umlClass)
        {
            if (string.IsNullOrWhiteSpace(umlClass.XmiId))
            {
                throw new InvalidOperationException($"Class '{umlClass.Describe()}' has no XMI identifier.");
            }

            return umlClass.XmiId;
        }
    }
}
