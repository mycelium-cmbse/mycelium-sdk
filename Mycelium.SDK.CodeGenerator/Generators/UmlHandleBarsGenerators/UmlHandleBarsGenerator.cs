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
        /// The current model-derived documentation-symbol mappings.
        /// </summary>
        private Dictionary<string, string> documentationSymbols =
            new(StringComparer.Ordinal);

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="UmlHandleBarsGenerator" /> class.
        /// </summary>
        protected UmlHandleBarsGenerator() : base("Uml")
        {
        }

        /// <summary>
        /// Loads and generates from the canonical FunctionalData resources.
        /// </summary>
        /// <param name="resourcesDirectory">
        /// The directory containing the FunctionalData XMI resources.
        /// </param>
        /// <param name="outputDirectory">
        /// The generated-output destination.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous generation operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="resourcesDirectory" /> or <paramref name="outputDirectory" /> is
        /// <see langword="null" />.
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">
        /// Thrown when <paramref name="resourcesDirectory" /> does not exist.
        /// </exception>
        /// <exception cref="FileNotFoundException">
        /// Thrown when a required FunctionalData resource is missing.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the loaded model does not contain exactly one FunctionalData package.
        /// </exception>
        public Task GenerateAsync(
            DirectoryInfo resourcesDirectory,
            DirectoryInfo outputDirectory)
        {
            ArgumentNullException.ThrowIfNull(resourcesDirectory);
            ArgumentNullException.ThrowIfNull(outputDirectory);

            var xmiReaderResult =
                XmiReaderResultExtensions.ReadFunctionalData(resourcesDirectory);

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
        public abstract Task GenerateAsync(
            XmiReaderResult xmiReaderResult,
            DirectoryInfo outputDirectory);

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
        /// <exception cref="InvalidOperationException">
        /// Thrown when the model does not contain exactly one FunctionalData package.
        /// </exception>
        protected static HandlebarsPayload CreateHandlebarsPayload(
            XmiReaderResult xmiReaderResult)
        {
            ArgumentNullException.ThrowIfNull(xmiReaderResult);

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
                .SelectMany(
                    package =>
                        package.PackagedElement.OfType<IEnumeration>())
                .OrderBy(
                    enumeration => enumeration.Name,
                    StringComparer.Ordinal);

            var primitiveTypes = allPackages
                .SelectMany(
                    package =>
                        package.PackagedElement.OfType<IPrimitiveType>())
                .OrderBy(
                    primitiveType => primitiveType.Name,
                    StringComparer.Ordinal);

            var dataTypes = generationPackages
                .SelectMany(
                    package =>
                        package.PackagedElement.OfType<IDataType>())
                .Where(
                    dataType =>
                        dataType is not IEnumeration
                        && dataType is not IPrimitiveType)
                .OrderBy(
                    dataType => dataType.Name,
                    StringComparer.Ordinal);

            var classes = generationPackages
                .SelectMany(
                    package =>
                        package.PackagedElement.OfType<IClass>())
                .OrderBy(
                    umlClass => umlClass.Name,
                    StringComparer.Ordinal);

            var interfaces = generationPackages
                .SelectMany(
                    package =>
                        package.PackagedElement.OfType<IInterface>())
                .OrderBy(
                    umlInterface => umlInterface.Name,
                    StringComparer.Ordinal);

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
        /// Configures documentation symbols from the current model payload.
        /// </summary>
        /// <param name="payload">
        /// The model-derived generation payload.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="payload" /> is <see langword="null" />.
        /// </exception>
        protected void ConfigureDocumentationSymbols(
            HandlebarsPayload payload)
        {
            ArgumentNullException.ThrowIfNull(payload);

            var symbols =
                new Dictionary<string, string>(StringComparer.Ordinal);

            var ambiguousNames =
                new HashSet<string>(StringComparer.Ordinal);

            void AddSymbol(string umlName, string generatedName)
            {
                if (string.IsNullOrWhiteSpace(umlName)
                    || ambiguousNames.Contains(umlName))
                {
                    return;
                }

                string generatedIdentifier;

                try
                {
                    generatedIdentifier =
                        ReservedCSharpNameMapper.Map(generatedName);
                }
                catch (ArgumentException)
                {
                    return;
                }

                if (symbols.TryAdd(umlName, generatedIdentifier))
                {
                    return;
                }

                symbols.Remove(umlName);
                ambiguousNames.Add(umlName);
            }

            foreach (var umlClass in payload.Classes)
            {
                AddSymbol(
                    umlClass.Name,
                    umlClass.IsAbstract
                        ? $"I{umlClass.Name}"
                        : umlClass.Name);
            }

            foreach (var enumerationName in payload.Enumerations.Select(enumeration => enumeration.Name))
            {
                AddSymbol(enumerationName, enumerationName);
            }

            foreach (var primitiveTypeName in payload.PrimitiveTypes.Select(primitiveType => primitiveType.Name))
            {
                AddSymbol(primitiveTypeName, primitiveTypeName);
            }

            this.documentationSymbols = symbols;
        }

        /// <summary>
        /// Clears the current model-derived documentation symbols.
        /// </summary>
        protected void ClearDocumentationSymbols()
        {
            this.documentationSymbols =
                new Dictionary<string, string>(StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves a UML documentation reference to its generated CLR symbol.
        /// </summary>
        /// <param name="cref">
        /// The UML documentation reference.
        /// </param>
        /// <returns>
        /// The generated CLR symbol, or <see langword="null" /> when no valid unambiguous symbol exists.
        /// </returns>
        protected string ResolveDocumentationCref(string cref)
        {
            return cref is not null
                   && this.documentationSymbols.TryGetValue(
                       cref,
                       out var generatedSymbol)
                ? generatedSymbol
                : null;
        }
    }
}
