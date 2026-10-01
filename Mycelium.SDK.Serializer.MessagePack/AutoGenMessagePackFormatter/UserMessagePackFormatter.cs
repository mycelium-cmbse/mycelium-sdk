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
    public sealed partial class UserMessagePackFormatter : MessagePackFormatterBase, IMessagePackFormatter<User>
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
        public void Serialize(ref MessagePackWriter writer, User dto, MessagePackSerializerOptions options)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            writer.WriteArrayHeader(12);

            GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.Id, options);
            GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.CreatedBy, options);
            WriteRoundTripDateTime(ref writer, dto.CreatedOn);
            WriteRequiredString(ref writer, dto.ExternalIdentifier, "ExternalIdentifier");
            if (dto.IsPartOfOrganizations == null)
            {
                writer.WriteNil();
            }
            else
            {
                writer.WriteArrayHeader(dto.IsPartOfOrganizations.Count);

                for (var i = 0; i < dto.IsPartOfOrganizations.Count; i++)
                {
                    GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.IsPartOfOrganizations[i], options);
                }
            }
            if (dto.IsPartOfProjects == null)
            {
                writer.WriteNil();
            }
            else
            {
                writer.WriteArrayHeader(dto.IsPartOfProjects.Count);

                for (var i = 0; i < dto.IsPartOfProjects.Count; i++)
                {
                    GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.IsPartOfProjects[i], options);
                }
            }
            WriteRequiredString(ref writer, dto.Mail, "Mail");
            WriteRequiredString(ref writer, dto.Name, "Name");
            ActivationStatusMessagePackFormatter.Instance.Serialize(ref writer, dto.Status, options);
            GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.UpdatedBy, options);
            WriteRoundTripDateTime(ref writer, dto.UpdatedOn);
            if (dto.UserPreferences == null)
            {
                writer.WriteNil();
            }
            else
            {
                StringDictionaryMessagePackFormatter.Instance.Serialize(ref writer, dto.UserPreferences, options);
            }
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
        public User Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                throw new MessagePackSerializationException("User may not be nil.");
            }

            options.Security.DepthStep(ref reader);

            try
            {
                var fieldCount = reader.ReadArrayHeader();

                if (fieldCount != 12)
                {
                    throw new MessagePackSerializationException($"User contains {fieldCount} fields; exactly 12 fields are required.");
                }

                var dto = new User();

                dto.Id = GuidMessagePackFormatter.Instance.Deserialize(ref reader, options);
                dto.CreatedBy = GuidMessagePackFormatter.Instance.Deserialize(ref reader, options);
                dto.CreatedOn = ReadRoundTripDateTime(ref reader, "CreatedOn");
                dto.ExternalIdentifier = ReadRequiredString(ref reader, "ExternalIdentifier");
                if (reader.TryReadNil())
                {
                    dto.IsPartOfOrganizations = null;
                }
                else
                {
                    var messagePackIsPartOfOrganizationsCount = reader.ReadArrayHeader();
                    dto.IsPartOfOrganizations.Clear();

                    if (dto.IsPartOfOrganizations.Capacity < messagePackIsPartOfOrganizationsCount)
                    {
                        dto.IsPartOfOrganizations.Capacity = messagePackIsPartOfOrganizationsCount;
                    }

                    for (var i = 0; i < messagePackIsPartOfOrganizationsCount; i++)
                    {
                        dto.IsPartOfOrganizations.Add(GuidMessagePackFormatter.Instance.Deserialize(ref reader, options));
                    }
                }
                if (reader.TryReadNil())
                {
                    dto.IsPartOfProjects = null;
                }
                else
                {
                    var messagePackIsPartOfProjectsCount = reader.ReadArrayHeader();
                    dto.IsPartOfProjects.Clear();

                    if (dto.IsPartOfProjects.Capacity < messagePackIsPartOfProjectsCount)
                    {
                        dto.IsPartOfProjects.Capacity = messagePackIsPartOfProjectsCount;
                    }

                    for (var i = 0; i < messagePackIsPartOfProjectsCount; i++)
                    {
                        dto.IsPartOfProjects.Add(GuidMessagePackFormatter.Instance.Deserialize(ref reader, options));
                    }
                }
                dto.Mail = ReadRequiredString(ref reader, "Mail");
                dto.Name = ReadRequiredString(ref reader, "Name");
                dto.Status = ActivationStatusMessagePackFormatter.Instance.Deserialize(ref reader, options);
                dto.UpdatedBy = GuidMessagePackFormatter.Instance.Deserialize(ref reader, options);
                dto.UpdatedOn = ReadRoundTripDateTime(ref reader, "UpdatedOn");
                if (reader.TryReadNil())
                {
                    dto.UserPreferences = null;
                }
                else
                {
                    dto.UserPreferences = StringDictionaryMessagePackFormatter.Instance.Deserialize(ref reader, options);
                }

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
