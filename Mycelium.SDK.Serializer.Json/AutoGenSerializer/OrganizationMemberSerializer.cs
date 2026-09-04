// ------------------------------------------------------------------------------------------------
//  <copyright file="OrganizationMemberSerializer.cs" company="Starion Group S.A.">
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
    /// Serializes an exact <see cref="OrganizationMember" /> DTO.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    internal static class OrganizationMemberSerializer
    {
        /// <summary>
        /// Serializes an exact <see cref="OrganizationMember" /> instance.
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
        /// <see cref="OrganizationMember" /> instance.
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

            if (obj.GetType() != typeof(OrganizationMember))
            {
                throw new NotSupportedException(
                $"Runtime DTO type '{obj.GetType().FullName}' is not supported by OrganizationMemberSerializer.");
            }

            var dto = (OrganizationMember)obj;

            writer.WriteStartObject();
            writer.WriteString("@type"u8, "OrganizationMember"u8);
            writer.WriteString("@id"u8, dto.Id);
            writer.WritePropertyName(
            "createdBy"u8);
            writer.WriteStartObject();
            writer.WriteString(
            "@id"u8,
            dto.CreatedBy);
            writer.WriteEndObject();
            writer.WritePropertyName(
            "createdOn"u8);
            writer.WriteStringValue(
            dto.CreatedOn);
            writer.WritePropertyName(
            "organization"u8);
            writer.WriteStartObject();
            writer.WriteString(
            "@id"u8,
            dto.Organization);
            writer.WriteEndObject();
            writer.WritePropertyName(
            "role"u8);
            writer.WriteStringValue(
            Mycelium.SDK.Extensions.OrganizationMembershipRoleProvider
            .Format(dto.Role)
            .ToUpperInvariant());
            writer.WritePropertyName(
            "updatedBy"u8);
            writer.WriteStartObject();
            writer.WriteString(
            "@id"u8,
            dto.UpdatedBy);
            writer.WriteEndObject();
            writer.WritePropertyName(
            "updatedOn"u8);
            writer.WriteStringValue(
            dto.UpdatedOn);
            writer.WritePropertyName(
            "user"u8);
            writer.WriteStartObject();
            writer.WriteString(
            "@id"u8,
            dto.User);
            writer.WriteEndObject();
            writer.WriteEndObject();
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
