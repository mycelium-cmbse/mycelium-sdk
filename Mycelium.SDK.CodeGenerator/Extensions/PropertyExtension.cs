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
    using uml4net.Classification;
    using uml4net.Extensions;
    using uml4net.SimpleClassifiers;
    using uml4net.StructuredClassifiers;

    /// <summary>
    /// Provides artifact-specific name, type, multiplicity and relational-storage queries for UML properties.
    /// </summary>
    public static class PropertyExtension
    {
        /// <summary>
        /// Maps a referenced UML class to its generated POCO interface identifier.
        /// </summary>
        /// <param name="umlClass">
        /// The referenced UML class.
        /// </param>
        /// <param name="property">
        /// The property containing the reference.
        /// </param>
        /// <returns>
        /// The legal generated interface identifier.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when <paramref name="umlClass" /> has no name.
        /// </exception>
        private static string QueryPocoInterfaceTypeName(IClass umlClass, IProperty property)
        {
            if (string.IsNullOrWhiteSpace(umlClass.Name))
            {
                throw new InvalidOperationException($"Property '{property.Describe()}' has an unnamed type.");
            }

            return ReservedCSharpNameMapper.Map($"I{umlClass.Name}");
        }

        /// <summary>
        /// Preserves the uml4net C# representation of a standard or custom primitive.
        /// </summary>
        /// <param name="primitiveType">
        /// The modeled standard or custom primitive.
        /// </param>
        /// <param name="property">
        /// The property using the primitive type.
        /// </param>
        /// <returns>
        /// The corresponding C# primitive or custom primitive representation.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when <paramref name="primitiveType" /> has no C# type name.
        /// </exception>
        private static string QueryPrimitiveTypeName(IPrimitiveType primitiveType, IProperty property)
        {
            var typeName = primitiveType.QueryCSharpTypeName();

            if (string.IsNullOrWhiteSpace(typeName))
            {
                throw new InvalidOperationException($"Property '{property.Describe()}' has an unnamed type.");
            }

            return typeName;
        }

        /// <summary>
        /// Determines whether an optional POCO element type requires nullable value-type syntax.
        /// </summary>
        /// <param name="property">
        /// The UML property whose type is examined.
        /// </param>
        /// <param name="elementTypeName">
        /// The mapped C# element type.
        /// </param>
        /// <returns>
        /// <see langword="true" /> when the mapped type is a C# value type.
        /// </returns>
        private static bool QueryPocoElementIsValueType(IProperty property, string elementTypeName)
        {
            if (property.Type is IEnumeration)
            {
                return true;
            }

            return property.Type is IPrimitiveType && elementTypeName is
                "bool" or "byte" or "sbyte" or "short" or "ushort" or "int" or "uint" or "long" or "ulong" or "nint" or "nuint" or "char" or
                "float" or "double" or "decimal" or "DateTime" or "DateTimeOffset" or "Guid" or "TimeSpan";
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

        /// <param name="property">
        /// The UML property that is queried.
        /// </param>
        extension(IProperty property)
        {
            /// <summary>
            /// Queries the eligible navigable scalar opposite that can store a SQL association.
            /// </summary>
            /// <returns>
            /// The named, non-derived scalar reference declared by its class, or <see langword="null" /> when no eligible opposite exists.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the property is <see langword="null" />.
            /// </exception>
            public IProperty QuerySqlScalarOpposite()
            {
                ArgumentNullException.ThrowIfNull(property);

                if (property.Opposite is not { Owner: IClass owner } opposite || opposite.QueryIsEnumerable())
                {
                    return null;
                }

                return owner.QuerySqlReferenceProperties()
                    .FirstOrDefault(reference => string.Equals(reference.XmiId, opposite.XmiId, StringComparison.Ordinal));
            }

            /// <summary>
            /// Determines whether a collection requires its own SQL link table.
            /// </summary>
            /// <returns>
            /// <see langword="true" /> for a collection without an eligible navigable scalar opposite.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the property is <see langword="null" />.
            /// </exception>
            public bool QuerySqlRequiresLinkTable()
            {
                ArgumentNullException.ThrowIfNull(property);

                return property.QueryIsEnumerable() && property.QuerySqlScalarOpposite() is null;
            }

            /// <summary>
            /// Determines whether composition deletion needs a trigger beyond a contained object's scalar owner reference.
            /// </summary>
            /// <returns>
            /// <see langword="true" /> for a composite reference without an eligible scalar owner opposite.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the property is <see langword="null" />.
            /// </exception>
            public bool QuerySqlRequiresCompositionTrigger()
            {
                ArgumentNullException.ThrowIfNull(property);

                return property.IsComposite && property.QuerySqlScalarOpposite() is null;
            }

            /// <summary>
            /// Queries the name of the SQL link table that stores the collection, derived from its declaring class and its own name only.
            /// </summary>
            /// <returns>
            /// The <c>&lt;owner&gt;_&lt;property&gt;</c> PostgreSQL identifier, for example <c>functional_project_branch_rules</c>.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the property is <see langword="null" />.
            /// </exception>
            /// <exception cref="InvalidOperationException">
            /// Thrown when the property is not owned by a class.
            /// </exception>
            public string QuerySqlLinkTableName()
            {
                ArgumentNullException.ThrowIfNull(property);

                if (property.Owner is not IClass owner)
                {
                    throw new InvalidOperationException($"Property '{property.Describe()}' is not owned by a class, so it has no SQL link table.");
                }

                return NamedElementExtensions.ToSqlIdentifier($"{owner.Name}_{property.Name}");
            }

            /// <summary>
            /// Determines whether the SQL link table of a collection records the position of each occurrence.
            /// </summary>
            /// <returns>
            /// <see langword="true" /> when the collection is ordered or permits duplicate occurrences.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the property is <see langword="null" />.
            /// </exception>
            public bool QuerySqlLinkHasPosition()
            {
                ArgumentNullException.ThrowIfNull(property);

                return property.IsOrdered || !property.IsUnique;
            }

            /// <summary>
            /// Queries the legal C# property identifier with its first letter capitalized.
            /// </summary>
            /// <returns>
            /// The legal PascalCase C# property identifier.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the property is <see langword="null" />.
            /// </exception>
            /// <exception cref="InvalidOperationException">
            /// Thrown when the UML property has no name.
            /// </exception>
            /// <exception cref="ArgumentException">
            /// Thrown when the capitalized property name cannot be represented as a legal C# identifier.
            /// </exception>
            public string QueryPropertyName()
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
            /// <returns>
            /// The complete C# DTO type name, including collection and scalar-nullability syntax.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the property is <see langword="null" />.
            /// </exception>
            /// <exception cref="InvalidOperationException">
            /// Thrown when the property type is unresolved, unsupported, or unnamed.
            /// </exception>
            /// <exception cref="ArgumentException">
            /// Thrown when a named UML type cannot be represented as a legal C# identifier.
            /// </exception>
            public string QueryDtoTypeName()
            {
                ArgumentNullException.ThrowIfNull(property);

                var elementTypeName = property.QueryDtoElementTypeName();

                if (property.QueryIsEnumerable())
                {
                    return $"List<{elementTypeName}>";
                }

                if (property.QueryIsNullableAndNotString() && elementTypeName is not ("Uri" or "Dictionary<string,string>"))
                {
                    return $"{elementTypeName}?";
                }

                return elementTypeName;
            }

            /// <summary>
            /// Queries the complete POCO property type, including interface references, collections, and
            /// value-type nullability.
            /// </summary>
            /// <returns>
            /// The complete C# POCO type name.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the property is <see langword="null" />.
            /// </exception>
            /// <exception cref="InvalidOperationException">
            /// Thrown when the property type is unresolved, unsupported, or unnamed.
            /// </exception>
            /// <exception cref="ArgumentException">
            /// Thrown when a named UML type cannot be represented as a legal C# identifier.
            /// </exception>
            public string QueryPocoTypeName()
            {
                ArgumentNullException.ThrowIfNull(property);

                var elementTypeName = property.QueryPocoElementTypeName();

                if (property.QueryIsEnumerable())
                {
                    return $"List<{elementTypeName}>";
                }

                if (property.QueryIsNullable() && QueryPocoElementIsValueType(property, elementTypeName))
                {
                    return $"{elementTypeName}?";
                }

                return elementTypeName;
            }

            /// <summary>
            /// Queries the opposite end of a property when that end is navigable from the property's type.
            /// </summary>
            /// <returns>
            /// The opposite end, or <see langword="null" /> when the association is navigable one way only.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the property is <see langword="null" />.
            /// </exception>
            public IProperty QueryNavigableOpposite()
            {
                ArgumentNullException.ThrowIfNull(property);

                return property.Opposite is { } opposite && property.Type is IClass oppositeOwner && oppositeOwner.OwnedAttribute.Contains(opposite) ? opposite : null;
            }

            /// <summary>
            /// Determines whether a property points back to the container that collects its owner.
            /// </summary>
            /// <returns>
            /// <see langword="true" /> when the navigable opposite end is a collection.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the property is <see langword="null" />.
            /// </exception>
            public bool QueryIsBackReference()
            {
                ArgumentNullException.ThrowIfNull(property);

                return property.QueryNavigableOpposite()
                    ?.QueryIsEnumerable() == true;
            }

            /// <summary>
            /// Determines whether a property collects parts that cannot exist without its owner.
            /// </summary>
            /// <returns>
            /// <see langword="true" /> when the property is a collection whose navigable opposite end is mandatory and single.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the property is <see langword="null" />.
            /// </exception>
            public bool QueryIsComposedPart()
            {
                ArgumentNullException.ThrowIfNull(property);

                return property.QueryIsEnumerable() && property.QueryNavigableOpposite() is { } opposite && !opposite.QueryIsEnumerable() && opposite.Lower > 0;
            }

            /// <summary>
            /// Queries the C# DTO element type before multiplicity and nullability
            /// syntax are applied.
            /// </summary>
            /// <returns>
            /// The corresponding C# DTO element type name.
            /// </returns>
            /// <exception cref="InvalidOperationException">
            /// Thrown when the property type is unresolved or unsupported.
            /// </exception>
            private string QueryDtoElementTypeName()
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
                    _ => throw new InvalidOperationException($"Property '{property.Describe()}' has unsupported UML type " + $"'{property.Type.Name}'.")
                };
            }

            /// <summary>
            /// Queries the POCO element type before multiplicity and nullability are applied.
            /// </summary>
            /// <returns>
            /// The corresponding POCO element type.
            /// </returns>
            /// <exception cref="InvalidOperationException">
            /// Thrown when the property's UML type is unresolved, unnamed, or unsupported.
            /// </exception>
            private string QueryPocoElementTypeName()
            {
                if (property.Type is null)
                {
                    throw new InvalidOperationException($"Property '{property.Describe()}' has no resolved type.");
                }

                return property.Type switch
                {
                    IClass umlClass => QueryPocoInterfaceTypeName(umlClass, property),
                    IEnumeration enumeration => MapNamedType(enumeration.Name, property),
                    IPrimitiveType primitiveType => QueryPrimitiveTypeName(primitiveType, property),
                    _ => throw new InvalidOperationException($"Property '{property.Describe()}' has unsupported UML type " + $"'{property.Type.Name}'.")
                };
            }
        }
    }
}
