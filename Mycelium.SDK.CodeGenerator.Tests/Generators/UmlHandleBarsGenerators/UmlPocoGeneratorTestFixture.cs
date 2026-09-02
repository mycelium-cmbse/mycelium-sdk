// ------------------------------------------------------------------------------------------------
//  <copyright file="UmlPocoGeneratorTestFixture.cs" company="Starion Group S.A.">
// 
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
// 
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Tests.Generators.UmlHandleBarsGenerators
{
    using System.Text;

    using Mycelium.SDK.CodeGenerator.Generators.UmlHandleBarsGenerators;

    [TestFixture]
    public class UmlPocoGeneratorTestFixture
    {
        private static readonly UTF8Encoding StrictUtf8WithoutBom =
            new(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

        private DirectoryInfo committedDirectory = null!;
        private DirectoryInfo expectedDirectory = null!;
        private DirectoryInfo stagingDirectory = null!;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.committedDirectory = new DirectoryInfo(
                Path.Combine(
                    TestContext.CurrentContext.TestDirectory,
                    "Committed",
                    "Mycelium.SDK",
                    "AutoGenPOCO"));

            this.expectedDirectory = new DirectoryInfo(
                Path.Combine(
                    TestContext.CurrentContext.TestDirectory,
                    "Expected",
                    "UML",
                    "AutoGenPOCO"));

            this.stagingDirectory = new DirectoryInfo(
                Path.Combine(
                    TestContext.CurrentContext.TestDirectory,
                    "UML",
                    "_Mycelium.SDK.AutoGenPOCO"));

            if (this.stagingDirectory.Exists)
            {
                this.stagingDirectory.Delete(recursive: true);
            }

            var generator = new UmlPocoGenerator();

            await generator.GenerateAsync(
                GeneratorSetupFixture.ResourcesDirectory,
                this.stagingDirectory);
        }

        [Test]
        public async Task Verify_that_complete_batch_matches_committed_SDK_POCOs()
        {
            Assert.That(
                this.committedDirectory.Exists,
                Is.True,
                "The committed SDK POCO directory was not copied to the test output.");

            if (!this.committedDirectory.Exists)
            {
                return;
            }

            var generatedFileNames = QueryCSharpFileNames(this.stagingDirectory);
            var committedFileNames = QueryCSharpFileNames(this.committedDirectory);

            Assert.That(
                generatedFileNames,
                Is.EqualTo(committedFileNames),
                "The generated and committed POCO file sets differ.");

            foreach (var fileName in generatedFileNames)
            {
                var generatedBytes = await File.ReadAllBytesAsync(
                    Path.Combine(this.stagingDirectory.FullName, fileName));

                var committedBytes = await File.ReadAllBytesAsync(
                    Path.Combine(this.committedDirectory.FullName, fileName));

                Assert.That(
                    generatedBytes,
                    Is.EqualTo(committedBytes),
                    $"Generated POCO '{fileName}' differs byte-for-byte from the committed SDK source.");
            }
        }

        [Test]
        [Category("Expected")]
        public async Task Verify_that_representative_POCOs_match_reviewed_goldens()
        {
            foreach (var expectedFile in this.expectedDirectory
                         .GetFiles("*.cs", SearchOption.TopDirectoryOnly)
                         .OrderBy(file => file.Name, StringComparer.Ordinal))
            {
                var generatedPath =
                    Path.Combine(this.stagingDirectory.FullName, expectedFile.Name);

                Assert.That(
                    File.Exists(generatedPath),
                    Is.True,
                    $"Representative POCO '{expectedFile.Name}' was not generated.");

                if (!File.Exists(generatedPath))
                {
                    continue;
                }

                var expectedBytes =
                    await File.ReadAllBytesAsync(expectedFile.FullName);

                var generatedBytes =
                    await File.ReadAllBytesAsync(generatedPath);

                Assert.That(
                    generatedBytes,
                    Is.EqualTo(expectedBytes),
                    $"Generated POCO '{expectedFile.Name}' differs byte-for-byte from its reviewed golden.");
            }
        }

        [Test]
        public async Task Verify_that_generated_POCOs_use_the_required_file_format()
        {
            foreach (var fileName in QueryCSharpFileNames(this.stagingDirectory))
            {
                var bytes = await File.ReadAllBytesAsync(
                    Path.Combine(this.stagingDirectory.FullName, fileName));

                var hasUtf8Bom =
                    bytes.Length >= 3
                    && bytes[0] == 0xEF
                    && bytes[1] == 0xBB
                    && bytes[2] == 0xBF;

                var source = StrictUtf8WithoutBom.GetString(bytes);

                var sourceWithoutCrLf = source.Replace(
                    "\r\n",
                    string.Empty,
                    StringComparison.Ordinal);

                using (Assert.EnterMultipleScope())
                {
                    Assert.That(
                        hasUtf8Bom,
                        Is.False,
                        $"Generated POCO '{fileName}' contains a UTF-8 byte-order mark.");

                    Assert.That(
                        source,
                        Does.Contain("\r\n"),
                        $"Generated POCO '{fileName}' contains no CRLF line endings.");

                    Assert.That(
                        sourceWithoutCrLf,
                        Does.Not.Contain("\r"),
                        $"Generated POCO '{fileName}' contains a standalone carriage return.");

                    Assert.That(
                        sourceWithoutCrLf,
                        Does.Not.Contain("\n"),
                        $"Generated POCO '{fileName}' contains a standalone line feed.");
                }
            }
        }

        private static string[] QueryCSharpFileNames(DirectoryInfo directory)
        {
            return directory
                .GetFiles("*.cs", SearchOption.TopDirectoryOnly)
                .Select(file => file.Name)
                .OrderBy(fileName => fileName, StringComparer.Ordinal)
                .ToArray();
        }
    }
}