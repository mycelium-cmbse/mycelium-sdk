// ------------------------------------------------------------------------------------------------
//  <copyright file="ReservedCSharpNameMapper.cs" company="Starion Group S.A.">
// 
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
// 
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Extensions
{
    using System;

    using Microsoft.CodeAnalysis.CSharp;

    /// <summary>
    /// Escapes reserved C# keywords without otherwise changing modeled names.
    /// </summary>
    public static class ReservedCSharpNameMapper
    {
        /// <summary>
        /// Escapes a reserved C# keyword and leaves every other modeled name unchanged.
        /// </summary>
        /// <param name="input">
        /// The modeled name to map.
        /// </param>
        /// <returns>
        /// The escaped keyword, or the unchanged modeled name.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="input"/> is <see langword="null" />.
        /// </exception>
        public static string Map(string input)
        {
            ArgumentNullException.ThrowIfNull(input);

            return QueryIsReserved(input) ? $"@{input}" : input;
        }

        /// <summary>
        /// Determines whether the supplied name is a reserved C# keyword.
        /// </summary>
        /// <param name="input">
        /// The name to test.
        /// </param>
        /// <returns>
        /// <see langword="true"/> when <paramref name="input"/> is a reserved C# keyword;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="input"/> is <see langword="null" />.
        /// </exception>
        public static bool QueryIsReserved(string input)
        {
            ArgumentNullException.ThrowIfNull(input);

            return SyntaxFacts.GetKeywordKind(input) != SyntaxKind.None;
        }
    }
}
