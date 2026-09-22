// ------------------------------------------------------------------------------------------------
//  <copyright file="OwnershipMessagePackFormatter.cs" company="Starion Group S.A.">
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
    /// <see cref="Ownership" /> DTO using MessagePack.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public sealed partial class OwnershipMessagePackFormatter : MessagePackFormatterBase, IMessagePackFormatter<Ownership?>
    {
        /// <summary>
        /// Serializes an exact <see cref="Ownership" /> DTO.
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
        public void Serialize(ref MessagePackWriter writer, Ownership? dto, MessagePackSerializerOptions options)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            writer.WriteArrayHeader(9);

            WriteGuidBin16(ref writer, dto.Id);
            WriteGuidBin16(ref writer, dto.CreatedBy);
            writer.Write(dto.CreatedOn);
            WriteRequiredString(ref writer, dto.Description, "Description");
            WriteGuidBin16(ref writer, dto.EngineeringMetadataId);
            WriteRequiredString(ref writer, dto.Name, "Name");
            WriteRequiredString(ref writer, dto.ShortName, "ShortName");
            WriteGuidBin16(ref writer, dto.UpdatedBy);
            writer.Write(dto.UpdatedOn);
        }

        /// <summary>
        /// Deserializes an exact <see cref="Ownership" /> DTO.
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
        public Ownership? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                throw new MessagePackSerializationException("Ownership may not be nil.");
            }

            options.Security.DepthStep(ref reader);

            try
            {
                var fieldCount = reader.ReadArrayHeader();

                if (fieldCount != 9)
                {
                    throw new MessagePackSerializationException($"Ownership contains {fieldCount} fields; exactly 9 fields are required.");
                }

                var dto = new Ownership();

                dto.Id = ReadGuidBin16(ref reader);
                dto.CreatedBy = ReadGuidBin16(ref reader);
                dto.CreatedOn = reader.ReadDateTime();
                dto.Description = ReadRequiredString(ref reader, "Description");
                dto.EngineeringMetadataId = ReadGuidBin16(ref reader);
                dto.Name = ReadRequiredString(ref reader, "Name");
                dto.ShortName = ReadRequiredString(ref reader, "ShortName");
                dto.UpdatedBy = ReadGuidBin16(ref reader);
                dto.UpdatedOn = reader.ReadDateTime();

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
