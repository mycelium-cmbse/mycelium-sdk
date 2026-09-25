// ------------------------------------------------------------------------------------------------
//  <copyright file="ISerializer.cs" company="Starion Group S.A.">
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
    /// Defines MessagePack serialization of heterogeneous Mycelium DTO sequences.
    /// </summary>
    public interface ISerializer
    {
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
        void Serialize(IEnumerable<IThing> dtos, Stream stream);

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
        void SerializeToBufferWriter(IEnumerable<IThing> dtos, IBufferWriter<byte> writer, CancellationToken cancellationToken = default);

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
        Task SerializeAsync(IEnumerable<IThing> dtos, Stream stream, CancellationToken cancellationToken);
    }
}
