// ------------------------------------------------------------------------------------------------
//  <copyright file="BranchProtectionRule.cs" company="Starion Group S.A.">
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
    /// Defines protection rules for a specific engineering branch, controlling merge permissions and review requirements.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public partial class BranchProtectionRule : IBranchProtectionRule
    {
        /// <summary>
        /// Represents the unique identifier that allow entity identification.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// References the <see cref="User" /> that created the current <see cref="AuditableThing" />.
        /// </summary>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// Provides the creation <see cref="DateTime" /> of the current <see cref="AuditableThing" />
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// References the default <see cref="ProjectMember" /> reviewers designated for this
        /// <see cref="BranchProtectionRule" />.
        /// </summary>
        public List<Guid> DefaultReviewers { get; set; } = [];

        /// <summary>
        /// The <see cref="Guid" /> referencing the protected branch in the Concurrent Server.
        /// </summary>
        public Guid EngineeringBranchId { get; set; }

        /// <summary>
        /// The set of <see cref="ProjectMemberRole" /> values permitted to merge into this protected branch.
        /// </summary>
        public List<ProjectMemberRole> MergeAllowedFor { get; set; } = [];

        /// <summary>
        /// Gets or sets the number of approval(s) required before being allowed to merge a Branch.
        /// </summary>
        public int MinimumRequiredApproval { get; set; }

        /// <summary>
        /// Gets or sets the name of the current rule.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Indicates whether at least one approved <see cref="Review" /> is required before a merge can proceed.
        /// </summary>
        public bool ReviewRequired { get; set; }

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
