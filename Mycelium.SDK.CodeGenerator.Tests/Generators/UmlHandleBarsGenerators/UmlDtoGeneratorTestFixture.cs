// ------------------------------------------------------------------------------------------------
//  <copyright file="UmlDtoGeneratorTestFixture.cs" company="Starion Group S.A.">
// 
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
// 
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Tests.Generators.UmlHandleBarsGenerators
{
    using System.Text;

    using Mycelium.SDK.CodeGenerator.Generators.UmlHandleBarsGenerators;
    using Mycelium.SDK.CodeGenerator.Tests.Expected;

    using uml4net.StructuredClassifiers;

    [TestFixture]
    public class UmlDtoGeneratorTestFixture
    {
        private static readonly UTF8Encoding StrictUtf8WithoutBom =
            new(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

        private Dictionary<string, IClass> classes = null!;
        private DirectoryInfo committedDirectory = null!;
        private DirectoryInfo expectedDirectory = null!;
        private DirectoryInfo stagingDirectory = null!;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.committedDirectory = new DirectoryInfo(
                Path.Combine(
                    TestContext.CurrentContext.TestDirectory,
                    "Committed",
                    "Mycelium.SDK",
                    "AutoGenDTO"));

            this.expectedDirectory = new DirectoryInfo(
                Path.Combine(
                    TestContext.CurrentContext.TestDirectory,
                    "Expected",
                    "UML",
                    "AutoGenDTO"));

            this.stagingDirectory = new DirectoryInfo(
                Path.Combine(
                    TestContext.CurrentContext.TestDirectory,
                    "UML",
                    "_Mycelium.SDK.AutoGenDTO"));

            if (this.stagingDirectory.Exists)
            {
                this.stagingDirectory.Delete(recursive: true);
            }

            var xmiReaderResult = GeneratorSetupFixture.ReadFunctionalData();

            var functionalData =
                GeneratorSetupFixture.QueryFunctionalDataPackage(xmiReaderResult);

            this.classes = functionalData.PackagedElement
                .OfType<IClass>()
                .ToDictionary(
                    umlClass => umlClass.Name,
                    StringComparer.Ordinal);

            var generator = new UmlDtoGenerator();

            await generator.GenerateAsync(
                xmiReaderResult,
                this.stagingDirectory);
        }

        [Test]
        public async Task Verify_that_complete_batch_matches_committed_SDK_DTOs()
        {
            Assert.That(
                this.committedDirectory.Exists,
                Is.True,
                "The committed SDK DTO directory was not copied to the test output.");

            if (!this.committedDirectory.Exists)
            {
                return;
            }

            var generatedFileNames = QueryCSharpFileNames(this.stagingDirectory);
            var committedFileNames = QueryCSharpFileNames(this.committedDirectory);

            Assert.That(
                generatedFileNames,
                Is.EqualTo(committedFileNames),
                "The generated and committed DTO file sets differ.");

            foreach (var fileName in generatedFileNames)
            {
                await AssertFilesMatchAsync(
                    Path.Combine(this.stagingDirectory.FullName, fileName),
                    Path.Combine(this.committedDirectory.FullName, fileName),
                    $"Generated DTO '{fileName}'",
                    "the committed SDK source");
            }
        }

        [Test]
        [TestCaseSource(typeof(RepresentativeClasses))]
        [Category("Expected")]
        public async Task Verify_that_representative_DTOs_match_reviewed_goldens(
            string className)
        {
            Assert.That(
                this.classes.TryGetValue(className, out var umlClass),
                Is.True,
                $"Representative UML class '{className}' was not found.");

            if (umlClass is null)
            {
                return;
            }

            var interfaceFileName = $"I{className}.cs";

            await AssertFilesMatchAsync(
                Path.Combine(this.stagingDirectory.FullName, interfaceFileName),
                Path.Combine(this.expectedDirectory.FullName, interfaceFileName),
                $"Generated DTO interface '{interfaceFileName}'",
                "its reviewed golden");

            if (umlClass.IsAbstract)
            {
                return;
            }

            var classFileName = $"{className}.cs";

            await AssertFilesMatchAsync(
                Path.Combine(this.stagingDirectory.FullName, classFileName),
                Path.Combine(this.expectedDirectory.FullName, classFileName),
                $"Generated DTO class '{classFileName}'",
                "its reviewed golden");
        }

        [Test]
        [Category("Expected")]
        public async Task Verify_that_IThing_DTO_interface_matches_reviewed_golden()
        {
            const string fileName = "IThing.cs";

            await AssertFilesMatchAsync(
                Path.Combine(this.stagingDirectory.FullName, fileName),
                Path.Combine(this.expectedDirectory.FullName, fileName),
                $"Generated DTO interface '{fileName}'",
                "its reviewed golden");
        }

        [Test]
        public async Task Verify_that_generated_DTOs_use_the_required_file_format()
        {
            foreach (var fileName in QueryCSharpFileNames(this.stagingDirectory))
            {
                var bytes = await File.ReadAllBytesAsync(
                    Path.Combine(this.stagingDirectory.FullName, fileName));

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
                        $"Generated DTO '{fileName}' contains a UTF-8 byte-order mark.");

                    Assert.That(
                        source,
                        Does.Contain("\r\n"),
                        $"Generated DTO '{fileName}' contains no CRLF line endings.");

                    Assert.That(
                        sourceWithoutCrLf,
                        Does.Not.Contain("\r"),
                        $"Generated DTO '{fileName}' contains a standalone carriage return.");

                    Assert.That(
                        sourceWithoutCrLf,
                        Does.Not.Contain("\n"),
                        $"Generated DTO '{fileName}' contains a standalone line feed.");
                }
            }
        }

        private static async Task AssertFilesMatchAsync(
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

        private static string[] QueryCSharpFileNames(DirectoryInfo directory)
        {
            return directory
                .GetFiles("*.cs", SearchOption.TopDirectoryOnly)
                .Select(file => file.Name)
                .OrderBy(fileName => fileName, StringComparer.Ordinal)
                .ToArray();
        }
    }
}
