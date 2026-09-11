// ------------------------------------------------------------------------------------------------
//  <copyright file="DeSerializer.cs" company="Starion Group S.A.">
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
    using Mycelium.SDK.Serializer.Json.Utility;

    /// <summary>
    /// Deserializes complete Mycelium JSON payloads from caller-owned streams.
    /// </summary>
    public class DeSerializer : IDeSerializer
    {
        /// <summary>
        /// The buffer size used to copy an input stream.
        /// </summary>
        private const int StreamCopyBufferSize = 81920;

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
        public IEnumerable<IThing> DeSerialize(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            using var payloadStream = new MemoryStream();

            stream.CopyTo(payloadStream);

            var payload = payloadStream.GetBuffer()
                .AsSpan(0, checked((int)payloadStream.Length));

            return DeSerializePayload(payload, CancellationToken.None);
        }

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
        public async Task<IEnumerable<IThing>> DeSerializeAsync(Stream stream, CancellationToken cancellationToken)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            cancellationToken.ThrowIfCancellationRequested();

            using var payloadStream = new MemoryStream();

            await stream.CopyToAsync(payloadStream, StreamCopyBufferSize, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            var payload = payloadStream.GetBuffer()
                .AsSpan(0, checked((int)payloadStream.Length));

            return DeSerializePayload(payload, cancellationToken);
        }

        /// <summary>
        /// Deserializes one complete raw UTF-8 payload.
        /// </summary>
        private static IEnumerable<IThing> DeSerializePayload(ReadOnlySpan<byte> payload, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var reader = new Utf8JsonReader(payload);

            if (!reader.Read())
            {
                throw new JsonException("The JSON payload is empty.");
            }

            List<IThing> result;

            switch (reader.TokenType)
            {
                case JsonTokenType.StartObject:
                    result =
                    [
                        DeSerializeObject(ref reader),
                    ];

                    break;
                case JsonTokenType.StartArray:
                    result = DeSerializeArray(ref reader, cancellationToken);
                    break;
                default:
                    throw new JsonException("The JSON payload root must be an object or an array.");
            }

            cancellationToken.ThrowIfCancellationRequested();

            if (reader.Read())
            {
                throw new JsonException("Unexpected JSON content follows the complete payload root.");
            }

            return result;
        }

        /// <summary>
        /// Deserializes every DTO object in the current JSON array.
        /// </summary>
        private static List<IThing> DeSerializeArray(ref Utf8JsonReader reader, CancellationToken cancellationToken)
        {
            var result = new List<IThing>();

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!reader.Read())
                {
                    throw new JsonException("The JSON payload array is incomplete.");
                }

                if (reader.TokenType == JsonTokenType.EndArray)
                {
                    return result;
                }

                if (reader.TokenType != JsonTokenType.StartObject)
                {
                    throw new JsonException("Every JSON payload array element must be an object.");
                }

                result.Add(DeSerializeObject(ref reader));
            }
        }

        /// <summary>
        /// Deserializes the DTO object on which the reader is positioned.
        /// </summary>
        private static IThing DeSerializeObject(ref Utf8JsonReader reader)
        {
            var typeName = ReadTypeName(ref reader);
            var operation = DeSerializationProvider.Provide(typeName);

            return operation(ref reader);
        }

        /// <summary>
        /// Reads the unique top-level discriminator without advancing the original reader.
        /// </summary>
        private static string ReadTypeName(ref Utf8JsonReader reader)
        {
            var discriminatorReader = reader;

            Utf8JsonReaderHelper.Expect(ref discriminatorReader, JsonTokenType.StartObject);

            var hasType = false;
            var hasEndObject = false;
            string typeName = null;

            while (discriminatorReader.Read())
            {
                if (discriminatorReader.TokenType == JsonTokenType.EndObject)
                {
                    hasEndObject = true;
                    break;
                }

                Utf8JsonReaderHelper.Expect(ref discriminatorReader, JsonTokenType.PropertyName);

                var isType = discriminatorReader.ValueTextEquals("@type"u8);

                if (isType && hasType)
                {
                    throw new JsonException("The @type metadata property occurs more than once.");
                }

                Utf8JsonReaderHelper.ReadNext(ref discriminatorReader);

                if (isType)
                {
                    typeName = Utf8JsonReaderHelper.ReadRequiredString(ref discriminatorReader);
                    hasType = true;
                    continue;
                }

                Utf8JsonReaderHelper.SkipValue(ref discriminatorReader);
            }

            if (!hasEndObject)
            {
                throw new JsonException("The JSON DTO object is incomplete.");
            }

            if (!hasType)
            {
                throw new JsonException("The required @type metadata property is missing.");
            }

            return typeName;
        }
    }
}
