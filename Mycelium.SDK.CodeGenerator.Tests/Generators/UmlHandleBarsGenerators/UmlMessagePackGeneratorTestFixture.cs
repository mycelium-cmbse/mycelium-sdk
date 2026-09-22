// ------------------------------------------------------------------------------------------------
//  <copyright file="UmlMessagePackGeneratorTestFixture.cs" company="Starion Group S.A.">
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

    using uml4net.Extensions;
    using uml4net.StructuredClassifiers;

    [TestFixture]
    public class UmlMessagePackGeneratorTestFixture
    {
        private const string ResolverFileName = "DataResolverGetFormatterHelper.cs";

        private static readonly UTF8Encoding StrictUtf8WithoutBom = new(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

        private Dictionary<string, IClass> classes = null!;

        private DirectoryInfo committedDirectory = null!;

        private DirectoryInfo expectedDirectory = null!;

        private DirectoryInfo stagingDirectory = null!;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            var repositoryDirectory = QueryRepositoryDirectory();

            this.committedDirectory = new DirectoryInfo(Path.Combine(repositoryDirectory.FullName, "Mycelium.SDK.Serializer.MessagePack", "AutoGenMessagePackFormatter"));

            this.expectedDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Expected", "UML", "AutoGenMessagePackFormatter"));

            var stagingPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "UML", "_Mycelium.SDK.Serializer.MessagePack.AutoGenMessagePackFormatter");

            this.stagingDirectory = new DirectoryInfo(stagingPath);

            if (this.stagingDirectory.Exists)
            {
                this.stagingDirectory.Delete(true);
            }

            var xmiReaderResult = GeneratorSetupFixture.ReadFunctionalData();

            var functionalData = GeneratorSetupFixture.QueryFunctionalDataPackage(xmiReaderResult);

            this.classes = functionalData.QueryPackages()
                .SelectMany(package => package.PackagedElement.OfType<IClass>())
                .ToDictionary(umlClass => umlClass.Name, StringComparer.Ordinal);

            var generator = new UmlMessagePackGenerator();

            await generator.GenerateAsync(xmiReaderResult, this.stagingDirectory);
        }

        [Test]
        public async Task Verify_that_complete_staged_output_matches_committed_MessagePack_formatters()
        {
            Assert.That(this.committedDirectory.Exists, Is.True, "The committed MessagePack formatter directory is missing.");

            if (!this.committedDirectory.Exists)
            {
                return;
            }

            var stagedFileNames = QueryCSharpFileNames(this.stagingDirectory);

            var committedFileNames = QueryCSharpFileNames(this.committedDirectory);

            Assert.That(stagedFileNames, Is.EqualTo(committedFileNames), "The staged and committed MessagePack formatter filename sets differ.");

            foreach (var fileName in stagedFileNames)
            {
                var stagedPath = Path.Combine(this.stagingDirectory.FullName, fileName);

                var committedPath = Path.Combine(this.committedDirectory.FullName, fileName);

                await AssertOrdinalFilesMatchAsync(stagedPath, committedPath, $"Staged MessagePack artifact '{fileName}'", "the committed production source");
            }
        }

        [Test]
        public void Verify_that_full_batch_contains_every_concrete_DTO_and_the_resolver()
        {
            var expectedFileNames = this.classes.Values.Where(umlClass => !umlClass.IsAbstract)
                .Select(umlClass => $"{umlClass.Name}MessagePackFormatter.cs")
                .Append(ResolverFileName)
                .OrderBy(fileName => fileName, StringComparer.Ordinal)
                .ToArray();

            var stagedFileNames = QueryCSharpFileNames(this.stagingDirectory);

            Assert.That(stagedFileNames, Is.EqualTo(expectedFileNames), "The MessagePack formatter batch does not match the current concrete model classes.");
        }

        [Test]
        public async Task Verify_that_generated_source_and_goldens_use_the_required_file_format()
        {
            var generatedFiles = this.stagingDirectory.GetFiles("*.cs", SearchOption.TopDirectoryOnly)
                .OrderBy(file => file.Name, StringComparer.Ordinal)
                .ToArray();

            var goldenFiles = this.expectedDirectory.GetFiles("*.cs", SearchOption.TopDirectoryOnly)
                .OrderBy(file => file.Name, StringComparer.Ordinal)
                .ToArray();

            Assert.That(generatedFiles, Is.Not.Empty, "MessagePack generation produced no C# files.");
            Assert.That(goldenFiles, Is.Not.Empty, "The representative MessagePack golden directory contains no C# files.");

            foreach (var sourceFile in generatedFiles.Concat(goldenFiles))
            {
                await AssertRequiredFileFormatAsync(sourceFile);
            }
        }

        [Test]
        [Category("Expected")]
        public void Verify_that_golden_set_matches_non_abstract_representative_selection_and_the_resolver()
        {
            var expectedFileNames = new List<string>();

            foreach (var className in new RepresentativeClasses())
            {
                if (!this.classes.TryGetValue(className, out var umlClass))
                {
                    Assert.Fail($"Representative UML class '{className}' was not found.");

                    return;
                }

                if (!umlClass.IsAbstract)
                {
                    expectedFileNames.Add($"{className}MessagePackFormatter.cs");
                }
            }

            expectedFileNames.Add(ResolverFileName);

            var orderedExpectedFileNames = expectedFileNames.OrderBy(fileName => fileName, StringComparer.Ordinal)
                .ToArray();

            var goldenFileNames = QueryCSharpFileNames(this.expectedDirectory);

            Assert.That(
                goldenFileNames,
                Is.EqualTo(orderedExpectedFileNames),
                "The MessagePack golden set must contain exactly the non-abstract representative DTO selection and resolver.");
        }

        [TestCaseSource(typeof(RepresentativeClasses))]
        [Category("Expected")]
        public async Task Verify_that_representative_formatters_match_their_goldens(string className)
        {
            if (!this.classes.TryGetValue(className, out var umlClass))
            {
                Assert.Fail($"Representative UML class '{className}' was not found.");

                return;
            }

            var fileName = $"{className}MessagePackFormatter.cs";

            var stagedPath = Path.Combine(this.stagingDirectory.FullName, fileName);

            var expectedPath = Path.Combine(this.expectedDirectory.FullName, fileName);

            if (umlClass.IsAbstract)
            {
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(File.Exists(stagedPath), Is.False, $"Abstract class '{className}' received a MessagePack formatter.");

                    Assert.That(File.Exists(expectedPath), Is.False, $"Abstract class '{className}' received a MessagePack formatter golden.");
                }

                return;
            }

            await AssertOrdinalFilesMatchAsync(stagedPath, expectedPath, $"Generated MessagePack formatter '{fileName}'", "its reviewed golden");
        }

        [Test]
        [Category("Expected")]
        public async Task Verify_that_resolver_matches_its_golden()
        {
            var stagedPath = Path.Combine(this.stagingDirectory.FullName, ResolverFileName);

            var expectedPath = Path.Combine(this.expectedDirectory.FullName, ResolverFileName);

            await AssertOrdinalFilesMatchAsync(stagedPath, expectedPath, "The generated MessagePack resolver", "its reviewed golden");
        }

        private static async Task AssertOrdinalFilesMatchAsync(string actualPath, string expectedPath, string actualDescription, string expectedDescription)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(File.Exists(actualPath), Is.True, $"{actualDescription} is missing.");

                Assert.That(File.Exists(expectedPath), Is.True, $"The file representing {expectedDescription} is missing.");
            }

            if (!File.Exists(actualPath) || !File.Exists(expectedPath))
            {
                return;
            }

            var actualSource = await File.ReadAllTextAsync(actualPath, StrictUtf8WithoutBom);

            var expectedSource = await File.ReadAllTextAsync(expectedPath, StrictUtf8WithoutBom);

            Assert.That(string.Equals(actualSource, expectedSource, StringComparison.Ordinal), Is.True, $"{actualDescription} differs from {expectedDescription}.");
        }

        private static async Task AssertRequiredFileFormatAsync(FileInfo sourceFile)
        {
            var bytes = await File.ReadAllBytesAsync(sourceFile.FullName);

            var hasUtf8Bom = bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF;

            var source = StrictUtf8WithoutBom.GetString(bytes);

            var sourceWithoutCrLf = source.Replace("\r\n", string.Empty, StringComparison.Ordinal);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(hasUtf8Bom, Is.False, $"Source file '{sourceFile.Name}' contains a UTF-8 byte-order mark.");

                Assert.That(source, Does.Contain("\r\n"), $"Source file '{sourceFile.Name}' contains no CRLF line endings.");

                Assert.That(source.EndsWith("\r\n", StringComparison.Ordinal), Is.True, $"Source file '{sourceFile.Name}' lacks a final CRLF.");

                Assert.That(sourceWithoutCrLf, Does.Not.Contain("\r"), $"Source file '{sourceFile.Name}' contains a standalone carriage return.");

                Assert.That(sourceWithoutCrLf, Does.Not.Contain("\n"), $"Source file '{sourceFile.Name}' contains a standalone line feed.");

                Assert.That(source, Does.Contain("[GeneratedCode(\"Mycelium.SDK\", \"latest\")]"), $"Source file '{sourceFile.Name}' lacks the generated-code marker.");
            }
        }

        private static string[] QueryCSharpFileNames(DirectoryInfo directory)
        {
            return directory.GetFiles("*.cs", SearchOption.TopDirectoryOnly)
                .Select(file => file.Name)
                .OrderBy(fileName => fileName, StringComparer.Ordinal)
                .ToArray();
        }

        private static DirectoryInfo QueryRepositoryDirectory()
        {
            var directory = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);

            while (directory != null)
            {
                if (File.Exists(Path.Combine(directory.FullName, "Mycelium.SDK.sln")))
                {
                    return directory;
                }

                directory = directory.Parent;
            }

            throw new DirectoryNotFoundException("The Mycelium SDK repository root could not be located from the test output directory.");
        }
    }
}
