// ------------------------------------------------------------------------------------------------
//  <copyright file="Serializer.cs" company="Starion Group S.A.">
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
    /// Serializes Mycelium DTOs as JSON to caller-owned streams.
    /// </summary>
    public class Serializer : ISerializer
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
        public void Serialize(IThing dto, Stream stream, JsonWriterOptions jsonWriterOptions)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            using var writer = new Utf8JsonWriter(stream, jsonWriterOptions);

            var serializationAction = SerializationProvider.Provide(dto.GetType());

            serializationAction(dto, writer);

            writer.Flush();
        }

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
        public void Serialize(IEnumerable<IThing> dtos, Stream stream, JsonWriterOptions jsonWriterOptions)
        {
            if (dtos == null)
            {
                throw new ArgumentNullException(nameof(dtos));
            }

            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            using var writer = new Utf8JsonWriter(stream, jsonWriterOptions);

            writer.WriteStartArray();

            foreach (var dto in dtos)
            {
                if (dto == null)
                {
                    throw new ArgumentNullException(nameof(dtos), "The DTO sequence contains a null element.");
                }

                var serializationAction = SerializationProvider.Provide(dto.GetType());

                serializationAction(dto, writer);
            }

            writer.WriteEndArray();
            writer.Flush();
        }

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
        public async Task SerializeAsync(IThing dto, Stream stream, JsonWriterOptions jsonWriterOptions, CancellationToken cancellationToken)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            cancellationToken.ThrowIfCancellationRequested();

            using var writer = new Utf8JsonWriter(stream, jsonWriterOptions);

            var serializationAction = SerializationProvider.Provide(dto.GetType());

            serializationAction(dto, writer);

            await writer.FlushAsync(cancellationToken);
        }

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
        public async Task SerializeAsync(IEnumerable<IThing> dtos, Stream stream, JsonWriterOptions jsonWriterOptions, CancellationToken cancellationToken)
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

            using var writer = new Utf8JsonWriter(stream, jsonWriterOptions);

            writer.WriteStartArray();

            foreach (var dto in dtos)
            {
                if (dto == null)
                {
                    throw new ArgumentNullException(nameof(dtos), "The DTO sequence contains a null element.");
                }

                cancellationToken.ThrowIfCancellationRequested();

                var serializationAction = SerializationProvider.Provide(dto.GetType());

                serializationAction(dto, writer);
            }

            writer.WriteEndArray();

            await writer.FlushAsync(cancellationToken);
        }
    }
}
