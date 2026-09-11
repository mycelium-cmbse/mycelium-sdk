// ------------------------------------------------------------------------------------------------
//  <copyright file="UserDeSerializer.cs" company="Starion Group S.A.">
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
    /// Deserializes an exact <see cref="User" /> DTO.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    internal static class UserDeSerializer
    {
        /// <summary>
        /// Deserializes an exact <see cref="User" /> instance.
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
        /// <see cref="User" />.
        /// </exception>
        internal static IThing DeSerialize(ref Utf8JsonReader reader)
        {
            Utf8JsonReaderHelper.Expect(ref reader, JsonTokenType.StartObject);

            var dto = new User();
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

                    if (!string.Equals(typeName, "User", StringComparison.Ordinal))
                    {
                        throw new NotSupportedException($"JSON discriminator '{typeName}' is not supported by UserDeSerializer.");
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

                    continue;
                }
                if (reader.ValueTextEquals("createdOn"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    dto.CreatedOn = Utf8JsonReaderHelper.ReadDateTime(ref reader);

                    continue;
                }
                if (reader.ValueTextEquals("externalIdentifier"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    dto.ExternalIdentifier = Utf8JsonReaderHelper.ReadRequiredString(ref reader);

                    continue;
                }
                if (reader.ValueTextEquals("isPartOfOrganizations"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    Utf8JsonReaderHelper.Expect(ref reader, JsonTokenType.StartArray);

                    dto.IsPartOfOrganizations.Clear();

                    var hasEndArray = false;

                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.EndArray)
                        {
                            hasEndArray = true;
                            break;
                        }

                        dto.IsPartOfOrganizations.Add(Utf8JsonReaderHelper.ReadGuid(ref reader));
                    }

                    if (!hasEndArray)
                    {
                        throw new JsonException("The isPartOfOrganizations array is incomplete.");
                    }

                    continue;
                }
                if (reader.ValueTextEquals("isPartOfProjects"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    Utf8JsonReaderHelper.Expect(ref reader, JsonTokenType.StartArray);

                    dto.IsPartOfProjects.Clear();

                    var hasEndArray = false;

                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.EndArray)
                        {
                            hasEndArray = true;
                            break;
                        }

                        dto.IsPartOfProjects.Add(Utf8JsonReaderHelper.ReadGuid(ref reader));
                    }

                    if (!hasEndArray)
                    {
                        throw new JsonException("The isPartOfProjects array is incomplete.");
                    }

                    continue;
                }
                if (reader.ValueTextEquals("mail"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    dto.Mail = Utf8JsonReaderHelper.ReadRequiredString(ref reader);

                    continue;
                }
                if (reader.ValueTextEquals("name"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    dto.Name = Utf8JsonReaderHelper.ReadRequiredString(ref reader);

                    continue;
                }
                if (reader.ValueTextEquals("status"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    dto.Status = ActivationStatusDeSerializer.DeSerialize(ref reader);

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
                if (reader.ValueTextEquals("userPreferences"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    dto.UserPreferences = Utf8JsonReaderHelper.ReadStringDictionary(ref reader);

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
