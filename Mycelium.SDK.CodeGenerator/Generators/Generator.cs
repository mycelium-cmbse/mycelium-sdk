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
    using System.IO;
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
        protected static async Task WriteAsync(string generatedCode, DirectoryInfo outputDirectory, string fileName)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(generatedCode);
            ArgumentNullException.ThrowIfNull(outputDirectory);
            ArgumentNullException.ThrowIfNullOrEmpty(fileName);

            var filePath = Path.Combine(outputDirectory.FullName, fileName);

            await File.WriteAllTextAsync(filePath,generatedCode,Utf8WithoutBom);
        }
    }
}