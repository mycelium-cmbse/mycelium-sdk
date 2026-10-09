// ------------------------------------------------------------------------------------------------
//  <copyright file="ProjectMemberValidator.cs" company="Starion Group S.A.">
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
    /// Validates local data for the <see cref="Mycelium.SDK.DTO.ProjectMember" /> DTO.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public partial class ProjectMemberValidator : AbstractValidator<Mycelium.SDK.DTO.ProjectMember>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectMemberValidator" /> class
        /// and registers its model-derived data-validation rules.
        /// </summary>
        public ProjectMemberValidator()
        {
            this.RuleFor(dto => dto.ActiveOwnership).Must(value => !value.HasValue || value.Value != Guid.Empty);
            this.RuleFor(dto => dto.CreatedBy).NotEmpty();
            this.RuleFor(dto => dto.IsPartOf).NotEmpty();
            this.RuleForEach(dto => dto.Owns).NotEmpty();
            this.RuleFor(dto => dto.UpdatedBy).NotEmpty();
            this.RuleFor(dto => dto.User).NotEmpty();
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
