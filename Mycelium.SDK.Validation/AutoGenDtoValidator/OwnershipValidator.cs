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
    /// Validates local data for objects implementing <see cref="Mycelium.SDK.DTO.IOwnership" />.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public partial class OwnershipValidator : AuditableThingValidator<Mycelium.SDK.DTO.IOwnership>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OwnershipValidator" /> class
        /// and registers its generated and handwritten data-validation rules.
        /// </summary>
        public OwnershipValidator()
        {
            this.RuleFor(dto => dto.Description).Must(value => !string.IsNullOrWhiteSpace(value));
            this.RuleFor(dto => dto.EngineeringMetadataId).NotEmpty();
            this.RuleFor(dto => dto.Name).Must(value => !string.IsNullOrWhiteSpace(value));
            this.RuleFor(dto => dto.ShortName).Must(value => !string.IsNullOrWhiteSpace(value));
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
