// ------------------------------------------------------------------------------------------------
//  <copyright file="OrganizationPolicyMessagePackFormatterTestFixture.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.Serializer.MessagePack.Tests.MessagePackFormatter
{
    using System;
    using System.Buffers;

    using global::MessagePack;

    using Mycelium.SDK.DTO;

    [TestFixture]
    public class OrganizationPolicyMessagePackFormatterTestFixture
    {
        [Test]
        public void Verify_that_formatter_serializes_the_approved_positional_mapping()
        {
            var dto = CreateOrganizationPolicy();
            var formatter = new OrganizationPolicyMessagePackFormatter();
            var buffer = new ArrayBufferWriter<byte>();
            var writer = new MessagePackWriter(buffer);

            formatter.Serialize(ref writer, dto, MessagePackSerializerOptions.Standard);
            writer.Flush();

            var reader = new MessagePackReader(buffer.WrittenMemory);

            Assert.That(reader.ReadArrayHeader(), Is.EqualTo(8));
            AssertGuid(ref reader, dto.Id);
            Assert.That(reader.ReadBoolean(), Is.EqualTo(dto.AllowProjectCreation));
            AssertGuid(ref reader, dto.CreatedBy);
            Assert.That(reader.ReadDateTime(), Is.EqualTo(dto.CreatedOn));
            Assert.That(reader.ReadString(), Is.EqualTo("review"));
            Assert.That(reader.ReadBoolean(), Is.EqualTo(dto.GrantReadOnlyViewForAudit));
            AssertGuid(ref reader, dto.UpdatedBy);
            Assert.That(reader.ReadDateTime(), Is.EqualTo(dto.UpdatedOn));
            Assert.That(reader.End, Is.True);
        }

        [Test]
        public void Verify_that_formatter_deserializes_the_approved_positional_mapping()
        {
            var expected = CreateOrganizationPolicy();
            var payload = CreatePayload(expected);
            var reader = new MessagePackReader(payload);
            var formatter = new OrganizationPolicyMessagePackFormatter();

            var actual = formatter.Deserialize(ref reader, MessagePackSerializerOptions.Standard);

            Assert.That(actual, Is.Not.Null);

            if (actual == null)
            {
                return;
            }

            using (Assert.EnterMultipleScope())
            {
                Assert.That(actual.Id, Is.EqualTo(expected.Id));
                Assert.That(actual.AllowProjectCreation, Is.EqualTo(expected.AllowProjectCreation));
                Assert.That(actual.CreatedBy, Is.EqualTo(expected.CreatedBy));
                Assert.That(actual.CreatedOn, Is.EqualTo(expected.CreatedOn));
                Assert.That(actual.DefaultProjectLifecycleOnCreate, Is.EqualTo(expected.DefaultProjectLifecycleOnCreate));
                Assert.That(actual.GrantReadOnlyViewForAudit, Is.EqualTo(expected.GrantReadOnlyViewForAudit));
                Assert.That(actual.UpdatedBy, Is.EqualTo(expected.UpdatedBy));
                Assert.That(actual.UpdatedOn, Is.EqualTo(expected.UpdatedOn));
                Assert.That(reader.End, Is.True);
            }
        }

        private static OrganizationPolicy CreateOrganizationPolicy()
        {
            return new OrganizationPolicy
            {
                Id = Guid.Parse("50000000-0000-0000-0000-000000000001"),
                AllowProjectCreation = true,
                CreatedBy = Guid.Parse("50000000-0000-0000-0000-000000000002"),
                CreatedOn = new DateTime(2026, 9, 10, 11, 12, 13, DateTimeKind.Utc),
                DefaultProjectLifecycleOnCreate = ProjectLifecycleKind.Review,
                GrantReadOnlyViewForAudit = false,
                UpdatedBy = Guid.Parse("50000000-0000-0000-0000-000000000003"),
                UpdatedOn = new DateTime(2026, 10, 11, 12, 13, 14, DateTimeKind.Utc)
            };
        }

        private static byte[] CreatePayload(OrganizationPolicy dto)
        {
            var buffer = new ArrayBufferWriter<byte>();
            var writer = new MessagePackWriter(buffer);

            writer.WriteArrayHeader(8);
            WriteGuid(ref writer, dto.Id);
            writer.Write(dto.AllowProjectCreation);
            WriteGuid(ref writer, dto.CreatedBy);
            writer.Write(dto.CreatedOn);
            writer.Write("review");
            writer.Write(dto.GrantReadOnlyViewForAudit);
            WriteGuid(ref writer, dto.UpdatedBy);
            writer.Write(dto.UpdatedOn);
            writer.Flush();

            return buffer.WrittenMemory.ToArray();
        }

        private static void AssertGuid(ref MessagePackReader reader, Guid expected)
        {
            var sequence = reader.ReadBytes();

            Assert.That(sequence.HasValue, Is.True, "The Guid value was encoded as nil.");

            if (!sequence.HasValue)
            {
                return;
            }

            var bytes = sequence.Value.ToArray();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(bytes, Has.Length.EqualTo(16));
                Assert.That(new Guid(bytes), Is.EqualTo(expected));
            }
        }

        private static void WriteGuid(ref MessagePackWriter writer, Guid value)
        {
            var bytes = value.ToByteArray();

            writer.WriteBinHeader(bytes.Length);
            writer.WriteRaw(bytes);
        }
    }
}
