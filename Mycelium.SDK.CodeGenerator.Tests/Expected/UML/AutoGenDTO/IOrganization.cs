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
    /// Represents a tenant boundary in the Mycelium platform. On SaaS each customer maps to one
    /// Organization; on-premise deployments may host multiple.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public partial interface IOrganization : IAuditableThing
    {
        /// <summary>
        /// A short human-readable description of the organization.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// References the <see cref="OrganizationMember" /> record.
        /// </summary>
        List<Guid> InvolvedUser { get; set; }

        /// <summary>
        /// The display name of the organization.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The <see cref="OrganizationPolicy" /> instance owned by this <see cref="Organization" />.
        /// </summary>
        Guid Policy { get; set; }

        /// <summary>
        /// Contains the <see cref="FunctionalProject" /> instances belonging to this
        /// <see cref="Organization" />.
        /// </summary>
        List<Guid> Projects { get; set; }

        /// <summary>
        /// The current <see cref="ActivationStatus" /> of the organization, controlling platform access for all
        /// its members.
        /// </summary>
        ActivationStatus Status { get; set; }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
