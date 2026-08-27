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
    using Microsoft.CodeAnalysis.CSharp;

    /// <summary>
    /// Validates modeled names and escapes reserved C# keywords without otherwise changing them.
    /// </summary>
    public static class ReservedCSharpNameMapper
    {
        /// <summary>
        /// Escapes a C# keyword and leaves an already valid identifier unchanged.
        /// </summary>
        /// <param name="input">
        /// The modeled name to map to a legal C# identifier.
        /// </param>
        /// <returns>
        /// The escaped keyword, or the unchanged valid identifier.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="input" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="input" /> is empty or cannot be represented as a legal C# identifier
        /// without changing the modeled value.
        /// </exception>
        public static string Map(string input)
        {
            ArgumentException.ThrowIfNullOrEmpty(input);

            if (QueryIsReserved(input))
            {
                return $"@{input}";
            }

            var parsedIdentifier = SyntaxFactory.ParseToken(input);

            if (SyntaxFacts.IsValidIdentifier(input) && string.Equals(parsedIdentifier.ValueText, input, StringComparison.Ordinal))
            {
                return input;
            }

            throw new ArgumentException(
                $"'{input}' is not a valid C# identifier and cannot be escaped without changing the modeled value.",
                nameof(input));
        }

        /// <summary>
        /// Determines whether the supplied name is a reserved C# keyword.
        /// </summary>
        /// <param name="input">
        /// The name to test.
        /// </param>
        /// <returns>
        /// <see langword="true" /> when <paramref name="input" /> is a reserved C# keyword;
        /// otherwise, <see langword="false" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="input" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="input" /> is empty.
        /// </exception>
        public static bool QueryIsReserved(string input)
        {
            ArgumentException.ThrowIfNullOrEmpty(input);

            return SyntaxFacts.GetKeywordKind(input) != SyntaxKind.None;
        }
    }
}
