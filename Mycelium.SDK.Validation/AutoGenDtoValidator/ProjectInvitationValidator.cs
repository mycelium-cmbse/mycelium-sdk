// ------------------------------------------------------------------------------------------------
//  <copyright file="ProjectInvitationValidator.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.Validation
{
    using System;
    using System.CodeDom.Compiler;

    using FluentValidation;

    /// <summary>
    /// Validates local data for objects implementing <see cref="Mycelium.SDK.DTO.IProjectInvitation" />.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public partial class ProjectInvitationValidator : InvitationValidator<Mycelium.SDK.DTO.IProjectInvitation>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectInvitationValidator" /> class
        /// and registers its generated and handwritten data-validation rules.
        /// </summary>
        public ProjectInvitationValidator()
        {
            this.RuleForEach(dto => dto.Ownerships).NotEmpty();
            this.RuleFor(dto => dto.Project).NotEmpty();
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
