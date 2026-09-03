// ------------------------------------------------------------------------------------------------
//  <copyright file="RepresentativeClasses.cs" company="Starion Group S.A.">
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
    /// Provides the representative UML class selection used by class-based golden tests.
    /// </summary>
    public sealed class RepresentativeClasses : IEnumerable<string>
    {
        /// <summary>
        /// The class names from the current INTERESTING CLASSES section of the model-inspector report.
        /// </summary>
        private static readonly string[] Names =
        [
            "AuditableThing",
            "BranchProtectionRule",
            "Comment",
            "FunctionalProject",
            "FunctionalProjectPolicy",
            "Organization",
            "ProjectMember"
        ];

        /// <summary>
        /// Returns the representative UML class names.
        /// </summary>
        /// <returns>
        /// The representative UML class-name enumerator.
        /// </returns>
        public IEnumerator<string> GetEnumerator()
        {
            return ((IEnumerable<string>)Names).GetEnumerator();
        }

        /// <summary>
        /// Returns the representative UML class names.
        /// </summary>
        /// <returns>
        /// The representative UML class-name enumerator.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
