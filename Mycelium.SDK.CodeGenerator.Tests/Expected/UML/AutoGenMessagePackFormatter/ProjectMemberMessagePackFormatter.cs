// ------------------------------------------------------------------------------------------------
//  <copyright file="ProjectMemberMessagePackFormatter.cs" company="Starion Group S.A.">
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
    /// <see cref="ProjectMember" /> DTO using MessagePack.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public sealed partial class ProjectMemberMessagePackFormatter : MessagePackFormatterBase, IMessagePackFormatter<ProjectMember>
    {
        /// <summary>
        /// Serializes an exact <see cref="ProjectMember" /> DTO.
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
        public void Serialize(ref MessagePackWriter writer, ProjectMember dto, MessagePackSerializerOptions options)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            writer.WriteArrayHeader(10);

            GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.Id, options);
            if (dto.ActiveOwnership.HasValue)
            {
                GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.ActiveOwnership.Value, options);
            }
            else
            {
                writer.WriteNil();
            }
            GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.CreatedBy, options);
            WriteRoundTripDateTime(ref writer, dto.CreatedOn);
            GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.IsPartOf, options);
            if (dto.Owns == null)
            {
                writer.WriteNil();
            }
            else
            {
                writer.WriteArrayHeader(dto.Owns.Count);

                for (var i = 0; i < dto.Owns.Count; i++)
                {
                    GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.Owns[i], options);
                }
            }
            ProjectMemberRoleMessagePackFormatter.Instance.Serialize(ref writer, dto.Role, options);
            GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.UpdatedBy, options);
            WriteRoundTripDateTime(ref writer, dto.UpdatedOn);
            GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.User, options);
        }

        /// <summary>
        /// Deserializes an exact <see cref="ProjectMember" /> DTO.
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
        public ProjectMember Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                throw new MessagePackSerializationException("ProjectMember may not be nil.");
            }

            options.Security.DepthStep(ref reader);

            try
            {
                var fieldCount = reader.ReadArrayHeader();

                if (fieldCount != 10)
                {
                    throw new MessagePackSerializationException($"ProjectMember contains {fieldCount} fields; exactly 10 fields are required.");
                }

                var dto = new ProjectMember();

                dto.Id = GuidMessagePackFormatter.Instance.Deserialize(ref reader, options);
                if (reader.TryReadNil())
                {
                    dto.ActiveOwnership = null;
                }
                else
                {
                    dto.ActiveOwnership = GuidMessagePackFormatter.Instance.Deserialize(ref reader, options);
                }
                dto.CreatedBy = GuidMessagePackFormatter.Instance.Deserialize(ref reader, options);
                dto.CreatedOn = ReadRoundTripDateTime(ref reader, "CreatedOn");
                dto.IsPartOf = GuidMessagePackFormatter.Instance.Deserialize(ref reader, options);
                if (reader.TryReadNil())
                {
                    dto.Owns = null;
                }
                else
                {
                    var messagePackOwnsCount = reader.ReadArrayHeader();
                    dto.Owns.Clear();

                    if (dto.Owns.Capacity < messagePackOwnsCount)
                    {
                        dto.Owns.Capacity = messagePackOwnsCount;
                    }

                    for (var i = 0; i < messagePackOwnsCount; i++)
                    {
                        dto.Owns.Add(GuidMessagePackFormatter.Instance.Deserialize(ref reader, options));
                    }
                }
                dto.Role = ProjectMemberRoleMessagePackFormatter.Instance.Deserialize(ref reader, options);
                dto.UpdatedBy = GuidMessagePackFormatter.Instance.Deserialize(ref reader, options);
                dto.UpdatedOn = ReadRoundTripDateTime(ref reader, "UpdatedOn");
                dto.User = GuidMessagePackFormatter.Instance.Deserialize(ref reader, options);

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
