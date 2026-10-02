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

    public sealed class RepresentativeClasses : IEnumerable<string>
    {
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

        public IEnumerator<string> GetEnumerator() => ((IEnumerable<string>)Names).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
