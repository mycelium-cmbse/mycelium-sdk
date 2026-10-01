// ------------------------------------------------------------------------------------------------
//  <copyright file="GuidMessagePackFormatterTestFixture.cs" company="Starion Group S.A.">
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

    [TestFixture]
    public class GuidMessagePackFormatterTestFixture
    {
        [Test]
        public void Verify_that_formatter_uses_the_default_dotnet_binary_layout()
        {
            var payload = Serialize(Guid.Parse("00112233-4455-6677-8899-aabbccddeeff"));
            var reader = new MessagePackReader(payload);

            Assert.That(reader.ReadBytes().Value.ToArray(), Is.EqualTo(new byte[]
            {
                0x33, 0x22, 0x11, 0x00, 0x55, 0x44, 0x77, 0x66,
                0x88, 0x99, 0xaa, 0xbb, 0xcc, 0xdd, 0xee, 0xff
            }));
            Assert.That(reader.End, Is.True);
        }

        [TestCase("00000000-0000-0000-0000-000000000000")]
        [TestCase("00112233-4455-6677-8899-aabbccddeeff")]
        public void Verify_that_identifiers_round_trip(string text)
        {
            var expected = Guid.Parse(text);

            Assert.That(Deserialize(new ReadOnlySequence<byte>(Serialize(expected))), Is.EqualTo(expected));
        }

        [Test]
        public void Verify_that_formatter_reads_binary_values_across_sequence_segments()
        {
            var expected = Guid.Parse("00112233-4455-6677-8899-aabbccddeeff");
            var payload = Serialize(expected);
            var first = new BufferSegment(payload.AsMemory(0, 7));
            var last = first.Append(payload.AsMemory(7));
            var sequence = new ReadOnlySequence<byte>(first, 0, last, last.Memory.Length);

            Assert.That(Deserialize(sequence), Is.EqualTo(expected));
        }

        [TestCase("C0")]
        [TestCase("D92430303131323233332D343435352D363637372D383839392D616162626363646465656666")]
        [TestCase("C400")]
        [TestCase("C40F000000000000000000000000000000")]
        [TestCase("C4110000000000000000000000000000000000")]
        public void Verify_that_nil_text_and_incorrect_binary_lengths_are_rejected(string hex)
        {
            var payload = new ReadOnlySequence<byte>(Convert.FromHexString(hex));

            Assert.Throws<MessagePackSerializationException>(() => Deserialize(payload));
        }

        private static byte[] Serialize(Guid value)
        {
            var buffer = new ArrayBufferWriter<byte>();
            var writer = new MessagePackWriter(buffer);

            GuidMessagePackFormatter.Instance.Serialize(ref writer, value, MessagePackSerializerOptions.Standard);
            writer.Flush();

            return buffer.WrittenMemory.ToArray();
        }

        private static Guid Deserialize(ReadOnlySequence<byte> payload)
        {
            var reader = new MessagePackReader(payload);
            var result = GuidMessagePackFormatter.Instance.Deserialize(ref reader, MessagePackSerializerOptions.Standard);

            Assert.That(reader.End, Is.True);

            return result;
        }

        private sealed class BufferSegment : ReadOnlySequenceSegment<byte>
        {
            public BufferSegment(ReadOnlyMemory<byte> memory)
            {
                this.Memory = memory;
            }

            public BufferSegment Append(ReadOnlyMemory<byte> memory)
            {
                var next = new BufferSegment(memory) { RunningIndex = this.RunningIndex + this.Memory.Length };
                this.Next = next;

                return next;
            }
        }
    }
}
