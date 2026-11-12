// ------------------------------------------------------------------------------------------------
//  <copyright file="UmlDaoGenerator.cs" company="Starion Group S.A.">
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
    /// Generates one data access object per concrete Fabric class, performing create, read, update and delete operations against the
    /// PostgreSQL schema delivered in <c>0001_initial.sql</c>.
    /// </summary>
    public sealed class UmlDaoGenerator : UmlHandleBarsGenerator
    {
        /// <summary>
        /// The registered template used for each data access object.
        /// </summary>
        private const string DaoTemplateName = "dao-uml-template";

        /// <summary>
        /// Renders the complete batch of data access objects and only then writes it to the output directory.
        /// </summary>
        /// <param name="xmiReaderResult">
        /// The parsed UML model from which the Fabric package is selected.
        /// </param>
        /// <param name="outputDirectory">
        /// The staging directory to which one <c>&lt;ConcreteClass&gt;Dao.cs</c> per concrete class is written.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous generation operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="xmiReaderResult" /> or <paramref name="outputDirectory" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the model does not contain exactly one Fabric package.
        /// </exception>
        public override async Task GenerateAsync(XmiReaderResult xmiReaderResult, DirectoryInfo outputDirectory)
        {
            ArgumentNullException.ThrowIfNull(xmiReaderResult);
            ArgumentNullException.ThrowIfNull(outputDirectory);

            var payload = CreateHandlebarsPayload(xmiReaderResult);

            var generatedFiles = QueryConcreteClasses(payload.Classes)
                .Select(this.RenderDao)
                .OrderBy(generatedFile => generatedFile.FileName, StringComparer.Ordinal)
                .ToArray();

            await WriteAsync(generatedFiles, outputDirectory);
        }

        /// <summary>
        /// Registers only the class and property helper groups used by the DAO template.
        /// </summary>
        /// <remarks>
        /// This method is invoked during base construction. Implementations
        /// must not depend on fields initialized by a derived constructor.
        /// </remarks>
        protected override void RegisterHelpers()
        {
            this.Handlebars.RegisterDtoClassHelper();
            this.Handlebars.RegisterDaoClassHelper();
            this.Handlebars.RegisterJsonSerializerPropertyHelper();
            this.Handlebars.RegisterDaoPropertyHelper();
        }

        /// <summary>
        /// Registers the template used to render each data access object.
        /// </summary>
        /// <remarks>
        /// This method is invoked during base construction. Implementations
        /// must not depend on fields initialized by a derived constructor.
        /// </remarks>
        protected override void RegisterTemplates() => this.RegisterTemplate(DaoTemplateName);

        /// <summary>
        /// Renders the data access object of one concrete class without writing it.
        /// </summary>
        /// <param name="umlClass">
        /// The concrete UML class whose objects the data access object stores.
        /// </param>
        /// <returns>
        /// The <c>&lt;ConcreteClass&gt;Dao.cs</c> filename and its formatted source.
        /// </returns>
        private GeneratedFile RenderDao(IClass umlClass)
        {
            var generatedCode = this.CodeCleanup(this.Templates[DaoTemplateName](umlClass));

            return new GeneratedFile($"{umlClass.Name}Dao.cs", generatedCode);
        }
    }
}
