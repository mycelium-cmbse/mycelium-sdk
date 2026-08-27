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
    using Mycelium.SDK.CodeGenerator.Extensions;

    using uml4net.Extensions;
    using uml4net.Packages;
    using uml4net.SimpleClassifiers;
    using uml4net.StructuredClassifiers;
    using uml4net.xmi.Readers;

    /// <summary>
    /// Base class for UML Handlebars generators targeting FunctionalData.
    /// </summary>
    public abstract class UmlHandleBarsGenerator : HandleBarsGenerator
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="UmlHandleBarsGenerator" /> class.
        /// </summary>
        protected UmlHandleBarsGenerator() : base("Uml")
        {
        }

        /// <summary>
        /// Loads, validates, and generates from the canonical FunctionalData resources.
        /// </summary>
        /// <param name="resourcesDirectory">
        /// The directory containing the reviewed XMI resources.
        /// </param>
        /// <param name="outputDirectory">
        /// The generated-output destination.
        /// </param>
        /// <returns>
        /// An awaitable task.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="resourcesDirectory" /> or <paramref name="outputDirectory" /> is
        /// <see langword="null" />.
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">
        /// Thrown when <paramref name="resourcesDirectory" /> does not exist.
        /// </exception>
        /// <exception cref="FileNotFoundException">
        /// Thrown when a required FunctionalData XMI resource is missing.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when a modeled name cannot be represented as a legal C# identifier.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when loading, semantic validation, rendering, syntax validation, or manifest validation
        /// fails.
        /// </exception>
        public Task GenerateAsync(DirectoryInfo resourcesDirectory, DirectoryInfo outputDirectory)
        {
            ArgumentNullException.ThrowIfNull(resourcesDirectory);
            ArgumentNullException.ThrowIfNull(outputDirectory);

            var xmiReaderResult = XmiReaderResultExtensions.ReadFunctionalData(resourcesDirectory);

            return this.GenerateAsync(xmiReaderResult, outputDirectory);
        }

        /// <summary>
        /// Generates the artifacts supported by the concrete generator.
        /// </summary>
        /// <param name="xmiReaderResult">
        /// The parsed UML model used for generation.
        /// </param>
        /// <param name="outputDirectory">
        /// The directory to which the generated artifacts are written.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous generation operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="xmiReaderResult" /> or <paramref name="outputDirectory" /> is
        /// <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when a modeled name cannot be represented as a legal C# identifier.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when semantic validation, rendering, syntax validation, or manifest validation fails.
        /// </exception>
        public abstract Task GenerateAsync(XmiReaderResult xmiReaderResult, DirectoryInfo outputDirectory);

        /// <summary>
        /// Creates a deterministic payload for the FunctionalData package.
        /// </summary>
        /// <param name="xmiReaderResult">
        /// The parsed UML model from which the payload is created.
        /// </param>
        /// <returns>
        /// A deterministic Handlebars payload for the FunctionalData package.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="xmiReaderResult" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when a modeled name cannot be represented as a legal C# identifier.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the model does not contain exactly one package named <c>FunctionalData</c> or
        /// does not satisfy the reviewed FunctionalData semantic contract.
        /// </exception>
        protected static HandlebarsPayload CreateHandlebarsPayload(XmiReaderResult xmiReaderResult)
        {
            ArgumentNullException.ThrowIfNull(xmiReaderResult);

            xmiReaderResult.ValidateFunctionalData();

            var allPackages = xmiReaderResult.Packages
                .SelectMany(package => package.QueryPackages())
                .Distinct<IPackage>(ReferenceEqualityComparer.Instance)
                .OrderBy(package => package.Name, StringComparer.Ordinal)
                .ToArray();

            var rootPackage = xmiReaderResult.QueryFunctionalDataPackage();

            var generationPackages = rootPackage
                .QueryPackages()
                .OrderBy(package => package.Name, StringComparer.Ordinal)
                .ToArray();

            var enumerations = generationPackages
                .SelectMany(package => package.PackagedElement.OfType<IEnumeration>())
                .OrderBy(
                    enumeration => enumeration.Name,
                    StringComparer.Ordinal);

            var primitiveTypes = allPackages
                .SelectMany(package => package.PackagedElement.OfType<IPrimitiveType>())
                .OrderBy(
                    primitiveType => primitiveType.Name,
                    StringComparer.Ordinal);

            var dataTypes = generationPackages
                .SelectMany(package => package.PackagedElement.OfType<IDataType>())
                .Where(dataType =>
                    dataType is not IEnumeration
                    && dataType is not IPrimitiveType)
                .OrderBy(
                    dataType => dataType.Name,
                    StringComparer.Ordinal);

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

        /// <summary>
        /// Validates one enumeration for single-artifact rendering.
        /// </summary>
        /// <param name="enumeration">
        /// The UML enumeration to validate.
        /// </param>
        /// <returns>
        /// Its validated modeled name.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="enumeration" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when the enumeration name or a literal name cannot be represented as a legal C#
        /// identifier.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the enumeration or a literal is unnamed, or mapped literal identifiers collide.
        /// </exception>
        protected static string ValidateEnumeration(IEnumeration enumeration)
        {
            ArgumentNullException.ThrowIfNull(enumeration);

            if (string.IsNullOrWhiteSpace(enumeration.Name))
            {
                throw new InvalidOperationException(
                    $"Enumeration '{enumeration.XmiId}' has no name.");
            }

            _ = ReservedCSharpNameMapper.Map(enumeration.Name);

            var duplicateLiteralIdentifier = enumeration.OwnedLiteral
                .Select(literal =>
                {
                    if (string.IsNullOrWhiteSpace(literal.Name))
                    {
                        throw new InvalidOperationException(
                            $"Enumeration '{enumeration.Name}' contains an unnamed literal.");
                    }

                    return ReservedCSharpNameMapper.Map(literal.Name);
                })
                .GroupBy(
                    identifier => identifier,
                    StringComparer.Ordinal)
                .FirstOrDefault(group => group.Count() > 1)
                ?.Key;

            if (duplicateLiteralIdentifier is not null)
            {
                throw new InvalidOperationException(
                    $"Enumeration '{enumeration.Name}' contains duplicate C# literal "
                    + $"identifier '{duplicateLiteralIdentifier}'.");
            }

            return enumeration.Name;
        }
    }
}
