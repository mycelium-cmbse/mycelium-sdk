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
    using System.Buffers;
    using System.Text.Json;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    using Mycelium.SDK.DTO;
    using Mycelium.SDK.Serializer.Json.Utility;

    /// <summary>
    /// Deserializes complete Mycelium JSON payloads incrementally from caller-owned streams.
    /// </summary>
    public class DeSerializer : IDeSerializer
    {
        /// <summary>
        /// The initial size of the incremental stream-read buffer.
        /// </summary>
        private const int InitialReadBufferSize = 16_384;

        /// <summary>
        /// The logger factory passed to generated DTO deserializers.
        /// </summary>
        private readonly ILoggerFactory loggerFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeSerializer" /> class without externally
        /// observable logging.
        /// </summary>
        public DeSerializer() : this(NullLoggerFactory.Instance)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeSerializer" /> class.
        /// </summary>
        /// <param name="loggerFactory">
        /// The logger factory passed to generated DTO deserializers.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="loggerFactory" /> is <see langword="null" />.
        /// </exception>
        public DeSerializer(ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            this.loggerFactory = loggerFactory;
        }

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
        public IEnumerable<IThing> DeSerialize(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var readBuffer = ArrayPool<byte>.Shared.Rent(InitialReadBufferSize);

            try
            {
                var context = new PayloadReaderContext();
                var readerState = new JsonReaderState();
                var bufferedByteCount = 0;
                var isFinalBlock = false;

                while (!isFinalBlock)
                {
                    EnsureReadCapacity(ref readBuffer, bufferedByteCount);

                    var bytesRead = stream.Read(readBuffer, bufferedByteCount, readBuffer.Length - bufferedByteCount);

                    if (bytesRead == 0)
                    {
                        isFinalBlock = true;
                    }
                    else
                    {
                        bufferedByteCount += bytesRead;
                    }

                    var consumedByteCount = this.ProcessBuffer(readBuffer.AsSpan(0, bufferedByteCount), isFinalBlock, ref readerState, context, CancellationToken.None);

                    bufferedByteCount = MoveRemainingBytes(readBuffer, bufferedByteCount, consumedByteCount);
                }

                context.ValidateCompleteInput();

                return context.Results;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(readBuffer);
            }
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
        public async Task<IEnumerable<IThing>> DeSerializeAsync(Stream stream, CancellationToken cancellationToken)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var readBuffer = ArrayPool<byte>.Shared.Rent(InitialReadBufferSize);

            try
            {
                var context = new PayloadReaderContext();
                var readerState = new JsonReaderState();
                var bufferedByteCount = 0;
                var isFinalBlock = false;

                while (!isFinalBlock)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    EnsureReadCapacity(ref readBuffer, bufferedByteCount);

                    var bytesRead = await stream.ReadAsync(readBuffer.AsMemory(bufferedByteCount, readBuffer.Length - bufferedByteCount), cancellationToken);

                    if (bytesRead == 0)
                    {
                        isFinalBlock = true;
                    }
                    else
                    {
                        bufferedByteCount += bytesRead;
                    }

                    var consumedByteCount = this.ProcessBuffer(readBuffer.AsSpan(0, bufferedByteCount), isFinalBlock, ref readerState, context, cancellationToken);

                    bufferedByteCount = MoveRemainingBytes(readBuffer, bufferedByteCount, consumedByteCount);
                }

                cancellationToken.ThrowIfCancellationRequested();

                context.ValidateCompleteInput();

                return context.Results;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(readBuffer);
            }
        }

        /// <summary>
        /// Processes every complete token currently available in the read buffer.
        /// </summary>
        private int ProcessBuffer(ReadOnlySpan<byte> buffer, bool isFinalBlock, ref JsonReaderState readerState, PayloadReaderContext context, CancellationToken cancellationToken)
        {
            var reader = new Utf8JsonReader(buffer, isFinalBlock, readerState);
            var captureStart = context.IsCapturingObject ? 0 : -1;

            while (reader.Read())
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (context.IsCapturingObject)
                {
                    if (reader.TokenType == JsonTokenType.EndObject && reader.CurrentDepth == context.CurrentObjectDepth)
                    {
                        var objectEnd = checked((int)reader.BytesConsumed);

                        context.AppendObjectBytes(buffer.Slice(captureStart, objectEnd - captureStart));

                        context.CompleteObject(this, cancellationToken);
                        captureStart = -1;
                    }

                    continue;
                }

                if (context.RootComplete)
                {
                    throw new JsonException("Unexpected JSON content follows the complete payload root.");
                }

                if (!context.RootStarted)
                {
                    switch (reader.TokenType)
                    {
                        case JsonTokenType.StartObject:
                            context.BeginObjectRoot(reader.CurrentDepth);
                            captureStart = checked((int)reader.TokenStartIndex);
                            break;
                        case JsonTokenType.StartArray:
                            context.BeginArrayRoot();
                            break;
                        default:
                            throw new JsonException("The JSON payload root must be an object or an array.");
                    }

                    continue;
                }

                if (reader.TokenType == JsonTokenType.StartObject && reader.CurrentDepth == 1)
                {
                    context.BeginArrayObject(reader.CurrentDepth);
                    captureStart = checked((int)reader.TokenStartIndex);
                    continue;
                }

                if (reader.TokenType == JsonTokenType.EndArray && reader.CurrentDepth == 0)
                {
                    context.CompleteArray();
                    continue;
                }

                throw new JsonException("Every JSON payload array element must be an object.");
            }

            var consumedByteCount = checked((int)reader.BytesConsumed);

            if (context.IsCapturingObject)
            {
                context.AppendObjectBytes(buffer.Slice(captureStart, consumedByteCount - captureStart));
            }

            readerState = reader.CurrentState;

            return consumedByteCount;
        }

        /// <summary>
        /// Deserializes one buffered DTO object through the generated provider.
        /// </summary>
        private IThing DeSerializeObjectPayload(ReadOnlySpan<byte> payload)
        {
            var reader = new Utf8JsonReader(payload);

            if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("Every JSON payload array element must be an object.");
            }

            var result = this.DeSerializeObject(ref reader);

            if (reader.TokenType != JsonTokenType.EndObject)
            {
                throw new JsonException("The JSON DTO object is incomplete.");
            }

            if (reader.Read())
            {
                throw new JsonException("Unexpected JSON content follows the complete DTO object.");
            }

            return result;
        }

        /// <summary>
        /// Deserializes the DTO object on which the reader is positioned.
        /// </summary>
        private IThing DeSerializeObject(ref Utf8JsonReader reader)
        {
            var typeName = ReadTypeName(ref reader);
            var operation = DeSerializationProvider.Provide(typeName);

            return operation(ref reader, this.loggerFactory);
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

        /// <summary>
        /// Expands the pooled read buffer when an incomplete token occupies the complete buffer.
        /// </summary>
        private static void EnsureReadCapacity(ref byte[] readBuffer, int bufferedByteCount)
        {
            if (bufferedByteCount < readBuffer.Length)
            {
                return;
            }

            var expandedBuffer = ArrayPool<byte>.Shared.Rent(checked(readBuffer.Length * 2));

            Buffer.BlockCopy(readBuffer, 0, expandedBuffer, 0, bufferedByteCount);

            ArrayPool<byte>.Shared.Return(readBuffer);
            readBuffer = expandedBuffer;
        }

        /// <summary>
        /// Moves bytes belonging to an incomplete token to the start of the read buffer.
        /// </summary>
        private static int MoveRemainingBytes(byte[] readBuffer, int bufferedByteCount, int consumedByteCount)
        {
            var remainingByteCount = bufferedByteCount - consumedByteCount;

            if (remainingByteCount > 0)
            {
                Buffer.BlockCopy(readBuffer, consumedByteCount, readBuffer, 0, remainingByteCount);
            }

            return remainingByteCount;
        }

        /// <summary>
        /// Identifies the complete JSON root representation.
        /// </summary>
        private enum RootKind
        {
            None,
            Object,
            Array,
        }

        /// <summary>
        /// Retains parser state, materialized DTOs and at most one raw DTO object.
        /// </summary>
        private sealed class PayloadReaderContext
        {
            private readonly ArrayBufferWriter<byte> objectBuffer = new(InitialReadBufferSize);

            private RootKind rootKind;

            internal List<IThing> Results { get; } = [];

            internal bool RootStarted => this.rootKind != RootKind.None;

            internal bool RootComplete { get; private set; }

            internal bool IsCapturingObject { get; private set; }

            internal int CurrentObjectDepth { get; private set; }

            internal void BeginObjectRoot(int depth)
            {
                this.rootKind = RootKind.Object;
                this.BeginObject(depth);
            }

            internal void BeginArrayRoot() => this.rootKind = RootKind.Array;

            internal void BeginArrayObject(int depth) => this.BeginObject(depth);

            internal void AppendObjectBytes(ReadOnlySpan<byte> bytes) => this.objectBuffer.Write(bytes);

            internal void CompleteObject(DeSerializer deSerializer, CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var result = deSerializer.DeSerializeObjectPayload(this.objectBuffer.WrittenSpan);

                cancellationToken.ThrowIfCancellationRequested();

                this.Results.Add(result);
                this.objectBuffer.Clear();
                this.IsCapturingObject = false;

                if (this.rootKind == RootKind.Object)
                {
                    this.RootComplete = true;
                }
            }

            internal void CompleteArray() => this.RootComplete = true;

            internal void ValidateCompleteInput()
            {
                if (!this.RootStarted)
                {
                    throw new JsonException("The JSON payload is empty.");
                }

                if (this.RootComplete)
                {
                    return;
                }

                if (this.rootKind == RootKind.Array)
                {
                    throw new JsonException("The JSON payload array is incomplete.");
                }

                throw new JsonException("The JSON DTO object is incomplete.");
            }

            private void BeginObject(int depth)
            {
                this.objectBuffer.Clear();
                this.CurrentObjectDepth = depth;
                this.IsCapturingObject = true;
            }
        }
    }
}
