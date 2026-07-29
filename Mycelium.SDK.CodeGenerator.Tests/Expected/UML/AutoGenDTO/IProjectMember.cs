// ------------------------------------------------------------------------------------------------
//  <copyright file="IProjectMember.cs" company="Starion Group S.A.">
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
    /// Represents the membership of a <see cref="User" /> within a <see cref="FunctionalProject" />, carrying the
    /// project-level role and optional ownership assignment.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public partial interface IProjectMember : IAuditableThing
    {
        /// <summary>
        /// References the currently active <see cref="Ownership" /> for this <see cref="ProjectMember" /> when assigned
        /// to multiple ownership domains.
        /// </summary>
        Guid? activeOwnership { get; }

        /// <summary>
        /// Asserts that the current <see cref="ProjectMember" /> is part of an external
        /// <see cref="Organization" /> than the related <see cref="FunctionalProject" /> owner.
        /// </summary>
        bool isOutsideCollaborator { get; }

        /// <summary>
        /// References the <see cref="FunctionalProject" /> this <see cref="ProjectMember" /> belongs to.
        /// </summary>
        Guid isPartOf { get; }

        /// <summary>
        /// References all <see cref="Ownership" /> domains assigned to this <see cref="ProjectMember" />.
        /// </summary>
        List<Guid> owns { get; }

        /// <summary>
        /// The <see cref="ProjectMemberRole" /> assigned to the user within the project, determining their editing
        /// and access permissions.
        /// </summary>
        ProjectMemberRole role { get; }

        /// <summary>
        /// References the <see cref="User" /> record.
        /// </summary>
        Guid user { get; }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
