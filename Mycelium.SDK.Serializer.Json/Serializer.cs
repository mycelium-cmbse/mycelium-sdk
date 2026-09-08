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
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    using Mycelium.SDK.DTO;

    /// <summary>
    /// Serializes Mycelium DTOs as JSON to caller-owned streams.
    /// </summary>
    public class Serializer : ISerializer
    {
        /// <inheritdoc />
        public void Serialize(
            IThing dto,
            Stream stream,
            JsonWriterOptions jsonWriterOptions)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            using var writer =
                new Utf8JsonWriter(stream, jsonWriterOptions);

            var serializationAction =
                SerializationProvider.Provide(dto.GetType());

            serializationAction(dto, writer);

            writer.Flush();
        }

        /// <inheritdoc />
        public void Serialize(
            IEnumerable<IThing> dtos,
            Stream stream,
            JsonWriterOptions jsonWriterOptions)
        {
            if (dtos == null)
            {
                throw new ArgumentNullException(nameof(dtos));
            }

            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            using var writer =
                new Utf8JsonWriter(stream, jsonWriterOptions);

            writer.WriteStartArray();

            foreach (var dto in dtos)
            {
                if (dto == null)
                {
                    throw new ArgumentNullException(
                        nameof(dtos),
                        "The DTO sequence contains a null element.");
                }

                var serializationAction =
                    SerializationProvider.Provide(dto.GetType());

                serializationAction(dto, writer);
            }

            writer.WriteEndArray();
            writer.Flush();
        }

        /// <inheritdoc />
        public async Task SerializeAsync(
            IThing dto,
            Stream stream,
            JsonWriterOptions jsonWriterOptions,
            CancellationToken cancellationToken)
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

            using var writer =
                new Utf8JsonWriter(stream, jsonWriterOptions);

            var serializationAction =
                SerializationProvider.Provide(dto.GetType());

            serializationAction(dto, writer);

            await writer.FlushAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task SerializeAsync(
            IEnumerable<IThing> dtos,
            Stream stream,
            JsonWriterOptions jsonWriterOptions,
            CancellationToken cancellationToken)
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

            using var writer =
                new Utf8JsonWriter(stream, jsonWriterOptions);

            writer.WriteStartArray();

            foreach (var dto in dtos)
            {
                if (dto == null)
                {
                    throw new ArgumentNullException(
                        nameof(dtos),
                        "The DTO sequence contains a null element.");
                }

                cancellationToken.ThrowIfCancellationRequested();

                var serializationAction =
                    SerializationProvider.Provide(dto.GetType());

                serializationAction(dto, writer);
            }

            writer.WriteEndArray();

            await writer.FlushAsync(cancellationToken);
        }
    }
}
