// ------------------------------------------------------------------------------------------------
//  <copyright file="UmlEnumGeneratorTestFixture.cs" company="Starion Group S.A.">
// 
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
// 
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Tests.Generators.UmlHandleBarsGenerators
{
    using Mycelium.SDK.CodeGenerator.Generators.UmlHandleBarsGenerators;
    using Mycelium.SDK.CodeGenerator.Tests.Expected;
    using Mycelium.SDK.CodeGenerator.Tests.Xmi;

    /// <summary>
    /// Verifies FunctionalData enumeration generation against the reviewed golden files.
    /// </summary>
    [TestFixture]
    public class UmlEnumGeneratorTestFixture
    {
        /// <summary>
        /// The directory containing the reviewed enumeration golden files.
        /// </summary>
        private DirectoryInfo expectedDirectory = null!;
        
        /// <summary>
        /// The staging directory containing the generated enumeration files.
        /// </summary>
        private DirectoryInfo stagingDirectory = null!;

        /// <summary>
        /// Generates the enumeration files used by the fixture's tests.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous fixture setup operation.
        /// </returns>
        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.expectedDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Expected", "UML", "AutoGenEnum"));
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "UML", "_Mycelium.SDK.AutoGenEnum");
            this.stagingDirectory = new DirectoryInfo(path);

            if (this.stagingDirectory.Exists)
            {
                this.stagingDirectory.Delete(recursive: true);
            }

            this.stagingDirectory.Create();

            var generator = new UmlEnumGenerator();
            var xmiReaderResult = XmiLoadingTestFixture.ReadFunctionalData();

            await generator.GenerateAsync(xmiReaderResult, this.stagingDirectory);
        }

        /// <summary>
        /// Verifies that the generated and reviewed enumeration file sets contain exactly the expected
        /// files.
        /// </summary>
        [Test]
        public void Verify_that_no_enumeration_files_are_missing_or_extra()
        {
            var expectedFileNames = new ExpectedEnumerations()
                .Select(name => $"{name}.cs")
                .OrderBy(name => name, StringComparer.Ordinal)
                .ToArray();

            var reviewedFileNames = QueryCSharpFileNames(this.expectedDirectory);

            var generatedFileNames = QueryCSharpFileNames(this.stagingDirectory);

            Assert.Multiple(() =>
            {
                Assert.That(expectedFileNames, Has.Length.EqualTo(8));

                Assert.That(
                    reviewedFileNames,
                    Is.EqualTo(expectedFileNames),
                    "The reviewed golden-file set is incorrect.");

                Assert.That(
                    generatedFileNames,
                    Is.EqualTo(expectedFileNames),
                    "The generated file set contains missing or extra files.");
            });
        }

        /// <summary>
        /// Verifies that a generated enumeration matches its reviewed golden file.
        /// </summary>
        /// <param name="enumerationName">
        /// The name of the enumeration to verify.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous test operation.
        /// </returns>
        [TestCaseSource(typeof(ExpectedEnumerations))]
        [Category("Expected")]
        public async Task Verify_that_generated_enumeration_matches_reviewed_golden_file(string enumerationName)
        {
            var fileName = $"{enumerationName}.cs";

            var expectedPath = Path.Combine(this.expectedDirectory.FullName, fileName);

            var generatedPath = Path.Combine(this.stagingDirectory.FullName, fileName);

            var expected = await File.ReadAllTextAsync(expectedPath);
            var generated = await File.ReadAllTextAsync(generatedPath);

            Assert.That(
                generated,
                Is.EqualTo(expected),
                $"Generated '{fileName}' differs from its reviewed golden file.");
        }

        /// <summary>
        /// Queries the C# filenames in the specified directory in deterministic order.
        /// </summary>
        /// <param name="directory">
        /// The directory whose C# filenames are queried.
        /// </param>
        /// <returns>
        /// The deterministically ordered C# filenames.
        /// </returns>
        private static string[] QueryCSharpFileNames(DirectoryInfo directory)
        {
            return directory
                .GetFiles("*.cs", SearchOption.TopDirectoryOnly)
                .Select(file => file.Name)
                .OrderBy(name => name, StringComparer.Ordinal)
                .ToArray();
        }
    }
}
