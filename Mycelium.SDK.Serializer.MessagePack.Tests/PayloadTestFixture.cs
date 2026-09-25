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
    using System.Linq;
    using System.Reflection;

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

        private sealed class UnsupportedOrganizationPolicy : OrganizationPolicy
        {
        }
    }
}
