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
    public sealed partial class FunctionalProjectMessagePackFormatter : MessagePackFormatterBase, IMessagePackFormatter<FunctionalProject?>
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
        public void Serialize(ref MessagePackWriter writer, FunctionalProject? dto, MessagePackSerializerOptions options)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            writer.WriteArrayHeader(18);

            WriteGuidBin16(ref writer, dto.Id);
            WriteGuidBin16(ref writer, dto.BelongsTo);
            if (dto.BranchRules == null)
            {
                throw new MessagePackSerializationException("Collection property 'BranchRules' may not be null.");
            }

            if (dto.BranchRules.Count > 1)
            {
                dto.BranchRules.Sort(static (left, right) => left.CompareTo(right));
            }

            writer.WriteArrayHeader(dto.BranchRules.Count);

            for (var i = 0; i < dto.BranchRules.Count; i++)
            {
                WriteGuidBin16(ref writer, dto.BranchRules[i]);
            }
            WriteGuidBin16(ref writer, dto.CreatedBy);
            writer.Write(dto.CreatedOn);
            writer.Write(dto.CurrentMode switch
            {
                ProjectMode.Regular => "regular",
                ProjectMode.Concurrent => "concurrent",
                _ => throw new MessagePackSerializationException($"Value '{dto.CurrentMode}' is not valid for ProjectMode."),
            });
            if (dto.Defines == null)
            {
                throw new MessagePackSerializationException("Collection property 'Defines' may not be null.");
            }

            if (dto.Defines.Count > 1)
            {
                dto.Defines.Sort(static (left, right) => left.CompareTo(right));
            }

            writer.WriteArrayHeader(dto.Defines.Count);

            for (var i = 0; i < dto.Defines.Count; i++)
            {
                WriteGuidBin16(ref writer, dto.Defines[i]);
            }
            WriteRequiredString(ref writer, dto.Description, "Description");
            WriteGuidBin16(ref writer, dto.EngineeringProjectId);
            if (dto.Involves == null)
            {
                throw new MessagePackSerializationException("Collection property 'Involves' may not be null.");
            }

            if (dto.Involves.Count > 1)
            {
                dto.Involves.Sort(static (left, right) => left.CompareTo(right));
            }

            writer.WriteArrayHeader(dto.Involves.Count);

            for (var i = 0; i < dto.Involves.Count; i++)
            {
                WriteGuidBin16(ref writer, dto.Involves[i]);
            }
            writer.Write(dto.Lifecycle switch
            {
                ProjectLifecycleKind.Preparation => "preparation",
                ProjectLifecycleKind.Open => "open",
                ProjectLifecycleKind.Review => "review",
                ProjectLifecycleKind.Archived => "archived",
                _ => throw new MessagePackSerializationException($"Value '{dto.Lifecycle}' is not valid for ProjectLifecycleKind."),
            });
            WriteRequiredString(ref writer, dto.Name, "Name");
            WriteGuidBin16(ref writer, dto.Policy);
            if (dto.Reviews == null)
            {
                throw new MessagePackSerializationException("Collection property 'Reviews' may not be null.");
            }

            if (dto.Reviews.Count > 1)
            {
                dto.Reviews.Sort(static (left, right) => left.CompareTo(right));
            }

            writer.WriteArrayHeader(dto.Reviews.Count);

            for (var i = 0; i < dto.Reviews.Count; i++)
            {
                WriteGuidBin16(ref writer, dto.Reviews[i]);
            }
            WriteStringDictionary(ref writer, dto.SharedPreferences, "SharedPreferences");
            WriteGuidBin16(ref writer, dto.UpdatedBy);
            writer.Write(dto.UpdatedOn);
            writer.Write(dto.Visibility switch
            {
                ProjectVisibility.Private => "private",
                ProjectVisibility.Organization => "organization",
                ProjectVisibility.Public => "public",
                _ => throw new MessagePackSerializationException($"Value '{dto.Visibility}' is not valid for ProjectVisibility."),
            });
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
        public FunctionalProject? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
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

                dto.Id = ReadGuidBin16(ref reader);
                dto.BelongsTo = ReadGuidBin16(ref reader);
                if (reader.TryReadNil())
                {
                    throw new MessagePackSerializationException("Collection property 'BranchRules' may not be nil.");
                }

                var messagePackBranchRulesCount = reader.ReadArrayHeader();
                dto.BranchRules.Clear();

                if (dto.BranchRules.Capacity < messagePackBranchRulesCount)
                {
                    dto.BranchRules.Capacity = messagePackBranchRulesCount;
                }

                for (var i = 0; i < messagePackBranchRulesCount; i++)
                {
                    dto.BranchRules.Add(ReadGuidBin16(ref reader));
                }
                dto.CreatedBy = ReadGuidBin16(ref reader);
                dto.CreatedOn = reader.ReadDateTime();
                var messagePackCurrentModeValue = ReadRequiredString(ref reader, "CurrentMode");
                dto.CurrentMode = messagePackCurrentModeValue switch
                {
                    "regular" => ProjectMode.Regular,
                    "concurrent" => ProjectMode.Concurrent,
                    _ => throw new MessagePackSerializationException($"Value '{messagePackCurrentModeValue}' is not valid for ProjectMode."),
                };
                if (reader.TryReadNil())
                {
                    throw new MessagePackSerializationException("Collection property 'Defines' may not be nil.");
                }

                var messagePackDefinesCount = reader.ReadArrayHeader();
                dto.Defines.Clear();

                if (dto.Defines.Capacity < messagePackDefinesCount)
                {
                    dto.Defines.Capacity = messagePackDefinesCount;
                }

                for (var i = 0; i < messagePackDefinesCount; i++)
                {
                    dto.Defines.Add(ReadGuidBin16(ref reader));
                }
                dto.Description = ReadRequiredString(ref reader, "Description");
                dto.EngineeringProjectId = ReadGuidBin16(ref reader);
                if (reader.TryReadNil())
                {
                    throw new MessagePackSerializationException("Collection property 'Involves' may not be nil.");
                }

                var messagePackInvolvesCount = reader.ReadArrayHeader();
                dto.Involves.Clear();

                if (dto.Involves.Capacity < messagePackInvolvesCount)
                {
                    dto.Involves.Capacity = messagePackInvolvesCount;
                }

                for (var i = 0; i < messagePackInvolvesCount; i++)
                {
                    dto.Involves.Add(ReadGuidBin16(ref reader));
                }
                var messagePackLifecycleValue = ReadRequiredString(ref reader, "Lifecycle");
                dto.Lifecycle = messagePackLifecycleValue switch
                {
                    "preparation" => ProjectLifecycleKind.Preparation,
                    "open" => ProjectLifecycleKind.Open,
                    "review" => ProjectLifecycleKind.Review,
                    "archived" => ProjectLifecycleKind.Archived,
                    _ => throw new MessagePackSerializationException($"Value '{messagePackLifecycleValue}' is not valid for ProjectLifecycleKind."),
                };
                dto.Name = ReadRequiredString(ref reader, "Name");
                dto.Policy = ReadGuidBin16(ref reader);
                if (reader.TryReadNil())
                {
                    throw new MessagePackSerializationException("Collection property 'Reviews' may not be nil.");
                }

                var messagePackReviewsCount = reader.ReadArrayHeader();
                dto.Reviews.Clear();

                if (dto.Reviews.Capacity < messagePackReviewsCount)
                {
                    dto.Reviews.Capacity = messagePackReviewsCount;
                }

                for (var i = 0; i < messagePackReviewsCount; i++)
                {
                    dto.Reviews.Add(ReadGuidBin16(ref reader));
                }
                dto.SharedPreferences = ReadStringDictionary(ref reader, "SharedPreferences");
                dto.UpdatedBy = ReadGuidBin16(ref reader);
                dto.UpdatedOn = reader.ReadDateTime();
                var messagePackVisibilityValue = ReadRequiredString(ref reader, "Visibility");
                dto.Visibility = messagePackVisibilityValue switch
                {
                    "private" => ProjectVisibility.Private,
                    "organization" => ProjectVisibility.Organization,
                    "public" => ProjectVisibility.Public,
                    _ => throw new MessagePackSerializationException($"Value '{messagePackVisibilityValue}' is not valid for ProjectVisibility."),
                };

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
