// ------------------------------------------------------------------------------------------------
//  <copyright file="MessageValidator.cs" company="Starion Group S.A.">
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
    /// Validates local data for objects implementing <see cref="Mycelium.SDK.DTO.IMessage" />.
    /// </summary>
    /// <typeparam name="T">
    /// The DTO interface or implementation whose data is validated.
    /// </typeparam>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public abstract partial class MessageValidator<T> : AuditableThingValidator<T>
        where T : Mycelium.SDK.DTO.IMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageValidator{T}" /> class
        /// and registers the generated data-validation rules declared by this model class.
        /// </summary>
        protected MessageValidator()
        {
            this.RuleFor(dto => dto.Content).Must(value => !string.IsNullOrWhiteSpace(value));
            this.RuleFor(dto => dto.Quotes).Must(value => !value.HasValue || value.Value != Guid.Empty);
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
