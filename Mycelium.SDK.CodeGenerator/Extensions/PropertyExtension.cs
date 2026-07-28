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

            if (property.QueryIsEnumerable())
            {
                return $"List<{elementTypeName}>";
            }

            if (property.QueryIsNullable() && !string.Equals(elementTypeName, "string", StringComparison.Ordinal))
            {
                return $"{elementTypeName}?";
            }

            return elementTypeName;
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
                IPrimitiveType primitiveType => primitiveType.QueryCSharpTypeName(), 
                _ => throw new InvalidOperationException($"Property '{Describe(property)}' has unsupported UML type "
                                                         + $"'{property.Type.Name}'.")
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
