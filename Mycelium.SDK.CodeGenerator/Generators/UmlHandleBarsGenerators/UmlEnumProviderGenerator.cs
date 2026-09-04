// ------------------------------------------------------------------------------------------------
//  <copyright file="UmlEnumProviderGenerator.cs" company="Starion Group S.A.">
// 
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
// 
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Generators.UmlHandleBarsGenerators
{
    using Mycelium.SDK.CodeGenerator.HandleBarHelpers;

    using uml4net.SimpleClassifiers;
    using uml4net.xmi.Readers;

    /// <summary>
    /// Generates exact XMI-literal providers for FunctionalData enumerations.
    /// </summary>
    public sealed class UmlEnumProviderGenerator : UmlHandleBarsGenerator
    {
        /// <summary>
        /// The registered Handlebars template name used for enumeration providers.
        /// </summary>
        private const string TemplateName = "enumprovider-uml-template";

        /// <inheritdoc />
        public override async Task GenerateAsync(
            XmiReaderResult xmiReaderResult,
            DirectoryInfo outputDirectory)
        {
            ArgumentNullException.ThrowIfNull(xmiReaderResult);
            ArgumentNullException.ThrowIfNull(outputDirectory);

            var payload = CreateHandlebarsPayload(xmiReaderResult);

            var generatedFiles = payload.Enumerations
                .Select(this.RenderEnumerationProvider)
                .OrderBy(generatedFile => generatedFile.FileName, StringComparer.Ordinal)
                .ToArray();

            await WriteAsync(generatedFiles, outputDirectory);
        }

        /// <summary>
        /// Generates one enumeration provider.
        /// </summary>
        /// <param name="outputDirectory">
        /// The directory to which the generated provider is written.
        /// </param>
        /// <param name="enumeration">
        /// The UML enumeration for which the provider is generated.
        /// </param>
        /// <returns>
        /// A task whose result contains the generated and formatted C# source.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="outputDirectory" /> or <paramref name="enumeration" /> is
        /// <see langword="null" />.
        /// </exception>
        public async Task<string> GenerateEnumerationProviderAsync(
            DirectoryInfo outputDirectory,
            IEnumeration enumeration)
        {
            ArgumentNullException.ThrowIfNull(outputDirectory);
            ArgumentNullException.ThrowIfNull(enumeration);

            var generatedFile = this.RenderEnumerationProvider(enumeration);

            await WriteAsync([generatedFile], outputDirectory);

            return generatedFile.Source;
        }

        /// <inheritdoc />
        protected override void RegisterHelpers()
        {
            this.Handlebars.RegisterEnumerationLiteralHelper();
            this.Handlebars.RegisterNamedElementHelper();
        }

        /// <inheritdoc />
        protected override void RegisterTemplates()
        {
            this.RegisterTemplate(TemplateName);
        }

        /// <summary>
        /// Renders one enumeration provider without writing it.
        /// </summary>
        /// <param name="enumeration">
        /// The UML enumeration to render.
        /// </param>
        /// <returns>
        /// The provider filename and formatted source.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="enumeration" /> is <see langword="null" />.
        /// </exception>
        private GeneratedFile RenderEnumerationProvider(IEnumeration enumeration)
        {
            ArgumentNullException.ThrowIfNull(enumeration);

            var generatedCode = this.Templates[TemplateName](enumeration);
            generatedCode = this.CodeCleanup(generatedCode);

            return new GeneratedFile(
                $"{enumeration.Name}Provider.cs",
                generatedCode);
        }
    }
}