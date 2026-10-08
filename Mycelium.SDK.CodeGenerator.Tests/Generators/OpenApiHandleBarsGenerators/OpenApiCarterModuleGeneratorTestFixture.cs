// ------------------------------------------------------------------------------------------------
//  <copyright file="OpenApiCarterModuleGeneratorTestFixture.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Tests.Generators.OpenApiHandleBarsGenerators
{
    using Mycelium.SDK.CodeGenerator.Generators.OpenApiHandleBarsGenerators;
    using Mycelium.SDK.CodeGenerator.Tests.OpenApi;

    [TestFixture]
    public class OpenApiCarterModuleGeneratorTestFixture
    {

        private DirectoryInfo expectedDirectory = null!;
        private DirectoryInfo stagingDirectory = null!;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.expectedDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Expected", "OpenApi", "AutoGenModules"));

            this.stagingDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "OpenApi", "_Mycelium.Fabric.AutoGenModules"));

            if (this.stagingDirectory.Exists)
            {
                this.stagingDirectory.Delete(true);
            }

            var document = await OpenApiLoadingTestFixture.ReadSystemsModelingApiAsync();
            var generator = new OpenApiCarterModuleGenerator();

            await generator.GenerateAsync(document, this.stagingDirectory);
        }

        [Test]
        public async Task VerifyThatBatchGenerationWritesNoFilesWhenTheDocumentCannotBeGenerated()
        {
            var outputDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "OpenApi", "_Mycelium.Fabric.InvalidAutoGenModules"));

            if (outputDirectory.Exists)
            {
                outputDirectory.Delete(true);
            }

            // An operation without a tag cannot be assigned to a module. The mutation is applied to the
            // freshly read in-memory document; the resource on disk is never touched.
            var document = await OpenApiLoadingTestFixture.ReadSystemsModelingApiAsync();

            document.Paths["/projects"].Operations[HttpMethod.Get].Tags.Clear();

            var generator = new OpenApiCarterModuleGenerator();

            await Assert.ThatAsync(() => generator.GenerateAsync(document, outputDirectory), Throws.TypeOf<InvalidOperationException>());

            outputDirectory.Refresh();

            Assert.That(outputDirectory.Exists, Is.False, "Batch preflight failure created the destination directory.");
        }

        [Test]
        public async Task VerifyThatGeneratedModulesMatchTheirGoldenFiles()
        {
            var stagedFileNames = GeneratedOutput.QueryRelativeFileNames(this.stagingDirectory);
            var goldenFileNames = GeneratedOutput.QueryRelativeFileNames(this.expectedDirectory);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(stagedFileNames, Is.EqualTo(goldenFileNames), "The generated and reviewed module manifests differ.");

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
