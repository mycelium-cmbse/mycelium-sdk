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
    public sealed partial class OrganizationMessagePackFormatter : MessagePackFormatterBase, IMessagePackFormatter<Organization>
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
        public void Serialize(ref MessagePackWriter writer, Organization dto, MessagePackSerializerOptions options)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            writer.WriteArrayHeader(11);

            GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.Id, options);
            GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.CreatedBy, options);
            WriteRoundTripDateTime(ref writer, dto.CreatedOn);
            WriteRequiredString(ref writer, dto.Description, "Description");
            if (dto.InvolvedUser == null)
            {
                writer.WriteNil();
            }
            else
            {
                writer.WriteArrayHeader(dto.InvolvedUser.Count);

                for (var i = 0; i < dto.InvolvedUser.Count; i++)
                {
                    GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.InvolvedUser[i], options);
                }
            }
            WriteRequiredString(ref writer, dto.Name, "Name");
            GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.Policy, options);
            if (dto.Projects == null)
            {
                writer.WriteNil();
            }
            else
            {
                writer.WriteArrayHeader(dto.Projects.Count);

                for (var i = 0; i < dto.Projects.Count; i++)
                {
                    GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.Projects[i], options);
                }
            }
            ActivationStatusMessagePackFormatter.Instance.Serialize(ref writer, dto.Status, options);
            GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.UpdatedBy, options);
            WriteRoundTripDateTime(ref writer, dto.UpdatedOn);
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
        public Organization Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
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

                dto.Id = GuidMessagePackFormatter.Instance.Deserialize(ref reader, options);
                dto.CreatedBy = GuidMessagePackFormatter.Instance.Deserialize(ref reader, options);
                dto.CreatedOn = ReadRoundTripDateTime(ref reader, "CreatedOn");
                dto.Description = ReadRequiredString(ref reader, "Description");
                if (reader.TryReadNil())
                {
                    dto.InvolvedUser = null;
                }
                else
                {
                    var messagePackInvolvedUserCount = reader.ReadArrayHeader();
                    dto.InvolvedUser.Clear();

                    if (dto.InvolvedUser.Capacity < messagePackInvolvedUserCount)
                    {
                        dto.InvolvedUser.Capacity = messagePackInvolvedUserCount;
                    }

                    for (var i = 0; i < messagePackInvolvedUserCount; i++)
                    {
                        dto.InvolvedUser.Add(GuidMessagePackFormatter.Instance.Deserialize(ref reader, options));
                    }
                }
                dto.Name = ReadRequiredString(ref reader, "Name");
                dto.Policy = GuidMessagePackFormatter.Instance.Deserialize(ref reader, options);
                if (reader.TryReadNil())
                {
                    dto.Projects = null;
                }
                else
                {
                    var messagePackProjectsCount = reader.ReadArrayHeader();
                    dto.Projects.Clear();

                    if (dto.Projects.Capacity < messagePackProjectsCount)
                    {
                        dto.Projects.Capacity = messagePackProjectsCount;
                    }

                    for (var i = 0; i < messagePackProjectsCount; i++)
                    {
                        dto.Projects.Add(GuidMessagePackFormatter.Instance.Deserialize(ref reader, options));
                    }
                }
                dto.Status = ActivationStatusMessagePackFormatter.Instance.Deserialize(ref reader, options);
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
