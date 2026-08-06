// ------------------------------------------------------------------------------------------------
//  <copyright file="UmlHandleBarsGenerator.cs" company="Starion Group S.A.">
// 
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
// 
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Generators.UmlHandleBarsGenerators
{
    using uml4net.Extensions;
    using uml4net.SimpleClassifiers;
    using uml4net.StructuredClassifiers;
    using uml4net.xmi.Readers;

    /// <summary>
    /// Base class for UML Handlebars generators targeting FunctionalData.
    /// </summary>
    public abstract class UmlHandleBarsGenerator : HandleBarsGenerator
    {
        /// <summary>
        /// The name of the UML package containing the FunctionalData model.
        /// </summary>
        private const string FunctionalDataPackageName = "FunctionalData";

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="UmlHandleBarsGenerator" /> class.
        /// </summary>
        protected UmlHandleBarsGenerator() : base("Uml")
        {
        }

        /// <summary>
        /// Generates the artifacts supported by the concrete generator.
        /// </summary>
        public abstract Task GenerateAsync(XmiReaderResult xmiReaderResult, DirectoryInfo outputDirectory);

        /// <summary>
        /// Creates a deterministic payload for the FunctionalData package.
        /// </summary>
        protected static HandlebarsPayload CreateHandlebarsPayload(XmiReaderResult xmiReaderResult)
        {
            ArgumentNullException.ThrowIfNull(xmiReaderResult);

            var allPackages = xmiReaderResult.Packages
                .SelectMany(package => package.QueryPackages())
                .OrderBy(package => package.Name, StringComparer.Ordinal)
                .ToArray();

            var rootPackage = allPackages.Single(package => package.Name == FunctionalDataPackageName);

            var generationPackages = rootPackage
                .QueryPackages()
                .OrderBy(package => package.Name, StringComparer.Ordinal)
                .ToArray();

            var enumerations = generationPackages
                .SelectMany(package => package.PackagedElement.OfType<IEnumeration>())
                .OrderBy(enumeration => enumeration.Name, StringComparer.Ordinal);

            var primitiveTypes = allPackages
                .SelectMany(package => package.PackagedElement.OfType<IPrimitiveType>())
                .OrderBy(primitiveType => primitiveType.Name, StringComparer.Ordinal);

            var dataTypes = generationPackages
                .SelectMany(package => package.PackagedElement.OfType<IDataType>())
                .Where(dataType => dataType is not IEnumeration && dataType is not IPrimitiveType)
                .OrderBy(dataType => dataType.Name, StringComparer.Ordinal);

            var classes = generationPackages
                .SelectMany(package => package.PackagedElement.OfType<IClass>())
                .OrderBy(umlClass => umlClass.Name, StringComparer.Ordinal);

            var interfaces = generationPackages
                .SelectMany(package => package.PackagedElement.OfType<IInterface>())
                .OrderBy(umlInterface => umlInterface.Name, StringComparer.Ordinal);

            return new HandlebarsPayload(
                rootPackage,
                allPackages,
                enumerations,
                primitiveTypes,
                dataTypes,
                classes,
                interfaces);
        }
    }
}
