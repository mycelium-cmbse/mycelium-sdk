// ------------------------------------------------------------------------------------------------
//  <copyright file="EnumerationHelper.cs" company="Starion Group S.A.">
// 
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
// 
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.HandleBarHelpers
{
    using System;
    using System.Linq;

    using HandlebarsDotNet;
    using HandlebarsDotNet.Helpers.Utils;

    using uml4net.CommonStructure;
    using uml4net.Extensions;
    using uml4net.SimpleClassifiers;

    /// <summary>
    /// Provides Handlebars support for UML enumerations.
    /// </summary>
    public static class EnumerationHelper
    {
        /// <summary>
        /// Registers the enumeration helpers.
        /// </summary>
        public static void RegisterEnumerationHelper(this IHandlebars handlebars)
        {
            ArgumentNullException.ThrowIfNull(handlebars);

            handlebars.RegisterHelper(
                "Enumeration.WriteDtoUsing",
                (writer, _, arguments) =>
                {
                    if (arguments.Length != 1)
                    {
                        throw new HandlebarsException("{{Enumeration.WriteDtoUsing}} requires exactly one argument.");
                    }

                    if (arguments.Single() is not IEnumeration enumeration)
                    {
                        throw new HandlebarsException("{{Enumeration.WriteDtoUsing}} requires an IEnumeration argument.");
                    }

                    if (RequiresDtoNamespace(enumeration))
                    {
                        writer.WriteSafeString($"{Environment.NewLine}{Environment.NewLine}using Mycelium.SDK.DTO;");
                    }
                });
        }

        /// <summary>
        /// Determines whether the enumeration or any of its owned literals
        /// contains a <c>&lt;see cref="..." /&gt;</c> documentation reference.
        /// </summary>
        /// <param name="enumeration">
        /// The UML enumeration to inspect.
        /// </param>
        /// <returns>
        /// <see langword="true"/> when the DTO namespace should be emitted;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        private static bool RequiresDtoNamespace(IEnumeration enumeration)
        {
            return ContainsDocumentationReference(enumeration) || enumeration.OwnedLiteral.Any(ContainsDocumentationReference);
        }

        /// <summary>
        /// Determines whether an element's documentation contains a
        /// <c>&lt;see cref="..." /&gt;</c> reference after being HTML-decoded once.
        /// </summary>
        /// <param name="element">
        /// The UML element whose documentation is inspected.
        /// </param>
        /// <returns>
        /// <see langword="true"/> when a documentation reference is present;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        private static bool ContainsDocumentationReference(IElement element)
        {
            var documentation = HtmlUtils.HtmlDecode(element.QueryRawDocumentation());

            return documentation.Contains("<see cref=", StringComparison.Ordinal);
        }
    }
}
