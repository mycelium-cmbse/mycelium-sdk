// ------------------------------------------------------------------------------------------------
//  <copyright file="DeSerializerBenchmarks.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.Serializer.Json.Benchmarks
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Columns;
    using BenchmarkDotNet.Configs;

    using Mycelium.SDK.DTO;

    /// <summary>
    /// Configures metrics that supplement BenchmarkDotNet's default timing columns.
    /// </summary>
    public sealed class BenchmarkConfiguration : ManualConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BenchmarkConfiguration" /> class.
        /// </summary>
        public BenchmarkConfiguration()
        {
            this.AddColumn(StatisticColumn.OperationsPerSecond);
        }
    }

    /// <summary>
    /// Measures deserialization of complete JSON object roots from a <see cref="MemoryStream" />.
    /// </summary>
    [Config(typeof(BenchmarkConfiguration))]
    [MemoryDiagnoser]
    public class ObjectRootDeserializationBenchmarks
    {
        /// <summary>
        /// The copy-buffer size used by the current asynchronous implementation.
        /// </summary>
        private const int CurrentStreamCopyBufferSize = 81_920;

        /// <summary>
        /// The prepared deserialization operation.
        /// </summary>
        private DeserializationOperation operation;

        /// <summary>
        /// Gets or sets the stream implementation reported with every result.
        /// </summary>
        [Params(nameof(MemoryStream))]
        public string StreamImplementation { get; set; }

        /// <summary>
        /// Gets or sets the number of characters in the object's bounded or oversized content
        /// property.
        /// </summary>
        [Params(256, 131_072)]
        public int ContentLength { get; set; }

        /// <summary>
        /// Generates the object payload outside the measured operation.
        /// </summary>
        [GlobalSetup]
        public void GlobalSetup()
        {
            var payload = JsonPayloadFactory.CreateObjectPayload(this.ContentLength);

            if (this.ContentLength > CurrentStreamCopyBufferSize && payload.Length <= CurrentStreamCopyBufferSize)
            {
                throw new InvalidOperationException("The oversized object payload must exceed the current stream-copy buffer.");
            }

            this.operation = new DeserializationOperation(payload, 1, this.StreamImplementation);
        }

        /// <summary>
        /// Releases the prepared input stream.
        /// </summary>
        [GlobalCleanup]
        public void GlobalCleanup() => this.operation.Dispose();

        /// <summary>
        /// Deserializes one complete object root synchronously.
        /// </summary>
        /// <returns>
        /// The validated materialized result.
        /// </returns>
        [Benchmark]
        public object Synchronous() => this.operation.DeSerialize();

        /// <summary>
        /// Deserializes one complete object root asynchronously.
        /// </summary>
        /// <returns>
        /// A task whose result is the validated materialized sequence.
        /// </returns>
        [Benchmark]
        public Task<object> Asynchronous() => this.operation.DeSerializeAsync();
    }

    /// <summary>
    /// Measures deserialization of complete JSON array roots from a <see cref="MemoryStream" />.
    /// </summary>
    [Config(typeof(BenchmarkConfiguration))]
    [MemoryDiagnoser]
    public class ArrayRootDeserializationBenchmarks
    {
        /// <summary>
        /// The fixed content length used by every bounded-size array element.
        /// </summary>
        private const int BoundedContentLength = 128;

        /// <summary>
        /// The prepared deserialization operation.
        /// </summary>
        private DeserializationOperation operation;

        /// <summary>
        /// Gets or sets the stream implementation reported with every result.
        /// </summary>
        [Params(nameof(MemoryStream))]
        public string StreamImplementation { get; set; }

        /// <summary>
        /// Gets or sets the number of bounded-size objects in the JSON array.
        /// </summary>
        [Params(100, 1_000, 10_000)]
        public int ObjectCount { get; set; }

        /// <summary>
        /// Generates the array payload outside the measured operation.
        /// </summary>
        [GlobalSetup]
        public void GlobalSetup()
        {
            var payload = JsonPayloadFactory.CreateArrayPayload(this.ObjectCount, BoundedContentLength);

            this.operation = new DeserializationOperation(payload, this.ObjectCount, this.StreamImplementation);
        }

        /// <summary>
        /// Releases the prepared input stream.
        /// </summary>
        [GlobalCleanup]
        public void GlobalCleanup() => this.operation.Dispose();

        /// <summary>
        /// Deserializes one complete array root synchronously.
        /// </summary>
        /// <returns>
        /// The validated materialized result.
        /// </returns>
        [Benchmark]
        public object Synchronous() => this.operation.DeSerialize();

        /// <summary>
        /// Deserializes one complete array root asynchronously.
        /// </summary>
        /// <returns>
        /// A task whose result is the validated materialized sequence.
        /// </returns>
        [Benchmark]
        public Task<object> Asynchronous() => this.operation.DeSerializeAsync();
    }

    /// <summary>
    /// Owns one prepared payload stream and validates every deserialization invocation.
    /// </summary>
    internal sealed class DeserializationOperation : IDisposable
    {
        /// <summary>
        /// The public JSON deserializer under measurement.
        /// </summary>
        private readonly DeSerializer deSerializer = new();

        /// <summary>
        /// The required number of returned DTOs.
        /// </summary>
        private readonly int expectedResultCount;

        /// <summary>
        /// The caller-owned input stream.
        /// </summary>
        private readonly MemoryStream stream;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeserializationOperation" /> class.
        /// </summary>
        /// <param name="payload">
        /// The complete prepared UTF-8 JSON payload.
        /// </param>
        /// <param name="expectedResultCount">
        /// The expected number of materialized DTOs.
        /// </param>
        /// <param name="streamImplementation">
        /// The requested stream implementation.
        /// </param>
        internal DeserializationOperation(byte[] payload, int expectedResultCount, string streamImplementation)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            if (expectedResultCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(expectedResultCount));
            }

            if (!string.Equals(streamImplementation, nameof(MemoryStream), StringComparison.Ordinal))
            {
                throw new ArgumentException($"Only {nameof(MemoryStream)} is supported by this baseline benchmark.", nameof(streamImplementation));
            }

            this.expectedResultCount = expectedResultCount;
            this.stream = new MemoryStream(payload, false);
        }

        /// <summary>
        /// Gets the exact UTF-8 payload length.
        /// </summary>
        internal int PayloadByteCount => checked((int)this.stream.Length);

        /// <summary>
        /// Gets the concrete input-stream type.
        /// </summary>
        internal string StreamImplementation => this.stream.GetType()
            .Name;

        /// <summary>
        /// Releases the caller-owned input stream after all measurements have completed.
        /// </summary>
        public void Dispose() => this.stream.Dispose();

        /// <summary>
        /// Runs and validates one synchronous deserialization invocation.
        /// </summary>
        /// <returns>
        /// The validated materialized result.
        /// </returns>
        internal object DeSerialize()
        {
            this.PrepareInvocation();

            var result = this.deSerializer.DeSerialize(this.stream);

            return this.ValidateResult(result);
        }

        /// <summary>
        /// Runs and validates one asynchronous deserialization invocation.
        /// </summary>
        /// <returns>
        /// A task whose result is the validated materialized sequence.
        /// </returns>
        internal async Task<object> DeSerializeAsync()
        {
            this.PrepareInvocation();

            var result = await this.deSerializer.DeSerializeAsync(this.stream, CancellationToken.None);

            return this.ValidateResult(result);
        }

        /// <summary>
        /// Restores the input stream to the start immediately before an invocation.
        /// </summary>
        private void PrepareInvocation() => this.stream.Position = 0;

        /// <summary>
        /// Consumes the result and validates stream consumption and result materialization.
        /// </summary>
        /// <param name="result">
        /// The returned materialized sequence.
        /// </param>
        /// <returns>
        /// The validated sequence.
        /// </returns>
        private IEnumerable<IThing> ValidateResult(IEnumerable<IThing> result)
        {
            if (result == null)
            {
                throw new InvalidOperationException("The deserializer returned a null result.");
            }

            var resultCount = 0;

            foreach (var dto in result)
            {
                if (dto == null)
                {
                    throw new InvalidOperationException("The deserializer returned a null DTO.");
                }

                resultCount++;
            }

            if (resultCount != this.expectedResultCount)
            {
                throw new InvalidOperationException($"Expected {this.expectedResultCount} DTOs but received {resultCount}.");
            }

            if (!this.stream.CanRead)
            {
                throw new InvalidOperationException("The deserializer closed the caller-owned input stream.");
            }

            if (this.stream.Position != this.stream.Length)
            {
                throw new InvalidOperationException($"The deserializer consumed {this.stream.Position} of " + $"{this.stream.Length} payload bytes.");
            }

            return result;
        }
    }

    /// <summary>
    /// Measures peak process working set in a fresh process outside BenchmarkDotNet timing.
    /// </summary>
    internal static class PeakMemoryMeasurement
    {
        /// <summary>
        /// The largest bounded-object array size used by the benchmark suite.
        /// </summary>
        private const int ArrayObjectCount = 10_000;

        /// <summary>
        /// The command-line switch selecting peak-process-memory measurement.
        /// </summary>
        private const string SwitchName = "--peak-memory";

        /// <summary>
        /// The synchronous operation argument.
        /// </summary>
        private const string SynchronousOperation = "synchronous";

        /// <summary>
        /// The asynchronous operation argument.
        /// </summary>
        private const string AsynchronousOperation = "asynchronous";

        /// <summary>
        /// Determines whether the separate peak-memory procedure was requested.
        /// </summary>
        /// <param name="arguments">
        /// The command-line arguments.
        /// </param>
        /// <returns>
        /// <see langword="true" /> when the peak-memory switch is present; otherwise,
        /// <see langword="false" />.
        /// </returns>
        internal static bool IsRequested(IReadOnlyList<string> arguments)
        {
            return arguments.Count > 0 && string.Equals(arguments[0], SwitchName, StringComparison.Ordinal);
        }

        /// <summary>
        /// Runs the largest-array scenario once and reports the process peak working set.
        /// </summary>
        /// <param name="arguments">
        /// The peak-memory command and operation selection.
        /// </param>
        /// <returns>
        /// A task representing the measurement.
        /// </returns>
        internal static async Task RunAsync(IReadOnlyList<string> arguments)
        {
            if (arguments.Count != 2)
            {
                throw CreateUsageException();
            }

            var operationName = arguments[1];

            var payload = JsonPayloadFactory.CreateArrayPayload(ArrayObjectCount, 128);

            using var operation = new DeserializationOperation(payload, ArrayObjectCount, nameof(MemoryStream));

            var result = operationName switch
            {
                SynchronousOperation => operation.DeSerialize(),
                AsynchronousOperation => await operation.DeSerializeAsync(),
                _ => throw CreateUsageException(),
            };

            using var process = Process.GetCurrentProcess();

            process.Refresh();

            var peakWorkingSetBytes = process.PeakWorkingSet64;

            Console.WriteLine("Metric: Peak process working set bytes (Process.PeakWorkingSet64)");

            Console.WriteLine($"Operation: {operationName}");
            Console.WriteLine("Root: Array");
            Console.WriteLine($"Stream implementation: {operation.StreamImplementation}");

            Console.WriteLine($"Payload bytes: " + $"{operation.PayloadByteCount.ToString(CultureInfo.InvariantCulture)}");

            Console.WriteLine($"Returned DTOs: " + $"{ArrayObjectCount.ToString(CultureInfo.InvariantCulture)}");

            Console.WriteLine($"Peak process working set bytes: " + $"{peakWorkingSetBytes.ToString(CultureInfo.InvariantCulture)}");

            GC.KeepAlive(result);
            GC.KeepAlive(operation);
            GC.KeepAlive(payload);
        }

        /// <summary>
        /// Creates the peak-memory command usage exception.
        /// </summary>
        /// <returns>
        /// The command usage exception.
        /// </returns>
        private static ArgumentException CreateUsageException()
        {
            return new ArgumentException($"Usage: {SwitchName} " + $"<{SynchronousOperation}|{AsynchronousOperation}>");
        }
    }

    /// <summary>
    /// Generates deterministic valid Mycelium JSON payloads outside measured operations.
    /// </summary>
    internal static class JsonPayloadFactory
    {
        /// <summary>
        /// The deterministic creation timestamp written to generated payloads.
        /// </summary>
        private const string CreatedOn = "2026-01-02T03:04:05.0000000Z";

        /// <summary>
        /// The deterministic update timestamp written to generated payloads.
        /// </summary>
        private const string UpdatedOn = "2026-02-03T04:05:06.0000000Z";

        /// <summary>
        /// The deterministic author identifier written to generated payloads.
        /// </summary>
        private static readonly Guid Author = Guid.Parse("10000000-0000-0000-0000-000000000001");

        /// <summary>
        /// The deterministic creator identifier written to generated payloads.
        /// </summary>
        private static readonly Guid CreatedBy = Guid.Parse("10000000-0000-0000-0000-000000000002");

        /// <summary>
        /// The first deterministic reply identifier written to generated payloads.
        /// </summary>
        private static readonly Guid FirstReply = Guid.Parse("10000000-0000-0000-0000-000000000003");

        /// <summary>
        /// The second deterministic reply identifier written to generated payloads.
        /// </summary>
        private static readonly Guid SecondReply = Guid.Parse("10000000-0000-0000-0000-000000000004");

        /// <summary>
        /// The deterministic target-element identifier written to generated payloads.
        /// </summary>
        private static readonly Guid TargetElementId = Guid.Parse("10000000-0000-0000-0000-000000000005");

        /// <summary>
        /// The deterministic updater identifier written to generated payloads.
        /// </summary>
        private static readonly Guid UpdatedBy = Guid.Parse("10000000-0000-0000-0000-000000000006");

        /// <summary>
        /// Creates a complete object-root payload.
        /// </summary>
        /// <param name="contentLength">
        /// The number of characters in the content property.
        /// </param>
        /// <returns>
        /// The complete UTF-8 JSON payload.
        /// </returns>
        internal static byte[] CreateObjectPayload(int contentLength)
        {
            if (contentLength < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(contentLength));
            }

            var content = new string('x', contentLength);

            using var stream = new MemoryStream();

            using (var writer = new Utf8JsonWriter(stream))
            {
                WriteComment(writer, 1, content);
                writer.Flush();
            }

            return stream.ToArray();
        }

        /// <summary>
        /// Creates a complete array-root payload containing bounded-size objects.
        /// </summary>
        /// <param name="objectCount">
        /// The number of objects to write.
        /// </param>
        /// <param name="contentLength">
        /// The fixed content length for every object.
        /// </param>
        /// <returns>
        /// The complete UTF-8 JSON payload.
        /// </returns>
        internal static byte[] CreateArrayPayload(int objectCount, int contentLength)
        {
            if (objectCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(objectCount));
            }

            if (contentLength < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(contentLength));
            }

            var content = new string('x', contentLength);

            using var stream = new MemoryStream();

            using (var writer = new Utf8JsonWriter(stream))
            {
                writer.WriteStartArray();

                for (var index = 0; index < objectCount; index++)
                {
                    WriteComment(writer, index + 1, content);
                }

                writer.WriteEndArray();
                writer.Flush();
            }

            return stream.ToArray();
        }

        /// <summary>
        /// Writes one complete deterministic Comment DTO object.
        /// </summary>
        /// <param name="writer">
        /// The JSON writer receiving the object.
        /// </param>
        /// <param name="sequenceNumber">
        /// The deterministic identifier sequence number.
        /// </param>
        /// <param name="content">
        /// The object's content value.
        /// </param>
        private static void WriteComment(Utf8JsonWriter writer, int sequenceNumber, string content)
        {
            writer.WriteStartObject();
            writer.WriteString("@type", "Comment");
            writer.WriteString("@id", CreateId(sequenceNumber));
            writer.WriteString("author", Author);
            writer.WriteString("commentStatus", "OPEN");
            writer.WriteString("content", content);
            writer.WriteString("createdBy", CreatedBy);
            writer.WriteString("createdOn", CreatedOn);
            writer.WritePropertyName("quotes");
            writer.WriteNullValue();
            writer.WriteStartArray("replies");
            writer.WriteStringValue(FirstReply);
            writer.WriteStringValue(SecondReply);
            writer.WriteEndArray();
            writer.WriteString("targetElementId", TargetElementId);
            writer.WriteString("updatedBy", UpdatedBy);
            writer.WriteString("updatedOn", UpdatedOn);
            writer.WriteEndObject();
        }

        /// <summary>
        /// Creates a deterministic valid identifier for one array element.
        /// </summary>
        /// <param name="sequenceNumber">
        /// The element's one-based sequence number.
        /// </param>
        /// <returns>
        /// The deterministic identifier.
        /// </returns>
        private static Guid CreateId(int sequenceNumber)
        {
            return new Guid(sequenceNumber, 0x1111, 0x2222, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88, 0x99, 0xAA);
        }
    }
}
