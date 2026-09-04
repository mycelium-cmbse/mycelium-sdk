// ------------------------------------------------------------------------------------------------
//  <copyright file="XmiLoadingTestFixture.cs" company="Starion Group S.A.">
// 
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
// 
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Tests.Xmi
{
    using System.Text;

    using Mycelium.SDK.CodeGenerator.Extensions;
    using Mycelium.SDK.CodeGenerator.Tests.Generators.UmlHandleBarsGenerators;

    using uml4net.Packages;
    using uml4net.xmi.Readers;

    [TestFixture]
    public class XmiLoadingTestFixture
    {
        private const string PrimitiveTypesUri =
            "http://www.omg.org/spec/UML/20161101/PrimitiveTypes.xmi";

        private static DirectoryInfo ResourcesDirectory =>
            GeneratorSetupFixture.ResourcesDirectory;

        [Test]
        public void Verify_that_FunctionalData_is_loaded_through_the_canonical_path()
        {
            var result = ReadFunctionalData();
            var package = QueryFunctionalDataPackage(result);

            Assert.That(
                package.Name,
                Is.EqualTo(XmiReaderResultExtensions.FunctionalDataPackageName));
        }

        [Test]
        public void Verify_that_reader_uses_local_reference_settings()
        {
            var settings =
                XmiReaderResultExtensions.CreateFunctionalDataReaderSettings(
                    ResourcesDirectory);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(
                    settings.LocalReferenceBasePath,
                    Is.EqualTo(ResourcesDirectory.FullName));

                Assert.That(
                    settings.PathMaps.TryGetValue(
                        PrimitiveTypesUri,
                        out var primitiveTypesPath),
                    Is.True);

                Assert.That(
                    primitiveTypesPath,
                    Is.EqualTo(
                        Path.Combine(
                            ResourcesDirectory.FullName,
                            "PrimitiveTypes.xmi")));

                Assert.That(settings.UseStrictReading, Is.True);
            }
        }

        internal static XmiReaderResult ReadFunctionalData()
        {
            return XmiReaderResultExtensions.ReadFunctionalData(
                ResourcesDirectory);
        }

        internal static IPackage QueryFunctionalDataPackage(
            XmiReaderResult result)
        {
            return result.QueryFunctionalDataPackage();
        }
    }
}