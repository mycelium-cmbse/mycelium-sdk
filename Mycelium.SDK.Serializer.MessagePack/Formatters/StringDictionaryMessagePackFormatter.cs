// ------------------------------------------------------------------------------------------------
//  <copyright file="StringDictionaryMessagePackFormatter.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.Serializer.MessagePack
{
    using global::MessagePack;
    using global::MessagePack.Formatters;

    /// <summary>
    /// Encodes non-null string dictionaries as maps and reads keys using ordinal comparison.
    /// </summary>
    internal sealed class StringDictionaryMessagePackFormatter : MessagePackFormatterBase, IMessagePackFormatter<Dictionary<string, string>>
    {
        /// <summary>
        /// The shared formatter used directly by generated DTO formatters.
        /// </summary>
        internal static readonly StringDictionaryMessagePackFormatter Instance = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="StringDictionaryMessagePackFormatter" /> class.
        /// </summary>
        internal StringDictionaryMessagePackFormatter()
        {
        }

        /// <summary>
        /// Writes a non-null string dictionary as a map in its existing enumeration order.
        /// </summary>
        /// <param name="writer">
        /// The MessagePack writer that receives the encoded value.
        /// </param>
        /// <param name="value">
        /// The non-null dictionary whose keys and values are written.
        /// </param>
        /// <param name="options">
        /// The serializer options supplied for the operation.
        /// </param>
        /// <exception cref="MessagePackSerializationException">
        /// Thrown when <paramref name="value" /> or an entry's key or value is <see langword="null" />.
        /// </exception>
        public void Serialize(ref MessagePackWriter writer, Dictionary<string, string> value, MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                throw new MessagePackSerializationException("Dictionary value may not be null.");
            }

            writer.WriteMapHeader(value.Count);

            foreach (var entry in value)
            {
                WriteRequiredString(ref writer, entry.Key, "Dictionary key");
                WriteRequiredString(ref writer, entry.Value, "Dictionary value");
            }
        }

        /// <summary>
        /// Reads a non-null string dictionary from a map with unique, non-null string keys and values.
        /// </summary>
        /// <param name="reader">
        /// The MessagePack reader from which the encoded value is read.
        /// </param>
        /// <param name="options">
        /// The serializer options supplied for the operation.
        /// </param>
        /// <returns>
        /// The decoded dictionary using ordinal key comparison.
        /// </returns>
        /// <exception cref="MessagePackSerializationException">
        /// Thrown when the value is <c>nil</c>, is not a map, contains non-string or <c>nil</c> entries, or contains duplicate keys.
        /// </exception>
        public Dictionary<string, string> Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                throw new MessagePackSerializationException("Dictionary value may not be nil.");
            }

            var count = reader.ReadMapHeader();
            var result = new Dictionary<string, string>(count, StringComparer.Ordinal);

            for (var i = 0; i < count; i++)
            {
                var key = ReadRequiredString(ref reader, "Dictionary key");
                var value = ReadRequiredString(ref reader, "Dictionary value");

                if (!result.TryAdd(key, value))
                {
                    throw new MessagePackSerializationException($"Dictionary value contains duplicate key '{key}'.");
                }
            }

            return result;
        }
    }
}
