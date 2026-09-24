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
    public sealed partial class ReviewMessagePackFormatter : MessagePackFormatterBase, IMessagePackFormatter<Review?>
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
        public void Serialize(ref MessagePackWriter writer, Review? dto, MessagePackSerializerOptions options)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            writer.WriteArrayHeader(13);

            WriteGuidBin16(ref writer, dto.Id);
            WriteGuidBin16(ref writer, dto.Author);
            if (dto.Comments == null)
            {
                throw new MessagePackSerializationException("Collection property 'Comments' may not be null.");
            }

            writer.WriteArrayHeader(dto.Comments.Count);

            for (var i = 0; i < dto.Comments.Count; i++)
            {
                WriteGuidBin16(ref writer, dto.Comments[i]);
            }
            WriteGuidBin16(ref writer, dto.CreatedBy);
            writer.Write(dto.CreatedOn);
            WriteRequiredString(ref writer, dto.Description, "Description");
            if (dto.Reviewers == null)
            {
                throw new MessagePackSerializationException("Collection property 'Reviewers' may not be null.");
            }

            writer.WriteArrayHeader(dto.Reviewers.Count);

            for (var i = 0; i < dto.Reviewers.Count; i++)
            {
                WriteGuidBin16(ref writer, dto.Reviewers[i]);
            }
            WriteGuidBin16(ref writer, dto.SourceBranchId);
            writer.Write(dto.Status switch
            {
                ReviewStatus.Draft => "draft",
                ReviewStatus.Ready => "ready",
                ReviewStatus.Approved => "approved",
                ReviewStatus.ChangesRequested => "changesrequested",
                ReviewStatus.Closed => "closed",
                _ => throw new MessagePackSerializationException($"Value '{dto.Status}' is not valid for ReviewStatus."),
            });
            WriteGuidBin16(ref writer, dto.TargetBranchId);
            WriteRequiredString(ref writer, dto.Title, "Title");
            WriteGuidBin16(ref writer, dto.UpdatedBy);
            writer.Write(dto.UpdatedOn);
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
        public Review? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
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

                dto.Id = ReadGuidBin16(ref reader);
                dto.Author = ReadGuidBin16(ref reader);
                if (reader.TryReadNil())
                {
                    throw new MessagePackSerializationException("Collection property 'Comments' may not be nil.");
                }

                var messagePackCommentsCount = reader.ReadArrayHeader();
                dto.Comments.Clear();

                if (dto.Comments.Capacity < messagePackCommentsCount)
                {
                    dto.Comments.Capacity = messagePackCommentsCount;
                }

                for (var i = 0; i < messagePackCommentsCount; i++)
                {
                    dto.Comments.Add(ReadGuidBin16(ref reader));
                }
                dto.CreatedBy = ReadGuidBin16(ref reader);
                dto.CreatedOn = reader.ReadDateTime();
                dto.Description = ReadRequiredString(ref reader, "Description");
                if (reader.TryReadNil())
                {
                    throw new MessagePackSerializationException("Collection property 'Reviewers' may not be nil.");
                }

                var messagePackReviewersCount = reader.ReadArrayHeader();
                dto.Reviewers.Clear();

                if (dto.Reviewers.Capacity < messagePackReviewersCount)
                {
                    dto.Reviewers.Capacity = messagePackReviewersCount;
                }

                for (var i = 0; i < messagePackReviewersCount; i++)
                {
                    dto.Reviewers.Add(ReadGuidBin16(ref reader));
                }
                dto.SourceBranchId = ReadGuidBin16(ref reader);
                var messagePackStatusValue = ReadRequiredString(ref reader, "Status");
                dto.Status = messagePackStatusValue switch
                {
                    "draft" => ReviewStatus.Draft,
                    "ready" => ReviewStatus.Ready,
                    "approved" => ReviewStatus.Approved,
                    "changesrequested" => ReviewStatus.ChangesRequested,
                    "closed" => ReviewStatus.Closed,
                    _ => throw new MessagePackSerializationException($"Value '{messagePackStatusValue}' is not valid for ReviewStatus."),
                };
                dto.TargetBranchId = ReadGuidBin16(ref reader);
                dto.Title = ReadRequiredString(ref reader, "Title");
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
