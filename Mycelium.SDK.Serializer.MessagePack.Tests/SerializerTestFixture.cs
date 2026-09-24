// ------------------------------------------------------------------------------------------------
//  <copyright file="SerializerTestFixture.cs" company="Starion Group S.A.">
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
    using System.Collections.Generic;

    using global::MessagePack;

    using Mycelium.SDK.DTO;
    using Mycelium.SDK.Serializer.MessagePack.Helpers;

    [TestFixture]
    public class SerializerTestFixture
    {
        private static readonly MessagePackSerializerOptions SerializerOptions = MessagePackSerializerOptions.Standard.WithResolver(DataFormatterResolver.Instance);

        [Test]
        public void Verify_that_exact_DTO_type_resolution_returns_the_same_cached_formatter()
        {
            var first = DataFormatterResolver.Instance.GetFormatter<Comment>();
            var second = DataFormatterResolver.Instance.GetFormatter<Comment>();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(first, Is.TypeOf<CommentMessagePackFormatter>());
                Assert.That(second, Is.SameAs(first));
            }
        }

        [Test]
        public void Verify_that_unsupported_types_are_rejected_without_interface_fallback()
        {
            Assert.Throws<NotSupportedException>(() => DataFormatterResolver.Instance.GetFormatter<IComment>());
            Assert.Throws<NotSupportedException>(() => DataFormatterResolver.Instance.GetFormatter<object>());
        }

        [Test]
        public void Verify_that_FunctionalProjectPolicy_uses_the_approved_positional_native_mapping()
        {
            var dto = CreateFunctionalProjectPolicy();

            var payload = MessagePackSerializer.Serialize(dto, SerializerOptions);

            var reader = new MessagePackReader(payload);

            Assert.That(reader.ReadArrayHeader(), Is.EqualTo(8));
            AssertGuid(ref reader, dto.Id);
            Assert.That(reader.ReadBoolean(), Is.EqualTo(dto.AllowAutoNamespaceImport));
            Assert.That(reader.ReadBoolean(), Is.EqualTo(dto.AllowAutoPublishMode));
            Assert.That(reader.ReadBoolean(), Is.EqualTo(dto.AllowVersionBranching));
            AssertGuid(ref reader, dto.CreatedBy);
            Assert.That(reader.ReadDateTime(), Is.EqualTo(dto.CreatedOn));
            AssertGuid(ref reader, dto.UpdatedBy);
            Assert.That(reader.ReadDateTime(), Is.EqualTo(dto.UpdatedOn));
            Assert.That(reader.End, Is.True);
        }

        [Test]
        public void Verify_that_Comment_uses_lowercase_enumeration_nil_and_collection_mappings()
        {
            var dto = CreateComment();
            dto.Quotes = null;
            dto.Replies =
            [
                Guid.Parse("00000000-0000-0000-0000-000000000002"),
                Guid.Parse("00000000-0000-0000-0000-000000000001")
            ];

            var payload = MessagePackSerializer.Serialize(dto, SerializerOptions);

            var reader = new MessagePackReader(payload);

            Assert.That(reader.ReadArrayHeader(), Is.EqualTo(11));
            AssertGuid(ref reader, dto.Id);
            AssertGuid(ref reader, dto.Author);
            Assert.That(reader.ReadString(), Is.EqualTo("open"));
            Assert.That(reader.ReadString(), Is.EqualTo(dto.Content));
            AssertGuid(ref reader, dto.CreatedBy);
            Assert.That(reader.ReadDateTime(), Is.EqualTo(dto.CreatedOn));
            Assert.That(reader.TryReadNil(), Is.True);
            Assert.That(reader.ReadArrayHeader(), Is.EqualTo(2));
            AssertGuid(ref reader, dto.Replies[0]);
            AssertGuid(ref reader, dto.Replies[1]);
            AssertGuid(ref reader, dto.TargetElementId);
            AssertGuid(ref reader, dto.UpdatedBy);
            Assert.That(reader.ReadDateTime(), Is.EqualTo(dto.UpdatedOn));
            Assert.That(reader.End, Is.True);
        }

        [Test]
        public void Verify_that_BranchProtectionRule_uses_integer_boolean_and_enumeration_collection_mappings()
        {
            var dto = CreateBranchProtectionRule();
            dto.DefaultReviewers.Insert(0, Guid.Parse("10000000-0000-0000-0000-000000000006"));
            var expectedReviewers = dto.DefaultReviewers.ToArray();

            var payload = MessagePackSerializer.Serialize(dto, SerializerOptions);

            Assert.That(dto.DefaultReviewers, Is.EqualTo(expectedReviewers));

            var reader = new MessagePackReader(payload);

            Assert.That(reader.ReadArrayHeader(), Is.EqualTo(11));
            AssertGuid(ref reader, dto.Id);
            AssertGuid(ref reader, dto.CreatedBy);
            Assert.That(reader.ReadDateTime(), Is.EqualTo(dto.CreatedOn));
            Assert.That(reader.ReadArrayHeader(), Is.EqualTo(dto.DefaultReviewers.Count));
            AssertGuid(ref reader, expectedReviewers[0]);
            AssertGuid(ref reader, expectedReviewers[1]);
            AssertGuid(ref reader, dto.EngineeringBranchId);
            Assert.That(reader.ReadArrayHeader(), Is.EqualTo(dto.MergeAllowedFor.Count));
            Assert.That(reader.ReadString(), Is.EqualTo("administrator"));
            Assert.That(reader.ReadString(), Is.EqualTo("viewer"));
            Assert.That(reader.ReadInt32(), Is.EqualTo(dto.MinimumRequiredApproval));
            Assert.That(reader.ReadString(), Is.EqualTo(dto.Name));
            Assert.That(reader.ReadBoolean(), Is.EqualTo(dto.ReviewRequired));
            AssertGuid(ref reader, dto.UpdatedBy);
            Assert.That(reader.ReadDateTime(), Is.EqualTo(dto.UpdatedOn));
            Assert.That(reader.End, Is.True);
        }

        [Test]
        public void Verify_that_FunctionalProject_dictionary_uses_its_approved_map_position()
        {
            var dto = CreateFunctionalProject();

            var payload = MessagePackSerializer.Serialize(dto, SerializerOptions);

            var reader = new MessagePackReader(payload);

            Assert.That(reader.ReadArrayHeader(), Is.EqualTo(18));

            for (var index = 0; index < 14; index++)
            {
                reader.Skip();
            }

            var entryCount = reader.ReadMapHeader();
            var entries = new Dictionary<string, string>(StringComparer.Ordinal);

            for (var index = 0; index < entryCount; index++)
            {
                entries.Add(reader.ReadString()!, reader.ReadString()!);
            }

            using (Assert.EnterMultipleScope())
            {
                Assert.That(entryCount, Is.EqualTo(dto.SharedPreferences.Count));
                Assert.That(entries, Is.EquivalentTo(dto.SharedPreferences));
            }
        }

        [Test]
        public void Verify_that_Uri_uses_its_original_string_representation()
        {
            var expected = new Uri("../models/item?version=1", UriKind.Relative);

            var payload = MessagePackFormatterProbe.SerializeUri(expected);

            var reader = new MessagePackReader(payload);

            Assert.That(reader.ReadString(), Is.EqualTo(expected.OriginalString));
            Assert.That(reader.End, Is.True);

            var actual = MessagePackFormatterProbe.DeserializeUri(payload);

            Assert.That(actual.OriginalString, Is.EqualTo(expected.OriginalString));
        }

        [Test]
        public void Verify_that_undefined_enumeration_values_are_rejected_during_serialization()
        {
            var dto = CreateComment();
            dto.CommentStatus = (CommentStatus)int.MaxValue;

            Assert.Throws<MessagePackSerializationException>(() => MessagePackSerializer.Serialize(dto, SerializerOptions));
        }

        [Test]
        public void Verify_that_non_lowercase_enumeration_values_are_rejected_during_deserialization()
        {
            Assert.That(MessagePackSerializer.Deserialize<Comment>(CreateCommentStatusPayload("open"), SerializerOptions), Is.Not.Null);

            var payload = CreateCommentStatusPayload("OPEN");

            var exception = Assert.Throws<MessagePackSerializationException>(() => MessagePackSerializer.Deserialize<Comment>(payload, SerializerOptions));

            Assert.That(exception!.ToString(), Does.Contain("Value 'OPEN' is not valid for CommentStatus."));
        }

        [Test]
        public void Verify_that_non_bin16_Guid_values_are_rejected()
        {
            Assert.That(MessagePackSerializer.Deserialize<FunctionalProjectPolicy>(CreateFunctionalProjectPolicyPayload(16), SerializerOptions), Is.Not.Null);

            var payload = CreateFunctionalProjectPolicyPayload(15);

            var exception = Assert.Throws<MessagePackSerializationException>(() => MessagePackSerializer.Deserialize<FunctionalProjectPolicy>(payload, SerializerOptions));

            Assert.That(exception!.ToString(), Does.Contain("Expected Guid as 16 bytes, but found 15 bytes."));
        }

        [Test]
        public void Verify_that_null_collections_are_rejected_during_serialization()
        {
            var dto = CreateBranchProtectionRule();
            dto.DefaultReviewers = null!;

            Assert.Throws<MessagePackSerializationException>(() => MessagePackSerializer.Serialize(dto, SerializerOptions));
        }

        [Test]
        public void Verify_that_nil_collections_are_rejected_during_deserialization()
        {
            Assert.That(MessagePackSerializer.Deserialize<BranchProtectionRule>(CreateBranchProtectionRulePayload(nilDefaultReviewers: false), SerializerOptions), Is.Not.Null);

            var payload = CreateBranchProtectionRulePayload(nilDefaultReviewers: true);

            var exception = Assert.Throws<MessagePackSerializationException>(() => MessagePackSerializer.Deserialize<BranchProtectionRule>(payload, SerializerOptions));

            Assert.That(exception!.ToString(), Does.Contain("Collection property 'DefaultReviewers' may not be nil."));
        }

        [Test]
        public void Verify_that_duplicate_dictionary_keys_are_rejected()
        {
            var payload = CreateDuplicateDictionaryPayload();

            Assert.Throws<MessagePackSerializationException>(() => MessagePackFormatterProbe.DeserializeStringDictionary(payload));
        }

        [TestCase(10)]
        [TestCase(12)]
        public void Verify_that_incorrect_DTO_field_counts_are_rejected(int fieldCount)
        {
            var payload = CreateIncorrectFieldCountPayload(fieldCount);

            var exception = Assert.Throws<MessagePackSerializationException>(() => MessagePackSerializer.Deserialize<Comment>(payload, SerializerOptions));

            Assert.That(exception!.ToString(), Does.Contain($"Comment contains {fieldCount} fields; exactly 11 fields are required."));
        }

        [Test]
        public void Verify_that_Comment_semantically_round_trips()
        {
            var expected = CreateComment();

            var payload = MessagePackSerializer.Serialize(expected, SerializerOptions);
            var actual = MessagePackSerializer.Deserialize<Comment>(payload, SerializerOptions);

            Assert.That(actual, Is.Not.Null);

            if (actual == null)
            {
                return;
            }

            using (Assert.EnterMultipleScope())
            {
                Assert.That(actual.Id, Is.EqualTo(expected.Id));
                Assert.That(actual.Author, Is.EqualTo(expected.Author));
                Assert.That(actual.CommentStatus, Is.EqualTo(expected.CommentStatus));
                Assert.That(actual.Content, Is.EqualTo(expected.Content));
                Assert.That(actual.CreatedBy, Is.EqualTo(expected.CreatedBy));
                Assert.That(actual.CreatedOn, Is.EqualTo(expected.CreatedOn));
                Assert.That(actual.Quotes, Is.EqualTo(expected.Quotes));
                Assert.That(actual.Replies, Is.EqualTo(expected.Replies));
                Assert.That(actual.TargetElementId, Is.EqualTo(expected.TargetElementId));
                Assert.That(actual.UpdatedBy, Is.EqualTo(expected.UpdatedBy));
                Assert.That(actual.UpdatedOn, Is.EqualTo(expected.UpdatedOn));
            }
        }

        [Test]
        public void Verify_that_nullable_reference_round_trips_as_nil()
        {
            var expected = CreateComment();
            expected.Quotes = null;

            var payload = MessagePackSerializer.Serialize(expected, SerializerOptions);
            var actual = MessagePackSerializer.Deserialize<Comment>(payload, SerializerOptions);

            Assert.That(actual, Is.Not.Null);
            Assert.That(actual?.Quotes, Is.Null);
        }

        [Test]
        public void Verify_that_FunctionalProject_semantically_round_trips()
        {
            var expected = CreateFunctionalProject();

            var payload = MessagePackSerializer.Serialize(expected, SerializerOptions);
            var actual = MessagePackSerializer.Deserialize<FunctionalProject>(payload, SerializerOptions);

            Assert.That(actual, Is.Not.Null);

            if (actual == null)
            {
                return;
            }

            using (Assert.EnterMultipleScope())
            {
                Assert.That(actual.Id, Is.EqualTo(expected.Id));
                Assert.That(actual.BelongsTo, Is.EqualTo(expected.BelongsTo));
                Assert.That(actual.BranchRules, Is.EqualTo(expected.BranchRules));
                Assert.That(actual.CreatedBy, Is.EqualTo(expected.CreatedBy));
                Assert.That(actual.CreatedOn, Is.EqualTo(expected.CreatedOn));
                Assert.That(actual.CurrentMode, Is.EqualTo(expected.CurrentMode));
                Assert.That(actual.Defines, Is.EqualTo(expected.Defines));
                Assert.That(actual.Description, Is.EqualTo(expected.Description));
                Assert.That(actual.EngineeringProjectId, Is.EqualTo(expected.EngineeringProjectId));
                Assert.That(actual.Involves, Is.EqualTo(expected.Involves));
                Assert.That(actual.Lifecycle, Is.EqualTo(expected.Lifecycle));
                Assert.That(actual.Name, Is.EqualTo(expected.Name));
                Assert.That(actual.Policy, Is.EqualTo(expected.Policy));
                Assert.That(actual.Reviews, Is.EqualTo(expected.Reviews));
                Assert.That(actual.SharedPreferences, Is.EquivalentTo(expected.SharedPreferences));
                Assert.That(actual.UpdatedBy, Is.EqualTo(expected.UpdatedBy));
                Assert.That(actual.UpdatedOn, Is.EqualTo(expected.UpdatedOn));
                Assert.That(actual.Visibility, Is.EqualTo(expected.Visibility));
            }
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

        private static BranchProtectionRule CreateBranchProtectionRule()
        {
            return new BranchProtectionRule
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                CreatedBy = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                CreatedOn = new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc),
                DefaultReviewers = [Guid.Parse("10000000-0000-0000-0000-000000000003")],
                EngineeringBranchId = Guid.Parse("10000000-0000-0000-0000-000000000004"),
                MergeAllowedFor = [ProjectMemberRole.Administrator, ProjectMemberRole.Viewer],
                MinimumRequiredApproval = 2,
                Name = "Protected main",
                ReviewRequired = true,
                UpdatedBy = Guid.Parse("10000000-0000-0000-0000-000000000005"),
                UpdatedOn = new DateTime(2026, 2, 3, 4, 5, 6, DateTimeKind.Utc)
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

        private static FunctionalProject CreateFunctionalProject()
        {
            return new FunctionalProject
            {
                Id = Guid.Parse("30000000-0000-0000-0000-000000000001"),
                BelongsTo = Guid.Parse("30000000-0000-0000-0000-000000000002"),
                BranchRules = [Guid.Parse("30000000-0000-0000-0000-000000000003")],
                CreatedBy = Guid.Parse("30000000-0000-0000-0000-000000000004"),
                CreatedOn = new DateTime(2026, 5, 6, 7, 8, 9, DateTimeKind.Utc),
                CurrentMode = ProjectMode.Concurrent,
                Defines = [Guid.Parse("30000000-0000-0000-0000-000000000005")],
                Description = "Functional project",
                EngineeringProjectId = Guid.Parse("30000000-0000-0000-0000-000000000006"),
                Involves = [Guid.Parse("30000000-0000-0000-0000-000000000007")],
                Lifecycle = ProjectLifecycleKind.Open,
                Name = "Project Alpha",
                Policy = Guid.Parse("30000000-0000-0000-0000-000000000008"),
                Reviews = [Guid.Parse("30000000-0000-0000-0000-000000000009")],
                SharedPreferences = new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["z-last"] = "last value",
                    ["theme"] = "dark",
                    ["Theme"] = "light"
                },
                UpdatedBy = Guid.Parse("30000000-0000-0000-0000-000000000010"),
                UpdatedOn = new DateTime(2026, 6, 7, 8, 9, 10, DateTimeKind.Utc),
                Visibility = ProjectVisibility.Organization
            };
        }

        private static FunctionalProjectPolicy CreateFunctionalProjectPolicy()
        {
            return new FunctionalProjectPolicy
            {
                Id = Guid.Parse("40000000-0000-0000-0000-000000000001"),
                AllowAutoNamespaceImport = true,
                AllowAutoPublishMode = false,
                AllowVersionBranching = true,
                CreatedBy = Guid.Parse("40000000-0000-0000-0000-000000000002"),
                CreatedOn = new DateTime(2026, 7, 8, 9, 10, 11, DateTimeKind.Utc),
                UpdatedBy = Guid.Parse("40000000-0000-0000-0000-000000000003"),
                UpdatedOn = new DateTime(2026, 8, 9, 10, 11, 12, DateTimeKind.Utc)
            };
        }

        private static byte[] CreateCommentStatusPayload(string commentStatus)
        {
            var dto = CreateComment();
            var buffer = new ArrayBufferWriter<byte>();
            var writer = new MessagePackWriter(buffer);

            writer.WriteArrayHeader(11);
            WriteGuid(ref writer, dto.Id);
            WriteGuid(ref writer, dto.Author);
            writer.Write(commentStatus);
            writer.Write(dto.Content);
            WriteGuid(ref writer, dto.CreatedBy);
            writer.Write(dto.CreatedOn);
            WriteGuid(ref writer, dto.Quotes!.Value);
            writer.WriteArrayHeader(dto.Replies.Count);

            foreach (var reply in dto.Replies)
            {
                WriteGuid(ref writer, reply);
            }

            WriteGuid(ref writer, dto.TargetElementId);
            WriteGuid(ref writer, dto.UpdatedBy);
            writer.Write(dto.UpdatedOn);
            writer.Flush();

            return buffer.WrittenMemory.ToArray();
        }

        private static byte[] CreateFunctionalProjectPolicyPayload(int idByteCount)
        {
            var dto = CreateFunctionalProjectPolicy();
            var buffer = new ArrayBufferWriter<byte>();
            var writer = new MessagePackWriter(buffer);

            writer.WriteArrayHeader(8);
            writer.WriteBinHeader(idByteCount);
            writer.WriteRaw(dto.Id.ToByteArray().AsSpan(0, idByteCount));
            writer.Write(dto.AllowAutoNamespaceImport);
            writer.Write(dto.AllowAutoPublishMode);
            writer.Write(dto.AllowVersionBranching);
            WriteGuid(ref writer, dto.CreatedBy);
            writer.Write(dto.CreatedOn);
            WriteGuid(ref writer, dto.UpdatedBy);
            writer.Write(dto.UpdatedOn);
            writer.Flush();

            return buffer.WrittenMemory.ToArray();
        }

        private static byte[] CreateBranchProtectionRulePayload(bool nilDefaultReviewers)
        {
            var dto = CreateBranchProtectionRule();
            var buffer = new ArrayBufferWriter<byte>();
            var writer = new MessagePackWriter(buffer);

            writer.WriteArrayHeader(11);
            WriteGuid(ref writer, dto.Id);
            WriteGuid(ref writer, dto.CreatedBy);
            writer.Write(dto.CreatedOn);

            if (nilDefaultReviewers)
            {
                writer.WriteNil();
            }
            else
            {
                writer.WriteArrayHeader(dto.DefaultReviewers.Count);

                foreach (var reviewer in dto.DefaultReviewers)
                {
                    WriteGuid(ref writer, reviewer);
                }
            }

            WriteGuid(ref writer, dto.EngineeringBranchId);
            writer.WriteArrayHeader(dto.MergeAllowedFor.Count);
            writer.Write("administrator");
            writer.Write("viewer");
            writer.Write(dto.MinimumRequiredApproval);
            writer.Write(dto.Name);
            writer.Write(dto.ReviewRequired);
            WriteGuid(ref writer, dto.UpdatedBy);
            writer.Write(dto.UpdatedOn);
            writer.Flush();

            return buffer.WrittenMemory.ToArray();
        }

        private static byte[] CreateDuplicateDictionaryPayload()
        {
            var buffer = new ArrayBufferWriter<byte>();
            var writer = new MessagePackWriter(buffer);

            writer.WriteMapHeader(2);
            writer.Write("duplicate");
            writer.Write("first");
            writer.Write("duplicate");
            writer.Write("second");
            writer.Flush();

            return buffer.WrittenMemory.ToArray();
        }

        private static byte[] CreateIncorrectFieldCountPayload(int fieldCount)
        {
            var buffer = new ArrayBufferWriter<byte>();
            var writer = new MessagePackWriter(buffer);

            writer.WriteArrayHeader(fieldCount);

            for (var index = 0; index < fieldCount; index++)
            {
                writer.WriteNil();
            }

            writer.Flush();

            return buffer.WrittenMemory.ToArray();
        }

        private static void WriteGuid(ref MessagePackWriter writer, Guid value)
        {
            var bytes = value.ToByteArray();

            writer.WriteBinHeader(bytes.Length);
            writer.WriteRaw(bytes);
        }

        private sealed class MessagePackFormatterProbe : MessagePackFormatterBase
        {
            public static byte[] SerializeUri(Uri value)
            {
                var buffer = new ArrayBufferWriter<byte>();
                var writer = new MessagePackWriter(buffer);

                WriteUri(ref writer, value, nullable: false, "Uri");
                writer.Flush();

                return buffer.WrittenMemory.ToArray();
            }

            public static Uri DeserializeUri(byte[] payload)
            {
                var reader = new MessagePackReader(payload);

                return ReadUri(ref reader, nullable: false, "Uri");
            }

            public static Dictionary<string, string> DeserializeStringDictionary(byte[] payload)
            {
                var reader = new MessagePackReader(payload);

                return ReadStringDictionary(ref reader, "Dictionary");
            }
        }
    }
}
