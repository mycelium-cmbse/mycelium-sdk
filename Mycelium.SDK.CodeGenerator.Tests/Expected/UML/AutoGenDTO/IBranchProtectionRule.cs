// ------------------------------------------------------------------------------------------------
//  <copyright file="IBranchProtectionRule.cs" company="Starion Group S.A.">
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
    public partial interface IBranchProtectionRule : IAuditableThing
    {
        /// <summary>
        /// References the default <see cref="ProjectMember" /> reviewers designated for this
        /// <see cref="BranchProtectionRule" />.
        /// </summary>
        List<Guid> defaultReviewers { get; }

        /// <summary>
        /// The <see cref="Guid" /> referencing the protected branch in the Concurrent Server.
        /// </summary>
        Guid EngineeringBranchId { get; }

        /// <summary>
        /// The set of <see cref="ProjectMemberRole" /> values permitted to merge into this protected branch.
        /// </summary>
        List<ProjectMemberRole> MergeAllowedFor { get; }

        /// <summary>
        /// Gets or sets the number of approval(s) required before being allowed to merge a Branch.
        /// </summary>
        int MinimumRequiredApproval { get; }

        /// <summary>
        /// Gets or sets the name of the current rule.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Indicates whether at least one approved <see cref="Review" /> is required before a merge can proceed.
        /// </summary>
        bool ReviewRequired { get; }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
