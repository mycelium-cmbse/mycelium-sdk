// ------------------------------------------------------------------------------------------------
//  <copyright file="FunctionalProjectPolicy.cs" company="Starion Group S.A.">
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
    /// Defines the policy associated with a specific <see cref="FunctionalProject" />.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public partial class FunctionalProjectPolicy : IFunctionalProjectPolicy
    {
        /// <summary>
        /// Represents the unique identifier that allow entity identification.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Asserts if automatically importing namespace is allowed or not.
        /// </summary>
        public bool AllowAutoNamespaceImport { get; set; }

        /// <summary>
        /// Asserts if publish mode can be set on this project or not.
        /// </summary>
        public bool AllowAutoPublishMode { get; set; }

        /// <summary>
        /// References the <see cref="User" /> that created the current <see cref="AuditableThing" />.
        /// </summary>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// Provides the creation <see cref="DateTime" /> of the current <see cref="AuditableThing" />
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// References the <see cref="User" /> that provide the last update on the current
        /// <see cref="AuditableThing" />.
        /// </summary>
        public Guid UpdatedBy { get; set; }

        /// <summary>
        /// Provides the last modification <see cref="DateTime" /> of the current
        /// <see cref="AuditableThing" />
        /// </summary>
        public DateTime UpdatedOn { get; set; }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
