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
    public sealed partial class ProjectMemberMessagePackFormatter : MessagePackFormatterBase, IMessagePackFormatter<ProjectMember?>
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
        public void Serialize(ref MessagePackWriter writer, ProjectMember? dto, MessagePackSerializerOptions options)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            writer.WriteArrayHeader(10);

            WriteGuidBin16(ref writer, dto.Id);
            if (dto.ActiveOwnership.HasValue)
            {
                WriteGuidBin16(ref writer, dto.ActiveOwnership.Value);
            }
            else
            {
                writer.WriteNil();
            }
            WriteGuidBin16(ref writer, dto.CreatedBy);
            writer.Write(dto.CreatedOn);
            WriteGuidBin16(ref writer, dto.IsPartOf);
            if (dto.Owns == null)
            {
                throw new MessagePackSerializationException("Collection property 'Owns' may not be null.");
            }

            if (dto.Owns.Count > 1)
            {
                dto.Owns.Sort(static (left, right) => left.CompareTo(right));
            }

            writer.WriteArrayHeader(dto.Owns.Count);

            for (var i = 0; i < dto.Owns.Count; i++)
            {
                WriteGuidBin16(ref writer, dto.Owns[i]);
            }
            writer.Write(dto.Role switch
            {
                ProjectMemberRole.Administrator => "administrator",
                ProjectMemberRole.Participant => "participant",
                ProjectMemberRole.Viewer => "viewer",
                _ => throw new MessagePackSerializationException($"Value '{dto.Role}' is not valid for ProjectMemberRole."),
            });
            WriteGuidBin16(ref writer, dto.UpdatedBy);
            writer.Write(dto.UpdatedOn);
            WriteGuidBin16(ref writer, dto.User);
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
        public ProjectMember? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
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

                dto.Id = ReadGuidBin16(ref reader);
                if (reader.TryReadNil())
                {
                    dto.ActiveOwnership = null;
                }
                else
                {
                    dto.ActiveOwnership = ReadGuidBin16(ref reader);
                }
                dto.CreatedBy = ReadGuidBin16(ref reader);
                dto.CreatedOn = reader.ReadDateTime();
                dto.IsPartOf = ReadGuidBin16(ref reader);
                if (reader.TryReadNil())
                {
                    throw new MessagePackSerializationException("Collection property 'Owns' may not be nil.");
                }

                var messagePackOwnsCount = reader.ReadArrayHeader();
                dto.Owns.Clear();

                if (dto.Owns.Capacity < messagePackOwnsCount)
                {
                    dto.Owns.Capacity = messagePackOwnsCount;
                }

                for (var i = 0; i < messagePackOwnsCount; i++)
                {
                    dto.Owns.Add(ReadGuidBin16(ref reader));
                }
                var messagePackRoleValue = ReadRequiredString(ref reader, "Role");
                dto.Role = messagePackRoleValue switch
                {
                    "administrator" => ProjectMemberRole.Administrator,
                    "participant" => ProjectMemberRole.Participant,
                    "viewer" => ProjectMemberRole.Viewer,
                    _ => throw new MessagePackSerializationException($"Value '{messagePackRoleValue}' is not valid for ProjectMemberRole."),
                };
                dto.UpdatedBy = ReadGuidBin16(ref reader);
                dto.UpdatedOn = reader.ReadDateTime();
                dto.User = ReadGuidBin16(ref reader);

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
