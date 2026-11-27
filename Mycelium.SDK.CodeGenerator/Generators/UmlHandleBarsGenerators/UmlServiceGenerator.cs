// ------------------------------------------------------------------------------------------------
//  <copyright file="UmlServiceGenerator.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Generators.UmlHandleBarsGenerators
{
    using Mycelium.SDK.CodeGenerator.Extensions;
    using Mycelium.SDK.CodeGenerator.HandleBarHelpers;

    using uml4net.StructuredClassifiers;
    using uml4net.xmi.Readers;

    /// <summary>
    /// Generates one Functional Data service per concrete Fabric class, each deriving from the base service that Fabric hand-writes, and the
    /// service provider that resolves the service of a class kind.
    /// </summary>
    public sealed class UmlServiceGenerator : UmlHandleBarsGenerator
    {
        /// <summary>
        /// The registered template used for each service.
        /// </summary>
        private const string ServiceTemplateName = "service-uml-template";

        /// <summary>
        /// The registered template used for the service provider.
        /// </summary>
        private const string ServiceProviderTemplateName = "service-provider-uml-template";

        /// <summary>
        /// The filename of the service provider.
        /// </summary>
        private const string ServiceProviderFileName = "ThingServiceProvider.cs";

        /// <summary>
        /// Renders the complete batch of services and their provider and only then writes it to the output directory.
        /// </summary>
        /// <param name="xmiReaderResult">
        /// The parsed UML model from which the Fabric package is selected.
        /// </param>
        /// <param name="outputDirectory">
        /// The staging directory to which one <c>&lt;ConcreteClass&gt;Service.cs</c> per concrete class and <c>ThingServiceProvider.cs</c> are
        /// written.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous generation operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="xmiReaderResult" /> or <paramref name="outputDirectory" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the model does not contain exactly one Fabric package, or when a required composite property has an abstract type or a
        /// required value of its part has no source; nothing is written in either case.
        /// </exception>
        public override async Task GenerateAsync(XmiReaderResult xmiReaderResult, DirectoryInfo outputDirectory)
        {
            ArgumentNullException.ThrowIfNull(xmiReaderResult);
            ArgumentNullException.ThrowIfNull(outputDirectory);

            var payload = CreateHandlebarsPayload(xmiReaderResult);

            var concreteClasses = QueryConcreteClasses(payload.Classes);

            var generatedFiles = concreteClasses
                .Select(this.RenderService)
                .Append(this.RenderServiceProvider(concreteClasses))
                .OrderBy(generatedFile => generatedFile.FileName, StringComparer.Ordinal)
                .ToArray();

            await WriteAsync(generatedFiles, outputDirectory);
        }

        /// <summary>
        /// Registers the class, property, named-element and string helper groups used by the service and service provider templates.
        /// </summary>
        /// <remarks>
        /// This method is invoked during base construction. Implementations
        /// must not depend on fields initialized by a derived constructor.
        /// </remarks>
        protected override void RegisterHelpers()
        {
            uml4net.HandleBars.PropertyHelper.RegisterPropertyHelper(this.Handlebars);

            this.Handlebars.RegisterNamedElementHelper();
            this.Handlebars.RegisterDtoClassHelper();
            this.Handlebars.RegisterServiceClassHelper();
            this.Handlebars.RegisterJsonSerializerPropertyHelper();
            this.Handlebars.RegisterServicePropertyHelper();
        }

        /// <summary>
        /// Registers the templates used to render each service and the service provider.
        /// </summary>
        /// <remarks>
        /// This method is invoked during base construction. Implementations
        /// must not depend on fields initialized by a derived constructor.
        /// </remarks>
        protected override void RegisterTemplates()
        {
            this.RegisterTemplate(ServiceTemplateName);
            this.RegisterTemplate(ServiceProviderTemplateName);
        }

        /// <summary>
        /// Renders the service of one concrete class without writing it, after checking that every required part of the class can be created.
        /// </summary>
        /// <param name="umlClass">
        /// The concrete UML class whose objects the service serves.
        /// </param>
        /// <returns>
        /// The <c>&lt;ConcreteClass&gt;Service.cs</c> filename and its formatted source.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a required composite property of the class has an abstract type or a required value of its part has no source.
        /// </exception>
        private GeneratedFile RenderService(IClass umlClass)
        {
            _ = umlClass.QueryRequiredPartProperties();

            var generatedCode = this.CodeCleanup(this.Templates[ServiceTemplateName](umlClass));

            return new GeneratedFile($"{umlClass.Name}Service.cs", generatedCode);
        }

        /// <summary>
        /// Renders the service provider over every concrete class without writing it.
        /// </summary>
        /// <param name="concreteClasses">
        /// The concrete UML classes, in ordinal name order, whose services the provider resolves.
        /// </param>
        /// <returns>
        /// The <c>ThingServiceProvider.cs</c> filename and its formatted source.
        /// </returns>
        private GeneratedFile RenderServiceProvider(IReadOnlyCollection<IClass> concreteClasses)
        {
            var generatedCode = this.CodeCleanup(this.Templates[ServiceProviderTemplateName](concreteClasses));

            return new GeneratedFile(ServiceProviderFileName, generatedCode);
        }
    }
}
