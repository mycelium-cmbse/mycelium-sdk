// ------------------------------------------------------------------------------------------------
//  <copyright file="UmlPocoGenerator.cs" company="Starion Group S.A.">
// 
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
// 
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Generators.UmlHandleBarsGenerators
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Mycelium.SDK.CodeGenerator.HandleBarHelpers;

    using uml4net.StructuredClassifiers;
    using uml4net.xmi.Readers;

    /// <summary>
    /// Generates the FunctionalData POCO interfaces and concrete implementations.
    /// </summary>
    public sealed class UmlPocoGenerator : UmlHandleBarsGenerator
    {
        /// <summary>
        /// The registered Handlebars template name used for concrete POCO classes.
        /// </summary>
        private const string ClassTemplateName = "poco-class-uml-template";
        
        /// <summary>
        /// The registered Handlebars template name used for POCO interfaces.
        /// </summary>
        private const string InterfaceTemplateName = "poco-interface-uml-template";

        /// <inheritdoc />
        public override async Task GenerateAsync(XmiReaderResult xmiReaderResult, DirectoryInfo outputDirectory)
        {
            ArgumentNullException.ThrowIfNull(xmiReaderResult);
            ArgumentNullException.ThrowIfNull(outputDirectory);

            var payload = CreateHandlebarsPayload(xmiReaderResult);

            // Render and validate the complete batch before creating or writing
            // the output directory. This prevents partial output when any model
            // element cannot be generated.
            var generatedFiles = payload.Classes
                .Select(this.RenderPocoInterface)
                .Concat(payload.Classes
                        .Where(umlClass => !umlClass.IsAbstract)
                        .Select(this.RenderPocoClass))
                .OrderBy(generatedFile => generatedFile.FileName, StringComparer.Ordinal)
                .ToArray();

            ThrowIfDuplicateFileNames(generatedFiles);

            outputDirectory.Create();

            foreach (var generatedFile in generatedFiles)
            {
                await WriteAsync(generatedFile.Source, outputDirectory, generatedFile.FileName);
            }
        }

        /// <summary>
        /// Generates one POCO interface.
        /// </summary>
        /// <param name="outputDirectory">
        /// The directory to which the generated POCO interface is written.
        /// </param>
        /// <param name="umlClass">
        /// The UML class for which the POCO interface is generated.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous POCO interface generation operation. The task result
        /// contains the generated and formatted C# source.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="outputDirectory"/> or <paramref name="umlClass"/> is
        /// <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the UML class lacks information required to generate its POCO interface or contains
        /// an unsupported property type.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when a modeled name cannot be represented as a legal C# identifier.
        /// </exception>
        public async Task<string> GeneratePocoInterfaceAsync(DirectoryInfo outputDirectory, IClass umlClass)
        {
            ArgumentNullException.ThrowIfNull(outputDirectory);
            ArgumentNullException.ThrowIfNull(umlClass);

            var generatedFile = this.RenderPocoInterface(umlClass);

            outputDirectory.Create();

            await WriteAsync(generatedFile.Source, outputDirectory, generatedFile.FileName);

            return generatedFile.Source;
        }

        /// <summary>
        /// Generates one concrete POCO implementation.
        /// </summary>
        /// <param name="outputDirectory">
        /// The directory to which the generated POCO implementation is written.
        /// </param>
        /// <param name="umlClass">
        /// The UML class for which the POCO implementation is generated.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous POCO implementation generation operation. The task result
        /// contains the generated and formatted C# source.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="outputDirectory"/> or <paramref name="umlClass"/> is
        /// <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the UML class is abstract, lacks information required to generate its concrete POCO,
        /// or contains an unsupported property type.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when a modeled name cannot be represented as a legal C# identifier.
        /// </exception>
        public async Task<string> GeneratePocoClassAsync(DirectoryInfo outputDirectory, IClass umlClass)
        {
            ArgumentNullException.ThrowIfNull(outputDirectory);
            ArgumentNullException.ThrowIfNull(umlClass);

            var generatedFile = this.RenderPocoClass(umlClass);

            outputDirectory.Create();

            await WriteAsync(generatedFile.Source, outputDirectory, generatedFile.FileName);

            return generatedFile.Source;
        }

        /// <inheritdoc />
        protected override void RegisterHelpers()
        {
            DocumentationHelper.RegisterDocumentationHelper(this.Handlebars);
            ClassHelper.RegisterPocoClassHelper(this.Handlebars);
            PropertyHelper.RegisterPocoPropertyHelper(this.Handlebars);
            NamedElementHelper.RegisterNamedElementHelper(this.Handlebars);
        }

        /// <inheritdoc />
        protected override void RegisterTemplates()
        {
            this.RegisterTemplate(ClassTemplateName);
            this.RegisterTemplate(InterfaceTemplateName);
        }

        /// <summary>
        /// Renders a POCO interface for the specified UML class.
        /// </summary>
        /// <param name="umlClass">
        /// The UML class for which the POCO interface is rendered.
        /// </param>
        /// <returns>
        /// The generated filename and formatted C# source.
        /// </returns>
        private GeneratedFile RenderPocoInterface(IClass umlClass)
        {
            ArgumentNullException.ThrowIfNull(umlClass);

            var className = QueryRequiredClassName(umlClass);
            var generatedCode = this.Templates[InterfaceTemplateName](umlClass);

            generatedCode = this.CodeCleanup(generatedCode);

            return new GeneratedFile($"I{className}.cs", generatedCode);
        }

        /// <summary>
        /// Renders a concrete POCO implementation for the specified UML class.
        /// </summary>
        /// <param name="umlClass">
        /// The UML class for which the concrete POCO is rendered.
        /// </param>
        /// <returns>
        /// The generated filename and formatted C# source.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the UML class is abstract or has no name.
        /// </exception>
        private GeneratedFile RenderPocoClass(IClass umlClass)
        {
            ArgumentNullException.ThrowIfNull(umlClass);

            if (umlClass.IsAbstract)
            {
                throw new InvalidOperationException(
                    $"Cannot generate a concrete POCO implementation for abstract class "
                    + $"'{QueryRequiredClassName(umlClass)}'.");
            }

            var className = QueryRequiredClassName(umlClass);
            var generatedCode = this.Templates[ClassTemplateName](umlClass);

            generatedCode = this.CodeCleanup(generatedCode);

            return new GeneratedFile($"{className}.cs", generatedCode);
        }

        /// <summary>
        /// Queries the required name of a UML class.
        /// </summary>
        /// <param name="umlClass">
        /// The UML class whose name is queried.
        /// </param>
        /// <returns>
        /// The UML class name.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the UML class has no name.
        /// </exception>
        private static string QueryRequiredClassName(IClass umlClass)
        {
            if (string.IsNullOrWhiteSpace(umlClass.Name))
            {
                throw new InvalidOperationException($"Class '{umlClass.XmiId}' has no name.");
            }

            return umlClass.Name;
        }

        /// <summary>
        /// Verifies that a generated batch contains no duplicate filenames.
        /// </summary>
        /// <param name="generatedFiles">
        /// The generated files to validate.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when multiple generated files have the same filename.
        /// </exception>
        private static void ThrowIfDuplicateFileNames(IReadOnlyCollection<GeneratedFile> generatedFiles)
        {
            var duplicateFileName = generatedFiles
                .GroupBy(generatedFile => generatedFile.FileName, StringComparer.Ordinal)
                .FirstOrDefault(group => group.Count() > 1)
                ?.Key;

            if (duplicateFileName is not null)
            {
                throw new InvalidOperationException($"POCO generation produced duplicate filename " + $"'{duplicateFileName}'.");
            }
        }

        /// <summary>
        /// Represents one generated source file.
        /// </summary>
        /// <param name="FileName">
        /// The generated filename.
        /// </param>
        /// <param name="Source">
        /// The formatted generated C# source.
        /// </param>
        private sealed record GeneratedFile(string FileName, string Source);
    }
}