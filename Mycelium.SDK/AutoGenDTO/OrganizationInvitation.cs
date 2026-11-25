// ------------------------------------------------------------------------------------------------
//  <copyright file="OrganizationInvitation.cs" company="Starion Group S.A.">
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
    /// An <see cref="IInvitation" /> to join an <see cref="Organization" /> with an
    /// <see cref="OrganizationMembershipRole" />. Accepting it creates the corresponding
    /// <see cref="OrganizationMember" />.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public partial class OrganizationInvitation : IOrganizationInvitation
    {
        /// <summary>
        /// Represents the unique identifier that allow entity identification.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// References the <see cref="User" /> that created the current <see cref="IAuditableThing" />.
        /// </summary>
        public Guid CreatedBy { get; set; }

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
        public Guid? Invitee { get; set; }

        /// <summary>
        /// The email address the invitation was sent to when the invitee is not yet identified as a
        /// <see cref="User" />; matched against a user's verified email at first sign-in.
        /// </summary>
        public string InviteeEmail { get; set; }

        /// <summary>
        /// References the <see cref="Organization" /> the invitee is invited to join.
        /// </summary>
        public Guid Organization { get; set; }

        /// <summary>
        /// The <see cref="DateTime" /> at which the invitation was accepted, declined or revoked.
        /// </summary>
        public DateTime? RespondedOn { get; set; }

        /// <summary>
        /// The <see cref="OrganizationMembershipRole" /> granted to the invitee on acceptance.
        /// </summary>
        public OrganizationMembershipRole Role { get; set; }

        /// <summary>
        /// The current <see cref="InvitationStatus" />, changed only by the accept, decline and revoke
        /// operations.
        /// </summary>
        public InvitationStatus Status { get; set; }

        /// <summary>
        /// References the <see cref="User" /> that provide the last update on the current
        /// <see cref="IAuditableThing" />.
        /// </summary>
        public Guid UpdatedBy { get; set; }

        /// <summary>
        /// Provides the last modification <see cref="DateTime" /> of the current <see cref="IAuditableThing" />
        /// </summary>
        public DateTime UpdatedOn { get; set; }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
