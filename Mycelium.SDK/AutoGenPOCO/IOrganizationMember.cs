// ------------------------------------------------------------------------------------------------
//  <copyright file="IOrganizationMember.cs" company="Starion Group S.A.">
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
    /// Represents the membership of a <see cref="User" /> within an <see cref="Organization" />, carrying
    /// the organization-level role.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public partial interface IOrganizationMember : IAuditableThing
    {
        /// <summary>
        /// References the <see cref="Organization" /> this <see cref="OrganizationMember" /> belongs to.
        /// </summary>
        IOrganization Organization { get; set; }

        /// <summary>
        /// The <see cref="OrganizationMembershipRole" /> assigned to the user within the organization.
        /// </summary>
        OrganizationMembershipRole Role { get; set; }

        /// <summary>
        /// References the <see cref="User" /> record.
        /// </summary>
        IUser User { get; set; }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
