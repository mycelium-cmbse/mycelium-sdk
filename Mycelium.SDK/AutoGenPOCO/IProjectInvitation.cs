// ------------------------------------------------------------------------------------------------
//  <copyright file="IProjectInvitation.cs" company="Starion Group S.A.">
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
    /// An <see cref="IInvitation" /> to join a <see cref="FunctionalProject" /> with a
    /// <see cref="ProjectMemberRole" /> and, optionally, a set of <see cref="Ownership" /> domains.
    /// Accepting it creates the corresponding <see cref="ProjectMember" />.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public partial interface IProjectInvitation : IInvitation
    {
        /// <summary>
        /// References the <see cref="Ownership" /> domains assigned to the invitee on acceptance.
        /// </summary>
        List<IOwnership> Ownerships { get; set; }

        /// <summary>
        /// References the <see cref="FunctionalProject" /> the invitee is invited to join.
        /// </summary>
        IFunctionalProject Project { get; set; }

        /// <summary>
        /// The <see cref="ProjectMemberRole" /> granted to the invitee on acceptance.
        /// </summary>
        ProjectMemberRole Role { get; set; }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
