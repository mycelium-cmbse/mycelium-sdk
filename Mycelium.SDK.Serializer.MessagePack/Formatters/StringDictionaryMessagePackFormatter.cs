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
    /// Encodes non-null string dictionaries as maps with ordinal, unique string keys.
    /// </summary>
    internal sealed class StringDictionaryMessagePackFormatter : MessagePackFormatterBase, IMessagePackFormatter<Dictionary<string, string>>
    {
        /// <summary>
        /// The shared formatter used directly by generated DTO formatters.
        /// </summary>
        internal static readonly StringDictionaryMessagePackFormatter Instance = new();

        internal StringDictionaryMessagePackFormatter()
        {
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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
