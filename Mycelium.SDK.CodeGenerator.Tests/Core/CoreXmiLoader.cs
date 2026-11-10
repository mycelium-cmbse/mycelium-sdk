// ------------------------------------------------------------------------------------------------
//  <copyright file="CoreXmiLoader.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Tests.Core
{
    using Mycelium.SDK.CodeGenerator.Extensions;
    using Mycelium.SDK.CodeGenerator.Tests.Generators.UmlHandleBarsGenerators;

    using uml4net.Packages;

    internal static class CoreXmiLoader
    {
        internal static IPackage ReadSysMLAbstractSyntaxModel()
        {
            return GeneratorSetupFixture.ResourcesDirectory
                .ReadModel(XmiResources.SysMLAbstractSyntaxFileName, useStrictReading: false)
                .QueryPackage(XmiResources.SysMLAbstractSyntaxModelName);
        }
    }
}
