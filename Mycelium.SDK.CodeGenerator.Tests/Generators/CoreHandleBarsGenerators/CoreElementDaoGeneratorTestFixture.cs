// ------------------------------------------------------------------------------------------------
//  <copyright file="CoreElementDaoGeneratorTestFixture.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Tests.Generators.CoreHandleBarsGenerators
{
    using Mycelium.SDK.CodeGenerator.Extensions;
    using Mycelium.SDK.CodeGenerator.Generators.CoreHandleBarsGenerators;
    using Mycelium.SDK.CodeGenerator.Tests.Core;
    using Mycelium.SDK.CodeGenerator.Tests.Expected;

    [TestFixture]
    public class CoreElementDaoGeneratorTestFixture
    {
        private const string ProviderFileName = "ElementDaoProvider.cs";

        private HashSet<string> concreteElementClassNames = null!;
        private DirectoryInfo expectedDirectory = null!;
        private DirectoryInfo stagingDirectory = null!;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.expectedDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Expected", "Core", "AutoGenElementDao"));

            this.stagingDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Core", "_Mycelium.Fabric.AutoGenElementDao"));

            if (this.stagingDirectory.Exists)
            {
                this.stagingDirectory.Delete(true);
            }

            var model = CoreXmiLoader.ReadSysMLAbstractSyntaxModel();

            this.concreteElementClassNames = model.QueryConcreteElementClasses()
                .Select(umlClass => umlClass.Name)
                .ToHashSet(StringComparer.Ordinal);

            await new CoreElementDaoGenerator().GenerateAsync(model, this.stagingDirectory);
        }

        [Test]
        [Category("Expected")]
        public async Task VerifyThatRepresentativeDaosMatchTheirGoldenFiles()
        {
            string[] representativeFileNames =
            [
                .. new RepresentativeElementClasses()
                    .Where(this.concreteElementClassNames.Contains)
                    .Select(className => $"{className}Dao.cs")
                    .Append(ProviderFileName)
                    .OrderBy(fileName => fileName, StringComparer.Ordinal)
            ];

            var goldenFileNames = GeneratedOutput.QueryRelativeFileNames(this.expectedDirectory);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(goldenFileNames, Is.EqualTo(representativeFileNames),
                    "The element DAO golden set must contain exactly the concrete representative metaclasses and the provider.");

                foreach (var fileName in representativeFileNames.Intersect(goldenFileNames, StringComparer.Ordinal))
                {
                    var stagedContent = await File.ReadAllTextAsync(Path.Combine(this.stagingDirectory.FullName, fileName));
                    var goldenContent = await File.ReadAllTextAsync(Path.Combine(this.expectedDirectory.FullName, fileName));

                    Assert.That(stagedContent, Is.EqualTo(goldenContent), $"Generated '{fileName}' differs from its approved golden.");
                }
            }
        }
    }
}
