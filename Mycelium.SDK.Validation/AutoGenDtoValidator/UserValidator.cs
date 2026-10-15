// ------------------------------------------------------------------------------------------------
//  <copyright file="UserValidator.cs" company="Starion Group S.A.">
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
    /// Validates local data for objects implementing <see cref="Mycelium.SDK.DTO.IUser" />.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public partial class UserValidator : AuditableThingValidator<Mycelium.SDK.DTO.IUser>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserValidator" /> class
        /// and registers its generated and handwritten data-validation rules.
        /// </summary>
        public UserValidator()
        {
            this.RuleFor(dto => dto.ExternalIdentifier).Must(value => !string.IsNullOrWhiteSpace(value));
            this.RuleFor(dto => dto.IsPartOfOrganizations).Must(value => (value?.Count ?? 0) >= 1);
            this.RuleForEach(dto => dto.IsPartOfOrganizations).NotEmpty();
            this.RuleForEach(dto => dto.IsPartOfProjects).NotEmpty();
            this.RuleFor(dto => dto.Mail).Must(value => !string.IsNullOrWhiteSpace(value));
            this.RuleFor(dto => dto.Name).Must(value => !string.IsNullOrWhiteSpace(value));
            this.RuleFor(dto => dto.UserPreferences).NotNull();
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
