// ------------------------------------------------------------------------------------------------
//  <copyright file="UmlSqlSchemaGeneratorTestFixture.cs" company="Starion Group S.A.">
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

    using uml4net.Classification;
    using uml4net.Extensions;
    using uml4net.StructuredClassifiers;
    using uml4net.Values;

    [TestFixture]
    public class UmlSqlSchemaGeneratorTestFixture : UmlClassGeneratorTestFixtureBase
    {
        private DirectoryInfo committedDirectory;
        private DirectoryInfo expectedDirectory;
        private DirectoryInfo stagingDirectory;
        private DirectoryInfo fragmentDirectory;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.committedDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Committed", "Mycelium.SDK.CodeGenerator", "AutoGenSql"));
            this.expectedDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Expected", "UML", "AutoGenSql"));
            this.stagingDirectory = CreateStagingDirectory("_Mycelium.SDK.CodeGenerator.AutoGenSql");
            this.fragmentDirectory = CreateStagingDirectory("_Mycelium.SDK.CodeGenerator.AutoGenSql.Representative");

            TestContext.Progress.WriteLine($"SQL staging: {this.stagingDirectory.FullName}");
            TestContext.Progress.WriteLine($"Representative SQL staging: {this.fragmentDirectory.FullName}");

            var model = GeneratorSetupFixture.ReadFunctionalData();
            var fabric = GeneratorSetupFixture.QueryFunctionalDataPackage(model);
            var classes = fabric.QueryPackages().SelectMany(package => package.PackagedElement.OfType<IClass>())
                .ToDictionary(umlClass => umlClass.Name, StringComparer.Ordinal);
            var generator = new UmlSqlSchemaGenerator();

            await generator.GenerateAsync(model, this.stagingDirectory);

            foreach (var className in new RepresentativeClasses())
            {
                await generator.GenerateClassFragmentAsync(this.fragmentDirectory, classes[className]);
            }
        }

        [Test]
        public async Task VerifyThatCompleteSchemaMatchesCommittedOutput()
        {
            Assert.That(this.committedDirectory.Exists, Is.True,
                $"The committed SQL baseline is missing. Review {this.stagingDirectory.FullName} before deliberate promotion.");

            var stagedFileNames = GeneratedOutput.QueryRelativeFileNames(this.stagingDirectory);
            var committedFileNames = GeneratedOutput.QueryRelativeFileNames(this.committedDirectory);

            Assert.That(stagedFileNames, Is.EqualTo(committedFileNames), "The staged and committed SQL filename sets differ.");

            foreach (var fileName in stagedFileNames)
            {
                await AssertFilesMatchAsync(Path.Combine(this.stagingDirectory.FullName, fileName), Path.Combine(this.committedDirectory.FullName, fileName),
                    $"Generated SQL '{fileName}'", "the committed production schema");
            }
        }

        [Test]
        [Category("Expected")]
        public async Task VerifyThatRepresentativeFragmentsMatchReviewedGoldens()
        {
            Assert.That(this.expectedDirectory.Exists, Is.True,
                $"The reviewed SQL goldens are missing. Review {this.fragmentDirectory.FullName} before adding them.");

            var representativeFileNames = new RepresentativeClasses().Select(className => $"{className}.sql")
                .OrderBy(fileName => fileName, StringComparer.Ordinal).ToArray();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(GeneratedOutput.QueryRelativeFileNames(this.fragmentDirectory), Is.EqualTo(representativeFileNames));
                Assert.That(GeneratedOutput.QueryRelativeFileNames(this.expectedDirectory), Is.EqualTo(representativeFileNames));
            }

            foreach (var fileName in representativeFileNames)
            {
                await AssertFilesMatchAsync(Path.Combine(this.fragmentDirectory.FullName, fileName), Path.Combine(this.expectedDirectory.FullName, fileName),
                    $"Representative SQL fragment '{fileName}'", "its reviewed golden");
            }
        }

        [Test]
        public async Task VerifyThatCanonicalEntryPointsProduceIdenticalUtf8WithoutBomAndLfOutput()
        {
            var replayDirectory = CreateStagingDirectory("_Mycelium.SDK.CodeGenerator.AutoGenSql.Replay");

            await new UmlSqlSchemaGenerator().GenerateAsync(GeneratorSetupFixture.ResourcesDirectory, replayDirectory);

            var stagedPath = Path.Combine(this.stagingDirectory.FullName, "schema.sql");
            var replayPath = Path.Combine(replayDirectory.FullName, "schema.sql");

            await AssertFilesMatchAsync(replayPath, stagedPath, "SQL generated through the resources-directory entry point", "SQL generated through the parsed-model entry point");

            foreach (var path in new[] { stagedPath, replayPath })
            {
                var bytes = await File.ReadAllBytesAsync(path);
                var source = await GeneratorSetupFixture.ReadSourceAsync(path);
                var hasBom = bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF;

                using (Assert.EnterMultipleScope())
                {
                    Assert.That(hasBom, Is.False, $"SQL '{path}' must not contain a UTF-8 BOM.");
                    Assert.That(source, Does.Not.Contain("\r"), $"SQL '{path}' must use LF line endings.");
                    Assert.That(source.EndsWith('\n'), Is.True, $"SQL '{path}' must end with a newline.");
                    Assert.That(source.EndsWith("\n\n", StringComparison.Ordinal), Is.False, $"SQL '{path}' must have only one final newline.");
                }
            }
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        [TestCase(true, false)]
        public async Task VerifyThatOrderedOrDuplicatePermittingCollectionsPreserveOccurrences(bool isOrdered, bool isUnique)
        {
            var owner = new Class { XmiId = "collection-owner", Name = "CollectionOwner" };
            var target = new Class { XmiId = "collection-target", Name = "CollectionTarget" };
            var reference = new Property
            {
                XmiId = "collection-reference", Name = "items", Type = target, IsOrdered = isOrdered, IsUnique = isUnique
            };

            reference.UpperValue.Add(new LiteralUnlimitedNatural { Value = "*" });
            owner.OwnedAttribute.Add(reference);

            var directory = CreateStagingDirectory($"_Mycelium.SDK.CodeGenerator.AutoGenSql.Collection_{isOrdered}_{isUnique}");
            var fragment = await new UmlSqlSchemaGenerator().GenerateClassFragmentAsync(directory, owner);
            const string PairUniqueness = "UNIQUE (\"sourceCollectionOwner\", \"targetCollectionTarget\")";

            using (Assert.EnterMultipleScope())
            {
                Assert.That(fragment, Does.Contain("\"position\" bigint NOT NULL"));
                Assert.That(fragment, Does.Contain("PRIMARY KEY (\"sourceCollectionOwner\", \"position\")"));

                if (isUnique)
                {
                    Assert.That(fragment, Does.Contain(PairUniqueness));
                }
                else
                {
                    Assert.That(fragment, Does.Not.Contain(PairUniqueness));
                }
            }
        }

        private static DirectoryInfo CreateStagingDirectory(string name)
        {
            var directory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "UML", name));

            if (directory.Exists)
            {
                directory.Delete(true);
            }

            directory.Create();

            return directory;
        }
    }
}
