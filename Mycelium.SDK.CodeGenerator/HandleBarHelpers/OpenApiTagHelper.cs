// ------------------------------------------------------------------------------------------------
//  <copyright file="OpenApiTagHelper.cs" company="Starion Group S.A.">
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
    using System.Security;

    using HandlebarsDotNet;

    using Mycelium.SDK.CodeGenerator.Extensions;

    /// <summary>
    /// Provides Handlebars support for the OpenAPI tags that Carter modules are generated from.
    /// </summary>
    public static class OpenApiTagHelper
    {
        /// <summary>
        /// Registers the tag helpers.
        /// </summary>
        /// <param name="handlebars">
        /// The Handlebars environment in which the tag helpers are registered.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="handlebars"/> is <see langword="null" />.
        /// </exception>
        public static void RegisterOpenApiTagHelper(this IHandlebars handlebars)
        {
            ArgumentNullException.ThrowIfNull(handlebars);

            handlebars.RegisterHelper(
                "Tag.WriteModuleName",
                (writer, _, arguments) =>
                {
                    var tag = QueryTag(arguments, "{{Tag.WriteModuleName}}");

                    writer.WriteSafeString(tag.QueryModuleName());
                });

            handlebars.RegisterHelper(
                "Tag.WriteXmlName",
                (writer, _, arguments) =>
                {
                    var tag = QueryTag(arguments, "{{Tag.WriteXmlName}}");

                    writer.WriteSafeString(SecurityElement.Escape(tag));
                });
        }

        /// <summary>
        /// Queries the single tag supplied to a Handlebars helper.
        /// </summary>
        /// <param name="arguments">
        /// The Handlebars helper arguments.
        /// </param>
        /// <param name="helperName">
        /// The helper name used in validation messages.
        /// </param>
        /// <returns>
        /// The supplied tag.
        /// </returns>
        /// <exception cref="HandlebarsException">
        /// Thrown when exactly one non-empty string argument was not supplied.
        /// </exception>
        private static string QueryTag(Arguments arguments, string helperName)
        {
            if (arguments.Length != 1)
            {
                throw new HandlebarsException($"{helperName} requires exactly one argument.");
            }

            if (arguments.Single() is not string tag || string.IsNullOrWhiteSpace(tag))
            {
                throw new HandlebarsException($"{helperName} requires a non-empty string argument.");
            }

            return tag;
        }
    }
}
