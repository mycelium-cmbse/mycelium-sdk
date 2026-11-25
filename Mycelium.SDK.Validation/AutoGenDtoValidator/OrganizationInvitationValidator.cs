// ------------------------------------------------------------------------------------------------
//  <copyright file="OrganizationInvitationValidator.cs" company="Starion Group S.A.">
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
    /// Validates local data for objects implementing <see cref="Mycelium.SDK.DTO.IOrganizationInvitation" />.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public partial class OrganizationInvitationValidator : InvitationValidator<Mycelium.SDK.DTO.IOrganizationInvitation>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrganizationInvitationValidator" /> class
        /// and registers its generated and handwritten data-validation rules.
        /// </summary>
        public OrganizationInvitationValidator()
        {
            this.RuleFor(dto => dto.Organization).NotEmpty();
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
