// ------------------------------------------------------------------------------------------------
//  <copyright file="IOrganizationPolicy.cs" company="Starion Group S.A.">
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
    public partial interface IOrganizationPolicy : IAuditableThing
    {
        /// <summary>
        /// When true, Organization Members are permitted to create new projects within this organization.
        /// </summary>
        bool AllowProjectCreation { get; set; }

        /// <summary>
        /// Defines the default ProjectLifecycle that should be set on a newly created project.
        /// </summary>
        ProjectLifecycleKind DefaultProjectLifecycleOnCreate { get; set; }

        /// <summary>
        /// When true, grants the Organization Administrator implicit read-only access to all projects in the
        /// organization for audit purposes.
        /// </summary>
        bool GrantReadOnlyViewForAudit { get; set; }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
