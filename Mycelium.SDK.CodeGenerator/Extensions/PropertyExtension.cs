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
    using System.Globalization;

    using uml4net.Classification;
    using uml4net.SimpleClassifiers;
    using uml4net.StructuredClassifiers;

    /// <summary>
    /// Provides DTO-specific type and multiplicity queries for UML properties.
    /// </summary>
    public static class PropertyExtension
    {
        /// <summary>
        /// Queries the exact, legal C# property identifier.
        /// </summary>
        public static string QueryDtoPropertyName(this IProperty property)
        {
            ArgumentNullException.ThrowIfNull(property);

            if (string.IsNullOrWhiteSpace(property.Name))
            {
                throw new InvalidOperationException($"Property '{property.XmiId}' has no name.");
            }

            return ReservedCSharpNameMapper.Map(property.Name);
        }

        /// <summary>
        /// Queries the complete DTO property type, including collection and
        /// scalar-nullability syntax.
        /// </summary>
        public static string QueryDtoTypeName(this IProperty property)
        {
            ArgumentNullException.ThrowIfNull(property);

            var elementTypeName = QueryDtoElementTypeName(property);

            if (property.QueryIsDtoCollection())
            {
                return $"List<{elementTypeName}>";
            }

            if (property.Lower == 0 && !string.Equals(elementTypeName, "string", StringComparison.Ordinal))
            {
                return $"{elementTypeName}?";
            }

            return elementTypeName;
        }

        /// <summary>
        /// Determines whether the property multiplicity requires a collection.
        /// </summary>
        public static bool QueryIsDtoCollection(this IProperty property)
        {
            ArgumentNullException.ThrowIfNull(property);

            if (property.Lower < 0)
            {
                throw new InvalidOperationException(
                    $"Property '{Describe(property)}' has invalid lower multiplicity " 
                    + $"'{property.Lower}'.");
            }

            if (property.Upper == "*")
            {
                return true;
            }

            if (!int.TryParse(property.Upper, NumberStyles.None, CultureInfo.InvariantCulture, out var upper))
            {
                throw new InvalidOperationException(
                    $"Property '{Describe(property)}' has invalid upper multiplicity "
                    + $"'{property.Upper}'.");
            }

            if (upper < property.Lower)
            {
                throw new InvalidOperationException(
                    $"Property '{Describe(property)}' has upper multiplicity "
                    + $"'{upper}' below lower multiplicity '{property.Lower}'.");
            }

            return upper > 1;
        }

        private static string QueryDtoElementTypeName(IProperty property)
        {
            if (property.Type is null)
            {
                throw new InvalidOperationException($"Property '{Describe(property)}' has no resolved type.");
            }

            return property.Type switch
            {
                IClass => "Guid",
                IEnumeration enumeration => MapNamedType(enumeration.Name, property),
                IPrimitiveType primitiveType => MapPrimitiveType(primitiveType, property),
                _ => throw new InvalidOperationException(
                    $"Property '{Describe(property)}' has unsupported UML type "
                    + $"'{property.Type.Name}'.")
            };
        }

        private static string MapPrimitiveType(
            IPrimitiveType primitiveType,
            IProperty property)
        {
            return primitiveType.Name switch
            {
                "Boolean" => "bool",
                "DateTime" => "DateTime",
                "Guid" => "Guid",
                "Integer" => "int",
                "Real" => "double",
                "String" => "string",
                "UnlimitedNatural" => "string",
                "Uri" => "Uri",
                _ => throw new InvalidOperationException(
                    $"Property '{Describe(property)}' uses unsupported primitive "
                    + $"'{primitiveType.Name}'.")
            };
        }

        private static string MapNamedType(string typeName, IProperty property)
        {
            if (string.IsNullOrWhiteSpace(typeName))
            {
                throw new InvalidOperationException($"Property '{Describe(property)}' has an unnamed type.");
            }

            return ReservedCSharpNameMapper.Map(typeName);
        }

        private static string Describe(IProperty property)
        {
            return string.IsNullOrWhiteSpace(property.Name) ? property.XmiId : property.Name;
        }
    }
}
