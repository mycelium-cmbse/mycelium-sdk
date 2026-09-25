// ------------------------------------------------------------------------------------------------
//  <copyright file="IDeSerializer.cs" company="Starion Group S.A.">
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

    /// <summary>
    /// Defines MessagePack deserialization of heterogeneous Mycelium DTO sequences.
    /// </summary>
    public interface IDeSerializer
    {
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
        IEnumerable<IThing> DeSerialize(Stream stream);

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
        IEnumerable<IThing> DeSerialize(ReadOnlySequence<byte> buffer);

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
        Task<IEnumerable<IThing>> DeSerializeAsync(Stream stream, CancellationToken cancellationToken);
    }
}
