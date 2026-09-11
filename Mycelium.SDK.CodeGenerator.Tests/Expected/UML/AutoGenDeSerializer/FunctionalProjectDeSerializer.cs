// ------------------------------------------------------------------------------------------------
//  <copyright file="FunctionalProjectDeSerializer.cs" company="Starion Group S.A.">
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
    /// Deserializes an exact <see cref="FunctionalProject" /> DTO.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    internal static class FunctionalProjectDeSerializer
    {
        /// <summary>
        /// Deserializes an exact <see cref="FunctionalProject" /> instance.
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
        /// <see cref="FunctionalProject" />.
        /// </exception>
        internal static IThing DeSerialize(ref Utf8JsonReader reader)
        {
            Utf8JsonReaderHelper.Expect(ref reader, JsonTokenType.StartObject);

            var dto = new FunctionalProject();
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

                    if (!string.Equals(typeName, "FunctionalProject", StringComparison.Ordinal))
                    {
                        throw new NotSupportedException($"JSON discriminator '{typeName}' is not supported by FunctionalProjectDeSerializer.");
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

                if (reader.ValueTextEquals("belongsTo"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    dto.BelongsTo = Utf8JsonReaderHelper.ReadGuid(ref reader);

                    continue;
                }
                if (reader.ValueTextEquals("branchRules"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    Utf8JsonReaderHelper.Expect(ref reader, JsonTokenType.StartArray);

                    dto.BranchRules.Clear();

                    var hasEndArray = false;

                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.EndArray)
                        {
                            hasEndArray = true;
                            break;
                        }

                        dto.BranchRules.Add(Utf8JsonReaderHelper.ReadGuid(ref reader));
                    }

                    if (!hasEndArray)
                    {
                        throw new JsonException("The branchRules array is incomplete.");
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
                if (reader.ValueTextEquals("currentMode"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    dto.CurrentMode = ProjectModeDeSerializer.DeSerialize(ref reader);

                    continue;
                }
                if (reader.ValueTextEquals("defines"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    Utf8JsonReaderHelper.Expect(ref reader, JsonTokenType.StartArray);

                    dto.Defines.Clear();

                    var hasEndArray = false;

                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.EndArray)
                        {
                            hasEndArray = true;
                            break;
                        }

                        dto.Defines.Add(Utf8JsonReaderHelper.ReadGuid(ref reader));
                    }

                    if (!hasEndArray)
                    {
                        throw new JsonException("The defines array is incomplete.");
                    }

                    continue;
                }
                if (reader.ValueTextEquals("description"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    dto.Description = Utf8JsonReaderHelper.ReadRequiredString(ref reader);

                    continue;
                }
                if (reader.ValueTextEquals("engineeringProjectId"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    dto.EngineeringProjectId = Utf8JsonReaderHelper.ReadGuid(ref reader);

                    continue;
                }
                if (reader.ValueTextEquals("involves"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    Utf8JsonReaderHelper.Expect(ref reader, JsonTokenType.StartArray);

                    dto.Involves.Clear();

                    var hasEndArray = false;

                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.EndArray)
                        {
                            hasEndArray = true;
                            break;
                        }

                        dto.Involves.Add(Utf8JsonReaderHelper.ReadGuid(ref reader));
                    }

                    if (!hasEndArray)
                    {
                        throw new JsonException("The involves array is incomplete.");
                    }

                    continue;
                }
                if (reader.ValueTextEquals("lifecycle"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    dto.Lifecycle = ProjectLifecycleKindDeSerializer.DeSerialize(ref reader);

                    continue;
                }
                if (reader.ValueTextEquals("name"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    dto.Name = Utf8JsonReaderHelper.ReadRequiredString(ref reader);

                    continue;
                }
                if (reader.ValueTextEquals("policy"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    dto.Policy = Utf8JsonReaderHelper.ReadGuid(ref reader);

                    continue;
                }
                if (reader.ValueTextEquals("reviews"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    Utf8JsonReaderHelper.Expect(ref reader, JsonTokenType.StartArray);

                    dto.Reviews.Clear();

                    var hasEndArray = false;

                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.EndArray)
                        {
                            hasEndArray = true;
                            break;
                        }

                        dto.Reviews.Add(Utf8JsonReaderHelper.ReadGuid(ref reader));
                    }

                    if (!hasEndArray)
                    {
                        throw new JsonException("The reviews array is incomplete.");
                    }

                    continue;
                }
                if (reader.ValueTextEquals("sharedPreferences"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    dto.SharedPreferences = Utf8JsonReaderHelper.ReadStringDictionary(ref reader);

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
                if (reader.ValueTextEquals("visibility"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    dto.Visibility = ProjectVisibilityDeSerializer.DeSerialize(ref reader);

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
