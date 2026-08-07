// ------------------------------------------------------------------------------------------------
//  <copyright file="PropertyExtension.cs" company="Starion Group S.A.">
// 
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
// 
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Extensions
{
    using System;

    using uml4net.Classification;
    using uml4net.Extensions;
    using uml4net.SimpleClassifiers;
    using uml4net.StructuredClassifiers;

    /// <summary>
    /// Provides DTO-specific name, type, and multiplicity queries for UML properties.
    /// </summary>
    public static class PropertyExtension
    {
        /// <summary>
        /// Queries the legal C# DTO property identifier with its first letter capitalized.
        /// </summary>
        /// <param name="property">
        /// The UML property whose DTO identifier is queried.
        /// </param>
        /// <returns>
        /// The legal C# DTO property identifier with its first letter capitalized.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="property"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the UML property has no name.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when the capitalized property name cannot be represented as a legal C# identifier.
        /// </exception>
        public static string QueryDtoPropertyName(this IProperty property)
        {
            ArgumentNullException.ThrowIfNull(property);

            if (string.IsNullOrWhiteSpace(property.Name))
            {
                throw new InvalidOperationException($"Property '{property.XmiId}' has no name.");
            }

            var cSharpPropertyName = property.Name.CapitalizeFirstLetter();

            return ReservedCSharpNameMapper.Map(cSharpPropertyName);
        }

        /// <summary>
        /// Queries the complete DTO property type, including collection and
        /// scalar-nullability syntax.
        /// </summary>
        /// <param name="property">
        /// The UML property whose complete DTO type is queried.
        /// </param>
        /// <returns>
        /// The complete C# DTO type name, including collection and scalar-nullability syntax.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="property"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the property type is unresolved, unsupported, or unnamed.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when a named UML type cannot be represented as a legal C# identifier.
        /// </exception>
        public static string QueryDtoTypeName(this IProperty property)
        {
            ArgumentNullException.ThrowIfNull(property);

            var elementTypeName = property.QueryDtoElementTypeName();

            if (property.QueryIsEnumerable())
            {
                return $"List<{elementTypeName}>";
            }

            if (property.QueryIsNullableAndNotString())
            {
                return $"{elementTypeName}?";
            }

            return elementTypeName;
        }

        /// <summary>
        /// Queries the C# DTO element type before multiplicity and nullability
        /// syntax are applied.
        /// </summary>
        /// <param name="property">
        /// The UML property whose DTO element type is queried.
        /// </param>
        /// <returns>
        /// The corresponding C# DTO element type name.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the property type is unresolved or unsupported.
        /// </exception>
        private static string QueryDtoElementTypeName(this IProperty property)
        {
            if (property.Type is null)
            {
                throw new InvalidOperationException($"Property '{property.Describe()}' has no resolved type.");
            }

            return property.Type switch
            {
                IClass => "Guid",
                IEnumeration enumeration => MapNamedType(enumeration.Name, property),
                IPrimitiveType primitiveType => primitiveType.QueryCSharpTypeName(),
                _ => throw new InvalidOperationException($"Property '{property.Describe()}' has unsupported UML type "
                                                         + $"'{property.Type.Name}'.")
            };
        }

        /// <summary>
        /// Maps a named UML type to a legal C# identifier.
        /// </summary>
        /// <param name="typeName">
        /// The UML type name to map.
        /// </param>
        /// <param name="property">
        /// The property using the named type.
        /// </param>
        /// <returns>
        /// The corresponding legal C# type identifier.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the UML type has no name.
        /// </exception>
        private static string MapNamedType(string typeName, IProperty property)
        {
            if (string.IsNullOrWhiteSpace(typeName))
            {
                throw new InvalidOperationException($"Property '{property.Describe()}' has an unnamed type.");
            }

            return ReservedCSharpNameMapper.Map(typeName);
        }
    }
}
