// ------------------------------------------------------------------------------------------------
//  <copyright file="Serializer.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.Serializer.MessagePack
{
    using System;
    using System.Buffers;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using global::MessagePack;

    using Mycelium.SDK.DTO;
    using Mycelium.SDK.Serializer.MessagePack.Helpers;

    /// <summary>
    /// Serializes heterogeneous Mycelium DTO sequences as MessagePack payloads.
    /// </summary>
    public sealed class Serializer : ISerializer
    {
        /// <summary>
        /// The MessagePack options that select the generated DTO and payload formatters.
        /// </summary>
        private static readonly MessagePackSerializerOptions SerializerOptions = MessagePackSerializerOptions.Standard.WithResolver(DataFormatterResolver.Instance);

        /// <summary>
        /// Initializes a new instance of the <see cref="Serializer" /> class.
        /// </summary>
        public Serializer()
        {
        }

        /// <summary>
        /// Serializes DTOs to a caller-owned stream.
        /// </summary>
        /// <param name="dtos">
        /// The DTOs to serialize.
        /// </param>
        /// <param name="stream">
        /// The stream that receives the payload.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="dtos" />, <paramref name="stream" />, or a DTO item is null.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Thrown when a DTO has no exact generated MessagePack representation.
        /// </exception>
        /// <exception cref="MessagePackSerializationException">
        /// Thrown when a DTO contains a value invalid for its MessagePack representation.
        /// </exception>
        public void Serialize(IEnumerable<IThing> dtos, Stream stream)
        {
            if (dtos == null)
            {
                throw new ArgumentNullException(nameof(dtos));
            }

            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var payload = PayloadFactory.ToPayload(dtos);
            global::MessagePack.MessagePackSerializer.Serialize(stream, payload, SerializerOptions);
        }

        /// <summary>
        /// Serializes DTOs to a byte buffer writer.
        /// </summary>
        /// <param name="dtos">
        /// The DTOs to serialize.
        /// </param>
        /// <param name="writer">
        /// The buffer writer that receives the payload.
        /// </param>
        /// <param name="cancellationToken">
        /// The token used to cancel the operation.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="dtos" />, <paramref name="writer" />, or a DTO item is null.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Thrown when a DTO has no exact generated MessagePack representation.
        /// </exception>
        /// <exception cref="MessagePackSerializationException">
        /// Thrown when a DTO contains a value invalid for its MessagePack representation.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Thrown when <paramref name="cancellationToken" /> is canceled.
        /// </exception>
        public void SerializeToBufferWriter(IEnumerable<IThing> dtos, IBufferWriter<byte> writer, CancellationToken cancellationToken = default)
        {
            if (dtos == null)
            {
                throw new ArgumentNullException(nameof(dtos));
            }

            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var payload = PayloadFactory.ToPayload(dtos);

            cancellationToken.ThrowIfCancellationRequested();

            global::MessagePack.MessagePackSerializer.Serialize(writer, payload, SerializerOptions, cancellationToken);
        }

        /// <summary>
        /// Asynchronously serializes DTOs to a caller-owned stream.
        /// </summary>
        /// <param name="dtos">
        /// The DTOs to serialize.
        /// </param>
        /// <param name="stream">
        /// The stream that receives the payload.
        /// </param>
        /// <param name="cancellationToken">
        /// The token used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task representing the serialization operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="dtos" />, <paramref name="stream" />, or a DTO item is null.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Thrown when a DTO has no exact generated MessagePack representation.
        /// </exception>
        /// <exception cref="MessagePackSerializationException">
        /// Thrown when a DTO contains a value invalid for its MessagePack representation.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Thrown when <paramref name="cancellationToken" /> is canceled.
        /// </exception>
        public Task SerializeAsync(IEnumerable<IThing> dtos, Stream stream, CancellationToken cancellationToken)
        {
            if (dtos == null)
            {
                throw new ArgumentNullException(nameof(dtos));
            }

            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var payload = PayloadFactory.ToPayload(dtos);

            cancellationToken.ThrowIfCancellationRequested();

            return global::MessagePack.MessagePackSerializer.SerializeAsync(stream, payload, SerializerOptions, cancellationToken);
        }
    }
}
