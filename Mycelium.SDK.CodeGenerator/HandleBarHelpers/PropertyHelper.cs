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
    /// Provides Handlebars support for UML properties used to generate DTOs, POCOs,
    /// and JSON serializers.
    /// </summary>
    public static class PropertyHelper
    {
        /// <summary>
        /// Registers the DTO property helpers.
        /// </summary>
        /// <param name="handlebars">
        /// The Handlebars environment in which the DTO property helpers are registered.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="handlebars" /> is <see langword="null" />.
        /// </exception>
        public static void RegisterDtoPropertyHelper(this IHandlebars handlebars)
        {
            ArgumentNullException.ThrowIfNull(handlebars);

            handlebars.RegisterHelper(
                "Property.WriteDtoInterfaceDeclaration",
                (writer, _, arguments) =>
                {
                    var property = QueryProperty(arguments, "{{Property.WriteDtoInterfaceDeclaration}}");

                    writer.WriteSafeString($"{property.QueryDtoTypeName()} " + $"{property.QueryPropertyName()} {QueryAccessors(property)}");
                });

            handlebars.RegisterHelper(
                "Property.WriteDtoImplementationDeclaration",
                (writer, _, arguments) =>
                {
                    var property = QueryProperty(arguments, "{{Property.WriteDtoImplementationDeclaration}}");
                    var propertyTypeName = property.QueryDtoTypeName();
                    var collectionInitializer = QueryCollectionInitializer(property, propertyTypeName);

                    writer.WriteSafeString(
                        $"public {propertyTypeName} " +
                        $"{property.QueryPropertyName()} {QueryAccessors(property)}" +
                        collectionInitializer);
                });
        }

        /// <summary>
        /// Registers the JSON serializer property helpers independently of the DTO
        /// and POCO declaration helpers.
        /// </summary>
        /// <param name="handlebars">
        /// The Handlebars environment in which the JSON serializer helpers are registered.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="handlebars" /> is <see langword="null" />.
        /// </exception>
        public static void RegisterJsonSerializerPropertyHelper(
            this IHandlebars handlebars)
        {
            ArgumentNullException.ThrowIfNull(handlebars);

            handlebars.RegisterHelper(
                "Property.QueryIsIdentifier",
                (_, arguments) =>
                {
                    var property = QueryProperty(
                        arguments,
                        "{{Property.QueryIsIdentifier}}");

                    return string.Equals(
                               property.Name,
                               "id",
                               StringComparison.Ordinal)
                           && string.Equals(
                               property.QueryDtoTypeName(),
                               "Guid",
                               StringComparison.Ordinal);
                });

            handlebars.RegisterHelper(
                "Property.QueryIsStringDictionary",
                (_, arguments) =>
                {
                    var property = QueryProperty(
                        arguments,
                        "{{Property.QueryIsStringDictionary}}");

                    return string.Equals(
                        property.QueryDtoTypeName(),
                        "Dictionary<string,string>",
                        StringComparison.Ordinal);
                });

            handlebars.RegisterHelper(
                "Property.WritePropertyName",
                (writer, _, arguments) =>
                {
                    var property = QueryProperty(
                        arguments,
                        "{{Property.WritePropertyName}}");

                    writer.WriteSafeString(property.QueryPropertyName());
                });
        }

        /// <summary>
        /// Registers the POCO property helpers independently of the DTO property helpers.
        /// </summary>
        /// <param name="handlebars">
        /// The Handlebars environment in which the POCO property helpers are registered.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="handlebars" /> is <see langword="null" />.
        /// </exception>
        public static void RegisterPocoPropertyHelper(this IHandlebars handlebars)
        {
            ArgumentNullException.ThrowIfNull(handlebars);

            handlebars.RegisterHelper(
                "Property.WritePocoInterfaceDeclaration",
                (writer, _, arguments) =>
                {
                    var property = QueryProperty(arguments, "{{Property.WritePocoInterfaceDeclaration}}");

                    writer.WriteSafeString($"{property.QueryPocoTypeName()} " + $"{property.QueryPropertyName()} {QueryAccessors(property)}");
                });

            handlebars.RegisterHelper(
                "Property.WritePocoImplementationDeclaration",
                (writer, _, arguments) =>
                {
                    var property = QueryProperty(arguments, "{{Property.WritePocoImplementationDeclaration}}");
                    var propertyTypeName = property.QueryPocoTypeName();
                    var propertyName = property.QueryPropertyName();

                    writer.WriteSafeString($"public {propertyTypeName} {propertyName} " + QueryPocoImplementationSuffix(property, propertyName, propertyTypeName));
                });
        }

        /// <summary>
        /// Queries the accessor declaration shared by generated DTO and POCO properties.
        /// </summary>
        /// <param name="property">
        /// The UML property.
        /// </param>
        /// <returns>
        /// A getter-only declaration for a derived or derived-union property; otherwise,
        /// a get-and-set declaration.
        /// </returns>
        private static string QueryAccessors(IProperty property)
        {
            return QueryIsDerived(property) ? "{ get; }" : "{ get; set; }";
        }

        /// <summary>
        /// Determines whether the UML property represents derived state.
        /// </summary>
        /// <param name="property">
        /// The UML property.
        /// </param>
        /// <returns>
        /// <see langword="true" /> for a derived or derived-union property.
        /// </returns>
        private static bool QueryIsDerived(IProperty property)
        {
            return property.IsDerived || property.IsDerivedUnion;
        }

        /// <summary>
        /// Queries the implementation suffix for a concrete POCO property.
        /// </summary>
        /// <param name="property">
        /// The UML property.
        /// </param>
        /// <param name="propertyName">
        /// The generated C# property name.
        /// </param>
        /// <param name="propertyTypeName">
        /// The generated C# property type.
        /// </param>
        /// <returns>
        /// Computation delegation for derived state; otherwise, mutable accessors
        /// and any required collection initializer.
        /// </returns>
        private static string QueryPocoImplementationSuffix(IProperty property, string propertyName, string propertyTypeName)
        {
            if (QueryIsDerived(property))
            {
                return $"=> this.Compute{propertyName}();";
            }

            return $"{QueryAccessors(property)}" + QueryCollectionInitializer(property, propertyTypeName);
        }

        /// <summary>
        /// Queries the initializer for a generated concrete collection property.
        /// </summary>
        /// <param name="property">
        /// The UML property.
        /// </param>
        /// <param name="propertyTypeName">
        /// The mapped artifact-specific C# property type.
        /// </param>
        /// <returns>
        /// An empty collection initializer when required; otherwise, an empty string.
        /// </returns>
        private static string QueryCollectionInitializer(IProperty property, string propertyTypeName)
        {
            return property.QueryIsEnumerable() || propertyTypeName.StartsWith("Dictionary<", StringComparison.Ordinal)
                ? " = [];"
                : string.Empty;
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
        /// Thrown when exactly one <see cref="IProperty" /> argument was not supplied.
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
