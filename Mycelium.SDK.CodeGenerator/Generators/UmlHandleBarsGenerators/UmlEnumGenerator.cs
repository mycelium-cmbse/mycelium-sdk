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

    using DocumentationHelper = Mycelium.SDK.CodeGenerator.HandleBarHelpers.DocumentationHelper;
    using NamedElementHelper = Mycelium.SDK.CodeGenerator.HandleBarHelpers.NamedElementHelper;

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
        public override async Task GenerateAsync(XmiReaderResult xmiReaderResult, DirectoryInfo outputDirectory)
        {
            ArgumentNullException.ThrowIfNull(xmiReaderResult);
            ArgumentNullException.ThrowIfNull(outputDirectory);

            var payload = CreateHandlebarsPayload(xmiReaderResult);

            outputDirectory.Create();

            foreach (var enumeration in payload.Enumerations)
            {
                await this.GenerateEnumerationAsync(outputDirectory, enumeration);
            }
        }

        /// <summary>
        /// Generates one enumeration.
        /// </summary>
        /// <returns>
        /// The generated and formatted C# source.
        /// </returns>
        public async Task<string> GenerateEnumerationAsync(DirectoryInfo outputDirectory, IEnumeration enumeration)
        {
            ArgumentNullException.ThrowIfNull(outputDirectory);
            ArgumentNullException.ThrowIfNull(enumeration);

            outputDirectory.Create();

            var template = this.Templates[TemplateName];
            var generatedCode = template(enumeration);

            generatedCode = this.CodeCleanup(generatedCode);

            var fileName = $"{enumeration.Name}.cs";

            await WriteAsync(generatedCode, outputDirectory, fileName);

            return generatedCode;
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
