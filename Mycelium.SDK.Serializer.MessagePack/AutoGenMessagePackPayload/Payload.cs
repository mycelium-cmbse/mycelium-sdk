// ------------------------------------------------------------------------------------------------
//  <copyright file="Payload.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.Serializer.MessagePack
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;

    using Mycelium.SDK.DTO;

    /// <summary>
    /// Groups concrete DTOs for the positional MessagePack payload envelope.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    internal sealed partial class Payload
    {
        /// <summary>
        /// Initializes a new payload with empty concrete-type groups.
        /// </summary>
        public Payload()
        {
        }

        /// <summary>
        /// Gets or sets the UTC time at which this payload was created.
        /// </summary>
        internal DateTime Created { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets the BranchProtectionRule DTO group.
        /// </summary>
        internal List<BranchProtectionRule> BranchProtectionRule { get; } = new();

        /// <summary>
        /// Gets the Comment DTO group.
        /// </summary>
        internal List<Comment> Comment { get; } = new();

        /// <summary>
        /// Gets the FunctionalProject DTO group.
        /// </summary>
        internal List<FunctionalProject> FunctionalProject { get; } = new();

        /// <summary>
        /// Gets the FunctionalProjectPolicy DTO group.
        /// </summary>
        internal List<FunctionalProjectPolicy> FunctionalProjectPolicy { get; } = new();

        /// <summary>
        /// Gets the Organization DTO group.
        /// </summary>
        internal List<Organization> Organization { get; } = new();

        /// <summary>
        /// Gets the OrganizationMember DTO group.
        /// </summary>
        internal List<OrganizationMember> OrganizationMember { get; } = new();

        /// <summary>
        /// Gets the OrganizationPolicy DTO group.
        /// </summary>
        internal List<OrganizationPolicy> OrganizationPolicy { get; } = new();

        /// <summary>
        /// Gets the Ownership DTO group.
        /// </summary>
        internal List<Ownership> Ownership { get; } = new();

        /// <summary>
        /// Gets the ProjectMember DTO group.
        /// </summary>
        internal List<ProjectMember> ProjectMember { get; } = new();

        /// <summary>
        /// Gets the Review DTO group.
        /// </summary>
        internal List<Review> Review { get; } = new();

        /// <summary>
        /// Gets the User DTO group.
        /// </summary>
        internal List<User> User { get; } = new();

    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
