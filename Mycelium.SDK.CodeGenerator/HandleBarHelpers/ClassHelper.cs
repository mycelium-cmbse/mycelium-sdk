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

            handlebars.RegisterHelper("Class.QueryDtoInterfaceProperties", (_, arguments) => arguments.QuerySingle<IClass>("{{Class.QueryDtoInterfaceProperties}}").QueryDtoInterfaceProperties());

            handlebars.RegisterHelper("Class.QueryDtoImplementationProperties", (_, arguments) => arguments.QuerySingle<IClass>("{{Class.QueryDtoImplementationProperties}}").QueryDtoImplementationProperties());

            handlebars.RegisterHelper("Class.WriteDtoInterfaceIdentifier", (writer, _, arguments) =>
            {
                var umlClass = arguments.QuerySingle<IClass>("{{Class.WriteDtoInterfaceIdentifier}}");

                writer.WriteSafeString(QueryGeneratedInterfaceIdentifier(umlClass));
            });

            handlebars.RegisterHelper("Class.WriteDtoInterfaceGeneralizations", (writer, _, arguments) =>
            {
                var umlClass = arguments.QuerySingle<IClass>("{{Class.WriteDtoInterfaceGeneralizations}}");
                var inheritance = string.Join(", ", umlClass.QueryGeneralizations()
                    .Select(QueryGeneratedInterfaceIdentifier));

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

            handlebars.RegisterHelper("Class.QueryPocoInterfaceProperties", (_, arguments) => arguments.QuerySingle<IClass>("{{Class.QueryPocoInterfaceProperties}}").QueryPocoInterfaceProperties());

            handlebars.RegisterHelper("Class.QueryPocoImplementationProperties", (_, arguments) => arguments.QuerySingle<IClass>("{{Class.QueryPocoImplementationProperties}}").QueryPocoImplementationProperties());

            handlebars.RegisterHelper("Class.WritePocoInterfaceIdentifier", (writer, _, arguments) =>
            {
                var umlClass = arguments.QuerySingle<IClass>("{{Class.WritePocoInterfaceIdentifier}}");
                writer.WriteSafeString(QueryGeneratedInterfaceIdentifier(umlClass));
            });

            handlebars.RegisterHelper("Class.WritePocoInterfaceGeneralizations", (writer, _, arguments) =>
            {
                var umlClass = arguments.QuerySingle<IClass>("{{Class.WritePocoInterfaceGeneralizations}}");

                var inheritance = string.Join(", ", umlClass.QueryGeneralizations()
                    .Select(QueryGeneratedInterfaceIdentifier));

                if (inheritance.Length > 0)
                {
                    writer.WriteSafeString($" : {inheritance}");
                }
            });
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
