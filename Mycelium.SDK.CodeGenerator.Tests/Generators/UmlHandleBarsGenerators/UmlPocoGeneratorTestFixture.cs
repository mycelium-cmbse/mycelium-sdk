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
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Mycelium.SDK.CodeGenerator.Generators.UmlHandleBarsGenerators;
    using Mycelium.SDK.CodeGenerator.Tests.Expected;
    using Mycelium.SDK.CodeGenerator.Tests.Xmi;

    [TestFixture]
    public class UmlPocoGeneratorTestFixture
    {
        private static readonly HashSet<string> AbstractClassNames =
            new(StringComparer.Ordinal)
            {
                "AuditableThing",
                "Thing"
            };

        private DirectoryInfo stagingDirectory = null!;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.stagingDirectory = new DirectoryInfo(
                Path.Combine(TestContext.CurrentContext.TestDirectory, "UML", "_Mycelium.SDK.AutoGenPOCO"));

            if (this.stagingDirectory.Exists)
            {
                this.stagingDirectory.Delete(recursive: true);
            }

            var xmiReaderResult = XmiLoadingTestFixture.ReadFunctionalData();

            var generator = new UmlPocoGenerator();

            await generator.GenerateAsync(xmiReaderResult, this.stagingDirectory);
        }

        [Test]
        public void Verify_that_batch_generation_produces_exactly_24_POCO_files()
        {
            var expectedClassNames = new ExpectedClasses().ToArray();

            var expectedInterfaceFileNames = expectedClassNames
                .Select(className => $"I{className}.cs")
                .OrderBy(fileName => fileName, StringComparer.Ordinal)
                .ToArray();

            var expectedConcreteFileNames = expectedClassNames
                .Where(className => !AbstractClassNames.Contains(className))
                .Select(className => $"{className}.cs")
                .OrderBy(fileName => fileName, StringComparer.Ordinal)
                .ToArray();

            var expectedFileNames = expectedInterfaceFileNames
                .Concat(expectedConcreteFileNames)
                .OrderBy(fileName => fileName, StringComparer.Ordinal)
                .ToArray();

            var generatedFileNames = QueryCSharpFileNames(this.stagingDirectory);

            Assert.Multiple(() =>
            {
                Assert.That(expectedInterfaceFileNames, Has.Length.EqualTo(13));
                Assert.That(expectedConcreteFileNames, Has.Length.EqualTo(11));
                Assert.That(expectedFileNames, Has.Length.EqualTo(24));
                Assert.That(
                    generatedFileNames,
                    Is.EqualTo(expectedFileNames),
                    "The generated POCO set contains missing or extra files.");
            });
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