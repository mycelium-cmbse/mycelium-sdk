// ------------------------------------------------------------------------------------------------
//  <copyright file="BranchProtectionRuleDeSerializer.cs" company="Starion Group S.A.">
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

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    using Mycelium.SDK.DTO;
    using Mycelium.SDK.Serializer.Json.Utility;

    /// <summary>
    /// Deserializes an exact <see cref="BranchProtectionRule" /> DTO.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    internal static class BranchProtectionRuleDeSerializer
    {
        /// <summary>
        /// Deserializes an exact <see cref="BranchProtectionRule" /> instance.
        /// </summary>
        /// <param name="reader">
        /// The JSON reader positioned on the object-start token.
        /// </param>
        /// <param name="loggerFactory">
        /// The optional logger factory used to produce missing-property diagnostics.
        /// </param>
        /// <returns>
        /// The deserialized DTO.
        /// </returns>
        /// <exception cref="JsonException">
        /// Thrown when the JSON object, metadata, reference, or modeled property value is invalid.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Thrown when the exact <c>@type</c> discriminator does not identify
        /// <see cref="BranchProtectionRule" />.
        /// </exception>
        internal static IThing DeSerialize(ref Utf8JsonReader reader, ILoggerFactory loggerFactory = null)
        {
            Utf8JsonReaderHelper.Expect(ref reader, JsonTokenType.StartObject);

            var logger = loggerFactory == null
                ? NullLogger.Instance
                : loggerFactory.CreateLogger("BranchProtectionRuleDeSerializer");

            var dto = new BranchProtectionRule();
            var hasType = false;
            var hasId = false;
            var hasEndObject = false;

            var hasCreatedBy = false;
            var hasCreatedOn = false;
            var hasDefaultReviewers = false;
            var hasEngineeringBranchId = false;
            var hasMergeAllowedFor = false;
            var hasMinimumRequiredApproval = false;
            var hasName = false;
            var hasReviewRequired = false;
            var hasUpdatedBy = false;
            var hasUpdatedOn = false;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    hasEndObject = true;
                    break;
                }

                Utf8JsonReaderHelper.Expect(ref reader, JsonTokenType.PropertyName);

                if (reader.ValueTextEquals("@type"u8))
                {
                    if (hasType)
                    {
                        throw new JsonException("The @type metadata property occurs more than once.");
                    }

                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    var typeName = Utf8JsonReaderHelper.ReadRequiredString(ref reader);

                    if (!string.Equals(typeName, "BranchProtectionRule", StringComparison.Ordinal))
                    {
                        throw new NotSupportedException($"JSON discriminator '{typeName}' is not supported by BranchProtectionRuleDeSerializer.");
                    }

                    hasType = true;
                    continue;
                }

                if (reader.ValueTextEquals("@id"u8))
                {
                    if (hasId)
                    {
                        throw new JsonException("The @id metadata property occurs more than once.");
                    }

                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    dto.Id = Utf8JsonReaderHelper.ReadGuid(ref reader);
                    hasId = true;
                    continue;
                }

                if (reader.ValueTextEquals("createdBy"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    dto.CreatedBy = Utf8JsonReaderHelper.ReadGuid(ref reader);

                    hasCreatedBy = true;
                    continue;
                }
                if (reader.ValueTextEquals("createdOn"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    dto.CreatedOn = Utf8JsonReaderHelper.ReadDateTime(ref reader);

                    hasCreatedOn = true;
                    continue;
                }
                if (reader.ValueTextEquals("defaultReviewers"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    Utf8JsonReaderHelper.Expect(ref reader, JsonTokenType.StartArray);

                    dto.DefaultReviewers.Clear();

                    var hasEndArray = false;

                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.EndArray)
                        {
                            hasEndArray = true;
                            break;
                        }

                        dto.DefaultReviewers.Add(Utf8JsonReaderHelper.ReadGuid(ref reader));
                    }

                    if (!hasEndArray)
                    {
                        throw new JsonException("The defaultReviewers array is incomplete.");
                    }

                    hasDefaultReviewers = true;
                    continue;
                }
                if (reader.ValueTextEquals("engineeringBranchId"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    dto.EngineeringBranchId = Utf8JsonReaderHelper.ReadGuid(ref reader);

                    hasEngineeringBranchId = true;
                    continue;
                }
                if (reader.ValueTextEquals("mergeAllowedFor"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    Utf8JsonReaderHelper.Expect(ref reader, JsonTokenType.StartArray);

                    dto.MergeAllowedFor.Clear();

                    var hasEndArray = false;

                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.EndArray)
                        {
                            hasEndArray = true;
                            break;
                        }

                        dto.MergeAllowedFor.Add(ProjectMemberRoleDeSerializer.DeSerialize(ref reader));
                    }

                    if (!hasEndArray)
                    {
                        throw new JsonException("The mergeAllowedFor array is incomplete.");
                    }

                    hasMergeAllowedFor = true;
                    continue;
                }
                if (reader.ValueTextEquals("minimumRequiredApproval"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    dto.MinimumRequiredApproval = Utf8JsonReaderHelper.ReadInt32(ref reader);

                    hasMinimumRequiredApproval = true;
                    continue;
                }
                if (reader.ValueTextEquals("name"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    dto.Name = Utf8JsonReaderHelper.ReadRequiredString(ref reader);

                    hasName = true;
                    continue;
                }
                if (reader.ValueTextEquals("reviewRequired"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    dto.ReviewRequired = Utf8JsonReaderHelper.ReadBoolean(ref reader);

                    hasReviewRequired = true;
                    continue;
                }
                if (reader.ValueTextEquals("updatedBy"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    dto.UpdatedBy = Utf8JsonReaderHelper.ReadGuid(ref reader);

                    hasUpdatedBy = true;
                    continue;
                }
                if (reader.ValueTextEquals("updatedOn"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    dto.UpdatedOn = Utf8JsonReaderHelper.ReadDateTime(ref reader);

                    hasUpdatedOn = true;
                    continue;
                }

                Utf8JsonReaderHelper.ReadNext(ref reader);
                Utf8JsonReaderHelper.SkipValue(ref reader);
            }

            if (!hasEndObject)
            {
                throw new JsonException("The JSON DTO object is incomplete.");
            }

            if (!hasType)
            {
                throw new JsonException("The required @type metadata property is missing.");
            }

            if (!hasId)
            {
                throw new JsonException("The required @id metadata property is missing.");
            }

            if (!hasCreatedBy)
            {
                logger.LogDebug(
                    "The {PropertyName} JSON property was not found in {DtoType}: {Id}. The construction default is retained.",
                    "createdBy",
                    "BranchProtectionRule",
                    dto.Id);
            }
            if (!hasCreatedOn)
            {
                logger.LogDebug(
                    "The {PropertyName} JSON property was not found in {DtoType}: {Id}. The construction default is retained.",
                    "createdOn",
                    "BranchProtectionRule",
                    dto.Id);
            }
            if (!hasDefaultReviewers)
            {
                logger.LogDebug(
                    "The {PropertyName} JSON property was not found in {DtoType}: {Id}. The construction default is retained.",
                    "defaultReviewers",
                    "BranchProtectionRule",
                    dto.Id);
            }
            if (!hasEngineeringBranchId)
            {
                logger.LogDebug(
                    "The {PropertyName} JSON property was not found in {DtoType}: {Id}. The construction default is retained.",
                    "engineeringBranchId",
                    "BranchProtectionRule",
                    dto.Id);
            }
            if (!hasMergeAllowedFor)
            {
                logger.LogDebug(
                    "The {PropertyName} JSON property was not found in {DtoType}: {Id}. The construction default is retained.",
                    "mergeAllowedFor",
                    "BranchProtectionRule",
                    dto.Id);
            }
            if (!hasMinimumRequiredApproval)
            {
                logger.LogDebug(
                    "The {PropertyName} JSON property was not found in {DtoType}: {Id}. The construction default is retained.",
                    "minimumRequiredApproval",
                    "BranchProtectionRule",
                    dto.Id);
            }
            if (!hasName)
            {
                logger.LogDebug(
                    "The {PropertyName} JSON property was not found in {DtoType}: {Id}. The construction default is retained.",
                    "name",
                    "BranchProtectionRule",
                    dto.Id);
            }
            if (!hasReviewRequired)
            {
                logger.LogDebug(
                    "The {PropertyName} JSON property was not found in {DtoType}: {Id}. The construction default is retained.",
                    "reviewRequired",
                    "BranchProtectionRule",
                    dto.Id);
            }
            if (!hasUpdatedBy)
            {
                logger.LogDebug(
                    "The {PropertyName} JSON property was not found in {DtoType}: {Id}. The construction default is retained.",
                    "updatedBy",
                    "BranchProtectionRule",
                    dto.Id);
            }
            if (!hasUpdatedOn)
            {
                logger.LogDebug(
                    "The {PropertyName} JSON property was not found in {DtoType}: {Id}. The construction default is retained.",
                    "updatedOn",
                    "BranchProtectionRule",
                    dto.Id);
            }

            return dto;
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
