// ------------------------------------------------------------------------------------------------
//  <copyright file="BranchProtectionRuleSerializer.cs" company="Starion Group S.A.">
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
    /// Serializes an exact <see cref="BranchProtectionRule" /> DTO.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    internal static class BranchProtectionRuleSerializer
    {
        /// <summary>
        /// Serializes an exact <see cref="BranchProtectionRule" /> instance.
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
        /// <see cref="BranchProtectionRule" /> instance.
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

            if (obj.GetType() != typeof(BranchProtectionRule))
            {
                throw new NotSupportedException(
                $"Runtime DTO type '{obj.GetType().FullName}' is not supported by BranchProtectionRuleSerializer.");
            }

            var dto = (BranchProtectionRule)obj;

            writer.WriteStartObject();
            writer.WriteString("@type"u8, "BranchProtectionRule"u8);
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
            writer.WriteStartArray("defaultReviewers"u8);

            foreach (var item in dto.DefaultReviewers)
            {
                writer.WriteStartObject();
                writer.WriteString("@id"u8, item);
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
            writer.WritePropertyName(
            "engineeringBranchId"u8);
            writer.WriteStringValue(
            dto.EngineeringBranchId);
            writer.WriteStartArray("mergeAllowedFor"u8);

            foreach (var item in dto.MergeAllowedFor)
            {
                writer.WriteStringValue(
                Mycelium.SDK.Extensions.ProjectMemberRoleProvider
                .Format(item)
                .ToUpperInvariant());
            }

            writer.WriteEndArray();
            writer.WritePropertyName(
            "minimumRequiredApproval"u8);
            writer.WriteNumberValue(
            dto.MinimumRequiredApproval);
            writer.WritePropertyName(
            "name"u8);
            writer.WriteStringValue(
            dto.Name);
            writer.WritePropertyName(
            "reviewRequired"u8);
            writer.WriteBooleanValue(
            dto.ReviewRequired);
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
