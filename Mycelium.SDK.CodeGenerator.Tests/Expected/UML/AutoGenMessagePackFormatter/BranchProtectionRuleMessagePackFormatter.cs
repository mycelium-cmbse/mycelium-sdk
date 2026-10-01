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
    public sealed partial class BranchProtectionRuleMessagePackFormatter : MessagePackFormatterBase, IMessagePackFormatter<BranchProtectionRule>
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
        public void Serialize(ref MessagePackWriter writer, BranchProtectionRule dto, MessagePackSerializerOptions options)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            writer.WriteArrayHeader(11);

            GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.Id, options);
            GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.CreatedBy, options);
            WriteRoundTripDateTime(ref writer, dto.CreatedOn);
            if (dto.DefaultReviewers == null)
            {
                writer.WriteNil();
            }
            else
            {
                writer.WriteArrayHeader(dto.DefaultReviewers.Count);

                for (var i = 0; i < dto.DefaultReviewers.Count; i++)
                {
                    GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.DefaultReviewers[i], options);
                }
            }
            GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.EngineeringBranchId, options);
            if (dto.MergeAllowedFor == null)
            {
                writer.WriteNil();
            }
            else
            {
                writer.WriteArrayHeader(dto.MergeAllowedFor.Count);

                for (var i = 0; i < dto.MergeAllowedFor.Count; i++)
                {
                    ProjectMemberRoleMessagePackFormatter.Instance.Serialize(ref writer, dto.MergeAllowedFor[i], options);
                }
            }
            writer.Write(dto.MinimumRequiredApproval);
            WriteRequiredString(ref writer, dto.Name, "Name");
            writer.Write(dto.ReviewRequired);
            GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.UpdatedBy, options);
            WriteRoundTripDateTime(ref writer, dto.UpdatedOn);
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
        public BranchProtectionRule Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                throw new MessagePackSerializationException("BranchProtectionRule may not be nil.");
            }

            options.Security.DepthStep(ref reader);

            try
            {
                var fieldCount = reader.ReadArrayHeader();

                if (fieldCount != 11)
                {
                    throw new MessagePackSerializationException($"BranchProtectionRule contains {fieldCount} fields; exactly 11 fields are required.");
                }

                var dto = new BranchProtectionRule();

                dto.Id = GuidMessagePackFormatter.Instance.Deserialize(ref reader, options);
                dto.CreatedBy = GuidMessagePackFormatter.Instance.Deserialize(ref reader, options);
                dto.CreatedOn = ReadRoundTripDateTime(ref reader, "CreatedOn");
                if (reader.TryReadNil())
                {
                    dto.DefaultReviewers = null;
                }
                else
                {
                    var messagePackDefaultReviewersCount = reader.ReadArrayHeader();
                    dto.DefaultReviewers.Clear();

                    if (dto.DefaultReviewers.Capacity < messagePackDefaultReviewersCount)
                    {
                        dto.DefaultReviewers.Capacity = messagePackDefaultReviewersCount;
                    }

                    for (var i = 0; i < messagePackDefaultReviewersCount; i++)
                    {
                        dto.DefaultReviewers.Add(GuidMessagePackFormatter.Instance.Deserialize(ref reader, options));
                    }
                }
                dto.EngineeringBranchId = GuidMessagePackFormatter.Instance.Deserialize(ref reader, options);
                if (reader.TryReadNil())
                {
                    dto.MergeAllowedFor = null;
                }
                else
                {
                    var messagePackMergeAllowedForCount = reader.ReadArrayHeader();
                    dto.MergeAllowedFor.Clear();

                    if (dto.MergeAllowedFor.Capacity < messagePackMergeAllowedForCount)
                    {
                        dto.MergeAllowedFor.Capacity = messagePackMergeAllowedForCount;
                    }

                    for (var i = 0; i < messagePackMergeAllowedForCount; i++)
                    {
                        dto.MergeAllowedFor.Add(ProjectMemberRoleMessagePackFormatter.Instance.Deserialize(ref reader, options));
                    }
                }
                dto.MinimumRequiredApproval = reader.ReadInt32();
                dto.Name = ReadRequiredString(ref reader, "Name");
                dto.ReviewRequired = reader.ReadBoolean();
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
