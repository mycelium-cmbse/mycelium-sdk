// ------------------------------------------------------------------------------------------------
//  <copyright file="PackageExtensions.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Extensions
{
    using uml4net.Extensions;
    using uml4net.Packages;
    using uml4net.StructuredClassifiers;

    /// <summary>
    /// Provides Mycelium-specific queries for UML packages.
    /// </summary>
    public static class PackageExtensions
    {
        /// <summary>
        /// The name of the KerML metaclass every element of a model specializes.
        /// </summary>
        private const string ElementClassName = "Element";

        /// <summary>
        /// Queries the outermost package that owns a package.
        /// </summary>
        /// <param name="package">
        /// The package the traversal starts at.
        /// </param>
        /// <returns>
        /// The package itself when no package owns it, otherwise the outermost package owning it.
        /// </returns>
        private static IPackage QueryRootPackage(IPackage package)
        {
            return package.Possessor is IPackage owningPackage ? QueryRootPackage(owningPackage) : package;
        }

        /// <summary>
        /// Determines whether a class is, or specializes, the KerML <c>Element</c> metaclass.
        /// </summary>
        /// <param name="umlClass">
        /// The class whose generalization hierarchy is walked.
        /// </param>
        /// <returns>
        /// <see langword="true" /> when the class is, or specializes, <c>Element</c>.
        /// </returns>
        private static bool QuerySpecializesElement(IClass umlClass)
        {
            if (string.Equals(umlClass.Name, ElementClassName, StringComparison.Ordinal))
            {
                return true;
            }

            return umlClass.Generalization
                .Select(generalization => generalization.General)
                .OfType<IClass>()
                .Any(QuerySpecializesElement);
        }

        /// <param name="package">
        /// The package that is queried.
        /// </param>
        extension(IPackage package)
        {
            /// <summary>
            /// Queries the SysML v2 and KerML metaclasses that a core service interface is generated for.
            /// </summary>
            /// <returns>
            /// The concrete classes that specialize <c>Element</c>, in the package and in every model it imports, ordered by name.
            /// </returns>
            /// <remarks>
            /// An import is followed to the root of the imported model, because the SysML v2 abstract syntax imports
            /// <c>KerML::Kernel</c> while <c>Element</c> and the other foundational metaclasses sit in its sibling packages.
            /// </remarks>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the package is <see langword="null" />.
            /// </exception>
            /// <exception cref="InvalidOperationException">
            /// Thrown when the package declares no concrete metaclass.
            /// </exception>
            public IReadOnlyList<IClass> QueryConcreteElementClasses()
            {
                ArgumentNullException.ThrowIfNull(package);

                var ownPackages = package.QueryPackages();

                var importedPackages = ownPackages
                    .SelectMany(ownPackage => ownPackage.PackageImport)
                    .SelectMany(packageImport => QueryRootPackage(packageImport.ImportedPackage).QueryPackages());

                var elementClasses = ownPackages
                    .Concat(importedPackages)
                    .DistinctBy(candidatePackage => candidatePackage.XmiId, StringComparer.Ordinal)
                    .SelectMany(candidatePackage => candidatePackage.PackagedElement.OfType<IClass>())
                    .Where(umlClass => !umlClass.IsAbstract && QuerySpecializesElement(umlClass))
                    .OrderBy(umlClass => umlClass.Name, StringComparer.Ordinal)
                    .ToArray();

                return elementClasses.Length == 0 ? throw new InvalidOperationException($"Package '{package.Describe()}' declares no concrete specialization of '{ElementClassName}'.") : elementClasses;
            }
        }
    }
}
