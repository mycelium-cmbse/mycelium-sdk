// ------------------------------------------------------------------------------------------------
//  <copyright file="ArgumentsExtensions.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.HandleBarHelpers
{
    using HandlebarsDotNet;

    /// <summary>
    /// Unpacks the arguments a template supplied to a Handlebars helper.
    /// </summary>
    public static class ArgumentsExtensions
    {
        extension(Arguments arguments)
        {
            /// <summary>
            /// Queries the single argument supplied to a helper.
            /// </summary>
            /// <typeparam name="T">
            /// The type the argument must have.
            /// </typeparam>
            /// <param name="helperName">
            /// The helper name used in validation messages.
            /// </param>
            /// <returns>
            /// The supplied argument.
            /// </returns>
            /// <exception cref="HandlebarsException">
            /// Thrown when exactly one argument of type <typeparamref name="T" /> was not supplied.
            /// </exception>
            public T QuerySingle<T>(string helperName)
            {
                if (arguments.Length != 1)
                {
                    throw new HandlebarsException($"{helperName} requires exactly one argument.");
                }

                return arguments[0] is T argument
                    ? argument
                    : throw new HandlebarsException($"{helperName} requires {DescribeExpectedType<T>()} argument.");
            }

            /// <summary>
            /// Queries the first of several arguments supplied to a helper.
            /// </summary>
            /// <typeparam name="T">
            /// The type the first argument must have.
            /// </typeparam>
            /// <param name="helperName">
            /// The helper name used in validation messages.
            /// </param>
            /// <returns>
            /// The first supplied argument.
            /// </returns>
            /// <exception cref="HandlebarsException">
            /// Thrown when the first argument is absent or is not of type <typeparamref name="T" />.
            /// </exception>
            public T QueryFirst<T>(string helperName)
            {
                return arguments.Length > 0 && arguments[0] is T argument
                    ? argument
                    : throw new HandlebarsException($"{helperName} requires {DescribeExpectedType<T>()} first argument.");
            }

            /// <summary>
            /// Queries the arguments from a position onwards, each of which must be a string.
            /// </summary>
            /// <param name="index">
            /// The zero-based position from which to read.
            /// </param>
            /// <param name="helperName">
            /// The helper name used in validation messages.
            /// </param>
            /// <returns>
            /// The supplied strings, empty when the helper was given none.
            /// </returns>
            /// <exception cref="HandlebarsException">
            /// Thrown when one of those arguments is not a string.
            /// </exception>
            public string[] QueryStringsFrom(int index, string helperName)
            {
                return arguments.Skip(index)
                    .Select(argument => argument as string ?? throw new HandlebarsException($"{helperName} requires every argument from position {index} to be a string."))
                    .ToArray();
            }
        }

        /// <summary>
        /// Describes an expected argument type with the article that reads correctly before it.
        /// </summary>
        /// <typeparam name="T">
        /// The expected argument type.
        /// </typeparam>
        /// <returns>
        /// The article and the type name - for example <c>an IClass</c>.
        /// </returns>
        private static string DescribeExpectedType<T>()
        {
            var typeName = typeof(T).Name;

            return "AEIOU".Contains(typeName[0]) ? $"an {typeName}" : $"a {typeName}";
        }
    }
}
