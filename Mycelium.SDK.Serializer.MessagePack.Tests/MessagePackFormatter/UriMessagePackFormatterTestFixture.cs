// ------------------------------------------------------------------------------------------------
//  <copyright file="UriMessagePackFormatterTestFixture.cs" company="Starion Group S.A.">
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
    public class UriMessagePackFormatterTestFixture
    {
        [TestCase("https://EXAMPLE.com:443/a%2Fb?x=one%20two#part", true)]
        [TestCase("../models/item?version=1", false)]
        [TestCase("", false)]
        public void Verify_that_relative_and_absolute_uris_preserve_their_original_strings(string text, bool absolute)
        {
            var expected = new Uri(text, UriKind.RelativeOrAbsolute);
            var payload = Serialize(expected);
            var reader = new MessagePackReader(payload);
            var actual = Deserialize(payload);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(reader.ReadString(), Is.EqualTo(text));
                Assert.That(reader.End, Is.True);
                Assert.That(actual.OriginalString, Is.EqualTo(text));
                Assert.That(actual.IsAbsoluteUri, Is.EqualTo(absolute));
            }
        }

        [Test]
        public void Verify_that_null_serialization_input_is_rejected()
        {
            Assert.Throws<MessagePackSerializationException>(() => Serialize(null));
        }

        [TestCase("C0")]
        [TestCase("01")]
        [TestCase("C3")]
        [TestCase("90")]
        public void Verify_that_nil_and_non_string_input_are_rejected(string hex)
        {
            Assert.Throws<MessagePackSerializationException>(() => Deserialize(Convert.FromHexString(hex)));
        }

        [Test]
        public void Verify_that_invalid_uri_strings_are_rejected()
        {
            var payload = MessagePackSerializer.Serialize("http://[invalid");

            Assert.Throws<MessagePackSerializationException>(() => Deserialize(payload));
        }

        private static byte[] Serialize(Uri value)
        {
            var buffer = new ArrayBufferWriter<byte>();
            var writer = new MessagePackWriter(buffer);

            UriMessagePackFormatter.Instance.Serialize(ref writer, value, MessagePackSerializerOptions.Standard);
            writer.Flush();

            return buffer.WrittenMemory.ToArray();
        }

        private static Uri Deserialize(byte[] payload)
        {
            var reader = new MessagePackReader(payload);
            var result = UriMessagePackFormatter.Instance.Deserialize(ref reader, MessagePackSerializerOptions.Standard);

            Assert.That(reader.End, Is.True);

            return result;
        }
    }
}
