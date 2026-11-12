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

    using SqlNames = Mycelium.SDK.CodeGenerator.Extensions.NamedElementExtensions;

    [TestFixture]
    public class UmlDaoGeneratorTestFixture
    {
        private Dictionary<string, IClass> classes;

        private DirectoryInfo expectedDirectory;

        private DirectoryInfo stagingDirectory;

        private XmiReaderResult xmiReaderResult;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.expectedDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Expected", "UML", "AutoGenDao"));

            this.stagingDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "UML", "_Mycelium.Fabric.AutoGenDao"));

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
            var columns = $"{SqlNames.SqlLinkSourceColumnName}, {SqlNames.SqlLinkTargetColumnName}, {SqlNames.SqlLinkPositionColumnName}";
            var occurrence = $"{SqlNames.SqlLinkTargetColumnName}, {SqlNames.SqlLinkPositionColumnName}";

            using (Assert.EnterMultipleScope())
            {
                Assert.That(source, Does.Contain($"INSERT INTO {SqlNames.SqlSchemaName}.{linkTable} ({columns}) "), $"The insert into '{linkTable}' omits the position column.");

                Assert.That(source, Does.Contain($"FROM unnest(@{orderedProperty.QuerySqlParameterName()}) WITH ORDINALITY AS element({occurrence});"), $"The insert into '{linkTable}' does not number the occurrences.");

                Assert.That(source, Does.Contain($"ORDER BY link.{SqlNames.SqlLinkPositionColumnName}) FROM {SqlNames.SqlSchemaName}.{linkTable} AS link"), $"The read of '{linkTable}' does not keep the stored order.");
            }
        }

        [Test]
        public async Task VerifyThatGenerationRejectsCollidingParameterNames()
        {
            var collidingModel = GeneratorSetupFixture.ReadFunctionalData();

            var owner = GeneratorSetupFixture.QueryFunctionalDataPackage(collidingModel).QueryPackages()
                .SelectMany(package => package.PackagedElement.OfType<IClass>())
                .Where(umlClass => !umlClass.IsAbstract && umlClass.QuerySqlColumnProperties().Count > 0 && umlClass.QuerySqlLinkProperties().Count > 0)
                .OrderBy(umlClass => umlClass.Name, StringComparer.Ordinal)
                .First();

            var linkProperty = owner.QuerySqlLinkProperties()[0];
            var columnProperty = owner.QuerySqlColumnProperties()[0];
            columnProperty.Name = linkProperty.Name;

            var outputDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "UML", "_CollidingParameterDao"));

            if (outputDirectory.Exists)
            {
                outputDirectory.Delete(true);
            }

            using (Assert.EnterMultipleScope())
            {
                await Assert.ThatAsync(() => new UmlDaoGenerator().GenerateAsync(collidingModel, outputDirectory),
                    Throws.InvalidOperationException.With.Message.Contains($"The DAO parameter '{linkProperty.QuerySqlParameterName()}' of class '{owner.Name}'")
                        .And.Message.Contains($"property '{owner.Name}.{linkProperty.Name}'"));

                Assert.That(outputDirectory.Exists, Is.False, "A batch with colliding parameter names must not be written.");
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
