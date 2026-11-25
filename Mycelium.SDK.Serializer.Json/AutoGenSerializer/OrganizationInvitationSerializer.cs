// ------------------------------------------------------------------------------------------------
//  <copyright file="OrganizationInvitationSerializer.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.Serializer.Json
{
    using System;
    using System.CodeDom.Compiler;
    using System.Text.Json;

    using Mycelium.SDK.DTO;

    /// <summary>
    /// Serializes an exact <see cref="OrganizationInvitation" /> DTO.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    internal static class OrganizationInvitationSerializer
    {
        /// <summary>
        /// Serializes an exact <see cref="OrganizationInvitation" /> instance.
        /// </summary>
        /// <param name="obj">
        /// The exact concrete DTO to serialize.
        /// </param>
        /// <param name="writer">
        /// The JSON writer that receives the serialized object.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="obj" /> or <paramref name="writer" /> is
        /// <see langword="null" />.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Thrown when <paramref name="obj" /> is not an exact
        /// <see cref="OrganizationInvitation" /> instance.
        /// </exception>
        internal static void Serialize(object obj, Utf8JsonWriter writer)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            if (obj.GetType() != typeof(OrganizationInvitation))
            {
                throw new NotSupportedException($"Runtime DTO type '{obj.GetType().FullName}' is not supported by OrganizationInvitationSerializer.");
            }

            var dto = (OrganizationInvitation)obj;

            writer.WriteStartObject();
            writer.WriteString("@type"u8, "OrganizationInvitation"u8);
            writer.WriteString("@id"u8, dto.Id);
            writer.WritePropertyName("createdBy"u8);
            writer.WriteStringValue(dto.CreatedBy);
            writer.WritePropertyName("createdOn"u8);
            writer.WriteStringValue(dto.CreatedOn.ToString("o", System.Globalization.CultureInfo.InvariantCulture));
            writer.WritePropertyName("expiresOn"u8);
            writer.WriteStringValue(dto.ExpiresOn.ToString("o", System.Globalization.CultureInfo.InvariantCulture));
            writer.WritePropertyName("invitee"u8);
            if (dto.Invitee.HasValue)
            {
                writer.WriteStringValue(dto.Invitee.Value);
            }
            else
            {
                writer.WriteNullValue();
            }
            writer.WritePropertyName("inviteeEmail"u8);
            writer.WriteStringValue(dto.InviteeEmail);
            writer.WritePropertyName("organization"u8);
            writer.WriteStringValue(dto.Organization);
            writer.WritePropertyName("respondedOn"u8);
            if (dto.RespondedOn.HasValue)
            {
                writer.WriteStringValue(dto.RespondedOn.Value.ToString("o", System.Globalization.CultureInfo.InvariantCulture));
            }
            else
            {
                writer.WriteNullValue();
            }
            writer.WritePropertyName("role"u8);
            writer.WriteStringValue(Mycelium.SDK.Extensions.OrganizationMembershipRoleProvider.Format(dto.Role).ToUpperInvariant());
            writer.WritePropertyName("status"u8);
            writer.WriteStringValue(Mycelium.SDK.Extensions.InvitationStatusProvider.Format(dto.Status).ToUpperInvariant());
            writer.WritePropertyName("updatedBy"u8);
            writer.WriteStringValue(dto.UpdatedBy);
            writer.WritePropertyName("updatedOn"u8);
            writer.WriteStringValue(dto.UpdatedOn.ToString("o", System.Globalization.CultureInfo.InvariantCulture));
            writer.WriteEndObject();
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
