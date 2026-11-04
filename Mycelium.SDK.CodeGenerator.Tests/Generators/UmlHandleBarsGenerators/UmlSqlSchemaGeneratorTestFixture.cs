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
    using System.Text;
    using System.Text.RegularExpressions;

    using Mycelium.SDK.CodeGenerator.Generators.UmlHandleBarsGenerators;

    using uml4net.Classification;
    using uml4net.Packages;
    using uml4net.StructuredClassifiers;
    using uml4net.Values;

    [TestFixture]
    public class UmlSqlSchemaGeneratorTestFixture
    {
        private const string SchemaResourceName = "Mycelium.SDK.Sql.schema.sql";

        private static readonly string[] ExpectedFileNames = ["schema.sql"];

        private DirectoryInfo expectedDirectory = null!;

        private DirectoryInfo stagingDirectory = null!;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.expectedDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Expected", "UML", "AutoGenSql"));

            this.stagingDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "UML", "_Mycelium.SDK.AutoGenSql"));

            if (this.stagingDirectory.Exists)
            {
                this.stagingDirectory.Delete(true);
            }

            var generator = new UmlSqlSchemaGenerator();

            await generator.GenerateAsync(GeneratorSetupFixture.ReadFunctionalData(), this.stagingDirectory);

            TestContext.Progress.WriteLine($"Retained generated SQL: {Path.Combine(this.stagingDirectory.FullName, "schema.sql")}");
        }

        [Test]
        [Category("Expected")]
        public async Task VerifyThatCompleteSchemaMatchesReviewedExpectedFile()
        {
            var stagedFileNames = GeneratedOutput.QueryRelativeFileNames(this.stagingDirectory);

            Assert.That(this.expectedDirectory.Exists, Is.True,
                "The complete reviewed SQL baseline is missing from the test output. Review the retained staged schema before adding the expected file.");

            var expectedFileNames = GeneratedOutput.QueryRelativeFileNames(this.expectedDirectory);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(stagedFileNames, Is.EqualTo(ExpectedFileNames), "SQL generation must produce exactly one schema.sql and no other files.");

                Assert.That(expectedFileNames, Is.EqualTo(ExpectedFileNames), "The reviewed SQL baseline must contain exactly one schema.sql and no other files.");

                foreach (var fileName in stagedFileNames.Intersect(expectedFileNames, StringComparer.Ordinal))
                {
                    var stagedSource = await GeneratorSetupFixture.ReadSourceAsync(Path.Combine(this.stagingDirectory.FullName, fileName));

                    var expectedSource = await GeneratorSetupFixture.ReadSourceAsync(Path.Combine(this.expectedDirectory.FullName, fileName));

                    Assert.That(stagedSource, Is.EqualTo(expectedSource).Using(StringComparer.Ordinal),
                        $"Generated '{fileName}' differs from the complete reviewed SQL baseline. Review the entire SQL diff before replacing the expected file.");
                }
            }
        }

        [Test]
        public async Task VerifyThatSdkAssemblyEmbedsTheCompleteStagedSchema()
        {
            var stagedSource = await GeneratorSetupFixture.ReadSourceAsync(Path.Combine(this.stagingDirectory.FullName, "schema.sql"));

            await using var resourceStream = typeof(Mycelium.SDK.DTO.IThing).Assembly.GetManifestResourceStream(SchemaResourceName);

            Assert.That(resourceStream, Is.Not.Null,
                $"Mycelium.SDK must embed the reviewed expected schema as '{SchemaResourceName}'. Check the EmbeddedResource item in Mycelium.SDK/Mycelium.SDK.csproj.");

            using var resourceReader = new StreamReader(resourceStream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true), false);

            var embeddedSource = await resourceReader.ReadToEndAsync();

            Assert.That(embeddedSource, Is.EqualTo(stagedSource).Using(StringComparer.Ordinal),
                "The schema embedded in Mycelium.SDK differs from the staged generation; review the SQL diff before replacing the expected file.");
        }

        [Test]
        public async Task VerifyThatNewModelNamesRemainValidUniqueAndIndependentOfInputOrder()
        {
            var names = new[] { "HTTPConnection", "HttpConnection", "Select", new string('A', 80) };
            var first = await this.RenderScenarioAsync(package =>
            {
                foreach (var name in names)
                {
                    AddModelClass(package, name);
                }
            });

            var reversed = await this.RenderScenarioAsync(package =>
            {
                foreach (var name in names.Reverse())
                {
                    AddModelClass(package, name);
                }
            });

            var tableNames = Regex.Matches(first, @"(?m)^CREATE TABLE fabric\.([a-z_][a-z0-9_]*) \(")
                .Select(match => match.Groups[1].Value)
                .ToArray();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(first, Is.EqualTo(reversed).Using(StringComparer.Ordinal), "Package element order must not change SQL names or output.");
                Assert.That(tableNames, Is.Unique, "Distinct modeled tables must not collide after snake_case conversion.");
                Assert.That(tableNames.All(name => name.Length <= 63), Is.True, "Generated ASCII identifiers must fit PostgreSQL's identifier limit.");
                Assert.That(tableNames.Count(name => name.StartsWith("http_connection_", StringComparison.Ordinal)), Is.EqualTo(2));
                Assert.That(tableNames, Does.Contain("model_select"), "A reserved SQL keyword must receive a valid unquoted mapping.");
                Assert.That(tableNames.Any(name => name.StartsWith(new string('a', 40), StringComparison.Ordinal)), Is.True,
                    "An overlong modeled name must retain a readable, shortened table name.");
            }
        }

        [Test]
        public async Task VerifyThatMultipleOwnerRolesOnOnePartClassHaveDistinctTriggersAndMandatoryOwnerChecks()
        {
            var sql = await this.RenderScenarioAsync(package =>
            {
                var owner = AddModelClass(package, "StorageOwner");
                var part = AddModelClass(package, "StoragePart");

                AddOwnerReference(owner, part, "primaryOwner");
                AddOwnerReference(owner, part, "secondaryOwner");
            });

            var guards = Regex.Matches(sql,
                @"CREATE TRIGGER ([a-z_][a-z0-9_]*)\s+AFTER INSERT OR UPDATE OR DELETE ON fabric\.storage_part\s+" +
                @"FOR EACH ROW EXECUTE FUNCTION fabric\.track_ownership\('id', '(primary_owner|secondary_owner)'\);");

            var checks = Regex.Matches(sql,
                @"CREATE CONSTRAINT TRIGGER ([a-z_][a-z0-9_]*)\s+AFTER INSERT OR UPDATE OR DELETE ON fabric\.storage_part\s+" +
                @"DEFERRABLE INITIALLY DEFERRED\s+FOR EACH ROW EXECUTE FUNCTION fabric\.validate_ownership\('id', '(primary_owner|secondary_owner)'\);");

            using (Assert.EnterMultipleScope())
            {
                Assert.That(guards.Count, Is.EqualTo(2), "Each owner role needs its own immediate ownership tracking trigger.");
                Assert.That(checks.Count, Is.EqualTo(2), "Each owner role needs its own deferred ownership check.");
                Assert.That(guards.Concat(checks).Select(match => match.Groups[1].Value), Is.Unique,
                    "Using the common part column 'id' must not produce duplicate trigger names.");
                Assert.That(sql, Does.Contain("WHERE relationship.id = p_id AND relationship.primary_owner IS NOT NULL"));
                Assert.That(sql, Does.Contain("WHERE relationship.id = p_id AND relationship.secondary_owner IS NOT NULL"));
                Assert.That(sql, Does.Not.Contain("CREATE TABLE fabric.storage_owner_primary_owner_parts_storage_part"),
                    "A navigable scalar owner opposite must supply storage instead of an additional collection link table.");
            }
        }

        [Test]
        public async Task VerifyThatNewMultipleInheritanceUsesBothDirectParentsAndConcreteTypeMembership()
        {
            var sql = await this.RenderScenarioAsync(package =>
            {
                var firstParent = AddModelClass(package, "StorageParentA");
                var secondParent = AddModelClass(package, "StorageParentB");

                firstParent.IsAbstract = true;
                secondParent.IsAbstract = true;

                AddModelClass(package, "StorageChild", firstParent, secondParent);
            });

            var parentTargets = Regex.Matches(sql,
                    @"ALTER TABLE fabric\.storage_child\s+ADD CONSTRAINT [a-z_][a-z0-9_]* FOREIGN KEY \(id\) REFERENCES fabric\.([a-z_][a-z0-9_]*) \(id\)")
                .Select(match => match.Groups[1].Value)
                .ToArray();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(parentTargets, Is.EquivalentTo(new[] { "storage_parent_a", "storage_parent_b" }),
                    "The child must reference both direct parents instead of only the common root.");

                foreach (var table in new[] { "storage_parent_a", "storage_parent_b", "storage_child" })
                {
                    Assert.That(sql, Does.Match(
                        @"IF \(\(kind IN \('StorageChild'\)\) IS TRUE\)\s+IS DISTINCT FROM \(EXISTS \(SELECT 1 FROM fabric\." + table + @" WHERE id = p_id\)\)"),
                        $"Concrete-type membership must require the '{table}' row for StorageChild.");
                }

                Assert.That(sql, Does.Not.Contain("'StorageParentA'"), "An abstract parent is not a permitted concrete discriminator.");
                Assert.That(sql, Does.Not.Contain("'StorageParentB'"), "An abstract parent is not a permitted concrete discriminator.");
            }
        }

        private async Task<string> RenderScenarioAsync(Action<IPackage> configure)
        {
            var result = GeneratorSetupFixture.ReadFunctionalData();

            configure(GeneratorSetupFixture.QueryFunctionalDataPackage(result));

            var scenarioDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "UML", "_Mycelium.SDK.SqlScenarios",
                TestContext.CurrentContext.Test.ID));

            await new UmlSqlSchemaGenerator().GenerateAsync(result, scenarioDirectory);

            return await GeneratorSetupFixture.ReadSourceAsync(Path.Combine(scenarioDirectory.FullName, "schema.sql"));
        }

        private static IClass AddModelClass(IPackage package, string name, params IClass[] parents)
        {
            var umlClass = new Class { XmiId = "sql-regression-" + name, Name = name };

            if (parents.Length == 0)
            {
                parents = [package.PackagedElement.OfType<IClass>().Single(candidate => candidate.Name == "Thing")];
            }

            foreach (var parent in parents)
            {
                umlClass.Generalization.Add(new Generalization
                {
                    XmiId = umlClass.XmiId + "-parent-" + parent.XmiId, Specific = umlClass, General = parent
                });
            }

            package.PackagedElement.Add(umlClass);

            return umlClass;
        }

        private static void AddOwnerReference(IClass owner, IClass part, string role)
        {
            var association = new Association { XmiId = "sql-regression-" + role };
            var collection = new Property
            {
                XmiId = association.XmiId + "-parts", Name = role + "Parts", Type = part, Association = association, Aggregation = AggregationKind.Composite
            };

            collection.UpperValue.Add(new LiteralUnlimitedNatural { Value = "*" });

            var reference = new Property { XmiId = association.XmiId + "-owner", Name = role, Type = owner, Association = association };

            reference.LowerValue.Add(new LiteralInteger { Value = 1 });
            reference.UpperValue.Add(new LiteralInteger { Value = 1 });

            owner.OwnedAttribute.Add(collection);
            part.OwnedAttribute.Add(reference);
            association.MemberEnd.AddRange([collection, reference]);
        }
    }
}
