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
    using Mycelium.SDK.CodeGenerator.Generators.UmlHandleBarsGenerators;
    using Mycelium.SDK.CodeGenerator.Tests.Expected;

    using uml4net.StructuredClassifiers;
    using uml4net.xmi.Readers;

    [TestFixture]
    public class UmlDtoGeneratorTestFixture : UmlClassGeneratorTestFixtureBase
    {
        private Dictionary<string, IClass> classes = null!;
        private DirectoryInfo committedDirectory = null!;
        private DirectoryInfo expectedDirectory = null!;
        private DirectoryInfo stagingDirectory = null!;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.committedDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Committed", "Mycelium.SDK", "AutoGenDTO"));

            this.expectedDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Expected", "UML", "AutoGenDTO"));

            this.stagingDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "UML", "_Mycelium.SDK.AutoGenDTO"));

            if (this.stagingDirectory.Exists)
            {
                this.stagingDirectory.Delete(true);
            }

            var xmiReaderResult = GeneratorSetupFixture.ReadFunctionalData();

            var functionalData = GeneratorSetupFixture.QueryFunctionalDataPackage(xmiReaderResult);

            this.classes = functionalData.PackagedElement.OfType<IClass>()
                .ToDictionary(umlClass => umlClass.Name, StringComparer.Ordinal);

            var generator = new UmlDtoGenerator();

            await generator.GenerateAsync(xmiReaderResult, this.stagingDirectory);
        }

        [Test]
        public async Task Verify_that_complete_batch_matches_committed_SDK_DTOs()
        {
            Assert.That(this.committedDirectory.Exists, Is.True, "The committed SDK DTO directory was not copied to the test output.");

            if (!this.committedDirectory.Exists)
            {
                return;
            }

            var generatedFileNames = GeneratorSetupFixture.QueryRelativeFileNames(this.stagingDirectory);
            var committedFileNames = GeneratorSetupFixture.QueryRelativeFileNames(this.committedDirectory);

            Assert.That(generatedFileNames, Is.EqualTo(committedFileNames), "The generated and committed DTO file sets differ.");

            foreach (var fileName in generatedFileNames)
            {
                await AssertFilesMatchAsync(Path.Combine(this.stagingDirectory.FullName, fileName), Path.Combine(this.committedDirectory.FullName, fileName), $"Generated DTO '{fileName}'", "the committed SDK source");
            }
        }

        [Test]
        [TestCaseSource(typeof(RepresentativeClasses))]
        [Category("Expected")]
        public async Task Verify_that_representative_DTOs_match_reviewed_goldens(string className)
        {
            Assert.That(this.classes.TryGetValue(className, out var umlClass), Is.True, $"Representative UML class '{className}' was not found.");

            if (umlClass is null)
            {
                return;
            }

            var interfaceFileName = $"I{className}.cs";

            await AssertFilesMatchAsync(Path.Combine(this.stagingDirectory.FullName, interfaceFileName), Path.Combine(this.expectedDirectory.FullName, interfaceFileName), $"Generated DTO interface '{interfaceFileName}'", "its reviewed golden");

            if (umlClass.IsAbstract)
            {
                return;
            }

            var classFileName = $"{className}.cs";

            await AssertFilesMatchAsync(Path.Combine(this.stagingDirectory.FullName, classFileName), Path.Combine(this.expectedDirectory.FullName, classFileName), $"Generated DTO class '{classFileName}'", "its reviewed golden");
        }

        [TestCase("UUID", "Guid")]
        [TestCase("Guid", "Guid")]
        [TestCase("URI", "Uri")]
        [TestCase("Uri", "Uri")]
        [TestCase("DateTime", "DateTime")]
        [TestCase("Dictionary<string,string>", "Dictionary{string,string}")]
        [TestCase("Integer", "int")]
        public void VerifyThatDocumentationReferencesUseTheSharedValueMappings(string reference, string expectedSymbol)
        {
            var generator = new DocumentationSymbolGenerator();
            var result = GeneratorSetupFixture.ReadFunctionalData();

            Assert.That(generator.Resolve(result, reference), Is.EqualTo(expectedSymbol));
        }

        private sealed class DocumentationSymbolGenerator : UmlHandleBarsGenerator
        {
            public string Resolve(XmiReaderResult result, string reference)
            {
                this.ConfigureDocumentationSymbols(CreateHandlebarsPayload(result));

                return this.ResolveDocumentationCref(reference);
            }

            public override Task GenerateAsync(XmiReaderResult xmiReaderResult, DirectoryInfo outputDirectory) => Task.CompletedTask;

            protected override void RegisterHelpers()
            {
            }

            protected override void RegisterTemplates()
            {
            }
        }
    }
}
