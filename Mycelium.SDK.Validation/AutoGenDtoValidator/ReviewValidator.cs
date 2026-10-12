// ------------------------------------------------------------------------------------------------
//  <copyright file="ReviewValidator.cs" company="Starion Group S.A.">
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
    /// Validates local data for objects implementing <see cref="Mycelium.SDK.DTO.IReview" />.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public partial class ReviewValidator : AuditableThingValidator<Mycelium.SDK.DTO.IReview>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReviewValidator" /> class
        /// and registers its generated and handwritten data-validation rules.
        /// </summary>
        public ReviewValidator()
        {
            this.RuleFor(dto => dto.Author).NotEmpty();
            this.RuleForEach(dto => dto.Comments).NotEmpty();
            this.RuleFor(dto => dto.Description).Must(value => !string.IsNullOrWhiteSpace(value));
            this.RuleForEach(dto => dto.Reviewers).NotEmpty();
            this.RuleFor(dto => dto.Title).Must(value => !string.IsNullOrWhiteSpace(value));

            this.AddCustomValidation();
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
