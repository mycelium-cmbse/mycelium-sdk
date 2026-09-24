// ------------------------------------------------------------------------------------------------
//  <copyright file="CommentMessagePackFormatter.cs" company="Starion Group S.A.">
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
    /// <see cref="Comment" /> DTO using MessagePack.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public sealed partial class CommentMessagePackFormatter : MessagePackFormatterBase, IMessagePackFormatter<Comment?>
    {
        /// <summary>
        /// Serializes an exact <see cref="Comment" /> DTO.
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
        public void Serialize(ref MessagePackWriter writer, Comment? dto, MessagePackSerializerOptions options)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            writer.WriteArrayHeader(11);

            WriteGuidBin16(ref writer, dto.Id);
            WriteGuidBin16(ref writer, dto.Author);
            writer.Write(dto.CommentStatus switch
            {
                CommentStatus.Open => "open",
                CommentStatus.Resolved => "resolved",
                _ => throw new MessagePackSerializationException($"Value '{dto.CommentStatus}' is not valid for CommentStatus."),
            });
            WriteRequiredString(ref writer, dto.Content, "Content");
            WriteGuidBin16(ref writer, dto.CreatedBy);
            writer.Write(dto.CreatedOn);
            if (dto.Quotes.HasValue)
            {
                WriteGuidBin16(ref writer, dto.Quotes.Value);
            }
            else
            {
                writer.WriteNil();
            }
            if (dto.Replies == null)
            {
                throw new MessagePackSerializationException("Collection property 'Replies' may not be null.");
            }

            writer.WriteArrayHeader(dto.Replies.Count);

            for (var i = 0; i < dto.Replies.Count; i++)
            {
                WriteGuidBin16(ref writer, dto.Replies[i]);
            }
            WriteGuidBin16(ref writer, dto.TargetElementId);
            WriteGuidBin16(ref writer, dto.UpdatedBy);
            writer.Write(dto.UpdatedOn);
        }

        /// <summary>
        /// Deserializes an exact <see cref="Comment" /> DTO.
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
        public Comment? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                throw new MessagePackSerializationException("Comment may not be nil.");
            }

            options.Security.DepthStep(ref reader);

            try
            {
                var fieldCount = reader.ReadArrayHeader();

                if (fieldCount != 11)
                {
                    throw new MessagePackSerializationException($"Comment contains {fieldCount} fields; exactly 11 fields are required.");
                }

                var dto = new Comment();

                dto.Id = ReadGuidBin16(ref reader);
                dto.Author = ReadGuidBin16(ref reader);
                var messagePackCommentStatusValue = ReadRequiredString(ref reader, "CommentStatus");
                dto.CommentStatus = messagePackCommentStatusValue switch
                {
                    "open" => CommentStatus.Open,
                    "resolved" => CommentStatus.Resolved,
                    _ => throw new MessagePackSerializationException($"Value '{messagePackCommentStatusValue}' is not valid for CommentStatus."),
                };
                dto.Content = ReadRequiredString(ref reader, "Content");
                dto.CreatedBy = ReadGuidBin16(ref reader);
                dto.CreatedOn = reader.ReadDateTime();
                if (reader.TryReadNil())
                {
                    dto.Quotes = null;
                }
                else
                {
                    dto.Quotes = ReadGuidBin16(ref reader);
                }
                if (reader.TryReadNil())
                {
                    throw new MessagePackSerializationException("Collection property 'Replies' may not be nil.");
                }

                var messagePackRepliesCount = reader.ReadArrayHeader();
                dto.Replies.Clear();

                if (dto.Replies.Capacity < messagePackRepliesCount)
                {
                    dto.Replies.Capacity = messagePackRepliesCount;
                }

                for (var i = 0; i < messagePackRepliesCount; i++)
                {
                    dto.Replies.Add(ReadGuidBin16(ref reader));
                }
                dto.TargetElementId = ReadGuidBin16(ref reader);
                dto.UpdatedBy = ReadGuidBin16(ref reader);
                dto.UpdatedOn = reader.ReadDateTime();

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
