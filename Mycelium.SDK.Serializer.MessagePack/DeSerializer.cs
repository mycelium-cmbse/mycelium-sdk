// ------------------------------------------------------------------------------------------------
//  <copyright file="DeSerializer.cs" company="Starion Group S.A.">
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
    /// Deserializes complete MessagePack payloads into heterogeneous Mycelium DTO sequences.
    /// </summary>
    public sealed class DeSerializer : IDeSerializer
    {
        /// <summary>
        /// The MessagePack options that select the generated DTO and payload formatters.
        /// </summary>
        private static readonly MessagePackSerializerOptions SerializerOptions = MessagePackSerializerOptions.Standard.WithResolver(DataFormatterResolver.Instance);

        /// <summary>
        /// Initializes a new instance of the <see cref="DeSerializer" /> class.
        /// </summary>
        public DeSerializer()
        {
        }

        /// <summary>
        /// Deserializes one complete payload from a caller-owned stream.
        /// </summary>
        /// <param name="stream">
        /// The stream containing the payload.
        /// </param>
        /// <returns>
        /// A materialized sequence of DTOs in payload group order.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="stream" /> is null.
        /// </exception>
        /// <exception cref="MessagePackSerializationException">
        /// Thrown when the payload is malformed, contains invalid values or field counts, or has trailing data.
        /// </exception>
        public IEnumerable<IThing> DeSerialize(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            using var buffer = new MemoryStream();
            stream.CopyTo(buffer);

            return this.DeSerialize(new ReadOnlySequence<byte>(buffer.ToArray()));
        }

        /// <summary>
        /// Deserializes one complete payload from a byte sequence.
        /// </summary>
        /// <param name="buffer">
        /// The byte sequence containing the payload.
        /// </param>
        /// <returns>
        /// A materialized sequence of DTOs in payload group order.
        /// </returns>
        /// <exception cref="MessagePackSerializationException">
        /// Thrown when the payload is malformed, contains invalid values or field counts, or has trailing data.
        /// </exception>
        public IEnumerable<IThing> DeSerialize(ReadOnlySequence<byte> buffer)
        {
            var reader = new MessagePackReader(buffer);
            var payload = global::MessagePack.MessagePackSerializer.Deserialize<Payload>(ref reader, SerializerOptions);

            if (!reader.End)
            {
                throw new MessagePackSerializationException("The MessagePack payload contains trailing data.");
            }

            if (payload == null)
            {
                throw new MessagePackSerializationException("The MessagePack payload may not be nil.");
            }

            return PayloadFactory.ToDataItems(payload);
        }

        /// <summary>
        /// Asynchronously deserializes one complete payload from a caller-owned stream.
        /// </summary>
        /// <param name="stream">
        /// The stream containing the payload.
        /// </param>
        /// <param name="cancellationToken">
        /// The token used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task whose result is a materialized sequence of DTOs in payload group order.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="stream" /> is null.
        /// </exception>
        /// <exception cref="MessagePackSerializationException">
        /// Thrown when the payload is malformed, contains invalid values or field counts, or has trailing data.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Thrown when <paramref name="cancellationToken" /> is canceled.
        /// </exception>
        public Task<IEnumerable<IThing>> DeSerializeAsync(Stream stream, CancellationToken cancellationToken)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            return this.DeSerializeInternalAsync(stream, cancellationToken);
        }

        /// <summary>
        /// Reads the complete stream asynchronously and materializes its DTOs.
        /// </summary>
        /// <param name="stream">
        /// The caller-owned stream containing the payload.
        /// </param>
        /// <param name="cancellationToken">
        /// The token used to cancel reading and deserialization.
        /// </param>
        /// <returns>
        /// A task whose result is a materialized sequence of DTOs.
        /// </returns>
        /// <exception cref="OperationCanceledException">
        /// Thrown when <paramref name="cancellationToken" /> is canceled.
        /// </exception>
        /// <exception cref="MessagePackSerializationException">
        /// Thrown when the payload is malformed, contains invalid values or field counts, or has trailing data.
        /// </exception>
        private async Task<IEnumerable<IThing>> DeSerializeInternalAsync(Stream stream, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var buffer = new MemoryStream();
            await stream.CopyToAsync(buffer, 81920, cancellationToken).ConfigureAwait(false);

            cancellationToken.ThrowIfCancellationRequested();

            var result = this.DeSerialize(new ReadOnlySequence<byte>(buffer.ToArray()));

            cancellationToken.ThrowIfCancellationRequested();

            return result;
        }
    }
}
