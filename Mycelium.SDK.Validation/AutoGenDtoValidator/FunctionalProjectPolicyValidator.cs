// ------------------------------------------------------------------------------------------------
//  <copyright file="FunctionalProjectPolicyValidator.cs" company="Starion Group S.A.">
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
    /// Validates local data for the <see cref="Mycelium.SDK.DTO.FunctionalProjectPolicy" /> DTO.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public partial class FunctionalProjectPolicyValidator : AbstractValidator<Mycelium.SDK.DTO.FunctionalProjectPolicy>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionalProjectPolicyValidator" /> class
        /// and registers its model-derived data-validation rules.
        /// </summary>
        public FunctionalProjectPolicyValidator()
        {
            this.RuleFor(dto => dto.CreatedBy).NotEmpty();
            this.RuleFor(dto => dto.UpdatedBy).NotEmpty();
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
