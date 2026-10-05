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

    /// <summary>
    /// Supplies the class names selected for representative generated-source golden comparisons.
    /// The selection mirrors the INTERESTING CLASSES section of FunctionalData.ModelInspector.txt.
    /// </summary>
    public sealed class RepresentativeClasses : IEnumerable<string>
    {
        /// <summary>
        /// The selected class names in test-case enumeration order.
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
        /// Returns an enumerator over the selected class names.
        /// </summary>
        /// <returns>
        /// An enumerator over the class names used for representative golden comparisons.
        /// </returns>
        public IEnumerator<string> GetEnumerator() => ((IEnumerable<string>)Names).GetEnumerator();

        /// <summary>
        /// Returns an enumerator over the selected class names.
        /// </summary>
        /// <returns>
        /// An enumerator over the class names used for representative golden comparisons.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
