// ------------------------------------------------------------------------------------------------
//  <copyright file="ReviewSerializer.cs" company="Starion Group S.A.">
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
    /// Serializes an exact <see cref="Review" /> DTO.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    internal static class ReviewSerializer
    {
        /// <summary>
        /// Serializes an exact <see cref="Review" /> instance.
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
        /// <see cref="Review" /> instance.
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

            if (obj.GetType() != typeof(Review))
            {
                throw new NotSupportedException(
                $"Runtime DTO type '{obj.GetType().FullName}' is not supported by ReviewSerializer.");
            }

            var dto = (Review)obj;

            writer.WriteStartObject();
            writer.WriteString("@type"u8, "Review"u8);
            writer.WriteString("@id"u8, dto.Id);
            writer.WritePropertyName(
            "author"u8);
            writer.WriteStartObject();
            writer.WriteString(
            "@id"u8,
            dto.Author);
            writer.WriteEndObject();
            writer.WriteStartArray("comments"u8);

            foreach (var item in dto.Comments)
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
            "description"u8);
            writer.WriteStringValue(
            dto.Description);
            writer.WriteStartArray("reviewers"u8);

            foreach (var item in dto.Reviewers)
            {
                writer.WriteStartObject();
                writer.WriteString("@id"u8, item);
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
            writer.WritePropertyName(
            "sourceBranchId"u8);
            writer.WriteStringValue(
            dto.SourceBranchId);
            writer.WritePropertyName(
            "status"u8);
            writer.WriteStringValue(
            Mycelium.SDK.Extensions.ReviewStatusProvider
            .Format(dto.Status)
            .ToUpperInvariant());
            writer.WritePropertyName(
            "targetBranchId"u8);
            writer.WriteStringValue(
            dto.TargetBranchId);
            writer.WritePropertyName(
            "title"u8);
            writer.WriteStringValue(
            dto.Title);
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
            writer.WriteEndObject();
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
