// ------------------------------------------------------------------------------------------------
//  <copyright file="UmlJsonDtoDeSerializerGenerator.cs" company="Starion Group S.A.">
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

    using uml4net.SimpleClassifiers;
    using uml4net.StructuredClassifiers;
    using uml4net.xmi.Readers;

    /// <summary>
    /// Generates deterministic JSON deserializers for concrete FunctionalData DTOs and
    /// enumerations, together with their exact-discriminator dispatch provider.
    /// </summary>
    public sealed class UmlJsonDtoDeSerializerGenerator : UmlHandleBarsGenerator
    {
        /// <summary>
        /// The concrete DTO deserializer template name.
        /// </summary>
        private const string DtoDeSerializerTemplateName = "json-dto-deserializer-uml-template";

        /// <summary>
        /// The enumeration deserializer template name.
        /// </summary>
        private const string EnumerationDeSerializerTemplateName = "json-enum-deserializer-uml-template";

        /// <summary>
        /// The deserialization-provider template name.
        /// </summary>
        private const string DeSerializationProviderTemplateName = "json-dto-deserialization-provider-uml-template";

        /// <summary>
        /// The per-property DTO deserializer partial-template name.
        /// </summary>
        private const string DtoDeSerializerPartialTemplateName = "json-dto-deserializer-uml-partial-template";

        /// <summary>
        /// Generates the complete deserializer family from the supplied model.
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
        /// Thrown when <paramref name="xmiReaderResult" /> or <paramref name="outputDirectory" />
        /// is <see langword="null" />.
        /// </exception>
        public override async Task GenerateAsync(XmiReaderResult xmiReaderResult, DirectoryInfo outputDirectory)
        {
            ArgumentNullException.ThrowIfNull(xmiReaderResult);
            ArgumentNullException.ThrowIfNull(outputDirectory);

            var payload = CreateHandlebarsPayload(xmiReaderResult);

            var concreteClasses = payload.Classes
                .Where(umlClass => !umlClass.IsAbstract)
                .OrderBy(umlClass => umlClass.Name, StringComparer.Ordinal)
                .ToArray();

            var generatedFiles = concreteClasses
                .Select(this.RenderDtoDeSerializer)
                .Concat(payload.Enumerations.Select(this.RenderEnumerationDeSerializer))
                .Append(this.RenderDeSerializationProvider(concreteClasses))
                .OrderBy(generatedFile => generatedFile.FileName, StringComparer.Ordinal)
                .ToArray();

            await WriteAsync(generatedFiles, outputDirectory);
        }

        /// <summary>
        /// Registers generator-specific helpers.
        /// </summary>
        /// <remarks>
        /// This method is invoked during base construction. Implementations must not depend on fields
        /// initialized by a derived constructor.
        /// </remarks>
        protected override void RegisterHelpers()
        {
            HandlebarsHelpers.Register(this.Handlebars);

            this.Handlebars.RegisterDtoClassHelper();
            this.Handlebars.RegisterJsonSerializerPropertyHelper();
            this.Handlebars.RegisterSafeContextHelper();

            NamedElementHelper.RegisterNamedElementHelper(this.Handlebars);

            uml4net.HandleBars.StringHelper.RegisterStringHelper(this.Handlebars);
            uml4net.HandleBars.PropertyHelper.RegisterPropertyHelper(this.Handlebars);
        }

        /// <summary>
        /// Registers generator-specific templates.
        /// </summary>
        /// <remarks>
        /// This method is invoked during base construction. Implementations must not depend on fields
        /// initialized by a derived constructor.
        /// </remarks>
        protected override void RegisterTemplates()
        {
            this.RegisterTemplate(DtoDeSerializerTemplateName);
            this.RegisterTemplate(EnumerationDeSerializerTemplateName);
            this.RegisterTemplate(DeSerializationProviderTemplateName);
            this.RegisterPartialTemplate(DtoDeSerializerPartialTemplateName);
        }

        /// <summary>
        /// Renders one concrete DTO deserializer without writing it.
        /// </summary>
        /// <param name="umlClass">
        /// The concrete UML class to render.
        /// </param>
        /// <returns>
        /// The generated filename and formatted source.
        /// </returns>
        private GeneratedFile RenderDtoDeSerializer(IClass umlClass)
        {
            var generatedCode = this.Templates[DtoDeSerializerTemplateName](umlClass);

            generatedCode = this.CodeCleanup(generatedCode);

            return new GeneratedFile($"{umlClass.Name}DeSerializer.cs", generatedCode);
        }

        /// <summary>
        /// Renders one enumeration deserializer without writing it.
        /// </summary>
        /// <param name="enumeration">
        /// The UML enumeration to render.
        /// </param>
        /// <returns>
        /// The generated filename and formatted source.
        /// </returns>
        private GeneratedFile RenderEnumerationDeSerializer(IEnumeration enumeration)
        {
            var generatedCode = this.Templates[EnumerationDeSerializerTemplateName](enumeration);

            generatedCode = this.CodeCleanup(generatedCode);

            return new GeneratedFile($"{enumeration.Name}DeSerializer.cs", generatedCode);
        }

        /// <summary>
        /// Renders the exact-discriminator deserialization provider without writing it.
        /// </summary>
        /// <param name="concreteClasses">
        /// The complete deterministically ordered concrete class selection.
        /// </param>
        /// <returns>
        /// The generated provider filename and formatted source.
        /// </returns>
        private GeneratedFile RenderDeSerializationProvider(IReadOnlyCollection<IClass> concreteClasses)
        {
            var generatedCode = this.Templates[DeSerializationProviderTemplateName](concreteClasses);

            generatedCode = this.CodeCleanup(generatedCode);

            return new GeneratedFile("DeSerializationProvider.cs", generatedCode);
        }
    }
}
