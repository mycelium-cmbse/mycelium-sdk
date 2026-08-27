// ------------------------------------------------------------------------------------------------
//  <copyright file="EnumerationLiteralHelper.cs" company="Starion Group S.A.">
// 
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
// 
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.HandleBarHelpers
{
    using HandlebarsDotNet;

    using Mycelium.SDK.CodeGenerator.Extensions;

    using uml4net.SimpleClassifiers;

    /// <summary>
    /// Provides Handlebars support for UML enumeration literals.
    /// </summary>
    public static class EnumerationLiteralHelper
    {
        /// <summary>
        /// Registers the enumeration-literal helper.
        /// </summary>
        /// <param name="handlebars">
        /// The Handlebars environment in which the enumeration-literal helper is registered.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="handlebars" /> is <see langword="null" />.
        /// </exception>
        public static void RegisterEnumerationLiteralHelper(
            this IHandlebars handlebars)
        {
            ArgumentNullException.ThrowIfNull(handlebars);

            handlebars.RegisterHelper(
                "EnumerationLiteral.Write",
                (writer, _, arguments) =>
                {
                    if (arguments.Length != 1)
                    {
                        throw new HandlebarsException(
                            "{{EnumerationLiteral.Write}} requires exactly one argument.");
                    }

                    if (arguments.Single() is not IEnumerationLiteral literal)
                    {
                        throw new HandlebarsException(
                            "{{EnumerationLiteral.Write}} requires an IEnumerationLiteral argument.");
                    }

                    writer.WriteSafeString(
                        ReservedCSharpNameMapper.Map(literal.Name));
                });
        }
    }
}
