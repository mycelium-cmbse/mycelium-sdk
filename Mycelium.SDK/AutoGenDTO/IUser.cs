// ------------------------------------------------------------------------------------------------
//  <copyright file="IUser.cs" company="Starion Group S.A.">
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
    /// A lightweight mirror of an identity managed by Keycloak. Holds only the data required by the
    /// Functional Server for permission resolution and UI display.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public partial interface IUser : IAuditableThing
    {
        /// <summary>
        /// The Keycloak user identifier used as the permanent join key between the Functional Server and the
        /// identity provider.
        /// </summary>
        string ExternalIdentifier { get; set; }

        /// <summary>
        /// References the <see cref="OrganizationMember" /> records linking this <see cref="User" /> to their
        /// organizations.
        /// </summary>
        List<Guid> IsPartOfOrganizations { get; set; }

        /// <summary>
        /// References the <see cref="ProjectMember" /> records linking this <see cref="User" /> to their
        /// projects.
        /// </summary>
        List<Guid> IsPartOfProjects { get; set; }

        /// <summary>
        /// The email address of the user, cached from Keycloak for display purposes.
        /// </summary>
        string Mail { get; set; }

        /// <summary>
        /// The display name of the user, cached from Keycloak for UI rendering.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The current <see cref="ActivationStatus" /> of the user. Set to Deleted when Keycloak publishes a
        /// user removal event, preserving referential integrity before cascade cleanup.
        /// </summary>
        ActivationStatus Status { get; set; }

        /// <summary>
        /// Collects all preferences of the current <see cref="User" /> to create custom views.
        /// </summary>
        Dictionary<string, string> UserPreferences { get; set; }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
