// ------------------------------------------------------------------------------------------------
//  <copyright file="NamedElementExtensions.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Extensions
{
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;

    using Humanizer;

    using uml4net.CommonStructure;

    /// <summary>
    /// Provides Mycelium-specific queries for UML named elements.
    /// </summary>
    public static partial class NamedElementExtensions
    {
        /// <summary>
        /// The PostgreSQL identifier limit in bytes; generated identifiers contain only ASCII characters.
        /// </summary>
        private const int MaximumSqlIdentifierBytes = 63;

        /// <summary>
        /// The number of hexadecimal hash characters appended to a name that exceeds the identifier limit.
        /// </summary>
        private const int SqlIdentifierHashLength = 8;

        /// <summary>
        /// The time allowed to replace unsupported identifier characters in one modeled name.
        /// </summary>
        private const int SqlIdentifierRegexTimeoutMilliseconds = 1000;

        /// <summary>
        /// PostgreSQL 18 reserved identifiers, including keywords reserved for particular SQL contexts.
        /// Source: https://www.postgresql.org/docs/18/sql-keywords-appendix.html.
        /// </summary>
        private static readonly HashSet<string> ReservedSqlIdentifiers = (
            "all analyse analyze and any array as asc asymmetric authorization binary both case cast check collate collation column concurrently " +
            "constraint create cross current_catalog current_date current_role current_schema current_time current_timestamp current_user default " +
            "deferrable desc distinct do else end except false fetch for foreign freeze from full grant group having ilike in initially inner " +
            "intersect into is isnull join lateral leading left like limit localtime localtimestamp natural not notnull null offset on only or " +
            "order outer overlaps placing primary references returning right select session_user similar some symmetric system_user table " +
            "tablesample then to trailing true union unique user using variadic verbose when where window with")
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .ToHashSet(StringComparer.Ordinal);

        /// <summary>
        /// Converts a modeled name to a lowercase snake_case PostgreSQL identifier, mapping reserved and invalid names.
        /// </summary>
        /// <param name="name">
        /// The modeled class, property or relationship name.
        /// </param>
        /// <returns>
        /// A valid unquoted PostgreSQL identifier, hashed only when it exceeds the identifier limit.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="name" /> is <see langword="null" />.
        /// </exception>
        public static string ToSqlIdentifier(string name)
        {
            ArgumentNullException.ThrowIfNull(name);

            var identifier = UnsupportedSqlIdentifierCharacters().Replace(name.Underscore(), "_");

            if (identifier.Length == 0 || char.IsDigit(identifier[0]) || ReservedSqlIdentifiers.Contains(identifier))
            {
                identifier = "model_" + identifier;
            }

            return ShortenSqlIdentifier(identifier);
        }

        /// <summary>
        /// Keeps a readable name unchanged unless it exceeds PostgreSQL's identifier limit, in which case it is truncated and given a reproducible hash.
        /// </summary>
        /// <param name="readableName">
        /// The full readable name.
        /// </param>
        /// <returns>
        /// The readable name, or its truncated form followed by <c>_</c> and the first 8 lowercase hexadecimal characters of the SHA-256 of the full name.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="readableName" /> is <see langword="null" />.
        /// </exception>
        public static string ShortenSqlIdentifier(string readableName)
        {
            ArgumentNullException.ThrowIfNull(readableName);

            if (readableName.Length <= MaximumSqlIdentifierBytes)
            {
                return readableName;
            }

            var hash = Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(readableName)))[..SqlIdentifierHashLength];

            return $"{readableName[..(MaximumSqlIdentifierBytes - SqlIdentifierHashLength - 1)]}_{hash}";
        }

        /// <summary>
        /// Matches every character that cannot appear in an unquoted lowercase PostgreSQL identifier.
        /// </summary>
        /// <returns>
        /// A compile-time generated expression with a bounded match time.
        /// </returns>
        [GeneratedRegex("[^a-z0-9_]", RegexOptions.None, SqlIdentifierRegexTimeoutMilliseconds)]
        private static partial Regex UnsupportedSqlIdentifierCharacters();

        /// <param name="namedElement">
        /// The UML named element that is described.
        /// </param>
        extension(INamedElement namedElement)
        {
            /// <summary>
            /// Returns a readable description of a UML named element.
            /// </summary>
            /// <returns>
            /// The element name when available; otherwise, its XMI identifier.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the named element is <see langword="null"/>.
            /// </exception>
            public string Describe()
            {
                ArgumentNullException.ThrowIfNull(namedElement);
                return string.IsNullOrWhiteSpace(namedElement.Name) ? namedElement.XmiId : namedElement.Name;
            }

            /// <summary>
            /// Queries the C# namespace that a named element is declared in, relative to its model.
            /// </summary>
            /// <returns>
            /// The packages between the model and the element, dot-separated - for example <c>Systems.Parts</c> for
            /// <c>SysML::Systems::Parts::PartUsage</c>.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the named element is <see langword="null"/>.
            /// </exception>
            /// <exception cref="InvalidOperationException">
            /// Thrown when the element is not nested in a package of a model.
            /// </exception>
            /// <exception cref="ArgumentException">
            /// Thrown when a package name cannot be represented as a legal C# identifier.
            /// </exception>
            public string QueryNamespace()
            {
                ArgumentNullException.ThrowIfNull(namedElement);

                var segments = namedElement.QualifiedName?.Split("::") ?? [];

                return segments.Length < 3
                    ? throw new InvalidOperationException($"'{namedElement.Describe()}' has the qualified name '{namedElement.QualifiedName}', which names no package within a model.")
                    : string.Join('.', segments[1..^1].Select(ReservedCSharpNameMapper.Map));
            }

            /// <summary>
            /// Queries the PostgreSQL identifier of the element's modeled name, as used for its table or column in the generated schema.
            /// </summary>
            /// <returns>
            /// The lowercase snake_case identifier, prefixed with <c>model_</c> when reserved or invalid and hashed only when it exceeds the identifier limit.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the named element or its name is <see langword="null"/>.
            /// </exception>
            public string QuerySqlIdentifier()
            {
                ArgumentNullException.ThrowIfNull(namedElement);

                return ToSqlIdentifier(namedElement.Name);
            }
        }
    }
}
