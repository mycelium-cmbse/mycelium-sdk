// ------------------------------------------------------------------------------------------------
//  <copyright file="ProjectInvitation.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.POCO
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;

    /// <summary>
    /// An <see cref="IInvitation" /> to join a <see cref="FunctionalProject" /> with a
    /// <see cref="ProjectMemberRole" /> and, optionally, a set of <see cref="Ownership" /> domains.
    /// Accepting it creates the corresponding <see cref="ProjectMember" />.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public partial class ProjectInvitation : IProjectInvitation
    {
        /// <summary>
        /// Represents the unique identifier that allow entity identification.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// References the <see cref="User" /> that created the current <see cref="IAuditableThing" />.
        /// </summary>
        public IUser CreatedBy { get; set; }

        /// <summary>
        /// Provides the creation <see cref="DateTime" /> of the current <see cref="IAuditableThing" />
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// The <see cref="DateTime" /> after which a Pending invitation can no longer be accepted, computed
        /// from the configured invitation lifetime.
        /// </summary>
        public DateTime ExpiresOn { get; set; }

        /// <summary>
        /// References the invited <see cref="User" /> when the invitee already holds an account.
        /// </summary>
        public IUser Invitee { get; set; }

        /// <summary>
        /// The email address the invitation was sent to when the invitee is not yet identified as a
        /// <see cref="User" />; matched against a user's verified email at first sign-in.
        /// </summary>
        public string InviteeEmail { get; set; }

        /// <summary>
        /// References the <see cref="Ownership" /> domains assigned to the invitee on acceptance.
        /// </summary>
        public List<IOwnership> Ownerships { get; set; } = [];

        /// <summary>
        /// References the <see cref="FunctionalProject" /> the invitee is invited to join.
        /// </summary>
        public IFunctionalProject Project { get; set; }

        /// <summary>
        /// The <see cref="DateTime" /> at which the invitation was accepted, declined or revoked.
        /// </summary>
        public DateTime? RespondedOn { get; set; }

        /// <summary>
        /// The <see cref="ProjectMemberRole" /> granted to the invitee on acceptance.
        /// </summary>
        public ProjectMemberRole Role { get; set; }

        /// <summary>
        /// The current <see cref="InvitationStatus" />, changed only by the accept, decline and revoke
        /// operations.
        /// </summary>
        public InvitationStatus Status { get; set; }

        /// <summary>
        /// References the <see cref="User" /> that provide the last update on the current
        /// <see cref="IAuditableThing" />.
        /// </summary>
        public IUser UpdatedBy { get; set; }

        /// <summary>
        /// Provides the last modification <see cref="DateTime" /> of the current <see cref="IAuditableThing" />
        /// </summary>
        public DateTime UpdatedOn { get; set; }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
