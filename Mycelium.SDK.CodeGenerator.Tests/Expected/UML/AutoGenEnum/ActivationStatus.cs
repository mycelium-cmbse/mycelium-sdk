// ------------------------------------------------------------------------------------------------
//  <copyright file="ActivationStatus.cs" company="Starion Group S.A.">
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
    /// Defines the lifecycle status shared by <see cref="User" /> and <see cref="Organization" /> entities,
    /// controlling platform access and visibility.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public enum ActivationStatus
    {
        /// <summary>
        /// Normal operational state. All platform features are accessible.
        /// </summary>
        Active,

        /// <summary>
        /// Created but setup not yet complete. Access is restricted until initial configuration is finalised.
        /// </summary>
        Pending,

        /// <summary>
        /// Administratively blocked. Data is preserved but all access is denied. Reactivation is possible.
        /// </summary>
        Suspended,

        /// <summary>
        /// Permanently deactivated. Read-only access for authorised roles only. No reactivation expected.
        /// </summary>
        Archived,

        /// <summary>
        /// Soft-deleted. Record retained for referential integrity within the data retention window before hard deletion.
        /// </summary>
        Deleted,
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
