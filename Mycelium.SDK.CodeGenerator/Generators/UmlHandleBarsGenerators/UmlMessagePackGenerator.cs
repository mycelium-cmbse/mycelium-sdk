// ------------------------------------------------------------------------------------------------
//  <copyright file="UmlMessagePackGenerator.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Generators.UmlHandleBarsGenerators
{
    using Mycelium.SDK.CodeGenerator.Extensions;
    using Mycelium.SDK.CodeGenerator.HandleBarHelpers;

    using uml4net.StructuredClassifiers;
    using uml4net.xmi.Readers;

    /// <summary>
    /// Generates deterministic MessagePack formatters for concrete FunctionalData DTOs
    /// and their exact-type formatter lookup.
    /// </summary>
    public sealed class UmlMessagePackGenerator : UmlHandleBarsGenerator
    {
        /// <summary>
        /// The concrete DTO formatter template name.
        /// </summary>
        private const string FormatterTemplateName = "dto-messagepackformatter-uml-template";

        /// <summary>
        /// The exact-type formatter lookup template name.
        /// </summary>
        private const string ResolverTemplateName = "messagepack-dataresolver-getformatter-helper";

        /// <summary>
        /// Generates the complete MessagePack formatter family from the supplied model.
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
                .Select(this.RenderFormatter)
                .Append(this.RenderResolver(concreteClasses))
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
            this.Handlebars.RegisterMessagePackFormatterPropertyHelper();

            NamedElementHelper.RegisterNamedElementHelper(this.Handlebars);
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
            this.RegisterTemplate(FormatterTemplateName);
            this.RegisterTemplate(ResolverTemplateName);
        }

        /// <summary>
        /// Renders one concrete DTO MessagePack formatter without writing it.
        /// </summary>
        /// <param name="umlClass">
        /// The concrete UML class to render.
        /// </param>
        /// <returns>
        /// The formatter filename and formatted source.
        /// </returns>
        private GeneratedFile RenderFormatter(IClass umlClass)
        {
            var properties = umlClass.QueryDtoImplementationProperties();

            var templatePayload = new
            {
                Class = umlClass,
                Properties = properties,
                PropertyCount = properties.Count
            };

            var generatedCode = this.Templates[FormatterTemplateName](templatePayload);

            generatedCode = this.CodeCleanup(generatedCode);

            return new GeneratedFile($"{umlClass.Name}MessagePackFormatter.cs", generatedCode);
        }

        /// <summary>
        /// Renders the generated exact-type formatter lookup without writing it.
        /// </summary>
        /// <param name="concreteClasses">
        /// The complete deterministically ordered concrete class selection.
        /// </param>
        /// <returns>
        /// The resolver lookup filename and formatted source.
        /// </returns>
        private GeneratedFile RenderResolver(IReadOnlyCollection<IClass> concreteClasses)
        {
            var generatedCode = this.Templates[ResolverTemplateName](concreteClasses);

            generatedCode = this.CodeCleanup(generatedCode);

            return new GeneratedFile("DataResolverGetFormatterHelper.cs", generatedCode);
        }
    }
}
