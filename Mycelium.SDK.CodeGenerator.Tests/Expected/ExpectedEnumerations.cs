// ------------------------------------------------------------------------------------------------
//  <copyright file="ExpectedEnumerations.cs" company="Starion Group S.A.">
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
    /// Provides the expected UML enumeration names.
    /// </summary>
    public class ExpectedEnumerations : IEnumerable<string>
    {
        /// <summary>
        /// Returns the expected UML enumeration names.
        /// </summary>
        /// <returns>
        /// The expected UML enumeration names.
        /// </returns>
        public IEnumerator<string> GetEnumerator()
        {
            yield return "ActivationStatus";
            yield return "CommentStatus";
            yield return "OrganizationMembershipRole";
            yield return "ProjectLifecyleKind";
            yield return "ProjectMemberRole";
            yield return "ProjectMode";
            yield return "ProjectVisibility";
            yield return "ReviewStatus";
        }

        /// <summary>
        /// Returns the expected UML enumeration names.
        /// </summary>
        /// <returns>
        /// The expected UML enumeration names.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
