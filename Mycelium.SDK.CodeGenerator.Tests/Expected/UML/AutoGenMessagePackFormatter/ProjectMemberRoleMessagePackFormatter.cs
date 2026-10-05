// ------------------------------------------------------------------------------------------------
//  <copyright file="ProjectMemberRoleMessagePackFormatter.cs" company="Starion Group S.A.">
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

    using Mycelium.SDK;

    /// <summary>
    /// Serializes and deserializes exact lowercase MessagePack values of
    /// <see cref="ProjectMemberRole" />.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    internal sealed class ProjectMemberRoleMessagePackFormatter : MessagePackFormatterBase, IMessagePackFormatter<ProjectMemberRole>
    {
        /// <summary>
        /// The shared formatter for <see cref="ProjectMemberRole" /> values.
        /// </summary>
        internal static readonly ProjectMemberRoleMessagePackFormatter Instance = new();

        /// <summary>
        /// Writes the invariant-lowercase XMI literal of an enumeration value.
        /// </summary>
        /// <param name="writer">
        /// The MessagePack writer that receives the value.
        /// </param>
        /// <param name="value">
        /// The enumeration value to write.
        /// </param>
        /// <param name="options">
        /// The MessagePack serializer options.
        /// </param>
        /// <exception cref="MessagePackSerializationException">
        /// Thrown when <paramref name="value" /> is not a defined enumeration value.
        /// </exception>
        public void Serialize(ref MessagePackWriter writer, ProjectMemberRole value, MessagePackSerializerOptions options)
        {
            writer.Write(value switch
            {
                ProjectMemberRole.Administrator => "administrator",
                ProjectMemberRole.Participant => "participant",
                ProjectMemberRole.Viewer => "viewer",
                _ => throw new MessagePackSerializationException($"Value '{value}' is not valid for ProjectMemberRole."),
            });
        }

        /// <summary>
        /// Reads an exact invariant-lowercase XMI literal as an enumeration value.
        /// </summary>
        /// <param name="reader">
        /// The MessagePack reader positioned on the value.
        /// </param>
        /// <param name="options">
        /// The MessagePack serializer options.
        /// </param>
        /// <returns>
        /// The corresponding enumeration value.
        /// </returns>
        /// <exception cref="MessagePackSerializationException">
        /// Thrown when the encoded value is not an exact lowercase string of a defined literal.
        /// </exception>
        public ProjectMemberRole Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            var value = ReadRequiredString(ref reader, "ProjectMemberRole");

            return value switch
            {
                "administrator" => ProjectMemberRole.Administrator,
                "participant" => ProjectMemberRole.Participant,
                "viewer" => ProjectMemberRole.Viewer,
                _ => throw new MessagePackSerializationException($"Value '{value}' is not valid for ProjectMemberRole."),
            };
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
