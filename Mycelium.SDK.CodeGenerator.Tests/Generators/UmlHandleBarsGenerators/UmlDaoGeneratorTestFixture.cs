// ------------------------------------------------------------------------------------------------
//  <copyright file="UmlDaoGeneratorTestFixture.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Tests.Generators.UmlHandleBarsGenerators
{
    using Mycelium.SDK.CodeGenerator.Extensions;
    using Mycelium.SDK.CodeGenerator.Generators.UmlHandleBarsGenerators;
    using Mycelium.SDK.CodeGenerator.Tests.Expected;

    using uml4net.Extensions;
    using uml4net.StructuredClassifiers;
    using uml4net.xmi.Readers;

    [TestFixture]
    public class UmlDaoGeneratorTestFixture
    {
        private Dictionary<string, IClass> classes;

        private DirectoryInfo committedDirectory;

        private DirectoryInfo expectedDirectory;

        private DirectoryInfo stagingDirectory;

        private XmiReaderResult xmiReaderResult;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.committedDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Committed", "Mycelium.SDK.Dal", "AutoGenDao"));

            this.expectedDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Expected", "UML", "AutoGenDao"));

            this.stagingDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "UML", "_Mycelium.SDK.Dal.AutoGenDao"));

            if (this.stagingDirectory.Exists)
            {
                this.stagingDirectory.Delete(true);
            }

            this.xmiReaderResult = GeneratorSetupFixture.ReadFunctionalData();

            this.classes = GeneratorSetupFixture.QueryFunctionalDataPackage(this.xmiReaderResult).QueryPackages()
                .SelectMany(package => package.PackagedElement.OfType<IClass>())
                .ToDictionary(umlClass => umlClass.Name, StringComparer.Ordinal);

            await new UmlDaoGenerator().GenerateAsync(this.xmiReaderResult, this.stagingDirectory);
        }

        [Test]
        public async Task VerifyThatCompleteStagedOutputMatchesCommittedDaos()
        {
            Assert.That(this.committedDirectory.Exists, Is.True, "The committed DAO directory Mycelium.SDK.Dal/AutoGenDao was not copied to the test output.");

            if (!this.committedDirectory.Exists)
            {
                return;
            }

            var stagedFileNames = GeneratedOutput.QueryRelativeFileNames(this.stagingDirectory);
            var committedFileNames = GeneratedOutput.QueryRelativeFileNames(this.committedDirectory);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(stagedFileNames, Is.EqualTo(committedFileNames), "The staged and committed DAO manifests differ.");

                foreach (var fileName in stagedFileNames.Intersect(committedFileNames, StringComparer.Ordinal))
                {
                    var stagedSource = await GeneratorSetupFixture.ReadSourceAsync(Path.Combine(this.stagingDirectory.FullName, fileName));
                    var committedSource = await GeneratorSetupFixture.ReadSourceAsync(Path.Combine(this.committedDirectory.FullName, fileName));

                    Assert.That(stagedSource, Is.EqualTo(committedSource), $"Staged DAO '{fileName}' differs from the committed source.");
                }
            }
        }

        [Test]
        [Category("Expected")]
        public async Task VerifyThatRepresentativeDaosMatchTheirGoldens()
        {
            Assert.That(this.expectedDirectory.Exists, Is.True, "The representative DAO golden directory Expected/UML/AutoGenDao is missing.");

            if (!this.expectedDirectory.Exists)
            {
                return;
            }

            var representativeFileNames = new RepresentativeClasses()
                .Where(className => this.classes.TryGetValue(className, out var umlClass) && !umlClass.IsAbstract)
                .Select(className => $"{className}Dao.cs")
                .OrderBy(fileName => fileName, StringComparer.Ordinal)
                .ToArray();

            var goldenFileNames = GeneratedOutput.QueryRelativeFileNames(this.expectedDirectory);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(goldenFileNames, Is.EqualTo(representativeFileNames), "The DAO golden set must contain exactly the concrete representative classes.");

                foreach (var fileName in representativeFileNames.Intersect(goldenFileNames, StringComparer.Ordinal))
                {
                    var stagedSource = await GeneratorSetupFixture.ReadSourceAsync(Path.Combine(this.stagingDirectory.FullName, fileName));
                    var goldenSource = await GeneratorSetupFixture.ReadSourceAsync(Path.Combine(this.expectedDirectory.FullName, fileName));

                    Assert.That(stagedSource, Is.EqualTo(goldenSource), $"Generated DAO '{fileName}' differs from its reviewed golden.");
                }
            }
        }

        [Test]
        public async Task VerifyThatOrderedLinkCollectionsStoreTheirPosition()
        {
            var orderedModel = GeneratorSetupFixture.ReadFunctionalData();

            var linkOwner = GeneratorSetupFixture.QueryFunctionalDataPackage(orderedModel).QueryPackages()
                .SelectMany(package => package.PackagedElement.OfType<IClass>())
                .Where(umlClass => !umlClass.IsAbstract && umlClass.QuerySqlLinkProperties().Count > 0)
                .OrderBy(umlClass => umlClass.Name, StringComparer.Ordinal)
                .First();

            var orderedProperty = linkOwner.QuerySqlLinkProperties()[0];
            orderedProperty.IsOrdered = true;

            var outputDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "UML", "_OrderedLinkDao"));

            if (outputDirectory.Exists)
            {
                outputDirectory.Delete(true);
            }

            await new UmlDaoGenerator().GenerateAsync(orderedModel, outputDirectory);

            var source = await GeneratorSetupFixture.ReadSourceAsync(Path.Combine(outputDirectory.FullName, $"{linkOwner.Name}Dao.cs"));
            var linkTable = orderedProperty.QuerySqlLinkTableName();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(source, Does.Contain($"INSERT INTO fabric.{linkTable} (source_id, target_id, position) "), $"The insert into '{linkTable}' omits the position column.");

                Assert.That(source, Does.Contain($"FROM unnest(@{linkTable}) WITH ORDINALITY AS element(target_id, position);"), $"The insert into '{linkTable}' does not number the occurrences.");
            }
        }

        [Test]
        public async Task VerifyThatGenerationRejectsMissingArguments()
        {
            var generator = new UmlDaoGenerator();

            using (Assert.EnterMultipleScope())
            {
                await Assert.ThatAsync(() => generator.GenerateAsync((XmiReaderResult)null, this.stagingDirectory), Throws.ArgumentNullException);

                await Assert.ThatAsync(() => generator.GenerateAsync(this.xmiReaderResult, null), Throws.ArgumentNullException);
            }
        }
    }
}
