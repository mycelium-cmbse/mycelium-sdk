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
        internal static IThing DeSerialize(ref Utf8JsonReader reader)
        {
            Utf8JsonReaderHelper.Expect(ref reader, JsonTokenType.StartObject);

            var dto = new Review();
            var hasType = false;
            var hasId = false;
            var hasEndObject = false;

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

                    continue;
                }
                if (reader.ValueTextEquals("createdBy"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    dto.CreatedBy = Utf8JsonReaderHelper.ReadGuid(ref reader);

                    continue;
                }
                if (reader.ValueTextEquals("createdOn"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    dto.CreatedOn = Utf8JsonReaderHelper.ReadDateTime(ref reader);

                    continue;
                }
                if (reader.ValueTextEquals("description"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    dto.Description = Utf8JsonReaderHelper.ReadRequiredString(ref reader);

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

                    continue;
                }
                if (reader.ValueTextEquals("sourceBranchId"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    dto.SourceBranchId = Utf8JsonReaderHelper.ReadGuid(ref reader);

                    continue;
                }
                if (reader.ValueTextEquals("status"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    dto.Status = ReviewStatusDeSerializer.DeSerialize(ref reader);

                    continue;
                }
                if (reader.ValueTextEquals("targetBranchId"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    dto.TargetBranchId = Utf8JsonReaderHelper.ReadGuid(ref reader);

                    continue;
                }
                if (reader.ValueTextEquals("title"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    dto.Title = Utf8JsonReaderHelper.ReadRequiredString(ref reader);

                    continue;
                }
                if (reader.ValueTextEquals("updatedBy"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    dto.UpdatedBy = Utf8JsonReaderHelper.ReadGuid(ref reader);

                    continue;
                }
                if (reader.ValueTextEquals("updatedOn"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    dto.UpdatedOn = Utf8JsonReaderHelper.ReadDateTime(ref reader);

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

            return dto;
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
