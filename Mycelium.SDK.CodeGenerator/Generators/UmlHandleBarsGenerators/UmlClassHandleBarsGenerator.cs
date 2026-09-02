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
    /// Base class for UML generators that produce interfaces and concrete class implementations.
    /// </summary>
    public abstract class UmlClassHandleBarsGenerator : UmlHandleBarsGenerator
    {
        /// <summary>
        /// Gets the artifact family name used in generation errors.
        /// </summary>
        /// <value>
        /// The artifact family name.
        /// </value>
        protected abstract string ArtifactName { get; }

        /// <summary>
        /// Gets the registered template name used for concrete classes.
        /// </summary>
        /// <value>
        /// The concrete-class template name.
        /// </value>
        protected abstract string ClassTemplate { get; }

        /// <summary>
        /// Gets the registered template name used for interfaces.
        /// </summary>
        /// <value>
        /// The interface template name.
        /// </value>
        protected abstract string InterfaceTemplate { get; }

        /// <inheritdoc />
        public sealed override async Task GenerateAsync(
            XmiReaderResult xmiReaderResult,
            DirectoryInfo outputDirectory)
        {
            ArgumentNullException.ThrowIfNull(xmiReaderResult);
            ArgumentNullException.ThrowIfNull(outputDirectory);

            var payload = CreateHandlebarsPayload(xmiReaderResult);

            this.ConfigureDocumentationSymbols(payload);

            var generatedFiles = payload.Classes
                .Select(this.RenderInterface)
                .Concat(
                    payload.Classes
                        .Where(umlClass => !umlClass.IsAbstract)
                        .Select(this.RenderClass))
                .OrderBy(
                    generatedFile => generatedFile.FileName,
                    StringComparer.Ordinal)
                .ToArray();

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
        /// A task whose result contains the generated and formatted source.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="outputDirectory" /> or <paramref name="umlClass" /> is
        /// <see langword="null" />.
        /// </exception>
        protected async Task<string> GenerateInterfaceAsync(
            DirectoryInfo outputDirectory,
            IClass umlClass)
        {
            ArgumentNullException.ThrowIfNull(outputDirectory);
            ArgumentNullException.ThrowIfNull(umlClass);

            this.ClearDocumentationSymbols();

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
        /// A task whose result contains the generated and formatted source.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="outputDirectory" /> or <paramref name="umlClass" /> is
        /// <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when <paramref name="umlClass" /> is abstract or unnamed.
        /// </exception>
        protected async Task<string> GenerateClassAsync(
            DirectoryInfo outputDirectory,
            IClass umlClass)
        {
            ArgumentNullException.ThrowIfNull(outputDirectory);
            ArgumentNullException.ThrowIfNull(umlClass);

            this.ClearDocumentationSymbols();

            var generatedFile = this.RenderClass(umlClass);

            await WriteAsync([generatedFile], outputDirectory);

            return generatedFile.Source;
        }

        /// <summary>
        /// Registers the helpers that differ between artifact families.
        /// </summary>
        protected abstract void RegisterArtifactHelpers();

        /// <inheritdoc />
        protected sealed override void RegisterHelpers()
        {
            this.Handlebars.RegisterDocumentationHelper(
                this.ResolveDocumentationCref);

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
        /// The generated filename and formatted source.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="umlClass" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when <paramref name="umlClass" /> is unnamed.
        /// </exception>
        private GeneratedFile RenderInterface(IClass umlClass)
        {
            ArgumentNullException.ThrowIfNull(umlClass);

            var className = QueryRequiredClassName(umlClass);
            var generatedCode =
                this.Templates[this.InterfaceTemplate](umlClass);

            generatedCode = this.CodeCleanup(generatedCode);

            return new GeneratedFile(
                $"I{className}.cs",
                generatedCode);
        }

        /// <summary>
        /// Renders a concrete implementation for the specified UML class.
        /// </summary>
        /// <param name="umlClass">
        /// The UML class to render.
        /// </param>
        /// <returns>
        /// The generated filename and formatted source.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="umlClass" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when <paramref name="umlClass" /> is abstract or unnamed.
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
            var generatedCode =
                this.Templates[this.ClassTemplate](umlClass);

            generatedCode = this.CodeCleanup(generatedCode);

            return new GeneratedFile(
                $"{className}.cs",
                generatedCode);
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
        /// Thrown when <paramref name="umlClass" /> is unnamed.
        /// </exception>
        private static string QueryRequiredClassName(IClass umlClass)
        {
            if (string.IsNullOrWhiteSpace(umlClass.Name))
            {
                throw new InvalidOperationException(
                    $"Class '{umlClass.XmiId}' has no name.");
            }

            return umlClass.Name;
        }
    }
}
