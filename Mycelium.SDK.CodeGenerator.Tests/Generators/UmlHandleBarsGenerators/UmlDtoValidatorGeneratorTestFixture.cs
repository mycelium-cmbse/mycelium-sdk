// ------------------------------------------------------------------------------------------------
//  <copyright file="UmlDtoValidatorGeneratorTestFixture.cs" company="Starion Group S.A.">
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
    public class UmlDtoValidatorGeneratorTestFixture : UmlClassGeneratorTestFixtureBase
    {
        private Dictionary<string, IClass> classes = [];
        private DirectoryInfo committedDirectory;
        private DirectoryInfo expectedDirectory;
        private DirectoryInfo stagingDirectory;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.committedDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Committed", "Mycelium.SDK.Validation", "AutoGenDtoValidator"));

            this.expectedDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Expected", "UML", "AutoGenDtoValidator"));

            this.stagingDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "UML", "_Mycelium.SDK.Validation.AutoGenDtoValidator"));

            if (this.stagingDirectory.Exists)
            {
                this.stagingDirectory.Delete(true);
            }

            var xmiReaderResult = GeneratorSetupFixture.ReadFunctionalData();
            var fabric = GeneratorSetupFixture.QueryFunctionalDataPackage(xmiReaderResult);

            this.classes = fabric.QueryPackages()
                .SelectMany(package => package.PackagedElement.OfType<IClass>())
                .ToDictionary(umlClass => umlClass.Name, StringComparer.Ordinal);

            var generator = new UmlDtoValidatorGenerator();

            await generator.GenerateAsync(xmiReaderResult, this.stagingDirectory);
        }

        [Test]
        public async Task VerifyThatCompleteBatchMatchesCommittedValidators()
        {
            Assert.That(this.committedDirectory.Exists, Is.True, "The committed validator directory was not copied to the test output.");

            var stagedFileNames = GeneratedOutput.QueryRelativeFileNames(this.stagingDirectory);
            var committedFileNames = GeneratedOutput.QueryRelativeFileNames(this.committedDirectory);

            Assert.That(stagedFileNames, Is.EqualTo(committedFileNames), "The staged and committed validator filename sets differ.");

            foreach (var fileName in stagedFileNames)
            {
                await AssertFilesMatchAsync(Path.Combine(this.stagingDirectory.FullName, fileName),
                    Path.Combine(this.committedDirectory.FullName, fileName), $"Generated validator '{fileName}'", "the committed production source");
            }
        }

        [Test]
        [Category("Expected")]
        public async Task VerifyThatRepresentativeValidatorsMatchReviewedGoldens()
        {
            Assert.That(this.expectedDirectory.Exists, Is.True, "The representative validator-golden directory is missing.");

            var representativeFileNames = this.QueryRepresentativeFileNames();
            var goldenFileNames = GeneratedOutput.QueryRelativeFileNames(this.expectedDirectory);

            Assert.That(goldenFileNames, Is.EqualTo(representativeFileNames), "The validator golden set must match the concrete representative selection.");

            foreach (var fileName in representativeFileNames)
            {
                await AssertFilesMatchAsync(Path.Combine(this.stagingDirectory.FullName, fileName),
                    Path.Combine(this.expectedDirectory.FullName, fileName), $"Generated validator '{fileName}'", "its reviewed golden");
            }
        }

        private string[] QueryRepresentativeFileNames()
        {
            return new RepresentativeClasses()
                .Select(className => this.classes[className])
                .Where(umlClass => !umlClass.IsAbstract)
                .Select(umlClass => $"{umlClass.Name}Validator.cs")
                .OrderBy(fileName => fileName, StringComparer.Ordinal)
                .ToArray();
        }
    }
}
