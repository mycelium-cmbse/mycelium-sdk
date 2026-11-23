// ------------------------------------------------------------------------------------------------
//  <copyright file="UmlServiceInterfaceGeneratorTestFixture.cs" company="Starion Group S.A.">
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

    [TestFixture]
    public class UmlServiceInterfaceGeneratorTestFixture : UmlClassGeneratorTestFixtureBase
    {
        private Dictionary<string, IClass> classes;

        private DirectoryInfo expectedDirectory;

        private DirectoryInfo stagingDirectory;

        private XmiReaderResult xmiReaderResult;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.expectedDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Expected", "UML", "AutoGenServices"));

            this.stagingDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "UML", "_Mycelium.Fabric.AutoGenServices"));

            if (this.stagingDirectory.Exists)
            {
                this.stagingDirectory.Delete(true);
            }

            this.xmiReaderResult = GeneratorSetupFixture.ReadFunctionalData();

            this.classes = GeneratorSetupFixture.QueryFunctionalDataPackage(this.xmiReaderResult).QueryPackages()
                .SelectMany(package => package.PackagedElement.OfType<IClass>())
                .ToDictionary(umlClass => umlClass.Name, StringComparer.Ordinal);

            await new UmlServiceInterfaceGenerator().GenerateAsync(this.xmiReaderResult, this.stagingDirectory);
        }

        [Test]
        [Category("Expected")]
        public async Task VerifyThatRepresentativeServiceInterfacesMatchTheirGoldens()
        {
            Assert.That(this.expectedDirectory.Exists, Is.True, "The representative service-interface golden directory Expected/UML/AutoGenServices is missing.");

            if (!this.expectedDirectory.Exists)
            {
                return;
            }

            var representativeFileNames = new RepresentativeClasses()
                .Where(className => this.classes.TryGetValue(className, out var umlClass) && !umlClass.IsAbstract)
                .Select(className => $"I{className}Service.cs")
                .OrderBy(fileName => fileName, StringComparer.Ordinal)
                .ToArray();

            var goldenFileNames = GeneratedOutput.QueryRelativeFileNames(this.expectedDirectory);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(goldenFileNames, Is.EqualTo(representativeFileNames), "The service-interface golden set must contain exactly the concrete representative classes.");

                foreach (var fileName in representativeFileNames.Intersect(goldenFileNames, StringComparer.Ordinal))
                {
                    await AssertFilesMatchAsync(
                        Path.Combine(this.stagingDirectory.FullName, fileName),
                        Path.Combine(this.expectedDirectory.FullName, fileName),
                        $"Generated service interface '{fileName}'",
                        "its reviewed golden");
                }
            }
        }

        [Test]
        public void VerifyThatAbstractRepresentativeClassesProduceNoServiceInterface()
        {
            var abstractFileNames = new RepresentativeClasses()
                .Where(className => this.classes.TryGetValue(className, out var umlClass) && umlClass.IsAbstract)
                .Select(className => $"I{className}Service.cs")
                .ToArray();

            var stagedFileNames = GeneratedOutput.QueryRelativeFileNames(this.stagingDirectory);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(abstractFileNames, Is.Not.Empty, "The representative class selection must contain an abstract class for this check to be meaningful.");

                foreach (var fileName in abstractFileNames)
                {
                    Assert.That(stagedFileNames, Does.Not.Contain(fileName), $"The abstract class behind '{fileName}' must not produce a service interface.");
                }
            }
        }

        [Test]
        public async Task VerifyThatGenerationRejectsMissingArguments()
        {
            var generator = new UmlServiceInterfaceGenerator();

            using (Assert.EnterMultipleScope())
            {
                await Assert.ThatAsync(() => generator.GenerateAsync((XmiReaderResult)null, this.stagingDirectory), Throws.ArgumentNullException);

                await Assert.ThatAsync(() => generator.GenerateAsync(this.xmiReaderResult, null), Throws.ArgumentNullException);
            }
        }
    }
}
