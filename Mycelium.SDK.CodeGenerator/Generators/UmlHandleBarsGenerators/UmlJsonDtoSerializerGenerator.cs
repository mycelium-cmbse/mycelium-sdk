// ------------------------------------------------------------------------------------------------
//  <copyright file="UmlJsonDtoSerializerGenerator.cs" company="Starion Group S.A.">
// 
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
// 
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Generators.UmlHandleBarsGenerators
{
    using HandlebarsDotNet.Helpers;

    using Mycelium.SDK.CodeGenerator.HandleBarHelpers;

    using uml4net.StructuredClassifiers;
    using uml4net.xmi.Readers;

    /// <summary>
    /// Generates deterministic JSON serializers for concrete FunctionalData DTOs
    /// and their exact-runtime-type dispatch provider.
    /// </summary>
    public sealed class UmlJsonDtoSerializerGenerator : UmlHandleBarsGenerator
    {
        /// <summary>
        /// The concrete DTO serializer template name.
        /// </summary>
        private const string SerializerTemplateName =
            "json-dto-serializer-uml-template";

        /// <summary>
        /// The serialization-provider template name.
        /// </summary>
        private const string SerializationProviderTemplateName =
            "json-dto-serialization-provider-uml-template";

        /// <summary>
        /// The per-property serializer partial-template name.
        /// </summary>
        private const string SerializerPartialTemplateName =
            "json-dto-serializer-uml-partial-template";

        /// <inheritdoc />
        public override async Task GenerateAsync(
            XmiReaderResult xmiReaderResult,
            DirectoryInfo outputDirectory)
        {
            ArgumentNullException.ThrowIfNull(xmiReaderResult);
            ArgumentNullException.ThrowIfNull(outputDirectory);

            var payload =
                CreateHandlebarsPayload(xmiReaderResult);

            var concreteClasses = payload.Classes
                .Where(umlClass => !umlClass.IsAbstract)
                .OrderBy(
                    umlClass => umlClass.Name,
                    StringComparer.Ordinal)
                .ToArray();

            var generatedFiles = concreteClasses
                .Select(this.RenderSerializer)
                .Append(
                    this.RenderSerializationProvider(concreteClasses))
                .OrderBy(
                    generatedFile => generatedFile.FileName,
                    StringComparer.Ordinal)
                .ToArray();

            ThrowIfDuplicateFileNames(
                generatedFiles,
                "JSON DTO serializer");

            await WriteAsync(
                generatedFiles,
                outputDirectory);
        }

        /// <inheritdoc />
        protected override void RegisterHelpers()
        {
            HandlebarsHelpers.Register(this.Handlebars);

            this.Handlebars.RegisterDtoClassHelper();
            this.Handlebars.RegisterJsonSerializerPropertyHelper();
            this.Handlebars.RegisterSafeContextHelper();

            NamedElementHelper.RegisterNamedElementHelper(
                this.Handlebars);

            uml4net.HandleBars.StringHelper.RegisterStringHelper(
                this.Handlebars);

            uml4net.HandleBars.PropertyHelper.RegisterPropertyHelper(
                this.Handlebars);
        }

        /// <inheritdoc />
        protected override void RegisterTemplates()
        {
            this.RegisterTemplate(SerializerTemplateName);
            this.RegisterTemplate(SerializationProviderTemplateName);
            this.RegisterPartialTemplate(SerializerPartialTemplateName);
        }

        /// <summary>
        /// Renders one concrete DTO serializer without writing it.
        /// </summary>
        /// <param name="umlClass">
        /// The concrete UML class to render.
        /// </param>
        /// <returns>
        /// The serializer filename and formatted source.
        /// </returns>
        private GeneratedFile RenderSerializer(IClass umlClass)
        {
            var generatedCode =
                this.Templates[SerializerTemplateName](umlClass);

            generatedCode =
                this.CodeCleanup(generatedCode);

            return new GeneratedFile(
                $"{umlClass.Name}Serializer.cs",
                generatedCode);
        }

        /// <summary>
        /// Renders the exact-runtime-type serialization provider without writing it.
        /// </summary>
        /// <param name="concreteClasses">
        /// The complete deterministically ordered concrete class selection.
        /// </param>
        /// <returns>
        /// The provider filename and formatted source.
        /// </returns>
        private GeneratedFile RenderSerializationProvider(
            IReadOnlyCollection<IClass> concreteClasses)
        {
            var generatedCode =
                this.Templates[SerializationProviderTemplateName](
                    concreteClasses);

            generatedCode =
                this.CodeCleanup(generatedCode);

            return new GeneratedFile(
                "SerializationProvider.cs",
                generatedCode);
        }
    }
}
