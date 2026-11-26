// ------------------------------------------------------------------------------------------------
//  <copyright file="InvitationValidator.cs" company="Starion Group S.A.">
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
    /// Validates local data for objects implementing <see cref="Mycelium.SDK.DTO.IInvitation" />.
    /// </summary>
    /// <typeparam name="T">
    /// The DTO interface or implementation whose data is validated.
    /// </typeparam>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public abstract partial class InvitationValidator<T> : AuditableThingValidator<T>
        where T : Mycelium.SDK.DTO.IInvitation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvitationValidator{T}" /> class
        /// and registers the generated data-validation rules declared by this model class.
        /// </summary>
        protected InvitationValidator()
        {
            this.RuleFor(dto => dto.Invitee).Must(value => !value.HasValue || value.Value != Guid.Empty);
            this.RuleFor(dto => dto.InviteeEmail).Must(value => value == null || !string.IsNullOrWhiteSpace(value));
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
