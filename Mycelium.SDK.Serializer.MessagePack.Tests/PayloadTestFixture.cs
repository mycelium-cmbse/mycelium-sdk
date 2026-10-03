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
    using System.Globalization;
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

        private Serializer serializer;

        private DeSerializer deSerializer;

        [SetUp]
        public void SetUp()
        {
            this.serializer = new Serializer();
            this.deSerializer = new DeSerializer();
        }

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

                foreach (var group in QueryGroupProperties())
                {
                    var items = group.GetValue(payload);

                    Assert.That(items, Is.InstanceOf<ICollection>(), group.Name);
                    Assert.That(((ICollection)items).Count, Is.Zero, group.Name);
                }
            }
        }

        [TestCase(DateTimeKind.Utc)]
        [TestCase(DateTimeKind.Local)]
        [TestCase(DateTimeKind.Unspecified)]
        public void Verify_that_envelope_writes_created_then_every_ordinal_group(DateTimeKind kind)
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

            payload.Created = new DateTime(2026, 1, 2, 3, 4, 5, kind).AddTicks(1234567);

            var bytes = MessagePackSerializer.Serialize(payload, SerializerOptions);
            var reader = new MessagePackReader(bytes);
            var groups = QueryGroupProperties();

            Assert.That(reader.ReadArrayHeader(), Is.EqualTo(groups.Length + 1));
            Assert.That(reader.ReadString(), Is.EqualTo(payload.Created.ToString("o", CultureInfo.InvariantCulture)));

            foreach (var group in groups)
            {
                var expectedCount = ((ICollection)group.GetValue(payload)).Count;
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
            var secondPolicy = CreateFunctionalProjectPolicy();
            secondPolicy.Id = Guid.Parse("20000000-0000-0000-0000-000000000001");

            var payload = PayloadFactory.ToPayload(new IThing[] { organizationPolicy, functionalProjectPolicy, secondPolicy });
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
                Assert.That(items, Has.Count.EqualTo(3));
                Assert.That(items[0], Is.TypeOf<FunctionalProjectPolicy>());
                Assert.That(items[1], Is.TypeOf<FunctionalProjectPolicy>());
                Assert.That(items[2], Is.TypeOf<OrganizationPolicy>());
            }

            var actualFunctionalProjectPolicy = (FunctionalProjectPolicy)items[0];
            var actualSecondPolicy = (FunctionalProjectPolicy)items[1];
            var actualOrganizationPolicy = (OrganizationPolicy)items[2];

            using (Assert.EnterMultipleScope())
            {
                Assert.That(actualFunctionalProjectPolicy.Id, Is.EqualTo(functionalProjectPolicy.Id));
                Assert.That(actualFunctionalProjectPolicy.AllowAutoNamespaceImport, Is.EqualTo(functionalProjectPolicy.AllowAutoNamespaceImport));
                Assert.That(actualFunctionalProjectPolicy.AllowAutoPublishMode, Is.EqualTo(functionalProjectPolicy.AllowAutoPublishMode));
                Assert.That(actualFunctionalProjectPolicy.AllowVersionBranching, Is.EqualTo(functionalProjectPolicy.AllowVersionBranching));
                Assert.That(actualFunctionalProjectPolicy.CreatedBy, Is.EqualTo(functionalProjectPolicy.CreatedBy));
                Assert.That(actualFunctionalProjectPolicy.CreatedOn, Is.EqualTo(functionalProjectPolicy.CreatedOn));
                Assert.That(actualFunctionalProjectPolicy.CreatedOn.Kind, Is.EqualTo(functionalProjectPolicy.CreatedOn.Kind));
                Assert.That(actualFunctionalProjectPolicy.UpdatedBy, Is.EqualTo(functionalProjectPolicy.UpdatedBy));
                Assert.That(actualFunctionalProjectPolicy.UpdatedOn, Is.EqualTo(functionalProjectPolicy.UpdatedOn));
                Assert.That(actualSecondPolicy.Id, Is.EqualTo(secondPolicy.Id));
                Assert.That(actualOrganizationPolicy.Id, Is.EqualTo(organizationPolicy.Id));
                Assert.That(actualOrganizationPolicy.AllowProjectCreation, Is.EqualTo(organizationPolicy.AllowProjectCreation));
                Assert.That(actualOrganizationPolicy.CreatedBy, Is.EqualTo(organizationPolicy.CreatedBy));
                Assert.That(actualOrganizationPolicy.CreatedOn, Is.EqualTo(organizationPolicy.CreatedOn));
                Assert.That(actualOrganizationPolicy.DefaultProjectLifecycleOnCreate, Is.EqualTo(organizationPolicy.DefaultProjectLifecycleOnCreate));
                Assert.That(actualOrganizationPolicy.GrantReadOnlyViewForAudit, Is.EqualTo(organizationPolicy.GrantReadOnlyViewForAudit));
                Assert.That(actualOrganizationPolicy.UpdatedBy, Is.EqualTo(organizationPolicy.UpdatedBy));
                Assert.That(actualOrganizationPolicy.UpdatedOn, Is.EqualTo(organizationPolicy.UpdatedOn));
            }
        }

        [TestCase(DateTimeKind.Utc)]
        [TestCase(DateTimeKind.Local)]
        [TestCase(DateTimeKind.Unspecified)]
        public void Verify_that_empty_payload_round_trips_created_value_and_kind(DateTimeKind kind)
        {
            var payload = PayloadFactory.ToPayload(Array.Empty<IThing>());
            payload.Created = new DateTime(2026, 1, 2, 3, 4, 5, kind).AddTicks(1234567);

            var bytes = MessagePackSerializer.Serialize(payload, SerializerOptions);
            var actual = MessagePackSerializer.Deserialize<Payload>(bytes, SerializerOptions);

            Assert.That(actual, Is.Not.Null);

            if (actual == null)
            {
                return;
            }

            using (Assert.EnterMultipleScope())
            {
                Assert.That(actual.Created, Is.EqualTo(payload.Created));
                Assert.That(actual.Created.Kind, Is.EqualTo(kind));
                Assert.That(PayloadFactory.ToDataItems(actual), Is.Empty);

                if (kind == DateTimeKind.Local)
                {
                    Assert.That(actual.Created.ToUniversalTime(), Is.EqualTo(payload.Created.ToUniversalTime()));
                }
            }
        }

        [Test]
        public void Verify_that_created_with_an_explicit_offset_preserves_the_instant_in_local_time()
        {
            const string created = "2026-01-02T03:04:05.1234567+05:30";

            var expected = DateTimeOffset.ParseExact(created, "o", CultureInfo.InvariantCulture, DateTimeStyles.None);
            var actual = MessagePackSerializer.Deserialize<Payload>(CreateEmptyEnvelope(created), SerializerOptions);

            Assert.That(actual, Is.Not.Null);

            if (actual == null)
            {
                return;
            }

            using (Assert.EnterMultipleScope())
            {
                Assert.That(actual.Created.Kind, Is.EqualTo(DateTimeKind.Local));
                Assert.That(actual.Created, Is.EqualTo(expected.LocalDateTime));
                Assert.That(actual.Created.ToUniversalTime(), Is.EqualTo(expected.UtcDateTime));
            }
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("invalid")]
        [TestCase("2026-01-02T03:04:05Z")]
        [TestCase("2026-02-30T03:04:05.1234567Z")]
        public void Verify_that_created_rejects_nil_and_non_round_trip_strings(string created)
        {
            var bytes = CreateEmptyEnvelope(created);

            Assert.That(() => DeserializeDirect(bytes), Throws.TypeOf<MessagePackSerializationException>());
        }

        [Test]
        public void Verify_that_created_rejects_native_timestamps()
        {
            var groups = QueryGroupProperties();
            var buffer = new ArrayBufferWriter<byte>();
            var writer = new MessagePackWriter(buffer);

            writer.WriteArrayHeader(groups.Length + 1);
            writer.Write(new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc));

            foreach (var group in groups)
            {
                writer.WriteArrayHeader(0);
            }

            writer.Flush();

            Assert.That(() => DeserializeDirect(buffer.WrittenMemory.ToArray()), Throws.TypeOf<MessagePackSerializationException>());
        }

        [Test]
        public void Verify_that_factory_rejects_null_and_unsupported_items()
        {
            Assert.That(() => PayloadFactory.ToPayload(null), Throws.TypeOf<ArgumentNullException>());
            Assert.That(() => PayloadFactory.ToPayload(new IThing[] { null }), Throws.TypeOf<ArgumentNullException>());
            Assert.That(() => PayloadFactory.ToPayload(new IThing[] { new UnsupportedOrganizationPolicy() }), Throws.TypeOf<NotSupportedException>());
            Assert.That(() => PayloadFactory.ToDataItems(null), Throws.TypeOf<ArgumentNullException>());

            var payload = new Payload();
            payload.OrganizationPolicy.Add(null);

            Assert.That(() => PayloadFactory.ToDataItems(payload), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Verify_that_formatter_rejects_null_payload_and_group_items()
        {
            Assert.That(() => SerializeDirect(null), Throws.TypeOf<ArgumentNullException>());

            var payload = new Payload();
            payload.OrganizationPolicy.Add(null);

            Assert.That(() => SerializeDirect(payload), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Verify_that_deserializer_rejects_nil_wrong_length_and_null_items()
        {
            var nilBuffer = new ArrayBufferWriter<byte>();
            var nilWriter = new MessagePackWriter(nilBuffer);
            nilWriter.WriteNil();
            nilWriter.Flush();

            Assert.That(() => DeserializeDirect(nilBuffer.WrittenMemory.ToArray()), Throws.TypeOf<MessagePackSerializationException>());

            var groups = QueryGroupProperties();

            foreach (var fieldCount in new[] { 0, groups.Length, groups.Length + 2 })
            {
                var countBuffer = new ArrayBufferWriter<byte>();
                var countWriter = new MessagePackWriter(countBuffer);
                countWriter.WriteArrayHeader(fieldCount);

                for (var index = 0; index < fieldCount; index++)
                {
                    countWriter.WriteNil();
                }

                countWriter.Flush();

                Assert.That(() => DeserializeDirect(countBuffer.WrittenMemory.ToArray()), Throws.TypeOf<MessagePackSerializationException>());
            }

            Assert.That(groups, Is.Not.Empty);

            var itemBuffer = new ArrayBufferWriter<byte>();
            var itemWriter = new MessagePackWriter(itemBuffer);
            itemWriter.WriteArrayHeader(groups.Length + 1);
            itemWriter.Write(new DateTime(2026, 3, 4, 5, 6, 7, DateTimeKind.Utc).ToString("o", CultureInfo.InvariantCulture));
            itemWriter.WriteArrayHeader(1);
            itemWriter.WriteNil();

            for (var index = 1; index < groups.Length; index++)
            {
                itemWriter.WriteArrayHeader(0);
            }

            itemWriter.Flush();

            Assert.That(() => DeserializeDirect(itemBuffer.WrittenMemory.ToArray()), Throws.TypeOf<MessagePackSerializationException>());
        }

        [TestCase(false)]
        [TestCase(true)]
        public void Verify_that_sync_stream_facades_round_trip_mixed_and_empty_payloads(bool empty)
        {
            var source = CreateTransportItems(empty);

            using var stream = new MemoryStream();

            this.serializer.Serialize(source, stream);

            Assert.That(stream.CanWrite, Is.True);

            stream.Position = 0;

            var actual = this.deSerializer.DeSerialize(stream);

            Assert.That(stream.CanRead, Is.True);

            stream.Dispose();

            AssertTransportItems(actual, empty);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void Verify_that_buffer_writer_and_sequence_facades_round_trip_mixed_and_empty_payloads(bool empty)
        {
            var writer = new ArrayBufferWriter<byte>();

            this.serializer.SerializeToBufferWriter(CreateTransportItems(empty), writer);

            var actual = this.deSerializer.DeSerialize(new ReadOnlySequence<byte>(writer.WrittenMemory));

            AssertTransportItems(actual, empty);
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task Verify_that_async_stream_facades_round_trip_mixed_and_empty_payloads(bool empty)
        {
            using var stream = new MemoryStream();

            await this.serializer.SerializeAsync(CreateTransportItems(empty), stream, CancellationToken.None);

            Assert.That(stream.CanWrite, Is.True);

            stream.Position = 0;

            var actual = await this.deSerializer.DeSerializeAsync(stream, CancellationToken.None);

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
            nullable.Replies = null;

            var empty = CreateComment();
            empty.Id = Guid.Parse("40000000-0000-0000-0000-000000000002");
            empty.Replies.Clear();

            var source = new IThing[] { ordered, CreateOrganizationPolicy(), nullable, empty };
            var originalSource = source.ToArray();
            var writer = new ArrayBufferWriter<byte>();

            this.serializer.SerializeToBufferWriter(source, writer);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(source, Is.EqualTo(originalSource));
                Assert.That(ordered.Replies, Is.EqualTo(originalReplies));
                Assert.That(nullable.Quotes, Is.Null);
                Assert.That(nullable.Replies, Is.Null);
                Assert.That(empty.Replies, Is.Empty);
            }

            var actual = this.deSerializer.DeSerialize(new ReadOnlySequence<byte>(writer.WrittenMemory)).ToArray();

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
            var writer = new ArrayBufferWriter<byte>();

            using var stream = new MemoryStream();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(() => this.serializer.Serialize(null, stream), Throws.TypeOf<ArgumentNullException>());
                Assert.That(() => this.serializer.Serialize([], null), Throws.TypeOf<ArgumentNullException>());
                Assert.That(() => this.serializer.Serialize(new IThing[] { null }, stream), Throws.TypeOf<ArgumentNullException>());
                Assert.That(() => this.serializer.SerializeToBufferWriter(null, writer), Throws.TypeOf<ArgumentNullException>());
                Assert.That(() => this.serializer.SerializeToBufferWriter([], null), Throws.TypeOf<ArgumentNullException>());
                Assert.That(() => this.serializer.SerializeToBufferWriter(new IThing[] { null }, writer), Throws.TypeOf<ArgumentNullException>());
                Assert.That(async () => await this.serializer.SerializeAsync(null, stream, CancellationToken.None), Throws.TypeOf<ArgumentNullException>());
                Assert.That(async () => await this.serializer.SerializeAsync([], null, CancellationToken.None), Throws.TypeOf<ArgumentNullException>());
                Assert.That(async () => await this.serializer.SerializeAsync(new IThing[] { null }, stream, CancellationToken.None), Throws.TypeOf<ArgumentNullException>());
                Assert.That(() => this.deSerializer.DeSerialize((Stream)null), Throws.TypeOf<ArgumentNullException>());
                Assert.That(async () => await this.deSerializer.DeSerializeAsync(null, CancellationToken.None), Throws.TypeOf<ArgumentNullException>());

                var unsupported = new UnsupportedOrganizationPolicy();

                Assert.That(() => this.serializer.Serialize(new IThing[] { unsupported }, stream), Throws.TypeOf<NotSupportedException>());
                Assert.That(() => this.serializer.SerializeToBufferWriter(new IThing[] { unsupported }, writer), Throws.TypeOf<NotSupportedException>());
                Assert.That(async () => await this.serializer.SerializeAsync(new IThing[] { unsupported }, stream, CancellationToken.None), Throws.TypeOf<NotSupportedException>());
            }
        }

        [Test]
        public void Verify_that_async_stream_facades_observe_cancellation()
        {
            var writer = new ArrayBufferWriter<byte>();

            using var stream = new MemoryStream();
            using var cancellationSource = new CancellationTokenSource();

            cancellationSource.Cancel();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(() => this.serializer.SerializeToBufferWriter([], writer, cancellationSource.Token), Throws.TypeOf<OperationCanceledException>());
                Assert.That(async () => await this.serializer.SerializeAsync([], stream, cancellationSource.Token), Throws.TypeOf<OperationCanceledException>());
                Assert.That(async () => await this.deSerializer.DeSerializeAsync(stream, cancellationSource.Token), Throws.TypeOf<OperationCanceledException>());
            }
        }

        [Test]
        public void Verify_that_facades_reject_invalid_values_during_serialization()
        {
            var invalid = CreateOrganizationPolicy();
            invalid.DefaultProjectLifecycleOnCreate = (ProjectLifecycleKind)int.MaxValue;

            var writer = new ArrayBufferWriter<byte>();

            using var stream = new MemoryStream();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(() => this.serializer.Serialize(new IThing[] { invalid }, stream), Throws.TypeOf<MessagePackSerializationException>());
                Assert.That(() => this.serializer.SerializeToBufferWriter(new IThing[] { invalid }, writer), Throws.TypeOf<MessagePackSerializationException>());
                Assert.That(
                    async () => await this.serializer.SerializeAsync(new IThing[] { invalid }, stream, CancellationToken.None),
                    Throws.TypeOf<MessagePackSerializationException>());
            }
        }

        [Test]
        public void Verify_that_facades_reject_malformed_invalid_trailing_and_wrong_field_counts()
        {
            var valid = this.SerializeEmptyPayload();

            this.AssertAllTransportsReject(valid[..^1]);
            this.AssertAllTransportsReject([0xc0]);
            this.AssertAllTransportsReject([0x90]);
            this.AssertAllTransportsReject(CreateInvalidCreatedPayload());
            this.AssertAllTransportsReject(CreateWrongDtoFieldCountPayload());

            var trailing = new byte[valid.Length + 1];
            Array.Copy(valid, trailing, valid.Length);
            trailing[^1] = 0xc0;

            this.AssertAllTransportsReject(trailing);
        }

        private static byte[] CreateEmptyEnvelope(string created)
        {
            var groups = QueryGroupProperties();
            var buffer = new ArrayBufferWriter<byte>();
            var writer = new MessagePackWriter(buffer);

            writer.WriteArrayHeader(groups.Length + 1);
            writer.Write(created);

            foreach (var group in groups)
            {
                writer.WriteArrayHeader(0);
            }

            writer.Flush();

            return buffer.WrittenMemory.ToArray();
        }

        private static PropertyInfo[] QueryGroupProperties()
        {
            return typeof(Payload).GetProperties(BindingFlags.Instance | BindingFlags.NonPublic)
                .Where(property => property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                .OrderBy(property => property.Name, StringComparer.Ordinal)
                .ToArray();
        }

        private static byte[] SerializeDirect(Payload payload)
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

            using (Assert.EnterMultipleScope())
            {
                Assert.That(actual, Has.Length.EqualTo(2));

                if (actual.Length == 2)
                {
                    Assert.That(actual[0], Is.TypeOf<FunctionalProjectPolicy>());
                    Assert.That(actual[1], Is.TypeOf<OrganizationPolicy>());
                }
            }

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

        private byte[] SerializeEmptyPayload()
        {
            var writer = new ArrayBufferWriter<byte>();

            this.serializer.SerializeToBufferWriter(Array.Empty<IThing>(), writer);

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
            writer.Write(new DateTime(2026, 8, 9, 10, 11, 12, DateTimeKind.Utc).ToString("o", CultureInfo.InvariantCulture));
            writer.WriteArrayHeader(1);
            writer.WriteArrayHeader(0);

            for (var index = 1; index < groups.Length; index++)
            {
                writer.WriteArrayHeader(0);
            }

            writer.Flush();

            return buffer.WrittenMemory.ToArray();
        }

        private void AssertAllTransportsReject(byte[] bytes)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(() => this.deSerializer.DeSerialize(new ReadOnlySequence<byte>(bytes)), Throws.TypeOf<MessagePackSerializationException>());

                using var syncStream = new MemoryStream(bytes);

                Assert.That(() => this.deSerializer.DeSerialize(syncStream), Throws.TypeOf<MessagePackSerializationException>());

                using var asyncStream = new MemoryStream(bytes);

                Assert.That(async () => await this.deSerializer.DeSerializeAsync(asyncStream, CancellationToken.None), Throws.TypeOf<MessagePackSerializationException>());
            }
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
