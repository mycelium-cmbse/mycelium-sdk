// ------------------------------------------------------------------------------------------------
//  <copyright file="OrganizationMemberMessagePackFormatter.cs" company="Starion Group S.A.">
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
    /// <see cref="OrganizationMember" /> DTO using MessagePack.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public sealed partial class OrganizationMemberMessagePackFormatter :
        MessagePackFormatterBase,
        IMessagePackFormatter<OrganizationMember>
    {
        /// <summary>
        /// Serializes an exact <see cref="OrganizationMember" /> DTO.
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
            OrganizationMember dto,
            MessagePackSerializerOptions options)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            writer.WriteArrayHeader(8);

            WriteGuidBin16(ref writer, dto.Id);
            WriteGuidBin16(ref writer, dto.CreatedBy);
            writer.Write(dto.CreatedOn);
            WriteGuidBin16(ref writer, dto.Organization);
            writer.Write(dto.Role switch
            {
                OrganizationMembershipRole.Administrator => "administrator",
                OrganizationMembershipRole.Member => "member",
                OrganizationMembershipRole.Owner => "owner",
                _ => throw new MessagePackSerializationException($"Value '{dto.Role}' is not valid for OrganizationMembershipRole."),
            });
            WriteGuidBin16(ref writer, dto.UpdatedBy);
            writer.Write(dto.UpdatedOn);
            WriteGuidBin16(ref writer, dto.User);
        }

        /// <summary>
        /// Deserializes an exact <see cref="OrganizationMember" /> DTO.
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
        public OrganizationMember Deserialize(
            ref MessagePackReader reader,
            MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                throw new MessagePackSerializationException(
                    "OrganizationMember may not be nil.");
            }

            options.Security.DepthStep(ref reader);

            try
            {
                var fieldCount = reader.ReadArrayHeader();

                if (fieldCount != 8)
                {
                    throw new MessagePackSerializationException(
                        $"OrganizationMember contains {fieldCount} fields; exactly 8 fields are required.");
                }

                var dto = new OrganizationMember();

                dto.Id = ReadGuidBin16(ref reader);
                dto.CreatedBy = ReadGuidBin16(ref reader);
                dto.CreatedOn = reader.ReadDateTime();
                dto.Organization = ReadGuidBin16(ref reader);
                var messagePackRoleValue = ReadRequiredString(ref reader, "Role");
                dto.Role = messagePackRoleValue switch
                {
                    "administrator" => OrganizationMembershipRole.Administrator,
                    "member" => OrganizationMembershipRole.Member,
                    "owner" => OrganizationMembershipRole.Owner,
                    _ => throw new MessagePackSerializationException($"Value '{messagePackRoleValue}' is not valid for OrganizationMembershipRole."),
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
