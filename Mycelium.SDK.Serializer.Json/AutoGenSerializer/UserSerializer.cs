// ------------------------------------------------------------------------------------------------
//  <copyright file="UserSerializer.cs" company="Starion Group S.A.">
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
    /// Serializes an exact <see cref="User" /> DTO.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    internal static class UserSerializer
    {
        /// <summary>
        /// Serializes an exact <see cref="User" /> instance.
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
        /// <see cref="User" /> instance.
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

            if (obj.GetType() != typeof(User))
            {
                throw new NotSupportedException(
                $"Runtime DTO type '{obj.GetType().FullName}' is not supported by UserSerializer.");
            }

            var dto = (User)obj;

            writer.WriteStartObject();
            writer.WriteString("@type"u8, "User"u8);
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
            "externalIdentifier"u8);
            writer.WriteStringValue(
            dto.ExternalIdentifier);
            writer.WriteStartArray("isPartOfOrganizations"u8);

            foreach (var item in dto.IsPartOfOrganizations)
            {
                writer.WriteStartObject();
                writer.WriteString("@id"u8, item);
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
            writer.WriteStartArray("isPartOfProjects"u8);

            foreach (var item in dto.IsPartOfProjects)
            {
                writer.WriteStartObject();
                writer.WriteString("@id"u8, item);
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
            writer.WritePropertyName(
            "mail"u8);
            writer.WriteStringValue(
            dto.Mail);
            writer.WritePropertyName(
            "name"u8);
            writer.WriteStringValue(
            dto.Name);
            writer.WritePropertyName(
            "status"u8);
            writer.WriteStringValue(
            Mycelium.SDK.Extensions.ActivationStatusProvider
            .Format(dto.Status)
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
            writer.WriteStartObject("userPreferences"u8);

            foreach (var entry in System.Linq.Enumerable.OrderBy(
            dto.UserPreferences,
            entry => entry.Key,
            StringComparer.Ordinal))
            {
                writer.WriteString(entry.Key, entry.Value);
            }

            writer.WriteEndObject();
            writer.WriteEndObject();
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
