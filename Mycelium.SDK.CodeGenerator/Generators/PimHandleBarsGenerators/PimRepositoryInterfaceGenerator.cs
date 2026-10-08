// ------------------------------------------------------------------------------------------------
//  <copyright file="PimRepositoryInterfaceGenerator.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Generators.PimHandleBarsGenerators
{
    using HandlebarsDotNet.Helpers;

    using Mycelium.SDK.CodeGenerator.Extensions;
    using Mycelium.SDK.CodeGenerator.HandleBarHelpers;

    using uml4net.Packages;
    using uml4net.StructuredClassifiers;

    /// <summary>
    /// Generates the repository interfaces of the Systems Modeling API and Services PIM.
    /// </summary>
    /// <remarks>
    /// One interface is generated per concrete specialization of the abstract <c>Record</c> class, which
    /// is what the API model declares a persisted record to be. The classifiers that realize <c>Data</c>
    /// are versioned payload reached through the <c>DataVersion</c> that wraps them, so they have no
    /// repository of their own.
    /// </remarks>
    public sealed class PimRepositoryInterfaceGenerator : HandleBarsGenerator
    {
        /// <summary>
        /// The registered Handlebars template name used for a repository interface.
        /// </summary>
        private const string RepositoryInterfaceTemplateName = "repository-interface-template";

        /// <summary>
        /// The artifact name used in validation messages.
        /// </summary>
        private const string ArtifactName = "Repository interface";

        /// <summary>
        /// Initializes a new instance of the <see cref="PimRepositoryInterfaceGenerator" /> class.
        /// </summary>
        public PimRepositoryInterfaceGenerator() : base("Pim")
        {
            PimParameterExtensions.RegisterPimCSharpTypeMappings();
        }

        /// <summary>
        /// Generates a repository interface for every persisted record of a PIM package.
        /// </summary>
        /// <param name="package">
        /// The PIM package from which the repository interfaces are generated.
        /// </param>
        /// <param name="outputDirectory">
        /// The directory to which the generated interfaces are written.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous generation operation.
        /// </returns>
        /// <remarks>
        /// The complete batch is rendered and validated before the output directory is created, so a model
        /// that cannot be generated leaves no partial output behind.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="package" /> or <paramref name="outputDirectory" /> is
        /// <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the model lacks information required to generate an interface, or when the batch
        /// would produce duplicate filenames.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when a record name cannot be represented as a legal C# identifier.
        /// </exception>
        public async Task GenerateAsync(IPackage package, DirectoryInfo outputDirectory)
        {
            ArgumentNullException.ThrowIfNull(package);
            ArgumentNullException.ThrowIfNull(outputDirectory);

            var generatedFiles = package.QueryRecordClasses()
                .Select(this.RenderRepositoryInterface)
                .OrderBy(generatedFile => generatedFile.FileName, StringComparer.Ordinal)
                .ToArray();

            ThrowIfDuplicateFileNames(generatedFiles, ArtifactName);

            await WriteAsync(generatedFiles, outputDirectory);
        }

        /// <summary>
        /// Generates one repository interface.
        /// </summary>
        /// <param name="recordClass">
        /// The PIM record class from which the interface is generated.
        /// </param>
        /// <param name="outputDirectory">
        /// The directory to which the generated interface is written.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous generation operation. The task result contains the
        /// generated and formatted C# source.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="recordClass" /> or <paramref name="outputDirectory" /> is
        /// <see langword="null" />.
        /// </exception>
        public async Task<string> GenerateRepositoryInterfaceAsync(IClass recordClass, DirectoryInfo outputDirectory)
        {
            ArgumentNullException.ThrowIfNull(recordClass);
            ArgumentNullException.ThrowIfNull(outputDirectory);

            var generatedFile = this.RenderRepositoryInterface(recordClass);

            await WriteAsync([generatedFile], outputDirectory);

            return generatedFile.Source;
        }

        /// <summary>
        /// Registers generator-specific helpers.
        /// </summary>
        /// <remarks>
        /// This method is invoked during base construction. Implementations
        /// must not depend on fields initialized by a derived constructor.
        /// </remarks>
        protected override void RegisterHelpers()
        {
            HandlebarsHelpers.Register(this.Handlebars);

            this.Handlebars.RegisterPimMappingHelper();
            this.Handlebars.RegisterPimRecordHelper();
        }

        /// <summary>
        /// Registers generator-specific templates.
        /// </summary>
        /// <remarks>
        /// This method is invoked during base construction. Implementations
        /// must not depend on fields initialized by a derived constructor.
        /// </remarks>
        protected override void RegisterTemplates() => this.RegisterTemplate(RepositoryInterfaceTemplateName);

        /// <summary>
        /// Renders the repository interface of a single PIM record class.
        /// </summary>
        /// <param name="recordClass">
        /// The PIM record class from which the interface is rendered.
        /// </param>
        /// <returns>
        /// The generated filename and source.
        /// </returns>
        private GeneratedFile RenderRepositoryInterface(IClass recordClass)
        {
            var interfaceName = recordClass.QueryRepositoryInterfaceName();
            var generatedCode = this.Templates[RepositoryInterfaceTemplateName](recordClass);

            generatedCode = this.CodeCleanup(generatedCode);

            return new GeneratedFile($"{interfaceName}.cs", generatedCode);
        }
    }
}
