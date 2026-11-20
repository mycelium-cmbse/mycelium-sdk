// ------------------------------------------------------------------------------------------------
//  <copyright file="CoreServiceProviderGenerator.cs" company="Starion Group S.A.">
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

    /// <summary>
    /// Generates the provider that maps the data transfer object type of every concrete SysML v2 and KerML metaclass onto its core-data service.
    /// </summary>
    /// <remarks>
    /// The service itself is Fabric's hand-written open generic <c>CoreDataService&lt;TElement&gt;</c>; the map is what needs the model,
    /// since a payload only reveals its data transfer object type at runtime.
    /// </remarks>
    public sealed class CoreServiceProviderGenerator : HandleBarsGenerator
    {
        /// <summary>
        /// The registered Handlebars template name used for the provider.
        /// </summary>
        private const string ServiceProviderTemplateName = "service-provider-template";

        /// <summary>
        /// The name of the file that maps each data type onto its service.
        /// </summary>
        private const string ServiceProviderFileName = "CoreDataServiceProvider.cs";

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreServiceProviderGenerator" /> class.
        /// </summary>
        public CoreServiceProviderGenerator() : base("Core")
        {
        }

        /// <summary>
        /// Generates the provider over every concrete metaclass of a model and of the models it imports.
        /// </summary>
        /// <param name="model">
        /// The SysML v2 abstract syntax model from which the provider is generated.
        /// </param>
        /// <param name="outputDirectory">
        /// The directory to which the generated provider is written.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous generation operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="model" /> or <paramref name="outputDirectory" /> is
        /// <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the model declares no concrete specialization of <c>Element</c>.
        /// </exception>
        public async Task GenerateAsync(IPackage model, DirectoryInfo outputDirectory)
        {
            ArgumentNullException.ThrowIfNull(model);
            ArgumentNullException.ThrowIfNull(outputDirectory);

            var generatedCode = this.CodeCleanup(this.Templates[ServiceProviderTemplateName](model.QueryConcreteElementClasses()));

            await WriteAsync([new GeneratedFile(ServiceProviderFileName, generatedCode)], outputDirectory);
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
        protected override void RegisterTemplates() => this.RegisterTemplate(ServiceProviderTemplateName);
    }
}
