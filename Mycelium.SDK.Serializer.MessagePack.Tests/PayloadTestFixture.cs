// ------------------------------------------------------------------------------------------------
//  <copyright file="PayloadTestFixture.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.Serializer.MessagePack.Tests
{
    using System;
    using System.Buffers;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using global::MessagePack;

    using Mycelium.SDK.DTO;
    using Mycelium.SDK.Serializer.MessagePack.Helpers;

    [TestFixture]
    public class PayloadTestFixture
    {
        private static readonly MessagePackSerializerOptions SerializerOptions = MessagePackSerializerOptions.Standard.WithResolver(DataFormatterResolver.Instance);

        [Test]
        public void Verify_that_resolver_selects_the_cached_payload_formatter()
        {
            var first = DataFormatterResolver.Instance.GetFormatter<Payload>();
            var second = DataFormatterResolver.Instance.GetFormatter<Payload>();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(first, Is.TypeOf<PayloadMessagePackFormatter>());
                Assert.That(second, Is.SameAs(first));
            }
        }

        [Test]
        public void Verify_that_new_payload_has_utc_created_and_empty_groups()
        {
            var before = DateTime.UtcNow;
            var payload = new Payload();
            var after = DateTime.UtcNow;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(payload.Created.Kind, Is.EqualTo(DateTimeKind.Utc));
                Assert.That(payload.Created, Is.InRange(before, after));
            }

            foreach (var group in QueryGroupProperties())
            {
                var items = group.GetValue(payload);

                Assert.That(items, Is.InstanceOf<ICollection>(), group.Name);
                Assert.That(((ICollection)items!).Count, Is.Zero, group.Name);
            }
        }

        [Test]
        public void Verify_that_envelope_writes_created_then_every_ordinal_group()
        {
            var secondPolicy = CreateFunctionalProjectPolicy();
            secondPolicy.Id = Guid.Parse("20000000-0000-0000-0000-000000000001");

            var payload = PayloadFactory.ToPayload(
                new IThing[]
                {
                    CreateOrganizationPolicy(),
                    CreateFunctionalProjectPolicy(),
                    secondPolicy
                });

            payload.Created = new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc);

            var bytes = MessagePackSerializer.Serialize(payload, SerializerOptions);
            var reader = new MessagePackReader(bytes);
            var groups = QueryGroupProperties();

            Assert.That(reader.ReadArrayHeader(), Is.EqualTo(groups.Length + 1));
            Assert.That(reader.ReadDateTime(), Is.EqualTo(payload.Created));

            foreach (var group in groups)
            {
                var expectedCount = ((ICollection)group.GetValue(payload)!).Count;
                var actualCount = reader.ReadArrayHeader();

                Assert.That(actualCount, Is.EqualTo(expectedCount), group.Name);

                for (var index = 0; index < actualCount; index++)
                {
                    reader.Skip();
                }
            }

            Assert.That(reader.End, Is.True);
        }

        [Test]
        public void Verify_that_mixed_DTO_payload_round_trips_in_group_order()
        {
            var organizationPolicy = CreateOrganizationPolicy();
            var functionalProjectPolicy = CreateFunctionalProjectPolicy();

            var payload = PayloadFactory.ToPayload(new IThing[] { organizationPolicy, functionalProjectPolicy });
            payload.Created = new DateTime(2026, 2, 3, 4, 5, 6, DateTimeKind.Utc);

            var bytes = MessagePackSerializer.Serialize(payload, SerializerOptions);
            var actual = MessagePackSerializer.Deserialize<Payload>(bytes, SerializerOptions);

            Assert.That(actual, Is.Not.Null);

            if (actual == null)
            {
                return;
            }

            var items = PayloadFactory.ToDataItems(actual);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(actual.Created, Is.EqualTo(payload.Created));
                Assert.That(actual.Created.Kind, Is.EqualTo(DateTimeKind.Utc));
                Assert.That(items, Has.Count.EqualTo(2));
                Assert.That(items[0], Is.TypeOf<FunctionalProjectPolicy>());
                Assert.That(items[1], Is.TypeOf<OrganizationPolicy>());
                Assert.That(((FunctionalProjectPolicy)items[0]).Id, Is.EqualTo(functionalProjectPolicy.Id));
                Assert.That(((OrganizationPolicy)items[1]).Id, Is.EqualTo(organizationPolicy.Id));
                Assert.That(((OrganizationPolicy)items[1]).DefaultProjectLifecycleOnCreate, Is.EqualTo(ProjectLifecycleKind.Review));
            }
        }

        [Test]
        public void Verify_that_factory_rejects_null_and_unsupported_items()
        {
            Assert.Throws<ArgumentNullException>(() => PayloadFactory.ToPayload(null!));
            Assert.Throws<ArgumentNullException>(() => PayloadFactory.ToPayload(new IThing[] { null! }));
            Assert.Throws<NotSupportedException>(() => PayloadFactory.ToPayload(new IThing[] { new UnsupportedOrganizationPolicy() }));
            Assert.Throws<ArgumentNullException>(() => PayloadFactory.ToDataItems(null!));

            var payload = new Payload();
            payload.OrganizationPolicy.Add(null!);

            Assert.Throws<ArgumentNullException>(() => PayloadFactory.ToDataItems(payload));
        }

        [Test]
        public void Verify_that_formatter_rejects_null_payload_and_group_items()
        {
            Assert.Throws<ArgumentNullException>(() => SerializeDirect(null));

            var payload = new Payload();
            payload.OrganizationPolicy.Add(null!);

            Assert.Throws<ArgumentNullException>(() => SerializeDirect(payload));
        }

        [Test]
        public void Verify_that_deserializer_rejects_nil_wrong_length_and_null_items()
        {
            var nilBuffer = new ArrayBufferWriter<byte>();
            var nilWriter = new MessagePackWriter(nilBuffer);
            nilWriter.WriteNil();
            nilWriter.Flush();

            Assert.Throws<MessagePackSerializationException>(() => DeserializeDirect(nilBuffer.WrittenMemory.ToArray()));

            var shortBuffer = new ArrayBufferWriter<byte>();
            var shortWriter = new MessagePackWriter(shortBuffer);
            shortWriter.WriteArrayHeader(0);
            shortWriter.Flush();

            Assert.Throws<MessagePackSerializationException>(() => DeserializeDirect(shortBuffer.WrittenMemory.ToArray()));

            var groups = QueryGroupProperties();

            Assert.That(groups, Is.Not.Empty);

            var itemBuffer = new ArrayBufferWriter<byte>();
            var itemWriter = new MessagePackWriter(itemBuffer);
            itemWriter.WriteArrayHeader(groups.Length + 1);
            itemWriter.Write(new DateTime(2026, 3, 4, 5, 6, 7, DateTimeKind.Utc));
            itemWriter.WriteArrayHeader(1);
            itemWriter.WriteNil();

            for (var index = 1; index < groups.Length; index++)
            {
                itemWriter.WriteArrayHeader(0);
            }

            itemWriter.Flush();

            Assert.Throws<MessagePackSerializationException>(() => DeserializeDirect(itemBuffer.WrittenMemory.ToArray()));
        }

        [TestCase(false)]
        [TestCase(true)]
        public void Verify_that_sync_stream_facades_round_trip_mixed_and_empty_payloads(bool empty)
        {
            var serializer = new Serializer();
            var deSerializer = new DeSerializer();
            var source = CreateTransportItems(empty);

            using var stream = new MemoryStream();

            serializer.Serialize(source, stream);

            Assert.That(stream.CanWrite, Is.True);

            stream.Position = 0;

            var actual = deSerializer.DeSerialize(stream);

            Assert.That(stream.CanRead, Is.True);

            stream.Dispose();

            AssertTransportItems(actual, empty);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void Verify_that_buffer_writer_and_sequence_facades_round_trip_mixed_and_empty_payloads(bool empty)
        {
            var serializer = new Serializer();
            var deSerializer = new DeSerializer();
            var writer = new ArrayBufferWriter<byte>();

            serializer.SerializeToBufferWriter(CreateTransportItems(empty), writer);

            var actual = deSerializer.DeSerialize(new ReadOnlySequence<byte>(writer.WrittenMemory));

            AssertTransportItems(actual, empty);
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task Verify_that_async_stream_facades_round_trip_mixed_and_empty_payloads(bool empty)
        {
            var serializer = new Serializer();
            var deSerializer = new DeSerializer();

            using var stream = new MemoryStream();

            await serializer.SerializeAsync(CreateTransportItems(empty), stream, CancellationToken.None);

            Assert.That(stream.CanWrite, Is.True);

            stream.Position = 0;

            var actual = await deSerializer.DeSerializeAsync(stream, CancellationToken.None);

            Assert.That(stream.CanRead, Is.True);

            stream.Dispose();

            AssertTransportItems(actual, empty);
        }

        [Test]
        public void Verify_that_facades_preserve_collection_semantics_and_source_order()
        {
            var ordered = CreateComment();
            var originalReplies = ordered.Replies.ToArray();

            var nullable = CreateComment();
            nullable.Id = Guid.Parse("40000000-0000-0000-0000-000000000001");
            nullable.Quotes = null;
            nullable.Replies = null!;

            var empty = CreateComment();
            empty.Id = Guid.Parse("40000000-0000-0000-0000-000000000002");
            empty.Replies.Clear();

            var source = new IThing[] { ordered, CreateOrganizationPolicy(), nullable, empty };
            var originalSource = source.ToArray();
            var writer = new ArrayBufferWriter<byte>();

            new Serializer().SerializeToBufferWriter(source, writer);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(source, Is.EqualTo(originalSource));
                Assert.That(ordered.Replies, Is.EqualTo(originalReplies));
                Assert.That(nullable.Quotes, Is.Null);
                Assert.That(nullable.Replies, Is.Null);
                Assert.That(empty.Replies, Is.Empty);
            }

            var actual = new DeSerializer().DeSerialize(new ReadOnlySequence<byte>(writer.WrittenMemory)).ToArray();

            Assert.That(actual, Has.Length.EqualTo(4));
            Assert.That(actual[3], Is.TypeOf<OrganizationPolicy>());

            var comments = actual.Take(3).Cast<Comment>().ToArray();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(comments.Select(comment => comment.Id), Is.EqualTo(new[] { ordered.Id, nullable.Id, empty.Id }));
                Assert.That(comments[0].Replies, Is.EqualTo(originalReplies));
                Assert.That(comments[1].Quotes, Is.Null);
                Assert.That(comments[1].Replies, Is.Null);
                Assert.That(comments[2].Replies, Is.Empty);
            }
        }

        [Test]
        public void Verify_that_facades_reject_null_arguments_items_and_unsupported_types()
        {
            var serializer = new Serializer();
            var deSerializer = new DeSerializer();
            var writer = new ArrayBufferWriter<byte>();

            using var stream = new MemoryStream();

            Assert.Throws<ArgumentNullException>(() => serializer.Serialize(null!, stream));
            Assert.Throws<ArgumentNullException>(() => serializer.Serialize([], null!));
            Assert.Throws<ArgumentNullException>(() => serializer.Serialize(new IThing[] { null! }, stream));
            Assert.Throws<ArgumentNullException>(() => serializer.SerializeToBufferWriter(null!, writer));
            Assert.Throws<ArgumentNullException>(() => serializer.SerializeToBufferWriter([], null!));
            Assert.Throws<ArgumentNullException>(() => serializer.SerializeToBufferWriter(new IThing[] { null! }, writer));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await serializer.SerializeAsync(null!, stream, CancellationToken.None));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await serializer.SerializeAsync([], null!, CancellationToken.None));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await serializer.SerializeAsync(new IThing[] { null! }, stream, CancellationToken.None));
            Assert.Throws<ArgumentNullException>(() => deSerializer.DeSerialize((Stream)null!));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await deSerializer.DeSerializeAsync(null!, CancellationToken.None));

            var unsupported = new UnsupportedOrganizationPolicy();

            Assert.Throws<NotSupportedException>(() => serializer.Serialize(new IThing[] { unsupported }, stream));
            Assert.Throws<NotSupportedException>(() => serializer.SerializeToBufferWriter(new IThing[] { unsupported }, writer));
            Assert.ThrowsAsync<NotSupportedException>(async () => await serializer.SerializeAsync(new IThing[] { unsupported }, stream, CancellationToken.None));
        }

        [Test]
        public void Verify_that_async_stream_facades_observe_cancellation()
        {
            var serializer = new Serializer();
            var deSerializer = new DeSerializer();
            var writer = new ArrayBufferWriter<byte>();

            using var stream = new MemoryStream();
            using var cancellationSource = new CancellationTokenSource();

            cancellationSource.Cancel();

            Assert.Throws<OperationCanceledException>(() => serializer.SerializeToBufferWriter([], writer, cancellationSource.Token));
            Assert.ThrowsAsync<OperationCanceledException>(async () => await serializer.SerializeAsync([], stream, cancellationSource.Token));
            Assert.ThrowsAsync<OperationCanceledException>(async () => await deSerializer.DeSerializeAsync(stream, cancellationSource.Token));
        }

        [Test]
        public void Verify_that_facades_reject_invalid_values_during_serialization()
        {
            var invalid = CreateOrganizationPolicy();
            invalid.DefaultProjectLifecycleOnCreate = (ProjectLifecycleKind)int.MaxValue;

            var serializer = new Serializer();
            var writer = new ArrayBufferWriter<byte>();

            using var stream = new MemoryStream();

            Assert.Throws<MessagePackSerializationException>(() => serializer.Serialize(new IThing[] { invalid }, stream));
            Assert.Throws<MessagePackSerializationException>(() => serializer.SerializeToBufferWriter(new IThing[] { invalid }, writer));
            Assert.ThrowsAsync<MessagePackSerializationException>(async () => await serializer.SerializeAsync(new IThing[] { invalid }, stream, CancellationToken.None));
        }

        [Test]
        public void Verify_that_facades_reject_malformed_invalid_trailing_and_wrong_field_counts()
        {
            var valid = SerializeEmptyPayload();

            AssertAllTransportsReject(valid[..^1]);
            AssertAllTransportsReject([0xc0]);
            AssertAllTransportsReject([0x90]);
            AssertAllTransportsReject(CreateInvalidCreatedPayload());
            AssertAllTransportsReject(CreateWrongDtoFieldCountPayload());

            var trailing = new byte[valid.Length + 1];
            Array.Copy(valid, trailing, valid.Length);
            trailing[^1] = 0xc0;

            AssertAllTransportsReject(trailing);
        }

        private static PropertyInfo[] QueryGroupProperties()
        {
            return typeof(Payload).GetProperties(BindingFlags.Instance | BindingFlags.NonPublic)
                .Where(property => property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                .OrderBy(property => property.Name, StringComparer.Ordinal)
                .ToArray();
        }

        private static byte[] SerializeDirect(Payload? payload)
        {
            var buffer = new ArrayBufferWriter<byte>();
            var writer = new MessagePackWriter(buffer);

            new PayloadMessagePackFormatter().Serialize(ref writer, payload, SerializerOptions);
            writer.Flush();

            return buffer.WrittenMemory.ToArray();
        }

        private static void DeserializeDirect(byte[] bytes)
        {
            var reader = new MessagePackReader(bytes);

            new PayloadMessagePackFormatter().Deserialize(ref reader, SerializerOptions);
        }

        private static IThing[] CreateTransportItems(bool empty)
        {
            if (empty)
            {
                return [];
            }

            return [CreateOrganizationPolicy(), CreateFunctionalProjectPolicy()];
        }

        private static void AssertTransportItems(IEnumerable<IThing> items, bool empty)
        {
            var actual = items.ToArray();

            if (empty)
            {
                Assert.That(actual, Is.Empty);

                return;
            }

            Assert.That(actual, Has.Length.EqualTo(2));
            Assert.That(actual[0], Is.TypeOf<FunctionalProjectPolicy>());
            Assert.That(actual[1], Is.TypeOf<OrganizationPolicy>());

            var functionalProjectPolicy = (FunctionalProjectPolicy)actual[0];
            var organizationPolicy = (OrganizationPolicy)actual[1];

            using (Assert.EnterMultipleScope())
            {
                Assert.That(functionalProjectPolicy.Id, Is.EqualTo(Guid.Parse("10000000-0000-0000-0000-000000000001")));
                Assert.That(functionalProjectPolicy.AllowAutoNamespaceImport, Is.True);
                Assert.That(functionalProjectPolicy.CreatedOn, Is.EqualTo(new DateTime(2026, 4, 5, 6, 7, 8, DateTimeKind.Utc)));
                Assert.That(organizationPolicy.Id, Is.EqualTo(Guid.Parse("30000000-0000-0000-0000-000000000001")));
                Assert.That(organizationPolicy.DefaultProjectLifecycleOnCreate, Is.EqualTo(ProjectLifecycleKind.Review));
            }
        }

        private static byte[] SerializeEmptyPayload()
        {
            var writer = new ArrayBufferWriter<byte>();

            new Serializer().SerializeToBufferWriter(Array.Empty<IThing>(), writer);

            return writer.WrittenMemory.ToArray();
        }

        private static byte[] CreateInvalidCreatedPayload()
        {
            var groups = QueryGroupProperties();
            var buffer = new ArrayBufferWriter<byte>();
            var writer = new MessagePackWriter(buffer);

            writer.WriteArrayHeader(groups.Length + 1);
            writer.Write("invalid timestamp");

            foreach (var group in groups)
            {
                writer.WriteArrayHeader(0);
            }

            writer.Flush();

            return buffer.WrittenMemory.ToArray();
        }

        private static byte[] CreateWrongDtoFieldCountPayload()
        {
            var groups = QueryGroupProperties();

            Assert.That(groups, Is.Not.Empty);

            var buffer = new ArrayBufferWriter<byte>();
            var writer = new MessagePackWriter(buffer);

            writer.WriteArrayHeader(groups.Length + 1);
            writer.Write(new DateTime(2026, 8, 9, 10, 11, 12, DateTimeKind.Utc));
            writer.WriteArrayHeader(1);
            writer.WriteArrayHeader(0);

            for (var index = 1; index < groups.Length; index++)
            {
                writer.WriteArrayHeader(0);
            }

            writer.Flush();

            return buffer.WrittenMemory.ToArray();
        }

        private static void AssertAllTransportsReject(byte[] bytes)
        {
            var deSerializer = new DeSerializer();

            Assert.Throws<MessagePackSerializationException>(() => deSerializer.DeSerialize(new ReadOnlySequence<byte>(bytes)));

            using var syncStream = new MemoryStream(bytes);

            Assert.Throws<MessagePackSerializationException>(() => deSerializer.DeSerialize(syncStream));

            using var asyncStream = new MemoryStream(bytes);

            Assert.ThrowsAsync<MessagePackSerializationException>(async () => await deSerializer.DeSerializeAsync(asyncStream, CancellationToken.None));
        }

        private static FunctionalProjectPolicy CreateFunctionalProjectPolicy()
        {
            return new FunctionalProjectPolicy
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                AllowAutoNamespaceImport = true,
                AllowAutoPublishMode = false,
                AllowVersionBranching = true,
                CreatedBy = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                CreatedOn = new DateTime(2026, 4, 5, 6, 7, 8, DateTimeKind.Utc),
                UpdatedBy = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                UpdatedOn = new DateTime(2026, 5, 6, 7, 8, 9, DateTimeKind.Utc)
            };
        }

        private static OrganizationPolicy CreateOrganizationPolicy()
        {
            return new OrganizationPolicy
            {
                Id = Guid.Parse("30000000-0000-0000-0000-000000000001"),
                AllowProjectCreation = true,
                CreatedBy = Guid.Parse("30000000-0000-0000-0000-000000000002"),
                CreatedOn = new DateTime(2026, 6, 7, 8, 9, 10, DateTimeKind.Utc),
                DefaultProjectLifecycleOnCreate = ProjectLifecycleKind.Review,
                GrantReadOnlyViewForAudit = false,
                UpdatedBy = Guid.Parse("30000000-0000-0000-0000-000000000003"),
                UpdatedOn = new DateTime(2026, 7, 8, 9, 10, 11, DateTimeKind.Utc)
            };
        }

        private static Comment CreateComment()
        {
            return new Comment
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000001"),
                Author = Guid.Parse("20000000-0000-0000-0000-000000000002"),
                CommentStatus = CommentStatus.Open,
                Content = "Review this element.",
                CreatedBy = Guid.Parse("20000000-0000-0000-0000-000000000003"),
                CreatedOn = new DateTime(2026, 3, 4, 5, 6, 7, DateTimeKind.Utc),
                Quotes = Guid.Parse("20000000-0000-0000-0000-000000000004"),
                Replies =
                [
                    Guid.Parse("20000000-0000-0000-0000-000000000006"),
                    Guid.Parse("20000000-0000-0000-0000-000000000005")
                ],
                TargetElementId = Guid.Parse("20000000-0000-0000-0000-000000000007"),
                UpdatedBy = Guid.Parse("20000000-0000-0000-0000-000000000008"),
                UpdatedOn = new DateTime(2026, 4, 5, 6, 7, 8, DateTimeKind.Utc)
            };
        }

        private sealed class UnsupportedOrganizationPolicy : OrganizationPolicy
        {
        }
    }
}
