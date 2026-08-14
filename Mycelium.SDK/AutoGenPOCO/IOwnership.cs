// ------------------------------------------------------------------------------------------------
//  <copyright file="IOwnership.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.POCO
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;

    /// <summary>
    /// Defines a named domain of responsibility within a <see cref="FunctionalProject" />. Used in
    /// Concurrent Design mode to enforce element-level access control via Owner metadata on SysML v2
    /// elements.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public partial interface IOwnership : IAuditableThing
    {
        /// <summary>
        /// Gets or sets the description of the current ownership.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Gets the engineering metadata definition that has to be mapped to replicate the Ownership on the
        /// concurrent server.
        /// </summary>
        Guid EngineeringMetadataId { get; set; }

        /// <summary>
        /// The full display name of the ownership domain (e.g. "Thermal", "Power").
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// An abbreviated label used for ownership indicators in diagrams and UI elements (e.g. "THM", "PWR").
        /// </summary>
        string ShortName { get; set; }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
