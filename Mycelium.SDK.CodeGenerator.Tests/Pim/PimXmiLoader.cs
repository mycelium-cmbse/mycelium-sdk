// ------------------------------------------------------------------------------------------------
//  <copyright file="PimXmiLoader.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Tests.Pim
{
    using Mycelium.SDK.CodeGenerator.Extensions;
    using Mycelium.SDK.CodeGenerator.Tests.Generators.UmlHandleBarsGenerators;

    using uml4net.Packages;

    /// <summary>
    /// Provides the single path through which the Systems Modeling API and Services PIM is read for
    /// generation.
    /// </summary>
    internal static class PimXmiLoader
    {
        /// <summary>
        /// Reads the PIM and returns the model that the service interfaces are generated from.
        /// </summary>
        /// <returns>
        /// The Systems Modeling API and Services PIM model.
        /// </returns>
        internal static IPackage ReadSystemsModelingApiPimModel()
        {
            return GeneratorSetupFixture.ResourcesDirectory
                .ReadModel(XmiResources.SystemsModelingApiPimFileName, useStrictReading: false)
                .QueryPackage(XmiResources.SystemsModelingApiPimModelName);
        }
    }
}
