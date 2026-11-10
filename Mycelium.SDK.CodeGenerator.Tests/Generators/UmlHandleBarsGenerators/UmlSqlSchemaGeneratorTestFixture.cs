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
    using System.Text.RegularExpressions;

    using Mycelium.SDK.CodeGenerator.Generators.UmlHandleBarsGenerators;
    using Mycelium.SDK.Dal;

    using uml4net.Classification;
    using uml4net.Packages;
    using uml4net.StructuredClassifiers;
    using uml4net.Values;

    [TestFixture]
    public partial class UmlSqlSchemaGeneratorTestFixture
    {
        private const string DeliveredScriptName = "0001_initial.sql";

        private static readonly string[] ExpectedFileNames = ["schema.sql"];

        private static readonly string[] ExpectedOwnershipTrackingTriggers = ["t10_track_ownership_primary_owner", "t10_track_ownership_secondary_owner"];

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

                    var expectedSource = (await GeneratorSetupFixture.ReadSourceAsync(Path.Combine(this.expectedDirectory.FullName, fileName))).ReplaceLineEndings("\n");

                    Assert.That(stagedSource, Is.EqualTo(expectedSource).Using(StringComparer.Ordinal),
                        $"Generated '{fileName}' differs from the complete reviewed SQL baseline. Review the entire SQL diff before replacing the expected file.");
                }
            }
        }

        [Test]
        public async Task VerifyThatDalDeliversTheCompleteStagedSchemaAsTheInitialScript()
        {
            var stagedSource = await GeneratorSetupFixture.ReadSourceAsync(Path.Combine(this.stagingDirectory.FullName, "schema.sql"));

            var deliveredScripts = SqlSchemaScripts.GetAll().Where(script => script.Name == DeliveredScriptName).ToList();

            Assert.That(deliveredScripts, Has.Count.EqualTo(1),
                $"Mycelium.SDK.Dal must embed exactly one '{DeliveredScriptName}'. Check Mycelium.SDK.Dal/AutoGenSql/ and the EmbeddedResource item in Mycelium.SDK.Dal/Mycelium.SDK.Dal.csproj.");

            Assert.That(deliveredScripts[0].Sql, Is.EqualTo(stagedSource).Using(StringComparer.Ordinal),
                $"Mycelium.SDK.Dal/AutoGenSql/{DeliveredScriptName} differs from the staged generation. Review the entire SQL diff, then replace both the expected schema.sql and the delivered copy.");
        }

        [Test]
        public async Task VerifyThatSyntheticModelNamesRemainValidReadableAndIndependentOfInputOrder()
        {
            string[] names = ["Select", new string('A', 80)];
            var first = await RenderScenarioAsync(package =>
            {
                foreach (var name in names)
                {
                    AddModelClass(package, name);
                }
            });

            var reversed = await RenderScenarioAsync(package =>
            {
                foreach (var name in names.Reverse())
                {
                    AddModelClass(package, name);
                }
            });

            var tableNames = TableDeclaration().Matches(first)
                .Select(match => match.Groups[1].Value)
                .ToArray();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(first, Is.EqualTo(reversed).Using(StringComparer.Ordinal), "Package element order must not change SQL names or output.");
                Assert.That(tableNames.All(name => name.Length <= 63), Is.True, "Generated ASCII identifiers must fit PostgreSQL's identifier limit.");
                Assert.That(tableNames, Does.Contain("model_select"), "A reserved SQL keyword must receive a valid unquoted mapping.");
                Assert.That(tableNames, Has.One.Match("^a{54}_[0-9a-f]{8}$"),
                    "Only an overlong name is hashed: a readable 54-character prefix followed by an 8-character hash.");
                Assert.That(first, Does.Not.Match("_[0-9a-f]{16}\\b"), "Names within the identifier limit must not carry a digest suffix.");
            }
        }

        [Test]
        public async Task VerifyThatModelNamesCollidingAfterConversionFailGenerationNamingBothElements()
        {
            var failure = await RenderFailureAsync(package =>
            {
                AddModelClass(package, "HTTPConnection");
                AddModelClass(package, "HttpConnection");
            });

            Assert.That(failure, Is.Not.Null, "Two classes that both become 'http_connection' must stop generation instead of being suffixed.");

            using (Assert.EnterMultipleScope())
            {
                Assert.That(failure, Does.Contain(nameof(InvalidOperationException)), "A name collision is a model problem reported as an invalid operation.");
                Assert.That(failure, Does.Contain("'http_connection'"), "The message must name the colliding SQL name.");
                Assert.That(failure, Does.Contain("'HTTPConnection'"), "The message must name the first colliding class.");
                Assert.That(failure, Does.Contain("'HttpConnection'"), "The message must name the second colliding class.");
            }
        }

        [Test]
        public async Task VerifyThatMultipleOwnerRolesOnOnePartClassHaveDistinctTriggersAndMandatoryOwnerChecks()
        {
            var sql = await RenderScenarioAsync(package =>
            {
                var owner = AddModelClass(package, "StorageOwner");
                var part = AddModelClass(package, "StoragePart");

                AddOwnerReference(owner, part, "primaryOwner");
                AddOwnerReference(owner, part, "secondaryOwner");
            });

            var guards = OwnershipTrackingTrigger().Matches(sql);

            var checks = OwnershipCheckTrigger().Matches(sql);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(guards.Count, Is.EqualTo(2), "Each owner role needs its own immediate ownership tracking trigger.");
                Assert.That(checks.Count, Is.EqualTo(2), "Each owner role needs its own deferred ownership check.");
                Assert.That(guards.Select(match => match.Groups[1].Value), Is.EquivalentTo(ExpectedOwnershipTrackingTriggers),
                    "Two ownership roles on one table must be distinguished by their relationship column, not by a digest.");
                Assert.That(guards.Concat(checks).Select(match => match.Groups[1].Value), Is.Unique,
                    "Using the common part column 'id' must not produce duplicate trigger names.");
                Assert.That(sql, Does.Contain("WHERE relationship.id = p_id AND relationship.primary_owner IS NOT NULL"));
                Assert.That(sql, Does.Contain("WHERE relationship.id = p_id AND relationship.secondary_owner IS NOT NULL"));
                Assert.That(sql, Does.Not.Contain("CREATE TABLE fabric.storage_owner_primary_owner_parts"),
                    "A navigable scalar owner opposite must supply storage instead of an additional collection link table.");
            }
        }

        [Test]
        public async Task VerifyThatMultipleDirectParentsFailGenerationNamingBothGeneralizations()
        {
            var failure = await RenderFailureAsync(package =>
            {
                var firstParent = AddModelClass(package, "StorageParentA");
                var secondParent = AddModelClass(package, "StorageParentB");

                firstParent.IsAbstract = true;
                secondParent.IsAbstract = true;

                AddModelClass(package, "StorageChild", firstParent, secondParent);
            });

            Assert.That(failure, Is.Not.Null, "The model guarantees single inheritance, so a second direct parent must stop generation.");

            using (Assert.EnterMultipleScope())
            {
                Assert.That(failure, Does.Contain(nameof(InvalidOperationException)), "A second parent is a model problem reported as an invalid operation.");
                Assert.That(failure, Does.Contain("'storage_child_id_fkey'"), "Both parents would claim the same parent foreign-key name.");
                Assert.That(failure, Does.Contain("'StorageParentA'"), "The message must name the first generalization.");
                Assert.That(failure, Does.Contain("'StorageParentB'"), "The message must name the second generalization.");
            }
        }

        [GeneratedRegex(@"(?m)^CREATE TABLE fabric\.([a-z_][a-z0-9_]*) \(")]
        private static partial Regex TableDeclaration();

        [GeneratedRegex(@"CREATE TRIGGER ([a-z_][a-z0-9_]*)\s+AFTER INSERT OR UPDATE OR DELETE ON fabric\.storage_part\s+" +
            @"FOR EACH ROW EXECUTE FUNCTION fabric\.track_ownership\('id', '(primary_owner|secondary_owner)'\);")]
        private static partial Regex OwnershipTrackingTrigger();

        [GeneratedRegex(@"CREATE CONSTRAINT TRIGGER ([a-z_][a-z0-9_]*)\s+AFTER INSERT OR UPDATE OR DELETE ON fabric\.storage_part\s+" +
            @"DEFERRABLE INITIALLY DEFERRED\s+FOR EACH ROW EXECUTE FUNCTION fabric\.validate_ownership\('id', '(primary_owner|secondary_owner)'\);")]
        private static partial Regex OwnershipCheckTrigger();

        private static async Task<string> RenderFailureAsync(Action<IPackage> configure)
        {
            try
            {
                await RenderScenarioAsync(configure);
            }
            catch (Exception exception)
            {
                var details = new List<string>();

                for (var current = exception; current != null; current = current.InnerException)
                {
                    details.Add($"{current.GetType().Name}: {current.Message}");
                }

                return string.Join(Environment.NewLine, details);
            }

            return null;
        }

        private static async Task<string> RenderScenarioAsync(Action<IPackage> configure)
        {
            var result = GeneratorSetupFixture.ReadFunctionalData();

            configure(GeneratorSetupFixture.QueryFunctionalDataPackage(result));

            var scenarioDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "UML", "_Mycelium.SDK.SqlScenarios",
                TestContext.CurrentContext.Test.ID));

            await new UmlSqlSchemaGenerator().GenerateAsync(result, scenarioDirectory);

            return await GeneratorSetupFixture.ReadSourceAsync(Path.Combine(scenarioDirectory.FullName, "schema.sql"));
        }

        private static Class AddModelClass(IPackage package, string name, params IClass[] parents)
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

        private static void AddOwnerReference(Class owner, Class part, string role)
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
