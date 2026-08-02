// ------------------------------------------------------------------------------------------------
//  <copyright file="Review.cs" company="Starion Group S.A.">
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
    public partial class Review : IReview
    {
        /// <summary>
        /// Represents the unique identifier that allow entity identification.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// References the <see cref="ProjectMember" /> who created this <see cref="Review" />.
        /// </summary>
        public Guid Author { get; set; }

        /// <summary>
        /// Contains the <see cref="Comment" /> instances posted on this <see cref="Review" />.
        /// </summary>
        public List<Guid> Comments { get; set; } = [];

        /// <summary>
        /// References the <see cref="User" /> that created the current <see cref="AuditableThing" />.
        /// </summary>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// Provides the creation <see cref="DateTime" /> of the current <see cref="AuditableThing" />
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the description of the current Review.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// References the <see cref="ProjectMember" /> instances designated as reviewers for this
        /// <see cref="Review" />.
        /// </summary>
        public List<Guid> Reviewers { get; set; } = [];

        /// <summary>
        /// The <see cref="Guid" /> referencing the source branch in the Concurrent Server whose changes are
        /// proposed for merge.
        /// </summary>
        public Guid SourceBranchId { get; set; }

        /// <summary>
        /// The current <see cref="ReviewStatus" /> of this review, tracking its progression from Draft to
        /// Closed.
        /// </summary>
        public ReviewStatus Status { get; set; }

        /// <summary>
        /// The <see cref="Guid" /> referencing the protected target branch in the Concurrent Server into which
        /// the merge is proposed.
        /// </summary>
        public Guid TargetBranchId { get; set; }

        /// <summary>
        /// Gets or sets the given title of the current review.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// References the <see cref="User" /> that provide the last update on the current
        /// <see cref="AuditableThing" />.
        /// </summary>
        public Guid UpdatedBy { get; set; }

        /// <summary>
        /// Provides the last modification <see cref="DateTime" /> of the current <see cref="AuditableThing" />
        /// </summary>
        public DateTime UpdatedOn { get; set; }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
