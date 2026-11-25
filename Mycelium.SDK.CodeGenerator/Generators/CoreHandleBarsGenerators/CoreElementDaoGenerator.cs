// ------------------------------------------------------------------------------------------------
//  <copyright file="CoreElementDaoGenerator.cs" company="Starion Group S.A.">
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
    /// Generates one data access object per concrete SysML v2 and KerML metaclass, writing and reading its element versions in the SysML v2
    /// PostgreSQL schema of <c>SysML2.NET.Dal</c>, and the provider that holds them all.
    /// </summary>
    /// <remarks>
    /// A generated object carries only its metaclass and its SQL; Fabric's hand-written <c>ElementDaoBase&lt;TElement&gt;</c> binds and runs it, and its
    /// hand-written <c>ElementDaoProviderBase</c> resolves the objects the generated provider lists.
    /// </remarks>
    public sealed class CoreElementDaoGenerator : HandleBarsGenerator
    {
        /// <summary>
        /// The registered Handlebars template name used for each data access object.
        /// </summary>
        private const string ElementDaoTemplateName = "element-dao-template";

        /// <summary>
        /// The registered Handlebars template name used for the provider of the data access objects.
        /// </summary>
        private const string ElementDaoProviderTemplateName = "element-dao-provider-template";

        /// <summary>
        /// The name of the file that lists the data access object of every concrete metaclass.
        /// </summary>
        private const string ElementDaoProviderFileName = "ElementDaoProvider.cs";

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreElementDaoGenerator" /> class.
        /// </summary>
        public CoreElementDaoGenerator() : base("Core")
        {
        }

        /// <summary>
        /// Renders the data access object of every concrete metaclass of a model and of the models it imports, and their provider, and only then
        /// writes them.
        /// </summary>
        /// <param name="model">
        /// The SysML v2 abstract syntax model from which the data access objects are generated.
        /// </param>
        /// <param name="outputDirectory">
        /// The directory to which one <c>&lt;Metaclass&gt;Dao.cs</c> per concrete metaclass and <c>ElementDaoProvider.cs</c> are written.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous generation operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="model" /> or <paramref name="outputDirectory" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the model declares no concrete specialization of <c>Element</c>, or when a stored property cannot be mapped onto the schema;
        /// nothing is written in either case.
        /// </exception>
        public async Task GenerateAsync(IPackage model, DirectoryInfo outputDirectory)
        {
            ArgumentNullException.ThrowIfNull(model);
            ArgumentNullException.ThrowIfNull(outputDirectory);

            var elementClasses = model.QueryConcreteElementClasses();

            GeneratedFile[] generatedFiles =
            [
                .. elementClasses.Select(this.RenderElementDao),
                new GeneratedFile(ElementDaoProviderFileName, this.CodeCleanup(this.Templates[ElementDaoProviderTemplateName](elementClasses)))
            ];

            ThrowIfDuplicateFileNames(generatedFiles, "Element DAO");

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
            uml4net.HandleBars.PropertyHelper.RegisterPropertyHelper(this.Handlebars);

            this.Handlebars.RegisterNamedElementHelper();
            this.Handlebars.RegisterCoreDaoClassHelper();
            this.Handlebars.RegisterCoreDaoPropertyHelper();
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
            this.RegisterTemplate(ElementDaoTemplateName);
            this.RegisterTemplate(ElementDaoProviderTemplateName);
        }

        /// <summary>
        /// Renders the data access object of one concrete metaclass without writing it.
        /// </summary>
        /// <param name="umlClass">
        /// The concrete metaclass whose element versions the data access object stores.
        /// </param>
        /// <returns>
        /// The <c>&lt;Metaclass&gt;Dao.cs</c> filename and its formatted source.
        /// </returns>
        private GeneratedFile RenderElementDao(IClass umlClass)
        {
            var generatedCode = this.CodeCleanup(this.Templates[ElementDaoTemplateName](umlClass));

            return new GeneratedFile($"{umlClass.Name}Dao.cs", generatedCode);
        }
    }
}
