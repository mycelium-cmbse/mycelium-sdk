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
    using Mycelium.SDK.CodeGenerator.Extensions;
    using Mycelium.SDK.CodeGenerator.HandleBarHelpers;

    using uml4net.StructuredClassifiers;
    using uml4net.xmi.Readers;

    /// <summary>
    /// Generates one data access object per concrete Fabric class, performing create, read, update and delete operations against the
    /// PostgreSQL schema delivered in <c>0001_initial.sql</c>, and the provider that resolves the data access object of a class kind.
    /// </summary>
    public sealed class UmlDaoGenerator : UmlHandleBarsGenerator
    {
        /// <summary>
        /// The registered template used for each data access object.
        /// </summary>
        private const string DaoTemplateName = "dao-uml-template";

        /// <summary>
        /// The registered template used for the data access object provider.
        /// </summary>
        private const string DaoProviderTemplateName = "dao-provider-uml-template";

        /// <summary>
        /// The filename of the data access object provider.
        /// </summary>
        private const string DaoProviderFileName = "DaoProvider.cs";

        /// <summary>
        /// Renders the complete batch of data access objects and their provider and only then writes it to the output directory.
        /// </summary>
        /// <param name="xmiReaderResult">
        /// The parsed UML model from which the Fabric package is selected.
        /// </param>
        /// <param name="outputDirectory">
        /// The staging directory to which one <c>&lt;ConcreteClass&gt;Dao.cs</c> per concrete class and <c>DaoProvider.cs</c> are written.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous generation operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="xmiReaderResult" /> or <paramref name="outputDirectory" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the model does not contain exactly one Fabric package, or when two parameters of one data access object would have the same
        /// name; nothing is written in either case.
        /// </exception>
        public override async Task GenerateAsync(XmiReaderResult xmiReaderResult, DirectoryInfo outputDirectory)
        {
            ArgumentNullException.ThrowIfNull(xmiReaderResult);
            ArgumentNullException.ThrowIfNull(outputDirectory);

            var payload = CreateHandlebarsPayload(xmiReaderResult);

            var concreteClasses = QueryConcreteClasses(payload.Classes);

            var generatedFiles = concreteClasses
                .Select(this.RenderDao)
                .Append(this.RenderDaoProvider(concreteClasses))
                .OrderBy(generatedFile => generatedFile.FileName, StringComparer.Ordinal)
                .ToArray();

            await WriteAsync(generatedFiles, outputDirectory);
        }

        /// <summary>
        /// Generates the data access object provider for the concrete classes among the supplied classes.
        /// </summary>
        /// <param name="outputDirectory">
        /// The directory to which <c>DaoProvider.cs</c> is written.
        /// </param>
        /// <param name="classes">
        /// The classes whose concrete members receive an entry; abstract classes receive none.
        /// </param>
        /// <returns>
        /// The generated provider source.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="outputDirectory" /> or <paramref name="classes" /> is <see langword="null" />.
        /// </exception>
        public async Task<string> GenerateDaoProviderAsync(DirectoryInfo outputDirectory, IEnumerable<IClass> classes)
        {
            ArgumentNullException.ThrowIfNull(outputDirectory);
            ArgumentNullException.ThrowIfNull(classes);

            var generatedFile = this.RenderDaoProvider(QueryConcreteClasses(classes));

            await WriteAsync([generatedFile], outputDirectory);

            return generatedFile.Source;
        }

        /// <summary>
        /// Registers the class, property, string and shared SQL-name helper groups used by the DAO and DAO provider templates.
        /// </summary>
        /// <remarks>
        /// This method is invoked during base construction. Implementations
        /// must not depend on fields initialized by a derived constructor.
        /// </remarks>
        protected override void RegisterHelpers()
        {
            uml4net.HandleBars.StringHelper.RegisterStringHelper(this.Handlebars);
            uml4net.HandleBars.PropertyHelper.RegisterPropertyHelper(this.Handlebars);

            this.Handlebars.RegisterSqlNameHelpers();
            this.Handlebars.RegisterDtoClassHelper();
            this.Handlebars.RegisterDaoClassHelper();
            this.Handlebars.RegisterJsonSerializerPropertyHelper();
            this.Handlebars.RegisterDaoPropertyHelper();
        }

        /// <summary>
        /// Registers the templates used to render each data access object and their provider.
        /// </summary>
        /// <remarks>
        /// This method is invoked during base construction. Implementations
        /// must not depend on fields initialized by a derived constructor.
        /// </remarks>
        protected override void RegisterTemplates()
        {
            this.RegisterTemplate(DaoTemplateName);
            this.RegisterTemplate(DaoProviderTemplateName);
        }

        /// <summary>
        /// Renders the data access object of one concrete class without writing it.
        /// </summary>
        /// <param name="umlClass">
        /// The concrete UML class whose objects the data access object stores.
        /// </param>
        /// <returns>
        /// The <c>&lt;ConcreteClass&gt;Dao.cs</c> filename and its formatted source.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when two parameters of the data access object would have the same name.
        /// </exception>
        private GeneratedFile RenderDao(IClass umlClass)
        {
            _ = umlClass.QuerySqlParameterNames();

            var generatedCode = this.CodeCleanup(this.Templates[DaoTemplateName](umlClass));

            return new GeneratedFile($"{umlClass.Name}Dao.cs", generatedCode);
        }

        /// <summary>
        /// Renders the data access object provider without writing it.
        /// </summary>
        /// <param name="concreteClasses">
        /// The ordered concrete UML classes that each receive one provider entry.
        /// </param>
        /// <returns>
        /// The <c>DaoProvider.cs</c> filename and its formatted source.
        /// </returns>
        private GeneratedFile RenderDaoProvider(IReadOnlyCollection<IClass> concreteClasses)
        {
            var generatedCode = this.CodeCleanup(this.Templates[DaoProviderTemplateName](concreteClasses));

            return new GeneratedFile(DaoProviderFileName, generatedCode);
        }
    }
}
