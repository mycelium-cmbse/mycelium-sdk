// ------------------------------------------------------------------------------------------------
//  <copyright file="PimServiceInterfaceGenerator.cs" company="Starion Group S.A.">
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
    /// Generates the service interfaces of the Systems Modeling API and Services PIM.
    /// </summary>
    /// <remarks>
    /// One interface is generated per PIM service class. Every operation the PIM declares is emitted,
    /// including those that the REST binding does not expose, so the generated contract is the platform
    /// independent model rather than a projection of the wire format.
    /// </remarks>
    public sealed class PimServiceInterfaceGenerator : HandleBarsGenerator
    {
        /// <summary>
        /// The registered Handlebars template name used for a service interface.
        /// </summary>
        private const string ServiceInterfaceTemplateName = "service-interface-template";

        /// <summary>
        /// The artifact name used in validation messages.
        /// </summary>
        private const string ArtifactName = "Service interface";

        /// <summary>
        /// Initializes a new instance of the <see cref="PimServiceInterfaceGenerator" /> class.
        /// </summary>
        public PimServiceInterfaceGenerator() : base("Pim")
        {
            PimParameterExtensions.RegisterPimCSharpTypeMappings();
        }

        /// <summary>
        /// Generates a service interface for every service class of a PIM package.
        /// </summary>
        /// <param name="package">
        /// The PIM package from which the service interfaces are generated.
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
        /// Thrown when a service, operation or parameter name cannot be represented as a legal C#
        /// identifier.
        /// </exception>
        public async Task GenerateAsync(IPackage package, DirectoryInfo outputDirectory)
        {
            ArgumentNullException.ThrowIfNull(package);
            ArgumentNullException.ThrowIfNull(outputDirectory);

            var generatedFiles = package.QueryServiceClasses()
                .Select(this.RenderServiceInterface)
                .OrderBy(generatedFile => generatedFile.FileName, StringComparer.Ordinal)
                .ToArray();

            ThrowIfDuplicateFileNames(generatedFiles, ArtifactName);

            await WriteAsync(generatedFiles, outputDirectory);
        }

        /// <summary>
        /// Generates one service interface.
        /// </summary>
        /// <param name="serviceClass">
        /// The PIM service class from which the interface is generated.
        /// </param>
        /// <param name="outputDirectory">
        /// The directory to which the generated interface is written.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous generation operation. The task result contains the
        /// generated and formatted C# source.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="serviceClass" /> or <paramref name="outputDirectory" /> is
        /// <see langword="null" />.
        /// </exception>
        public async Task<string> GenerateServiceInterfaceAsync(IClass serviceClass, DirectoryInfo outputDirectory)
        {
            ArgumentNullException.ThrowIfNull(serviceClass);
            ArgumentNullException.ThrowIfNull(outputDirectory);

            var generatedFile = this.RenderServiceInterface(serviceClass);

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

            this.Handlebars.RegisterDocumentationHelper();
            this.Handlebars.RegisterPimMappingHelper();
            this.Handlebars.RegisterPimServiceHelper();
            this.Handlebars.RegisterPimOperationHelper();
            this.Handlebars.RegisterPimParameterHelper();
        }

        /// <summary>
        /// Registers generator-specific templates.
        /// </summary>
        /// <remarks>
        /// This method is invoked during base construction. Implementations
        /// must not depend on fields initialized by a derived constructor.
        /// </remarks>
        protected override void RegisterTemplates() => this.RegisterTemplate(ServiceInterfaceTemplateName);

        /// <summary>
        /// Renders the service interface of a single PIM service class.
        /// </summary>
        /// <param name="serviceClass">
        /// The PIM service class from which the interface is rendered.
        /// </param>
        /// <returns>
        /// The generated filename and source.
        /// </returns>
        private GeneratedFile RenderServiceInterface(IClass serviceClass)
        {
            var interfaceName = serviceClass.QueryServiceInterfaceName();
            var generatedCode = this.Templates[ServiceInterfaceTemplateName](serviceClass);

            generatedCode = this.CodeCleanup(generatedCode);

            return new GeneratedFile($"{interfaceName}.cs", generatedCode);
        }
    }
}
