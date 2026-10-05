// ------------------------------------------------------------------------------------------------
//  <copyright file="OrganizationPolicyMessagePackFormatter.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.Serializer.MessagePack
{
    using System;
    using System.CodeDom.Compiler;

    using global::MessagePack;
    using global::MessagePack.Formatters;

    using Mycelium.SDK.DTO;

    /// <summary>
    /// Serializes and deserializes an exact
    /// <see cref="OrganizationPolicy" /> DTO using MessagePack.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public sealed partial class OrganizationPolicyMessagePackFormatter : MessagePackFormatterBase, IMessagePackFormatter<OrganizationPolicy>
    {
        /// <summary>
        /// Serializes an exact <see cref="OrganizationPolicy" /> DTO.
        /// </summary>
        /// <param name="writer">
        /// The MessagePack writer that receives the serialized DTO.
        /// </param>
        /// <param name="dto">
        /// The DTO to serialize.
        /// </param>
        /// <param name="options">
        /// The MessagePack serializer options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="dto" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="MessagePackSerializationException">
        /// Thrown when a DTO property contains a value that is invalid for its approved
        /// MessagePack representation.
        /// </exception>
        public void Serialize(ref MessagePackWriter writer, OrganizationPolicy dto, MessagePackSerializerOptions options)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            writer.WriteArrayHeader(8);

            GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.Id, options);
            writer.Write(dto.AllowProjectCreation);
            GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.CreatedBy, options);
            WriteRoundTripDateTime(ref writer, dto.CreatedOn);
            ProjectLifecycleKindMessagePackFormatter.Instance.Serialize(ref writer, dto.DefaultProjectLifecycleOnCreate, options);
            writer.Write(dto.GrantReadOnlyViewForAudit);
            GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.UpdatedBy, options);
            WriteRoundTripDateTime(ref writer, dto.UpdatedOn);
        }

        /// <summary>
        /// Deserializes an exact <see cref="OrganizationPolicy" /> DTO.
        /// </summary>
        /// <param name="reader">
        /// The MessagePack reader from which the DTO is deserialized.
        /// </param>
        /// <param name="options">
        /// The MessagePack serializer options.
        /// </param>
        /// <returns>
        /// The deserialized DTO.
        /// </returns>
        /// <exception cref="MessagePackSerializationException">
        /// Thrown when the encoded DTO is <c>nil</c>, has an incorrect field count or contains
        /// a value that is invalid for its approved MessagePack representation.
        /// </exception>
        public OrganizationPolicy Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                throw new MessagePackSerializationException("OrganizationPolicy may not be nil.");
            }

            options.Security.DepthStep(ref reader);

            try
            {
                var fieldCount = reader.ReadArrayHeader();

                if (fieldCount != 8)
                {
                    throw new MessagePackSerializationException($"OrganizationPolicy contains {fieldCount} fields; exactly 8 fields are required.");
                }

                var dto = new OrganizationPolicy();

                dto.Id = GuidMessagePackFormatter.Instance.Deserialize(ref reader, options);
                dto.AllowProjectCreation = reader.ReadBoolean();
                dto.CreatedBy = GuidMessagePackFormatter.Instance.Deserialize(ref reader, options);
                dto.CreatedOn = ReadRoundTripDateTime(ref reader, "CreatedOn");
                dto.DefaultProjectLifecycleOnCreate = ProjectLifecycleKindMessagePackFormatter.Instance.Deserialize(ref reader, options);
                dto.GrantReadOnlyViewForAudit = reader.ReadBoolean();
                dto.UpdatedBy = GuidMessagePackFormatter.Instance.Deserialize(ref reader, options);
                dto.UpdatedOn = ReadRoundTripDateTime(ref reader, "UpdatedOn");

                return dto;
            }
            finally
            {
                reader.Depth--;
            }
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
