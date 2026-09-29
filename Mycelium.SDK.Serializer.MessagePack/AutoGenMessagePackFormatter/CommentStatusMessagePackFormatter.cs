// ------------------------------------------------------------------------------------------------
//  <copyright file="CommentStatusMessagePackFormatter.cs" company="Starion Group S.A.">
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
    /// <see cref="CommentStatus" />.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    internal sealed class CommentStatusMessagePackFormatter : MessagePackFormatterBase, IMessagePackFormatter<CommentStatus>
    {
        /// <summary>
        /// The shared formatter for <see cref="CommentStatus" /> values.
        /// </summary>
        internal static readonly CommentStatusMessagePackFormatter Instance = new();

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
        public void Serialize(ref MessagePackWriter writer, CommentStatus value, MessagePackSerializerOptions options)
        {
            writer.Write(value switch
            {
                CommentStatus.Open => "open",
                CommentStatus.Resolved => "resolved",
                _ => throw new MessagePackSerializationException($"Value '{value}' is not valid for CommentStatus."),
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
        public CommentStatus Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            var value = ReadRequiredString(ref reader, "CommentStatus");

            return value switch
            {
                "open" => CommentStatus.Open,
                "resolved" => CommentStatus.Resolved,
                _ => throw new MessagePackSerializationException($"Value '{value}' is not valid for CommentStatus."),
            };
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
