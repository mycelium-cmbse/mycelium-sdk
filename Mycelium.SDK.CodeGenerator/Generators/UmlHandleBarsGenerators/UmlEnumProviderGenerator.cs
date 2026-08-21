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
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Mycelium.SDK.CodeGenerator.Extensions;
    using Mycelium.SDK.CodeGenerator.HandleBarHelpers;

    using uml4net.SimpleClassifiers;
    using uml4net.xmi.Readers;

    /// <summary>
    /// Generates exact XMI-literal providers for the reviewed FunctionalData enumerations.
    /// </summary>
    public sealed class UmlEnumProviderGenerator : UmlHandleBarsGenerator
    {
        /// <summary>
        /// The registered Handlebars template name used for enumeration providers.
        /// </summary>
        private const string TemplateName = "enumprovider-uml-template";

        /// <summary>
        /// The complete reviewed enumeration-provider output manifest.
        /// </summary>
        private static readonly string[] ExpectedFileNames =
        [
            "ActivationStatusProvider.cs",
            "CommentStatusProvider.cs",
            "OrganizationMembershipRoleProvider.cs",
            "ProjectLifecycleKindProvider.cs",
            "ProjectMemberRoleProvider.cs",
            "ProjectModeProvider.cs",
            "ProjectVisibilityProvider.cs",
            "ReviewStatusProvider.cs"
        ];

        /// <inheritdoc />
        public override async Task GenerateAsync(XmiReaderResult xmiReaderResult, DirectoryInfo outputDirectory)
        {
            ArgumentNullException.ThrowIfNull(xmiReaderResult);
            ArgumentNullException.ThrowIfNull(outputDirectory);

            var payload = CreateHandlebarsPayload(xmiReaderResult);

            var generatedFiles = PrepareBatch(
                payload.Enumerations.Select(this.RenderEnumerationProvider),
                ExpectedFileNames,
                "Enumeration provider");

            await WriteBatchAsync(generatedFiles, outputDirectory);
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
        /// <exception cref="ArgumentException">
        /// Thrown when a modeled name cannot be represented as a legal C# identifier or the template
        /// renders empty source.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when literal identifiers collide or the rendered or formatted source contains invalid
        /// C# syntax.
        /// </exception>
        public async Task<string> GenerateEnumerationProviderAsync(
            DirectoryInfo outputDirectory,
            IEnumeration enumeration)
        {
            ArgumentNullException.ThrowIfNull(outputDirectory);
            ArgumentNullException.ThrowIfNull(enumeration);

            var generatedFile = this.RenderEnumerationProvider(enumeration);

            await WriteBatchAsync([generatedFile], outputDirectory);

            return generatedFile.Source;
        }

        /// <summary>
        /// Validates and renders one enumeration provider without writing it.
        /// </summary>
        /// <param name="enumeration">
        /// The UML enumeration to render.
        /// </param>
        /// <returns>
        /// The validated provider filename and source.
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
        private GeneratedFile RenderEnumerationProvider(IEnumeration enumeration)
        {
            ArgumentNullException.ThrowIfNull(enumeration);

            var enumerationName = ValidateEnumeration(enumeration);

            _ = ReservedCSharpNameMapper.Map($"{enumerationName}Provider");

            var generatedCode = this.Templates[TemplateName](enumeration);

            return this.CreateGeneratedFile($"{enumerationName}Provider.cs", generatedCode);
        }

        /// <inheritdoc />
        protected override void RegisterHelpers()
        {
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
