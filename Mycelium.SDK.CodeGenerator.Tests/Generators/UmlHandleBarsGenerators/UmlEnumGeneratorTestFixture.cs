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
    using Mycelium.SDK.CodeGenerator.Generators.UmlHandleBarsGenerators;
    using Mycelium.SDK.CodeGenerator.Tests.Expected;

    using uml4net.SimpleClassifiers;

    [TestFixture]
    public class UmlEnumGeneratorTestFixture
    {
        private static readonly string[] ExpectedKeywordFileNames = ["class.cs"];


        private DirectoryInfo committedDirectory = null!;
        private DirectoryInfo expectedDirectory = null!;
        private DirectoryInfo stagingDirectory = null!;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.committedDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Committed", "Mycelium.SDK", "AutoGenEnum"));

            this.expectedDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Expected", "UML", "AutoGenEnum"));

            this.stagingDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "UML", "_Mycelium.SDK.AutoGenEnum"));

            if (this.stagingDirectory.Exists)
            {
                this.stagingDirectory.Delete(true);
            }

            var generator = new UmlEnumGenerator();
            var xmiReaderResult = GeneratorSetupFixture.ReadFunctionalData();

            await generator.GenerateAsync(xmiReaderResult, this.stagingDirectory);
        }

        [Test]
        public async Task Verify_that_complete_staged_output_matches_committed_AutoGenEnum()
        {
            Assert.That(this.committedDirectory.Exists, Is.True, "The committed SDK enum directory was not copied to the test output.");

            if (!this.committedDirectory.Exists)
            {
                return;
            }

            var stagedFileNames = GeneratedOutput.QueryRelativeFileNames(this.stagingDirectory);
            var committedFileNames = GeneratedOutput.QueryRelativeFileNames(this.committedDirectory);

            Assert.That(stagedFileNames, Is.EqualTo(committedFileNames), "The staged and committed enum manifests differ.");

            foreach (var fileName in stagedFileNames)
            {
                var stagedSource = await GeneratorSetupFixture.ReadSourceAsync(Path.Combine(this.stagingDirectory.FullName, fileName));

                var committedSource = await GeneratorSetupFixture.ReadSourceAsync(Path.Combine(this.committedDirectory.FullName, fileName));

                Assert.That(stagedSource, Is.EqualTo(committedSource), $"Staged enum '{fileName}' differs from the committed SDK source.");
            }
        }

        [Test]
        public async Task Verify_that_enum_and_literal_keywords_are_escaped()
        {
            var outputDirectory = QueryFreshOutputDirectory("_Mycelium.SDK.KeywordAutoGenEnum");

            var enumeration = CreateEnumeration("class", "event", "Value");
            var generator = new UmlEnumGenerator();

            var source = await generator.GenerateEnumerationAsync(outputDirectory, enumeration);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(GeneratedOutput.QueryRelativeFileNames(outputDirectory), Is.EqualTo(ExpectedKeywordFileNames));

                Assert.That(source, Does.Contain("enum @class"));
                Assert.That(source, Does.Contain("@event,"));
                Assert.That(source, Does.Contain("Value,"));
            }
        }

        [Test]
        [Category("Expected")]
        public void Verify_that_golden_set_matches_representative_selection()
        {
            var representativeFileNames = new RepresentativeEnumerations()
                .Select(enumerationName => $"{enumerationName}.cs")
                .OrderBy(fileName => fileName, StringComparer.Ordinal)
                .ToArray();

            var goldenFileNames = GeneratedOutput.QueryRelativeFileNames(this.expectedDirectory);

            Assert.That(goldenFileNames, Is.EqualTo(representativeFileNames), "The reviewed enum golden set must contain exactly the bounded representative selection.");
        }

        [TestCaseSource(typeof(RepresentativeEnumerations))]
        [Category("Expected")]
        public async Task Verify_that_representative_enumerations_match_their_goldens_exactly(string enumerationName)
        {
            var fileName = $"{enumerationName}.cs";

            var expectedSource = await GeneratorSetupFixture.ReadSourceAsync(Path.Combine(this.expectedDirectory.FullName, fileName));

            var generatedSource = await GeneratorSetupFixture.ReadSourceAsync(Path.Combine(this.stagingDirectory.FullName, fileName));

            Assert.That(generatedSource, Is.EqualTo(expectedSource), $"Generated '{fileName}' differs from its approved golden.");
        }

        private static Enumeration CreateEnumeration(string enumerationName, params string[] literalNames)
        {
            var enumeration = new Enumeration { XmiId = $"enumeration-{enumerationName}", Name = enumerationName };

            for (var index = 0; index < literalNames.Length; index++)
            {
                enumeration.OwnedLiteral.Add(new EnumerationLiteral { XmiId = $"literal-{index}", Name = literalNames[index] });
            }

            return enumeration;
        }

        private static DirectoryInfo QueryFreshOutputDirectory(string directoryName)
        {
            var directory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "UML", directoryName));

            if (directory.Exists)
            {
                directory.Delete(true);
            }

            return directory;
        }
    }
}
