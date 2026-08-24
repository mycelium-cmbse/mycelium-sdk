// ------------------------------------------------------------------------------------------------
//  <copyright file="OpenApiOperationHelper.cs" company="Starion Group S.A.">
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

    using Microsoft.OpenApi;

    using Mycelium.SDK.CodeGenerator.Extensions;

    /// <summary>
    /// Provides Handlebars support for the OpenAPI operations that back a Carter route.
    /// </summary>
    public static class OpenApiOperationHelper
    {
        /// <summary>
        /// Registers the operation helpers.
        /// </summary>
        /// <param name="handlebars">
        /// The Handlebars environment in which the operation helpers are registered.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="handlebars"/> is <see langword="null" />.
        /// </exception>
        public static void RegisterOpenApiOperationHelper(this IHandlebars handlebars)
        {
            ArgumentNullException.ThrowIfNull(handlebars);

            handlebars.RegisterHelper(
                "Operation.WriteHttpMethodName",
                (writer, _, arguments) =>
                {
                    var searchResult = QueryOperation(arguments, "{{Operation.WriteHttpMethodName}}");

                    writer.WriteSafeString(searchResult.QueryHttpMethodName());
                });

            handlebars.RegisterHelper(
                "Operation.WriteRouteTemplate",
                (writer, _, arguments) =>
                {
                    var searchResult = QueryOperation(arguments, "{{Operation.WriteRouteTemplate}}");

                    writer.WriteSafeString(searchResult.QueryRouteTemplate());
                });

            handlebars.RegisterHelper(
                "Operation.WriteHandlerName",
                (writer, _, arguments) =>
                {
                    var searchResult = QueryOperation(arguments, "{{Operation.WriteHandlerName}}");

                    writer.WriteSafeString(searchResult.QueryHandlerName());
                });
        }

        /// <summary>
        /// Queries the single operation supplied to a Handlebars helper.
        /// </summary>
        /// <param name="arguments">
        /// The Handlebars helper arguments.
        /// </param>
        /// <param name="helperName">
        /// The helper name used in validation messages.
        /// </param>
        /// <returns>
        /// The supplied operation.
        /// </returns>
        /// <exception cref="HandlebarsException">
        /// Thrown when exactly one <see cref="SearchResult" /> argument was not supplied.
        /// </exception>
        private static SearchResult QueryOperation(Arguments arguments, string helperName)
        {
            if (arguments.Length != 1)
            {
                throw new HandlebarsException($"{helperName} requires exactly one argument.");
            }

            if (arguments.Single() is not SearchResult searchResult)
            {
                throw new HandlebarsException($"{helperName} requires a SearchResult argument.");
            }

            return searchResult;
        }
    }
}
