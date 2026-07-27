// ------------------------------------------------------------------------------------------------
//  <copyright file="ProjectMode.cs" company="Starion Group S.A.">
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
    /// Defines the editing mode of a <see cref="FunctionalProject" />, controlling whether ownership
    /// enforcement is active.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public enum ProjectMode
    {
        /// <summary>
        /// No ownership enforcement. All participants can edit any element within the project.
        /// </summary>
        Regular,

        /// <summary>
        /// Strict ownership enforcement active. Participants can only modify elements annotated with their
        /// assigned <see cref="Ownership" />.
        /// </summary>
        Concurrent,
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
