// ------------------------------------------------------------------------------------------------
//  <copyright file="ReviewDeSerializer.cs" company="Starion Group S.A.">
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
    /// Deserializes an exact <see cref="Review" /> DTO.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    internal static class ReviewDeSerializer
    {
        /// <summary>
        /// Deserializes an exact <see cref="Review" /> instance.
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
        /// <see cref="Review" />.
        /// </exception>
        internal static IThing DeSerialize(ref Utf8JsonReader reader, ILoggerFactory loggerFactory = null)
        {
            Utf8JsonReaderHelper.Expect(ref reader, JsonTokenType.StartObject);

            var logger = loggerFactory == null
                ? NullLogger.Instance
                : loggerFactory.CreateLogger("ReviewDeSerializer");

            var dto = new Review();
            var hasType = false;
            var hasId = false;
            var hasEndObject = false;

            var hasAuthor = false;
            var hasComments = false;
            var hasCreatedBy = false;
            var hasCreatedOn = false;
            var hasDescription = false;
            var hasReviewers = false;
            var hasSourceBranchId = false;
            var hasStatus = false;
            var hasTargetBranchId = false;
            var hasTitle = false;
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

                    if (!string.Equals(typeName, "Review", StringComparison.Ordinal))
                    {
                        throw new NotSupportedException($"JSON discriminator '{typeName}' is not supported by ReviewDeSerializer.");
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

                if (reader.ValueTextEquals("author"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    dto.Author = Utf8JsonReaderHelper.ReadGuid(ref reader);

                    hasAuthor = true;
                    continue;
                }
                if (reader.ValueTextEquals("comments"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    Utf8JsonReaderHelper.Expect(ref reader, JsonTokenType.StartArray);

                    dto.Comments.Clear();

                    var hasEndArray = false;

                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.EndArray)
                        {
                            hasEndArray = true;
                            break;
                        }

                        dto.Comments.Add(Utf8JsonReaderHelper.ReadGuid(ref reader));
                    }

                    if (!hasEndArray)
                    {
                        throw new JsonException("The comments array is incomplete.");
                    }

                    hasComments = true;
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
                if (reader.ValueTextEquals("description"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    dto.Description = Utf8JsonReaderHelper.ReadRequiredString(ref reader);

                    hasDescription = true;
                    continue;
                }
                if (reader.ValueTextEquals("reviewers"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    Utf8JsonReaderHelper.Expect(ref reader, JsonTokenType.StartArray);

                    dto.Reviewers.Clear();

                    var hasEndArray = false;

                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.EndArray)
                        {
                            hasEndArray = true;
                            break;
                        }

                        dto.Reviewers.Add(Utf8JsonReaderHelper.ReadGuid(ref reader));
                    }

                    if (!hasEndArray)
                    {
                        throw new JsonException("The reviewers array is incomplete.");
                    }

                    hasReviewers = true;
                    continue;
                }
                if (reader.ValueTextEquals("sourceBranchId"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    dto.SourceBranchId = Utf8JsonReaderHelper.ReadGuid(ref reader);

                    hasSourceBranchId = true;
                    continue;
                }
                if (reader.ValueTextEquals("status"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    dto.Status = ReviewStatusDeSerializer.DeSerialize(ref reader);

                    hasStatus = true;
                    continue;
                }
                if (reader.ValueTextEquals("targetBranchId"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    dto.TargetBranchId = Utf8JsonReaderHelper.ReadGuid(ref reader);

                    hasTargetBranchId = true;
                    continue;
                }
                if (reader.ValueTextEquals("title"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    dto.Title = Utf8JsonReaderHelper.ReadRequiredString(ref reader);

                    hasTitle = true;
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

            if (!hasAuthor)
            {
                logger.LogDebug(
                    "The {PropertyName} JSON property was not found in {DtoType}: {Id}. The construction default is retained.",
                    "author",
                    "Review",
                    dto.Id);
            }
            if (!hasComments)
            {
                logger.LogDebug(
                    "The {PropertyName} JSON property was not found in {DtoType}: {Id}. The construction default is retained.",
                    "comments",
                    "Review",
                    dto.Id);
            }
            if (!hasCreatedBy)
            {
                logger.LogDebug(
                    "The {PropertyName} JSON property was not found in {DtoType}: {Id}. The construction default is retained.",
                    "createdBy",
                    "Review",
                    dto.Id);
            }
            if (!hasCreatedOn)
            {
                logger.LogDebug(
                    "The {PropertyName} JSON property was not found in {DtoType}: {Id}. The construction default is retained.",
                    "createdOn",
                    "Review",
                    dto.Id);
            }
            if (!hasDescription)
            {
                logger.LogDebug(
                    "The {PropertyName} JSON property was not found in {DtoType}: {Id}. The construction default is retained.",
                    "description",
                    "Review",
                    dto.Id);
            }
            if (!hasReviewers)
            {
                logger.LogDebug(
                    "The {PropertyName} JSON property was not found in {DtoType}: {Id}. The construction default is retained.",
                    "reviewers",
                    "Review",
                    dto.Id);
            }
            if (!hasSourceBranchId)
            {
                logger.LogDebug(
                    "The {PropertyName} JSON property was not found in {DtoType}: {Id}. The construction default is retained.",
                    "sourceBranchId",
                    "Review",
                    dto.Id);
            }
            if (!hasStatus)
            {
                logger.LogDebug(
                    "The {PropertyName} JSON property was not found in {DtoType}: {Id}. The construction default is retained.",
                    "status",
                    "Review",
                    dto.Id);
            }
            if (!hasTargetBranchId)
            {
                logger.LogDebug(
                    "The {PropertyName} JSON property was not found in {DtoType}: {Id}. The construction default is retained.",
                    "targetBranchId",
                    "Review",
                    dto.Id);
            }
            if (!hasTitle)
            {
                logger.LogDebug(
                    "The {PropertyName} JSON property was not found in {DtoType}: {Id}. The construction default is retained.",
                    "title",
                    "Review",
                    dto.Id);
            }
            if (!hasUpdatedBy)
            {
                logger.LogDebug(
                    "The {PropertyName} JSON property was not found in {DtoType}: {Id}. The construction default is retained.",
                    "updatedBy",
                    "Review",
                    dto.Id);
            }
            if (!hasUpdatedOn)
            {
                logger.LogDebug(
                    "The {PropertyName} JSON property was not found in {DtoType}: {Id}. The construction default is retained.",
                    "updatedOn",
                    "Review",
                    dto.Id);
            }

            return dto;
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
