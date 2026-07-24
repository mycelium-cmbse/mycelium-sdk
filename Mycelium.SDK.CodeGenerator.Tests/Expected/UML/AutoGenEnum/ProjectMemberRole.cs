// ------------------------------------------------------------------------------------------------
//  <copyright file="ProjectMemberRole.cs" company="Starion Group S.A.">
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
    /// Defines the role a <see cref="User" /> holds within a <see cref="FunctionalProject" />, governing editing
    /// permissions and ownership enforcement.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public enum ProjectMemberRole
    {
        /// <summary>
        /// Full control over the project: team, branches, lifecycle state, ownership assignments, and merges.
        /// </summary>
        Administrator,

        /// <summary>
        /// Subject matter specialist who creates and modifies model elements within their assigned
        /// <see cref="Ownership" />.
        /// </summary>
        Participant,

        /// <summary>
        /// Read-only observer. Cannot create, modify, or delete any model element.
        /// </summary>
        Viewer,
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
