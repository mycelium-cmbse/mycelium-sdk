// ------------------------------------------------------------------------------------------------
//  <copyright file="OpenApiCarterModuleGenerator.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Generators.OpenApiHandleBarsGenerators
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.OpenApi;

    using Mycelium.SDK.CodeGenerator.Extensions;
    using Mycelium.SDK.CodeGenerator.HandleBarHelpers;

    /// <summary>
    /// Generates the Carter modules that register the routes of an OpenAPI document.
    /// </summary>
    /// <remarks>
    /// One module is generated per OpenAPI tag. Only the route registration is generated: each route is
    /// mapped onto a hand-written handler declared on a companion partial, so that handler signatures
    /// stay under the control of their author.
    /// </remarks>
    public sealed class OpenApiCarterModuleGenerator : HandleBarsGenerator
    {
        /// <summary>
        /// The registered Handlebars template name used for a Carter module.
        /// </summary>
        private const string ModuleTemplateName = "carter-module-template";

        /// <summary>
        /// The artifact name used in validation messages.
        /// </summary>
        private const string ArtifactName = "Carter module";

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenApiCarterModuleGenerator"/> class.
        /// </summary>
        public OpenApiCarterModuleGenerator() : base("OpenApi")
        {
        }

        /// <summary>
        /// Generates a Carter module for every tag of an OpenAPI document.
        /// </summary>
        /// <param name="document">
        /// The OpenAPI document from which the routes are generated.
        /// </param>
        /// <param name="outputDirectory">
        /// The directory to which the generated modules are written.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous generation operation.
        /// </returns>
        /// <remarks>
        /// The complete batch is rendered and validated before the output directory is created, so a
        /// document that cannot be generated leaves no partial output behind.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="document"/> or <paramref name="outputDirectory"/> is
        /// <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the document lacks information required to generate a module, or when the batch
        /// would produce duplicate filenames.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when a tag or operation identifier cannot be represented as a legal C# identifier.
        /// </exception>
        public async Task GenerateAsync(OpenApiDocument document, DirectoryInfo outputDirectory)
        {
            ArgumentNullException.ThrowIfNull(document);
            ArgumentNullException.ThrowIfNull(outputDirectory);

            var generatedFiles = document.QueryOperationGroups()
                .Select(this.RenderModule)
                .OrderBy(generatedFile => generatedFile.FileName, StringComparer.Ordinal)
                .ToArray();

            ThrowIfDuplicateFileNames(generatedFiles, ArtifactName);

            await WriteBatchAsync(generatedFiles, outputDirectory);
        }

        /// <summary>
        /// Generates one Carter module.
        /// </summary>
        /// <param name="outputDirectory">
        /// The directory to which the generated module is written.
        /// </param>
        /// <param name="operations">
        /// The operations of a single tag, keyed by that tag.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous generation operation. The task result contains the
        /// generated and formatted C# source.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="outputDirectory"/> or <paramref name="operations"/> is
        /// <see langword="null" />.
        /// </exception>
        public async Task<string> GenerateCarterModuleAsync(
            DirectoryInfo outputDirectory,
            IGrouping<string, SearchResult> operations)
        {
            ArgumentNullException.ThrowIfNull(outputDirectory);
            ArgumentNullException.ThrowIfNull(operations);

            var generatedFile = this.RenderModule(operations);

            await WriteBatchAsync([generatedFile], outputDirectory);

            return generatedFile.Source;
        }

        /// <inheritdoc />
        protected override void RegisterHelpers()
        {
            this.Handlebars.RegisterOpenApiTagHelper();
            this.Handlebars.RegisterOpenApiOperationHelper();
        }

        /// <inheritdoc />
        protected override void RegisterTemplates()
        {
            this.RegisterTemplate(ModuleTemplateName);
        }

        /// <summary>
        /// Renders the Carter module of a single tag.
        /// </summary>
        /// <param name="operations">
        /// The operations of a single tag, keyed by that tag.
        /// </param>
        /// <returns>
        /// The generated filename and source.
        /// </returns>
        private GeneratedFile RenderModule(IGrouping<string, SearchResult> operations)
        {
            var moduleName = operations.Key.QueryModuleName();
            var generatedCode = this.Templates[ModuleTemplateName](operations);

            generatedCode = this.CodeCleanup(generatedCode);

            return new GeneratedFile($"{moduleName}.cs", generatedCode);
        }

    }
}
