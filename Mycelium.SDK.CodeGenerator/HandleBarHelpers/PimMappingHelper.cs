// ------------------------------------------------------------------------------------------------
//  <copyright file="PimMappingHelper.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.HandleBarHelpers
{
    using HandlebarsDotNet;

    using uml4net.CommonStructure;

    /// <summary>
    /// Provides the Handlebars primitive that lets a PIM template carry a decision the model does not make.
    /// </summary>
    /// <remarks>
    /// The clause a service is specified in is editorial and is not carried by the PIM export. That table
    /// belongs in the reviewed template rather than in compiled code, so the helper takes the model element
    /// to map followed by the key/value pairs that map it.
    /// </remarks>
    public static class PimMappingHelper
    {
        /// <summary>
        /// Registers the mapping helpers.
        /// </summary>
        /// <param name="handlebars">
        /// The Handlebars environment in which the mapping helpers are registered.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="handlebars" /> is <see langword="null" />.
        /// </exception>
        public static void RegisterPimMappingHelper(this IHandlebars handlebars)
        {
            ArgumentNullException.ThrowIfNull(handlebars);

            handlebars.RegisterHelper("Pim.WriteMappedName", (writer, _, arguments) =>
            {
                const string helperName = "{{Pim.WriteMappedName}}";

                if (QueryFirstArgument(arguments, helperName) is not INamedElement namedElement)
                {
                    throw new HandlebarsException($"{helperName} requires an INamedElement argument.");
                }

                writer.WriteSafeString(QueryMappedValue(arguments, namedElement.Name, helperName));
            });
        }

        /// <summary>
        /// Queries the model element supplied to a mapping helper.
        /// </summary>
        /// <param name="arguments">
        /// The Handlebars helper arguments.
        /// </param>
        /// <param name="helperName">
        /// The helper name used in validation messages.
        /// </param>
        /// <returns>
        /// The first argument.
        /// </returns>
        /// <exception cref="HandlebarsException">
        /// Thrown when the helper was not supplied an element followed by at least one key/value pair.
        /// </exception>
        private static object QueryFirstArgument(Arguments arguments, string helperName)
        {
            if (arguments.Length < 3 || arguments.Length % 2 == 0)
            {
                throw new HandlebarsException($"{helperName} requires an element followed by at least one key/value pair.");
            }

            return arguments[0];
        }

        /// <summary>
        /// Queries the value that a key maps onto.
        /// </summary>
        /// <param name="arguments">
        /// The Handlebars helper arguments, holding the key/value pairs from the second position onwards.
        /// </param>
        /// <param name="key">
        /// The key to map.
        /// </param>
        /// <param name="helperName">
        /// The helper name used in validation messages.
        /// </param>
        /// <returns>
        /// The mapped value.
        /// </returns>
        /// <exception cref="HandlebarsException">
        /// Thrown when a key or value is not a string, or when the key has no mapping.
        /// </exception>
        private static string QueryMappedValue(Arguments arguments, string key, string helperName)
        {
            for (var index = 1; index < arguments.Length; index += 2)
            {
                if (string.Equals(QueryString(arguments, index, helperName), key, StringComparison.Ordinal))
                {
                    return QueryString(arguments, index + 1, helperName);
                }
            }

            throw new HandlebarsException($"{helperName} has no mapping for '{key}'.");
        }

        /// <summary>
        /// Queries one string argument supplied to a Handlebars helper.
        /// </summary>
        /// <param name="arguments">
        /// The Handlebars helper arguments.
        /// </param>
        /// <param name="index">
        /// The zero-based argument position.
        /// </param>
        /// <param name="helperName">
        /// The helper name used in validation messages.
        /// </param>
        /// <returns>
        /// The supplied string.
        /// </returns>
        /// <exception cref="HandlebarsException">
        /// Thrown when the argument at the supplied position is not a string.
        /// </exception>
        private static string QueryString(Arguments arguments, int index, string helperName) => arguments[index] as string ?? throw new HandlebarsException($"{helperName} requires a string argument at position {index}.");
    }
}
