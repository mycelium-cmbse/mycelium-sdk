// ------------------------------------------------------------------------------------------------
//  <copyright file="Generator.cs" company="Starion Group S.A.">
// 
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
// 
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Generators
{
    using System.Text;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Formatting;

    /// <summary>
    /// Base class for code generators.
    /// </summary>
    public abstract class Generator
    {
        /// <summary>
        /// The UTF-8 encoding without a byte-order mark used for generated files.
        /// </summary>
        private static readonly Encoding Utf8WithoutBom = new UTF8Encoding(false);

        /// <summary>
        /// Initializes a new instance of the <see cref="Generator" /> class.
        /// </summary>
        /// <param name="templateSubfolder">
        /// The optional template subdirectory.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the code-generator assembly directory cannot be resolved.
        /// </exception>
        protected Generator(string templateSubfolder = null)
        {
            var assemblyDirectory =
                Path.GetDirectoryName(typeof(Generator).Assembly.Location)
                ?? throw new InvalidOperationException("The code-generator assembly directory could not be resolved.");

            this.TemplateFolderPath = Path.Combine(assemblyDirectory, "Templates");

            if (!string.IsNullOrWhiteSpace(templateSubfolder))
            {
                this.TemplateFolderPath = Path.Combine(this.TemplateFolderPath, templateSubfolder);
            }
        }

        /// <summary>
        /// Gets or sets the directory containing this generator's templates.
        /// </summary>
        public string TemplateFolderPath { get; protected set; }

        /// <summary>
        /// Formats generated C# and normalizes it for deterministic comparison.
        /// </summary>
        /// <param name="generatedCode">
        /// The generated C# source.
        /// </param>
        /// <returns>
        /// The formatted C# source using CRLF line endings.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="generatedCode" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="generatedCode" /> is empty.
        /// </exception>
        protected virtual string CodeCleanup(string generatedCode)
        {
            ArgumentException.ThrowIfNullOrEmpty(generatedCode);

            generatedCode = generatedCode.Replace("&nbsp;", " ", StringComparison.OrdinalIgnoreCase);
            using var workspace = new AdhocWorkspace();

            var syntaxTree = CSharpSyntaxTree.ParseText(generatedCode);
            var root = syntaxTree.GetRoot();
            var formattedRoot = Formatter.Format(root, workspace);

            return formattedRoot.SyntaxTree
                .GetText()
                .ToString()
                .ReplaceLineEndings("\r\n");
        }

        /// <summary>
        /// Validates, formats, and revalidates one generated C# file without writing it.
        /// </summary>
        /// <param name="fileName">
        /// The deterministic output filename.
        /// </param>
        /// <param name="generatedCode">
        /// The rendered C# source.
        /// </param>
        /// <returns>
        /// The validated generated file.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="fileName" /> or <paramref name="generatedCode" /> is
        /// <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="fileName" /> or <paramref name="generatedCode" /> is empty.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the rendered or formatted source contains invalid C# syntax.
        /// </exception>
        protected GeneratedFile CreateGeneratedFile(string fileName, string generatedCode)
        {
            ArgumentException.ThrowIfNullOrEmpty(fileName);
            ArgumentException.ThrowIfNullOrEmpty(generatedCode);

            ThrowIfInvalidSyntax(fileName, generatedCode);

            var formattedCode = this.CodeCleanup(generatedCode);

            ThrowIfInvalidSyntax(fileName, formattedCode);

            return new GeneratedFile(fileName, formattedCode);
        }

        /// <summary>
        /// Materializes, sorts, and validates a complete generated batch.
        /// </summary>
        /// <param name="generatedFiles">
        /// The completely rendered batch.
        /// </param>
        /// <param name="expectedFileNames">
        /// The independent reviewed manifest.
        /// </param>
        /// <param name="artifactName">
        /// The artifact name used in diagnostics.
        /// </param>
        /// <returns>
        /// The filename-sorted validated batch.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="generatedFiles" /> or <paramref name="expectedFileNames" /> is
        /// <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="artifactName" /> is empty.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the batch contains duplicate filenames or does not match the reviewed manifest.
        /// </exception>
        protected static GeneratedFile[] PrepareBatch(
            IEnumerable<GeneratedFile> generatedFiles,
            IReadOnlyList<string> expectedFileNames,
            string artifactName)
        {
            ArgumentNullException.ThrowIfNull(generatedFiles);
            ArgumentNullException.ThrowIfNull(expectedFileNames);
            ArgumentException.ThrowIfNullOrEmpty(artifactName);

            var orderedFiles = generatedFiles
                .OrderBy(
                    generatedFile => generatedFile.FileName,
                    StringComparer.Ordinal)
                .ToArray();

            var duplicateFileName = orderedFiles
                .GroupBy(
                    generatedFile => generatedFile.FileName,
                    StringComparer.OrdinalIgnoreCase)
                .FirstOrDefault(group => group.Count() > 1)
                ?.Key;

            if (duplicateFileName is not null)
            {
                throw new InvalidOperationException(
                    $"{artifactName} generation produced duplicate filename "
                    + $"'{duplicateFileName}'.");
            }

            var actualFileNames = orderedFiles
                .Select(generatedFile => generatedFile.FileName)
                .ToArray();

            if (!actualFileNames.SequenceEqual(
                    expectedFileNames,
                    StringComparer.Ordinal))
            {
                throw new InvalidOperationException(
                    $"{artifactName} generation produced an unexpected manifest."
                    + $"{Environment.NewLine}Expected: {string.Join(", ", expectedFileNames)}"
                    + $"{Environment.NewLine}Actual: {string.Join(", ", actualFileNames)}");
            }

            return orderedFiles;
        }

        /// <summary>
        /// Writes a fully preflighted batch.
        /// </summary>
        /// <param name="generatedFiles">
        /// The validated generated batch.
        /// </param>
        /// <param name="outputDirectory">
        /// The destination directory.
        /// </param>
        /// <returns>
        /// An awaitable task.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="generatedFiles" /> or <paramref name="outputDirectory" /> is
        /// <see langword="null" />.
        /// </exception>
        protected static async Task WriteBatchAsync(
            IReadOnlyCollection<GeneratedFile> generatedFiles,
            DirectoryInfo outputDirectory)
        {
            ArgumentNullException.ThrowIfNull(generatedFiles);
            ArgumentNullException.ThrowIfNull(outputDirectory);

            outputDirectory.Create();

            foreach (var generatedFile in generatedFiles)
            {
                await WriteAsync(
                    generatedFile.Source,
                    outputDirectory,
                    generatedFile.FileName);
            }
        }

        /// <summary>
        /// Writes generated source as UTF-8 without a byte-order mark.
        /// </summary>
        /// <param name="generatedCode">
        /// The generated source.
        /// </param>
        /// <param name="outputDirectory">
        /// The existing staging output directory.
        /// </param>
        /// <param name="fileName">
        /// The deterministic output filename.
        /// </param>
        /// <returns>
        /// An awaitable task.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="generatedCode" />, <paramref name="outputDirectory" />, or
        /// <paramref name="fileName" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="generatedCode" /> or <paramref name="fileName" /> is empty.
        /// </exception>
        protected static async Task WriteAsync(string generatedCode, DirectoryInfo outputDirectory, string fileName)
        {
            ArgumentException.ThrowIfNullOrEmpty(generatedCode);
            ArgumentNullException.ThrowIfNull(outputDirectory);
            ArgumentException.ThrowIfNullOrEmpty(fileName);

            var filePath = Path.Combine(outputDirectory.FullName, fileName);

            await File.WriteAllTextAsync(filePath, generatedCode, Utf8WithoutBom);
        }

        /// <summary>
        /// Validates that generated source contains no C# syntax errors.
        /// </summary>
        /// <param name="fileName">
        /// The generated filename used in validation diagnostics.
        /// </param>
        /// <param name="source">
        /// The generated C# source to validate.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when <paramref name="source" /> contains invalid C# syntax.
        /// </exception>
        private static void ThrowIfInvalidSyntax(string fileName, string source)
        {
            var syntaxErrors = CSharpSyntaxTree
                .ParseText(
                    source,
                    CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp14))
                .GetDiagnostics()
                .Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
                .ToArray();

            if (syntaxErrors.Length == 0)
            {
                return;
            }

            throw new InvalidOperationException(
                $"Generation produced invalid C# for '{fileName}'."
                + $"{Environment.NewLine}{string.Join(Environment.NewLine, syntaxErrors)}");
        }

        /// <summary>
        /// Represents one validated generated C# source file.
        /// </summary>
        /// <param name="FileName">
        /// The output filename.
        /// </param>
        /// <param name="Source">
        /// The formatted C# source.
        /// </param>
        protected sealed record GeneratedFile(string FileName, string Source);
    }
}
