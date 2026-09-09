// ------------------------------------------------------------------------------------------------
//  <copyright file="ISerializer.cs" company="Starion Group S.A.">
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
    /// Defines operations that serialize Mycelium DTOs as JSON to caller-owned streams.
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// Serializes one DTO as a JSON object.
        /// </summary>
        /// <param name="dto">
        /// The DTO to serialize.
        /// </param>
        /// <param name="stream">
        /// The stream that receives the JSON.
        /// </param>
        /// <param name="jsonWriterOptions">
        /// The JSON writer options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="dto" /> or <paramref name="stream" /> is
        /// <see langword="null" />.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Thrown when the exact runtime DTO type is not supported.
        /// </exception>
        void Serialize(IThing dto, Stream stream, JsonWriterOptions jsonWriterOptions);

        /// <summary>
        /// Serializes a sequence of DTOs as a JSON array.
        /// </summary>
        /// <param name="dtos">
        /// The DTOs to serialize.
        /// </param>
        /// <param name="stream">
        /// The stream that receives the JSON.
        /// </param>
        /// <param name="jsonWriterOptions">
        /// The JSON writer options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="dtos" />, <paramref name="stream" />, or
        /// an element of <paramref name="dtos" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Thrown when an exact runtime DTO type is not supported.
        /// </exception>
        void Serialize(IEnumerable<IThing> dtos, Stream stream, JsonWriterOptions jsonWriterOptions);

        /// <summary>
        /// Asynchronously serializes one DTO as a JSON object.
        /// </summary>
        /// <param name="dto">
        /// The DTO to serialize.
        /// </param>
        /// <param name="stream">
        /// The stream that receives the JSON.
        /// </param>
        /// <param name="jsonWriterOptions">
        /// The JSON writer options.
        /// </param>
        /// <param name="cancellationToken">
        /// The token used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="dto" /> or <paramref name="stream" /> is
        /// <see langword="null" />.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Thrown when the exact runtime DTO type is not supported.
        /// </exception>
        Task SerializeAsync(IThing dto, Stream stream, JsonWriterOptions jsonWriterOptions, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously serializes a sequence of DTOs as a JSON array.
        /// </summary>
        /// <param name="dtos">
        /// The DTOs to serialize.
        /// </param>
        /// <param name="stream">
        /// The stream that receives the JSON.
        /// </param>
        /// <param name="jsonWriterOptions">
        /// The JSON writer options.
        /// </param>
        /// <param name="cancellationToken">
        /// The token used to cancel the operation.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="dtos" />, <paramref name="stream" />, or
        /// an element of <paramref name="dtos" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Thrown when an exact runtime DTO type is not supported.
        /// </exception>
        Task SerializeAsync(IEnumerable<IThing> dtos, Stream stream, JsonWriterOptions jsonWriterOptions, CancellationToken cancellationToken);
    }
}
