// ------------------------------------------------------------------------------------------------
//  <copyright file="UmlClassHandleBarsGenerator.cs" company="Starion Group S.A.">
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
    /// Base class for UML generators that produce interfaces and concrete
    /// class implementations.
    /// </summary>
    public abstract class UmlClassHandleBarsGenerator : UmlHandleBarsGenerator
    {
        /// <summary>
        /// Gets the artifact name used in validation messages.
        /// </summary>
        protected abstract string ArtifactName { get; }

        /// <summary>
        /// Gets the registered template name used for concrete classes.
        /// </summary>
        /// <remarks>
        /// The implementation must not depend on derived-constructor state because
        /// templates are registered during base construction.
        /// </remarks>
        protected abstract string ClassTemplate { get; }

        /// <summary>
        /// Gets the registered template name used for interfaces.
        /// </summary>
        /// <remarks>
        /// The implementation must not depend on derived-constructor state because
        /// templates are registered during base construction.
        /// </remarks>
        protected abstract string InterfaceTemplate { get; }

        /// <inheritdoc />
        public sealed override async Task GenerateAsync(XmiReaderResult xmiReaderResult, DirectoryInfo outputDirectory)
        {
            ArgumentNullException.ThrowIfNull(xmiReaderResult);
            ArgumentNullException.ThrowIfNull(outputDirectory);

            var payload = CreateHandlebarsPayload(xmiReaderResult);

            // Render and validate the complete batch before creating or writing
            // the output directory. This prevents partial output when any model
            // element cannot be generated.
            var generatedFiles = payload.Classes
                .Select(this.RenderInterface)
                .Concat(payload.Classes
                    .Where(umlClass => !umlClass.IsAbstract)
                    .Select(this.RenderClass))
                .OrderBy(generatedFile => generatedFile.FileName, StringComparer.Ordinal)
                .ToArray();

            ThrowIfDuplicateFileNames(generatedFiles, this.ArtifactName);

            await WriteAsync(generatedFiles, outputDirectory);
        }

        /// <summary>
        /// Generates one interface.
        /// </summary>
        /// <param name="outputDirectory">
        /// The directory to which the generated interface is written.
        /// </param>
        /// <param name="umlClass">
        /// The UML class for which the interface is generated.
        /// </param>
        /// <returns>
        /// A task whose result contains the generated and formatted C# source.
        /// </returns>
        protected async Task<string> GenerateInterfaceAsync(DirectoryInfo outputDirectory, IClass umlClass)
        {
            ArgumentNullException.ThrowIfNull(outputDirectory);
            ArgumentNullException.ThrowIfNull(umlClass);

            var generatedFile = this.RenderInterface(umlClass);

            await WriteAsync([generatedFile], outputDirectory);

            return generatedFile.Source;
        }

        /// <summary>
        /// Generates one concrete class implementation.
        /// </summary>
        /// <param name="outputDirectory">
        /// The directory to which the generated class is written.
        /// </param>
        /// <param name="umlClass">
        /// The UML class for which the implementation is generated.
        /// </param>
        /// <returns>
        /// A task whose result contains the generated and formatted C# source.
        /// </returns>
        protected async Task<string> GenerateClassAsync(DirectoryInfo outputDirectory, IClass umlClass)
        {
            ArgumentNullException.ThrowIfNull(outputDirectory);
            ArgumentNullException.ThrowIfNull(umlClass);

            var generatedFile = this.RenderClass(umlClass);

            await WriteAsync([generatedFile], outputDirectory);

            return generatedFile.Source;
        }

        /// <summary>
        /// Registers the helpers that differ between artifact types.
        /// </summary>
        /// <remarks>
        /// This method is invoked during base construction and must not depend
        /// on fields initialized by a derived constructor.
        /// </remarks>
        protected abstract void RegisterArtifactHelpers();

        /// <inheritdoc />
        protected sealed override void RegisterHelpers()
        {
            DocumentationHelper.RegisterDocumentationHelper(this.Handlebars);
            this.RegisterArtifactHelpers();
            NamedElementHelper.RegisterNamedElementHelper(this.Handlebars);
        }

        /// <inheritdoc />
        protected sealed override void RegisterTemplates()
        {
            this.RegisterTemplate(this.ClassTemplate);
            this.RegisterTemplate(this.InterfaceTemplate);
        }

        /// <summary>
        /// Renders an interface for the specified UML class.
        /// </summary>
        /// <param name="umlClass">
        /// The UML class to render.
        /// </param>
        /// <returns>
        /// The generated filename and source.
        /// </returns>
        private GeneratedFile RenderInterface(IClass umlClass)
        {
            ArgumentNullException.ThrowIfNull(umlClass);

            var className = QueryRequiredClassName(umlClass);
            var generatedCode = this.Templates[this.InterfaceTemplate](umlClass);

            generatedCode = this.CodeCleanup(generatedCode);

            return new GeneratedFile($"I{className}.cs", generatedCode);
        }

        /// <summary>
        /// Renders a concrete implementation for the specified UML class.
        /// </summary>
        /// <param name="umlClass">
        /// The UML class to render.
        /// </param>
        /// <returns>
        /// The generated filename and source.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the UML class is abstract or has no name.
        /// </exception>
        private GeneratedFile RenderClass(IClass umlClass)
        {
            ArgumentNullException.ThrowIfNull(umlClass);

            if (umlClass.IsAbstract)
            {
                throw new InvalidOperationException(
                    $"Cannot generate a concrete {this.ArtifactName} implementation for abstract class "
                    + $"'{QueryRequiredClassName(umlClass)}'.");
            }

            var className = QueryRequiredClassName(umlClass);
            var generatedCode = this.Templates[this.ClassTemplate](umlClass);

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

    }
}