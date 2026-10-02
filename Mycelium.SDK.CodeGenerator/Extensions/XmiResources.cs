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
        /// The canonical URI by which a model references the standard UML primitive types.
        /// </summary>
        public const string PrimitiveTypesUri = "http://www.omg.org/spec/UML/20161101/PrimitiveTypes.xmi";

        /// <summary>
        /// The file name under which the standard UML primitive types are resolved locally.
        /// </summary>
        public const string PrimitiveTypesFileName = "PrimitiveTypes.xmi";

        /// <summary>
        /// The file name of the FunctionalData export.
        /// </summary>
        public const string FunctionalDataFileName = "FunctionalData.xmi";

        /// <summary>
        /// The exact name of the package carrying the FunctionalData model.
        /// </summary>
        public const string FunctionalDataPackageName = "FunctionalData";

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
