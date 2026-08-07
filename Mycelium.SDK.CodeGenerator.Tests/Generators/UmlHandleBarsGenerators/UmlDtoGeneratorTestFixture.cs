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
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Mycelium.SDK.CodeGenerator.Generators.UmlHandleBarsGenerators;
    using Mycelium.SDK.CodeGenerator.Tests.Expected;
    using Mycelium.SDK.CodeGenerator.Tests.Xmi;

    using uml4net.StructuredClassifiers;

    /// <summary>
    /// Verifies FunctionalData DTO generation against reviewed and committed source files.
    /// </summary>
    [TestFixture]
    public class UmlDtoGeneratorTestFixture
    {
        /// <summary>
        /// The UML class names for which only DTO interfaces are generated.
        /// </summary>
        private static readonly HashSet<string> AbstractClassNames =
            new(StringComparer.Ordinal)
            {
                "AuditableThing",
                "Thing"
            };

        /// <summary>
        /// The UML class names whose generated DTO interfaces have reviewed golden files.
        /// </summary>
        private static readonly string[] RepresentativeInterfaceNames =
        [
            "AuditableThing",
            "BranchProtectionRule",
            "Comment",
            "FunctionalProject",
            "FunctionalProjectPolicy",
            "Organization",
            "ProjectMember",
            "Thing"
        ];

        /// <summary>
        /// The UML class names whose generated concrete DTOs have reviewed golden files.
        /// </summary>
        private static readonly string[] RepresentativeConcreteClassNames =
        [
            "BranchProtectionRule",
            "Comment",
            "FunctionalProject",
            "FunctionalProjectPolicy",
            "Organization",
            "ProjectMember"
        ];

        /// <summary>
        /// The FunctionalData UML classes indexed by name.
        /// </summary>
        private Dictionary<string, IClass> classes = null!;
        
        /// <summary>
        /// The directory containing the committed SDK DTO source files.
        /// </summary>
        private DirectoryInfo committedDirectory = null!;
        
        /// <summary>
        /// The directory containing the reviewed DTO golden files.
        /// </summary>
        private DirectoryInfo expectedDirectory = null!;
        
        /// <summary>
        /// The DTO generator used by the fixture.
        /// </summary>
        private UmlDtoGenerator generator = null!;
        
        /// <summary>
        /// The staging directory containing the generated DTO files.
        /// </summary>
        private DirectoryInfo stagingDirectory = null!;

        /// <summary>
        /// Loads the FunctionalData model and generates the DTO files used by the fixture's tests.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous fixture setup operation.
        /// </returns>
        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.committedDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Committed", "Mycelium.SDK", "AutoGenDTO"));
            this.expectedDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Expected", "UML", "AutoGenDTO"));
            this.stagingDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "UML", "_Mycelium.SDK.AutoGenDTO"));

            if (this.stagingDirectory.Exists)
            {
                this.stagingDirectory.Delete(recursive: true);
            }

            var xmiReaderResult = XmiLoadingTestFixture.ReadFunctionalData();
            var functionalData = XmiLoadingTestFixture.QueryFunctionalDataPackage(xmiReaderResult);

            this.classes = functionalData.PackagedElement
                .OfType<IClass>()
                .ToDictionary(umlClass => umlClass.Name, StringComparer.Ordinal);

            this.generator = new UmlDtoGenerator();

            await this.generator.GenerateAsync(xmiReaderResult, this.stagingDirectory);
        }

        /// <summary>
        /// Verifies that the reviewed golden-file set contains exactly the representative DTOs.
        /// </summary>
        [Test]
        public void Verify_that_reviewed_golden_set_contains_exactly_the_representative_DTOs()
        {
            var expectedFileNames = RepresentativeInterfaceNames
                .Select(className => $"I{className}.cs")
                .Concat(RepresentativeConcreteClassNames.Select(className => $"{className}.cs"))
                .OrderBy(fileName => fileName, StringComparer.Ordinal)
                .ToArray();

            var reviewedFileNames = QueryCSharpFileNames(this.expectedDirectory);

            Assert.Multiple(() =>
            {
                Assert.That(expectedFileNames, Has.Length.EqualTo(14));
                Assert.That(reviewedFileNames, Is.EqualTo(expectedFileNames), "The reviewed DTO golden-file set must contain exactly the representative files.");
            });
        }
        
        /// <summary>
        /// Verifies that batch generation produces exactly the expected 24 DTO files.
        /// </summary>
        [Test]
        public void Verify_that_batch_generation_produces_exactly_24_DTO_files()
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

            Assert.Multiple(() =>
            {
                Assert.That(expectedInterfaceFileNames, Has.Length.EqualTo(13));
                Assert.That(expectedConcreteFileNames, Has.Length.EqualTo(11));
                Assert.That(expectedFileNames, Has.Length.EqualTo(24));
                Assert.That(generatedFileNames, Is.EqualTo(expectedFileNames), "The generated DTO set contains missing or extra files.");
            });
        }

        /// <summary>
        /// Verifies that the complete generated DTO batch matches the committed SDK DTO source files.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous test operation.
        /// </returns>
        [Test]
        public async Task Verify_that_complete_batch_matches_committed_SDK_DTOs()
        {
            Assert.That(this.committedDirectory.Exists, Is.True, "The committed SDK DTO directory was not copied to the test output.");

            var generatedFileNames = QueryCSharpFileNames(this.stagingDirectory);
            var committedFileNames = QueryCSharpFileNames(this.committedDirectory);

            Assert.That(generatedFileNames, Is.EqualTo(committedFileNames), "The generated and committed DTO file sets differ.");

            foreach (var fileName in generatedFileNames)
            {
                var generated = await File.ReadAllTextAsync(Path.Combine(this.stagingDirectory.FullName, fileName));
                var committed = await File.ReadAllTextAsync(Path.Combine(this.committedDirectory.FullName, fileName));

                Assert.That(generated, Is.EqualTo(committed), $"Generated DTO '{fileName}' differs from the committed SDK source.");
            }
        }

        /// <summary>
        /// Verifies that a representative generated DTO interface matches its reviewed golden file.
        /// </summary>
        /// <param name="className">
        /// The UML class name whose DTO interface is verified.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous test operation.
        /// </returns>
        [TestCaseSource(nameof(RepresentativeInterfaceNames))]
        [Category("Expected")]
        public async Task Verify_that_representative_DTO_interface_matches_reviewed_golden_file(string className)
        {
            var generated = await this.generator
                .GenerateDataTransferObjectInterfaceAsync(this.stagingDirectory, this.classes[className]);

            var expected = await File.ReadAllTextAsync(Path.Combine(this.expectedDirectory.FullName, $"I{className}.cs"));

            Assert.That(
                generated,
                Is.EqualTo(expected),
                $"Generated interface 'I{className}.cs' differs from its "
                + "reviewed golden file.");
        }

        /// <summary>
        /// Verifies that a representative generated DTO class matches its reviewed golden file.
        /// </summary>
        /// <param name="className">
        /// The UML class name whose concrete DTO is verified.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous test operation.
        /// </returns>
        [TestCaseSource(nameof(RepresentativeConcreteClassNames))]
        [Category("Expected")]
        public async Task Verify_that_representative_DTO_class_matches_reviewed_golden_file(string className)
        {
            var generated = await this.generator
                .GenerateDataTransferObjectClassAsync(this.stagingDirectory, this.classes[className]);

            var expected = await File.ReadAllTextAsync(Path.Combine(this.expectedDirectory.FullName, $"{className}.cs"));

            Assert.That(
                generated,
                Is.EqualTo(expected),
                $"Generated class '{className}.cs' differs from its "
                + "reviewed golden file.");
        }

        /// <summary>
        /// Verifies that batch generation writes no files when model validation fails.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous test operation.
        /// </returns>
        [Test]
        public async Task Verify_that_batch_generation_writes_no_files_when_model_validation_fails()
        {
            var invalidReaderResult = XmiLoadingTestFixture.ReadFunctionalData();

            var functionalData = XmiLoadingTestFixture.QueryFunctionalDataPackage(invalidReaderResult);

            var thing = functionalData.PackagedElement
                .OfType<IClass>()
                .Single(umlClass => umlClass.Name == "Thing");

            var idProperty = thing.OwnedAttribute
                .Single(property => property.Name == "id");

            idProperty.Type = null!;

            var invalidOutputDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "UML", "_Mycelium.SDK.InvalidAutoGenDTO"));

            if (invalidOutputDirectory.Exists)
            {
                invalidOutputDirectory.Delete(recursive: true);
            }

            var invalidGenerator = new UmlDtoGenerator();

            await Assert.ThatAsync(
                () => invalidGenerator.GenerateAsync(invalidReaderResult, invalidOutputDirectory), Throws.TypeOf<InvalidOperationException>());

            invalidOutputDirectory.Refresh();

            Assert.That(
                invalidOutputDirectory.Exists,
                Is.False,
                "The generator created output after model preflight failed.");
        }

        /// <summary>
        /// Queries the C# filenames in the specified directory in deterministic order.
        /// </summary>
        /// <param name="directory">
        /// The directory whose C# filenames are queried.
        /// </param>
        /// <returns>
        /// The deterministically ordered C# filenames.
        /// </returns>
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
