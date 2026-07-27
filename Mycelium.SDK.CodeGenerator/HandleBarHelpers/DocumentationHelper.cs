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
    using System.Text.RegularExpressions;

    using HandlebarsDotNet;

    using uml4net.CommonStructure;
    using uml4net.Extensions;

    /// <summary>
    /// Writes canonical C# XML documentation.
    /// </summary>
    public static class DocumentationHelper
    {
        private const int DocumentationLineLength = 100;
        private static readonly Regex SeeCrefOpeningTag = new(@"<see\s+cref=""([^""]+)""\s*>", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        private static readonly Regex DocumentationToken = new(@"<see\s+cref=""[^""]+""\s*/>[.,;:!?]?|\S+", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /// <summary>
        /// Registers the documentation helper.
        /// </summary>
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

                    writer.WriteSafeString($"/// <summary>{Environment.NewLine}");
                    var documentation = element.QueryRawDocumentation();

                    documentation = SeeCrefOpeningTag
                        .Replace(documentation, "<see cref=\"$1\" />")
                        .Replace("</see>", string.Empty, StringComparison.Ordinal);

                    if (!string.IsNullOrEmpty(documentation))
                    {
                        foreach (var line in SplitDocumentation(documentation))
                        {
                            writer.WriteSafeString($"/// {line}{Environment.NewLine}");
                        }
                    }

                    writer.WriteSafeString($"/// </summary>{Environment.NewLine}");
                });
        }

        private static IEnumerable<string> SplitDocumentation(string documentation)
        {
            var line = string.Empty;

            foreach (Match match in DocumentationToken.Matches(documentation))
            {
                var token = match.Value;
                var candidate = string.IsNullOrEmpty(line)
                    ? token
                    : $"{line} {token}";

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
