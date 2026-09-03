// ------------------------------------------------------------------------------------------------
//  <copyright file="UmlClassGeneratorTestFixtureBase.cs" company="Starion Group S.A.">
// 
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
// 
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Tests.Generators.UmlHandleBarsGenerators
{
    using System.Text;

    /// <summary>
    /// Provides shared file-verification behavior for UML class generator tests.
    /// </summary>
    public abstract class UmlClassGeneratorTestFixtureBase
    {
        /// <summary>
        /// Gets the strict UTF-8 encoding used to validate generated source files.
        /// </summary>
        protected static UTF8Encoding StrictUtf8WithoutBom { get; } =
            new(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

        /// <summary>
        /// Verifies that two generated source files contain identical text.
        /// </summary>
        /// <param name="generatedPath">
        /// The path of the generated file.
        /// </param>
        /// <param name="expectedPath">
        /// The path of the expected or committed file.
        /// </param>
        /// <param name="generatedDescription">
        /// A description of the generated file used in assertion messages.
        /// </param>
        /// <param name="expectedDescription">
        /// A description of the expected file used in assertion messages.
        /// </param>
        protected static async Task AssertFilesMatchAsync(
            string generatedPath,
            string expectedPath,
            string generatedDescription,
            string expectedDescription)
        {
            Assert.That(
                File.Exists(generatedPath),
                Is.True,
                $"{generatedDescription} was not generated.");

            Assert.That(
                File.Exists(expectedPath),
                Is.True,
                $"The file representing {expectedDescription} is missing.");

            if (!File.Exists(generatedPath) || !File.Exists(expectedPath))
            {
                return;
            }

            var generatedSource =
                await File.ReadAllTextAsync(generatedPath, StrictUtf8WithoutBom);

            var expectedSource =
                await File.ReadAllTextAsync(expectedPath, StrictUtf8WithoutBom);

            Assert.That(
                generatedSource,
                Is.EqualTo(expectedSource),
                $"{generatedDescription} differs from {expectedDescription}.");
        }

        /// <summary>
        /// Returns the names of the C# files in a directory in deterministic order.
        /// </summary>
        /// <param name="directory">
        /// The directory to inspect.
        /// </param>
        /// <returns>
        /// The ordinally sorted C# filenames.
        /// </returns>
        protected static string[] QueryCSharpFileNames(DirectoryInfo directory)
        {
            return directory
                .GetFiles("*.cs", SearchOption.TopDirectoryOnly)
                .Select(file => file.Name)
                .OrderBy(fileName => fileName, StringComparer.Ordinal)
                .ToArray();
        }
    }
}
