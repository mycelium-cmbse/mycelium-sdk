// ------------------------------------------------------------------------------------------------
//  <copyright file="BranchProtectionRuleMessagePackFormatter.cs" company="Starion Group S.A.">
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
    /// <see cref="BranchProtectionRule" /> DTO using MessagePack.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public sealed partial class BranchProtectionRuleMessagePackFormatter :
        MessagePackFormatterBase,
        IMessagePackFormatter<BranchProtectionRule>
    {
        /// <summary>
        /// Serializes an exact <see cref="BranchProtectionRule" /> DTO.
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
        public void Serialize(
            ref MessagePackWriter writer,
            BranchProtectionRule dto,
            MessagePackSerializerOptions options)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            writer.WriteArrayHeader(11);

            WriteGuidBin16(ref writer, dto.Id);
            WriteGuidBin16(ref writer, dto.CreatedBy);
            writer.Write(dto.CreatedOn);
            if (dto.DefaultReviewers == null)
            {
                throw new MessagePackSerializationException("Collection property 'DefaultReviewers' may not be null.");
            }

            if (dto.DefaultReviewers.Count > 1)
            {
                dto.DefaultReviewers.Sort(static (left, right) => left.CompareTo(right));
            }

            writer.WriteArrayHeader(dto.DefaultReviewers.Count);

            for (var i = 0; i < dto.DefaultReviewers.Count; i++)
            {
                WriteGuidBin16(ref writer, dto.DefaultReviewers[i]);
            }
            WriteGuidBin16(ref writer, dto.EngineeringBranchId);
            if (dto.MergeAllowedFor == null)
            {
                throw new MessagePackSerializationException("Collection property 'MergeAllowedFor' may not be null.");
            }

            writer.WriteArrayHeader(dto.MergeAllowedFor.Count);

            for (var i = 0; i < dto.MergeAllowedFor.Count; i++)
            {
                writer.Write(dto.MergeAllowedFor[i] switch
                {
                    ProjectMemberRole.Administrator => "administrator",
                    ProjectMemberRole.Participant => "participant",
                    ProjectMemberRole.Viewer => "viewer",
                    _ => throw new MessagePackSerializationException($"Value '{dto.MergeAllowedFor[i]}' is not valid for ProjectMemberRole."),
                });
            }
            writer.Write(dto.MinimumRequiredApproval);
            WriteRequiredString(ref writer, dto.Name, "Name");
            writer.Write(dto.ReviewRequired);
            WriteGuidBin16(ref writer, dto.UpdatedBy);
            writer.Write(dto.UpdatedOn);
        }

        /// <summary>
        /// Deserializes an exact <see cref="BranchProtectionRule" /> DTO.
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
        public BranchProtectionRule Deserialize(
            ref MessagePackReader reader,
            MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                throw new MessagePackSerializationException(
                    "BranchProtectionRule may not be nil.");
            }

            options.Security.DepthStep(ref reader);

            try
            {
                var fieldCount = reader.ReadArrayHeader();

                if (fieldCount != 11)
                {
                    throw new MessagePackSerializationException(
                        $"BranchProtectionRule contains {fieldCount} fields; exactly 11 fields are required.");
                }

                var dto = new BranchProtectionRule();

                dto.Id = ReadGuidBin16(ref reader);
                dto.CreatedBy = ReadGuidBin16(ref reader);
                dto.CreatedOn = reader.ReadDateTime();
                if (reader.TryReadNil())
                {
                    throw new MessagePackSerializationException("Collection property 'DefaultReviewers' may not be nil.");
                }

                var messagePackDefaultReviewersCount = reader.ReadArrayHeader();
                dto.DefaultReviewers.Clear();

                if (dto.DefaultReviewers.Capacity < messagePackDefaultReviewersCount)
                {
                    dto.DefaultReviewers.Capacity = messagePackDefaultReviewersCount;
                }

                for (var i = 0; i < messagePackDefaultReviewersCount; i++)
                {
                    dto.DefaultReviewers.Add(ReadGuidBin16(ref reader));
                }
                dto.EngineeringBranchId = ReadGuidBin16(ref reader);
                if (reader.TryReadNil())
                {
                    throw new MessagePackSerializationException("Collection property 'MergeAllowedFor' may not be nil.");
                }

                var messagePackMergeAllowedForCount = reader.ReadArrayHeader();
                dto.MergeAllowedFor.Clear();

                if (dto.MergeAllowedFor.Capacity < messagePackMergeAllowedForCount)
                {
                    dto.MergeAllowedFor.Capacity = messagePackMergeAllowedForCount;
                }

                for (var i = 0; i < messagePackMergeAllowedForCount; i++)
                {
                    var messagePackMergeAllowedForValue = ReadRequiredString(ref reader, "MergeAllowedFor item");
                    dto.MergeAllowedFor.Add(messagePackMergeAllowedForValue switch
                    {
                        "administrator" => ProjectMemberRole.Administrator,
                        "participant" => ProjectMemberRole.Participant,
                        "viewer" => ProjectMemberRole.Viewer,
                        _ => throw new MessagePackSerializationException($"Value '{messagePackMergeAllowedForValue}' is not valid for ProjectMemberRole."),
                    });
                }
                dto.MinimumRequiredApproval = reader.ReadInt32();
                dto.Name = ReadRequiredString(ref reader, "Name");
                dto.ReviewRequired = reader.ReadBoolean();
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
