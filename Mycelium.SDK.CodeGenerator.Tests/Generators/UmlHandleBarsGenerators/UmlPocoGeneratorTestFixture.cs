// ------------------------------------------------------------------------------------------------
//  <copyright file="UmlPocoGeneratorTestFixture.cs" company="Starion Group S.A.">
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

    using uml4net.StructuredClassifiers;

    [TestFixture]
    public class UmlPocoGeneratorTestFixture
    {
        private static readonly UTF8Encoding StrictUtf8WithoutBom = new(false, true);

        private static readonly HashSet<string> AbstractClassNames =
            new(StringComparer.Ordinal)
            {
                "AuditableThing",
                "Thing"
            };

        private static readonly string[] RepresentativeInterfaceNames =
        [
            "AuditableThing",
            "BranchProtectionRule",
            "Comment",
            "FunctionalProject",
            "FunctionalProjectPolicy",
            "Organization",
            "ProjectMember"
        ];

        private static readonly string[] RepresentativeConcreteClassNames =
        [
            "BranchProtectionRule",
            "Comment",
            "FunctionalProject",
            "FunctionalProjectPolicy",
            "Organization",
            "ProjectMember"
        ];

        private Dictionary<string, IClass> classes = null!;
        private DirectoryInfo committedDirectory = null!;
        private DirectoryInfo expectedDirectory = null!;
        private UmlPocoGenerator generator = null!;
        private DirectoryInfo stagingDirectory = null!;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.committedDirectory = new DirectoryInfo(
                Path.Combine(TestContext.CurrentContext.TestDirectory, "Committed", "Mycelium.SDK", "AutoGenPOCO"));

            this.expectedDirectory = new DirectoryInfo(
                Path.Combine(TestContext.CurrentContext.TestDirectory, "Expected", "UML", "AutoGenPOCO"));

            this.stagingDirectory = new DirectoryInfo(
                Path.Combine(TestContext.CurrentContext.TestDirectory, "UML", "_Mycelium.SDK.AutoGenPOCO"));

            if (this.stagingDirectory.Exists)
            {
                this.stagingDirectory.Delete(true);
            }

            var xmiReaderResult = XmiLoadingTestFixture.ReadFunctionalData();
            var functionalData = XmiLoadingTestFixture.QueryFunctionalDataPackage(xmiReaderResult);

            this.classes = functionalData.PackagedElement
                .OfType<IClass>()
                .ToDictionary(umlClass => umlClass.Name, StringComparer.Ordinal);

            this.generator = new UmlPocoGenerator();

            await this.generator.GenerateAsync(GeneratorSetupFixture.ResourcesDirectory, this.stagingDirectory);
        }

        [Test]
        public void Verify_that_batch_generation_produces_exactly_24_POCO_files()
        {
            var expectedClassNames = new ExpectedClasses().ToArray();

            var expectedInterfaceFileNames = expectedClassNames
                .Select(className => $"I{className}.cs")
                .OrderBy(fileName => fileName, StringComparer.Ordinal)
                .ToArray();

            var expectedConcreteFileNames = expectedClassNames
                .Where(className => !AbstractClassNames.Contains(className))
                .Select(className => $"{className}.cs")
                .OrderBy(fileName => fileName, StringComparer.Ordinal)
                .ToArray();

            var expectedFileNames = expectedInterfaceFileNames
                .Concat(expectedConcreteFileNames)
                .OrderBy(fileName => fileName, StringComparer.Ordinal)
                .ToArray();

            var generatedFileNames = QueryCSharpFileNames(this.stagingDirectory);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(expectedInterfaceFileNames, Has.Length.EqualTo(13));
                Assert.That(expectedConcreteFileNames, Has.Length.EqualTo(11));
                Assert.That(expectedFileNames, Has.Length.EqualTo(24));

                Assert.That(
                    generatedFileNames,
                    Is.EqualTo(expectedFileNames),
                    "The generated POCO set contains missing or extra files.");
            }
        }

        [Test]
        public async Task Verify_that_complete_batch_matches_committed_SDK_POCOs()
        {
            Assert.That(
                this.committedDirectory.Exists,
                Is.True,
                "The committed SDK POCO directory was not copied to the test output.");

            var generatedFileNames = QueryCSharpFileNames(this.stagingDirectory);
            var committedFileNames = QueryCSharpFileNames(this.committedDirectory);

            Assert.That(
                generatedFileNames,
                Is.EqualTo(committedFileNames),
                "The generated and committed POCO file sets differ.");

            foreach (var fileName in generatedFileNames)
            {
                var generatedBytes = await File.ReadAllBytesAsync(
                    Path.Combine(this.stagingDirectory.FullName, fileName));

                var committedBytes = await File.ReadAllBytesAsync(
                    Path.Combine(this.committedDirectory.FullName, fileName));

                Assert.That(
                    generatedBytes,
                    Is.EqualTo(committedBytes),
                    $"Generated POCO '{fileName}' differs byte-for-byte from the committed SDK source.");
            }
        }

        [Test]
        public void Verify_that_reviewed_golden_set_contains_exactly_the_representative_POCOs()
        {
            var expectedFileNames = RepresentativeInterfaceNames
                .Select(className => $"I{className}.cs")
                .Concat(RepresentativeConcreteClassNames.Select(className => $"{className}.cs"))
                .OrderBy(fileName => fileName, StringComparer.Ordinal)
                .ToArray();

            var reviewedFileNames = QueryCSharpFileNames(this.expectedDirectory);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(expectedFileNames, Has.Length.EqualTo(13));

                Assert.That(
                    reviewedFileNames,
                    Is.EqualTo(expectedFileNames),
                    "The reviewed POCO golden-file set must contain exactly the representative files.");
            }
        }

        [TestCase(GeneratorSetupFixture.ClassPreflightFailure.InvalidIdentifier)]
        [TestCase(GeneratorSetupFixture.ClassPreflightFailure.InvalidRenderedSyntax)]
        [TestCase(GeneratorSetupFixture.ClassPreflightFailure.DuplicateFileName)]
        [TestCase(GeneratorSetupFixture.ClassPreflightFailure.InvalidModelReference)]
        [TestCase(GeneratorSetupFixture.ClassPreflightFailure.UnexpectedManifest)]
        public async Task Verify_that_batch_preflight_failure_leaves_destination_unchanged(
            GeneratorSetupFixture.ClassPreflightFailure failure)
        {
            await AssertBatchPreflightFailureLeavesDestinationUntouched(failure, false);
            await AssertBatchPreflightFailureLeavesDestinationUntouched(failure, true);
        }

        private static async Task AssertBatchPreflightFailureLeavesDestinationUntouched(
            GeneratorSetupFixture.ClassPreflightFailure failure,
            bool destinationExists)
        {
            var state = destinationExists ? "Existing" : "Absent";

            var outputDirectory =
                GeneratorSetupFixture.QueryFreshOutputDirectory(
                    $"_Mycelium.SDK.InvalidAutoGenPOCO.{failure}.{state}");

            IReadOnlyDictionary<string, byte[]> expectedSnapshot = null;

            if (destinationExists)
            {
                outputDirectory.Create();

                await File.WriteAllBytesAsync(
                    Path.Combine(outputDirectory.FullName, "IThing.cs"),
                    new byte[] { 0x01, 0x02, 0x03 });

                var nestedDirectory = outputDirectory.CreateSubdirectory("preserve");

                await File.WriteAllBytesAsync(
                    Path.Combine(nestedDirectory.FullName, "keep.bin"),
                    new byte[] { 0x04, 0x05, 0x06 });

                expectedSnapshot = await GeneratorSetupFixture.QueryDirectorySnapshotAsync(outputDirectory);
            }

            var xmiReaderResult = XmiLoadingTestFixture.ReadFunctionalData();
            var functionalData = XmiLoadingTestFixture.QueryFunctionalDataPackage(xmiReaderResult);

            var classes = functionalData.PackagedElement
                .OfType<IClass>()
                .ToDictionary(umlClass => umlClass.Name, StringComparer.Ordinal);

            var generator = new UmlPocoGenerator();

            switch (failure)
            {
                case GeneratorSetupFixture.ClassPreflightFailure.InvalidIdentifier:
                    classes["Thing"].Name = "Invalid-Thing";
                    break;

                case GeneratorSetupFixture.ClassPreflightFailure.InvalidRenderedSyntax:
                    generator.Templates["poco-class-uml-template"] =
                        (_, _) => "namespace Mycelium.SDK\r\n{";

                    break;

                case GeneratorSetupFixture.ClassPreflightFailure.DuplicateFileName:
                    GeneratorSetupFixture.RegisterPostFirstRenderMutation(
                        generator,
                        "poco-interface-uml-template",
                        () => classes["Review"].Name = "User");

                    break;

                case GeneratorSetupFixture.ClassPreflightFailure.InvalidModelReference:
                    classes["Thing"].OwnedAttribute
                        .Single(property => property.Name == "id")
                        .Type = null!;

                    break;

                case GeneratorSetupFixture.ClassPreflightFailure.UnexpectedManifest:
                    GeneratorSetupFixture.RegisterPostFirstRenderMutation(
                        generator,
                        "poco-interface-uml-template",
                        () => classes["Review"].Name = "UnexpectedReview");

                    break;

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(failure),
                        failure,
                        "Unsupported POCO preflight failure.");
            }

            if (failure == GeneratorSetupFixture.ClassPreflightFailure.InvalidIdentifier)
            {
                await Assert.ThatAsync(
                    () => generator.GenerateAsync(xmiReaderResult, outputDirectory),
                    Throws.TypeOf<ArgumentException>());
            }
            else
            {
                var expectedMessage = failure switch
                {
                    GeneratorSetupFixture.ClassPreflightFailure.InvalidRenderedSyntax =>
                        "invalid C#",

                    GeneratorSetupFixture.ClassPreflightFailure.DuplicateFileName =>
                        "POCO generation produced duplicate filename",

                    GeneratorSetupFixture.ClassPreflightFailure.InvalidModelReference =>
                        "no resolved type",

                    GeneratorSetupFixture.ClassPreflightFailure.UnexpectedManifest =>
                        "POCO generation produced an unexpected manifest",

                    _ => throw new ArgumentOutOfRangeException(
                        nameof(failure),
                        failure,
                        "Unsupported POCO preflight failure.")
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
                    $"Preflight failure '{failure}' created the absent POCO destination.");

                return;
            }

            Assert.That(
                outputDirectory.Exists,
                Is.True,
                $"Preflight failure '{failure}' removed the existing POCO destination.");

            await GeneratorSetupFixture.AssertDirectoryMatchesSnapshotAsync(outputDirectory, expectedSnapshot);
        }

        [Test]
        public async Task Verify_that_generated_POCOs_use_the_required_file_format()
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
                        $"Generated POCO '{fileName}' contains a UTF-8 byte-order mark.");

                    Assert.That(
                        source,
                        Does.Contain("\r\n"),
                        $"Generated POCO '{fileName}' contains no CRLF line endings.");

                    Assert.That(
                        sourceWithoutCrLf,
                        Does.Not.Contain("\r"),
                        $"Generated POCO '{fileName}' contains a standalone carriage return.");

                    Assert.That(
                        sourceWithoutCrLf,
                        Does.Not.Contain("\n"),
                        $"Generated POCO '{fileName}' contains a standalone line feed.");
                }
            }
        }

        [TestCaseSource(nameof(RepresentativeInterfaceNames))]
        [Category("Expected")]
        public async Task Verify_that_representative_POCO_interface_matches_reviewed_golden_file(string className)
        {
            var fileName = $"I{className}.cs";

            await this.generator.GeneratePocoInterfaceAsync(this.stagingDirectory, this.classes[className]);

            var expectedBytes = await File.ReadAllBytesAsync(
                Path.Combine(this.expectedDirectory.FullName, fileName));

            var generatedBytes = await File.ReadAllBytesAsync(
                Path.Combine(this.stagingDirectory.FullName, fileName));

            Assert.That(
                generatedBytes,
                Is.EqualTo(expectedBytes),
                $"Generated interface '{fileName}' differs byte-for-byte from its reviewed golden file.");
        }

        [TestCaseSource(nameof(RepresentativeConcreteClassNames))]
        [Category("Expected")]
        public async Task Verify_that_representative_POCO_class_matches_reviewed_golden_file(string className)
        {
            var fileName = $"{className}.cs";

            await this.generator.GeneratePocoClassAsync(this.stagingDirectory, this.classes[className]);

            var expectedBytes = await File.ReadAllBytesAsync(
                Path.Combine(this.expectedDirectory.FullName, fileName));

            var generatedBytes = await File.ReadAllBytesAsync(
                Path.Combine(this.stagingDirectory.FullName, fileName));

            Assert.That(
                generatedBytes,
                Is.EqualTo(expectedBytes),
                $"Generated class '{fileName}' differs byte-for-byte from its reviewed golden file.");
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
