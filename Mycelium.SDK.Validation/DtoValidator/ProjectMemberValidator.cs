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
    /// Provides handwritten domain validation for the <see cref="Mycelium.SDK.DTO.ProjectMember" /> DTO.
    /// </summary>
    public partial class ProjectMemberValidator
    {
        /// <summary>
        /// Registers the <c>ProjectMember.partOfOwns</c> constraint requiring a supplied active ownership to belong to the member's ownerships.
        /// </summary>
        partial void AddCustomValidation()
        {
            this.RuleFor(dto => dto.ActiveOwnership).Must((dto, activeOwnership) => !activeOwnership.HasValue || (dto.Owns != null && dto.Owns.Contains(activeOwnership.Value)));
        }
    }
}
