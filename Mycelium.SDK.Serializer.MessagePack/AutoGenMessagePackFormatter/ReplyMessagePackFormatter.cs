// ------------------------------------------------------------------------------------------------
//  <copyright file="ReplyMessagePackFormatter.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.Serializer.MessagePack
{
    using System;
    using System.CodeDom.Compiler;

    using global::MessagePack;
    using global::MessagePack.Formatters;

    using Mycelium.SDK.DTO;

    /// <summary>
    /// Serializes and deserializes an exact
    /// <see cref="Reply" /> DTO using MessagePack.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public sealed partial class ReplyMessagePackFormatter : MessagePackFormatterBase, IMessagePackFormatter<Reply>
    {
        /// <summary>
        /// Serializes an exact <see cref="Reply" /> DTO.
        /// </summary>
        /// <param name="writer">
        /// The MessagePack writer that receives the serialized DTO.
        /// </param>
        /// <param name="dto">
        /// The DTO to serialize.
        /// </param>
        /// <param name="options">
        /// The MessagePack serializer options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="dto" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="MessagePackSerializationException">
        /// Thrown when a DTO property contains a value that is invalid for its approved
        /// MessagePack representation.
        /// </exception>
        public void Serialize(ref MessagePackWriter writer, Reply dto, MessagePackSerializerOptions options)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            writer.WriteArrayHeader(7);

            GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.Id, options);
            WriteRequiredString(ref writer, dto.Content, "Content");
            GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.CreatedBy, options);
            WriteRoundTripDateTime(ref writer, dto.CreatedOn);
            if (dto.Quotes.HasValue)
            {
                GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.Quotes.Value, options);
            }
            else
            {
                writer.WriteNil();
            }
            GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.UpdatedBy, options);
            WriteRoundTripDateTime(ref writer, dto.UpdatedOn);
        }

        /// <summary>
        /// Deserializes an exact <see cref="Reply" /> DTO.
        /// </summary>
        /// <param name="reader">
        /// The MessagePack reader from which the DTO is deserialized.
        /// </param>
        /// <param name="options">
        /// The MessagePack serializer options.
        /// </param>
        /// <returns>
        /// The deserialized DTO.
        /// </returns>
        /// <exception cref="MessagePackSerializationException">
        /// Thrown when the encoded DTO is <c>nil</c>, has an incorrect field count or contains
        /// a value that is invalid for its approved MessagePack representation.
        /// </exception>
        public Reply Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                throw new MessagePackSerializationException("Reply may not be nil.");
            }

            options.Security.DepthStep(ref reader);

            try
            {
                var fieldCount = reader.ReadArrayHeader();

                if (fieldCount != 7)
                {
                    throw new MessagePackSerializationException($"Reply contains {fieldCount} fields; exactly 7 fields are required.");
                }

                var dto = new Reply();

                dto.Id = GuidMessagePackFormatter.Instance.Deserialize(ref reader, options);
                dto.Content = ReadRequiredString(ref reader, "Content");
                dto.CreatedBy = GuidMessagePackFormatter.Instance.Deserialize(ref reader, options);
                dto.CreatedOn = ReadRoundTripDateTime(ref reader, "CreatedOn");
                if (reader.TryReadNil())
                {
                    dto.Quotes = null;
                }
                else
                {
                    dto.Quotes = GuidMessagePackFormatter.Instance.Deserialize(ref reader, options);
                }
                dto.UpdatedBy = GuidMessagePackFormatter.Instance.Deserialize(ref reader, options);
                dto.UpdatedOn = ReadRoundTripDateTime(ref reader, "UpdatedOn");

                return dto;
            }
            finally
            {
                reader.Depth--;
            }
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
