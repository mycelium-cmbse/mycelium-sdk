// ------------------------------------------------------------------------------------------------
//  <copyright file="OrganizationMemberValidator.cs" company="Starion Group S.A.">
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
    /// Validates local data for the <see cref="Mycelium.SDK.DTO.OrganizationMember" /> DTO.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public partial class OrganizationMemberValidator : AbstractValidator<Mycelium.SDK.DTO.OrganizationMember>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrganizationMemberValidator" /> class
        /// and registers its model-derived data-validation rules.
        /// </summary>
        public OrganizationMemberValidator()
        {
            this.RuleFor(dto => dto.CreatedBy).NotEmpty();
            this.RuleFor(dto => dto.Organization).NotEmpty();
            this.RuleFor(dto => dto.UpdatedBy).NotEmpty();
            this.RuleFor(dto => dto.User).NotEmpty();
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
