// ------------------------------------------------------------------------------------------------
//  <copyright file="BranchProtectionRuleValidator.cs" company="Starion Group S.A.">
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
    /// Validates local data for objects implementing <see cref="Mycelium.SDK.DTO.IBranchProtectionRule" />.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public partial class BranchProtectionRuleValidator : AuditableThingValidator<Mycelium.SDK.DTO.IBranchProtectionRule>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BranchProtectionRuleValidator" /> class
        /// and registers its generated and handwritten data-validation rules.
        /// </summary>
        public BranchProtectionRuleValidator()
        {
            this.RuleForEach(dto => dto.DefaultReviewers).NotEmpty();
            this.RuleFor(dto => dto.MergeAllowedFor).Must(value => (value?.Count ?? 0) >= 1);
            this.RuleFor(dto => dto.Name).Must(value => !string.IsNullOrWhiteSpace(value));

            this.AddCustomValidation();
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
