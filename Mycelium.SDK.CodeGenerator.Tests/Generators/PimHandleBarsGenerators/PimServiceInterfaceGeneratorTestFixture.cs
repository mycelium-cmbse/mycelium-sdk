// ------------------------------------------------------------------------------------------------
//  <copyright file="PimServiceInterfaceGeneratorTestFixture.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Tests.Generators.PimHandleBarsGenerators
{
    using Mycelium.SDK.CodeGenerator.Extensions;
    using Mycelium.SDK.CodeGenerator.Generators.PimHandleBarsGenerators;
    using Mycelium.SDK.CodeGenerator.Tests.Pim;

    [TestFixture]
    public class PimServiceInterfaceGeneratorTestFixture
    {

        private DirectoryInfo expectedDirectory = null!;
        private DirectoryInfo stagingDirectory = null!;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.expectedDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Expected", "Pim", "AutoGenServices"));

            this.stagingDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Pim", "_Mycelium.Fabric.AutoGenServices"));

            if (this.stagingDirectory.Exists)
            {
                this.stagingDirectory.Delete(true);
            }

            var generator = new PimServiceInterfaceGenerator();

            await generator.GenerateAsync(PimXmiLoader.ReadSystemsModelingApiPimModel(), this.stagingDirectory);
        }

        [Test]
        public async Task VerifyThatBatchGenerationWritesNoFilesWhenTheModelCannotBeGenerated()
        {
            var outputDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Pim", "_Mycelium.Fabric.InvalidAutoGenServices"));

            if (outputDirectory.Exists)
            {
                outputDirectory.Delete(true);
            }

            // A parameter without a resolved type cannot be generated. The mutation is applied to the freshly
            // read in-memory model; the resource on disk is never touched.
            var model = PimXmiLoader.ReadSystemsModelingApiPimModel();

            model.QueryServiceClasses()
                .Single(serviceClass => string.Equals(serviceClass.Name, "ProjectService", StringComparison.Ordinal))
                .QueryServiceOperations()
                .Single(operation => string.Equals(operation.Name, "createProject", StringComparison.Ordinal))
                .QueryInputParameters()[0].Type = null!;

            var generator = new PimServiceInterfaceGenerator();

            await Assert.ThatAsync(() => generator.GenerateAsync(model, outputDirectory), Throws.InstanceOf<Exception>());

            outputDirectory.Refresh();

            Assert.That(outputDirectory.Exists, Is.False, "Batch preflight failure created the destination directory.");
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
