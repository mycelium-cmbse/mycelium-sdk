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
        /// <summary>
        /// The OMG Systems Modeling API and Services REST/HTTP platform specific model.
        /// </summary>
        private const string SystemsModelingApiFileName = "ptc-25-02-30.json";

        private static string ResourcesDirectory => Path.Combine(AppContext.BaseDirectory, "Resources");

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

        [Test]
        public void VerifyThatReadingAnAbsentResourceThrows()
        {
            Assert.That(
                () => ReadAsync("does-not-exist.json"),
                Throws.InstanceOf<FileNotFoundException>());
        }

        /// <summary>
        /// Reads the OMG Systems Modeling API and Services document.
        /// </summary>
        /// <returns>
        /// The parsed OpenAPI document.
        /// </returns>
        internal static async Task<OpenApiDocument> ReadSystemsModelingApiAsync()
        {
            var readResult = await ReadAsync(SystemsModelingApiFileName);

            return readResult.Document;
        }

        /// <summary>
        /// Reads an OpenAPI document from the test resources.
        /// </summary>
        /// <param name="fileName">
        /// The name of the resource to read.
        /// </param>
        /// <returns>
        /// The read result, carrying both the document and its diagnostic.
        /// </returns>
        private static async Task<ReadResult> ReadAsync(string fileName)
        {
            await using var stream = File.OpenRead(Path.Combine(ResourcesDirectory, fileName));

            return await OpenApiDocument.LoadAsync(stream, "json", CreateReaderSettings());
        }

        /// <summary>
        /// Creates the reader settings used for every OpenAPI resource.
        /// </summary>
        /// <returns>
        /// The reader settings.
        /// </returns>
        /// <remarks>
        /// The default rule set validates every schema reference, which fails on this document: the OMG
        /// export gives each schema a <c>$id</c> that the validator tries to resolve as a
        /// <see cref="Uri"/>, and throws <see cref="UriFormatException"/> when it cannot. Validation of
        /// the model shape is not what this generator relies on - the generator fails loudly by itself
        /// on anything it cannot render - so the rule set is emptied.
        /// </remarks>
        private static OpenApiReaderSettings CreateReaderSettings()
        {
            return new OpenApiReaderSettings
            {
                RuleSet = ValidationRuleSet.GetEmptyRuleSet()
            };
        }
    }
}
