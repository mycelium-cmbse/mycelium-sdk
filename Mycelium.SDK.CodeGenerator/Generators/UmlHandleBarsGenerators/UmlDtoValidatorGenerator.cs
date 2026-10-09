// ------------------------------------------------------------------------------------------------
//  <copyright file="UmlDtoValidatorGenerator.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Generators.UmlHandleBarsGenerators
{
    using Mycelium.SDK.CodeGenerator.HandleBarHelpers;

    using uml4net.StructuredClassifiers;
    using uml4net.xmi.Readers;

    /// <summary>
    /// Generates deterministic local data validators for concrete Fabric DTOs.
    /// </summary>
    public sealed class UmlDtoValidatorGenerator : UmlHandleBarsGenerator
    {
        /// <summary>
        /// The DTO validator template name.
        /// </summary>
        private const string ValidatorTemplateName = "dto-validator-uml-template";

        /// <summary>
        /// Initializes a new instance of the <see cref="UmlDtoValidatorGenerator" /> class
        /// and registers its helpers and template.
        /// </summary>
        /// <exception cref="FileNotFoundException">
        /// Thrown when the DTO validator template is missing from the template directory.
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">
        /// Thrown when the template directory does not exist.
        /// </exception>
        public UmlDtoValidatorGenerator()
        {
        }

        /// <summary>
        /// Generates one data validator for every concrete DTO in the loaded Fabric model.
        /// </summary>
        /// <param name="xmiReaderResult">
        /// The parsed UML model used for generation.
        /// </param>
        /// <param name="outputDirectory">
        /// The directory to which the complete rendered validator batch is written.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous generation operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="xmiReaderResult" /> or <paramref name="outputDirectory" /> is
        /// <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the model does not contain exactly one Fabric package or a property cannot
        /// be mapped to its DTO representation.
        /// </exception>
        public override async Task GenerateAsync(XmiReaderResult xmiReaderResult, DirectoryInfo outputDirectory)
        {
            ArgumentNullException.ThrowIfNull(xmiReaderResult);
            ArgumentNullException.ThrowIfNull(outputDirectory);

            var payload = CreateHandlebarsPayload(xmiReaderResult);

            var generatedFiles = QueryConcreteClasses(payload.Classes)
                .Select(this.RenderValidator)
                .OrderBy(generatedFile => generatedFile.FileName, StringComparer.Ordinal)
                .ToArray();

            await WriteAsync(generatedFiles, outputDirectory);
        }

        /// <summary>
        /// Registers the class, property and identifier helpers used by the validator template.
        /// </summary>
        /// <remarks>
        /// This method is invoked during base construction and uses no derived instance fields.
        /// </remarks>
        protected override void RegisterHelpers()
        {
            this.Handlebars.RegisterDtoClassHelper();
            this.Handlebars.RegisterDtoValidatorPropertyHelper();
            this.Handlebars.RegisterNamedElementHelper();
        }

        /// <summary>
        /// Loads and registers the DTO validator template.
        /// </summary>
        /// <remarks>
        /// This method is invoked during base construction and uses no derived instance fields.
        /// </remarks>
        /// <exception cref="FileNotFoundException">
        /// Thrown when the DTO validator template is missing from the template directory.
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">
        /// Thrown when the template directory does not exist.
        /// </exception>
        protected override void RegisterTemplates()
        {
            this.RegisterTemplate(ValidatorTemplateName);
        }

        /// <summary>
        /// Renders and formats one concrete DTO validator without writing it.
        /// </summary>
        /// <param name="umlClass">
        /// The concrete UML class whose DTO is validated.
        /// </param>
        /// <returns>
        /// The validator filename and rendered source.
        /// </returns>
        private GeneratedFile RenderValidator(IClass umlClass)
        {
            var generatedCode = this.Templates[ValidatorTemplateName](umlClass);

            generatedCode = this.CodeCleanup(generatedCode);

            return new GeneratedFile($"{umlClass.Name}Validator.cs", generatedCode);
        }
    }
}
