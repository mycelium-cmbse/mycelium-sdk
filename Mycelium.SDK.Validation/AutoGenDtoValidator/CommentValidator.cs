// ------------------------------------------------------------------------------------------------
//  <copyright file="CommentValidator.cs" company="Starion Group S.A.">
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
    /// Validates local data for objects implementing <see cref="Mycelium.SDK.DTO.IComment" />.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public partial class CommentValidator : AuditableThingValidator<Mycelium.SDK.DTO.IComment>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommentValidator" /> class
        /// and registers its generated and handwritten data-validation rules.
        /// </summary>
        public CommentValidator()
        {
            this.RuleFor(dto => dto.Author).NotEmpty();
            this.RuleFor(dto => dto.Content).Must(value => !string.IsNullOrWhiteSpace(value));
            this.RuleFor(dto => dto.Quotes).Must(value => !value.HasValue || value.Value != Guid.Empty);
            this.RuleForEach(dto => dto.Replies).NotEmpty();
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
