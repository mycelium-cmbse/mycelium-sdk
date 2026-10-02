// ------------------------------------------------------------------------------------------------
//  <copyright file="PimParameterHelper.cs" company="Starion Group S.A.">
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

    using uml4net.Classification;
    using uml4net.Extensions;

    /// <summary>
    /// Provides Handlebars support for the PIM parameters of a generated service member.
    /// </summary>
    public static class PimParameterHelper
    {
        /// <summary>
        /// Registers the parameter helpers.
        /// </summary>
        /// <param name="handlebars">
        /// The Handlebars environment in which the parameter helpers are registered.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="handlebars" /> is <see langword="null" />.
        /// </exception>
        public static void RegisterPimParameterHelper(this IHandlebars handlebars)
        {
            ArgumentNullException.ThrowIfNull(handlebars);

            handlebars.RegisterHelper("Parameter.WriteName", (writer, _, arguments) =>
            {
                var parameter = arguments.QuerySingle<IParameter>("{{Parameter.WriteName}}");

                writer.WriteSafeString(parameter.QueryParameterName());
            });

            handlebars.RegisterHelper("Parameter.WriteIdentifierName", (writer, _, arguments) =>
            {
                var parameter = arguments.QuerySingle<IParameter>("{{Parameter.WriteIdentifierName}}");

                writer.WriteSafeString(parameter.QueryIdentifierParameterName());
            });

            handlebars.RegisterHelper("Parameter.WriteTypeName", (writer, _, arguments) =>
            {
                const string HelperName = "{{Parameter.WriteTypeName}}";

                var parameter = arguments.QueryFirst<IParameter>(HelperName);
                var hyperlinkMappings = arguments.QueryStringsFrom(1, HelperName);

                writer.WriteSafeString(parameter.QueryCSharpTypeName(hyperlinkMappings));
            });

            handlebars.RegisterHelper("Parameter.WriteElementTypeName", (writer, _, arguments) =>
            {
                const string HelperName = "{{Parameter.WriteElementTypeName}}";

                var parameter = arguments.QueryFirst<IParameter>(HelperName);
                var hyperlinkMappings = arguments.QueryStringsFrom(1, HelperName);

                writer.WriteSafeString(parameter.QueryCSharpElementTypeName(hyperlinkMappings));
            });

            handlebars.RegisterHelper("Parameter.WriteIdentifierTypeName", (writer, _, arguments) =>
            {
                var parameter = arguments.QuerySingle<IParameter>("{{Parameter.WriteIdentifierTypeName}}");

                writer.WriteSafeString(parameter.QueryIdentifierTypeName());
            });

            handlebars.RegisterHelper("Parameter.QueryIsIdentifier", (_, arguments) =>
            {
                const string HelperName = "{{Parameter.QueryIsIdentifier}}";

                var parameter = arguments.QueryFirst<IParameter>(HelperName);

                if (arguments.Length < 2 || arguments[1] is not IOperation operation)
                {
                    throw new HandlebarsException($"{HelperName} requires an IOperation second argument.");
                }

                var contentCarryingNames = arguments.QueryStringsFrom(2, HelperName);

                return parameter.QueryIsIdentifier(operation, contentCarryingNames);
            });

            handlebars.RegisterHelper("Parameter.QueryIsCollection", (_, arguments) =>
            {
                var parameter = arguments.QuerySingle<IParameter>("{{Parameter.QueryIsCollection}}");

                return parameter.QueryIsEnumerable();
            });

            handlebars.RegisterHelper("Parameter.QueryIsOptionalValueType", (_, arguments) =>
            {
                var parameter = arguments.QuerySingle<IParameter>("{{Parameter.QueryIsOptionalValueType}}");

                return parameter.QueryIsOptionalValueType();
            });
        }
    }
}
