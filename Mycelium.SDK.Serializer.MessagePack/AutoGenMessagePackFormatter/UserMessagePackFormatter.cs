// ------------------------------------------------------------------------------------------------
//  <copyright file="UserMessagePackFormatter.cs" company="Starion Group S.A.">
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
    /// <see cref="User" /> DTO using MessagePack.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public sealed partial class UserMessagePackFormatter :
        MessagePackFormatterBase,
        IMessagePackFormatter<User>
    {
        /// <summary>
        /// Serializes an exact <see cref="User" /> DTO.
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
            User dto,
            MessagePackSerializerOptions options)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            writer.WriteArrayHeader(12);

            WriteGuidBin16(ref writer, dto.Id);
            WriteGuidBin16(ref writer, dto.CreatedBy);
            writer.Write(dto.CreatedOn);
            WriteRequiredString(ref writer, dto.ExternalIdentifier, "ExternalIdentifier");
            if (dto.IsPartOfOrganizations == null)
            {
                throw new MessagePackSerializationException("Collection property 'IsPartOfOrganizations' may not be null.");
            }

            if (dto.IsPartOfOrganizations.Count > 1)
            {
                dto.IsPartOfOrganizations.Sort(static (left, right) => left.CompareTo(right));
            }

            writer.WriteArrayHeader(dto.IsPartOfOrganizations.Count);

            for (var i = 0; i < dto.IsPartOfOrganizations.Count; i++)
            {
                WriteGuidBin16(ref writer, dto.IsPartOfOrganizations[i]);
            }
            if (dto.IsPartOfProjects == null)
            {
                throw new MessagePackSerializationException("Collection property 'IsPartOfProjects' may not be null.");
            }

            if (dto.IsPartOfProjects.Count > 1)
            {
                dto.IsPartOfProjects.Sort(static (left, right) => left.CompareTo(right));
            }

            writer.WriteArrayHeader(dto.IsPartOfProjects.Count);

            for (var i = 0; i < dto.IsPartOfProjects.Count; i++)
            {
                WriteGuidBin16(ref writer, dto.IsPartOfProjects[i]);
            }
            WriteRequiredString(ref writer, dto.Mail, "Mail");
            WriteRequiredString(ref writer, dto.Name, "Name");
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
            WriteStringDictionary(ref writer, dto.UserPreferences, "UserPreferences");
        }

        /// <summary>
        /// Deserializes an exact <see cref="User" /> DTO.
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
        public User Deserialize(
            ref MessagePackReader reader,
            MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                throw new MessagePackSerializationException(
                    "User may not be nil.");
            }

            options.Security.DepthStep(ref reader);

            try
            {
                var fieldCount = reader.ReadArrayHeader();

                if (fieldCount != 12)
                {
                    throw new MessagePackSerializationException(
                        $"User contains {fieldCount} fields; exactly 12 fields are required.");
                }

                var dto = new User();

                dto.Id = ReadGuidBin16(ref reader);
                dto.CreatedBy = ReadGuidBin16(ref reader);
                dto.CreatedOn = reader.ReadDateTime();
                dto.ExternalIdentifier = ReadRequiredString(ref reader, "ExternalIdentifier");
                if (reader.TryReadNil())
                {
                    throw new MessagePackSerializationException("Collection property 'IsPartOfOrganizations' may not be nil.");
                }

                var messagePackIsPartOfOrganizationsCount = reader.ReadArrayHeader();
                dto.IsPartOfOrganizations.Clear();

                if (dto.IsPartOfOrganizations.Capacity < messagePackIsPartOfOrganizationsCount)
                {
                    dto.IsPartOfOrganizations.Capacity = messagePackIsPartOfOrganizationsCount;
                }

                for (var i = 0; i < messagePackIsPartOfOrganizationsCount; i++)
                {
                    dto.IsPartOfOrganizations.Add(ReadGuidBin16(ref reader));
                }
                if (reader.TryReadNil())
                {
                    throw new MessagePackSerializationException("Collection property 'IsPartOfProjects' may not be nil.");
                }

                var messagePackIsPartOfProjectsCount = reader.ReadArrayHeader();
                dto.IsPartOfProjects.Clear();

                if (dto.IsPartOfProjects.Capacity < messagePackIsPartOfProjectsCount)
                {
                    dto.IsPartOfProjects.Capacity = messagePackIsPartOfProjectsCount;
                }

                for (var i = 0; i < messagePackIsPartOfProjectsCount; i++)
                {
                    dto.IsPartOfProjects.Add(ReadGuidBin16(ref reader));
                }
                dto.Mail = ReadRequiredString(ref reader, "Mail");
                dto.Name = ReadRequiredString(ref reader, "Name");
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
                dto.UserPreferences = ReadStringDictionary(ref reader, "UserPreferences");

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
