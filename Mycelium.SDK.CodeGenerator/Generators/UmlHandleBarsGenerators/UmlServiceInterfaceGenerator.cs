// ------------------------------------------------------------------------------------------------
//  <copyright file="UmlServiceInterfaceGenerator.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Generators.UmlHandleBarsGenerators
{
    using Mycelium.SDK.CodeGenerator.HandleBarHelpers;

    using uml4net.StructuredClassifiers;
    using uml4net.xmi.Readers;

    /// <summary>
    /// Generates one Functional Data service interface per concrete Fabric class, each deriving from the generic base service interface that
    /// Fabric hand-writes.
    /// </summary>
    public sealed class UmlServiceInterfaceGenerator : UmlHandleBarsGenerator
    {
        /// <summary>
        /// The registered template used for each service interface.
        /// </summary>
        private const string ServiceInterfaceTemplateName = "service-interface-uml-template";

        /// <summary>
        /// Renders the complete batch of service interfaces and only then writes it to the output directory.
        /// </summary>
        /// <param name="xmiReaderResult">
        /// The parsed UML model from which the Fabric package is selected.
        /// </param>
        /// <param name="outputDirectory">
        /// The staging directory to which one <c>I&lt;ConcreteClass&gt;Service.cs</c> per concrete class is written.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous generation operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="xmiReaderResult" /> or <paramref name="outputDirectory" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the model does not contain exactly one Fabric package; nothing is written in that case.
        /// </exception>
        public override async Task GenerateAsync(XmiReaderResult xmiReaderResult, DirectoryInfo outputDirectory)
        {
            ArgumentNullException.ThrowIfNull(xmiReaderResult);
            ArgumentNullException.ThrowIfNull(outputDirectory);

            var payload = CreateHandlebarsPayload(xmiReaderResult);

            var generatedFiles = QueryConcreteClasses(payload.Classes)
                .Select(this.RenderServiceInterface)
                .OrderBy(generatedFile => generatedFile.FileName, StringComparer.Ordinal)
                .ToArray();

            await WriteAsync(generatedFiles, outputDirectory);
        }

        /// <summary>
        /// Registers the DTO class helper group used by the service-interface template.
        /// </summary>
        /// <remarks>
        /// This method is invoked during base construction. Implementations
        /// must not depend on fields initialized by a derived constructor.
        /// </remarks>
        protected override void RegisterHelpers() => this.Handlebars.RegisterDtoClassHelper();

        /// <summary>
        /// Registers the template used to render each service interface.
        /// </summary>
        /// <remarks>
        /// This method is invoked during base construction. Implementations
        /// must not depend on fields initialized by a derived constructor.
        /// </remarks>
        protected override void RegisterTemplates() => this.RegisterTemplate(ServiceInterfaceTemplateName);

        /// <summary>
        /// Renders the service interface of one concrete class without writing it.
        /// </summary>
        /// <param name="umlClass">
        /// The concrete UML class whose objects the service interface serves.
        /// </param>
        /// <returns>
        /// The <c>I&lt;ConcreteClass&gt;Service.cs</c> filename and its formatted source.
        /// </returns>
        private GeneratedFile RenderServiceInterface(IClass umlClass)
        {
            var generatedCode = this.CodeCleanup(this.Templates[ServiceInterfaceTemplateName](umlClass));

            return new GeneratedFile($"I{umlClass.Name}Service.cs", generatedCode);
        }
    }
}
