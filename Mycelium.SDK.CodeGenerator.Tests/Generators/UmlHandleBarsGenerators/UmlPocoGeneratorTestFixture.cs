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
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Mycelium.SDK.CodeGenerator.Generators.UmlHandleBarsGenerators;
    using Mycelium.SDK.CodeGenerator.Tests.Expected;
    using Mycelium.SDK.CodeGenerator.Tests.Xmi;

    using uml4net.StructuredClassifiers;

    [TestFixture]
    public class UmlPocoGeneratorTestFixture
    {
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
                this.stagingDirectory.Delete(recursive: true);
            }

            var xmiReaderResult = XmiLoadingTestFixture.ReadFunctionalData();
            var functionalData = XmiLoadingTestFixture.QueryFunctionalDataPackage(xmiReaderResult);

            this.classes = functionalData.PackagedElement
                .OfType<IClass>()
                .ToDictionary(umlClass => umlClass.Name, StringComparer.Ordinal);

            this.generator = new UmlPocoGenerator();

            await this.generator.GenerateAsync(xmiReaderResult, this.stagingDirectory);
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
        public async Task Verify_that_batch_generation_writes_no_files_when_model_validation_fails()
        {
            var invalidReaderResult = XmiLoadingTestFixture.ReadFunctionalData();
            var functionalData = XmiLoadingTestFixture.QueryFunctionalDataPackage(invalidReaderResult);

            var thing = functionalData.PackagedElement
                .OfType<IClass>()
                .Single(umlClass => umlClass.Name == "Thing");

            var idProperty = thing.OwnedAttribute.Single(property => property.Name == "id");

            idProperty.Type = null!;

            var invalidOutputDirectory = new DirectoryInfo(
                Path.Combine(TestContext.CurrentContext.TestDirectory, "UML", "_Mycelium.SDK.InvalidAutoGenPOCO"));

            if (invalidOutputDirectory.Exists)
            {
                invalidOutputDirectory.Delete(recursive: true);
            }

            var invalidGenerator = new UmlPocoGenerator();

            await Assert.ThatAsync(() => invalidGenerator.GenerateAsync(invalidReaderResult, invalidOutputDirectory),
                Throws.TypeOf<InvalidOperationException>());

            invalidOutputDirectory.Refresh();

            Assert.That(
                invalidOutputDirectory.Exists,
                Is.False,
                "The generator created output after model preflight failed.");
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
                var generated = await File.ReadAllTextAsync(Path.Combine(this.stagingDirectory.FullName, fileName));
                var committed = await File.ReadAllTextAsync(Path.Combine(this.committedDirectory.FullName, fileName));

                Assert.That(
                    generated,
                    Is.EqualTo(committed),
                    $"Generated POCO '{fileName}' differs from the committed SDK source.");
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

        [TestCaseSource(nameof(RepresentativeInterfaceNames))]
        [Category("Expected")]
        public async Task Verify_that_representative_POCO_interface_matches_reviewed_golden_file(string className)
        {
            var generated = await this.generator.GeneratePocoInterfaceAsync(this.stagingDirectory, this.classes[className]);
            var expected = await File.ReadAllTextAsync(Path.Combine(this.expectedDirectory.FullName, $"I{className}.cs"));

            Assert.That(
                generated,
                Is.EqualTo(expected),
                $"Generated interface 'I{className}.cs' differs from its reviewed golden file.");
        }

        [TestCaseSource(nameof(RepresentativeConcreteClassNames))]
        [Category("Expected")]
        public async Task Verify_that_representative_POCO_class_matches_reviewed_golden_file(string className)
        {
            var generated = await this.generator.GeneratePocoClassAsync(this.stagingDirectory, this.classes[className]);
            var expected = await File.ReadAllTextAsync(Path.Combine(this.expectedDirectory.FullName, $"{className}.cs"));

            Assert.That(
                generated,
                Is.EqualTo(expected),
                $"Generated class '{className}.cs' differs from its reviewed golden file.");
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
