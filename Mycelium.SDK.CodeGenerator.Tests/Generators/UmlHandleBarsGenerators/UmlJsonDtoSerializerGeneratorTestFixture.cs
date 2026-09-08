// ------------------------------------------------------------------------------------------------
//  <copyright file="UmlJsonDtoSerializerGeneratorTestFixture.cs" company="Starion Group S.A.">
// 
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
// 
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Tests.Generators.UmlHandleBarsGenerators
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Mycelium.SDK.CodeGenerator.Generators.UmlHandleBarsGenerators;
    using Mycelium.SDK.CodeGenerator.Tests.Expected;

    using uml4net.Extensions;
    using uml4net.StructuredClassifiers;

    /// <summary>
    /// Verifies deterministic JSON DTO serializer generation.
    /// </summary>
    [TestFixture]
    public class UmlJsonDtoSerializerGeneratorTestFixture
    {
        /// <summary>
        /// Strict UTF-8 encoding without a byte-order mark.
        /// </summary>
        private static readonly UTF8Encoding StrictUtf8WithoutBom =
            new(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

        /// <summary>
        /// Classes derived from the currently loaded canonical model.
        /// </summary>
        private Dictionary<string, IClass> classes = null!;

        /// <summary>
        /// Complete committed production output copied into the test directory.
        /// </summary>
        private DirectoryInfo committedDirectory = null!;

        /// <summary>
        /// Separately reviewed representative golden directory.
        /// </summary>
        private DirectoryInfo expectedDirectory = null!;

        /// <summary>
        /// Isolated generated-output staging directory.
        /// </summary>
        private DirectoryInfo stagingDirectory = null!;

        /// <summary>
        /// Loads the canonical model and generates the complete serializer batch.
        /// </summary>
        /// <returns>
        /// A task representing asynchronous setup.
        /// </returns>
        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.committedDirectory = new DirectoryInfo(
                Path.Combine(
                    TestContext.CurrentContext.TestDirectory,
                    "Committed",
                    "Mycelium.SDK.Serializer.Json",
                    "AutoGenSerializer"));

            this.expectedDirectory = new DirectoryInfo(
                Path.Combine(
                    TestContext.CurrentContext.TestDirectory,
                    "Expected",
                    "UML",
                    "AutoGenSerializer"));

            this.stagingDirectory = new DirectoryInfo(
                Path.Combine(
                    TestContext.CurrentContext.TestDirectory,
                    "UML",
                    "_Mycelium.SDK.Serializer.Json.AutoGenSerializer"));

            if (this.stagingDirectory.Exists)
            {
                this.stagingDirectory.Delete(recursive: true);
            }

            var xmiReaderResult = GeneratorSetupFixture.ReadFunctionalData();

            var functionalData =
                GeneratorSetupFixture.QueryFunctionalDataPackage(xmiReaderResult);

            this.classes = functionalData
                .QueryPackages()
                .SelectMany(
                    package =>
                        package.PackagedElement.OfType<IClass>())
                .ToDictionary(
                    umlClass => umlClass.Name,
                    StringComparer.Ordinal);

            var generator = new UmlJsonDtoSerializerGenerator();

            await generator.GenerateAsync(
                xmiReaderResult,
                this.stagingDirectory);
        }

        /// <summary>
        /// Verifies that the batch contains exactly one serializer per concrete
        /// class plus the serialization provider.
        /// </summary>
        [Test]
        public void Verify_that_full_batch_contains_every_concrete_DTO_and_no_abstract_DTOs()
        {
            var expectedFileNames = this.classes.Values
                .Where(umlClass => !umlClass.IsAbstract)
                .Select(umlClass => $"{umlClass.Name}Serializer.cs")
                .Append("SerializationProvider.cs")
                .OrderBy(fileName => fileName, StringComparer.Ordinal)
                .ToArray();

            var stagedFileNames =
                QueryCSharpFileNames(this.stagingDirectory);

            Assert.That(
                stagedFileNames,
                Is.EqualTo(expectedFileNames),
                "The serializer batch does not match the current concrete model classes.");
        }

        /// <summary>
        /// Verifies that the golden directory contains exactly the serializers
        /// for the bounded non-abstract representative selection.
        /// </summary>
        [Test]
        [Category("Expected")]
        public void Verify_that_golden_set_matches_non_abstract_representative_selection()
        {
            Assert.That(
                this.expectedDirectory.Exists,
                Is.True,
                "The representative serializer-golden directory is missing.");

            if (!this.expectedDirectory.Exists)
            {
                return;
            }

            var expectedFileNames = new List<string>();

            foreach (var className in new RepresentativeClasses())
            {
                if (!this.classes.TryGetValue(className, out var umlClass))
                {
                    Assert.Fail(
                        $"Representative UML class '{className}' was not found.");

                    return;
                }

                if (!umlClass.IsAbstract)
                {
                    expectedFileNames.Add($"{className}Serializer.cs");
                }
            }

            var orderedExpectedFileNames = expectedFileNames
                .OrderBy(fileName => fileName, StringComparer.Ordinal)
                .ToArray();

            var goldenFileNames =
                QueryCSharpFileNames(this.expectedDirectory);

            Assert.That(
                goldenFileNames,
                Is.EqualTo(orderedExpectedFileNames),
                "The serializer golden set must contain exactly the non-abstract representative selection.");
        }

        /// <summary>
        /// Verifies representative serializer output using strict UTF-8
        /// decoding and ordinal text equality.
        /// </summary>
        /// <param name="className">
        /// The representative UML class name.
        /// </param>
        /// <returns>
        /// A task representing asynchronous verification.
        /// </returns>
        [TestCaseSource(typeof(RepresentativeClasses))]
        [Category("Expected")]
        public async Task Verify_that_representative_serializers_match_their_goldens(
            string className)
        {
            if (!this.classes.TryGetValue(className, out var umlClass))
            {
                Assert.Fail(
                    $"Representative UML class '{className}' was not found.");

                return;
            }

            var fileName = $"{className}Serializer.cs";

            var stagedPath =
                Path.Combine(this.stagingDirectory.FullName, fileName);

            var expectedPath =
                Path.Combine(this.expectedDirectory.FullName, fileName);

            if (umlClass.IsAbstract)
            {
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(
                        File.Exists(stagedPath),
                        Is.False,
                        $"Abstract class '{className}' received a serializer.");

                    Assert.That(
                        File.Exists(expectedPath),
                        Is.False,
                        $"Abstract class '{className}' received a serializer golden.");
                }

                return;
            }

            await AssertOrdinalFilesMatchAsync(
                stagedPath,
                expectedPath,
                $"Generated serializer '{fileName}'",
                "its reviewed golden");
        }

        /// <summary>
        /// Verifies complete staged and committed serializer filenames and
        /// contents independently.
        /// </summary>
        /// <returns>
        /// A task representing asynchronous verification.
        /// </returns>
        [Test]
        public async Task Verify_that_complete_staged_output_matches_committed_serializers()
        {
            Assert.That(
                this.committedDirectory.Exists,
                Is.True,
                "The committed serializer directory was not copied to the test output.");

            if (!this.committedDirectory.Exists)
            {
                return;
            }

            var stagedFileNames =
                QueryCSharpFileNames(this.stagingDirectory);

            var committedFileNames =
                QueryCSharpFileNames(this.committedDirectory);

            Assert.That(
                stagedFileNames,
                Is.EqualTo(committedFileNames),
                "The staged and committed serializer filename sets differ.");

            foreach (var fileName in stagedFileNames)
            {
                await AssertOrdinalFilesMatchAsync(
                    Path.Combine(this.stagingDirectory.FullName, fileName),
                    Path.Combine(this.committedDirectory.FullName, fileName),
                    $"Staged serializer '{fileName}'",
                    "the committed production source");
            }
        }

        /// <summary>
        /// Verifies strict UTF-8 encoding, CRLF line endings, absence of a BOM,
        /// and the generated-code marker.
        /// </summary>
        /// <returns>
        /// A task representing asynchronous verification.
        /// </returns>
        [Test]
        public async Task Verify_that_generated_serializers_use_the_required_file_format()
        {
            var generatedFiles = this.stagingDirectory
                .GetFiles("*.cs", SearchOption.TopDirectoryOnly)
                .OrderBy(file => file.Name, StringComparer.Ordinal)
                .ToArray();

            Assert.That(
                generatedFiles,
                Is.Not.Empty,
                "Serializer generation produced no C# files.");

            foreach (var generatedFile in generatedFiles)
            {
                var bytes =
                    await File.ReadAllBytesAsync(generatedFile.FullName);

                var hasUtf8Bom =
                    bytes.Length >= 3
                    && bytes[0] == 0xEF
                    && bytes[1] == 0xBB
                    && bytes[2] == 0xBF;

                var source = StrictUtf8WithoutBom.GetString(bytes);

                var sourceWithoutCrLf = source.Replace(
                    "\r\n",
                    string.Empty,
                    StringComparison.Ordinal);

                using (Assert.EnterMultipleScope())
                {
                    Assert.That(
                        hasUtf8Bom,
                        Is.False,
                        $"Generated serializer '{generatedFile.Name}' contains a UTF-8 byte-order mark.");

                    Assert.That(
                        source,
                        Does.Contain("\r\n"),
                        $"Generated serializer '{generatedFile.Name}' contains no CRLF line endings.");

                    Assert.That(
                        sourceWithoutCrLf,
                        Does.Not.Contain("\r"),
                        $"Generated serializer '{generatedFile.Name}' contains a standalone carriage return.");

                    Assert.That(
                        sourceWithoutCrLf,
                        Does.Not.Contain("\n"),
                        $"Generated serializer '{generatedFile.Name}' contains a standalone line feed.");

                    Assert.That(
                        source,
                        Does.Contain("[GeneratedCode(\"Mycelium.SDK\", \"latest\")]"),
                        $"Generated serializer '{generatedFile.Name}' lacks the generated-code marker.");
                }
            }
        }

        /// <summary>
        /// Compares two source files using strict UTF-8 decoding and ordinal
        /// string equality.
        /// </summary>
        private static async Task AssertOrdinalFilesMatchAsync(
            string actualPath,
            string expectedPath,
            string actualDescription,
            string expectedDescription)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(
                    File.Exists(actualPath),
                    Is.True,
                    $"{actualDescription} is missing.");

                Assert.That(
                    File.Exists(expectedPath),
                    Is.True,
                    $"The file representing {expectedDescription} is missing.");
            }

            if (!File.Exists(actualPath) || !File.Exists(expectedPath))
            {
                return;
            }

            var actualSource =
                await File.ReadAllTextAsync(actualPath, StrictUtf8WithoutBom);

            var expectedSource =
                await File.ReadAllTextAsync(expectedPath, StrictUtf8WithoutBom);

            Assert.That(
                string.Equals(
                    actualSource,
                    expectedSource,
                    StringComparison.Ordinal),
                Is.True,
                $"{actualDescription} differs from {expectedDescription}.");
        }

        /// <summary>
        /// Returns ordinally sorted C# filenames from a directory.
        /// </summary>
        private static string[] QueryCSharpFileNames(
            DirectoryInfo directory)
        {
            return directory
                .GetFiles("*.cs", SearchOption.TopDirectoryOnly)
                .Select(file => file.Name)
                .OrderBy(fileName => fileName, StringComparer.Ordinal)
                .ToArray();
        }
    }
}
