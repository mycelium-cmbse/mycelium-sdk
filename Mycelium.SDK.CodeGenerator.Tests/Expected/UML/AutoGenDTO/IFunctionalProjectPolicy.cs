// ------------------------------------------------------------------------------------------------
//  <copyright file="IFunctionalProjectPolicy.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.DTO
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;

    /// <summary>
    /// The FunctionalProjectPolicy defines policies t associated to as specific
    /// <see cref="FunctionalProject" />.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public partial interface IFunctionalProjectPolicy : IAuditableThing
    {
        /// <summary>
        /// Asserts if automatically importing namespace is allowed or not.
        /// </summary>
        bool AllowAutoNamespaceImport { get; set; }

        /// <summary>
        /// Asserts if publish mode can be set on this project or not.
        /// </summary>
        bool AllowAutoPublishMode { get; set; }

        /// <summary>
        /// Asserts that the branching system for the version control is allowed or not.
        /// </summary>
        bool AllowVersionBranching { get; set; }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
