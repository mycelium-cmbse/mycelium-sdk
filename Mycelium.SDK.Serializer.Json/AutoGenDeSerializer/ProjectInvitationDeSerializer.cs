// ------------------------------------------------------------------------------------------------
//  <copyright file="ProjectInvitationDeSerializer.cs" company="Starion Group S.A.">
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
    /// Deserializes an exact <see cref="ProjectInvitation" /> DTO.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    internal static class ProjectInvitationDeSerializer
    {
        /// <summary>
        /// Deserializes an exact <see cref="ProjectInvitation" /> instance.
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
        /// <see cref="ProjectInvitation" />.
        /// </exception>
        internal static IThing DeSerialize(ref Utf8JsonReader reader, ILoggerFactory loggerFactory = null)
        {
            Utf8JsonReaderHelper.Expect(ref reader, JsonTokenType.StartObject);

            var logger = loggerFactory == null
                ? NullLogger.Instance
                : loggerFactory.CreateLogger("ProjectInvitationDeSerializer");

            var dto = new ProjectInvitation();
            var hasType = false;
            var hasId = false;
            var hasEndObject = false;

            var hasCreatedBy = false;
            var hasCreatedOn = false;
            var hasExpiresOn = false;
            var hasInvitee = false;
            var hasInviteeEmail = false;
            var hasOwnerships = false;
            var hasProject = false;
            var hasRespondedOn = false;
            var hasRole = false;
            var hasStatus = false;
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

                    if (!string.Equals(typeName, "ProjectInvitation", StringComparison.Ordinal))
                    {
                        throw new NotSupportedException($"JSON discriminator '{typeName}' is not supported by ProjectInvitationDeSerializer.");
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
                if (reader.ValueTextEquals("expiresOn"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    dto.ExpiresOn = Utf8JsonReaderHelper.ReadDateTime(ref reader);

                    hasExpiresOn = true;
                    continue;
                }
                if (reader.ValueTextEquals("invitee"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    dto.Invitee = Utf8JsonReaderHelper.ReadGuidOrNull(ref reader);

                    hasInvitee = true;
                    continue;
                }
                if (reader.ValueTextEquals("inviteeEmail"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    dto.InviteeEmail = Utf8JsonReaderHelper.ReadStringOrNull(ref reader);

                    hasInviteeEmail = true;
                    continue;
                }
                if (reader.ValueTextEquals("ownerships"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    Utf8JsonReaderHelper.Expect(ref reader, JsonTokenType.StartArray);

                    dto.Ownerships.Clear();

                    var hasEndArray = false;

                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.EndArray)
                        {
                            hasEndArray = true;
                            break;
                        }

                        dto.Ownerships.Add(Utf8JsonReaderHelper.ReadGuid(ref reader));
                    }

                    if (!hasEndArray)
                    {
                        throw new JsonException("The ownerships array is incomplete.");
                    }

                    hasOwnerships = true;
                    continue;
                }
                if (reader.ValueTextEquals("project"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    dto.Project = Utf8JsonReaderHelper.ReadGuid(ref reader);

                    hasProject = true;
                    continue;
                }
                if (reader.ValueTextEquals("respondedOn"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    dto.RespondedOn = Utf8JsonReaderHelper.ReadDateTimeOrNull(ref reader);

                    hasRespondedOn = true;
                    continue;
                }
                if (reader.ValueTextEquals("role"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    dto.Role = ProjectMemberRoleDeSerializer.DeSerialize(ref reader);

                    hasRole = true;
                    continue;
                }
                if (reader.ValueTextEquals("status"u8))
                {
                    Utf8JsonReaderHelper.ReadNext(ref reader);

                    dto.Status = InvitationStatusDeSerializer.DeSerialize(ref reader);

                    hasStatus = true;
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
                    "ProjectInvitation",
                    dto.Id);
            }
            if (!hasCreatedOn)
            {
                logger.LogDebug(
                    "The {PropertyName} JSON property was not found in {DtoType}: {Id}. The construction default is retained.",
                    "createdOn",
                    "ProjectInvitation",
                    dto.Id);
            }
            if (!hasExpiresOn)
            {
                logger.LogDebug(
                    "The {PropertyName} JSON property was not found in {DtoType}: {Id}. The construction default is retained.",
                    "expiresOn",
                    "ProjectInvitation",
                    dto.Id);
            }
            if (!hasInvitee)
            {
                logger.LogDebug(
                    "The {PropertyName} JSON property was not found in {DtoType}: {Id}. The construction default is retained.",
                    "invitee",
                    "ProjectInvitation",
                    dto.Id);
            }
            if (!hasInviteeEmail)
            {
                logger.LogDebug(
                    "The {PropertyName} JSON property was not found in {DtoType}: {Id}. The construction default is retained.",
                    "inviteeEmail",
                    "ProjectInvitation",
                    dto.Id);
            }
            if (!hasOwnerships)
            {
                logger.LogDebug(
                    "The {PropertyName} JSON property was not found in {DtoType}: {Id}. The construction default is retained.",
                    "ownerships",
                    "ProjectInvitation",
                    dto.Id);
            }
            if (!hasProject)
            {
                logger.LogDebug(
                    "The {PropertyName} JSON property was not found in {DtoType}: {Id}. The construction default is retained.",
                    "project",
                    "ProjectInvitation",
                    dto.Id);
            }
            if (!hasRespondedOn)
            {
                logger.LogDebug(
                    "The {PropertyName} JSON property was not found in {DtoType}: {Id}. The construction default is retained.",
                    "respondedOn",
                    "ProjectInvitation",
                    dto.Id);
            }
            if (!hasRole)
            {
                logger.LogDebug(
                    "The {PropertyName} JSON property was not found in {DtoType}: {Id}. The construction default is retained.",
                    "role",
                    "ProjectInvitation",
                    dto.Id);
            }
            if (!hasStatus)
            {
                logger.LogDebug(
                    "The {PropertyName} JSON property was not found in {DtoType}: {Id}. The construction default is retained.",
                    "status",
                    "ProjectInvitation",
                    dto.Id);
            }
            if (!hasUpdatedBy)
            {
                logger.LogDebug(
                    "The {PropertyName} JSON property was not found in {DtoType}: {Id}. The construction default is retained.",
                    "updatedBy",
                    "ProjectInvitation",
                    dto.Id);
            }
            if (!hasUpdatedOn)
            {
                logger.LogDebug(
                    "The {PropertyName} JSON property was not found in {DtoType}: {Id}. The construction default is retained.",
                    "updatedOn",
                    "ProjectInvitation",
                    dto.Id);
            }

            return dto;
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
