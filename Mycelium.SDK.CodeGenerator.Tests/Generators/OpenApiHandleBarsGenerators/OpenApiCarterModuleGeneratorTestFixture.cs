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
    using System.Text;

    using Mycelium.SDK.CodeGenerator.Generators.OpenApiHandleBarsGenerators;
    using Mycelium.SDK.CodeGenerator.Tests.OpenApi;

    [TestFixture]
    public class OpenApiCarterModuleGeneratorTestFixture
    {
        private static readonly UTF8Encoding StrictUtf8WithoutBom = new(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

        private DirectoryInfo expectedDirectory = null!;
        private DirectoryInfo stagingDirectory = null!;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.expectedDirectory = new DirectoryInfo(
                Path.Combine(TestContext.CurrentContext.TestDirectory, "Expected", "OpenApi", "AutoGenModules"));

            this.stagingDirectory = new DirectoryInfo(
                Path.Combine(TestContext.CurrentContext.TestDirectory, "OpenApi", "_Mycelium.Fabric.AutoGenModules"));

            if (this.stagingDirectory.Exists)
            {
                this.stagingDirectory.Delete(recursive: true);
            }

            var document = await OpenApiLoadingTestFixture.ReadSystemsModelingApiAsync();
            var generator = new OpenApiCarterModuleGenerator();

            await generator.GenerateAsync(document, this.stagingDirectory);
        }

        [Test]
        public async Task VerifyThatGeneratedModulesMatchTheirGoldenFiles()
        {
            var stagedFileNames = QueryRelativeFileNames(this.stagingDirectory);
            var goldenFileNames = QueryRelativeFileNames(this.expectedDirectory);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(stagedFileNames, Is.EqualTo(goldenFileNames),
                    "The generated and reviewed module manifests differ.");

                foreach (var fileName in stagedFileNames.Intersect(goldenFileNames, StringComparer.Ordinal))
                {
                    var stagedContent = await File.ReadAllTextAsync(Path.Combine(this.stagingDirectory.FullName, fileName));
                    var goldenContent = await File.ReadAllTextAsync(Path.Combine(this.expectedDirectory.FullName, fileName));

                    Assert.That(stagedContent, Is.EqualTo(goldenContent),
                        $"Generated '{fileName}' differs from its approved golden.");
                }
            }
        }

        [Test]
        public async Task VerifyThatGeneratedModulesUseTheRequiredFileFormat()
        {
            using (Assert.EnterMultipleScope())
            {
                foreach (var fileName in QueryRelativeFileNames(this.stagingDirectory))
                {
                    var bytes = await File.ReadAllBytesAsync(Path.Combine(this.stagingDirectory.FullName, fileName));

                    var hasUtf8Bom = bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF;

                    var source = StrictUtf8WithoutBom.GetString(bytes);
                    var sourceWithoutCrLf = source.Replace("\r\n", string.Empty, StringComparison.Ordinal);

                    Assert.That(hasUtf8Bom, Is.False, $"Generated '{fileName}' contains a UTF-8 byte-order mark.");
                    Assert.That(source, Does.Contain("\r\n"), $"Generated '{fileName}' contains no CRLF line endings.");
                    Assert.That(sourceWithoutCrLf, Does.Not.Contain("\r"), $"Generated '{fileName}' contains a standalone carriage return.");
                    Assert.That(sourceWithoutCrLf, Does.Not.Contain("\n"), $"Generated '{fileName}' contains a standalone line feed.");
                    Assert.That(source, Does.Contain("[GeneratedCode(\"Mycelium.SDK\", \"latest\")]"),
                        $"Generated '{fileName}' does not contain the generated-code marker.");
                    Assert.That(source, Does.Contain(": ICarterModule"), $"Generated '{fileName}' is not a Carter module.");
                }
            }
        }

        [Test]
        public async Task VerifyThatBatchGenerationWritesNoFilesWhenTheDocumentCannotBeGenerated()
        {
            var outputDirectory = new DirectoryInfo(
                Path.Combine(TestContext.CurrentContext.TestDirectory, "OpenApi", "_Mycelium.Fabric.InvalidAutoGenModules"));

            if (outputDirectory.Exists)
            {
                outputDirectory.Delete(recursive: true);
            }

            // An operation without a tag cannot be assigned to a module. The mutation is applied to the
            // freshly read in-memory document; the resource on disk is never touched.
            var document = await OpenApiLoadingTestFixture.ReadSystemsModelingApiAsync();

            document.Paths["/projects"].Operations[HttpMethod.Get].Tags.Clear();

            var generator = new OpenApiCarterModuleGenerator();

            await Assert.ThatAsync(
                () => generator.GenerateAsync(document, outputDirectory),
                Throws.TypeOf<InvalidOperationException>());

            outputDirectory.Refresh();

            Assert.That(outputDirectory.Exists, Is.False,
                "Batch preflight failure created the destination directory.");
        }

        private static string[] QueryRelativeFileNames(DirectoryInfo directory)
        {
            return directory
                .GetFiles("*", SearchOption.AllDirectories)
                .Select(file => Path.GetRelativePath(directory.FullName, file.FullName))
                .OrderBy(fileName => fileName, StringComparer.Ordinal)
                .ToArray();
        }
    }
}
