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
    using System.Linq;
    using System.Text;

    using HandlebarsDotNet;

    using Mycelium.SDK.CodeGenerator.Extensions;

    using uml4net.Classification;
    using uml4net.Extensions;
    using uml4net.SimpleClassifiers;
    using uml4net.StructuredClassifiers;

    /// <summary>
    /// Provides Handlebars support for UML properties used to generate DTOs, POCOs,
    /// JSON serializers, and MessagePack formatters.
    /// </summary>
    public static class PropertyHelper
    {
        /// <summary>
        /// The generated statement that writes a MessagePack nil value.
        /// </summary>
        private const string MessagePackWriteNilStatement = "writer.WriteNil();";

        /// <summary>
        /// The generated condition that reads a MessagePack nil value.
        /// </summary>
        private const string MessagePackTryReadNilCondition = "if (reader.TryReadNil())";

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

            handlebars.RegisterHelper("Property.WriteDtoInterfaceDeclaration", (writer, _, arguments) =>
            {
                var property = QueryProperty(arguments, "{{Property.WriteDtoInterfaceDeclaration}}");

                writer.WriteSafeString($"{property.QueryDtoTypeName()} " + $"{property.QueryPropertyName()} {QueryAccessors(property)}");
            });

            handlebars.RegisterHelper("Property.WriteDtoImplementationDeclaration", (writer, _, arguments) =>
            {
                var property = QueryProperty(arguments, "{{Property.WriteDtoImplementationDeclaration}}");
                var propertyTypeName = property.QueryDtoTypeName();
                var collectionInitializer = QueryCollectionInitializer(property, propertyTypeName);

                writer.WriteSafeString($"public {propertyTypeName} " + $"{property.QueryPropertyName()} {QueryAccessors(property)}" + collectionInitializer);
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
        public static void RegisterJsonSerializerPropertyHelper(this IHandlebars handlebars)
        {
            ArgumentNullException.ThrowIfNull(handlebars);

            handlebars.RegisterHelper("Property.QueryIsIdentifier", (_, arguments) =>
            {
                var property = QueryProperty(arguments, "{{Property.QueryIsIdentifier}}");

                return string.Equals(property.Name, "id", StringComparison.Ordinal) && string.Equals(property.QueryDtoTypeName(), "Guid", StringComparison.Ordinal);
            });

            handlebars.RegisterHelper("Property.QueryIsStringDictionary", (_, arguments) =>
            {
                var property = QueryProperty(arguments, "{{Property.QueryIsStringDictionary}}");

                return string.Equals(property.QueryDtoTypeName(), "Dictionary<string,string>", StringComparison.Ordinal);
            });

            handlebars.RegisterHelper("Property.WritePropertyName", (writer, _, arguments) =>
            {
                var property = QueryProperty(arguments, "{{Property.WritePropertyName}}");

                writer.WriteSafeString(property.QueryPropertyName());
            });
        }

        /// <summary>
        /// Registers MessagePack property-rendering helpers independently of the DTO, POCO, and
        /// JSON helpers.
        /// </summary>
        /// <param name="handlebars">
        /// The Handlebars environment in which the MessagePack property helpers are registered.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="handlebars" /> is <see langword="null" />.
        /// </exception>
        public static void RegisterMessagePackFormatterPropertyHelper(this IHandlebars handlebars)
        {
            ArgumentNullException.ThrowIfNull(handlebars);

            handlebars.RegisterHelper("Property.WriteMessagePackSerialization", (writer, _, arguments) =>
            {
                var property = QueryProperty(arguments, "{{Property.WriteMessagePackSerialization}}");

                writer.WriteSafeString(RenderMessagePackSerialization(property));
            });

            handlebars.RegisterHelper("Property.WriteMessagePackDeserialization", (writer, _, arguments) =>
            {
                var property = QueryProperty(arguments, "{{Property.WriteMessagePackDeserialization}}");

                writer.WriteSafeString(RenderMessagePackDeserialization(property));
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

            handlebars.RegisterHelper("Property.WritePocoInterfaceDeclaration", (writer, _, arguments) =>
            {
                var property = QueryProperty(arguments, "{{Property.WritePocoInterfaceDeclaration}}");

                writer.WriteSafeString($"{property.QueryPocoTypeName()} " + $"{property.QueryPropertyName()} {QueryAccessors(property)}");
            });

            handlebars.RegisterHelper("Property.WritePocoImplementationDeclaration", (writer, _, arguments) =>
            {
                var property = QueryProperty(arguments, "{{Property.WritePocoImplementationDeclaration}}");
                var propertyTypeName = property.QueryPocoTypeName();
                var propertyName = property.QueryPropertyName();

                writer.WriteSafeString($"public {propertyTypeName} {propertyName} " + QueryPocoImplementationSuffix(property, propertyName, propertyTypeName));
            });
        }

        /// <summary>
        /// Renders serialization statements for one MessagePack DTO property.
        /// </summary>
        /// <param name="property">
        /// The property whose serialization statements are rendered.
        /// </param>
        /// <returns>
        /// The rendered serialization statements.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="property" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the property has no resolved type or uses a type outside the approved mapping.
        /// </exception>
        private static string RenderMessagePackSerialization(IProperty property)
        {
            ArgumentNullException.ThrowIfNull(property);

            var builder = new StringBuilder();
            var propertyName = property.QueryPropertyName();
            var valueExpression = $"dto.{propertyName}";

            if (!property.QueryIsEnumerable())
            {
                AppendSerializeValue(builder, property, valueExpression, property.QueryIsNullable(), propertyName, 0);

                return builder.ToString();
            }

            AppendLine(builder, 0, $"if ({valueExpression} == null)");
            AppendLine(builder, 0, "{");
            AppendLine(builder, 1, MessagePackWriteNilStatement);
            AppendLine(builder, 0, "}");
            AppendLine(builder, 0, "else");
            AppendLine(builder, 0, "{");
            AppendLine(builder, 1, $"writer.WriteArrayHeader({valueExpression}.Count);");
            AppendLine(builder, 1, string.Empty);
            AppendLine(builder, 1, $"for (var i = 0; i < {valueExpression}.Count; i++)");
            AppendLine(builder, 1, "{");
            AppendSerializeValue(builder, property, $"{valueExpression}[i]", false, $"{propertyName} item", 2);
            AppendLine(builder, 1, "}");
            AppendLine(builder, 0, "}");

            return builder.ToString();
        }

        /// <summary>
        /// Renders deserialization statements for one MessagePack DTO property.
        /// </summary>
        /// <param name="property">
        /// The property whose deserialization statements are rendered.
        /// </param>
        /// <returns>
        /// The rendered deserialization statements.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="property" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the property has no resolved type or uses a type outside the approved mapping.
        /// </exception>
        private static string RenderMessagePackDeserialization(IProperty property)
        {
            ArgumentNullException.ThrowIfNull(property);

            var builder = new StringBuilder();
            var propertyName = property.QueryPropertyName();
            var destination = $"dto.{propertyName}";
            var localName = $"messagePack{propertyName.TrimStart('@')}Value";

            if (!property.QueryIsEnumerable())
            {
                AppendDeserializeValue(builder, property, new DeserializationContext(destination, false, property.QueryIsNullable(), propertyName, localName, 0));

                return builder.ToString();
            }

            var countName = $"messagePack{propertyName.TrimStart('@')}Count";

            AppendLine(builder, 0, MessagePackTryReadNilCondition);
            AppendLine(builder, 0, "{");
            AppendLine(builder, 1, $"{destination} = null;");
            AppendLine(builder, 0, "}");
            AppendLine(builder, 0, "else");
            AppendLine(builder, 0, "{");
            AppendLine(builder, 1, $"var {countName} = reader.ReadArrayHeader();");
            AppendLine(builder, 1, $"{destination}.Clear();");
            AppendLine(builder, 1, string.Empty);
            AppendLine(builder, 1, $"if ({destination}.Capacity < {countName})");
            AppendLine(builder, 1, "{");
            AppendLine(builder, 2, $"{destination}.Capacity = {countName};");
            AppendLine(builder, 1, "}");
            AppendLine(builder, 1, string.Empty);
            AppendLine(builder, 1, $"for (var i = 0; i < {countName}; i++)");
            AppendLine(builder, 1, "{");
            AppendDeserializeValue(builder, property, new DeserializationContext(destination, true, false, $"{propertyName} item", localName, 2));
            AppendLine(builder, 1, "}");
            AppendLine(builder, 0, "}");

            return builder.ToString();
        }

        /// <summary>
        /// Appends serialization statements for one scalar value or collection element.
        /// </summary>
        /// <param name="builder">
        /// The target source builder.
        /// </param>
        /// <param name="property">
        /// The modeled property defining the value representation.
        /// </param>
        /// <param name="valueExpression">
        /// The generated expression that supplies the value.
        /// </param>
        /// <param name="nullable">
        /// Whether the value may be encoded as <c>nil</c>.
        /// </param>
        /// <param name="valueDescription">
        /// The value description used in generated validation messages.
        /// </param>
        /// <param name="indentationLevel">
        /// The generated indentation level.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the property has no resolved type or uses a type outside the approved mapping.
        /// </exception>
        private static void AppendSerializeValue(StringBuilder builder, IProperty property, string valueExpression, bool nullable, string valueDescription, int indentationLevel)
        {
            if (property.Type is IClass || property.Type is IPrimitiveType { Name: "Guid" })
            {
                if (nullable)
                {
                    AppendLine(builder, indentationLevel, $"if ({valueExpression}.HasValue)");
                    AppendLine(builder, indentationLevel, "{");
                    AppendLine(builder, indentationLevel + 1, $"GuidMessagePackFormatter.Instance.Serialize(ref writer, {valueExpression}.Value, options);");
                    AppendLine(builder, indentationLevel, "}");
                    AppendLine(builder, indentationLevel, "else");
                    AppendLine(builder, indentationLevel, "{");
                    AppendLine(builder, indentationLevel + 1, MessagePackWriteNilStatement);
                    AppendLine(builder, indentationLevel, "}");
                }
                else
                {
                    AppendLine(builder, indentationLevel, $"GuidMessagePackFormatter.Instance.Serialize(ref writer, {valueExpression}, options);");
                }

                return;
            }

            if (property.Type is IEnumeration enumeration)
            {
                AppendSerializeEnumeration(builder, enumeration, valueExpression, nullable, indentationLevel);

                return;
            }

            if (property.Type is not IPrimitiveType primitiveType)
            {
                throw new InvalidOperationException($"Property '{property.Describe()}' has no supported MessagePack type.");
            }

            switch (primitiveType.Name)
            {
                case "Boolean":
                case "Integer":
                case "Real":
                    AppendSerializeNativeValue(builder, valueExpression, nullable, indentationLevel);
                    break;
                case "DateTime":
                    AppendSerializeRoundTripDateTime(builder, valueExpression, nullable, indentationLevel);
                    break;
                case "Dictionary<string,string>":
                    AppendSerializeReferenceValue(builder, valueExpression, !property.QueryIsEnumerable(), valueDescription, "StringDictionaryMessagePackFormatter", "Dictionary", indentationLevel);
                    break;
                case "String":
                case "UnlimitedNatural":

                    if (nullable)
                    {
                        AppendLine(builder, indentationLevel, $"writer.Write({valueExpression});");
                    }
                    else
                    {
                        AppendLine(builder, indentationLevel, $"WriteRequiredString(ref writer, {valueExpression}, \"{valueDescription}\");");
                    }

                    break;
                case "Uri":
                    AppendSerializeReferenceValue(builder, valueExpression, nullable, valueDescription, "UriMessagePackFormatter", "URI", indentationLevel);
                    break;
                default:
                    throw new InvalidOperationException($"Property '{property.Describe()}' uses unsupported MessagePack primitive '{primitiveType.Name}'.");
            }
        }

        /// <summary>
        /// Appends deserialization statements for one scalar value or collection element.
        /// </summary>
        /// <param name="builder">
        /// The target source builder.
        /// </param>
        /// <param name="property">
        /// The modeled property defining the value representation.
        /// </param>
        /// <param name="context">
        /// The destination, value validation and generated code context.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the property has no resolved type or uses a type outside the approved mapping.
        /// </exception>
        private static void AppendDeserializeValue(StringBuilder builder, IProperty property, DeserializationContext context)
        {
            var (destination, addToCollection, nullable, valueDescription, _, indentationLevel) = context;

            if (property.Type is IClass || property.Type is IPrimitiveType { Name: "Guid" })
            {
                AppendDeserializeNativeValue(builder, destination, addToCollection, nullable, "GuidMessagePackFormatter.Instance.Deserialize(ref reader, options)", indentationLevel);

                return;
            }

            if (property.Type is IEnumeration enumeration)
            {
                AppendDeserializeEnumeration(builder, enumeration, context);

                return;
            }

            if (property.Type is not IPrimitiveType primitiveType)
            {
                throw new InvalidOperationException($"Property '{property.Describe()}' has no supported MessagePack type.");
            }

            switch (primitiveType.Name)
            {
                case "Boolean":
                    AppendDeserializeNativeValue(builder, destination, addToCollection, nullable, "reader.ReadBoolean()", indentationLevel);
                    break;
                case "DateTime":
                    AppendDeserializeNativeValue(builder, destination, addToCollection, nullable, $"ReadRoundTripDateTime(ref reader, \"{valueDescription}\")", indentationLevel);
                    break;
                case "Dictionary<string,string>":
                    AppendDeserializeReferenceValue(builder, context, !property.QueryIsEnumerable(), "StringDictionaryMessagePackFormatter", "Dictionary");

                    break;
                case "Integer":
                    AppendDeserializeNativeValue(builder, destination, addToCollection, nullable, "reader.ReadInt32()", indentationLevel);
                    break;
                case "Real":
                    AppendDeserializeNativeValue(builder, destination, addToCollection, nullable, "reader.ReadDouble()", indentationLevel);
                    break;
                case "String":
                case "UnlimitedNatural":

                    AppendAssignment(builder, destination, addToCollection, nullable ? "reader.ReadString()" : $"ReadRequiredString(ref reader, \"{valueDescription}\")", indentationLevel);

                    break;
                case "Uri":

                    AppendDeserializeReferenceValue(builder, context, nullable, "UriMessagePackFormatter", "URI");

                    break;
                default:
                    throw new InvalidOperationException($"Property '{property.Describe()}' uses unsupported MessagePack primitive '{primitiveType.Name}'.");
            }
        }

        /// <summary>
        /// Appends reference-value delegation after handling null according to the DTO contract.
        /// </summary>
        /// <param name="builder">The target source builder.</param>
        /// <param name="valueExpression">The generated expression supplying the value.</param>
        /// <param name="nullable">Whether null is permitted.</param>
        /// <param name="valueDescription">The property or collection item description.</param>
        /// <param name="formatterName">The dedicated formatter type.</param>
        /// <param name="typeDescription">The type description used for required-value errors.</param>
        /// <param name="indentationLevel">The generated indentation level.</param>
        private static void AppendSerializeReferenceValue(StringBuilder builder, string valueExpression, bool nullable, string valueDescription, string formatterName, string typeDescription, int indentationLevel)
        {
            AppendLine(builder, indentationLevel, $"if ({valueExpression} == null)");
            AppendLine(builder, indentationLevel, "{");

            if (nullable)
            {
                AppendLine(builder, indentationLevel + 1, MessagePackWriteNilStatement);
            }
            else
            {
                AppendLine(builder, indentationLevel + 1, $"throw new MessagePackSerializationException(\"{typeDescription} value '{valueDescription}' may not be null.\");");
            }

            AppendLine(builder, indentationLevel, "}");
            AppendLine(builder, indentationLevel, "else");
            AppendLine(builder, indentationLevel, "{");
            AppendLine(builder, indentationLevel + 1, $"{formatterName}.Instance.Serialize(ref writer, {valueExpression}, options);");
            AppendLine(builder, indentationLevel, "}");
        }

        /// <summary>
        /// Appends reference-value delegation after handling nil according to the DTO contract.
        /// </summary>
        /// <param name="builder">The target source builder.</param>
        /// <param name="context">The destination and value validation context.</param>
        /// <param name="nullable">Whether nil is permitted.</param>
        /// <param name="formatterName">The dedicated formatter type.</param>
        /// <param name="typeDescription">The type description used for required-value errors.</param>
        private static void AppendDeserializeReferenceValue(StringBuilder builder, DeserializationContext context, bool nullable, string formatterName, string typeDescription)
        {
            var (destination, addToCollection, _, valueDescription, _, indentationLevel) = context;

            AppendLine(builder, indentationLevel, MessagePackTryReadNilCondition);
            AppendLine(builder, indentationLevel, "{");

            if (nullable)
            {
                AppendAssignment(builder, destination, addToCollection, "null", indentationLevel + 1);
            }
            else
            {
                AppendLine(builder, indentationLevel + 1, $"throw new MessagePackSerializationException(\"{typeDescription} value '{valueDescription}' may not be nil.\");");
            }

            AppendLine(builder, indentationLevel, "}");
            AppendLine(builder, indentationLevel, "else");
            AppendLine(builder, indentationLevel, "{");
            AppendAssignment(builder, destination, addToCollection, $"{formatterName}.Instance.Deserialize(ref reader, options)", indentationLevel + 1);
            AppendLine(builder, indentationLevel, "}");
        }

        /// <summary>
        /// Appends exact lowercase enumeration serialization.
        /// </summary>
        /// <param name="builder">
        /// The target source builder.
        /// </param>
        /// <param name="enumeration">
        /// The modeled enumeration.
        /// </param>
        /// <param name="valueExpression">
        /// The generated expression supplying the enumeration value.
        /// </param>
        /// <param name="nullable">
        /// Whether the value may be encoded as <c>nil</c>.
        /// </param>
        /// <param name="indentationLevel">
        /// The generated indentation level.
        /// </param>
        private static void AppendSerializeEnumeration(StringBuilder builder, IEnumeration enumeration, string valueExpression, bool nullable, int indentationLevel)
        {
            var enumerationName = ReservedCSharpNameMapper.Map(enumeration.Name);
            var formatterName = $"{enumerationName}MessagePackFormatter.Instance";

            if (nullable)
            {
                AppendLine(builder, indentationLevel, $"if ({valueExpression}.HasValue)");
                AppendLine(builder, indentationLevel, "{");
                AppendLine(builder, indentationLevel + 1, $"{formatterName}.Serialize(ref writer, {valueExpression}.Value, options);");
                AppendLine(builder, indentationLevel, "}");
                AppendLine(builder, indentationLevel, "else");
                AppendLine(builder, indentationLevel, "{");
                AppendLine(builder, indentationLevel + 1, MessagePackWriteNilStatement);
                AppendLine(builder, indentationLevel, "}");

                return;
            }

            AppendLine(builder, indentationLevel, $"{formatterName}.Serialize(ref writer, {valueExpression}, options);");
        }

        /// <summary>
        /// Appends exact lowercase enumeration deserialization.
        /// </summary>
        /// <param name="builder">
        /// The target source builder.
        /// </param>
        /// <param name="enumeration">
        /// The modeled enumeration.
        /// </param>
        /// <param name="context">
        /// The destination, value validation and generated code context.
        /// </param>
        private static void AppendDeserializeEnumeration(StringBuilder builder, IEnumeration enumeration, DeserializationContext context)
        {
            var (destination, addToCollection, nullable, _, _, indentationLevel) = context;
            var enumerationName = ReservedCSharpNameMapper.Map(enumeration.Name);
            var formatterCall = $"{enumerationName}MessagePackFormatter.Instance.Deserialize(ref reader, options)";

            if (nullable)
            {
                AppendLine(builder, indentationLevel, MessagePackTryReadNilCondition);
                AppendLine(builder, indentationLevel, "{");
                AppendAssignment(builder, destination, addToCollection, "null", indentationLevel + 1);
                AppendLine(builder, indentationLevel, "}");
                AppendLine(builder, indentationLevel, "else");
                AppendLine(builder, indentationLevel, "{");
                AppendAssignment(builder, destination, addToCollection, formatterCall, indentationLevel + 1);
                AppendLine(builder, indentationLevel, "}");

                return;
            }

            AppendAssignment(builder, destination, addToCollection, formatterCall, indentationLevel);
        }

        /// <summary>
        /// Groups the generated deserialization destination and value context.
        /// </summary>
        private readonly record struct DeserializationContext(string Destination, bool AddToCollection, bool Nullable, string ValueDescription, string LocalName, int IndentationLevel);

        /// <summary>
        /// Appends invariant round-trip date and time serialization with optional <c>nil</c> handling.
        /// </summary>
        /// <param name="builder">
        /// The target source builder.
        /// </param>
        /// <param name="valueExpression">
        /// The generated expression supplying the date and time.
        /// </param>
        /// <param name="nullable">
        /// Whether the value may be encoded as <c>nil</c>.
        /// </param>
        /// <param name="indentationLevel">
        /// The generated indentation level.
        /// </param>
        private static void AppendSerializeRoundTripDateTime(StringBuilder builder, string valueExpression, bool nullable, int indentationLevel)
        {
            if (!nullable)
            {
                AppendLine(builder, indentationLevel, $"WriteRoundTripDateTime(ref writer, {valueExpression});");

                return;
            }

            AppendLine(builder, indentationLevel, $"if ({valueExpression}.HasValue)");
            AppendLine(builder, indentationLevel, "{");
            AppendLine(builder, indentationLevel + 1, $"WriteRoundTripDateTime(ref writer, {valueExpression}.Value);");
            AppendLine(builder, indentationLevel, "}");
            AppendLine(builder, indentationLevel, "else");
            AppendLine(builder, indentationLevel, "{");
            AppendLine(builder, indentationLevel + 1, MessagePackWriteNilStatement);
            AppendLine(builder, indentationLevel, "}");
        }

        /// <summary>
        /// Appends native scalar serialization with optional <c>nil</c> handling.
        /// </summary>
        /// <param name="builder">
        /// The target source builder.
        /// </param>
        /// <param name="valueExpression">
        /// The generated expression supplying the value.
        /// </param>
        /// <param name="nullable">
        /// Whether the value may be encoded as <c>nil</c>.
        /// </param>
        /// <param name="indentationLevel">
        /// The generated indentation level.
        /// </param>
        private static void AppendSerializeNativeValue(StringBuilder builder, string valueExpression, bool nullable, int indentationLevel)
        {
            if (!nullable)
            {
                AppendLine(builder, indentationLevel, $"writer.Write({valueExpression});");

                return;
            }

            AppendLine(builder, indentationLevel, $"if ({valueExpression}.HasValue)");
            AppendLine(builder, indentationLevel, "{");
            AppendLine(builder, indentationLevel + 1, $"writer.Write({valueExpression}.Value);");
            AppendLine(builder, indentationLevel, "}");
            AppendLine(builder, indentationLevel, "else");
            AppendLine(builder, indentationLevel, "{");
            AppendLine(builder, indentationLevel + 1, MessagePackWriteNilStatement);
            AppendLine(builder, indentationLevel, "}");
        }

        /// <summary>
        /// Appends native scalar deserialization with optional <c>nil</c> handling.
        /// </summary>
        /// <param name="builder">
        /// The target source builder.
        /// </param>
        /// <param name="destination">
        /// The generated DTO property receiving the value.
        /// </param>
        /// <param name="addToCollection">
        /// Whether the value is added to a collection instead of assigned.
        /// </param>
        /// <param name="nullable">
        /// Whether the value may be encoded as <c>nil</c>.
        /// </param>
        /// <param name="readExpression">
        /// The MessagePack reader expression that obtains the value.
        /// </param>
        /// <param name="indentationLevel">
        /// The generated indentation level.
        /// </param>
        private static void AppendDeserializeNativeValue(StringBuilder builder, string destination, bool addToCollection, bool nullable, string readExpression, int indentationLevel)
        {
            if (!nullable)
            {
                AppendAssignment(builder, destination, addToCollection, readExpression, indentationLevel);

                return;
            }

            AppendLine(builder, indentationLevel, MessagePackTryReadNilCondition);
            AppendLine(builder, indentationLevel, "{");
            AppendAssignment(builder, destination, addToCollection, "null", indentationLevel + 1);
            AppendLine(builder, indentationLevel, "}");
            AppendLine(builder, indentationLevel, "else");
            AppendLine(builder, indentationLevel, "{");
            AppendAssignment(builder, destination, addToCollection, readExpression, indentationLevel + 1);
            AppendLine(builder, indentationLevel, "}");
        }

        /// <summary>
        /// Appends a generated scalar assignment or collection insertion.
        /// </summary>
        /// <param name="builder">
        /// The target source builder.
        /// </param>
        /// <param name="destination">
        /// The generated DTO property receiving the value.
        /// </param>
        /// <param name="addToCollection">
        /// Whether the value is added to a collection instead of assigned.
        /// </param>
        /// <param name="valueExpression">
        /// The expression that supplies the deserialized value.
        /// </param>
        /// <param name="indentationLevel">
        /// The generated indentation level.
        /// </param>
        private static void AppendAssignment(StringBuilder builder, string destination, bool addToCollection, string valueExpression, int indentationLevel)
        {
            AppendLine(builder, indentationLevel, addToCollection ? $"{destination}.Add({valueExpression});" : $"{destination} = {valueExpression};");
        }

        /// <summary>
        /// Appends one deterministically indented generated source line.
        /// </summary>
        /// <param name="builder">
        /// The target source builder.
        /// </param>
        /// <param name="indentationLevel">
        /// The generated indentation level.
        /// </param>
        /// <param name="line">
        /// The line content without leading indentation.
        /// </param>
        private static void AppendLine(StringBuilder builder, int indentationLevel, string line)
        {
            builder.Append(' ', indentationLevel * 4);
            builder.AppendLine(line);
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
        private static string QueryAccessors(IProperty property) => QueryIsDerived(property) ? "{ get; }" : "{ get; set; }";

        /// <summary>
        /// Determines whether the UML property represents derived state.
        /// </summary>
        /// <param name="property">
        /// The UML property.
        /// </param>
        /// <returns>
        /// <see langword="true" /> for a derived or derived-union property.
        /// </returns>
        private static bool QueryIsDerived(IProperty property) => property.IsDerived || property.IsDerivedUnion;

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
            return property.QueryIsEnumerable() || propertyTypeName.StartsWith("Dictionary<", StringComparison.Ordinal) ? " = [];" : string.Empty;
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
