// ------------------------------------------------------------------------------------------------
//  <copyright file="IncrementalDeSerializerTestFixture.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.Serializer.Json.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    using Mycelium.SDK.DTO;

    [TestFixture]
    public class IncrementalDeSerializerTestFixture
    {
        private const int OversizedContentLength = 131_072;

        private static readonly DeSerializer JsonDeSerializer = new();

        [Test]
        public async Task Verify_async_cancellation_is_observed_and_the_stream_remains_open()
        {
            using var stream = new BlockingReadStream();
            using var cancellationTokenSource = new CancellationTokenSource();

            var operation = JsonDeSerializer.DeSerializeAsync(stream, cancellationTokenSource.Token);

            await stream.ReadStarted;

            cancellationTokenSource.Cancel();

            await Assert.ThatAsync(() => operation, Throws.TypeOf<OperationCanceledException>());

            using (Assert.EnterMultipleScope())
            {
                Assert.That(stream.CanRead, Is.True);
                Assert.That(stream.IsDisposed, Is.False);
            }
        }

        [Test]
        public async Task Verify_fragmented_non_seekable_streams_produce_equivalent_results()
        {
            var payload = CreateArrayPayload(3, "fragmented-\U0001F600-value");

            using var synchronousStream = new FragmentedNonSeekableStream(payload, 1);

            var synchronousResult = JsonDeSerializer.DeSerialize(synchronousStream)
                .Cast<Comment>()
                .ToArray();

            using var asynchronousStream = new FragmentedNonSeekableStream(payload, 3);

            var asynchronousResult = (await JsonDeSerializer.DeSerializeAsync(asynchronousStream, CancellationToken.None)).Cast<Comment>()
                .ToArray();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(synchronousResult.Select(comment => comment.Id), Is.EqualTo(asynchronousResult.Select(comment => comment.Id)));

                Assert.That(synchronousResult.Select(comment => comment.Content), Is.EqualTo(asynchronousResult.Select(comment => comment.Content)));

                Assert.That(synchronousStream.BytesRead, Is.EqualTo(payload.Length));

                Assert.That(asynchronousStream.BytesRead, Is.EqualTo(payload.Length));

                Assert.That(synchronousStream.CanRead, Is.True);
                Assert.That(asynchronousStream.CanRead, Is.True);
            }
        }

        [Test]
        public void Verify_large_arrays_preserve_order_and_consume_the_complete_payload()
        {
            const int objectCount = 4_096;

            var payload = CreateArrayPayload(objectCount, "bounded");

            using var stream = new FragmentedNonSeekableStream(payload, 17);

            var result = JsonDeSerializer.DeSerialize(stream)
                .Cast<Comment>()
                .ToArray();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Has.Length.EqualTo(objectCount));

                Assert.That(result.Select(comment => comment.Id), Is.EqualTo(Enumerable.Range(1, objectCount)
                    .Select(CreateId)));

                Assert.That(stream.BytesRead, Is.EqualTo(payload.Length));
                Assert.That(stream.CanRead, Is.True);
            }
        }

        [Test]
        public async Task Verify_objects_larger_than_the_initial_buffer_deserialize()
        {
            var content = new string('x', OversizedContentLength);
            var payload = CreateObjectPayload(content);

            using var synchronousStream = new FragmentedNonSeekableStream(payload, 257);

            var synchronousComment = (Comment)JsonDeSerializer.DeSerialize(synchronousStream)
                .Single();

            using var asynchronousStream = new FragmentedNonSeekableStream(payload, 509);

            var asynchronousComment = (Comment)(await JsonDeSerializer.DeSerializeAsync(asynchronousStream, CancellationToken.None)).Single();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(synchronousComment.Content, Has.Length.EqualTo(OversizedContentLength));

                Assert.That(asynchronousComment.Content, Is.EqualTo(synchronousComment.Content));

                Assert.That(synchronousStream.BytesRead, Is.EqualTo(payload.Length));

                Assert.That(asynchronousStream.BytesRead, Is.EqualTo(payload.Length));
            }
        }

        private static byte[] CreateObjectPayload(string content)
        {
            using var stream = new MemoryStream();

            using (var writer = new Utf8JsonWriter(stream))
            {
                WriteComment(writer, 1, content);
                writer.Flush();
            }

            return stream.ToArray();
        }

        private static byte[] CreateArrayPayload(int objectCount, string content)
        {
            using var stream = new MemoryStream();

            using (var writer = new Utf8JsonWriter(stream))
            {
                writer.WriteStartArray();

                for (var index = 1; index <= objectCount; index++)
                {
                    WriteComment(writer, index, content);
                }

                writer.WriteEndArray();
                writer.Flush();
            }

            return stream.ToArray();
        }

        private static void WriteComment(Utf8JsonWriter writer, int sequenceNumber, string content)
        {
            writer.WriteStartObject();
            writer.WriteString("@type", "Comment");
            writer.WriteString("@id", CreateId(sequenceNumber));
            writer.WriteString("content", content);
            writer.WriteEndObject();
        }

        private static Guid CreateId(int sequenceNumber)
        {
            return new Guid(sequenceNumber, 0x1111, 0x2222, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88, 0x99, 0xAA);
        }

        private sealed class FragmentedNonSeekableStream : Stream
        {
            private readonly int fragmentSize;
            private readonly byte[] payload;

            private bool isDisposed;

            private int offset;

            internal FragmentedNonSeekableStream(byte[] payload, int fragmentSize)
            {
                this.payload = payload ?? throw new ArgumentNullException(nameof(payload));

                if (fragmentSize < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(fragmentSize));
                }

                this.fragmentSize = fragmentSize;
            }

            internal int BytesRead => this.offset;

            public override bool CanRead => !this.isDisposed;

            public override bool CanSeek => false;

            public override bool CanWrite => false;

            public override long Length =>
                throw new NotSupportedException();

            public override long Position
            {
                get => throw new NotSupportedException();
                set => throw new NotSupportedException();
            }

            public override void Flush()
            {
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                return this.ReadCore(buffer.AsSpan(offset, count));
            }

            public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
            {
                cancellationToken.ThrowIfCancellationRequested();

                return new ValueTask<int>(this.ReadCore(buffer.Span));
            }

            public override long Seek(long offset, SeekOrigin origin) =>
                throw new NotSupportedException();

            public override void SetLength(long value) => throw new NotSupportedException();

            public override void Write(byte[] buffer, int offset, int count) =>
                throw new NotSupportedException();

            protected override void Dispose(bool disposing)
            {
                this.isDisposed = true;
                base.Dispose(disposing);
            }

            private int ReadCore(Span<byte> destination)
            {
                if (this.isDisposed)
                {
                    throw new ObjectDisposedException(nameof(FragmentedNonSeekableStream));
                }

                if (this.offset == this.payload.Length)
                {
                    return 0;
                }

                var byteCount = Math.Min(destination.Length, Math.Min(this.fragmentSize, this.payload.Length - this.offset));

                this.payload.AsSpan(this.offset, byteCount)
                    .CopyTo(destination);

                this.offset += byteCount;

                return byteCount;
            }
        }

        private sealed class BlockingReadStream : Stream
        {
            private readonly TaskCompletionSource<object> readStarted = new(TaskCreationOptions.RunContinuationsAsynchronously);

            private bool isDisposed;

            internal Task ReadStarted => this.readStarted.Task;

            internal bool IsDisposed => this.isDisposed;

            public override bool CanRead => !this.isDisposed;

            public override bool CanSeek => false;

            public override bool CanWrite => false;

            public override long Length =>
                throw new NotSupportedException();

            public override long Position
            {
                get => throw new NotSupportedException();
                set => throw new NotSupportedException();
            }

            public override void Flush()
            {
            }

            public override int Read(byte[] buffer, int offset, int count) =>
                throw new NotSupportedException();

            public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
            {
                if (this.isDisposed)
                {
                    throw new ObjectDisposedException(nameof(BlockingReadStream));
                }

                this.readStarted.TrySetResult(null);

                await Task.Delay(Timeout.Infinite, cancellationToken);

                return 0;
            }

            public override long Seek(long offset, SeekOrigin origin) =>
                throw new NotSupportedException();

            public override void SetLength(long value) => throw new NotSupportedException();

            public override void Write(byte[] buffer, int offset, int count) =>
                throw new NotSupportedException();

            protected override void Dispose(bool disposing)
            {
                this.isDisposed = true;
                base.Dispose(disposing);
            }
        }
    }
}
