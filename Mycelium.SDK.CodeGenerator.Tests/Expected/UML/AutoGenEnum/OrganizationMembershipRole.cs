// ------------------------------------------------------------------------------------------------
//  <copyright file="OrganizationMembershipRole.cs" company="Starion Group S.A.">
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
    /// Defines the role a <see cref="User" /> holds within an <see cref="Organization" />.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public enum OrganizationMembershipRole
    {
        /// <summary>
        /// Full administrative control over the organization, its members, projects, and authentication settings.
        /// </summary>
        Administrator,

        /// <summary>
        /// Regular organization member. Can create projects if permitted by the organization policy.
        /// </summary>
        Member,

        /// <summary>
        /// Defines the owner of an Organization.
        /// </summary>
        Owner,
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
