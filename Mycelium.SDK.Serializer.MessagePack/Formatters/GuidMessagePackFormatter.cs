// ------------------------------------------------------------------------------------------------
//  <copyright file="GuidMessagePackFormatter.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.Serializer.MessagePack
{
    using System.Buffers;

    using global::MessagePack;
    using global::MessagePack.Formatters;

    /// <summary>
    /// Encodes identifiers as binary values in the default .NET Guid byte layout.
    /// </summary>
    internal sealed class GuidMessagePackFormatter : IMessagePackFormatter<Guid>
    {
        /// <summary>
        /// The shared formatter used directly by generated DTO formatters.
        /// </summary>
        internal static readonly GuidMessagePackFormatter Instance = new();

        /// <summary>
        /// The reusable thread-local buffer for one identifier.
        /// </summary>
        [ThreadStatic]
        private static byte[] guidBuffer;

        internal GuidMessagePackFormatter()
        {
        }

        /// <inheritdoc />
        public void Serialize(ref MessagePackWriter writer, Guid value, MessagePackSerializerOptions options)
        {
            var buffer = guidBuffer ??= new byte[16];

            if (!value.TryWriteBytes(buffer))
            {
                throw new MessagePackSerializationException("The Guid could not be written as a 16-byte binary value.");
            }

            writer.WriteBinHeader(16);
            writer.WriteRaw(buffer);
        }

        /// <inheritdoc />
        public Guid Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            var bytes = reader.ReadBytes();

            if (!bytes.HasValue)
            {
                throw new MessagePackSerializationException("Expected Guid as bin(16), but found nil.");
            }

            var sequence = bytes.Value;

            if (sequence.Length != 16)
            {
                throw new MessagePackSerializationException($"Expected Guid as 16 bytes, but found {sequence.Length} bytes.");
            }

            if (sequence.IsSingleSegment)
            {
                return new Guid(sequence.FirstSpan);
            }

            Span<byte> buffer = stackalloc byte[16];
            sequence.CopyTo(buffer);

            return new Guid(buffer);
        }
    }
}
