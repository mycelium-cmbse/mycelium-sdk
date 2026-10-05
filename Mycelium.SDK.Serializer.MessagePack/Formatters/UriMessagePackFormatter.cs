// ------------------------------------------------------------------------------------------------
//  <copyright file="UriMessagePackFormatter.cs" company="Starion Group S.A.">
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
    /// Encodes a non-null relative or absolute URI using its original string.
    /// </summary>
    internal sealed class UriMessagePackFormatter : MessagePackFormatterBase, IMessagePackFormatter<Uri>
    {
        /// <summary>
        /// The shared formatter used directly by generated DTO formatters.
        /// </summary>
        internal static readonly UriMessagePackFormatter Instance = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="UriMessagePackFormatter" /> class.
        /// </summary>
        internal UriMessagePackFormatter()
        {
        }

        /// <summary>
        /// Writes a non-null relative or absolute URI using its original string.
        /// </summary>
        /// <param name="writer">
        /// The MessagePack writer that receives the encoded value.
        /// </param>
        /// <param name="value">
        /// The non-null URI to write.
        /// </param>
        /// <param name="options">
        /// The serializer options supplied for the operation.
        /// </param>
        /// <exception cref="MessagePackSerializationException">
        /// Thrown when <paramref name="value" /> is <see langword="null" />.
        /// </exception>
        public void Serialize(ref MessagePackWriter writer, Uri value, MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                throw new MessagePackSerializationException("URI value may not be null.");
            }

            writer.Write(value.OriginalString);
        }

        /// <summary>
        /// Reads a non-null relative or absolute URI from its original string.
        /// </summary>
        /// <param name="reader">
        /// The MessagePack reader from which the encoded value is read.
        /// </param>
        /// <param name="options">
        /// The serializer options supplied for the operation.
        /// </param>
        /// <returns>
        /// The decoded URI, preserving the encoded string in its original form.
        /// </returns>
        /// <exception cref="MessagePackSerializationException">
        /// Thrown when the value is <c>nil</c>, is not a string, or is not a valid relative or absolute URI.
        /// </exception>
        public Uri Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            var value = ReadRequiredString(ref reader, "URI");

            try
            {
                return new Uri(value, UriKind.RelativeOrAbsolute);
            }
            catch (UriFormatException exception)
            {
                throw new MessagePackSerializationException("URI value is invalid.", exception);
            }
        }
    }
}
