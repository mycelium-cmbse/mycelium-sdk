// ------------------------------------------------------------------------------------------------
//  <copyright file="Utf8JsonReaderHelperTestFixture.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.Serializer.Json.Tests.Utility
{
    using System;
    using System.Text;
    using System.Text.Json;

    using Mycelium.SDK.Serializer.Json.Utility;

    /// <summary>
    /// Verifies the low-level JSON reader operations used by generated deserializers.
    /// </summary>
    [TestFixture]
    public class Utf8JsonReaderHelperTestFixture
    {
        /// <summary>
        /// Verifies identifier and date-time conversions.
        /// </summary>
        [Test]
        public void Verify_Guid_and_DateTime_reading()
        {
            var expectedGuid = Guid.Parse("11111111-2222-3333-4444-555555555555");

            var expectedDateTime = new DateTime(2026, 5, 6, 7, 8, 9, DateTimeKind.Utc);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(Read("\"11111111-2222-3333-4444-555555555555\"", Utf8JsonReaderHelper.ReadGuid), Is.EqualTo(expectedGuid));

                Assert.That(Read("\"2026-05-06T07:08:09.0000000Z\"", Utf8JsonReaderHelper.ReadDateTime), Is.EqualTo(expectedDateTime));

                Assert.That(Read("null", Utf8JsonReaderHelper.ReadGuidOrNull), Is.Null);

                Assert.That(Read("null", Utf8JsonReaderHelper.ReadDateTimeOrNull), Is.Null);
            }

            using (Assert.EnterMultipleScope())
            {
                Assert.That(() => Read("\"not-a-guid\"", Utf8JsonReaderHelper.ReadGuid), Throws.TypeOf<JsonException>());

                Assert.That(() => Read("\"not-a-date\"", Utf8JsonReaderHelper.ReadDateTime), Throws.TypeOf<JsonException>());
            }
        }

        /// <summary>
        /// Verifies that one complete nested unknown value can be skipped.
        /// </summary>
        [Test]
        public void Verify_nested_value_skipping()
        {
            Assert.That(SkipNestedValueAndReadFollowingString(), Is.EqualTo("retained"));
        }

        /// <summary>
        /// Verifies representative scalar and nullable conversions.
        /// </summary>
        [Test]
        public void Verify_scalar_and_nullable_value_reading()
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(Read("\"value\"", Utf8JsonReaderHelper.ReadRequiredString), Is.EqualTo("value"));

                Assert.That(Read("true", Utf8JsonReaderHelper.ReadBoolean), Is.True);

                Assert.That(Read("42", Utf8JsonReaderHelper.ReadInt32), Is.EqualTo(42));

                Assert.That(Read("1.25", Utf8JsonReaderHelper.ReadDouble), Is.EqualTo(1.25d));

                Assert.That(Read("null", Utf8JsonReaderHelper.ReadStringOrNull), Is.Null);

                Assert.That(Read("null", Utf8JsonReaderHelper.ReadBooleanOrNull), Is.Null);

                Assert.That(Read("null", Utf8JsonReaderHelper.ReadInt32OrNull), Is.Null);

                Assert.That(Read("null", Utf8JsonReaderHelper.ReadDoubleOrNull), Is.Null);
            }

            Assert.That(() => Read("\"42\"", Utf8JsonReaderHelper.ReadInt32), Throws.TypeOf<JsonException>());
        }

        /// <summary>
        /// Verifies exact dictionary parsing and duplicate-key rejection.
        /// </summary>
        [Test]
        public void Verify_string_dictionary_reading()
        {
            var dictionary = Read("{\"Key\":\"FIRST\",\"key\":\"second\"}", Utf8JsonReaderHelper.ReadStringDictionary);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(dictionary, Has.Count.EqualTo(2));
                Assert.That(dictionary.Comparer, Is.SameAs(StringComparer.Ordinal));
                Assert.That(dictionary["Key"], Is.EqualTo("FIRST"));
                Assert.That(dictionary["key"], Is.EqualTo("second"));
            }

            Assert.That(() => Read("{\"key\":\"first\",\"key\":\"second\"}", Utf8JsonReaderHelper.ReadStringDictionary), Throws.TypeOf<JsonException>());
        }

        /// <summary>
        /// Verifies token advancement and exact token enforcement.
        /// </summary>
        [Test]
        public void Verify_token_navigation_and_expectation()
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(ReadFirstStringUsingReadNext("\"value\""), Is.EqualTo("value"));

                Assert.That(() => ExpectToken("true", JsonTokenType.String), Throws.TypeOf<JsonException>());

                Assert.That(ReadBeyondEnd, Throws.TypeOf<JsonException>());
            }
        }

        /// <summary>
        /// Represents an operation that reads a value from a positioned JSON reader.
        /// </summary>
        private delegate T ReaderOperation<out T>(ref Utf8JsonReader reader);

        /// <summary>
        /// Reads one JSON value using the supplied helper operation.
        /// </summary>
        private static T Read<T>(string json, ReaderOperation<T> operation)
        {
            var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(json));

            if (!reader.Read())
            {
                throw new InvalidOperationException("The test JSON contains no value.");
            }

            return operation(ref reader);
        }

        /// <summary>
        /// Positions a reader and verifies its first token.
        /// </summary>
        private static void ExpectToken(string json, JsonTokenType tokenType)
        {
            var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(json));

            if (!reader.Read())
            {
                throw new InvalidOperationException("The test JSON contains no value.");
            }

            Utf8JsonReaderHelper.Expect(ref reader, tokenType);
        }

        /// <summary>
        /// Advances from the reader's initial state and reads a string.
        /// </summary>
        private static string ReadFirstStringUsingReadNext(string json)
        {
            var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(json));

            Utf8JsonReaderHelper.ReadNext(ref reader);

            return Utf8JsonReaderHelper.ReadRequiredString(ref reader);
        }

        /// <summary>
        /// Attempts to advance beyond complete input.
        /// </summary>
        private static void ReadBeyondEnd()
        {
            var reader = new Utf8JsonReader("null"u8);

            Utf8JsonReaderHelper.ReadNext(ref reader);
            Utf8JsonReaderHelper.ReadNext(ref reader);
        }

        /// <summary>
        /// Skips a nested value and reads the property following it.
        /// </summary>
        private static string SkipNestedValueAndReadFollowingString()
        {
            var reader = new Utf8JsonReader("""
                                            {
                                              "unknown": {
                                                "nested": [
                                                  1,
                                                  {
                                                    "value": true
                                                  }
                                                ]
                                              },
                                              "next": "retained"
                                            }
                                            """u8);

            Utf8JsonReaderHelper.ReadNext(ref reader);
            Utf8JsonReaderHelper.Expect(ref reader, JsonTokenType.StartObject);

            Utf8JsonReaderHelper.ReadNext(ref reader);
            Utf8JsonReaderHelper.Expect(ref reader, JsonTokenType.PropertyName);

            Utf8JsonReaderHelper.ReadNext(ref reader);
            Utf8JsonReaderHelper.SkipValue(ref reader);

            Utf8JsonReaderHelper.ReadNext(ref reader);
            Utf8JsonReaderHelper.Expect(ref reader, JsonTokenType.PropertyName);

            Utf8JsonReaderHelper.ReadNext(ref reader);

            return Utf8JsonReaderHelper.ReadRequiredString(ref reader);
        }
    }
}
