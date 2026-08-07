// ------------------------------------------------------------------------------------------------
//  <copyright file="ProjectMember.cs" company="Starion Group S.A.">
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
    /// Represents the membership of a <see cref="User" /> within a <see cref="FunctionalProject" />,
    /// carrying the project-level role and optional ownership assignment.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public partial class ProjectMember : IProjectMember
    {
        /// <summary>
        /// Represents the unique identifier that allow entity identification.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// References the currently active <see cref="Ownership" /> for this <see cref="ProjectMember" /> when
        /// assigned to multiple ownership domains.
        /// </summary>
        public Guid? ActiveOwnership { get; set; }

        /// <summary>
        /// References the <see cref="User" /> that created the current <see cref="AuditableThing" />.
        /// </summary>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// Provides the creation <see cref="DateTime" /> of the current <see cref="AuditableThing" />
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Asserts that the current <see cref="ProjectMember" /> is part of an external
        /// <see cref="Organization" /> than the related <see cref="FunctionalProject" /> owner.
        /// </summary>
        public bool IsOutsideCollaborator { get; }

        /// <summary>
        /// References the <see cref="FunctionalProject" /> this <see cref="ProjectMember" /> belongs to.
        /// </summary>
        public Guid IsPartOf { get; set; }

        /// <summary>
        /// References all <see cref="Ownership" /> domains assigned to this <see cref="ProjectMember" />.
        /// </summary>
        public List<Guid> Owns { get; set; } = [];

        /// <summary>
        /// The <see cref="ProjectMemberRole" /> assigned to the user within the project, determining their
        /// editing and access permissions.
        /// </summary>
        public ProjectMemberRole Role { get; set; }

        /// <summary>
        /// References the <see cref="User" /> that provide the last update on the current
        /// <see cref="AuditableThing" />.
        /// </summary>
        public Guid UpdatedBy { get; set; }

        /// <summary>
        /// Provides the last modification <see cref="DateTime" /> of the current <see cref="AuditableThing" />
        /// </summary>
        public DateTime UpdatedOn { get; set; }

        /// <summary>
        /// References the <see cref="User" /> record.
        /// </summary>
        public Guid User { get; set; }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
