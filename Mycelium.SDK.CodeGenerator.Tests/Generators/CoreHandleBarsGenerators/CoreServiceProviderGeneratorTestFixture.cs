// ------------------------------------------------------------------------------------------------
//  <copyright file="CoreServiceProviderGeneratorTestFixture.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Tests.Generators.CoreHandleBarsGenerators
{
    using Mycelium.SDK.CodeGenerator.Generators.CoreHandleBarsGenerators;
    using Mycelium.SDK.CodeGenerator.Tests.Core;
    using Mycelium.SDK.CodeGenerator.Tests.Generators.UmlHandleBarsGenerators;

    [TestFixture]
    public class CoreServiceProviderGeneratorTestFixture : UmlClassGeneratorTestFixtureBase
    {
        private const string ProviderFileName = "CoreDataServiceProvider.cs";

        private DirectoryInfo expectedDirectory;
        private DirectoryInfo stagingDirectory;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.expectedDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Expected", "Core", "AutoGenCoreServices"));

            this.stagingDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Core", "_Mycelium.Fabric.AutoGenCoreServices"));

            if (this.stagingDirectory.Exists)
            {
                this.stagingDirectory.Delete(true);
            }

            var generator = new CoreServiceProviderGenerator();

            await generator.GenerateAsync(CoreXmiLoader.ReadSysMLAbstractSyntaxModel(), this.stagingDirectory);
        }

        [Test]
        [Category("Expected")]
        public async Task VerifyThatProviderMatchesItsGoldenFile()
        {
            await AssertFilesMatchAsync(Path.Combine(this.stagingDirectory.FullName, ProviderFileName), Path.Combine(this.expectedDirectory.FullName, ProviderFileName),
                $"Generated '{ProviderFileName}'", "its reviewed golden");
        }
    }
}
