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
    using System.Collections.Generic;

    /// <summary>
    /// Provides the bounded representative enumeration selection used by golden tests.
    /// </summary>
    public sealed class RepresentativeEnumerations : IEnumerable<string>
    {
        /// <summary>
        /// The representative enumeration names and their distinct coverage reasons.
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
        /// Returns the representative enumeration names.
        /// </summary>
        /// <returns>
        /// The representative enumeration-name enumerator.
        /// </returns>
        public IEnumerator<string> GetEnumerator()
        {
            return ((IEnumerable<string>)Names).GetEnumerator();
        }

        /// <summary>
        /// Returns the representative enumeration names.
        /// </summary>
        /// <returns>
        /// The representative enumeration-name enumerator.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
