// ------------------------------------------------------------------------------------------------
//  <copyright file="OrganizationMemberDeSerializer.cs" company="Starion Group S.A.">
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
    /// Deserializes an exact <see cref="OrganizationMember" /> DTO.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    internal static class OrganizationMemberDeSerializer
    {
        /// <summary>
        /// Deserializes an exact <see cref="OrganizationMember" /> instance.
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
        /// <see cref="OrganizationMember" />.
        /// </exception>
        internal static IThing DeSerialize(ref Utf8JsonReader reader, ILoggerFactory loggerFactory = null)
        {
            Utf8JsonReaderHelper.Expect(ref reader, JsonTokenType.StartObject);

            var logger = loggerFactory == null
                ? NullLogger.Instance
                : loggerFactory.CreateLogger("OrganizationMemberDeSerializer");

            var dto = new OrganizationMember();
            var hasType = false;
            var hasId = false;
            var hasEndObject = false;

            var hasCreatedBy = false;
            var hasCreatedOn = false;
            var hasOrganization = false;
            var hasRole = false;
            var hasUpdatedBy = false;
            var hasUpdatedOn = false;
            var hasUser = false;

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

                    if (!string.Equals(typeName, "OrganizationMember", StringComparison.Ordinal))
                    {
                        throw new NotSupportedException($"JSON discriminator '{typeName}' is not supported by OrganizationMemberDeSerializer.");
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
                if (reader.ValueTextEquals("organization"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    dto.Organization = Utf8JsonReaderHelper.ReadGuid(ref reader);

                    hasOrganization = true;
                    continue;
                }
                if (reader.ValueTextEquals("role"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    dto.Role = OrganizationMembershipRoleDeSerializer.DeSerialize(ref reader);

                    hasRole = true;
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
                if (reader.ValueTextEquals("user"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    dto.User = Utf8JsonReaderHelper.ReadGuid(ref reader);

                    hasUser = true;
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
                    "OrganizationMember",
                    dto.Id);
            }
            if (!hasCreatedOn)
            {
                logger.LogDebug(
                    "The {PropertyName} JSON property was not found in {DtoType}: {Id}. The construction default is retained.",
                    "createdOn",
                    "OrganizationMember",
                    dto.Id);
            }
            if (!hasOrganization)
            {
                logger.LogDebug(
                    "The {PropertyName} JSON property was not found in {DtoType}: {Id}. The construction default is retained.",
                    "organization",
                    "OrganizationMember",
                    dto.Id);
            }
            if (!hasRole)
            {
                logger.LogDebug(
                    "The {PropertyName} JSON property was not found in {DtoType}: {Id}. The construction default is retained.",
                    "role",
                    "OrganizationMember",
                    dto.Id);
            }
            if (!hasUpdatedBy)
            {
                logger.LogDebug(
                    "The {PropertyName} JSON property was not found in {DtoType}: {Id}. The construction default is retained.",
                    "updatedBy",
                    "OrganizationMember",
                    dto.Id);
            }
            if (!hasUpdatedOn)
            {
                logger.LogDebug(
                    "The {PropertyName} JSON property was not found in {DtoType}: {Id}. The construction default is retained.",
                    "updatedOn",
                    "OrganizationMember",
                    dto.Id);
            }
            if (!hasUser)
            {
                logger.LogDebug(
                    "The {PropertyName} JSON property was not found in {DtoType}: {Id}. The construction default is retained.",
                    "user",
                    "OrganizationMember",
                    dto.Id);
            }

            return dto;
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
