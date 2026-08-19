// ------------------------------------------------------------------------------------------------
//  <copyright file="UmlEnumGenerator.cs" company="Starion Group S.A.">
// 
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
// 
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Generators.UmlHandleBarsGenerators
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    using Mycelium.SDK.CodeGenerator.Extensions;
    using Mycelium.SDK.CodeGenerator.HandleBarHelpers;

    using uml4net.HandleBars;
    using uml4net.SimpleClassifiers;
    using uml4net.xmi.Readers;

    using DocumentationHelper = Mycelium.SDK.CodeGenerator.HandleBarHelpers.DocumentationHelper;
    using NamedElementHelper = Mycelium.SDK.CodeGenerator.HandleBarHelpers.NamedElementHelper;

    /// <summary>
    /// Generates the FunctionalData enumerations.
    /// </summary>
    public sealed class UmlEnumGenerator : UmlHandleBarsGenerator
    {
        /// <summary>
        /// The registered Handlebars template name used for UML enumerations.
        /// </summary>
        private const string TemplateName = "enumeration-uml-template";
        
        /// <summary>
        /// The complete reviewed enumeration output manifest.
        /// </summary>
        private static readonly string[] ExpectedFileNames =
        [
            "ActivationStatus.cs",
            "CommentStatus.cs",
            "OrganizationMembershipRole.cs",
            "ProjectLifecycleKind.cs",
            "ProjectMemberRole.cs",
            "ProjectMode.cs",
            "ProjectVisibility.cs",
            "ReviewStatus.cs"
        ];

        /// <inheritdoc />
        public override async Task GenerateAsync(XmiReaderResult xmiReaderResult, DirectoryInfo outputDirectory)
        {
            ArgumentNullException.ThrowIfNull(xmiReaderResult);
            ArgumentNullException.ThrowIfNull(outputDirectory);

            var payload = CreateHandlebarsPayload(xmiReaderResult);

            var generatedFiles = payload.Enumerations
                .Select(this.RenderEnumeration)
                .OrderBy(generatedFile => generatedFile.FileName, StringComparer.Ordinal)
                .ToArray();

            ThrowIfDuplicateFileNames(generatedFiles);
            ThrowIfUnexpectedManifest(generatedFiles);

            outputDirectory.Create();

            foreach (var generatedFile in generatedFiles)
            {
                await WriteAsync(generatedFile.Source, outputDirectory, generatedFile.FileName);
            }
        }

        /// <summary>
        /// Generates one enumeration.
        /// </summary>
        /// <param name="outputDirectory">
        /// The directory to which the generated enumeration is written.
        /// </param>
        /// <param name="enumeration">
        /// The UML enumeration to generate.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous enumeration generation operation. The task result
        /// contains the generated and formatted C# source.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="outputDirectory"/> or <paramref name="enumeration"/> is
        /// <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when the enumeration name or one of its literal names cannot be represented as a legal
        /// C# identifier.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when literal identifiers collide or the rendered source contains C# syntax errors.
        /// </exception>
        public async Task<string> GenerateEnumerationAsync(DirectoryInfo outputDirectory, IEnumeration enumeration)
        {
            ArgumentNullException.ThrowIfNull(outputDirectory);
            ArgumentNullException.ThrowIfNull(enumeration);

            var generatedFile = this.RenderEnumeration(enumeration);

            outputDirectory.Create();

            await WriteAsync(generatedFile.Source, outputDirectory, generatedFile.FileName);

            return generatedFile.Source;
        }

        /// <summary>
        /// Validates and renders one enumeration without writing it.
        /// </summary>
        /// <param name="enumeration">
        /// The UML enumeration to render.
        /// </param>
        /// <returns>
        /// The generated filename and source.
        /// </returns>
        private GeneratedFile RenderEnumeration(IEnumeration enumeration)
        {
            ArgumentNullException.ThrowIfNull(enumeration);

            _ = ReservedCSharpNameMapper.Map(enumeration.Name);

            var duplicateLiteralIdentifier = enumeration.OwnedLiteral
                .Select(literal => ReservedCSharpNameMapper.Map(literal.Name))
                .GroupBy(identifier => identifier, StringComparer.Ordinal)
                .FirstOrDefault(group => group.Count() > 1)
                ?.Key;

            if (duplicateLiteralIdentifier is not null)
            {
                throw new InvalidOperationException(
                    $"Enumeration '{enumeration.Name}' contains duplicate C# literal identifier "
                    + $"'{duplicateLiteralIdentifier}'.");
            }

            var fileName = $"{enumeration.Name}.cs";
            var template = this.Templates[TemplateName];
            var generatedCode = template(enumeration);

            ThrowIfInvalidSyntax(fileName, generatedCode);

            generatedCode = this.CodeCleanup(generatedCode);

            ThrowIfInvalidSyntax(fileName, generatedCode);

            return new GeneratedFile(fileName, generatedCode);
        }

        /// <summary>
        /// Rejects duplicate output filenames.
        /// </summary>
        /// <param name="generatedFiles">
        /// The complete rendered batch.
        /// </param>
        private static void ThrowIfDuplicateFileNames(IReadOnlyCollection<GeneratedFile> generatedFiles)
        {
            var duplicateFileName = generatedFiles
                .GroupBy(generatedFile => generatedFile.FileName, StringComparer.Ordinal)
                .FirstOrDefault(group => group.Count() > 1)
                ?.Key;

            if (duplicateFileName is not null)
            {
                throw new InvalidOperationException($"Enumeration generation produced duplicate filename '{duplicateFileName}'.");
            }
        }

        /// <summary>
        /// Verifies the complete reviewed output manifest.
        /// </summary>
        /// <param name="generatedFiles">
        /// The complete filename-sorted rendered batch.
        /// </param>
        private static void ThrowIfUnexpectedManifest(IReadOnlyCollection<GeneratedFile> generatedFiles)
        {
            var actualFileNames = generatedFiles
                .Select(generatedFile => generatedFile.FileName)
                .ToArray();

            if (actualFileNames.SequenceEqual(ExpectedFileNames, StringComparer.Ordinal))
            {
                return;
            }

            throw new InvalidOperationException(
                "Enumeration generation produced an unexpected manifest."
                + $"{Environment.NewLine}Expected: {string.Join(", ", ExpectedFileNames)}"
                + $"{Environment.NewLine}Actual: {string.Join(", ", actualFileNames)}");
        }

        /// <summary>
        /// Rejects rendered C# containing syntax errors.
        /// </summary>
        /// <param name="fileName">
        /// The generated filename.
        /// </param>
        /// <param name="source">
        /// The rendered source.
        /// </param>
        private static void ThrowIfInvalidSyntax(string fileName, string source)
        {
            var syntaxErrors = CSharpSyntaxTree
                .ParseText(source)
                .GetDiagnostics()
                .Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
                .ToArray();

            if (syntaxErrors.Length == 0)
            {
                return;
            }

            throw new InvalidOperationException(
                $"Enumeration generation produced invalid C# for '{fileName}'."
                + $"{Environment.NewLine}{string.Join(Environment.NewLine, syntaxErrors)}");
        }

        /// <summary>
        /// Represents one validated generated enumeration file.
        /// </summary>
        /// <param name="FileName">
        /// The output filename.
        /// </param>
        /// <param name="Source">
        /// The formatted C# source.
        /// </param>
        private sealed record GeneratedFile(string FileName, string Source);

        /// <inheritdoc />
        protected override void RegisterHelpers()
        {
            DocumentationHelper.RegisterDocumentationHelper(this.Handlebars);
            EnumHelper.RegisterEnumHelper(this.Handlebars);

            this.Handlebars.RegisterEnumerationHelper();
            this.Handlebars.RegisterEnumerationLiteralHelper();

            NamedElementHelper.RegisterNamedElementHelper(this.Handlebars);
        }

        /// <inheritdoc />
        protected override void RegisterTemplates()
        {
            this.RegisterTemplate(TemplateName);
        }
    }
}
