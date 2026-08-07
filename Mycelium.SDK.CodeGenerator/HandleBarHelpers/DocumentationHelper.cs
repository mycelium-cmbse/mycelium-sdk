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
    using System.Linq;
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
        /// Gets the regular expression used to match opening XML documentation see tags.
        /// </summary>
        /// <returns>
        /// The generated regular expression.
        /// </returns>
        [GeneratedRegex(@"<see\s+cref=""([^""]+)""\s*>", RegexOptions.CultureInvariant, RegexMatchTimeoutMilliseconds)]
        private static partial Regex SeeCrefOpeningTag();
        
        /// <summary>
        /// Gets the regular expression used to tokenize documentation for line wrapping.
        /// </summary>
        /// <returns>
        /// The generated regular expression.
        /// </returns>
        [GeneratedRegex(@"<see\s+cref=""[^""]+""\s*/>[.,;:!?]?|\S+", RegexOptions.CultureInvariant, RegexMatchTimeoutMilliseconds)]
        private static partial Regex DocumentationToken();

        /// <summary>
        /// Registers the Documentation Handlebars helper that writes canonical C# XML documentation
        /// for an <see cref="IElement"/> context.
        /// </summary>
        /// <param name="handlebars">
        /// The Handlebars environment in which to register the helper.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="handlebars"/> is <see langword="null"/>.
        /// </exception>
        public static void RegisterDocumentationHelper(this IHandlebars handlebars)
        {
            ArgumentNullException.ThrowIfNull(handlebars);

            handlebars.RegisterHelper(
                "Documentation",
                (writer, context, _) =>
                {
                    if (context.Value is not IElement element)
                    {
                        throw new HandlebarsException("{{Documentation}} requires an IElement context.");
                    }

                    var documentation = HtmlUtils.HtmlDecode(element.QueryRawDocumentation());

                    if (string.IsNullOrWhiteSpace(documentation))
                    {
                        return;
                    }

                    documentation = SeeCrefOpeningTag()
                        .Replace(documentation, "<see cref=\"$1\" />")
                        .Replace("</see>", string.Empty, StringComparison.Ordinal);

                    writer.WriteSafeString($"/// <summary>{Environment.NewLine}");

                    foreach (var line in SplitDocumentation(documentation))
                    {
                        writer.WriteSafeString($"/// {line}{Environment.NewLine}");
                    }

                    writer.WriteSafeString($"/// </summary>{Environment.NewLine}");
                });
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
        private static IEnumerable<string> SplitDocumentation(string documentation)
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

                var candidate = line.Length == 0 ? token : $"{line} {token}";

                if (line.Length > 0 && candidate.Length > DocumentationLineLength)
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
