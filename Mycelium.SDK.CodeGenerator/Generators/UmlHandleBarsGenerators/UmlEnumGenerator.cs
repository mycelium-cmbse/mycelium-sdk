// ------------------------------------------------------------------------------------------------
//  <copyright file="UmlEnumGenerator.cs" company="Starion Group S.A.">
// 
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
// 
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Generators.UmlHandleBarsGenerators
{
    using Mycelium.SDK.CodeGenerator.HandleBarHelpers;

    using uml4net.HandleBars;
    using uml4net.SimpleClassifiers;
    using uml4net.xmi.Readers;

    using DocumentationHelper =
        Mycelium.SDK.CodeGenerator.HandleBarHelpers.DocumentationHelper;

    using NamedElementHelper =
        Mycelium.SDK.CodeGenerator.HandleBarHelpers.NamedElementHelper;

    /// <summary>
    /// Generates the FunctionalData enumerations.
    /// </summary>
    public sealed class UmlEnumGenerator : UmlHandleBarsGenerator
    {
        /// <summary>
        /// The registered Handlebars template name used for UML enumerations.
        /// </summary>
        private const string TemplateName = "enumeration-uml-template";

        /// <inheritdoc />
        public override async Task GenerateAsync(
            XmiReaderResult xmiReaderResult,
            DirectoryInfo outputDirectory)
        {
            ArgumentNullException.ThrowIfNull(xmiReaderResult);
            ArgumentNullException.ThrowIfNull(outputDirectory);

            var payload = CreateHandlebarsPayload(xmiReaderResult);

            var generatedFiles = payload.Enumerations
                .Select(this.RenderEnumeration)
                .OrderBy(generatedFile => generatedFile.FileName, StringComparer.Ordinal)
                .ToArray();

            await WriteAsync(generatedFiles, outputDirectory);
        }

        /// <summary>
        /// Generates one enumeration.
        /// </summary>
        /// <param name="outputDirectory">
        /// The directory to which the generated enumeration is written.
        /// </param>
        /// <param name="enumeration">
        /// The UML enumeration to generate.
        /// </param>
        /// <returns>
        /// A task whose result contains the generated and formatted C# source.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="outputDirectory" /> or <paramref name="enumeration" /> is
        /// <see langword="null" />.
        /// </exception>
        public async Task<string> GenerateEnumerationAsync(
            DirectoryInfo outputDirectory,
            IEnumeration enumeration)
        {
            ArgumentNullException.ThrowIfNull(outputDirectory);
            ArgumentNullException.ThrowIfNull(enumeration);

            var generatedFile = this.RenderEnumeration(enumeration);

            await WriteAsync([generatedFile], outputDirectory);

            return generatedFile.Source;
        }

        /// <summary>
        /// Renders one enumeration without writing it.
        /// </summary>
        /// <param name="enumeration">
        /// The UML enumeration to render.
        /// </param>
        /// <returns>
        /// The generated filename and formatted source.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="enumeration" /> is <see langword="null" />.
        /// </exception>
        private GeneratedFile RenderEnumeration(IEnumeration enumeration)
        {
            ArgumentNullException.ThrowIfNull(enumeration);

            var generatedCode = this.Templates[TemplateName](enumeration);
            generatedCode = this.CodeCleanup(generatedCode);

            return new GeneratedFile(
                $"{enumeration.Name}.cs",
                generatedCode);
        }

        /// <inheritdoc />
        protected override void RegisterHelpers()
        {
            DocumentationHelper.RegisterDocumentationHelper(this.Handlebars);
            EnumHelper.RegisterEnumHelper(this.Handlebars);

            this.Handlebars.RegisterEnumerationHelper();
            this.Handlebars.RegisterEnumerationLiteralHelper();

            NamedElementHelper.RegisterNamedElementHelper(this.Handlebars);
        }

        /// <inheritdoc />
        protected override void RegisterTemplates()
        {
            this.RegisterTemplate(TemplateName);
        }
    }
}
