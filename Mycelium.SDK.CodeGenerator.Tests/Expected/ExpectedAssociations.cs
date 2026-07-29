// ------------------------------------------------------------------------------------------------
//  <copyright file="ExpectedAssociations.cs" company="Starion Group S.A.">
// 
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
// 
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Tests.Expected
{
    using System.Collections;

    /// <summary>
    /// Provides the expected UML association signatures.
    /// </summary>
    public class ExpectedAssociations : IEnumerable<string>
    {
        /// <summary>
        /// Returns the expected semantic UML association signatures.
        /// </summary>
        /// <returns>
        /// The expected semantic UML association signatures.
        /// </returns>
        public IEnumerator<string> GetEnumerator()
        {
            yield return CreateSignature(
                "AuditableThing", null,
                "User", "updatedBy");

            yield return CreateSignature(
                "AuditableThing", null,
                "User", "createdBy");

            yield return CreateSignature(
                "BranchProtectionRule", null,
                "User", "defaultReviewers");

            yield return CreateSignature(
                "FunctionalProject", null,
                "BranchProtectionRule", "branchRules");

            yield return CreateSignature(
                "Comment", null,
                "User", "author");

            yield return CreateSignature(
                "Comment", null,
                "Comment", "quotes");
            
            yield return CreateSignature(
                "Comment", null,
                "Comment", "replies");

            yield return CreateSignature(
                "Review", null,
                "Comment", "comments");

            yield return CreateSignature(
                "Organization", "belongsTo",
                "FunctionalProject", "projects");

            yield return CreateSignature(
                "FunctionalProject", null,
                "Ownership", "defines");

            yield return CreateSignature(
                "FunctionalProject", "isPartOf",
                "ProjectMember", "involves");

            yield return CreateSignature(
                "FunctionalProject", null,
                "FunctionalProjectPolicy", "policy");

            yield return CreateSignature(
                "FunctionalProject", null,
                "Review", "reviews");

            yield return CreateSignature(
                "Organization", "organization",
                "OrganizationMember", "involvedUser");

            yield return CreateSignature(
                "Organization", null,
                "OrganizationPolicy", "policy");

            yield return CreateSignature(
                "OrganizationMember", "isPartOfOrganizations",
                "User", "user");

            yield return CreateSignature(
                "Ownership", "owns",
                "ProjectMember", null);

            yield return CreateSignature(
                "Ownership", "activeOwnership",
                "ProjectMember", null);

            yield return CreateSignature(
                "ProjectMember", "isPartOfProjects",
                "User", "user");

            yield return CreateSignature(
                "Review", null,
                "User", "reviewers");

            yield return CreateSignature(
                "Review", null,
                "User", "author");
        }

        /// <summary>
        /// Creates an order-independent semantic association signature.
        /// </summary>
        public static string CreateSignature(
            string firstType,
            string? firstRole,
            string secondType,
            string? secondRole)
        {
            var firstEnd = $"{firstType}:{firstRole ?? string.Empty}";
            var secondEnd = $"{secondType}:{secondRole ?? string.Empty}";

            return string.CompareOrdinal(firstEnd, secondEnd) <= 0
                ? $"{firstEnd}|{secondEnd}"
                : $"{secondEnd}|{firstEnd}";
        }
        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
