// ------------------------------------------------------------------------------------------------
//  <copyright file="ReviewStatus.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK
{
    using System.CodeDom.Compiler;

    /// <summary>
    /// Tracks the progression of a <see cref="Review" /> through its lifecycle from creation to closure.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public enum ReviewStatus
    {
        /// <summary>
        /// Review is being prepared by the author and has not yet been submitted to reviewers.
        /// </summary>
        Draft,

        /// <summary>
        /// Submitted to designated reviewers and awaiting verdicts.
        /// </summary>
        Ready,

        /// <summary>
        /// All required reviewers have approved. The merge is permitted subject to branch protection rules.
        /// </summary>
        Approved,

        /// <summary>
        /// At least one reviewer has requested changes. The merge is blocked until the author addresses the feedback.
        /// </summary>
        RequestedChanged,

        /// <summary>
        /// Review has been closed, either by a completed merge or by dismissal without merging.
        /// </summary>
        Closed,
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------