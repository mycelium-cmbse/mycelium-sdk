// ------------------------------------------------------------------------------------------------
//  <copyright file="ExpectedClasses.cs" company="Starion Group S.A.">
// 
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
// 
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Tests.Expected
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Provides the expected abstract and concrete UML class names.
    /// </summary>
    public class ExpectedClasses : IEnumerable<string>
    {
        /// <summary>
        /// Returns the expected UML class names.
        /// </summary>
        /// <returns>
        /// The expected UML class names.
        /// </returns>
        public IEnumerator<string> GetEnumerator()
        {
            yield return "AuditableThing";
            yield return "BranchProtectionRule";
            yield return "Comment";
            yield return "FunctionalProject";
            yield return "FunctionalProjectPolicy";
            yield return "Organization";
            yield return "OrganizationMember";
            yield return "OrganizationPolicy";
            yield return "Ownership";
            yield return "ProjectMember";
            yield return "Review";
            yield return "Thing";
            yield return "User";
        }

        /// <summary>
        /// Returns the expected UML class names.
        /// </summary>
        /// <returns>
        /// The expected UML class names.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
