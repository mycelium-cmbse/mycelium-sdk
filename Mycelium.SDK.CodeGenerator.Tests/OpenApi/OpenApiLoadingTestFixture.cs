// ------------------------------------------------------------------------------------------------
//  <copyright file="OpenApiLoadingTestFixture.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Tests.OpenApi
{
    using Microsoft.OpenApi;
    using Microsoft.OpenApi.Reader;

    [TestFixture]
    public class OpenApiLoadingTestFixture
    {
        private const string SystemsModelingApiFileName = "ptc-25-02-30.json";

        private static string ResourcesDirectory => Path.Combine(AppContext.BaseDirectory, "Resources");

        [Test]
        public void VerifyThatReadingAnAbsentResourceThrows()
        {
            Assert.That(() => ReadAsync("does-not-exist.json"), Throws.InstanceOf<FileNotFoundException>());
        }

        [Test]
        public async Task VerifyThatTheSystemsModelingApiResourceIsReadCleanly()
        {
            var readResult = await ReadAsync(SystemsModelingApiFileName);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(readResult.Diagnostic.Errors, Is.Empty);
                Assert.That(readResult.Diagnostic.Warnings, Is.Empty);
                Assert.That(readResult.Diagnostic.SpecificationVersion, Is.EqualTo(OpenApiSpecVersion.OpenApi3_1));
                Assert.That(readResult.Document, Is.Not.Null);
                Assert.That(readResult.Document.Info.Title, Is.EqualTo("Systems Modeling API and Services"));
                Assert.That(readResult.Document.Info.Version, Is.EqualTo("1.0"));
            }
        }

        internal static async Task<OpenApiDocument> ReadSystemsModelingApiAsync()
        {
            var readResult = await ReadAsync(SystemsModelingApiFileName);

            return readResult.Document;
        }

        private static async Task<ReadResult> ReadAsync(string fileName)
        {
            await using var stream = File.OpenRead(Path.Combine(ResourcesDirectory, fileName));

            return await OpenApiDocument.LoadAsync(stream, "json", CreateReaderSettings());
        }

        private static OpenApiReaderSettings CreateReaderSettings() => new() { RuleSet = ValidationRuleSet.GetEmptyRuleSet() };
    }
}
