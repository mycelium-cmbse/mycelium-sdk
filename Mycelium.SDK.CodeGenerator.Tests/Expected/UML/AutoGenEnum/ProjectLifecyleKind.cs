// ------------------------------------------------------------------------------------------------
//  <copyright file="ProjectLifecyleKind.cs" company="Starion Group S.A.">
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
    /// Defines the lifecycle phase of a <see cref="FunctionalProject" />, controlling which roles may perform
    /// editing operations.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public enum ProjectLifecyleKind
    {
        /// <summary>
        /// Project is being configured. Only the Project Administrator may edit structure, team, and reference data.
        /// </summary>
        Preparation,

        /// <summary>
        /// Active modeling phase. All Participants may create and modify elements within their assigned Ownership.
        /// </summary>
        Open,

        /// <summary>
        /// Model is frozen for formal review. All roles are read-only. Reviewers may add comments.
        /// </summary>
        Review,

        /// <summary>
        /// Study completed. Model is preserved as an immutable historical record. Read-only for all roles.
        /// </summary>
        Archived,
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------