// ------------------------------------------------------------------------------------------------
//  <copyright file="InvitationStatus.cs" company="Starion Group S.A.">
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

    using Mycelium.SDK.DTO;

    /// <summary>
    /// Defines the response state of an <see cref="IInvitation" />. Expiry is not a state: a Pending
    /// invitation whose expiresOn has passed is expired.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public enum InvitationStatus
    {
        /// <summary>
        /// Sent and awaiting the invitee's response.
        /// </summary>
        Pending,

        /// <summary>
        /// Accepted by the invitee; the corresponding membership has been created.
        /// </summary>
        Accepted,

        /// <summary>
        /// Declined by the invitee.
        /// </summary>
        Declined,

        /// <summary>
        /// Withdrawn by the sender before the invitee responded.
        /// </summary>
        Revoked,
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
