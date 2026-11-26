// ------------------------------------------------------------------------------------------------
//  <copyright file="IInvitation.cs" company="Starion Group S.A.">
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
    /// Abstract base of an invitation sent by a <see cref="User" /> to join a target with a given role,
    /// taking effect only once the invitee accepts it. The sender is the inherited createdBy.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public partial interface IInvitation : IAuditableThing
    {
        /// <summary>
        /// The <see cref="DateTime" /> after which a Pending invitation can no longer be accepted, computed
        /// from the configured invitation lifetime.
        /// </summary>
        DateTime ExpiresOn { get; set; }

        /// <summary>
        /// References the invited <see cref="User" /> when the invitee already holds an account.
        /// </summary>
        Guid? Invitee { get; set; }

        /// <summary>
        /// The email address the invitation was sent to when the invitee is not yet identified as a
        /// <see cref="User" />; matched against a user's verified email at first sign-in.
        /// </summary>
        string InviteeEmail { get; set; }

        /// <summary>
        /// The <see cref="DateTime" /> at which the invitation was accepted, declined or revoked.
        /// </summary>
        DateTime? RespondedOn { get; set; }

        /// <summary>
        /// The current <see cref="InvitationStatus" />, changed only by the accept, decline and revoke
        /// operations.
        /// </summary>
        InvitationStatus Status { get; set; }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
