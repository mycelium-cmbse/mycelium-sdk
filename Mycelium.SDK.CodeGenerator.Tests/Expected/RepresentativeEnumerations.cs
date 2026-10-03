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

    public sealed class RepresentativeEnumerations : IEnumerable<string>
    {
        private static readonly string[] Names =
        [
            // Plain enumeration and literal-documentation baseline without documentation references.
            "CommentStatus",

            // Multi-word literal identifier and enumeration-level documentation reference.
            "ReviewStatus",

            // Wrapped documentation with enumeration-level and literal-level documentation references.
            "ProjectMemberRole"
        ];

        public IEnumerator<string> GetEnumerator() => ((IEnumerable<string>)Names).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
