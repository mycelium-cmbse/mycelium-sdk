// ------------------------------------------------------------------------------------------------
//  <copyright file="UmlSqlSchemaGenerator.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Generators.UmlHandleBarsGenerators
{
    using System.Reflection;

    using Mycelium.SDK.CodeGenerator.Extensions;
    using Mycelium.SDK.CodeGenerator.HandleBarHelpers;

    using uml4net.StructuredClassifiers;
    using uml4net.xmi.Readers;

    /// <summary>
    /// Generates the initial PostgreSQL schema from the canonical packaged Fabric UML model.
    /// </summary>
    public sealed class UmlSqlSchemaGenerator : UmlHandleBarsGenerator
    {
        /// <summary>
        /// The registered template used for the complete initial schema script.
        /// </summary>
        private const string TemplateName = "sql-schema-uml-template";

        /// <summary>
        /// Generates the complete initial schema without executing SQL or promoting staged output.
        /// </summary>
        /// <param name="xmiReaderResult">
        /// The parsed UML model from which the Fabric package is selected.
        /// </param>
        /// <param name="outputDirectory">
        /// The staging directory to which <c>schema.sql</c> is written.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous generation operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="xmiReaderResult" /> or <paramref name="outputDirectory" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the model does not contain exactly one Fabric package or required model-package metadata is missing or duplicated.
        /// </exception>
        public override async Task GenerateAsync(XmiReaderResult xmiReaderResult, DirectoryInfo outputDirectory)
        {
            ArgumentNullException.ThrowIfNull(xmiReaderResult);
            ArgumentNullException.ThrowIfNull(outputDirectory);

            var payload = CreateHandlebarsPayload(xmiReaderResult);
            var generatedFile = new GeneratedFile("schema.sql", this.RenderSchema(payload.Classes));

            await WriteAsync([generatedFile], outputDirectory, newLine: "\n");
        }

        /// <summary>
        /// Registers only the SQL helpers used by the schema template.
        /// </summary>
        protected override void RegisterHelpers() => this.Handlebars.RegisterSqlSchemaHelpers();

        /// <summary>
        /// Registers the template used to render the complete initial schema script.
        /// </summary>
        protected override void RegisterTemplates() => this.RegisterTemplate(TemplateName);

        /// <summary>
        /// Normalizes rendered SQL to LF with one final newline without applying C# formatting.
        /// </summary>
        /// <param name="generatedCode">
        /// The rendered SQL text.
        /// </param>
        /// <returns>
        /// The SQL text with LF line endings and one final newline.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="generatedCode" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="generatedCode" /> is empty.
        /// </exception>
        protected override string CodeCleanup(string generatedCode)
        {
            ArgumentException.ThrowIfNullOrEmpty(generatedCode);

            return generatedCode.ReplaceLineEndings("\n").TrimEnd('\n') + "\n";
        }

        /// <summary>
        /// Renders every selected class and shared infrastructure through the SQL template.
        /// </summary>
        /// <param name="classes">
        /// The deterministically selected classes to render.
        /// </param>
        /// <returns>
        /// The complete initial schema script with LF line endings and a final newline.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when required model-package metadata is missing or duplicated.
        /// </exception>
        private string RenderSchema(IReadOnlyCollection<IClass> classes)
        {
            var context = new
            {
                Classes = classes,
                BaseClasses = classes.SelectMany(umlClass => umlClass.QueryGeneralizations())
                    .DistinctBy(umlClass => umlClass.XmiId, StringComparer.Ordinal)
                    .OrderBy(umlClass => umlClass.Name, StringComparer.Ordinal)
                    .ThenBy(umlClass => umlClass.XmiId, StringComparer.Ordinal)
                    .ToArray(),
                FabricModelVersion = QueryModelPackageVersion("Mycelium.Model.Fabric"),
                CommonPrimitivesModelVersion = QueryModelPackageVersion("Mycelium.Model.CommonPrimitives")
            };

            return this.CodeCleanup(this.Templates[TemplateName](context));
        }

        /// <summary>
        /// Reads the selected model-package version recorded by the generator project during compilation.
        /// </summary>
        /// <param name="packageName">
        /// The model-package identifier used as the assembly metadata key.
        /// </param>
        /// <returns>
        /// The package version shared by the central package declaration and generated provenance comment.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the requested metadata key is missing or duplicated.
        /// </exception>
        private static string QueryModelPackageVersion(string packageName)
        {
            return typeof(UmlSqlSchemaGenerator).Assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
                .Single(attribute => string.Equals(attribute.Key, packageName, StringComparison.Ordinal)).Value;
        }
    }
}
