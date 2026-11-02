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

    [TestFixture]
    public class UmlSqlSchemaGeneratorTestFixture
    {
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
    }
}
