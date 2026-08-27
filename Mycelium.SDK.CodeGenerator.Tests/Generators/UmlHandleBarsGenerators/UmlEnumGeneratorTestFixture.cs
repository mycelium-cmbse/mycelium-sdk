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
    using System.Text;

    using Mycelium.SDK.CodeGenerator.Generators.UmlHandleBarsGenerators;
    using Mycelium.SDK.CodeGenerator.Tests.Expected;
    using Mycelium.SDK.CodeGenerator.Tests.Xmi;

    using uml4net.SimpleClassifiers;
    using uml4net.StructuredClassifiers;
    using uml4net.xmi.Readers;

    [TestFixture]
    public class UmlEnumGeneratorTestFixture
    {
        private const string TemplateName = "enumeration-uml-template";
        private static readonly string[] ExpectedKeywordFileNames = ["class.cs"];
        private static readonly UTF8Encoding StrictUtf8WithoutBom = new(false, true);

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
                Path.Combine(TestContext.CurrentContext.TestDirectory,
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
                this.stagingDirectory.Delete(true);
            }

            var generator = new UmlEnumGenerator();

            await generator.GenerateAsync(GeneratorSetupFixture.ResourcesDirectory, this.stagingDirectory);
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
                var stagedBytes = await File.ReadAllBytesAsync(Path.Combine(this.stagingDirectory.FullName, fileName));
                var committedBytes = await File.ReadAllBytesAsync(Path.Combine(this.committedDirectory.FullName, fileName));

                Assert.That(
                    stagedBytes,
                    Is.EqualTo(committedBytes),
                    $"Staged enum '{fileName}' differs byte-for-byte from the committed SDK source.");
            }
        }

        [Test]
        public async Task Verify_that_enum_and_literal_keywords_are_escaped()
        {
            var outputDirectory = GeneratorSetupFixture.QueryFreshOutputDirectory("_Mycelium.SDK.KeywordAutoGenEnum");
            var enumeration = CreateEnumeration("class", "event", "Value");
            var generator = new UmlEnumGenerator();

            var source = await generator.GenerateEnumerationAsync(outputDirectory, enumeration);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(QueryRelativeFileNames(outputDirectory), Is.EqualTo(ExpectedKeywordFileNames));
                Assert.That(source, Does.Contain("enum @class"));
                Assert.That(source, Does.Contain("@event,"));
                Assert.That(source, Does.Contain("Value,"));
            }
        }

        [Test]
        public void Verify_that_golden_staged_and_committed_manifests_are_exact()
        {
            var expectedFileNames = QueryExpectedFileNames();
            var reviewedFileNames = QueryRelativeFileNames(this.expectedDirectory);
            var stagedFileNames = QueryRelativeFileNames(this.stagingDirectory);

            var committedFileNames = this.committedDirectory.Exists
                ? QueryRelativeFileNames(this.committedDirectory)
                : Array.Empty<string>();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(this.committedDirectory.Exists, Is.True,
                    "The committed SDK enum directory was not copied to the test output.");

                Assert.That(expectedFileNames, Has.Length.EqualTo(8));

                Assert.That(reviewedFileNames, Is.EqualTo(expectedFileNames),
                    "The reviewed golden manifest contains missing or extra files.");

                Assert.That(stagedFileNames, Is.EqualTo(expectedFileNames),
                    "The staged enum manifest contains missing or extra files.");

                Assert.That(committedFileNames, Is.EqualTo(expectedFileNames),
                    "The committed AutoGenEnum manifest contains missing or extra files.");
            }
        }

        [TestCaseSource(typeof(ExpectedEnumerations))]
        [Category("Expected")]
        public async Task Verify_that_every_generated_enumeration_matches_its_golden_exactly(string enumerationName)
        {
            var fileName = $"{enumerationName}.cs";

            var expectedBytes = await File.ReadAllBytesAsync(Path.Combine(this.expectedDirectory.FullName, fileName));
            var generatedBytes = await File.ReadAllBytesAsync(Path.Combine(this.stagingDirectory.FullName, fileName));

            Assert.That(
                generatedBytes,
                Is.EqualTo(expectedBytes),
                $"Generated '{fileName}' differs byte-for-byte from its approved golden.");
        }

        [TestCaseSource(typeof(ExpectedEnumerations))]
        public async Task Verify_that_generated_enumerations_use_the_required_file_format(string enumerationName)
        {
            var fileName = $"{enumerationName}.cs";
            var bytes = await File.ReadAllBytesAsync(Path.Combine(this.stagingDirectory.FullName, fileName));

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
                    $"Generated '{fileName}' contains a UTF-8 byte-order mark.");

                Assert.That(
                    source,
                    Does.Contain("\r\n"),
                    $"Generated '{fileName}' contains no CRLF line endings.");

                Assert.That(
                    sourceWithoutCrLf,
                    Does.Not.Contain("\r"),
                    $"Generated '{fileName}' contains a standalone carriage return.");

                Assert.That(
                    sourceWithoutCrLf,
                    Does.Not.Contain("\n"),
                    $"Generated '{fileName}' contains a standalone line feed.");

                Assert.That(
                    source,
                    Does.Contain("[GeneratedCode(\"Mycelium.SDK\", \"latest\")]"),
                    $"Generated '{fileName}' does not contain the generated-code marker.");
            }
        }

        [TestCase("Invalid-Type", "Value", "InvalidType")]
        [TestCase("ValidType", "Invalid-Literal", "InvalidLiteral")]
        public async Task Verify_that_single_enum_identifier_failure_occurs_before_destination_creation(
            string enumerationName,
            string literalName,
            string testSuffix)
        {
            var outputDirectory = GeneratorSetupFixture.QueryFreshOutputDirectory($"_Mycelium.SDK.{testSuffix}.AutoGenEnum");
            var enumeration = CreateEnumeration(enumerationName, literalName);

            var generator = new UmlEnumGenerator();

            await Assert.ThatAsync(
                () => generator.GenerateEnumerationAsync(outputDirectory, enumeration),
                Throws.TypeOf<ArgumentException>());

            outputDirectory.Refresh();

            Assert.That(
                outputDirectory.Exists,
                Is.False,
                "Single-enum validation created the destination directory.");
        }

        [TestCase(BatchPreflightFailure.InvalidEnumerationIdentifier)]
        [TestCase(BatchPreflightFailure.InvalidLiteralIdentifier)]
        [TestCase(BatchPreflightFailure.DuplicateLiteralIdentifier)]
        [TestCase(BatchPreflightFailure.InvalidModelReference)]
        [TestCase(BatchPreflightFailure.DuplicateFileName)]
        [TestCase(BatchPreflightFailure.InvalidRenderedSyntax)]
        [TestCase(BatchPreflightFailure.UnexpectedManifest)]
        public async Task Verify_that_batch_preflight_failure_leaves_no_new_or_modified_output(BatchPreflightFailure failure)
        {
            await AssertBatchPreflightFailureLeavesDestinationUntouched(failure, false);
            await AssertBatchPreflightFailureLeavesDestinationUntouched(failure, true);
        }

        private static void ApplyBatchPreflightFailure(BatchPreflightFailure failure, XmiReaderResult xmiReaderResult, UmlEnumGenerator generator)
        {
            switch (failure)
            {
                case BatchPreflightFailure.InvalidEnumerationIdentifier:
                    QueryEnumeration(xmiReaderResult, "ReviewStatus")
                        .Name = "Review-Status";

                    break;

                case BatchPreflightFailure.InvalidLiteralIdentifier:
                    QueryEnumeration(xmiReaderResult, "ReviewStatus")
                        .OwnedLiteral[0]
                        .Name = "Invalid-Literal";

                    break;

                case BatchPreflightFailure.DuplicateLiteralIdentifier:
                    var reviewStatus = QueryEnumeration(xmiReaderResult, "ReviewStatus");

                    reviewStatus.OwnedLiteral[1].Name = reviewStatus.OwnedLiteral[0].Name;
                    break;

                case BatchPreflightFailure.InvalidModelReference:
                    var functionalData = XmiLoadingTestFixture.QueryFunctionalDataPackage(xmiReaderResult);

                    functionalData.PackagedElement
                        .OfType<IClass>()
                        .Single(umlClass => umlClass.Name == "Thing")
                        .OwnedAttribute
                        .Single(property => property.Name == "id")
                        .Type = null!;

                    break;

                case BatchPreflightFailure.DuplicateFileName:
                    var duplicateFileNameEnumeration =
                        QueryEnumeration(xmiReaderResult, "ReviewStatus");

                    GeneratorSetupFixture.RegisterPostFirstRenderMutation(
                        generator,
                        TemplateName,
                        () => duplicateFileNameEnumeration.Name = "ActivationStatus");

                    break;

                case BatchPreflightFailure.InvalidRenderedSyntax:
                    generator.Templates[TemplateName] = (_, _) => "namespace Mycelium.SDK\r\n{";
                    break;

                case BatchPreflightFailure.UnexpectedManifest:
                    var unexpectedManifestEnumeration = QueryEnumeration(xmiReaderResult, "ReviewStatus");

                    GeneratorSetupFixture.RegisterPostFirstRenderMutation(
                        generator,
                        TemplateName,
                        () => unexpectedManifestEnumeration.Name = "UnexpectedStatus");

                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(failure), failure, "Unsupported enum preflight failure.");
            }
        }

        private static Enumeration CreateEnumeration(string enumerationName, params string[] literalNames)
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

        private static string[] QueryExpectedFileNames()
        {
            return new ExpectedEnumerations()
                .Select(enumerationName => $"{enumerationName}.cs")
                .OrderBy(fileName => fileName, StringComparer.Ordinal)
                .ToArray();
        }

        private static IEnumeration QueryEnumeration(XmiReaderResult xmiReaderResult, string enumerationName)
        {
            var functionalData =
                XmiLoadingTestFixture.QueryFunctionalDataPackage(
                    xmiReaderResult);

            return functionalData.PackagedElement
                .OfType<IEnumeration>()
                .Single(enumeration => string.Equals(
                    enumeration.Name,
                    enumerationName,
                    StringComparison.Ordinal));
        }

        private static string[] QueryRelativeFileNames(DirectoryInfo directory)
        {
            return directory
                .GetFiles("*", SearchOption.AllDirectories)
                .Select(file => Path.GetRelativePath(directory.FullName, file.FullName))
                .OrderBy(fileName => fileName, StringComparer.Ordinal)
                .ToArray();
        }

        private static async Task AssertBatchPreflightFailureLeavesDestinationUntouched(
            BatchPreflightFailure failure,
            bool destinationExists)
        {
            var state = destinationExists ? "Existing" : "Absent";
            var outputDirectory = GeneratorSetupFixture.QueryFreshOutputDirectory($"_Mycelium.SDK.InvalidAutoGenEnum.{failure}.{state}");

            IReadOnlyDictionary<string, byte[]> expectedSnapshot = null;

            if (destinationExists)
            {
                outputDirectory.Create();

                await File.WriteAllBytesAsync(
                    Path.Combine(outputDirectory.FullName, "ActivationStatus.cs"),
                    new byte[] { 0x01, 0x02, 0x03 });

                var nestedDirectory = outputDirectory.CreateSubdirectory("preserve");

                await File.WriteAllTextAsync(
                    Path.Combine(nestedDirectory.FullName, "keep.txt"),
                    "existing content",
                    StrictUtf8WithoutBom);

                expectedSnapshot = await GeneratorSetupFixture.QueryDirectorySnapshotAsync(outputDirectory);
            }

            var xmiReaderResult = XmiLoadingTestFixture.ReadFunctionalData();
            var generator = new UmlEnumGenerator();

            ApplyBatchPreflightFailure(failure, xmiReaderResult, generator);

            if (failure is
                BatchPreflightFailure.InvalidEnumerationIdentifier
                or BatchPreflightFailure.InvalidLiteralIdentifier)
            {
                await Assert.ThatAsync(
                    () => generator.GenerateAsync(xmiReaderResult, outputDirectory),
                    Throws.TypeOf<ArgumentException>());
            }
            else
            {
                var expectedMessage = failure switch
                {
                    BatchPreflightFailure.DuplicateLiteralIdentifier =>
                        "duplicate C# literal identifier",

                    BatchPreflightFailure.InvalidModelReference =>
                        "no resolved type",

                    BatchPreflightFailure.DuplicateFileName =>
                        "Enumeration generation produced duplicate filename",

                    BatchPreflightFailure.InvalidRenderedSyntax =>
                        "invalid C#",

                    BatchPreflightFailure.UnexpectedManifest =>
                        "Enumeration generation produced an unexpected manifest",

                    _ => throw new ArgumentOutOfRangeException(
                        nameof(failure),
                        failure,
                        "Unsupported enum preflight failure.")
                };

                await Assert.ThatAsync(
                    () => generator.GenerateAsync(xmiReaderResult, outputDirectory),
                    Throws.TypeOf<InvalidOperationException>()
                        .With.Message.Contains(expectedMessage));
            }

            outputDirectory.Refresh();

            if (!destinationExists)
            {
                Assert.That(
                    outputDirectory.Exists,
                    Is.False,
                    $"Preflight failure '{failure}' created the destination directory.");

                return;
            }

            Assert.That(outputDirectory.Exists, Is.True, $"Preflight failure '{failure}' removed the existing destination.");

            await GeneratorSetupFixture.AssertDirectoryMatchesSnapshotAsync(outputDirectory, expectedSnapshot);
        }

        public enum BatchPreflightFailure
        {
            InvalidEnumerationIdentifier,
            InvalidLiteralIdentifier,
            DuplicateLiteralIdentifier,
            InvalidModelReference,
            DuplicateFileName,
            InvalidRenderedSyntax,
            UnexpectedManifest
        }
    }
}
