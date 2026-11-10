// ------------------------------------------------------------------------------------------------
//  <copyright file="CoreServiceInterfaceGeneratorTestFixture.cs" company="Starion Group S.A.">
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
    public class CoreServiceInterfaceGeneratorTestFixture
    {
        private DirectoryInfo expectedDirectory = null!;
        private DirectoryInfo stagingDirectory = null!;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.expectedDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Expected", "Core", "AutoGenCoreServices"));

            this.stagingDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Core", "_Mycelium.Fabric.AutoGenCoreServices"));

            if (this.stagingDirectory.Exists)
            {
                this.stagingDirectory.Delete(true);
            }

            var generator = new CoreServiceInterfaceGenerator();

            await generator.GenerateAsync(CoreXmiLoader.ReadSysMLAbstractSyntaxModel(), this.stagingDirectory);
        }

        [Test]
        [TestCaseSource(typeof(RepresentativeCoreClasses))]
        [Category("Expected")]
        public async Task VerifyThatRepresentativeInterfacesMatchTheirGoldenFiles(string className)
        {
            var fileName = $"I{className}Service.cs";
            var stagedPath = Path.Combine(this.stagingDirectory.FullName, fileName);

            Assert.That(File.Exists(stagedPath), Is.True, $"No interface was generated for the representative metaclass '{className}'.");

            var stagedContent = await File.ReadAllTextAsync(stagedPath);
            var goldenContent = await File.ReadAllTextAsync(Path.Combine(this.expectedDirectory.FullName, fileName));

            Assert.That(stagedContent, Is.EqualTo(goldenContent), $"Generated '{fileName}' differs from its approved golden.");
        }

        [Test]
        public async Task VerifyThatBatchGenerationWritesNoFilesWhenModelValidationFails()
        {
            var model = CoreXmiLoader.ReadSysMLAbstractSyntaxModel();
            var outputDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Core", "_InvalidCoreServices"));

            if (outputDirectory.Exists)
            {
                outputDirectory.Delete(true);
            }

            model.QueryConcreteElementClasses()[^1].Name = null!;

            await Assert.ThatAsync(() => new CoreServiceInterfaceGenerator().GenerateAsync(model, outputDirectory), Throws.TypeOf<InvalidOperationException>());

            outputDirectory.Refresh();

            Assert.That(outputDirectory.Exists, Is.False, "A failed batch created its output directory.");
        }
    }
}
