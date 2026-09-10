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
        /// Verifies that the reader is positioned on the expected token.
        /// </summary>
        /// <param name="reader">
        /// The reader positioned on the token to verify.
        /// </param>
        /// <param name="tokenType">
        /// The required token type.
        /// </param>
        /// <exception cref="JsonException">
        /// Thrown when the current token does not have the required type.
        /// </exception>
        internal static void Expect(ref Utf8JsonReader reader, JsonTokenType tokenType)
        {
            if (reader.TokenType != tokenType)
            {
                throw new JsonException($"Expected JSON token '{tokenType}', but found '{reader.TokenType}'.");
            }
        }

        /// <summary>
        /// Advances the reader to the next token.
        /// </summary>
        /// <param name="reader">
        /// The reader to advance.
        /// </param>
        /// <exception cref="JsonException">
        /// Thrown when no complete next token is available.
        /// </exception>
        internal static void ReadNext(ref Utf8JsonReader reader)
        {
            if (!reader.Read())
            {
                throw new JsonException("Unexpected end of JSON input.");
            }
        }

        /// <summary>
        /// Reads a required JSON string.
        /// </summary>
        /// <param name="reader">
        /// The reader positioned on the value token.
        /// </param>
        /// <returns>
        /// The decoded string.
        /// </returns>
        /// <exception cref="JsonException">
        /// Thrown when the current value is null or is not a string.
        /// </exception>
        internal static string ReadRequiredString(ref Utf8JsonReader reader)
        {
            Expect(ref reader, JsonTokenType.String);

            return reader.GetString() ?? throw new JsonException("Expected a non-null JSON string.");
        }

        /// <summary>
        /// Reads a JSON string or an explicit JSON null.
        /// </summary>
        /// <param name="reader">
        /// The reader positioned on the value token.
        /// </param>
        /// <returns>
        /// The decoded string, or <see langword="null" />.
        /// </returns>
        /// <exception cref="JsonException">
        /// Thrown when the current value is neither a string nor null.
        /// </exception>
        internal static string ReadStringOrNull(ref Utf8JsonReader reader)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            return ReadRequiredString(ref reader);
        }

        /// <summary>
        /// Reads a required JSON boolean.
        /// </summary>
        /// <param name="reader">
        /// The reader positioned on the value token.
        /// </param>
        /// <returns>
        /// The decoded boolean.
        /// </returns>
        /// <exception cref="JsonException">
        /// Thrown when the current value is not a boolean.
        /// </exception>
        internal static bool ReadBoolean(ref Utf8JsonReader reader)
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
        /// <param name="reader">
        /// The reader positioned on the value token.
        /// </param>
        /// <returns>
        /// The decoded boolean, or <see langword="null" />.
        /// </returns>
        /// <exception cref="JsonException">
        /// Thrown when the current value is neither a boolean nor null.
        /// </exception>
        internal static bool? ReadBooleanOrNull(ref Utf8JsonReader reader)
        {
            return reader.TokenType == JsonTokenType.Null ? null : ReadBoolean(ref reader);
        }

        /// <summary>
        /// Reads a required 32-bit integer.
        /// </summary>
        /// <param name="reader">
        /// The reader positioned on the value token.
        /// </param>
        /// <returns>
        /// The decoded integer.
        /// </returns>
        /// <exception cref="JsonException">
        /// Thrown when the current value is not a valid 32-bit integer.
        /// </exception>
        internal static int ReadInt32(ref Utf8JsonReader reader)
        {
            Expect(ref reader, JsonTokenType.Number);

            if (!reader.TryGetInt32(out var value))
            {
                throw new JsonException("Expected a valid 32-bit integer.");
            }

            return value;
        }

        /// <summary>
        /// Reads a nullable 32-bit integer.
        /// </summary>
        /// <param name="reader">
        /// The reader positioned on the value token.
        /// </param>
        /// <returns>
        /// The decoded integer, or <see langword="null" />.
        /// </returns>
        /// <exception cref="JsonException">
        /// Thrown when the current value is neither a valid 32-bit integer nor null.
        /// </exception>
        internal static int? ReadInt32OrNull(ref Utf8JsonReader reader)
        {
            return reader.TokenType == JsonTokenType.Null ? null : ReadInt32(ref reader);
        }

        /// <summary>
        /// Reads a required double-precision number.
        /// </summary>
        /// <param name="reader">
        /// The reader positioned on the value token.
        /// </param>
        /// <returns>
        /// The decoded number.
        /// </returns>
        /// <exception cref="JsonException">
        /// Thrown when the current value is not a valid double-precision number.
        /// </exception>
        internal static double ReadDouble(ref Utf8JsonReader reader)
        {
            Expect(ref reader, JsonTokenType.Number);

            if (!reader.TryGetDouble(out var value))
            {
                throw new JsonException("Expected a valid double-precision number.");
            }

            return value;
        }

        /// <summary>
        /// Reads a nullable double-precision number.
        /// </summary>
        /// <param name="reader">
        /// The reader positioned on the value token.
        /// </param>
        /// <returns>
        /// The decoded number, or <see langword="null" />.
        /// </returns>
        /// <exception cref="JsonException">
        /// Thrown when the current value is neither a valid double-precision number nor null.
        /// </exception>
        internal static double? ReadDoubleOrNull(ref Utf8JsonReader reader)
        {
            return reader.TokenType == JsonTokenType.Null ? null : ReadDouble(ref reader);
        }

        /// <summary>
        /// Reads a required valid <see cref="Guid" /> string.
        /// </summary>
        /// <param name="reader">
        /// The reader positioned on the value token.
        /// </param>
        /// <returns>
        /// The decoded identifier.
        /// </returns>
        /// <exception cref="JsonException">
        /// Thrown when the current value is not a non-null string containing a valid identifier.
        /// </exception>
        internal static Guid ReadGuid(ref Utf8JsonReader reader)
        {
            var value = ReadRequiredString(ref reader);

            if (!Guid.TryParse(value, out var result))
            {
                throw new JsonException($"'{value}' is not a valid Guid value.");
            }

            return result;
        }

        /// <summary>
        /// Reads a nullable valid <see cref="Guid" /> string.
        /// </summary>
        /// <param name="reader">
        /// The reader positioned on the value token.
        /// </param>
        /// <returns>
        /// The decoded identifier, or <see langword="null" />.
        /// </returns>
        /// <exception cref="JsonException">
        /// Thrown when a non-null value is not a string containing a valid identifier.
        /// </exception>
        internal static Guid? ReadGuidOrNull(ref Utf8JsonReader reader)
        {
            return reader.TokenType == JsonTokenType.Null ? null : ReadGuid(ref reader);
        }

        /// <summary>
        /// Reads a required ISO-8601 <see cref="DateTime" /> string.
        /// </summary>
        /// <param name="reader">
        /// The reader positioned on the value token.
        /// </param>
        /// <returns>
        /// The decoded date and time.
        /// </returns>
        /// <exception cref="JsonException">
        /// Thrown when the current value is not a valid non-null date-time string.
        /// </exception>
        internal static DateTime ReadDateTime(ref Utf8JsonReader reader)
        {
            Expect(ref reader, JsonTokenType.String);

            if (!reader.TryGetDateTime(out var value))
            {
                throw new JsonException("Expected a valid ISO-8601 date-time string.");
            }

            return value;
        }

        /// <summary>
        /// Reads a nullable ISO-8601 <see cref="DateTime" /> string.
        /// </summary>
        /// <param name="reader">
        /// The reader positioned on the value token.
        /// </param>
        /// <returns>
        /// The decoded date and time, or <see langword="null" />.
        /// </returns>
        /// <exception cref="JsonException">
        /// Thrown when a non-null value is not a valid date-time string.
        /// </exception>
        internal static DateTime? ReadDateTimeOrNull(ref Utf8JsonReader reader)
        {
            return reader.TokenType == JsonTokenType.Null ? null : ReadDateTime(ref reader);
        }

        /// <summary>
        /// Reads a JSON object containing string keys and non-null string values.
        /// </summary>
        /// <param name="reader">
        /// The reader positioned on the object-start token.
        /// </param>
        /// <returns>
        /// The decoded ordinal dictionary.
        /// </returns>
        /// <exception cref="JsonException">
        /// Thrown for a non-object value, malformed object member, duplicate key, null value, or
        /// non-string value.
        /// </exception>
        internal static Dictionary<string, string> ReadStringDictionary(ref Utf8JsonReader reader)
        {
            Expect(ref reader, JsonTokenType.StartObject);

            var result = new Dictionary<string, string>(StringComparer.Ordinal);

            while (true)
            {
                ReadNext(ref reader);

                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return result;
                }

                Expect(ref reader, JsonTokenType.PropertyName);

                var key = reader.GetString() ?? throw new JsonException("Expected a non-null dictionary key.");

                ReadNext(ref reader);

                var value = ReadRequiredString(ref reader);

                if (!result.TryAdd(key, value))
                {
                    throw new JsonException($"Duplicate dictionary key '{key}'.");
                }
            }
        }

        /// <summary>
        /// Skips the current JSON value, including a nested object or array.
        /// </summary>
        /// <param name="reader">
        /// The reader positioned on the value to skip.
        /// </param>
        internal static void SkipValue(ref Utf8JsonReader reader)
        {
            reader.Skip();
        }
    }
}
