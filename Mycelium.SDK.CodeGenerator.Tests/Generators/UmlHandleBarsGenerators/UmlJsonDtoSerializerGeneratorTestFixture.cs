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
    using Mycelium.SDK.CodeGenerator.Generators.UmlHandleBarsGenerators;
    using Mycelium.SDK.CodeGenerator.Tests.Expected;

    using uml4net.Extensions;
    using uml4net.StructuredClassifiers;

    [TestFixture]
    public class UmlJsonDtoSerializerGeneratorTestFixture
    {

        private Dictionary<string, IClass> classes = null!;

        private DirectoryInfo committedDirectory = null!;

        private DirectoryInfo expectedDirectory = null!;

        private DirectoryInfo stagingDirectory = null!;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.committedDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Committed", "Mycelium.SDK.Serializer.Json", "AutoGenSerializer"));

            this.expectedDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Expected", "UML", "AutoGenSerializer"));

            this.stagingDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "UML", "_Mycelium.SDK.Serializer.Json.AutoGenSerializer"));

            if (this.stagingDirectory.Exists)
            {
                this.stagingDirectory.Delete(true);
            }

            var xmiReaderResult = GeneratorSetupFixture.ReadFunctionalData();

            var functionalData = GeneratorSetupFixture.QueryFunctionalDataPackage(xmiReaderResult);

            this.classes = functionalData.QueryPackages()
                .SelectMany(package => package.PackagedElement.OfType<IClass>())
                .ToDictionary(umlClass => umlClass.Name, StringComparer.Ordinal);

            var generator = new UmlJsonDtoSerializerGenerator();

            await generator.GenerateAsync(xmiReaderResult, this.stagingDirectory);
        }

        [Test]
        public async Task Verify_that_complete_staged_output_matches_committed_serializers()
        {
            Assert.That(this.committedDirectory.Exists, Is.True, "The committed serializer directory was not copied to the test output.");

            if (!this.committedDirectory.Exists)
            {
                return;
            }

            var stagedFileNames = GeneratorSetupFixture.QueryRelativeFileNames(this.stagingDirectory);

            var committedFileNames = GeneratorSetupFixture.QueryRelativeFileNames(this.committedDirectory);

            Assert.That(stagedFileNames, Is.EqualTo(committedFileNames), "The staged and committed serializer filename sets differ.");

            foreach (var fileName in stagedFileNames)
            {
                await AssertOrdinalFilesMatchAsync(Path.Combine(this.stagingDirectory.FullName, fileName), Path.Combine(this.committedDirectory.FullName, fileName), $"Staged serializer '{fileName}'", "the committed production source");
            }
        }

        [Test]
        public void Verify_that_full_batch_contains_every_concrete_DTO_and_no_abstract_DTOs()
        {
            var expectedFileNames = this.classes.Values.Where(umlClass => !umlClass.IsAbstract)
                .Select(umlClass => $"{umlClass.Name}Serializer.cs")
                .Append("SerializationProvider.cs")
                .OrderBy(fileName => fileName, StringComparer.Ordinal)
                .ToArray();

            var stagedFileNames = QueryCSharpFileNames(this.stagingDirectory);

            Assert.That(stagedFileNames, Is.EqualTo(expectedFileNames), "The serializer batch does not match the current concrete model classes.");
        }

        [Test]
        [Category("Expected")]
        public void Verify_that_golden_set_matches_non_abstract_representative_selection()
        {
            Assert.That(this.expectedDirectory.Exists, Is.True, "The representative serializer-golden directory is missing.");

            if (!this.expectedDirectory.Exists)
            {
                return;
            }

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
                    expectedFileNames.Add($"{className}Serializer.cs");
                }
            }

            var orderedExpectedFileNames = expectedFileNames.OrderBy(fileName => fileName, StringComparer.Ordinal)
                .ToArray();

            var goldenFileNames = QueryCSharpFileNames(this.expectedDirectory);

            Assert.That(goldenFileNames, Is.EqualTo(orderedExpectedFileNames), "The serializer golden set must contain exactly the non-abstract representative selection.");
        }

        [TestCaseSource(typeof(RepresentativeClasses))]
        [Category("Expected")]
        public async Task Verify_that_representative_serializers_match_their_goldens(string className)
        {
            if (!this.classes.TryGetValue(className, out var umlClass))
            {
                Assert.Fail($"Representative UML class '{className}' was not found.");

                return;
            }

            var fileName = $"{className}Serializer.cs";

            var stagedPath = Path.Combine(this.stagingDirectory.FullName, fileName);

            var expectedPath = Path.Combine(this.expectedDirectory.FullName, fileName);

            if (umlClass.IsAbstract)
            {
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(File.Exists(stagedPath), Is.False, $"Abstract class '{className}' received a serializer.");

                    Assert.That(File.Exists(expectedPath), Is.False, $"Abstract class '{className}' received a serializer golden.");
                }

                return;
            }

            await AssertOrdinalFilesMatchAsync(stagedPath, expectedPath, $"Generated serializer '{fileName}'", "its reviewed golden");
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

            var actualSource = await GeneratorSetupFixture.ReadSourceAsync(actualPath);

            var expectedSource = await GeneratorSetupFixture.ReadSourceAsync(expectedPath);

            Assert.That(string.Equals(actualSource, expectedSource, StringComparison.Ordinal), Is.True, $"{actualDescription} differs from {expectedDescription}.");
        }

        private static string[] QueryCSharpFileNames(DirectoryInfo directory)
        {
            return directory.GetFiles("*.cs", SearchOption.TopDirectoryOnly)
                .Select(file => file.Name)
                .OrderBy(fileName => fileName, StringComparer.Ordinal)
                .ToArray();
        }
    }
}
