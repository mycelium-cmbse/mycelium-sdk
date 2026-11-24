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

        private const string DaoProviderFileName = "DaoProvider.cs";

        private DirectoryInfo expectedDirectory;

        private DirectoryInfo providerStagingDirectory;

        private DirectoryInfo stagingDirectory;

        private XmiReaderResult xmiReaderResult;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.expectedDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Expected", "UML", "AutoGenDao"));

            this.stagingDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "UML", "_Mycelium.Fabric.AutoGenDao"));

            this.providerStagingDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "UML", "_RepresentativeDaoProvider"));

            DirectoryInfo[] stagingDirectories = [this.stagingDirectory, this.providerStagingDirectory];

            foreach (var directory in stagingDirectories.Where(directory => directory.Exists))
            {
                directory.Delete(true);
            }

            this.xmiReaderResult = GeneratorSetupFixture.ReadFunctionalData();

            this.classes = GeneratorSetupFixture.QueryFunctionalDataPackage(this.xmiReaderResult).QueryPackages()
                .SelectMany(package => package.PackagedElement.OfType<IClass>())
                .ToDictionary(umlClass => umlClass.Name, StringComparer.Ordinal);

            var generator = new UmlDaoGenerator();

            await generator.GenerateAsync(this.xmiReaderResult, this.stagingDirectory);

            var representativeClasses = new RepresentativeClasses()
                .Where(this.classes.ContainsKey)
                .Select(className => this.classes[className])
                .ToArray();

            await generator.GenerateDaoProviderAsync(this.providerStagingDirectory, representativeClasses);
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
                .Append(DaoProviderFileName)
                .OrderBy(fileName => fileName, StringComparer.Ordinal)
                .ToArray();

            var goldenFileNames = GeneratedOutput.QueryRelativeFileNames(this.expectedDirectory);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(goldenFileNames, Is.EqualTo(representativeFileNames),
                    "The DAO golden set must contain exactly the concrete representative classes and the representative DAO provider.");

                foreach (var fileName in representativeFileNames.Intersect(goldenFileNames, StringComparer.Ordinal))
                {
                    var stagedDirectory = fileName == DaoProviderFileName ? this.providerStagingDirectory : this.stagingDirectory;
                    var stagedSource = await GeneratorSetupFixture.ReadSourceAsync(Path.Combine(stagedDirectory.FullName, fileName));
                    var goldenSource = await GeneratorSetupFixture.ReadSourceAsync(Path.Combine(this.expectedDirectory.FullName, fileName));

                    Assert.That(stagedSource, Is.EqualTo(goldenSource), $"Generated DAO file '{fileName}' differs from its reviewed golden.");
                }
            }
        }

        [Test]
        public async Task VerifyThatAbstractRepresentativeClassesHaveNoDaoProviderEntry()
        {
            var abstractClassNames = new RepresentativeClasses()
                .Where(className => this.classes.TryGetValue(className, out var umlClass) && umlClass.IsAbstract)
                .ToArray();

            var source = await GeneratorSetupFixture.ReadSourceAsync(Path.Combine(this.stagingDirectory.FullName, DaoProviderFileName));

            using (Assert.EnterMultipleScope())
            {
                Assert.That(abstractClassNames, Is.Not.Empty, "The representative class selection must contain an abstract class for this check to be meaningful.");

                foreach (var className in abstractClassNames)
                {
                    Assert.That(source, Does.Not.Contain($"[\"{className}\"]"), $"The abstract class '{className}' must have no entry in the DAO provider.");
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
            var parameter = orderedProperty.QuerySqlParameterName();
            var position = SqlNames.SqlLinkPositionColumnName;
            var columns = $"{SqlNames.SqlLinkSourceColumnName}, {SqlNames.SqlLinkTargetColumnName}, {position}";
            var occurrence = $"{SqlNames.SqlLinkTargetColumnName}, {position}";

            using (Assert.EnterMultipleScope())
            {
                Assert.That(source, Does.Contain($"INSERT INTO {SqlNames.SqlSchemaName}.{linkTable} ({columns})"), $"The insert into '{linkTable}' omits the position column.");

                Assert.That(source, Does.Contain($"FROM unnest(@{parameter}) WITH ORDINALITY AS element({occurrence});"), $"The insert into '{linkTable}' does not number the occurrences.");

                Assert.That(source, Does.Contain($"ORDER BY link.{position}) FROM {SqlNames.SqlSchemaName}.{linkTable} AS link"), $"The read of '{linkTable}' does not keep the stored order.");

                Assert.That(source, Does.Contain($"({position} > cardinality(@{parameter}) OR {SqlNames.SqlLinkTargetColumnName} IS DISTINCT FROM (@{parameter})[{position}]);"),
                    $"The update of '{linkTable}' does not delete only the occurrences that moved or no longer exist.");

                Assert.That(source, Does.Contain($"ON CONFLICT ({SqlNames.SqlLinkSourceColumnName}, {position}) DO NOTHING;"), $"The update of '{linkTable}' does not keep the unchanged occurrences.");
            }
        }

        [Test]
        public async Task VerifyThatDaosWithoutRelationalStateGenerateNoRelationUpdate()
        {
            var unrelatedModel = GeneratorSetupFixture.ReadFunctionalData();

            var concreteClass = GeneratorSetupFixture.QueryFunctionalDataPackage(unrelatedModel).QueryPackages()
                .SelectMany(package => package.PackagedElement.OfType<IClass>())
                .Where(umlClass => !umlClass.IsAbstract)
                .OrderBy(umlClass => umlClass.Name, StringComparer.Ordinal)
                .First();

            foreach (var property in concreteClass.QuerySqlChainRelationProperties())
            {
                property.IsDerived = true;
            }

            var outputDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "UML", "_UnrelatedDao"));

            if (outputDirectory.Exists)
            {
                outputDirectory.Delete(true);
            }

            await new UmlDaoGenerator().GenerateAsync(unrelatedModel, outputDirectory);

            var source = await GeneratorSetupFixture.ReadSourceAsync(Path.Combine(outputDirectory.FullName, $"{concreteClass.Name}Dao.cs"));

            using (Assert.EnterMultipleScope())
            {
                Assert.That(source, Does.Not.Contain("UpdateRelationsAsync"), $"The DAO of '{concreteClass.Name}' overrides the relation update although it has no relational state.");

                Assert.That(source, Does.Not.Contain("UpdateRelationSql"), $"The DAO of '{concreteClass.Name}' declares relation statements although it has no relational state.");

                Assert.That(source, Does.Contain($"protected override void AddReferenceParameters(NpgsqlCommand command, I{concreteClass.Name} dto)"),
                    $"The DAO of '{concreteClass.Name}' does not supply its reference parameters.");
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

                await Assert.ThatAsync(() => generator.GenerateDaoProviderAsync(null, this.classes.Values), Throws.ArgumentNullException);

                await Assert.ThatAsync(() => generator.GenerateDaoProviderAsync(this.providerStagingDirectory, null), Throws.ArgumentNullException);
            }
        }
    }
}
