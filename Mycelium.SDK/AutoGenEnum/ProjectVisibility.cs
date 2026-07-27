// ------------------------------------------------------------------------------------------------
//  <copyright file="ProjectVisibility.cs" company="Starion Group S.A.">
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
    /// Controls who can discover and access a <see cref="FunctionalProject" />.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public enum ProjectVisibility
    {
        /// <summary>
        /// Visible only to explicitly assigned project members.
        /// </summary>
        Private,

        /// <summary>
        /// Visible to all members of the owning organization.
        /// </summary>
        Organization,

        /// <summary>
        /// Visible to all authenticated users across the installation.
        /// </summary>
        Public,
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
