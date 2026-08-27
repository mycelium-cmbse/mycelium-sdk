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

        /// <summary>
        /// The complete reviewed enumeration output manifest.
        /// </summary>
        private static readonly string[] ExpectedFileNames =
        [
            "ActivationStatus.cs",
            "CommentStatus.cs",
            "OrganizationMembershipRole.cs",
            "ProjectLifecycleKind.cs",
            "ProjectMemberRole.cs",
            "ProjectMode.cs",
            "ProjectVisibility.cs",
            "ReviewStatus.cs"
        ];

        /// <inheritdoc />
        public override async Task GenerateAsync(XmiReaderResult xmiReaderResult, DirectoryInfo outputDirectory)
        {
            ArgumentNullException.ThrowIfNull(xmiReaderResult);
            ArgumentNullException.ThrowIfNull(outputDirectory);

            var payload = CreateHandlebarsPayload(xmiReaderResult);

            var generatedFiles = PrepareBatch(
                payload.Enumerations.Select(this.RenderEnumeration),
                ExpectedFileNames,
                "Enumeration");

            await WriteBatchAsync(generatedFiles, outputDirectory);
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
        /// A task that represents the asynchronous enumeration generation operation. The task result
        /// contains the generated and formatted C# source.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="outputDirectory" /> or <paramref name="enumeration" /> is
        /// <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when the enumeration name or one of its literal names cannot be represented as a legal
        /// C# identifier, or the template renders empty source.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the enumeration or a literal is unnamed, mapped literal identifiers collide, or the
        /// rendered or formatted source contains invalid C# syntax.
        /// </exception>
        public async Task<string> GenerateEnumerationAsync(DirectoryInfo outputDirectory, IEnumeration enumeration)
        {
            ArgumentNullException.ThrowIfNull(outputDirectory);
            ArgumentNullException.ThrowIfNull(enumeration);

            var generatedFile = this.RenderEnumeration(enumeration);

            await WriteBatchAsync([generatedFile], outputDirectory);

            return generatedFile.Source;
        }

        /// <inheritdoc />
        protected override void RegisterHelpers()
        {
            DocumentationHelper.RegisterDocumentationHelper(this.Handlebars);
            this.Handlebars.RegisterEnumHelper();

            this.Handlebars.RegisterEnumerationHelper();
            this.Handlebars.RegisterEnumerationLiteralHelper();

            NamedElementHelper.RegisterNamedElementHelper(this.Handlebars);
        }

        /// <inheritdoc />
        protected override void RegisterTemplates()
        {
            this.RegisterTemplate(TemplateName);
        }

        /// <summary>
        /// Validates and renders one enumeration without writing it.
        /// </summary>
        /// <param name="enumeration">
        /// The UML enumeration to render.
        /// </param>
        /// <returns>
        /// The generated filename and source.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="enumeration" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when a modeled name cannot be represented as a legal C# identifier or the template
        /// renders empty source.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when literal identifiers collide or the rendered or formatted source contains invalid
        /// C# syntax.
        /// </exception>
        private GeneratedFile RenderEnumeration(IEnumeration enumeration)
        {
            ArgumentNullException.ThrowIfNull(enumeration);

            var enumerationName = ValidateEnumeration(enumeration);
            var fileName = $"{enumerationName}.cs";
            var template = this.Templates[TemplateName];
            var generatedCode = template(enumeration);

            return this.CreateGeneratedFile(fileName, generatedCode);
        }
    }
}
