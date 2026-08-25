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
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

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
        /// Initializes a new instance of the <see cref="Generator"/> class.
        /// </summary>
        /// <param name="templateSubfolder">
        /// The optional template subdirectory.
        /// </param>
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
        /// Thrown when <paramref name="generatedCode"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="generatedCode"/> is empty.
        /// </exception>
        protected virtual string CodeCleanup(string generatedCode)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(generatedCode);

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
        /// Thrown when <paramref name="generatedCode"/>, <paramref name="outputDirectory"/>, or
        /// <paramref name="fileName"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="generatedCode"/> or <paramref name="fileName"/> is empty.
        /// </exception>
        protected static async Task WriteAsync(string generatedCode, DirectoryInfo outputDirectory, string fileName)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(generatedCode);
            ArgumentNullException.ThrowIfNull(outputDirectory);
            ArgumentNullException.ThrowIfNullOrEmpty(fileName);

            var filePath = Path.Combine(outputDirectory.FullName, fileName);

            await File.WriteAllTextAsync(filePath,generatedCode,Utf8WithoutBom);
        }

        /// <summary>
        /// Creates the output directory and writes a complete rendered batch to it.
        /// </summary>
        /// <param name="generatedFiles">
        /// The complete rendered batch.
        /// </param>
        /// <param name="outputDirectory">
        /// The output directory, created if it does not exist.
        /// </param>
        /// <returns>
        /// An awaitable task.
        /// </returns>
        /// <remarks>
        /// Callers must render and validate the whole batch before calling this method: it is the point
        /// at which the output directory comes into existence, so anything that can reject the batch has
        /// to have run already. That ordering is what keeps generation all-or-nothing.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="generatedFiles"/> or <paramref name="outputDirectory"/> is
        /// <see langword="null" />.
        /// </exception>
        protected static async Task WriteAsync(IReadOnlyCollection<GeneratedFile> generatedFiles, DirectoryInfo outputDirectory)
        {
            ArgumentNullException.ThrowIfNull(generatedFiles);
            ArgumentNullException.ThrowIfNull(outputDirectory);

            outputDirectory.Create();

            foreach (var generatedFile in generatedFiles)
            {
                await WriteAsync(generatedFile.Source, outputDirectory, generatedFile.FileName);
            }
        }

        /// <summary>
        /// Verifies that a generated batch contains no duplicate filenames.
        /// </summary>
        /// <param name="generatedFiles">
        /// The generated files to validate.
        /// </param>
        /// <param name="artifactName">
        /// The artifact name used in the validation message.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="generatedFiles"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="artifactName"/> is <see langword="null" /> or empty.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when multiple generated files have the same filename.
        /// </exception>
        protected static void ThrowIfDuplicateFileNames(IReadOnlyCollection<GeneratedFile> generatedFiles, string artifactName)
        {
            ArgumentNullException.ThrowIfNull(generatedFiles);
            ArgumentException.ThrowIfNullOrEmpty(artifactName);

            var duplicateFileName = generatedFiles
                .GroupBy(generatedFile => generatedFile.FileName, StringComparer.Ordinal)
                .FirstOrDefault(group => group.Count() > 1)
                ?.Key;

            if (duplicateFileName is not null)
            {
                throw new InvalidOperationException(
                    $"{artifactName} generation produced duplicate filename '{duplicateFileName}'.");
            }
        }

        /// <summary>
        /// Represents one generated source file.
        /// </summary>
        /// <param name="FileName">
        /// The generated filename.
        /// </param>
        /// <param name="Source">
        /// The formatted generated C# source.
        /// </param>
        protected sealed record GeneratedFile(string FileName, string Source);
    }
}