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
        /// and registers the generated data-validation rules declared by this model class.
        /// </summary>
        protected ThingValidator()
        {
        }

        /// <summary>
        /// Registers approved handwritten validation rules supplied by overrides in partial validator declarations.
        /// </summary>
        /// <remarks>
        /// The default implementation registers no rules. Concrete validator constructors invoke this method
        /// once after registering all generated rules. Overrides call the base implementation before adding rules
        /// and must not depend on state initialized by a more-derived constructor.
        /// </remarks>
        protected virtual void AddCustomValidation()
        {
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
