// ------------------------------------------------------------------------------------------------
//  <copyright file="ClassHelper.cs" company="Starion Group S.A.">
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

    using uml4net.StructuredClassifiers;

    /// <summary>
    /// Provides Handlebars support for UML classes used to generate DTOs and POCOs.
    /// </summary>
    public static class ClassHelper
    {
        /// <summary>
        /// Registers the DTO class helpers.
        /// </summary>
        /// <param name="handlebars">
        /// The Handlebars environment in which the DTO class helpers are registered.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="handlebars" /> is <see langword="null" />.
        /// </exception>
        public static void RegisterDtoClassHelper(this IHandlebars handlebars)
        {
            ArgumentNullException.ThrowIfNull(handlebars);

            handlebars.RegisterHelper(
                "Class.QueryDtoInterfaceProperties",
                (_, arguments) =>
                    QueryClass(arguments, "{{Class.QueryDtoInterfaceProperties}}")
                        .QueryDtoInterfaceProperties());

            handlebars.RegisterHelper(
                "Class.QueryDtoImplementationProperties",
                (_, arguments) =>
                    QueryClass(arguments, "{{Class.QueryDtoImplementationProperties}}")
                        .QueryDtoImplementationProperties());

            handlebars.RegisterHelper(
                "Class.WriteDtoInterfaceIdentifier",
                (writer, _, arguments) =>
                {
                    var umlClass = QueryClass(
                        arguments,
                        "{{Class.WriteDtoInterfaceIdentifier}}");

                    writer.WriteSafeString(QueryGeneratedInterfaceIdentifier(umlClass));
                });

            handlebars.RegisterHelper(
                "Class.WriteDtoInterfaceGeneralizations",
                (writer, _, arguments) =>
                {
                    var umlClass = QueryClass(arguments, "{{Class.WriteDtoInterfaceGeneralizations}}");
                    var inheritance = string.Join(", ", umlClass.QueryDtoGeneralizations().Select(QueryGeneratedInterfaceIdentifier));

                    if (inheritance.Length > 0)
                    {
                        writer.WriteSafeString($" : {inheritance}");
                    }
                });
        }

        /// <summary>
        /// Registers the POCO class helpers independently of the DTO class helpers.
        /// </summary>
        /// <param name="handlebars">
        /// The Handlebars environment in which the POCO class helpers are registered.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="handlebars" /> is <see langword="null" />.
        /// </exception>
        public static void RegisterPocoClassHelper(this IHandlebars handlebars)
        {
            ArgumentNullException.ThrowIfNull(handlebars);

            handlebars.RegisterHelper(
                "Class.QueryPocoInterfaceProperties",
                (_, arguments) => QueryClass(arguments, "{{Class.QueryPocoInterfaceProperties}}").QueryPocoInterfaceProperties());

            handlebars.RegisterHelper(
                "Class.QueryPocoImplementationProperties",
                (_, arguments) => QueryClass(arguments, "{{Class.QueryPocoImplementationProperties}}").QueryPocoImplementationProperties());

            handlebars.RegisterHelper(
                "Class.WritePocoInterfaceIdentifier",
                (writer, _, arguments) =>
                {
                    var umlClass = QueryClass(arguments, "{{Class.WritePocoInterfaceIdentifier}}");
                    writer.WriteSafeString(QueryGeneratedInterfaceIdentifier(umlClass));
                });

            handlebars.RegisterHelper(
                "Class.WritePocoInterfaceGeneralizations",
                (writer, _, arguments) =>
                {
                    var umlClass = QueryClass(arguments, "{{Class.WritePocoInterfaceGeneralizations}}");

                    var inheritance = string.Join(", ", umlClass.QueryPocoGeneralizations().Select(QueryGeneratedInterfaceIdentifier));

                    if (inheritance.Length > 0)
                    {
                        writer.WriteSafeString($" : {inheritance}");
                    }
                });
        }

        /// <summary>
        /// Queries the single UML class supplied to a Handlebars helper.
        /// </summary>
        /// <param name="arguments">
        /// The Handlebars helper arguments.
        /// </param>
        /// <param name="helperName">
        /// The helper name used in validation messages.
        /// </param>
        /// <returns>
        /// The supplied UML class.
        /// </returns>
        /// <exception cref="HandlebarsException">
        /// Thrown when exactly one <see cref="IClass" /> argument was not supplied.
        /// </exception>
        private static IClass QueryClass(Arguments arguments, string helperName)
        {
            if (arguments.Length != 1)
            {
                throw new HandlebarsException($"{helperName} requires exactly one argument.");
            }

            if (arguments.Single() is not IClass umlClass)
            {
                throw new HandlebarsException($"{helperName} requires an IClass argument.");
            }

            return umlClass;
        }

        /// <summary>
        /// Queries the legal generated C# interface identifier for a UML class.
        /// </summary>
        /// <param name="umlClass">
        /// The UML class whose generated interface identifier is queried.
        /// </param>
        /// <returns>
        /// The legal generated C# interface identifier.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the UML class has no name.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when the generated name cannot be represented as a legal C# identifier.
        /// </exception>
        private static string QueryGeneratedInterfaceIdentifier(IClass umlClass)
        {
            if (string.IsNullOrWhiteSpace(umlClass.Name))
            {
                throw new InvalidOperationException($"Class '{umlClass.XmiId}' has no name.");
            }

            return ReservedCSharpNameMapper.Map($"I{umlClass.Name}");
        }
    }
}
