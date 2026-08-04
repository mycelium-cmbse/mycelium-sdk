// ------------------------------------------------------------------------------------------------
//  <copyright file="PropertyHelper.cs" company="Starion Group S.A.">
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
    /// Provides Handlebars support for UML properties used to generate DTOs.
    /// </summary>
    public static class PropertyHelper
    {
        /// <summary>
        /// Registers the DTO property helpers.
        /// </summary>
        public static void RegisterPropertyHelper(this IHandlebars handlebars)
        {
            ArgumentNullException.ThrowIfNull(handlebars);

            handlebars.RegisterHelper(
                "Property.WriteDtoInterfaceDeclaration",
                (writer, _, arguments) =>
                {
                    var property = QueryProperty(arguments, "{{Property.WriteDtoInterfaceDeclaration}}");
                    writer.WriteSafeString($"{property.QueryDtoTypeName()} " + $"{property.QueryDtoPropertyName()} {QueryDtoAccessors(property)}");
                });

            handlebars.RegisterHelper(
                "Property.WriteDtoImplementationDeclaration",
                (writer, _, arguments) =>
                {
                    var property = QueryProperty(arguments, "{{Property.WriteDtoImplementationDeclaration}}");

                    var collectionInitializer = property.QueryIsEnumerable()
                        ? " = [];"
                        : string.Empty;

                    writer.WriteSafeString(
                        $"public {property.QueryDtoTypeName()} " +
                        $"{property.QueryDtoPropertyName()} {QueryDtoAccessors(property)}" +
                        collectionInitializer);
                });
        }

        /// <summary>
        /// Queries the DTO accessor declaration for a UML property.
        /// </summary>
        /// <param name="property">
        /// The UML property.
        /// </param>
        /// <returns>
        /// A get-only declaration for a derived property; otherwise,
        /// a get-and-set declaration.
        /// </returns>
        private static string QueryDtoAccessors(IProperty property)
        {
            return property.IsDerived ? "{ get; }" : "{ get; set; }";
        }

        /// <summary>
        /// Queries the single UML property supplied to a Handlebars helper.
        /// </summary>
        /// <param name="arguments">
        /// The Handlebars helper arguments.
        /// </param>
        /// <param name="helperName">
        /// The helper name used in validation messages.
        /// </param>
        /// <returns>
        /// The supplied UML property.
        /// </returns>
        /// <exception cref="HandlebarsException">
        /// Thrown when exactly one <see cref="IProperty"/> argument was not supplied.
        /// </exception>
        private static IProperty QueryProperty(Arguments arguments, string helperName)
        {
            if (arguments.Length != 1)
            {
                throw new HandlebarsException($"{helperName} requires exactly one argument.");
            }

            if (arguments.Single() is not IProperty property)
            {
                throw new HandlebarsException($"{helperName} requires an IProperty argument.");
            }

            return property;
        }
    }
}
