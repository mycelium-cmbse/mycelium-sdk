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

            return OrderDtoProperties(QueryDirectProperties(umlClass))
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

            return OrderDtoProperties(hierarchy.Values.SelectMany(candidate => candidate.QueryDtoInterfaceProperties()))
                .ToArray();
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

        private static IEnumerable<IProperty> QueryDirectProperties(IClass umlClass)
        {
            foreach (var property in umlClass.OwnedAttribute)
            {
                yield return property;
            }

            var rootPackage = uml4net.Extensions.ElementExtensions
                .QueryRootPackage(umlClass) ?? throw new InvalidOperationException($"The root package for class '{Describe(umlClass)}' could not be resolved.");

            var associations = uml4net.Extensions.PackageExtensions
                    .QueryPackages(rootPackage)
                    .SelectMany(package => package.PackagedElement.OfType<IAssociation>())
                    .OrderBy(association => association.XmiId, StringComparer.Ordinal);

            foreach (var association in associations)
            {
                if (association.MemberEnd.Count != 2)
                {
                    throw new InvalidOperationException($"Association '{association.XmiId}' must have exactly two member ends.");
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
                .OrderBy(property => string.Equals(property.Name, "Id", StringComparison.Ordinal) ? 0 : 1)
                .ThenBy(property => property.Name, StringComparer.OrdinalIgnoreCase)
                .ThenBy(property => property.Name, StringComparer.Ordinal)
                .ThenBy(property => property.XmiId, StringComparer.Ordinal);
        }

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

        private static void ValidateAssociationEnd(IAssociation association, IProperty associationEnd)
        {
            if (associationEnd.Type is null)
            {
                throw new InvalidOperationException($"Association end '{associationEnd.XmiId}' in association " + $"'{association.XmiId}' has no resolved type.");
            }
        }

        private static bool ReferencesClass(IProperty associationEnd, IClass umlClass)
        {
            return ReferenceEquals(associationEnd.Type, umlClass)
                || string.Equals(associationEnd.Type?.XmiId, umlClass.XmiId, StringComparison.Ordinal);
        }

        private static string QueryRequiredXmiId(IClass umlClass)
        {
            if (string.IsNullOrWhiteSpace(umlClass.XmiId))
            {
                throw new InvalidOperationException($"Class '{Describe(umlClass)}' has no XMI identifier.");
            }

            return umlClass.XmiId;
        }

        private static string Describe(IClass umlClass)
        {
            return string.IsNullOrWhiteSpace(umlClass.Name) ? umlClass.XmiId : umlClass.Name;
        }
    }
}
