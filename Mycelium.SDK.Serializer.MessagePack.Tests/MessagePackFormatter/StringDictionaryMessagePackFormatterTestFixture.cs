// ------------------------------------------------------------------------------------------------
//  <copyright file="StringDictionaryMessagePackFormatterTestFixture.cs" company="Starion Group S.A.">
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
    using System.Collections.Generic;
    using System.Linq;

    using global::MessagePack;

    [TestFixture]
    public class StringDictionaryMessagePackFormatterTestFixture
    {
        [Test]
        public void Verify_that_empty_dictionary_round_trips_as_an_empty_ordinal_map()
        {
            var payload = Serialize(new Dictionary<string, string>());
            var reader = new MessagePackReader(payload);
            var actual = Deserialize(payload);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(reader.ReadMapHeader(), Is.Zero);
                Assert.That(reader.End, Is.True);
                Assert.That(actual, Is.Empty);
                Assert.That(actual.Comparer, Is.SameAs(StringComparer.Ordinal));
            }
        }

        [Test]
        public void Verify_that_entries_round_trip_without_changing_the_dictionary_or_its_order()
        {
            var expected = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["z"] = "last",
                ["a"] = "lowercase",
                ["A"] = "uppercase",
                [""] = "",
                ["é"] = "value with spaces"
            };
            var originalEntries = expected.ToArray();
            var payload = Serialize(expected);
            var reader = new MessagePackReader(payload);

            Assert.That(reader.ReadMapHeader(), Is.EqualTo(originalEntries.Length));

            foreach (var entry in originalEntries)
            {
                Assert.That(reader.ReadString(), Is.EqualTo(entry.Key));
                Assert.That(reader.ReadString(), Is.EqualTo(entry.Value));
            }

            var actual = Deserialize(payload);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(reader.End, Is.True);
                Assert.That(expected.ToArray(), Is.EqualTo(originalEntries));
                Assert.That(actual.ToArray(), Is.EqualTo(originalEntries));
                Assert.That(actual.Comparer, Is.SameAs(StringComparer.Ordinal));
                Assert.That(actual["a"], Is.EqualTo("lowercase"));
                Assert.That(actual["A"], Is.EqualTo("uppercase"));
            }
        }

        [Test]
        public void Verify_that_null_serialization_input_and_values_are_rejected()
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.Throws<MessagePackSerializationException>(() => Serialize(null));
                Assert.Throws<MessagePackSerializationException>(() => Serialize(new Dictionary<string, string> { ["key"] = null }));
            }
        }

        [TestCase("C0")]
        [TestCase("90")]
        [TestCase("01")]
        [TestCase("A0")]
        [TestCase("81C0A176")]
        [TestCase("81A16BC0")]
        [TestCase("8101A176")]
        [TestCase("81A16B01")]
        [TestCase("82A16BA161A16BA162")]
        public void Verify_that_nil_non_maps_invalid_entries_and_duplicate_keys_are_rejected(string hex)
        {
            Assert.Throws<MessagePackSerializationException>(() => Deserialize(Convert.FromHexString(hex)));
        }

        private static byte[] Serialize(Dictionary<string, string> value)
        {
            var buffer = new ArrayBufferWriter<byte>();
            var writer = new MessagePackWriter(buffer);

            StringDictionaryMessagePackFormatter.Instance.Serialize(ref writer, value, MessagePackSerializerOptions.Standard);
            writer.Flush();

            return buffer.WrittenMemory.ToArray();
        }

        private static Dictionary<string, string> Deserialize(byte[] payload)
        {
            var reader = new MessagePackReader(payload);
            var result = StringDictionaryMessagePackFormatter.Instance.Deserialize(ref reader, MessagePackSerializerOptions.Standard);

            Assert.That(reader.End, Is.True);

            return result;
        }
    }
}
