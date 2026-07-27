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

        private static bool RequiresDtoNamespace(IEnumeration enumeration)
        {
            return ContainsDocumentationReference(enumeration) || enumeration.OwnedLiteral.Any(ContainsDocumentationReference);
        }

        private static bool ContainsDocumentationReference(IElement element)
        {
            return element.QueryRawDocumentation().Contains("<see cref=", StringComparison.Ordinal);
        }
    }
}