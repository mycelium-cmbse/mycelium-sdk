// ------------------------------------------------------------------------------------------------
//  <copyright file="UmlClassGeneratorTestFixtureBase.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Tests.Generators.UmlHandleBarsGenerators
{

    public abstract class UmlClassGeneratorTestFixtureBase
    {

        protected static async Task AssertFilesMatchAsync(string generatedPath, string expectedPath, string generatedDescription, string expectedDescription)
        {
            Assert.That(File.Exists(generatedPath), Is.True, $"{generatedDescription} was not generated.");

            Assert.That(File.Exists(expectedPath), Is.True, $"The file representing {expectedDescription} is missing.");

            if (!File.Exists(generatedPath) || !File.Exists(expectedPath))
            {
                return;
            }

            var generatedSource = await GeneratorSetupFixture.ReadSourceAsync(generatedPath);

            var expectedSource = await GeneratorSetupFixture.ReadSourceAsync(expectedPath);

            Assert.That(generatedSource, Is.EqualTo(expectedSource), $"{generatedDescription} differs from {expectedDescription}.");
        }

        protected static string[] QueryCSharpFileNames(DirectoryInfo directory)
        {
            return directory.GetFiles("*.cs", SearchOption.TopDirectoryOnly)
                .Select(file => file.Name)
                .OrderBy(fileName => fileName, StringComparer.Ordinal)
                .ToArray();
        }
    }
}
