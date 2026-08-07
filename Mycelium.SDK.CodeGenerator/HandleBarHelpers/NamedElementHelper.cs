// ------------------------------------------------------------------------------------------------
//  <copyright file="NamedElementHelper.cs" company="Starion Group S.A.">
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

    using Mycelium.SDK.CodeGenerator.Extensions;

    using uml4net.CommonStructure;

    /// <summary>
    /// Provides Handlebars support for UML named elements.
    /// </summary>
    public static class NamedElementHelper
    {
        /// <summary>
        /// Registers the named-element helpers.
        /// </summary>
        /// <param name="handlebars">
        /// The Handlebars environment in which the named-element helpers are registered.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="handlebars"/> is <see langword="null" />.
        /// </exception>
        public static void RegisterNamedElementHelper(this IHandlebars handlebars)
        {
            ArgumentNullException.ThrowIfNull(handlebars);

            handlebars.RegisterHelper(
                "NamedElement.WriteIdentifier",
                (writer, _, arguments) =>
                {
                    if (arguments.Length != 1)
                    {
                        throw new HandlebarsException(
                            "{{NamedElement.WriteIdentifier}} requires exactly one argument.");
                    }

                    if (arguments.Single() is not INamedElement namedElement)
                    {
                        throw new HandlebarsException(
                            "{{NamedElement.WriteIdentifier}} requires an INamedElement argument.");
                    }

                    writer.WriteSafeString(
                        ReservedCSharpNameMapper.Map(namedElement.Name));
                });
        }
    }
}
