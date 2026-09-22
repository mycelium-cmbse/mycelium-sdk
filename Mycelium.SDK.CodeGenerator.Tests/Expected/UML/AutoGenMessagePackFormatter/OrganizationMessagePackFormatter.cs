// ------------------------------------------------------------------------------------------------
//  <copyright file="OrganizationMessagePackFormatter.cs" company="Starion Group S.A.">
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
    /// <see cref="Organization" /> DTO using MessagePack.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public sealed partial class OrganizationMessagePackFormatter : MessagePackFormatterBase, IMessagePackFormatter<Organization?>
    {
        /// <summary>
        /// Serializes an exact <see cref="Organization" /> DTO.
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
        public void Serialize(ref MessagePackWriter writer, Organization? dto, MessagePackSerializerOptions options)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            writer.WriteArrayHeader(11);

            WriteGuidBin16(ref writer, dto.Id);
            WriteGuidBin16(ref writer, dto.CreatedBy);
            writer.Write(dto.CreatedOn);
            WriteRequiredString(ref writer, dto.Description, "Description");
            if (dto.InvolvedUser == null)
            {
                throw new MessagePackSerializationException("Collection property 'InvolvedUser' may not be null.");
            }

            if (dto.InvolvedUser.Count > 1)
            {
                dto.InvolvedUser.Sort(static (left, right) => left.CompareTo(right));
            }

            writer.WriteArrayHeader(dto.InvolvedUser.Count);

            for (var i = 0; i < dto.InvolvedUser.Count; i++)
            {
                WriteGuidBin16(ref writer, dto.InvolvedUser[i]);
            }
            WriteRequiredString(ref writer, dto.Name, "Name");
            WriteGuidBin16(ref writer, dto.Policy);
            if (dto.Projects == null)
            {
                throw new MessagePackSerializationException("Collection property 'Projects' may not be null.");
            }

            if (dto.Projects.Count > 1)
            {
                dto.Projects.Sort(static (left, right) => left.CompareTo(right));
            }

            writer.WriteArrayHeader(dto.Projects.Count);

            for (var i = 0; i < dto.Projects.Count; i++)
            {
                WriteGuidBin16(ref writer, dto.Projects[i]);
            }
            writer.Write(dto.Status switch
            {
                ActivationStatus.Active => "active",
                ActivationStatus.Pending => "pending",
                ActivationStatus.Suspended => "suspended",
                ActivationStatus.Archived => "archived",
                ActivationStatus.Deleted => "deleted",
                _ => throw new MessagePackSerializationException($"Value '{dto.Status}' is not valid for ActivationStatus."),
            });
            WriteGuidBin16(ref writer, dto.UpdatedBy);
            writer.Write(dto.UpdatedOn);
        }

        /// <summary>
        /// Deserializes an exact <see cref="Organization" /> DTO.
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
        public Organization? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                throw new MessagePackSerializationException("Organization may not be nil.");
            }

            options.Security.DepthStep(ref reader);

            try
            {
                var fieldCount = reader.ReadArrayHeader();

                if (fieldCount != 11)
                {
                    throw new MessagePackSerializationException($"Organization contains {fieldCount} fields; exactly 11 fields are required.");
                }

                var dto = new Organization();

                dto.Id = ReadGuidBin16(ref reader);
                dto.CreatedBy = ReadGuidBin16(ref reader);
                dto.CreatedOn = reader.ReadDateTime();
                dto.Description = ReadRequiredString(ref reader, "Description");
                if (reader.TryReadNil())
                {
                    throw new MessagePackSerializationException("Collection property 'InvolvedUser' may not be nil.");
                }

                var messagePackInvolvedUserCount = reader.ReadArrayHeader();
                dto.InvolvedUser.Clear();

                if (dto.InvolvedUser.Capacity < messagePackInvolvedUserCount)
                {
                    dto.InvolvedUser.Capacity = messagePackInvolvedUserCount;
                }

                for (var i = 0; i < messagePackInvolvedUserCount; i++)
                {
                    dto.InvolvedUser.Add(ReadGuidBin16(ref reader));
                }
                dto.Name = ReadRequiredString(ref reader, "Name");
                dto.Policy = ReadGuidBin16(ref reader);
                if (reader.TryReadNil())
                {
                    throw new MessagePackSerializationException("Collection property 'Projects' may not be nil.");
                }

                var messagePackProjectsCount = reader.ReadArrayHeader();
                dto.Projects.Clear();

                if (dto.Projects.Capacity < messagePackProjectsCount)
                {
                    dto.Projects.Capacity = messagePackProjectsCount;
                }

                for (var i = 0; i < messagePackProjectsCount; i++)
                {
                    dto.Projects.Add(ReadGuidBin16(ref reader));
                }
                var messagePackStatusValue = ReadRequiredString(ref reader, "Status");
                dto.Status = messagePackStatusValue switch
                {
                    "active" => ActivationStatus.Active,
                    "pending" => ActivationStatus.Pending,
                    "suspended" => ActivationStatus.Suspended,
                    "archived" => ActivationStatus.Archived,
                    "deleted" => ActivationStatus.Deleted,
                    _ => throw new MessagePackSerializationException($"Value '{messagePackStatusValue}' is not valid for ActivationStatus."),
                };
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
