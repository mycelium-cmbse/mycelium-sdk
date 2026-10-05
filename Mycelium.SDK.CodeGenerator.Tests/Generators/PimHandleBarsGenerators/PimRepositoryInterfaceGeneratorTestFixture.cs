// ------------------------------------------------------------------------------------------------
//  <copyright file="PimRepositoryInterfaceGeneratorTestFixture.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Tests.Generators.PimHandleBarsGenerators
{
    using Mycelium.SDK.CodeGenerator.Generators.PimHandleBarsGenerators;
    using Mycelium.SDK.CodeGenerator.Tests.Pim;

    [TestFixture]
    public class PimRepositoryInterfaceGeneratorTestFixture
    {
        private DirectoryInfo expectedDirectory = null!;
        private DirectoryInfo stagingDirectory = null!;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.expectedDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Expected", "Pim", "AutoGenRepositories"));

            this.stagingDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Pim", "_Mycelium.Fabric.AutoGenRepositories"));

            if (this.stagingDirectory.Exists)
            {
                this.stagingDirectory.Delete(true);
            }

            var generator = new PimRepositoryInterfaceGenerator();

            await generator.GenerateAsync(PimXmiLoader.ReadSystemsModelingApiPimModel(), this.stagingDirectory);
        }

        [Test]
        public async Task VerifyThatGeneratedInterfacesMatchTheirGoldenFiles()
        {
            var stagedFileNames = GeneratedOutput.QueryRelativeFileNames(this.stagingDirectory);
            var goldenFileNames = GeneratedOutput.QueryRelativeFileNames(this.expectedDirectory);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(stagedFileNames, Is.EqualTo(goldenFileNames), "The generated and reviewed interface manifests differ.");

                foreach (var fileName in stagedFileNames.Intersect(goldenFileNames, StringComparer.Ordinal))
                {
                    var stagedContent = await File.ReadAllTextAsync(Path.Combine(this.stagingDirectory.FullName, fileName));
                    var goldenContent = await File.ReadAllTextAsync(Path.Combine(this.expectedDirectory.FullName, fileName));

                    Assert.That(stagedContent, Is.EqualTo(goldenContent), $"Generated '{fileName}' differs from its approved golden.");
                }
            }
        }
    }
}
