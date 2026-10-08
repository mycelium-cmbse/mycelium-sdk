// ------------------------------------------------------------------------------------------------
//  <copyright file="XmiResources.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Extensions
{
    /// <summary>
    /// Names the XMI resources under <c>Resources/</c> and the models they carry.
    /// </summary>
    public static class XmiResources
    {
        /// <summary>
        /// The file name of the packaged common primitives resolved through the local reference base path.
        /// </summary>
        public const string CSharpPrimitivesFileName = "mycelium-commonprimitives.xmi";

        /// <summary>
        /// The file name of the packaged Fabric export used by the existing FunctionalData loading entry points.
        /// </summary>
        public const string FunctionalDataFileName = "mycelium-fabric.xmi";

        /// <summary>
        /// The exact name of the package carrying the Fabric SDK model.
        /// </summary>
        public const string FunctionalDataPackageName = "Fabric";

        /// <summary>
        /// The file name of the Systems Modeling API and Services PIM export.
        /// </summary>
        public const string SystemsModelingApiPimFileName = "ptc-25-02-29.xmi";

        /// <summary>
        /// The exact name of the model carrying the Systems Modeling API and Services PIM.
        /// </summary>
        public const string SystemsModelingApiPimModelName = "Systems Modeling API and Services PIM";
    }
}
