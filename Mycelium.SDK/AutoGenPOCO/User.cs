// ------------------------------------------------------------------------------------------------
//  <copyright file="User.cs" company="Starion Group S.A.">
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
    /// A lightweight mirror of an identity managed by Keycloak. Holds only the data required by the
    /// Functional Server for permission resolution and UI display.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public partial class User : IUser
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
        /// The Keycloak user identifier used as the permanent join key between the Functional Server and the
        /// identity provider.
        /// </summary>
        public string ExternalIdentifier { get; set; }

        /// <summary>
        /// References the <see cref="OrganizationMember" /> records linking this <see cref="User" /> to their
        /// organizations.
        /// </summary>
        public List<IOrganizationMember> IsPartOfOrganizations { get; set; } = [];

        /// <summary>
        /// References the <see cref="ProjectMember" /> records linking this <see cref="User" /> to their
        /// projects.
        /// </summary>
        public List<IProjectMember> IsPartOfProjects { get; set; } = [];

        /// <summary>
        /// The email address of the user, cached from Keycloak for display purposes.
        /// </summary>
        public string Mail { get; set; }

        /// <summary>
        /// The display name of the user, cached from Keycloak for UI rendering.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The current <see cref="ActivationStatus" /> of the user. Set to Deleted when Keycloak publishes a
        /// user removal event, preserving referential integrity before cascade cleanup.
        /// </summary>
        public ActivationStatus Status { get; set; }

        /// <summary>
        /// References the <see cref="User" /> that provide the last update on the current
        /// <see cref="IAuditableThing" />.
        /// </summary>
        public IUser UpdatedBy { get; set; }

        /// <summary>
        /// Provides the last modification <see cref="DateTime" /> of the current <see cref="IAuditableThing" />
        /// </summary>
        public DateTime UpdatedOn { get; set; }

        /// <summary>
        /// Collects all preferences of the current <see cref="User" /> to create custom views.
        /// </summary>
        public Dictionary<string, string> UserPreferences { get; set; } = [];
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
