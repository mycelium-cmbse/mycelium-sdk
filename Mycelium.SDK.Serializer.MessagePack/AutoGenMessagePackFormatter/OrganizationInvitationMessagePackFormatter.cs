// ------------------------------------------------------------------------------------------------
//  <copyright file="OrganizationInvitationMessagePackFormatter.cs" company="Starion Group S.A.">
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
    /// <see cref="OrganizationInvitation" /> DTO using MessagePack.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public sealed partial class OrganizationInvitationMessagePackFormatter : MessagePackFormatterBase, IMessagePackFormatter<OrganizationInvitation>
    {
        /// <summary>
        /// Serializes an exact <see cref="OrganizationInvitation" /> DTO.
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
        public void Serialize(ref MessagePackWriter writer, OrganizationInvitation dto, MessagePackSerializerOptions options)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            writer.WriteArrayHeader(12);

            GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.Id, options);
            GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.CreatedBy, options);
            WriteRoundTripDateTime(ref writer, dto.CreatedOn);
            WriteRoundTripDateTime(ref writer, dto.ExpiresOn);
            if (dto.Invitee.HasValue)
            {
                GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.Invitee.Value, options);
            }
            else
            {
                writer.WriteNil();
            }
            writer.Write(dto.InviteeEmail);
            GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.Organization, options);
            if (dto.RespondedOn.HasValue)
            {
                WriteRoundTripDateTime(ref writer, dto.RespondedOn.Value);
            }
            else
            {
                writer.WriteNil();
            }
            OrganizationMembershipRoleMessagePackFormatter.Instance.Serialize(ref writer, dto.Role, options);
            InvitationStatusMessagePackFormatter.Instance.Serialize(ref writer, dto.Status, options);
            GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.UpdatedBy, options);
            WriteRoundTripDateTime(ref writer, dto.UpdatedOn);
        }

        /// <summary>
        /// Deserializes an exact <see cref="OrganizationInvitation" /> DTO.
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
        public OrganizationInvitation Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                throw new MessagePackSerializationException("OrganizationInvitation may not be nil.");
            }

            options.Security.DepthStep(ref reader);

            try
            {
                var fieldCount = reader.ReadArrayHeader();

                if (fieldCount != 12)
                {
                    throw new MessagePackSerializationException($"OrganizationInvitation contains {fieldCount} fields; exactly 12 fields are required.");
                }

                var dto = new OrganizationInvitation();

                dto.Id = GuidMessagePackFormatter.Instance.Deserialize(ref reader, options);
                dto.CreatedBy = GuidMessagePackFormatter.Instance.Deserialize(ref reader, options);
                dto.CreatedOn = ReadRoundTripDateTime(ref reader, "CreatedOn");
                dto.ExpiresOn = ReadRoundTripDateTime(ref reader, "ExpiresOn");
                if (reader.TryReadNil())
                {
                    dto.Invitee = null;
                }
                else
                {
                    dto.Invitee = GuidMessagePackFormatter.Instance.Deserialize(ref reader, options);
                }
                dto.InviteeEmail = reader.ReadString();
                dto.Organization = GuidMessagePackFormatter.Instance.Deserialize(ref reader, options);
                if (reader.TryReadNil())
                {
                    dto.RespondedOn = null;
                }
                else
                {
                    dto.RespondedOn = ReadRoundTripDateTime(ref reader, "RespondedOn");
                }
                dto.Role = OrganizationMembershipRoleMessagePackFormatter.Instance.Deserialize(ref reader, options);
                dto.Status = InvitationStatusMessagePackFormatter.Instance.Deserialize(ref reader, options);
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
