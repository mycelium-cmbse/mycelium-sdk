// ------------------------------------------------------------------------------------------------
//  <copyright file="Ownership.cs" company="Starion Group S.A.">
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
    public partial class Ownership : IOwnership
    {
        /// <summary>
        /// Represents the unique identifier that allow entity identification.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// References the <see cref="User" /> that created the current <see cref="IAuditableThing" />.
        /// </summary>
        public IUser CreatedBy { get; set; }

        /// <summary>
        /// Provides the creation <see cref="DateTime" /> of the current <see cref="IAuditableThing" />
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the description of the current ownership.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets the engineering metadata definition that has to be mapped to replicate the Ownership on the
        /// concurrent server.
        /// </summary>
        public Guid EngineeringMetadataId { get; set; }

        /// <summary>
        /// The full display name of the ownership domain (e.g. "Thermal", "Power").
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// An abbreviated label used for ownership indicators in diagrams and UI elements (e.g. "THM", "PWR").
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// References the <see cref="User" /> that provide the last update on the current
        /// <see cref="IAuditableThing" />.
        /// </summary>
        public IUser UpdatedBy { get; set; }

        /// <summary>
        /// Provides the last modification <see cref="DateTime" /> of the current <see cref="IAuditableThing" />
        /// </summary>
        public DateTime UpdatedOn { get; set; }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
