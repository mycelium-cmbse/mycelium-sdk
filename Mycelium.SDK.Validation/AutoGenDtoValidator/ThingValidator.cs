// ------------------------------------------------------------------------------------------------
//  <copyright file="ThingValidator.cs" company="Starion Group S.A.">
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
    /// Validates local data for objects implementing <see cref="Mycelium.SDK.DTO.IThing" />.
    /// </summary>
    /// <typeparam name="T">
    /// The DTO interface or implementation whose data is validated.
    /// </typeparam>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public abstract partial class ThingValidator<T> : AbstractValidator<T>
        where T : Mycelium.SDK.DTO.IThing
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThingValidator{T}" /> class
        /// and registers its generated rules and the handwritten rules supplied by the validator hierarchy.
        /// </summary>
        protected ThingValidator()
        {
            this.RuleFor(dto => dto.Id).NotEmpty();
            this.AddCustomValidation();
        }

        /// <summary>
        /// Registers approved handwritten validation rules supplied by overrides in partial validator declarations.
        /// </summary>
        /// <remarks>
        /// The default implementation registers no rules. The root validator constructor invokes this method once
        /// after registering its own generated rules and before derived constructor bodies run. Overrides call the base
        /// implementation before adding rules and must not depend on state initialized by a more-derived constructor body.
        /// </remarks>
        protected virtual void AddCustomValidation()
        {
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
