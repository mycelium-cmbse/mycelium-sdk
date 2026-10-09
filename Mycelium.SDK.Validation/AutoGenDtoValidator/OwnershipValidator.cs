// ------------------------------------------------------------------------------------------------
//  <copyright file="OwnershipValidator.cs" company="Starion Group S.A.">
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
    /// Validates local data for the <see cref="Mycelium.SDK.DTO.Ownership" /> DTO.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public partial class OwnershipValidator : AbstractValidator<Mycelium.SDK.DTO.Ownership>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OwnershipValidator" /> class
        /// and registers its model-derived data-validation rules.
        /// </summary>
        public OwnershipValidator()
        {
            this.RuleFor(dto => dto.CreatedBy).NotEmpty();
            this.RuleFor(dto => dto.Description).Must(value => !string.IsNullOrWhiteSpace(value));
            this.RuleFor(dto => dto.Name).Must(value => !string.IsNullOrWhiteSpace(value));
            this.RuleFor(dto => dto.ShortName).Must(value => !string.IsNullOrWhiteSpace(value));
            this.RuleFor(dto => dto.UpdatedBy).NotEmpty();
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
