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

        internal UriMessagePackFormatter()
        {
        }

        /// <inheritdoc />
        public void Serialize(ref MessagePackWriter writer, Uri value, MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                throw new MessagePackSerializationException("URI value may not be null.");
            }

            writer.Write(value.OriginalString);
        }

        /// <inheritdoc />
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
