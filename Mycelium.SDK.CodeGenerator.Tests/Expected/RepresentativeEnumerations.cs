// ------------------------------------------------------------------------------------------------
//  <copyright file="RepresentativeEnumerations.cs" company="Starion Group S.A.">
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
    /// Supplies the enumeration names selected for representative generated-source golden comparisons.
    /// The selection covers distinct identifier and documentation cases rather than the full model inventory.
    /// </summary>
    public sealed class RepresentativeEnumerations : IEnumerable<string>
    {
        /// <summary>
        /// The selected enumeration names in test-case enumeration order.
        /// </summary>
        private static readonly string[] Names =
        [
            // Plain enumeration and literal-documentation baseline without documentation references.
            "CommentStatus",

            // Multi-word literal identifier and enumeration-level documentation reference.
            "ReviewStatus",

            // Wrapped documentation with enumeration-level and literal-level documentation references.
            "ProjectMemberRole"
        ];

        /// <summary>
        /// Returns an enumerator over the selected enumeration names.
        /// </summary>
        /// <returns>
        /// An enumerator over the enumeration names used for representative golden comparisons.
        /// </returns>
        public IEnumerator<string> GetEnumerator() => ((IEnumerable<string>)Names).GetEnumerator();

        /// <summary>
        /// Returns an enumerator over the selected enumeration names.
        /// </summary>
        /// <returns>
        /// An enumerator over the enumeration names used for representative golden comparisons.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
