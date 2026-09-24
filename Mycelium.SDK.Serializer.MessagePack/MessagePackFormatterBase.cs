// ------------------------------------------------------------------------------------------------
//  <copyright file="MessagePackFormatterBase.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.Serializer.MessagePack
{
    using System.Buffers;

    using global::MessagePack;

    /// <summary>
    /// Provides the shared wire-format operations used by generated MessagePack DTO formatters.
    /// </summary>
    public abstract class MessagePackFormatterBase
    {
        /// <summary>
        /// The thread-local buffer used to encode identifiers without allocating per value.
        /// </summary>
        [ThreadStatic]
        private static byte[]? guidBuffer;

        /// <summary>
        /// Writes an identifier as a MessagePack binary value containing exactly 16 bytes.
        /// </summary>
        /// <param name="writer">
        /// The MessagePack writer that receives the identifier.
        /// </param>
        /// <param name="value">
        /// The identifier to write.
        /// </param>
        /// <exception cref="MessagePackSerializationException">
        /// Thrown when the identifier cannot be written to the fixed-length buffer.
        /// </exception>
        protected static void WriteGuidBin16(ref MessagePackWriter writer, Guid value)
        {
            var buffer = guidBuffer ??= new byte[16];

            if (!value.TryWriteBytes(buffer))
            {
                throw new MessagePackSerializationException("The Guid could not be written as a 16-byte binary value.");
            }

            writer.WriteBinHeader(16);
            writer.WriteRaw(buffer);
        }

        /// <summary>
        /// Reads an identifier from a MessagePack binary value containing exactly 16 bytes.
        /// </summary>
        /// <param name="reader">
        /// The MessagePack reader from which the identifier is read.
        /// </param>
        /// <returns>
        /// The decoded identifier.
        /// </returns>
        /// <exception cref="MessagePackSerializationException">
        /// Thrown when the value is <c>nil</c>, is not binary, or does not contain exactly 16 bytes.
        /// </exception>
        protected static Guid ReadGuidBin16(ref MessagePackReader reader)
        {
            var bytes = reader.ReadBytes();

            if (!bytes.HasValue)
            {
                throw new MessagePackSerializationException("Expected Guid as bin(16), bt found nil.");
            }

            var sequence = bytes.Value;

            if (sequence.Length != 16)
            {
                throw new MessagePackSerializationException($"Expected Guid as 16 bytes, but found {sequence.Length} bytes.");
            }

            if (sequence.IsSingleSegment)
            {
                return new Guid(sequence.FirstSpan);
            }

            Span<byte> buffer = stackalloc byte[16];
            sequence.CopyTo(buffer);

            return new Guid(buffer);
        }

        /// <summary>
        /// Writes a required string value.
        /// </summary>
        /// <param name="writer">
        /// The MessagePack writer that receives the string.
        /// </param>
        /// <param name="value">
        /// The string to write.
        /// </param>
        /// <param name="valueDescription">
        /// The modeled value description used in validation messages.
        /// </param>
        /// <exception cref="MessagePackSerializationException">
        /// Thrown when <paramref name="value" /> is <see langword="null" />.
        /// </exception>
        protected static void WriteRequiredString(ref MessagePackWriter writer, string? value, string valueDescription)
        {
            if (value == null)
            {
                throw new MessagePackSerializationException($"String value '{valueDescription}' may not be null.");
            }

            writer.Write(value);
        }

        /// <summary>
        /// Reads a required string value.
        /// </summary>
        /// <param name="reader">
        /// The MessagePack reader from which the string is read.
        /// </param>
        /// <param name="valueDescription">
        /// The modeled value description used in validation messages.
        /// </param>
        /// <returns>
        /// The decoded non-null string.
        /// </returns>
        /// <exception cref="MessagePackSerializationException">
        /// Thrown when the value is <c>nil</c> or is not a MessagePack string.
        /// </exception>
        protected static string ReadRequiredString(ref MessagePackReader reader, string valueDescription)
        {
            return reader.ReadString() ?? throw new MessagePackSerializationException($"String value '{valueDescription}' may not be nil.");
        }

        /// <summary>
        /// Writes a required string dictionary as a MessagePack map.
        /// </summary>
        /// <param name="writer">
        /// The MessagePack writer that receives the map.
        /// </param>
        /// <param name="value">
        /// The dictionary to write.
        /// </param>
        /// <param name="valueDescription">
        /// The modeled value description used in validation messages.
        /// </param>
        /// <exception cref="MessagePackSerializationException">
        /// Thrown when a dictionary key or value is <see langword="null" />.
        /// </exception>
        protected static void WriteStringDictionary(ref MessagePackWriter writer, IReadOnlyDictionary<string, string>? value, string valueDescription)
        {
            if (value == null)
            {
                writer.WriteNil();

                return;
            }

            writer.WriteMapHeader(value.Count);

            foreach (var entry in value)
            {
                if (entry.Key == null)
                {
                    throw new MessagePackSerializationException($"Dictionary value '{valueDescription}' contains a null key.");
                }

                if (entry.Value == null)
                {
                    throw new MessagePackSerializationException($"Dictionary value '{valueDescription}' contains a null value.");
                }

                writer.Write(entry.Key);
                writer.Write(entry.Value);
            }
        }

        /// <summary>
        /// Reads a nullable string dictionary from a MessagePack map.
        /// </summary>
        /// <param name="reader">
        /// The MessagePack reader from which the map is read.
        /// </param>
        /// <param name="valueDescription">
        /// The modeled value description used in validation messages.
        /// </param>
        /// <returns>
        /// The decoded ordinal string dictionary, or <see langword="null" /> for <c>nil</c>.
        /// </returns>
        /// <exception cref="MessagePackSerializationException">
        /// Thrown when the value is not a map, contains a null or non-string entry,
        /// or contains a duplicate key.
        /// </exception>
        protected static Dictionary<string, string>? ReadStringDictionary(ref MessagePackReader reader, string valueDescription)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            var count = reader.ReadMapHeader();
            var result = new Dictionary<string, string>(count, StringComparer.Ordinal);

            for (var i = 0; i < count; i++)
            {
                var key = ReadRequiredString(ref reader, $"{valueDescription} key");
                var value = ReadRequiredString(ref reader, $"{valueDescription} value");

                if (!result.TryAdd(key, value))
                {
                    throw new MessagePackSerializationException($"Dictionary value '{valueDescription}' contains duplicate key '{key}'.");
                }
            }

            return result;
        }

        /// <summary>
        /// Writes a URI as its original string representation.
        /// </summary>
        /// <param name="writer">
        /// The MessagePack writer that receives the URI.
        /// </param>
        /// <param name="value">
        /// The URI to write.
        /// </param>
        /// <param name="nullable">
        /// Whether a <see langword="null" /> value may be written as <c>nil</c>.
        /// </param>
        /// <param name="valueDescription">
        /// The modeled value description used in validation messages.
        /// </param>
        /// <exception cref="MessagePackSerializationException">
        /// Thrown when <paramref name="value" /> is <see langword="null" /> and
        /// <paramref name="nullable" /> is <see langword="false" />.
        /// </exception>
        protected static void WriteUri(ref MessagePackWriter writer, Uri? value, bool nullable, string valueDescription)
        {
            if (value == null)
            {
                if (nullable)
                {
                    writer.WriteNil();

                    return;
                }

                throw new MessagePackSerializationException($"URI value '{valueDescription}' may not be null.");
            }

            writer.Write(value.OriginalString);
        }

        /// <summary>
        /// Reads a URI from its original string representation.
        /// </summary>
        /// <param name="reader">
        /// The MessagePack reader from which the URI is read.
        /// </param>
        /// <param name="nullable">
        /// Whether <c>nil</c> is accepted.
        /// </param>
        /// <param name="valueDescription">
        /// The modeled value description used in validation messages.
        /// </param>
        /// <returns>
        /// The decoded relative or absolute URI, or <see langword="null" /> when permitted.
        /// </returns>
        /// <exception cref="MessagePackSerializationException">
        /// Thrown when the value is an unpermitted <c>nil</c>, is not a string, or is not a valid URI.
        /// </exception>
        protected static Uri ReadUri(ref MessagePackReader reader, bool nullable, string valueDescription)
        {
            if (reader.TryReadNil())
            {
                if (nullable)
                {
                    return null!;
                }

                throw new MessagePackSerializationException($"URI value '{valueDescription}' may not be nil.");
            }

            var value = ReadRequiredString(ref reader, valueDescription);

            try
            {
                return new Uri(value, UriKind.RelativeOrAbsolute);
            }
            catch (UriFormatException exception)
            {
                throw new MessagePackSerializationException($"URI value '{valueDescription}' is invalid.", exception);
            }
        }
    }
}
