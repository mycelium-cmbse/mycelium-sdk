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

            handlebars.RegisterHelper("Operation.WriteHttpMethodName", (writer, _, arguments) =>
            {
                var searchResult = arguments.QuerySingle<SearchResult>("{{Operation.WriteHttpMethodName}}");

                writer.WriteSafeString(searchResult.QueryHttpMethodName());
            });

            handlebars.RegisterHelper("Operation.WriteRouteTemplate", (writer, _, arguments) =>
            {
                var searchResult = arguments.QuerySingle<SearchResult>("{{Operation.WriteRouteTemplate}}");

                writer.WriteSafeString(searchResult.QueryRouteTemplate());
            });

            handlebars.RegisterHelper("Operation.WriteHandlerName", (writer, _, arguments) =>
            {
                var searchResult = arguments.QuerySingle<SearchResult>("{{Operation.WriteHandlerName}}");

                writer.WriteSafeString(searchResult.QueryHandlerName());
            });
        }
    }
}
