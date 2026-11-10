// ------------------------------------------------------------------------------------------------
//  <copyright file="CoreServiceInterfaceGenerator.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Generators.CoreHandleBarsGenerators
{
    using HandlebarsDotNet.Helpers;

    using Mycelium.SDK.CodeGenerator.Extensions;
    using Mycelium.SDK.CodeGenerator.HandleBarHelpers;

    using uml4net.Packages;
    using uml4net.StructuredClassifiers;

    /// <summary>
    /// Generates the service interfaces of the concrete SysML v2 and KerML metaclasses.
    /// </summary>
    /// <remarks>
    /// Each interface binds the hand-written <c>ICoreDataService&lt;TElement&gt;</c> of Fabric to the SysML2.NET data
    /// transfer object of one metaclass, so the operations themselves are declared once.
    /// </remarks>
    public sealed class CoreServiceInterfaceGenerator : HandleBarsGenerator
    {
        /// <summary>
        /// The registered Handlebars template name used for a service interface.
        /// </summary>
        private const string ServiceInterfaceTemplateName = "service-interface-template";

        /// <summary>
        /// The artifact name used in validation messages.
        /// </summary>
        private const string ArtifactName = "Core service interface";

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreServiceInterfaceGenerator" /> class.
        /// </summary>
        public CoreServiceInterfaceGenerator() : base("Core")
        {
        }

        /// <summary>
        /// Generates a service interface for every concrete metaclass of a model and of the models it imports.
        /// </summary>
        /// <param name="model">
        /// The SysML v2 abstract syntax model from which the service interfaces are generated.
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
        /// Thrown when <paramref name="model" /> or <paramref name="outputDirectory" /> is
        /// <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the model lacks information required to generate an interface, or when the batch
        /// would produce duplicate filenames.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when a metaclass name cannot be represented as a legal C# identifier.
        /// </exception>
        public async Task GenerateAsync(IPackage model, DirectoryInfo outputDirectory)
        {
            ArgumentNullException.ThrowIfNull(model);
            ArgumentNullException.ThrowIfNull(outputDirectory);

            var generatedFiles = model.QueryConcreteElementClasses()
                .Select(this.RenderServiceInterface)
                .OrderBy(generatedFile => generatedFile.FileName, StringComparer.Ordinal)
                .ToArray();

            ThrowIfDuplicateFileNames(generatedFiles, ArtifactName);

            await WriteAsync(generatedFiles, outputDirectory);
        }

        /// <summary>
        /// Generates one service interface.
        /// </summary>
        /// <param name="elementClass">
        /// The metaclass from which the interface is generated.
        /// </param>
        /// <param name="outputDirectory">
        /// The directory to which the generated interface is written.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous generation operation. The task result contains the
        /// generated and formatted C# source.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="elementClass" /> or <paramref name="outputDirectory" /> is
        /// <see langword="null" />.
        /// </exception>
        public async Task<string> GenerateServiceInterfaceAsync(IClass elementClass, DirectoryInfo outputDirectory)
        {
            ArgumentNullException.ThrowIfNull(elementClass);
            ArgumentNullException.ThrowIfNull(outputDirectory);

            var generatedFile = this.RenderServiceInterface(elementClass);

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

            this.Handlebars.RegisterNamedElementHelper();
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
        /// Renders the service interface of a single metaclass.
        /// </summary>
        /// <param name="elementClass">
        /// The metaclass from which the interface is rendered.
        /// </param>
        /// <returns>
        /// The generated filename and source.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the metaclass has no name.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when the metaclass name cannot be represented as a legal C# identifier.
        /// </exception>
        private GeneratedFile RenderServiceInterface(IClass elementClass)
        {
            if (string.IsNullOrWhiteSpace(elementClass.Name))
            {
                throw new InvalidOperationException($"Metaclass '{elementClass.XmiId}' has no name, so the generated interface would have no name.");
            }

            var interfaceName = ReservedCSharpNameMapper.Map($"I{elementClass.Name}Service");
            var generatedCode = this.Templates[ServiceInterfaceTemplateName](elementClass);

            generatedCode = this.CodeCleanup(generatedCode);

            return new GeneratedFile($"{interfaceName}.cs", generatedCode);
        }
    }
}
