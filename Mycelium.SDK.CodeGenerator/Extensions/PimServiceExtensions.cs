// ------------------------------------------------------------------------------------------------
//  <copyright file="PimServiceExtensions.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Extensions
{
    using uml4net.Classification;
    using uml4net.Packages;
    using uml4net.StructuredClassifiers;

    /// <summary>
    /// Provides Mycelium-specific queries for the PIM classes that a service interface is generated from.
    /// </summary>
    public static class PimServiceExtensions
    {
        extension(IPackage package)
        {
            /// <summary>
            /// Queries the PIM classes that declare a service.
            /// </summary>
            /// <returns>
            /// The classes that own at least one operation, ordered by name.
            /// </returns>
            /// <remarks>
            /// A service is identified by owning operations rather than by a name suffix, so a service
            /// added or renamed by a future specification export needs no change here.
            /// </remarks>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the package is <see langword="null" />.
            /// </exception>
            /// <exception cref="InvalidOperationException">
            /// Thrown when the package declares no service.
            /// </exception>
            public IReadOnlyList<IClass> QueryServiceClasses()
            {
                ArgumentNullException.ThrowIfNull(package);

                var serviceClasses = package.PackagedElement
                    .OfType<IClass>()
                    .Where(umlClass => umlClass.OwnedOperation.Count > 0)
                    .OrderBy(umlClass => umlClass.Name, StringComparer.Ordinal)
                    .ToArray();

                return serviceClasses.Length == 0 ? throw new InvalidOperationException($"Package '{package.Describe()}' declares no class owning an operation, so it declares no service.") : serviceClasses;
            }
        }

        extension(IClass serviceClass)
        {
            /// <summary>
            /// Queries the type name of the interface that declares a PIM service.
            /// </summary>
            /// <returns>
            /// The interface name - for example <c>IProjectService</c> for the <c>ProjectService</c> class.
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
            public string QueryServiceInterfaceName()
            {
                ArgumentNullException.ThrowIfNull(serviceClass);

                return string.IsNullOrWhiteSpace(serviceClass.Name) ? throw new InvalidOperationException($"Service class '{serviceClass.XmiId}' has no name, so the generated interface would have no name.") : ReservedCSharpNameMapper.Map($"I{serviceClass.Name}");
            }

            /// <summary>
            /// Queries the operations of a PIM service in deterministic emission order.
            /// </summary>
            /// <returns>
            /// The owned operations, ordered by name and then by XMI identifier.
            /// </returns>
            /// <remarks>
            /// Document order is not guaranteed across exports, so emission order is imposed here.
            /// </remarks>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the class is <see langword="null" />.
            /// </exception>
            /// <exception cref="InvalidOperationException">
            /// Thrown when an owned operation has no name.
            /// </exception>
            public IReadOnlyList<IOperation> QueryServiceOperations()
            {
                ArgumentNullException.ThrowIfNull(serviceClass);

                var unnamedOperation = serviceClass.OwnedOperation.FirstOrDefault(operation => string.IsNullOrWhiteSpace(operation.Name));

                if (unnamedOperation is not null)
                {
                    throw new InvalidOperationException($"Service class '{serviceClass.Describe()}' owns operation '{unnamedOperation.XmiId}' which has no name.");
                }

                return serviceClass.OwnedOperation
                    .OrderBy(operation => operation.Name, StringComparer.Ordinal)
                    .ThenBy(operation => operation.XmiId, StringComparer.Ordinal)
                    .ToArray();
            }
        }
    }
}
