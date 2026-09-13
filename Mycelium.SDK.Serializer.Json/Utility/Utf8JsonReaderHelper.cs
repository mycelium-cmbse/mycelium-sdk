// ------------------------------------------------------------------------------------------------
//  <copyright file="Utf8JsonReaderHelper.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.Serializer.Json.Utility
{
    using System.Text.Json;

    /// <summary>
    /// Provides strict low-level operations for generated streaming JSON deserializers.
    /// </summary>
    internal static class Utf8JsonReaderHelper
    {
        /// <summary>
        /// Defines strict low-level operations for a streaming JSON reader.
        /// </summary>
        /// <param name="reader">
        /// The JSON reader on which the operations are performed.
        /// </param>
        extension(ref Utf8JsonReader reader)
        {
            /// <summary>
            /// Verifies that the reader is positioned on the expected token.
            /// </summary>
            /// <param name="tokenType">
            /// The required token type.
            /// </param>
            /// <exception cref="JsonException">
            /// Thrown when the current token does not have the required type.
            /// </exception>
            internal void Expect(JsonTokenType tokenType)
            {
                if (reader.TokenType != tokenType)
                {
                    throw new JsonException($"Expected JSON token '{tokenType}', but found '{reader.TokenType}'.");
                }
            }

            /// <summary>
            /// Advances the reader to the next token.
            /// </summary>
            /// <exception cref="JsonException">
            /// Thrown when no complete next token is available.
            /// </exception>
            internal void ReadNext()
            {
                if (!reader.Read())
                {
                    throw new JsonException("Unexpected end of JSON input.");
                }
            }

            /// <summary>
            /// Reads a required JSON string.
            /// </summary>
            /// <returns>
            /// The decoded string.
            /// </returns>
            /// <exception cref="JsonException">
            /// Thrown when the current value is null or is not a string.
            /// </exception>
            internal string ReadRequiredString()
            {
                reader.Expect(JsonTokenType.String);

                return reader.GetString() ?? throw new JsonException("Expected a non-null JSON string.");
            }

            /// <summary>
            /// Reads a JSON string or an explicit JSON null.
            /// </summary>
            /// <returns>
            /// The decoded string, or <see langword="null" />.
            /// </returns>
            /// <exception cref="JsonException">
            /// Thrown when the current value is neither a string nor null.
            /// </exception>
            internal string ReadStringOrNull()
            {
                if (reader.TokenType == JsonTokenType.Null)
                {
                    return null;
                }

                return reader.ReadRequiredString();
            }

            /// <summary>
            /// Reads a required JSON boolean.
            /// </summary>
            /// <returns>
            /// The decoded boolean.
            /// </returns>
            /// <exception cref="JsonException">
            /// Thrown when the current value is not a boolean.
            /// </exception>
            internal bool ReadBoolean()
            {
                return reader.TokenType switch
                {
                    JsonTokenType.True => true,
                    JsonTokenType.False => false,
                    _ => throw new JsonException($"Expected a JSON boolean, but found '{reader.TokenType}'.")
                };
            }

            /// <summary>
            /// Reads a nullable JSON boolean.
            /// </summary>
            /// <returns>
            /// The decoded boolean, or <see langword="null" />.
            /// </returns>
            /// <exception cref="JsonException">
            /// Thrown when the current value is neither a boolean nor null.
            /// </exception>
            internal bool? ReadBooleanOrNull()
            {
                return reader.TokenType == JsonTokenType.Null ? null : reader.ReadBoolean();
            }

            /// <summary>
            /// Reads a required 32-bit integer.
            /// </summary>
            /// <returns>
            /// The decoded integer.
            /// </returns>
            /// <exception cref="JsonException">
            /// Thrown when the current value is not a valid 32-bit integer.
            /// </exception>
            internal int ReadInt32()
            {
                reader.Expect(JsonTokenType.Number);

                if (!reader.TryGetInt32(out var value))
                {
                    throw new JsonException("Expected a valid 32-bit integer.");
                }

                return value;
            }

            /// <summary>
            /// Reads a nullable 32-bit integer.
            /// </summary>
            /// <returns>
            /// The decoded integer, or <see langword="null" />.
            /// </returns>
            /// <exception cref="JsonException">
            /// Thrown when the current value is neither a valid 32-bit integer nor null.
            /// </exception>
            internal int? ReadInt32OrNull()
            {
                return reader.TokenType == JsonTokenType.Null ? null : reader.ReadInt32();
            }

            /// <summary>
            /// Reads a required double-precision number.
            /// </summary>
            /// <returns>
            /// The decoded number.
            /// </returns>
            /// <exception cref="JsonException">
            /// Thrown when the current value is not a valid double-precision number.
            /// </exception>
            internal double ReadDouble()
            {
                reader.Expect(JsonTokenType.Number);

                if (!reader.TryGetDouble(out var value))
                {
                    throw new JsonException("Expected a valid double-precision number.");
                }

                return value;
            }

            /// <summary>
            /// Reads a nullable double-precision number.
            /// </summary>
            /// <returns>
            /// The decoded number, or <see langword="null" />.
            /// </returns>
            /// <exception cref="JsonException">
            /// Thrown when the current value is neither a valid double-precision number nor null.
            /// </exception>
            internal double? ReadDoubleOrNull()
            {
                return reader.TokenType == JsonTokenType.Null ? null : reader.ReadDouble();
            }

            /// <summary>
            /// Reads a required valid <see cref="Guid" /> string.
            /// </summary>
            /// <returns>
            /// The decoded identifier.
            /// </returns>
            /// <exception cref="JsonException">
            /// Thrown when the current value is not a non-null string containing a valid identifier.
            /// </exception>
            internal Guid ReadGuid()
            {
                var value = reader.ReadRequiredString();

                if (!Guid.TryParse(value, out var result))
                {
                    throw new JsonException($"'{value}' is not a valid Guid value.");
                }

                return result;
            }

            /// <summary>
            /// Reads a nullable valid <see cref="Guid" /> string.
            /// </summary>
            /// <returns>
            /// The decoded identifier, or <see langword="null" />.
            /// </returns>
            /// <exception cref="JsonException">
            /// Thrown when a non-null value is not a string containing a valid identifier.
            /// </exception>
            internal Guid? ReadGuidOrNull()
            {
                return reader.TokenType == JsonTokenType.Null ? null : reader.ReadGuid();
            }

            /// <summary>
            /// Reads a required ISO-8601 <see cref="DateTime" /> string.
            /// </summary>
            /// <returns>
            /// The decoded date and time.
            /// </returns>
            /// <exception cref="JsonException">
            /// Thrown when the current value is not a valid non-null date-time string.
            /// </exception>
            internal DateTime ReadDateTime()
            {
                reader.Expect(JsonTokenType.String);

                if (!reader.TryGetDateTime(out var value))
                {
                    throw new JsonException("Expected a valid ISO-8601 date-time string.");
                }

                return value;
            }

            /// <summary>
            /// Reads a nullable ISO-8601 <see cref="DateTime" /> string.
            /// </summary>
            /// <returns>
            /// The decoded date and time, or <see langword="null" />.
            /// </returns>
            /// <exception cref="JsonException">
            /// Thrown when a non-null value is not a valid date-time string.
            /// </exception>
            internal DateTime? ReadDateTimeOrNull()
            {
                return reader.TokenType == JsonTokenType.Null ? null : reader.ReadDateTime();
            }

            /// <summary>
            /// Reads a JSON object containing string keys and non-null string values.
            /// </summary>
            /// <returns>
            /// The decoded ordinal dictionary.
            /// </returns>
            /// <exception cref="JsonException">
            /// Thrown for a non-object value, malformed object member, duplicate key, null value, or
            /// non-string value.
            /// </exception>
            internal Dictionary<string, string> ReadStringDictionary()
            {
                reader.Expect(JsonTokenType.StartObject);

                var result = new Dictionary<string, string>(StringComparer.Ordinal);

                while (true)
                {
                    reader.ReadNext();

                    if (reader.TokenType == JsonTokenType.EndObject)
                    {
                        return result;
                    }

                    reader.Expect(JsonTokenType.PropertyName);

                    var key = reader.GetString() ?? throw new JsonException("Expected a non-null dictionary key.");

                    reader.ReadNext();

                    var value = reader.ReadRequiredString();

                    if (!result.TryAdd(key, value))
                    {
                        throw new JsonException($"Duplicate dictionary key '{key}'.");
                    }
                }
            }

            /// <summary>
            /// Skips the current JSON value, including a nested object or array.
            /// </summary>
            internal void SkipValue()
            {
                reader.Skip();
            }
        }
    }
}
