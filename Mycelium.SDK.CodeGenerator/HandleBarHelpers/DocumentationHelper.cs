// ------------------------------------------------------------------------------------------------
//  <copyright file="DocumentationHelper.cs" company="Starion Group S.A.">
// 
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
// 
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.HandleBarHelpers
{
    using System.Net;
    using System.Text.RegularExpressions;

    using HandlebarsDotNet;
    using HandlebarsDotNet.Helpers.Utils;

    using uml4net.CommonStructure;
    using uml4net.Extensions;

    /// <summary>
    /// Writes canonical C# XML documentation.
    /// </summary>
    public static partial class DocumentationHelper
    {
        /// <summary>
        /// The line-wrapping threshold number of characters in a generated documentation content line.
        /// </summary>
        private const int DocumentationLineLength = 100;

        /// <summary>
        /// The regular-expression matching timeout, in milliseconds.
        /// </summary>
        private const int RegexMatchTimeoutMilliseconds = 1000;

        /// <summary>
        /// Registers the documentation helper without changing cref values.
        /// </summary>
        /// <param name="handlebars">
        /// The Handlebars environment in which to register the helper.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="handlebars" /> is <see langword="null" />.
        /// </exception>
        public static void RegisterDocumentationHelper(
            this IHandlebars handlebars)
        {
            RegisterDocumentationHelper(
                handlebars,
                static cref => cref);
        }

        /// <summary>
        /// Registers the documentation helper with CLR-symbol normalization.
        /// </summary>
        /// <param name="handlebars">
        /// The Handlebars environment in which to register the helper.
        /// </param>
        /// <param name="crefNormalizer">
        /// Resolves a UML cref value to its generated CLR symbol, or returns
        /// <see langword="null" /> when no valid symbol exists.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="handlebars" /> or <paramref name="crefNormalizer" /> is
        /// <see langword="null" />.
        /// </exception>
        public static void RegisterDocumentationHelper(
            this IHandlebars handlebars,
            Func<string, string> crefNormalizer)
        {
            ArgumentNullException.ThrowIfNull(handlebars);
            ArgumentNullException.ThrowIfNull(crefNormalizer);

            handlebars.RegisterHelper(
                "Documentation",
                (writer, context, _) =>
                {
                    if (context.Value is not IElement element)
                    {
                        throw new HandlebarsException(
                            "{{Documentation}} requires an IElement context.");
                    }

                    var documentation =
                        HtmlUtils.HtmlDecode(element.QueryRawDocumentation());

                    if (string.IsNullOrWhiteSpace(documentation))
                    {
                        return;
                    }

                    documentation = SeeCrefTag()
                        .Replace(
                            documentation,
                            match => NormalizeDocumentationCref(
                                match,
                                crefNormalizer))
                        .Replace(
                            "</see>",
                            string.Empty,
                            StringComparison.Ordinal);

                    writer.WriteSafeString(
                        $"/// <summary>{Environment.NewLine}");

                    foreach (var line in SplitDocumentation(documentation))
                    {
                        writer.WriteSafeString(
                            $"/// {line}{Environment.NewLine}");
                    }

                    writer.WriteSafeString(
                        $"/// </summary>{Environment.NewLine}");
                });
        }

        /// <summary>
        /// Gets the regular expression used to match XML documentation see tags.
        /// </summary>
        /// <returns>
        /// The generated regular expression.
        /// </returns>
        [GeneratedRegex(
            @"<see\s+cref=""([^""]+)""\s*/?>",
            RegexOptions.CultureInvariant,
            RegexMatchTimeoutMilliseconds)]
        private static partial Regex SeeCrefTag();

        /// <summary>
        /// Gets the regular expression used to tokenize documentation for line wrapping.
        /// </summary>
        /// <returns>
        /// The generated regular expression.
        /// </returns>
        [GeneratedRegex(
            @"(?:<see\s+cref=""[^""]+""\s*/>|<c>.*?</c>)[.,;:!?]?|\S+",
            RegexOptions.CultureInvariant,
            RegexMatchTimeoutMilliseconds)]
        private static partial Regex DocumentationToken();

        /// <summary>
        /// Normalizes one documentation cref tag.
        /// </summary>
        /// <param name="match">
        /// The matched cref tag.
        /// </param>
        /// <param name="crefNormalizer">
        /// The CLR-symbol resolver.
        /// </param>
        /// <returns>
        /// A resolved cref tag or safe code-formatted text.
        /// </returns>
        private static string NormalizeDocumentationCref(
            Match match,
            Func<string, string> crefNormalizer)
        {
            var cref = match.Groups[1].Value;
            var normalizedCref = crefNormalizer(cref);

            return string.IsNullOrWhiteSpace(normalizedCref)
                ? $"<c>{WebUtility.HtmlEncode(cref)}</c>"
                : $"<see cref=\"{normalizedCref}\" />";
        }

        /// <summary>
        /// Splits documentation into lines using the configured maximum length.
        /// </summary>
        /// <param name="documentation">
        /// The documentation text to split.
        /// </param>
        /// <returns>
        /// The wrapped documentation lines.
        /// </returns>
        private static IEnumerable<string> SplitDocumentation(
            string documentation)
        {
            var line = string.Empty;

            foreach (var token in DocumentationToken()
                         .Matches(documentation)
                         .Select(match => match.Value))
            {
                if (string.IsNullOrEmpty(token))
                {
                    continue;
                }

                var candidate =
                    line.Length == 0
                        ? token
                        : $"{line} {token}";

                if (line.Length > 0
                    && candidate.Length > DocumentationLineLength)
                {
                    yield return line;
                    line = token;
                    continue;
                }

                line = candidate;
            }

            if (line.Length > 0)
            {
                yield return line;
            }
        }
    }
}