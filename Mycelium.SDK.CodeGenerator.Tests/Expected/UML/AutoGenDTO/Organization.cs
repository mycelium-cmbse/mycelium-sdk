// ------------------------------------------------------------------------------------------------
//  <copyright file="Organization.cs" company="Starion Group S.A.">
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
    public partial class Organization : IOrganization
    {
        /// <summary>
        /// Represents the unique identifier that allow entity identification.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// References the <see cref="User" /> that created the current <see cref="AuditableThing" />.
        /// </summary>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// Provides the creation <see cref="DateTime" /> of the current <see cref="AuditableThing" />
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// A short human-readable description of the organization.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// References the <see cref="OrganizationMember" /> record.
        /// </summary>
        public List<Guid> InvolvedUser { get; set; } = [];

        /// <summary>
        /// The display name of the organization.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The <see cref="OrganizationPolicy" /> instance owned by this <see cref="Organization" />.
        /// </summary>
        public Guid Policy { get; set; }

        /// <summary>
        /// Contains the <see cref="FunctionalProject" /> instances belonging to this
        /// <see cref="Organization" />.
        /// </summary>
        public List<Guid> Projects { get; set; } = [];

        /// <summary>
        /// The current <see cref="ActivationStatus" /> of the organization, controlling platform access for all
        /// its members.
        /// </summary>
        public ActivationStatus Status { get; set; }

        /// <summary>
        /// References the <see cref="User" /> that provide the last update on the current
        /// <see cref="AuditableThing" />.
        /// </summary>
        public Guid UpdatedBy { get; set; }

        /// <summary>
        /// Provides the last modification <see cref="DateTime" /> of the current <see cref="AuditableThing" />
        /// </summary>
        public DateTime UpdatedOn { get; set; }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
