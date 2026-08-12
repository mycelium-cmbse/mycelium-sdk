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
        /// <exception cref="InvalidOperationException">
        /// Thrown when a directly owned property has no XMI identifier or name.
        /// </exception>
        public static IReadOnlyList<IProperty> QueryDtoInterfaceProperties(this IClass umlClass)
        {
            ArgumentNullException.ThrowIfNull(umlClass);

            return OrderProperties(
                    umlClass.OwnedAttribute
                        .Where(property => !property.IsDerived && !property.IsDerivedUnion), "DTO")
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
        /// <exception cref="InvalidOperationException">
        /// Thrown when a directly owned property has no XMI identifier or name.
        /// </exception>
        public static IReadOnlyList<IProperty> QueryPocoInterfaceProperties(this IClass umlClass)
        {
            ArgumentNullException.ThrowIfNull(umlClass);

            return OrderProperties(umlClass.OwnedAttribute, "POCO").ToArray();
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
        /// <exception cref="InvalidOperationException">
        /// Thrown when the generalization hierarchy is invalid or cyclic, or when a direct or inherited
        /// property has no XMI identifier or name.
        /// </exception>
        public static IReadOnlyList<IProperty> QueryDtoImplementationProperties(this IClass umlClass)
        {
            ArgumentNullException.ThrowIfNull(umlClass);

            ValidateGeneralizationHierarchy(umlClass);

            return OrderProperties(
                    umlClass.QueryAllProperties()
                        .Where(property => !property.IsDerived && !property.IsDerivedUnion), "DTO")
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
        /// <exception cref="InvalidOperationException">
        /// Thrown when the generalization hierarchy is invalid or cyclic, or when a direct or inherited
        /// property has no XMI identifier or name.
        /// </exception>
        public static IReadOnlyList<IProperty> QueryPocoImplementationProperties(this IClass umlClass)
        {
            ArgumentNullException.ThrowIfNull(umlClass);

            ValidateGeneralizationHierarchy(umlClass);

            return OrderProperties(umlClass.QueryAllProperties(), "POCO").ToArray();
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
        /// <exception cref="InvalidOperationException">
        /// Thrown when a generalization is unresolved or is not a UML class, or when a generalized class
        /// has no XMI identifier.
        /// </exception>
        public static IReadOnlyList<IClass> QueryGeneralizations(this IClass umlClass)
        {
            ArgumentNullException.ThrowIfNull(umlClass);

            return QueryDirectGeneralizations(umlClass);
        }

        /// <summary>
        /// Validates the complete UML generalization hierarchy before inherited elements are queried.
        /// </summary>
        /// <param name="umlClass">
        /// The root UML class whose hierarchy is validated.
        /// </param>
        private static void ValidateGeneralizationHierarchy(IClass umlClass)
        {
            var visitingClasses = new HashSet<IClass>(ReferenceEqualityComparer.Instance);
            var visitedClasses = new HashSet<IClass>(ReferenceEqualityComparer.Instance);
            var path = new List<IClass>();

            ValidateGeneralizationHierarchy(umlClass, visitingClasses, visitedClasses, path);
        }

        /// <summary>
        /// Recursively validates a UML generalization hierarchy and detects cycles.
        /// </summary>
        /// <param name="umlClass">
        /// The UML class currently being visited.
        /// </param>
        /// <param name="visitingClasses">
        /// Classes in the current traversal path.
        /// </param>
        /// <param name="visitedClasses">
        /// Classes whose hierarchies have already been validated.
        /// </param>
        /// <param name="path">
        /// The current traversal path used to describe a detected cycle.
        /// </param>
        private static void ValidateGeneralizationHierarchy(IClass umlClass, ISet<IClass> visitingClasses, ISet<IClass> visitedClasses, List<IClass> path)
        {
            if (visitedClasses.Contains(umlClass))
            {
                return;
            }

            if (!visitingClasses.Add(umlClass))
            {
                var cycleStartIndex = path.FindIndex(candidate => ReferenceEquals(candidate, umlClass));

                var cycle = path
                    .Skip(cycleStartIndex)
                    .Append(umlClass)
                    .Select(cycleClass => $"'{cycleClass.Describe()}'");

                throw new InvalidOperationException($"Generalization cycle detected: {string.Join(" -> ", cycle)}.");
            }

            path.Add(umlClass);

            foreach (var generalClass in QueryDirectGeneralizations(umlClass))
            {
                ValidateGeneralizationHierarchy(generalClass, visitingClasses, visitedClasses, path);
            }

            path.RemoveAt(path.Count - 1);
            visitingClasses.Remove(umlClass);
            visitedClasses.Add(umlClass);
        }

        /// <summary>
        /// Validates, deduplicates, and deterministically orders direct UML generalizations.
        /// </summary>
        /// <param name="umlClass">
        /// The UML class whose direct generalizations are queried.
        /// </param>
        /// <returns>
        /// The distinct direct UML classes in deterministic order.
        /// </returns>
        private static IClass[] QueryDirectGeneralizations(IClass umlClass)
        {
            var generalClasses = umlClass.SuperClass;

            if (generalClasses.Count != umlClass.Generalization.Count)
            {
                throw new InvalidOperationException($"Class '{umlClass.Describe()}' has an unresolved or non-class generalization.");
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
        /// <param name="artifactName">
        /// The artifact name used in validation messages.
        /// </param>
        /// <returns>
        /// The deterministically ordered properties.
        /// </returns>
        private static IEnumerable<IProperty> OrderProperties(IEnumerable<IProperty> properties, string artifactName)
        {
            var distinctProperties = new List<IProperty>();
            var propertyIds = new HashSet<string>(StringComparer.Ordinal);

            foreach (var property in properties)
            {
                if (string.IsNullOrWhiteSpace(property.XmiId))
                {
                    throw new InvalidOperationException($"A {artifactName} property has no XMI identifier.");
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
                .OrderBy(property => string.Equals(property.Name, "id", StringComparison.OrdinalIgnoreCase) ? 0 : 1)
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
