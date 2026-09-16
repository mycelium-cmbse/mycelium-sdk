// ------------------------------------------------------------------------------------------------
//  <copyright file="GeneratorSetupFixture.cs" company="Starion Group S.A.">
// 
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
// 
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Tests.Generators.UmlHandleBarsGenerators
{
    using Mycelium.SDK.CodeGenerator.Extensions;

    using uml4net.Packages;
    using uml4net.xmi.Readers;

    [SetUpFixture]
    public sealed class GeneratorSetupFixture
    {
        public static DirectoryInfo ResourcesDirectory =>
            new(Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources"));

        public static XmiReaderResult ReadFunctionalData() => XmiReaderResultExtensions.ReadFunctionalData(ResourcesDirectory);

        public static IPackage QueryFunctionalDataPackage(XmiReaderResult xmiReaderResult)
        {
            ArgumentNullException.ThrowIfNull(xmiReaderResult);

            return xmiReaderResult.QueryFunctionalDataPackage();
        }
    }
}
