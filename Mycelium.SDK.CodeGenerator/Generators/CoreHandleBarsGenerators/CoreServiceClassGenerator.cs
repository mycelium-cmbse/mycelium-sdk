// ------------------------------------------------------------------------------------------------
//  <copyright file="CoreServiceClassGenerator.cs" company="Starion Group S.A.">
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
    /// Generates the service classes of the concrete SysML v2 and KerML metaclasses and the provider that maps a data type onto its service.
    /// </summary>
    /// <remarks>
    /// Each class binds the hand-written <c>CoreDataService&lt;TElement&gt;</c> of Fabric to one metaclass and is <c>partial</c>, so that
    /// its metaclass-specific validation is hand-written beside it.
    /// </remarks>
    public sealed class CoreServiceClassGenerator : HandleBarsGenerator
    {
        /// <summary>
        /// The registered Handlebars template name used for a service class.
        /// </summary>
        private const string ServiceClassTemplateName = "service-class-template";

        /// <summary>
        /// The registered Handlebars template name used for the provider of every service class.
        /// </summary>
        private const string ServiceProviderTemplateName = "service-provider-template";

        /// <summary>
        /// The name of the file that maps each data type onto its service class.
        /// </summary>
        private const string ServiceProviderFileName = "CoreDataServiceProvider.cs";

        /// <summary>
        /// The artifact name used in validation messages.
        /// </summary>
        private const string ArtifactName = "Core service class";

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreServiceClassGenerator" /> class.
        /// </summary>
        public CoreServiceClassGenerator() : base("Core")
        {
        }

        /// <summary>
        /// Generates a service class for every concrete metaclass of a model and of the models it imports, and the provider that maps each
        /// data type onto its service class.
        /// </summary>
        /// <param name="model">
        /// The SysML v2 abstract syntax model from which the service classes are generated.
        /// </param>
        /// <param name="outputDirectory">
        /// The directory to which the generated files are written.
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
        /// Thrown when a metaclass has no name, or when the batch would produce duplicate filenames.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when a metaclass name cannot be represented as a legal C# identifier.
        /// </exception>
        public async Task GenerateAsync(IPackage model, DirectoryInfo outputDirectory)
        {
            ArgumentNullException.ThrowIfNull(model);
            ArgumentNullException.ThrowIfNull(outputDirectory);

            var elementClasses = model.QueryConcreteElementClasses();

            GeneratedFile[] generatedFiles =
            [
                .. elementClasses.Select(this.RenderServiceClass).OrderBy(generatedFile => generatedFile.FileName, StringComparer.Ordinal),
                new(ServiceProviderFileName, this.CodeCleanup(this.Templates[ServiceProviderTemplateName](elementClasses)))
            ];

            ThrowIfDuplicateFileNames(generatedFiles, ArtifactName);

            await WriteAsync(generatedFiles, outputDirectory);
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
        protected override void RegisterTemplates()
        {
            this.RegisterTemplate(ServiceClassTemplateName);
            this.RegisterTemplate(ServiceProviderTemplateName);
        }

        /// <summary>
        /// Renders the service class of a single metaclass.
        /// </summary>
        /// <param name="elementClass">
        /// The metaclass from which the class is rendered.
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
        private GeneratedFile RenderServiceClass(IClass elementClass)
        {
            if (string.IsNullOrWhiteSpace(elementClass.Name))
            {
                throw new InvalidOperationException($"Metaclass '{elementClass.XmiId}' has no name, so the generated service class would have no name.");
            }

            var className = ReservedCSharpNameMapper.Map($"{elementClass.Name}Service");
            var generatedCode = this.CodeCleanup(this.Templates[ServiceClassTemplateName](elementClass));

            return new GeneratedFile($"{className}.cs", generatedCode);
        }
    }
}
