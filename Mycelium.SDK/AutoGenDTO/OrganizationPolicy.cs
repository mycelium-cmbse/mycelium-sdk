// ------------------------------------------------------------------------------------------------
//  <copyright file="OrganizationPolicy.cs" company="Starion Group S.A.">
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
    /// Holds configurable policy settings for an <see cref="Organization" />. Separated from identity data
    /// to allow independent auditing of policy changes.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public partial class OrganizationPolicy : IOrganizationPolicy
    {
        /// <summary>
        /// Represents the unique identifier that allow entity identification.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// When true, Organization Members are permitted to create new projects within this organization.
        /// </summary>
        public bool AllowProjectCreation { get; set; }

        /// <summary>
        /// References the <see cref="User" /> that created the current <see cref="IAuditableThing" />.
        /// </summary>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// Provides the creation <see cref="DateTime" /> of the current <see cref="IAuditableThing" />
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Defines the default ProjectLifecycle that should be set on a newly created project.
        /// </summary>
        public ProjectLifecycleKind DefaultProjectLifecycleOnCreate { get; set; }

        /// <summary>
        /// When true, grants the Organization Administrator implicit read-only access to all projects in the
        /// organization for audit purposes.
        /// </summary>
        public bool GrantReadOnlyViewForAudit { get; set; }

        /// <summary>
        /// References the <see cref="User" /> that provide the last update on the current
        /// <see cref="IAuditableThing" />.
        /// </summary>
        public Guid UpdatedBy { get; set; }

        /// <summary>
        /// Provides the last modification <see cref="DateTime" /> of the current <see cref="IAuditableThing" />
        /// </summary>
        public DateTime UpdatedOn { get; set; }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
