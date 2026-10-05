// ------------------------------------------------------------------------------------------------
//  <copyright file="PimRecordExtensions.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Extensions
{
    using uml4net.Packages;
    using uml4net.StructuredClassifiers;

    /// <summary>
    /// Provides Mycelium-specific queries for the PIM classes that a repository interface is generated
    /// from.
    /// </summary>
    public static class PimRecordExtensions
    {
        /// <summary>
        /// The name of the abstract PIM class that gives a specialization a server-assigned identifier.
        /// </summary>
        private const string IdentityBearingClassName = "Record";

        /// <summary>
        /// The prefix of the interface that declares the persistence contract of a record.
        /// </summary>
        private const string RepositoryInterfacePrefix = "I";

        /// <summary>
        /// The suffix of the interface that declares the persistence contract of a record.
        /// </summary>
        private const string RepositoryInterfaceSuffix = "Repository";

        /// <summary>
        /// Determines whether a class is, or specializes, the abstract identity-bearing PIM class.
        /// </summary>
        /// <param name="umlClass">
        /// The class whose generalization hierarchy is walked.
        /// </param>
        /// <returns>
        /// <see langword="true" /> when the class is, or specializes, the identity-bearing class.
        /// </returns>
        private static bool QueryBearsIdentity(IClass umlClass)
        {
            if (string.Equals(umlClass.Name, IdentityBearingClassName, StringComparison.Ordinal))
            {
                return true;
            }

            return umlClass.Generalization
                .Select(generalization => generalization.General)
                .OfType<IClass>()
                .Any(QueryBearsIdentity);
        }

        /// <summary>
        /// Queries a package and the packages nested within it.
        /// </summary>
        /// <param name="package">
        /// The package the traversal starts at.
        /// </param>
        /// <returns>
        /// The package itself followed by every package nested within it, at any depth.
        /// </returns>
        private static IEnumerable<IPackage> QuerySelfAndNestedPackages(IPackage package)
        {
            return [package, .. package.PackagedElement.OfType<IPackage>().SelectMany(QuerySelfAndNestedPackages)];
        }

        extension(IPackage package)
        {
            /// <summary>
            /// Queries the PIM classes that a persistence contract is generated for.
            /// </summary>
            /// <returns>
            /// The concrete classes that bear identity, ordered by name.
            /// </returns>
            /// <remarks>
            /// A persisted record is identified by specializing the abstract <c>Record</c> class rather
            /// than by a name suffix or a list, so a record type added by a future specification export
            /// needs no change here. The classifiers that realize <c>Data</c> are versioned payload and
            /// are reached through the <c>DataVersion</c> that wraps them, not through a repository of
            /// their own.
            /// </remarks>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the package is <see langword="null" />.
            /// </exception>
            /// <exception cref="InvalidOperationException">
            /// Thrown when the package declares no persisted record.
            /// </exception>
            public IReadOnlyList<IClass> QueryRecordClasses()
            {
                ArgumentNullException.ThrowIfNull(package);

                var recordClasses = QuerySelfAndNestedPackages(package)
                    .SelectMany(nestedPackage => nestedPackage.PackagedElement.OfType<IClass>())
                    .Where(umlClass => !umlClass.IsAbstract && QueryBearsIdentity(umlClass))
                    .OrderBy(umlClass => umlClass.Name, StringComparer.Ordinal)
                    .ToArray();

                return recordClasses.Length == 0 ? throw new InvalidOperationException($"Package '{package.Describe()}' declares no concrete specialization of '{IdentityBearingClassName}', so it declares no persisted record.") : recordClasses;
            }
        }

        extension(IClass recordClass)
        {
            /// <summary>
            /// Queries the type name of the interface that declares the persistence contract of a record.
            /// </summary>
            /// <returns>
            /// The interface name - for example <c>IProjectRepository</c> for the <c>Project</c> class.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the class is <see langword="null" />.
            /// </exception>
            /// <exception cref="InvalidOperationException">
            /// Thrown when the class has no name.
            /// </exception>
            /// <exception cref="ArgumentException">
            /// Thrown when the derived name cannot be represented as a legal C# identifier.
            /// </exception>
            public string QueryRepositoryInterfaceName()
            {
                ArgumentNullException.ThrowIfNull(recordClass);

                return ReservedCSharpNameMapper.Map($"{RepositoryInterfacePrefix}{recordClass.QueryRecordTypeName()}{RepositoryInterfaceSuffix}");
            }

            /// <summary>
            /// Queries the C# type the records of a class are read and written as.
            /// </summary>
            /// <returns>
            /// The name of the data transfer object - for example <c>Project</c>.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the class is <see langword="null" />.
            /// </exception>
            /// <exception cref="InvalidOperationException">
            /// Thrown when the class has no name.
            /// </exception>
            /// <exception cref="ArgumentException">
            /// Thrown when the modeled name cannot be represented as a legal C# identifier.
            /// </exception>
            public string QueryRecordTypeName()
            {
                ArgumentNullException.ThrowIfNull(recordClass);

                return string.IsNullOrWhiteSpace(recordClass.Name) ? throw new InvalidOperationException($"Record class '{recordClass.XmiId}' has no name, so the generated interface would have no name.") : ReservedCSharpNameMapper.Map(recordClass.Name);
            }
        }
    }
}
