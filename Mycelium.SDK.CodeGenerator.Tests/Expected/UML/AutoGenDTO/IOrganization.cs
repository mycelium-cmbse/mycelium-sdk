// ------------------------------------------------------------------------------------------------
//  <copyright file="IOrganization.cs" company="Starion Group S.A.">
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
    /// Represents a tenant boundary in the Mycelium platform. On SaaS each customer maps to one Organization;
    /// on-premise deployments may host multiple.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public partial interface IOrganization : IAuditableThing
    {
        /// <summary>
        /// A short human-readable description of the organization.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// References the <see cref="OrganizationMembe" /> record.
        /// </summary>
        List<Guid> involvedUser { get; }

        /// <summary>
        /// The display name of the organization.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The <see cref="OrgnaizationPolicy" /> instance owned by this <see cref="Organization" />.
        /// </summary>
        Guid policy { get; }

        /// <summary>
        /// Contains the <see cref="FunctionalProject" /> instances belonging to this <see cref="Organization" />.
        /// </summary>
        List<Guid> projects { get; }

        /// <summary>
        /// The current <see cref="ActivationStatus" /> of the organization, controlling platform access for all its members.
        /// </summary>
        ActivationStatus Status { get; }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------