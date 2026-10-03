// ------------------------------------------------------------------------------------------------
//  <copyright file="ProjectMember.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.POCO
{
    using System;

    /// <summary>
    /// Provides handwritten behavior for a project membership.
    /// </summary>
    public partial class ProjectMember
    {
        /// <summary>
        /// Determines whether the associated user is outside the organization
        /// owning the associated functional project.
        /// </summary>
        /// <returns>
        /// <see langword="true" /> when the user has no membership in the
        /// project-owning organization; otherwise, <see langword="false" />.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the object graph lacks the required user, project,
        /// owning organization, or organization-membership collection, or when a membership
        /// entry or its required organization is null.
        /// </exception>
        private bool ComputeIsOutsideCollaborator()
        {
            var user = this.User ?? throw new InvalidOperationException("A project member must reference a user.");

            var project = this.IsPartOf ?? throw new InvalidOperationException("A project member must reference a functional project.");

            var owningOrganization = project.BelongsTo ?? throw new InvalidOperationException("A functional project must reference its owning organization.");

            var organizationMemberships = user.IsPartOfOrganizations ?? throw new InvalidOperationException("A user must provide its organization memberships.");

            var hasMatchingMembership = false;

            foreach (var membership in organizationMemberships)
            {
                if (membership == null)
                {
                    throw new InvalidOperationException("An organization-membership entry must not be null.");
                }

                var organization = membership.Organization ?? throw new InvalidOperationException("An organization membership must reference an organization.");

                hasMatchingMembership |= organization.Id == owningOrganization.Id;
            }

            return !hasMatchingMembership;
        }
    }
}
