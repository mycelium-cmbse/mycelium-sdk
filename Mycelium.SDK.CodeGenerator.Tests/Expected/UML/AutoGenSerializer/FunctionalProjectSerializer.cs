// ------------------------------------------------------------------------------------------------
//  <copyright file="FunctionalProjectSerializer.cs" company="Starion Group S.A.">
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
    /// Serializes an exact <see cref="FunctionalProject" /> DTO.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    internal static class FunctionalProjectSerializer
    {
        /// <summary>
        /// Serializes an exact <see cref="FunctionalProject" /> instance.
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
        /// <see cref="FunctionalProject" /> instance.
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

            if (obj.GetType() != typeof(FunctionalProject))
            {
                throw new NotSupportedException(
                $"Runtime DTO type '{obj.GetType().FullName}' is not supported by FunctionalProjectSerializer.");
            }

            var dto = (FunctionalProject)obj;

            writer.WriteStartObject();
            writer.WriteString("@type"u8, "FunctionalProject"u8);
            writer.WriteString("@id"u8, dto.Id);
            writer.WritePropertyName(
            "belongsTo"u8);
            writer.WriteStartObject();
            writer.WriteString(
            "@id"u8,
            dto.BelongsTo);
            writer.WriteEndObject();
            writer.WriteStartArray("branchRules"u8);

            foreach (var item in dto.BranchRules)
            {
                writer.WriteStartObject();
                writer.WriteString("@id"u8, item);
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
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
            "currentMode"u8);
            writer.WriteStringValue(
            Mycelium.SDK.Extensions.ProjectModeProvider
            .Format(dto.CurrentMode)
            .ToUpperInvariant());
            writer.WriteStartArray("defines"u8);

            foreach (var item in dto.Defines)
            {
                writer.WriteStartObject();
                writer.WriteString("@id"u8, item);
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
            writer.WritePropertyName(
            "description"u8);
            writer.WriteStringValue(
            dto.Description);
            writer.WritePropertyName(
            "engineeringProjectId"u8);
            writer.WriteStringValue(
            dto.EngineeringProjectId);
            writer.WriteStartArray("involves"u8);

            foreach (var item in dto.Involves)
            {
                writer.WriteStartObject();
                writer.WriteString("@id"u8, item);
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
            writer.WritePropertyName(
            "lifecycle"u8);
            writer.WriteStringValue(
            Mycelium.SDK.Extensions.ProjectLifecycleKindProvider
            .Format(dto.Lifecycle)
            .ToUpperInvariant());
            writer.WritePropertyName(
            "name"u8);
            writer.WriteStringValue(
            dto.Name);
            writer.WritePropertyName(
            "policy"u8);
            writer.WriteStartObject();
            writer.WriteString(
            "@id"u8,
            dto.Policy);
            writer.WriteEndObject();
            writer.WriteStartArray("reviews"u8);

            foreach (var item in dto.Reviews)
            {
                writer.WriteStartObject();
                writer.WriteString("@id"u8, item);
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
            writer.WriteStartObject("sharedPreferences"u8);

            foreach (var entry in System.Linq.Enumerable.OrderBy(
            dto.SharedPreferences,
            entry => entry.Key,
            StringComparer.Ordinal))
            {
                writer.WriteString(entry.Key, entry.Value);
            }

            writer.WriteEndObject();
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
            "visibility"u8);
            writer.WriteStringValue(
            Mycelium.SDK.Extensions.ProjectVisibilityProvider
            .Format(dto.Visibility)
            .ToUpperInvariant());
            writer.WriteEndObject();
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
