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
        /// The name of the UML profile, applied in the Fabric export, whose stereotypes carry generation semantics.
        /// </summary>
        public const string MyceliumProfileName = "Mycelium";

        /// <summary>
        /// The name of the stereotype that marks a property needing a dedicated index in the generated SQL schema.
        /// </summary>
        public const string IndexStereotypeName = "index";

        /// <summary>
        /// The name of the stereotype that marks a property whose value the server sets.
        /// </summary>
        public const string ManagedStereotypeName = "managed";

        /// <summary>
        /// The name of the tagged value, exported only in the Enterprise Architect extension, that states when the server assigns a managed value.
        /// </summary>
        public const string OnActionTaggedValueName = "onAction";

        /// <summary>
        /// The <c>onAction</c> value that assigns a managed value when an object is created.
        /// </summary>
        public const string OnCreateAction = "OnCreate";

        /// <summary>
        /// The <c>onAction</c> value that assigns a managed value when an object is updated.
        /// </summary>
        public const string OnUpdateAction = "OnUpdate";

        /// <summary>
        /// The file name of the SysML v2 abstract syntax export, which imports the KerML one.
        /// </summary>
        public const string SysMLAbstractSyntaxFileName = "SysML_only_xmi.uml";

        /// <summary>
        /// The exact name of the model carrying the SysML v2 abstract syntax.
        /// </summary>
        public const string SysMLAbstractSyntaxModelName = "SysML";

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
