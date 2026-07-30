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
    /// Makes modeled names legal C# identifiers without changing their spelling.
    /// </summary>
    public static class ReservedCSharpNameMapper
    {
        /// <summary>
        /// Escapes a C# keyword and leaves an already valid identifier unchanged.
        /// </summary>
        public static string Map(string input)
        {
            ArgumentException.ThrowIfNullOrEmpty(input);

            if (SyntaxFacts.IsValidIdentifier(input))
            {
                return input;
            }

            if (QueryIsReserved(input))
            {
                return $"@{input}";
            }

            throw new ArgumentException(
                $"'{input}' is not a valid C# identifier and cannot be escaped without changing the modeled value.",
                nameof(input));
        }

        /// <summary>
        /// Determines whether the supplied name is a reserved C# keyword.
        /// </summary>
        public static bool QueryIsReserved(string input)
        {
            ArgumentException.ThrowIfNullOrEmpty(input);

            return SyntaxFacts.GetKeywordKind(input) != SyntaxKind.None;
        }
    }
}
