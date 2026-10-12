// ------------------------------------------------------------------------------------------------
//  <copyright file="ProjectMemberValidator.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.Validation
{
    using FluentValidation;

    /// <summary>
    /// Provides handwritten domain validation for objects implementing <see cref="Mycelium.SDK.DTO.IProjectMember" />.
    /// </summary>
    public partial class ProjectMemberValidator
    {
        /// <summary>
        /// Registers inherited handwritten rules and the <c>ProjectMember.partOfOwns</c> constraint
        /// requiring a supplied active ownership to belong to the member's ownerships.
        /// </summary>
        protected override void AddCustomValidation()
        {
            base.AddCustomValidation();

            this.RuleFor(dto => dto.ActiveOwnership).Must((dto, activeOwnership) => !activeOwnership.HasValue || (dto.Owns != null && dto.Owns.Contains(activeOwnership.Value)));
        }
    }
}
