// ------------------------------------------------------------------------------------------------
//  <copyright file="UmlEnumGeneratorTestFixture.cs" company="Starion Group S.A.">
// 
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
// 
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Tests.Generators.UmlHandleBarsGenerators
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Mycelium.SDK.CodeGenerator.Generators.UmlHandleBarsGenerators;
    using Mycelium.SDK.CodeGenerator.Tests.Expected;

    using uml4net.SimpleClassifiers;

    [TestFixture]
    public class UmlEnumGeneratorTestFixture
    {
        private static readonly string[] ExpectedKeywordFileNames = ["class.cs"];

        private static readonly UTF8Encoding StrictUtf8WithoutBom =
            new(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

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
                    "AutoGenEnum"));

            this.expectedDirectory = new DirectoryInfo(
                Path.Combine(
                    TestContext.CurrentContext.TestDirectory,
                    "Expected",
                    "UML",
                    "AutoGenEnum"));

            this.stagingDirectory = new DirectoryInfo(
                Path.Combine(
                    TestContext.CurrentContext.TestDirectory,
                    "UML",
                    "_Mycelium.SDK.AutoGenEnum"));

            if (this.stagingDirectory.Exists)
            {
                this.stagingDirectory.Delete(recursive: true);
            }

            var generator = new UmlEnumGenerator();
            var xmiReaderResult = GeneratorSetupFixture.ReadFunctionalData();

            await generator.GenerateAsync(xmiReaderResult, this.stagingDirectory);
        }

        [Test]
        [Category("Expected")]
        public void Verify_that_golden_set_matches_representative_selection()
        {
            var representativeFileNames = new RepresentativeEnumerations()
                .Select(enumerationName => $"{enumerationName}.cs")
                .OrderBy(fileName => fileName, StringComparer.Ordinal)
                .ToArray();

            var goldenFileNames = QueryRelativeFileNames(this.expectedDirectory);

            Assert.That(
                goldenFileNames,
                Is.EqualTo(representativeFileNames),
                "The reviewed enum golden set must contain exactly the bounded representative selection.");
        }

        [TestCaseSource(typeof(RepresentativeEnumerations))]
        [Category("Expected")]
        public async Task Verify_that_representative_enumerations_match_their_goldens_exactly(
            string enumerationName)
        {
            var fileName = $"{enumerationName}.cs";

            var expectedBytes = await File.ReadAllBytesAsync(
                Path.Combine(this.expectedDirectory.FullName, fileName));

            var generatedBytes = await File.ReadAllBytesAsync(
                Path.Combine(this.stagingDirectory.FullName, fileName));

            Assert.That(
                generatedBytes,
                Is.EqualTo(expectedBytes),
                $"Generated '{fileName}' differs byte-for-byte from its approved golden.");
        }

        [Test]
        public async Task Verify_that_complete_staged_output_matches_committed_AutoGenEnum()
        {
            Assert.That(
                this.committedDirectory.Exists,
                Is.True,
                "The committed SDK enum directory was not copied to the test output.");

            if (!this.committedDirectory.Exists)
            {
                return;
            }

            var stagedFileNames = QueryRelativeFileNames(this.stagingDirectory);
            var committedFileNames = QueryRelativeFileNames(this.committedDirectory);

            Assert.That(
                stagedFileNames,
                Is.EqualTo(committedFileNames),
                "The staged and committed enum manifests differ.");

            foreach (var fileName in stagedFileNames)
            {
                var stagedBytes = await File.ReadAllBytesAsync(
                    Path.Combine(this.stagingDirectory.FullName, fileName));

                var committedBytes = await File.ReadAllBytesAsync(
                    Path.Combine(this.committedDirectory.FullName, fileName));

                Assert.That(
                    stagedBytes,
                    Is.EqualTo(committedBytes),
                    $"Staged enum '{fileName}' differs byte-for-byte from the committed SDK source.");
            }
        }

        [Test]
        public async Task Verify_that_all_generated_enumerations_use_the_required_file_format()
        {
            var generatedFiles = this.stagingDirectory
                .GetFiles("*.cs", SearchOption.TopDirectoryOnly)
                .OrderBy(file => file.Name, StringComparer.Ordinal);

            foreach (var generatedFile in generatedFiles)
            {
                var bytes = await File.ReadAllBytesAsync(generatedFile.FullName);

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
                        $"Generated '{generatedFile.Name}' contains a UTF-8 byte-order mark.");

                    Assert.That(
                        source,
                        Does.Contain("\r\n"),
                        $"Generated '{generatedFile.Name}' contains no CRLF line endings.");

                    Assert.That(
                        sourceWithoutCrLf,
                        Does.Not.Contain("\r"),
                        $"Generated '{generatedFile.Name}' contains a standalone carriage return.");

                    Assert.That(
                        sourceWithoutCrLf,
                        Does.Not.Contain("\n"),
                        $"Generated '{generatedFile.Name}' contains a standalone line feed.");

                    Assert.That(
                        source,
                        Does.Contain("[GeneratedCode(\"Mycelium.SDK\", \"latest\")]"),
                        $"Generated '{generatedFile.Name}' does not contain the generated-code marker.");
                }
            }
        }

        [Test]
        public async Task Verify_that_enum_and_literal_keywords_are_escaped()
        {
            var outputDirectory =
                QueryFreshOutputDirectory("_Mycelium.SDK.KeywordAutoGenEnum");

            var enumeration = CreateEnumeration("class", "event", "Value");
            var generator = new UmlEnumGenerator();

            var source =
                await generator.GenerateEnumerationAsync(outputDirectory, enumeration);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(
                    QueryRelativeFileNames(outputDirectory),
                    Is.EqualTo(ExpectedKeywordFileNames));

                Assert.That(source, Does.Contain("enum @class"));
                Assert.That(source, Does.Contain("@event,"));
                Assert.That(source, Does.Contain("Value,"));
            }
        }

        private static Enumeration CreateEnumeration(
            string enumerationName,
            params string[] literalNames)
        {
            var enumeration = new Enumeration
            {
                XmiId = $"enumeration-{enumerationName}",
                Name = enumerationName
            };

            for (var index = 0; index < literalNames.Length; index++)
            {
                enumeration.OwnedLiteral.Add(
                    new EnumerationLiteral
                    {
                        XmiId = $"literal-{index}",
                        Name = literalNames[index]
                    });
            }

            return enumeration;
        }

        private static string[] QueryRelativeFileNames(DirectoryInfo directory)
        {
            return directory
                .GetFiles("*", SearchOption.AllDirectories)
                .Select(
                    file => Path.GetRelativePath(
                        directory.FullName,
                        file.FullName))
                .OrderBy(fileName => fileName, StringComparer.Ordinal)
                .ToArray();
        }

        private static DirectoryInfo QueryFreshOutputDirectory(string directoryName)
        {
            var directory = new DirectoryInfo(
                Path.Combine(
                    TestContext.CurrentContext.TestDirectory,
                    "UML",
                    directoryName));

            if (directory.Exists)
            {
                directory.Delete(recursive: true);
            }

            return directory;
        }
    }
}
