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
        /// Queries the properties declared by the class, including navigable
        /// association ends represented outside the class in the XMI.
        /// </summary>
        public static IReadOnlyList<IProperty> QueryDtoInterfaceProperties(this IClass umlClass)
        {
            ArgumentNullException.ThrowIfNull(umlClass);

            return OrderDtoProperties(umlClass.OwnedAttribute.Concat(QueryAssociationProperties(umlClass)))
                .ToArray();
        }


        /// <summary>
        /// Queries every property that a concrete DTO implementation must
        /// implement, including inherited properties.
        /// </summary>
        public static IReadOnlyList<IProperty> QueryDtoImplementationProperties(this IClass umlClass)
        {
            ArgumentNullException.ThrowIfNull(umlClass);

            var hierarchy = new Dictionary<string, IClass>(StringComparer.Ordinal);
            VisitGeneralizations(umlClass, new HashSet<string>(StringComparer.Ordinal), hierarchy);

            var properties = umlClass.QueryAllProperties()
                .Concat(hierarchy.Values.SelectMany(QueryAssociationProperties));

            return OrderDtoProperties(properties).ToArray();
        }

        /// <summary>
        /// Queries the direct UML generalizations in deterministic order.
        /// </summary>
        public static IReadOnlyList<IClass> QueryDtoGeneralizations(this IClass umlClass)
        {
            ArgumentNullException.ThrowIfNull(umlClass);

            var generalClasses = umlClass.SuperClass;

            if (generalClasses.Count != umlClass.Generalization.Count)
            {
                throw new InvalidOperationException(
                    $"Class '{Describe(umlClass)}' has an unresolved or non-class generalization.");
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
        /// Queries navigable association properties applicable to the specified UML class.
        /// </summary>
        /// <param name="umlClass">
        /// The UML class whose association properties are queried.
        /// </param>
        /// <returns>
        /// The navigable association properties applicable to the UML class.
        /// </returns>
        private static IEnumerable<IProperty> QueryAssociationProperties(IClass umlClass)
        {
            var rootPackage = umlClass.QueryRootPackage()
                ?? throw new InvalidOperationException($"The root package for class '{Describe(umlClass)}' could not be resolved.");

            var associations = rootPackage.QueryPackages()
                .SelectMany(package => package.PackagedElement.OfType<IAssociation>())
                .OrderBy(association => association.XmiId, StringComparer.Ordinal);

            foreach (var association in associations)
            {
                if (association.MemberEnd.Count != 2)
                {
                    throw new InvalidOperationException(
                        $"Association '{association.XmiId}' must have exactly two member ends.");
                }

                foreach (var associationEnd in association.MemberEnd)
                {
                    ValidateAssociationEnd(association, associationEnd);
                }

                foreach (var associationEnd in association.MemberEnd)
                {
                    var oppositeEnd = associationEnd.Opposite
                        ?? throw new InvalidOperationException(
                            $"Association end '{associationEnd.XmiId}' in association "
                            + $"'{association.XmiId}' has no resolved opposite end.");

                    if (ReferencesClass(oppositeEnd, umlClass) && !string.IsNullOrWhiteSpace(associationEnd.Name))
                    {
                        yield return associationEnd;
                    }
                }
            }
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
        /// Visits the class hierarchy while validating generalizations and detecting cycles.
        /// </summary>
        /// <param name="umlClass">
        /// The current UML class.
        /// </param>
        /// <param name="currentPath">
        /// The XMI identifiers on the current traversal path.
        /// </param>
        /// <param name="result">
        /// The validated UML classes indexed by XMI identifier.
        /// </param>
        private static void VisitGeneralizations(IClass umlClass, ISet<string> currentPath, IDictionary<string, IClass> result)
        {
            var xmiId = QueryRequiredXmiId(umlClass);

            if (result.ContainsKey(xmiId))
            {
                return;
            }

            if (!currentPath.Add(xmiId))
            {
                throw new InvalidOperationException($"A generalization cycle was detected at class '{Describe(umlClass)}'.");
            }

            foreach (var generalClass in umlClass.QueryDtoGeneralizations())
            {
                VisitGeneralizations(generalClass, currentPath, result);
            }

            currentPath.Remove(xmiId);
            result.Add(xmiId, umlClass);
        }

        /// <summary>
        /// Validates that an association end has a resolved type.
        /// </summary>
        /// <param name="association">
        /// The containing UML association.
        /// </param>
        /// <param name="associationEnd">
        /// The association end to validate.
        /// </param>
        private static void ValidateAssociationEnd(IAssociation association, IProperty associationEnd)
        {
            if (associationEnd.Type is null)
            {
                throw new InvalidOperationException(
                    $"Association end '{associationEnd.XmiId}' in association "
                    + $"'{association.XmiId}' has no resolved type.");
            }
        }

        /// <summary>
        /// Determines whether an association end references the specified UML class.
        /// </summary>
        /// <param name="associationEnd">
        /// The association end to inspect.
        /// </param>
        /// <param name="umlClass">
        /// The expected referenced UML class.
        /// </param>
        /// <returns>
        /// <see langword="true"/> when the association end references the class;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        private static bool ReferencesClass(
            IProperty associationEnd,
            IClass umlClass)
        {
            return ReferenceEquals(associationEnd.Type, umlClass)
                || string.Equals(associationEnd.Type?.XmiId, umlClass.XmiId, StringComparison.Ordinal);
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
                throw new InvalidOperationException($"Class '{Describe(umlClass)}' has no XMI identifier.");
            }

            return umlClass.XmiId;
        }

        /// <summary>
        /// Returns a readable description of a UML class.
        /// </summary>
        /// <param name="umlClass">
        /// The UML class.
        /// </param>
        /// <returns>
        /// The class name when available; otherwise, its XMI identifier.
        /// </returns>
        private static string Describe(IClass umlClass)
        {
            return string.IsNullOrWhiteSpace(umlClass.Name) ? umlClass.XmiId : umlClass.Name;
        }
    }
}
