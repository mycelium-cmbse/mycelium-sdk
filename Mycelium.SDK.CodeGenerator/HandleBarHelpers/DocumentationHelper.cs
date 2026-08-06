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
        private const int DocumentationLineLength = 100;
        private const int RegexMatchTimeoutMilliseconds = 1000;
        
        [GeneratedRegex(@"<see\s+cref=""([^""]+)""\s*>", RegexOptions.CultureInvariant, RegexMatchTimeoutMilliseconds)]
        private static partial Regex SeeCrefOpeningTag();
        
        [GeneratedRegex(@"<see\s+cref=""[^""]+""\s*/>[.,;:!?]?|\S+", RegexOptions.CultureInvariant, RegexMatchTimeoutMilliseconds)]
        private static partial Regex DocumentationToken();

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
