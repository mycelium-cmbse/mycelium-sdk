// ------------------------------------------------------------------------------------------------
//  <copyright file="IDeSerializer.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.Serializer.Json
{
    using System.Text.Json;

    using Mycelium.SDK.DTO;

    /// <summary>
    /// Defines operations that deserialize complete JSON payloads from caller-owned streams.
    /// </summary>
    public interface IDeSerializer
    {
        /// <summary>
        /// Deserializes one complete JSON object-or-array payload.
        /// </summary>
        /// <param name="stream">
        /// The stream containing the JSON payload.
        /// </param>
        /// <returns>
        /// A materialized sequence containing one DTO for an object root or every DTO for an array
        /// root.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="stream" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="JsonException">
        /// Thrown when the JSON payload is malformed or violates the JSON DTO contract.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Thrown when an exact <c>@type</c> discriminator is not supported.
        /// </exception>
        IEnumerable<IThing> DeSerialize(Stream stream);

        /// <summary>
        /// Asynchronously deserializes one complete JSON object-or-array payload.
        /// </summary>
        /// <param name="stream">
        /// The stream containing the JSON payload.
        /// </param>
        /// <param name="cancellationToken">
        /// The token used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task whose result is a materialized sequence containing one DTO for an object root or
        /// every DTO for an array root.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="stream" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="JsonException">
        /// Thrown when the JSON payload is malformed or violates the JSON DTO contract.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Thrown when an exact <c>@type</c> discriminator is not supported.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Thrown when <paramref name="cancellationToken" /> is cancelled.
        /// </exception>
        Task<IEnumerable<IThing>> DeSerializeAsync(Stream stream, CancellationToken cancellationToken);
    }
}
