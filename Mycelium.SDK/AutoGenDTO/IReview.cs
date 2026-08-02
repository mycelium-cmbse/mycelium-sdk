// ------------------------------------------------------------------------------------------------
//  <copyright file="IReview.cs" company="Starion Group S.A.">
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
    /// Represents a merge proposal from a source branch into a protected target branch, tracking review
    /// status, designated reviewers, and associated comments.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public partial interface IReview : IAuditableThing
    {
        /// <summary>
        /// References the <see cref="ProjectMember" /> who created this <see cref="Review" />.
        /// </summary>
        Guid Author { get; set; }

        /// <summary>
        /// Contains the <see cref="Comment" /> instances posted on this <see cref="Review" />.
        /// </summary>
        List<Guid> Comments { get; set; }

        /// <summary>
        /// Gets or sets the description of the current Review.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// References the <see cref="ProjectMember" /> instances designated as reviewers for this
        /// <see cref="Review" />.
        /// </summary>
        List<Guid> Reviewers { get; set; }

        /// <summary>
        /// The <see cref="Guid" /> referencing the source branch in the Concurrent Server whose changes are
        /// proposed for merge.
        /// </summary>
        Guid SourceBranchId { get; set; }

        /// <summary>
        /// The current <see cref="ReviewStatus" /> of this review, tracking its progression from Draft to
        /// Closed.
        /// </summary>
        ReviewStatus Status { get; set; }

        /// <summary>
        /// The <see cref="Guid" /> referencing the protected target branch in the Concurrent Server into which
        /// the merge is proposed.
        /// </summary>
        Guid TargetBranchId { get; set; }

        /// <summary>
        /// Gets or sets the given title of the current review.
        /// </summary>
        string Title { get; set; }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
