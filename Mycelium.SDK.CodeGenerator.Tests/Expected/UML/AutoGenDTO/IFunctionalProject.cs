// ------------------------------------------------------------------------------------------------
//  <copyright file="IFunctionalProject.cs" company="Starion Group S.A.">
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
    /// Represents the Functional Data Server projection of an engineering project, linking platform metadata to the
    /// corresponding SysML v2 model in the Concurrent Server.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public partial interface IFunctionalProject : IAuditableThing
    {
        /// <summary>
        /// References the <see cref="Organization" /> to which the <see cref="FunctionalProject"/> belongs to.
        /// </summary>
        Guid BelongsTo { get; set; }

        /// <summary>
        /// Contains the <see cref="BranchProtectionRule" /> instances defined for this
        /// <see cref="FunctionalProject" />.
        /// </summary>
        List<Guid> BranchRules { get; set; }

        /// <summary>
        /// The current <see cref="ProjectMode" /> (Regular or Concurrent) governing ownership enforcement for this project.
        /// </summary>
        ProjectMode CurrentMode { get; set; }

        /// <summary>
        /// Contains the <see cref="Ownership" /> domains defined within this <see cref="FunctionalProject" />.
        /// </summary>
        List<Guid> Defines { get; set; }

        /// <summary>
        /// A short human-readable description of the project purpose.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// The <see cref="Guid" /> referencing the corresponding OMG Systems Modelling API Project in the Concurrent Server.
        /// </summary>
        Guid EngineeringProjectId { get; set; }

        /// <summary>
        /// References the <see cref="ProjectMember" /> that are part of the  <see cref="FunctionalProject" /> .
        /// </summary>
        List<Guid> Involves { get; set; }

        /// <summary>
        /// The current <see cref="ProjectLifecycleKind" /> controlling editing permissions across the project.
        /// </summary>
        ProjectLifecycleKind Lifecycle { get; set; }

        /// <summary>
        /// The display name of the project.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The related <see cref="FunctionalProjectPolicy" />
        /// </summary>
        Guid Policy { get; set; }

        /// <summary>
        /// Contains the <see cref="Review" /> instances associated with this <see cref="FunctionalProject" />.
        /// </summary>
        List<Guid> Reviews { get; set; }

        /// <summary>
        /// The <see cref="ProjectVisibility" /> controlling who can discover and access this project.
        /// </summary>
        ProjectVisibility Visibility { get; set; }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
