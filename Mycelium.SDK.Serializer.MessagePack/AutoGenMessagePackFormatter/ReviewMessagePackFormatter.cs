// ------------------------------------------------------------------------------------------------
//  <copyright file="ReviewMessagePackFormatter.cs" company="Starion Group S.A.">
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
    /// <see cref="Review" /> DTO using MessagePack.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public sealed partial class ReviewMessagePackFormatter : MessagePackFormatterBase, IMessagePackFormatter<Review>
    {
        /// <summary>
        /// Serializes an exact <see cref="Review" /> DTO.
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
        public void Serialize(ref MessagePackWriter writer, Review dto, MessagePackSerializerOptions options)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            writer.WriteArrayHeader(13);

            GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.Id, options);
            GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.Author, options);
            if (dto.Comments == null)
            {
                writer.WriteNil();
            }
            else
            {
                writer.WriteArrayHeader(dto.Comments.Count);

                for (var i = 0; i < dto.Comments.Count; i++)
                {
                    GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.Comments[i], options);
                }
            }
            GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.CreatedBy, options);
            WriteRoundTripDateTime(ref writer, dto.CreatedOn);
            WriteRequiredString(ref writer, dto.Description, "Description");
            if (dto.Reviewers == null)
            {
                writer.WriteNil();
            }
            else
            {
                writer.WriteArrayHeader(dto.Reviewers.Count);

                for (var i = 0; i < dto.Reviewers.Count; i++)
                {
                    GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.Reviewers[i], options);
                }
            }
            GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.SourceBranchId, options);
            ReviewStatusMessagePackFormatter.Instance.Serialize(ref writer, dto.Status, options);
            GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.TargetBranchId, options);
            WriteRequiredString(ref writer, dto.Title, "Title");
            GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.UpdatedBy, options);
            WriteRoundTripDateTime(ref writer, dto.UpdatedOn);
        }

        /// <summary>
        /// Deserializes an exact <see cref="Review" /> DTO.
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
        public Review Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                throw new MessagePackSerializationException("Review may not be nil.");
            }

            options.Security.DepthStep(ref reader);

            try
            {
                var fieldCount = reader.ReadArrayHeader();

                if (fieldCount != 13)
                {
                    throw new MessagePackSerializationException($"Review contains {fieldCount} fields; exactly 13 fields are required.");
                }

                var dto = new Review();

                dto.Id = GuidMessagePackFormatter.Instance.Deserialize(ref reader, options);
                dto.Author = GuidMessagePackFormatter.Instance.Deserialize(ref reader, options);
                if (reader.TryReadNil())
                {
                    dto.Comments = null;
                }
                else
                {
                    var messagePackCommentsCount = reader.ReadArrayHeader();
                    dto.Comments.Clear();

                    if (dto.Comments.Capacity < messagePackCommentsCount)
                    {
                        dto.Comments.Capacity = messagePackCommentsCount;
                    }

                    for (var i = 0; i < messagePackCommentsCount; i++)
                    {
                        dto.Comments.Add(GuidMessagePackFormatter.Instance.Deserialize(ref reader, options));
                    }
                }
                dto.CreatedBy = GuidMessagePackFormatter.Instance.Deserialize(ref reader, options);
                dto.CreatedOn = ReadRoundTripDateTime(ref reader, "CreatedOn");
                dto.Description = ReadRequiredString(ref reader, "Description");
                if (reader.TryReadNil())
                {
                    dto.Reviewers = null;
                }
                else
                {
                    var messagePackReviewersCount = reader.ReadArrayHeader();
                    dto.Reviewers.Clear();

                    if (dto.Reviewers.Capacity < messagePackReviewersCount)
                    {
                        dto.Reviewers.Capacity = messagePackReviewersCount;
                    }

                    for (var i = 0; i < messagePackReviewersCount; i++)
                    {
                        dto.Reviewers.Add(GuidMessagePackFormatter.Instance.Deserialize(ref reader, options));
                    }
                }
                dto.SourceBranchId = GuidMessagePackFormatter.Instance.Deserialize(ref reader, options);
                dto.Status = ReviewStatusMessagePackFormatter.Instance.Deserialize(ref reader, options);
                dto.TargetBranchId = GuidMessagePackFormatter.Instance.Deserialize(ref reader, options);
                dto.Title = ReadRequiredString(ref reader, "Title");
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
