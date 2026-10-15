// ------------------------------------------------------------------------------------------------
//  <copyright file="FunctionalProjectValidator.cs" company="Starion Group S.A.">
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
    /// Validates local data for objects implementing <see cref="Mycelium.SDK.DTO.IFunctionalProject" />.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public partial class FunctionalProjectValidator : AuditableThingValidator<Mycelium.SDK.DTO.IFunctionalProject>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionalProjectValidator" /> class
        /// and registers its generated and handwritten data-validation rules.
        /// </summary>
        public FunctionalProjectValidator()
        {
            this.RuleFor(dto => dto.BelongsTo).NotEmpty();
            this.RuleForEach(dto => dto.BranchRules).NotEmpty();
            this.RuleForEach(dto => dto.Defines).NotEmpty();
            this.RuleFor(dto => dto.Description).Must(value => !string.IsNullOrWhiteSpace(value));
            this.RuleFor(dto => dto.EngineeringProjectId).NotEmpty();
            this.RuleForEach(dto => dto.Involves).NotEmpty();
            this.RuleFor(dto => dto.Name).Must(value => !string.IsNullOrWhiteSpace(value));
            this.RuleFor(dto => dto.Policy).NotEmpty();
            this.RuleForEach(dto => dto.Reviews).NotEmpty();
            this.RuleFor(dto => dto.SharedPreferences).NotNull();
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
