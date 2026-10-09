// ------------------------------------------------------------------------------------------------
//  <copyright file="OrganizationValidator.cs" company="Starion Group S.A.">
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
    /// Validates local data for the <see cref="Mycelium.SDK.DTO.Organization" /> DTO.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public partial class OrganizationValidator : AbstractValidator<Mycelium.SDK.DTO.Organization>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrganizationValidator" /> class
        /// and registers its model-derived data-validation rules.
        /// </summary>
        public OrganizationValidator()
        {
            this.RuleFor(dto => dto.CreatedBy).NotEmpty();
            this.RuleFor(dto => dto.Description).Must(value => !string.IsNullOrWhiteSpace(value));
            this.RuleFor(dto => dto.InvolvedUser).Must(value => (value?.Count ?? 0) >= 1);
            this.RuleForEach(dto => dto.InvolvedUser).NotEmpty();
            this.RuleFor(dto => dto.Name).Must(value => !string.IsNullOrWhiteSpace(value));
            this.RuleFor(dto => dto.Policy).NotEmpty();
            this.RuleForEach(dto => dto.Projects).NotEmpty();
            this.RuleFor(dto => dto.UpdatedBy).NotEmpty();
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
