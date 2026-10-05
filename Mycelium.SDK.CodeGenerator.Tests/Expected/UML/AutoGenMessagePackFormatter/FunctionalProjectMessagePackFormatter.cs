// ------------------------------------------------------------------------------------------------
//  <copyright file="FunctionalProjectMessagePackFormatter.cs" company="Starion Group S.A.">
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
    /// <see cref="FunctionalProject" /> DTO using MessagePack.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    public sealed partial class FunctionalProjectMessagePackFormatter : MessagePackFormatterBase, IMessagePackFormatter<FunctionalProject>
    {
        /// <summary>
        /// Serializes an exact <see cref="FunctionalProject" /> DTO.
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
        public void Serialize(ref MessagePackWriter writer, FunctionalProject dto, MessagePackSerializerOptions options)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            writer.WriteArrayHeader(18);

            GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.Id, options);
            GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.BelongsTo, options);
            if (dto.BranchRules == null)
            {
                writer.WriteNil();
            }
            else
            {
                writer.WriteArrayHeader(dto.BranchRules.Count);

                for (var i = 0; i < dto.BranchRules.Count; i++)
                {
                    GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.BranchRules[i], options);
                }
            }
            GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.CreatedBy, options);
            WriteRoundTripDateTime(ref writer, dto.CreatedOn);
            ProjectModeMessagePackFormatter.Instance.Serialize(ref writer, dto.CurrentMode, options);
            if (dto.Defines == null)
            {
                writer.WriteNil();
            }
            else
            {
                writer.WriteArrayHeader(dto.Defines.Count);

                for (var i = 0; i < dto.Defines.Count; i++)
                {
                    GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.Defines[i], options);
                }
            }
            WriteRequiredString(ref writer, dto.Description, "Description");
            GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.EngineeringProjectId, options);
            if (dto.Involves == null)
            {
                writer.WriteNil();
            }
            else
            {
                writer.WriteArrayHeader(dto.Involves.Count);

                for (var i = 0; i < dto.Involves.Count; i++)
                {
                    GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.Involves[i], options);
                }
            }
            ProjectLifecycleKindMessagePackFormatter.Instance.Serialize(ref writer, dto.Lifecycle, options);
            WriteRequiredString(ref writer, dto.Name, "Name");
            GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.Policy, options);
            if (dto.Reviews == null)
            {
                writer.WriteNil();
            }
            else
            {
                writer.WriteArrayHeader(dto.Reviews.Count);

                for (var i = 0; i < dto.Reviews.Count; i++)
                {
                    GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.Reviews[i], options);
                }
            }
            if (dto.SharedPreferences == null)
            {
                writer.WriteNil();
            }
            else
            {
                StringDictionaryMessagePackFormatter.Instance.Serialize(ref writer, dto.SharedPreferences, options);
            }
            GuidMessagePackFormatter.Instance.Serialize(ref writer, dto.UpdatedBy, options);
            WriteRoundTripDateTime(ref writer, dto.UpdatedOn);
            ProjectVisibilityMessagePackFormatter.Instance.Serialize(ref writer, dto.Visibility, options);
        }

        /// <summary>
        /// Deserializes an exact <see cref="FunctionalProject" /> DTO.
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
        public FunctionalProject Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                throw new MessagePackSerializationException("FunctionalProject may not be nil.");
            }

            options.Security.DepthStep(ref reader);

            try
            {
                var fieldCount = reader.ReadArrayHeader();

                if (fieldCount != 18)
                {
                    throw new MessagePackSerializationException($"FunctionalProject contains {fieldCount} fields; exactly 18 fields are required.");
                }

                var dto = new FunctionalProject();

                dto.Id = GuidMessagePackFormatter.Instance.Deserialize(ref reader, options);
                dto.BelongsTo = GuidMessagePackFormatter.Instance.Deserialize(ref reader, options);
                if (reader.TryReadNil())
                {
                    dto.BranchRules = null;
                }
                else
                {
                    var messagePackBranchRulesCount = reader.ReadArrayHeader();
                    dto.BranchRules.Clear();

                    if (dto.BranchRules.Capacity < messagePackBranchRulesCount)
                    {
                        dto.BranchRules.Capacity = messagePackBranchRulesCount;
                    }

                    for (var i = 0; i < messagePackBranchRulesCount; i++)
                    {
                        dto.BranchRules.Add(GuidMessagePackFormatter.Instance.Deserialize(ref reader, options));
                    }
                }
                dto.CreatedBy = GuidMessagePackFormatter.Instance.Deserialize(ref reader, options);
                dto.CreatedOn = ReadRoundTripDateTime(ref reader, "CreatedOn");
                dto.CurrentMode = ProjectModeMessagePackFormatter.Instance.Deserialize(ref reader, options);
                if (reader.TryReadNil())
                {
                    dto.Defines = null;
                }
                else
                {
                    var messagePackDefinesCount = reader.ReadArrayHeader();
                    dto.Defines.Clear();

                    if (dto.Defines.Capacity < messagePackDefinesCount)
                    {
                        dto.Defines.Capacity = messagePackDefinesCount;
                    }

                    for (var i = 0; i < messagePackDefinesCount; i++)
                    {
                        dto.Defines.Add(GuidMessagePackFormatter.Instance.Deserialize(ref reader, options));
                    }
                }
                dto.Description = ReadRequiredString(ref reader, "Description");
                dto.EngineeringProjectId = GuidMessagePackFormatter.Instance.Deserialize(ref reader, options);
                if (reader.TryReadNil())
                {
                    dto.Involves = null;
                }
                else
                {
                    var messagePackInvolvesCount = reader.ReadArrayHeader();
                    dto.Involves.Clear();

                    if (dto.Involves.Capacity < messagePackInvolvesCount)
                    {
                        dto.Involves.Capacity = messagePackInvolvesCount;
                    }

                    for (var i = 0; i < messagePackInvolvesCount; i++)
                    {
                        dto.Involves.Add(GuidMessagePackFormatter.Instance.Deserialize(ref reader, options));
                    }
                }
                dto.Lifecycle = ProjectLifecycleKindMessagePackFormatter.Instance.Deserialize(ref reader, options);
                dto.Name = ReadRequiredString(ref reader, "Name");
                dto.Policy = GuidMessagePackFormatter.Instance.Deserialize(ref reader, options);
                if (reader.TryReadNil())
                {
                    dto.Reviews = null;
                }
                else
                {
                    var messagePackReviewsCount = reader.ReadArrayHeader();
                    dto.Reviews.Clear();

                    if (dto.Reviews.Capacity < messagePackReviewsCount)
                    {
                        dto.Reviews.Capacity = messagePackReviewsCount;
                    }

                    for (var i = 0; i < messagePackReviewsCount; i++)
                    {
                        dto.Reviews.Add(GuidMessagePackFormatter.Instance.Deserialize(ref reader, options));
                    }
                }
                if (reader.TryReadNil())
                {
                    dto.SharedPreferences = null;
                }
                else
                {
                    dto.SharedPreferences = StringDictionaryMessagePackFormatter.Instance.Deserialize(ref reader, options);
                }
                dto.UpdatedBy = GuidMessagePackFormatter.Instance.Deserialize(ref reader, options);
                dto.UpdatedOn = ReadRoundTripDateTime(ref reader, "UpdatedOn");
                dto.Visibility = ProjectVisibilityMessagePackFormatter.Instance.Deserialize(ref reader, options);

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
