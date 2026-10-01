// ------------------------------------------------------------------------------------------------
//  <copyright file="MessagePackFormatterBase.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.Serializer.MessagePack
{
    using System.Globalization;

    using global::MessagePack;

    /// <summary>
    /// Provides the shared wire-format operations used by generated MessagePack DTO formatters.
    /// </summary>
    public abstract class MessagePackFormatterBase
    {
        /// <summary>
        /// Writes a required string value.
        /// </summary>
        /// <param name="writer">
        /// The MessagePack writer that receives the string.
        /// </param>
        /// <param name="value">
        /// The string to write.
        /// </param>
        /// <param name="valueDescription">
        /// The modeled value description used in validation messages.
        /// </param>
        /// <exception cref="MessagePackSerializationException">
        /// Thrown when <paramref name="value" /> is <see langword="null" />.
        /// </exception>
        protected static void WriteRequiredString(ref MessagePackWriter writer, string value, string valueDescription)
        {
            if (value == null)
            {
                throw new MessagePackSerializationException($"String value '{valueDescription}' may not be null.");
            }

            writer.Write(value);
        }

        /// <summary>
        /// Reads a required string value.
        /// </summary>
        /// <param name="reader">
        /// The MessagePack reader from which the string is read.
        /// </param>
        /// <param name="valueDescription">
        /// The modeled value description used in validation messages.
        /// </param>
        /// <returns>
        /// The decoded non-null string.
        /// </returns>
        /// <exception cref="MessagePackSerializationException">
        /// Thrown when the value is <c>nil</c> or is not a MessagePack string.
        /// </exception>
        protected static string ReadRequiredString(ref MessagePackReader reader, string valueDescription)
        {
            return reader.ReadString() ?? throw new MessagePackSerializationException($"String value '{valueDescription}' may not be nil.");
        }

        /// <summary>
        /// Writes a date and time as an invariant round-trip MessagePack string.
        /// </summary>
        /// <param name="writer">
        /// The MessagePack writer that receives the date and time.
        /// </param>
        /// <param name="value">
        /// The date and time to write.
        /// </param>
        protected static void WriteRoundTripDateTime(ref MessagePackWriter writer, DateTime value)
        {
            writer.Write(value.ToString("o", CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Reads a date and time from an invariant round-trip MessagePack string.
        /// </summary>
        /// <param name="reader">
        /// The MessagePack reader from which the date and time is read.
        /// </param>
        /// <param name="valueDescription">
        /// The modeled value description used in validation messages.
        /// </param>
        /// <returns>
        /// The decoded date and time.
        /// </returns>
        /// <exception cref="MessagePackSerializationException">
        /// Thrown when the value is <c>nil</c>, is not a MessagePack string, or is not in the invariant round-trip format.
        /// </exception>
        protected static DateTime ReadRoundTripDateTime(ref MessagePackReader reader, string valueDescription)
        {
            var value = ReadRequiredString(ref reader, valueDescription);

            if (!DateTime.TryParseExact(value, "o", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var result))
            {
                throw new MessagePackSerializationException($"DateTime value '{valueDescription}' is not in the invariant round-trip format.");
            }

            return result;
        }
    }
}
