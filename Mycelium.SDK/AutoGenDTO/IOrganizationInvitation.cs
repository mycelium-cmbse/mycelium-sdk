// ------------------------------------------------------------------------------------------------
//  <copyright file="IOrganizationInvitation.cs" company="Starion Group S.A.">
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
    public partial interface IOrganizationInvitation : IInvitation
    {
        /// <summary>
        /// References the <see cref="Organization" /> the invitee is invited to join.
        /// </summary>
        Guid Organization { get; set; }

        /// <summary>
        /// The <see cref="OrganizationMembershipRole" /> granted to the invitee on acceptance.
        /// </summary>
        OrganizationMembershipRole Role { get; set; }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
