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
    using Mycelium.SDK.CodeGenerator.Generators.UmlHandleBarsGenerators;
    using Mycelium.SDK.CodeGenerator.Tests.Expected;

    using uml4net.StructuredClassifiers;

    [TestFixture]
    public class UmlPocoGeneratorTestFixture : UmlClassGeneratorTestFixtureBase
    {
        private Dictionary<string, IClass> classes = null!;
        private DirectoryInfo committedDirectory = null!;
        private DirectoryInfo expectedDirectory = null!;
        private DirectoryInfo stagingDirectory = null!;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.committedDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Committed", "Mycelium.SDK", "AutoGenPOCO"));

            this.expectedDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Expected", "UML", "AutoGenPOCO"));

            this.stagingDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "UML", "_Mycelium.SDK.AutoGenPOCO"));

            if (this.stagingDirectory.Exists)
            {
                this.stagingDirectory.Delete(true);
            }

            var xmiReaderResult = GeneratorSetupFixture.ReadFunctionalData();

            var functionalData = GeneratorSetupFixture.QueryFunctionalDataPackage(xmiReaderResult);

            this.classes = functionalData.PackagedElement
                .OfType<IClass>()
                .ToDictionary(umlClass => umlClass.Name, StringComparer.Ordinal);

            var generator = new UmlPocoGenerator();

            await generator.GenerateAsync(xmiReaderResult, this.stagingDirectory);
        }

        [Test]
        public async Task Verify_that_complete_batch_matches_committed_SDK_POCOs()
        {
            Assert.That(this.committedDirectory.Exists, Is.True, "The committed SDK POCO directory was not copied to the test output.");

            if (!this.committedDirectory.Exists)
            {
                return;
            }

            var generatedFileNames = GeneratorSetupFixture.QueryRelativeFileNames(this.stagingDirectory);
            var committedFileNames = GeneratorSetupFixture.QueryRelativeFileNames(this.committedDirectory);

            Assert.That(generatedFileNames, Is.EqualTo(committedFileNames), "The generated and committed POCO file sets differ.");

            foreach (var fileName in generatedFileNames)
            {
                await AssertFilesMatchAsync(Path.Combine(this.stagingDirectory.FullName, fileName), Path.Combine(this.committedDirectory.FullName, fileName), $"Generated POCO '{fileName}'", "the committed SDK source");
            }
        }

        [Test]
        [TestCaseSource(typeof(RepresentativeClasses))]
        [Category("Expected")]
        public async Task Verify_that_representative_POCOs_match_reviewed_goldens(string className)
        {
            Assert.That(this.classes.TryGetValue(className, out var umlClass), Is.True, $"Representative UML class '{className}' was not found.");

            if (umlClass is null)
            {
                return;
            }

            var interfaceFileName = $"I{className}.cs";

            await AssertFilesMatchAsync(Path.Combine(this.stagingDirectory.FullName, interfaceFileName), Path.Combine(this.expectedDirectory.FullName, interfaceFileName), $"Generated POCO interface '{interfaceFileName}'", "its reviewed golden");

            if (umlClass.IsAbstract)
            {
                return;
            }

            var classFileName = $"{className}.cs";

            await AssertFilesMatchAsync(Path.Combine(this.stagingDirectory.FullName, classFileName), Path.Combine(this.expectedDirectory.FullName, classFileName), $"Generated POCO class '{classFileName}'", "its reviewed golden");
        }
    }
}
