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
    using System.Text.RegularExpressions;

    using HandlebarsDotNet.Helpers.Utils;

    using uml4net.Classification;
    using uml4net.Extensions;
    using uml4net.Profiling;
    using uml4net.SimpleClassifiers;
    using uml4net.StructuredClassifiers;
    using uml4net.Values;

    using EnterpriseArchitectAttribute = uml4net.xmi.Extensions.EnterpriseArchitect.Structure.IAttribute;
    using EnterpriseArchitectConnector = uml4net.xmi.Extensions.EnterpriseArchitect.Structure.IConnector;
    using EnterpriseArchitectTag = uml4net.xmi.Extensions.EnterpriseArchitect.Structure.ITag;

    /// <summary>
    /// Provides artifact-specific name, type, multiplicity, relational-storage and Mycelium profile queries for UML properties.
    /// </summary>
    public static partial class PropertyExtension
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

        /// <summary>
        /// Determines whether a property carries a stereotype of the Mycelium profile.
        /// </summary>
        /// <param name="property">
        /// The UML property whose stereotype applications are examined.
        /// </param>
        /// <param name="stereotypeName">
        /// The name of the Mycelium stereotype.
        /// </param>
        /// <returns>
        /// <see langword="true" /> when the export applies the named Mycelium stereotype to the property.
        /// </returns>
        private static bool QueryHasMyceliumStereotype(IProperty property, string stereotypeName)
        {
            return property.QueryStereoTypeApplication(stereotypeName) is { ProfileName: XmiResources.MyceliumProfileName };
        }

        /// <summary>
        /// Queries the Enterprise Architect tags of a property: those of its attribute, and those of its end of the connector that exports its
        /// association. A connector end is matched to its member end by the <c>src</c> / <c>dst</c> identifier convention of the export, as uml4net
        /// matches documentation.
        /// </summary>
        /// <param name="property">
        /// The UML property whose extension tags are queried.
        /// </param>
        /// <returns>
        /// The tags exported for the property.
        /// </returns>
        private static IEnumerable<EnterpriseArchitectTag> QueryExtensionTags(IProperty property)
        {
            IReadOnlyCollection<object> attributeExtenders = [];
            IReadOnlyCollection<object> connectorExtenders = [];

            property.Cache?.TryGetExtenders(property, out attributeExtenders);

            if (property.Association is { } association)
            {
                association.Cache?.TryGetExtenders(association, out connectorExtenders);
            }

            var attributeTags = (attributeExtenders ?? []).OfType<EnterpriseArchitectAttribute>()
                .SelectMany(attribute => attribute.Tags ?? []);

            var connectorEndTags = (connectorExtenders ?? []).OfType<EnterpriseArchitectConnector>()
                .Select(connector => property.XmiId.Contains("src", StringComparison.Ordinal) ? connector.Source : connector.Target)
                .SelectMany(connectorEnd => connectorEnd?.Tags ?? []);

            return attributeTags.Concat(connectorEndTags);
        }

        /// <summary>
        /// Queries the <c>onAction</c> values of a managed property.
        /// </summary>
        /// <param name="property">
        /// The UML property whose <c>onAction</c> tagged value is read.
        /// </param>
        /// <returns>
        /// The actions on which the server assigns the value; empty for a property that is not managed or whose value business logic sets.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the tagged value names an action other than <c>OnCreate</c> and <c>OnUpdate</c>.
        /// </exception>
        private static string[] QueryOnActions(IProperty property)
        {
            if (!QueryHasMyceliumStereotype(property, XmiResources.ManagedStereotypeName))
            {
                return [];
            }

            var actions = QueryExtensionTags(property)
                .Where(tag => string.Equals(tag.Name, XmiResources.OnActionTaggedValueName, StringComparison.Ordinal))
                .SelectMany(tag => (tag.Value ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                .ToArray();

            var unknownAction = actions.FirstOrDefault(action => action is not (XmiResources.OnCreateAction or XmiResources.OnUpdateAction));

            return unknownAction is null
                ? actions
                : throw new InvalidOperationException(
                    $"Property '{property.Describe()}' has the {XmiResources.OnActionTaggedValueName} value '{unknownAction}'; only " +
                    $"'{XmiResources.OnCreateAction}' and '{XmiResources.OnUpdateAction}' are supported.");
        }

        /// <summary>
        /// Determines whether an OCL invariant body is exactly <c>&lt;Class&gt;.allInstances()-&gt;isUnique(v | v.&lt;property&gt;)</c> for the
        /// property and the class that declares it. The body is compared as text; no OCL is parsed or evaluated.
        /// </summary>
        /// <param name="body">
        /// The decoded invariant body, possibly preceded by OCL comment lines.
        /// </param>
        /// <param name="className">
        /// The name of the class that declares the property and owns the invariant.
        /// </param>
        /// <param name="propertyName">
        /// The name of the property.
        /// </param>
        /// <returns>
        /// <see langword="true" /> when the body states the uniqueness of the property's value across every instance of its class.
        /// </returns>
        private static bool QueryIsUniquenessInvariant(string body, string className, string propertyName)
        {
            return body.Split('\n')
                .Select(line => line.Trim())
                .Where(line => line.Length > 0 && !line.StartsWith("--", StringComparison.Ordinal))
                .Select(line => UniquenessInvariantPattern().Match(line))
                .Any(match => match.Success &&
                    string.Equals(match.Groups["class"].Value, className, StringComparison.Ordinal) &&
                    string.Equals(match.Groups["property"].Value, propertyName, StringComparison.Ordinal));
        }

        /// <summary>
        /// Matches one OCL invariant of the form <c>inv &lt;name&gt;: &lt;Class&gt;.allInstances()-&gt;isUnique(v | v.&lt;property&gt;)</c>.
        /// </summary>
        /// <returns>
        /// The compiled pattern, with the <c>class</c> and <c>property</c> groups.
        /// </returns>
        [GeneratedRegex(@"^inv\s+\w+\s*:\s*(?<class>\w+)\.allInstances\(\)\s*->\s*isUnique\(\s*(?<variable>\w+)\s*\|\s*\k<variable>\.(?<property>\w+)\s*\)$", RegexOptions.CultureInvariant)]
        private static partial Regex UniquenessInvariantPattern();

        /// <param name="property">
        /// The UML property that is queried.
        /// </param>
        extension(IProperty property)
        {
            /// <summary>
            /// Determines whether the property carries the «index» stereotype of the Mycelium profile, which asks for a dedicated SQL index.
            /// </summary>
            /// <returns>
            /// <see langword="true" /> when the property is indexed.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the property is <see langword="null" />.
            /// </exception>
            public bool QueryIsIndexed()
            {
                ArgumentNullException.ThrowIfNull(property);

                return QueryHasMyceliumStereotype(property, XmiResources.IndexStereotypeName);
            }

            /// <summary>
            /// Determines whether the property carries the «managed» stereotype of the Mycelium profile: the server sets its value, so it is excluded
            /// from data validation.
            /// </summary>
            /// <returns>
            /// <see langword="true" /> when the property is managed.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the property is <see langword="null" />.
            /// </exception>
            public bool QueryIsManaged()
            {
                ArgumentNullException.ThrowIfNull(property);

                return QueryHasMyceliumStereotype(property, XmiResources.ManagedStereotypeName);
            }

            /// <summary>
            /// Determines whether the server assigns the managed property when an object is created.
            /// </summary>
            /// <returns>
            /// <see langword="true" /> when the property is managed and its <c>onAction</c> tagged value includes <c>OnCreate</c>.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the property is <see langword="null" />.
            /// </exception>
            /// <exception cref="InvalidOperationException">
            /// Thrown when the <c>onAction</c> tagged value names an unsupported action.
            /// </exception>
            public bool QueryIsAssignedOnCreate()
            {
                ArgumentNullException.ThrowIfNull(property);

                return QueryOnActions(property).Contains(XmiResources.OnCreateAction, StringComparer.Ordinal);
            }

            /// <summary>
            /// Determines whether the server assigns the managed property when an object is updated.
            /// </summary>
            /// <returns>
            /// <see langword="true" /> when the property is managed and its <c>onAction</c> tagged value includes <c>OnUpdate</c>.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the property is <see langword="null" />.
            /// </exception>
            /// <exception cref="InvalidOperationException">
            /// Thrown when the <c>onAction</c> tagged value names an unsupported action.
            /// </exception>
            public bool QueryIsAssignedOnUpdate()
            {
                ArgumentNullException.ThrowIfNull(property);

                return QueryOnActions(property).Contains(XmiResources.OnUpdateAction, StringComparer.Ordinal);
            }

            /// <summary>
            /// Determines whether the server assigns the managed property on creation only, so that an update keeps its stored value.
            /// </summary>
            /// <returns>
            /// <see langword="true" /> when the property is assigned on creation and not on update.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the property is <see langword="null" />.
            /// </exception>
            /// <exception cref="InvalidOperationException">
            /// Thrown when the <c>onAction</c> tagged value names an unsupported action.
            /// </exception>
            public bool QueryIsAssignedOnCreateOnly()
            {
                ArgumentNullException.ThrowIfNull(property);

                return property.QueryIsAssignedOnCreate() && !property.QueryIsAssignedOnUpdate();
            }

            /// <summary>
            /// Determines whether the SQL index of an indexed property is unique: its declaring class owns an invariant of the exact form
            /// <c>&lt;Class&gt;.allInstances()-&gt;isUnique(v | v.&lt;property&gt;)</c>. The invariant body is decoded once, as documentation is,
            /// and compared as text; no OCL is parsed or evaluated.
            /// </summary>
            /// <returns>
            /// <see langword="true" /> when the property is indexed and its declaring class states its uniqueness.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the property is <see langword="null" />.
            /// </exception>
            public bool QueryIsUniquelyIndexed()
            {
                ArgumentNullException.ThrowIfNull(property);

                if (!property.QueryIsIndexed() || property.Owner is not IClass owner)
                {
                    return false;
                }

                return owner.OwnedRule
                    .SelectMany(constraint => constraint.Specification.OfType<IOpaqueExpression>())
                    .SelectMany(expression => expression.Body)
                    .Any(body => QueryIsUniquenessInvariant(HtmlUtils.HtmlDecode(body), owner.Name, property.Name));
            }

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
            /// Queries the name of the DAO parameter that carries the property's stored value: <c>&lt;table&gt;_&lt;column&gt;</c> for a scalar reference
            /// column, and the link-table name for a link-table collection.
            /// </summary>
            /// <returns>
            /// The parameter name, for example <c>auditable_thing_created_by</c> or <c>functional_project_branch_rules</c>.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when the property is <see langword="null" />.
            /// </exception>
            /// <exception cref="InvalidOperationException">
            /// Thrown when the property is not owned by a class, or is a collection stored through its scalar opposite, which the DAO does not write.
            /// </exception>
            public string QuerySqlParameterName()
            {
                ArgumentNullException.ThrowIfNull(property);

                if (property.Owner is not IClass owner)
                {
                    throw new InvalidOperationException($"Property '{property.Describe()}' is not owned by a class, so it has no DAO parameter.");
                }

                if (!property.QueryIsEnumerable())
                {
                    return $"{owner.QuerySqlIdentifier()}_{property.QuerySqlIdentifier()}";
                }

                return property.QuerySqlRequiresLinkTable()
                    ? property.QuerySqlLinkTableName()
                    : throw new InvalidOperationException($"Property '{owner.Name}.{property.Name}' is stored through its scalar opposite, so its DAO writes no parameter for it.");
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
