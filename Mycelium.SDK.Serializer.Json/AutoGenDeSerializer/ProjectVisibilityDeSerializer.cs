// ------------------------------------------------------------------------------------------------
//  <copyright file="ProjectVisibilityDeSerializer.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.Serializer.Json
{
    using System;
    using System.CodeDom.Compiler;
    using System.Text.Json;

    using Mycelium.SDK;
    using Mycelium.SDK.Extensions;
    using Mycelium.SDK.Serializer.Json.Utility;

    /// <summary>
    /// Deserializes the JSON wire representation of <see cref="ProjectVisibility" />.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    internal static class ProjectVisibilityDeSerializer
    {
        /// <summary>
        /// Deserializes a required exact-uppercase
        /// <see cref="ProjectVisibility" /> value.
        /// </summary>
        /// <param name="reader">
        /// The JSON reader positioned on the enumeration value.
        /// </param>
        /// <returns>
        /// The deserialized enumeration value.
        /// </returns>
        /// <exception cref="JsonException">
        /// Thrown when the value is null, is not a string, or is not an exact uppercase
        /// representation of a defined literal.
        /// </exception>
        internal static ProjectVisibility DeSerialize(
            ref Utf8JsonReader reader)
        {
            var value = Utf8JsonReaderHelper.ReadRequiredString(ref reader);

            if (ProjectVisibilityProvider.TryParse(
                    value,
                    out var result)
                && string.Equals(
                    value,
                    ProjectVisibilityProvider
                        .Format(result)
                        .ToUpperInvariant(),
                    StringComparison.Ordinal))
            {
                return result;
            }

            throw new JsonException(
                $"'{value}' is not a valid uppercase ProjectVisibility JSON value.");
        }

        /// <summary>
        /// Deserializes a nullable exact-uppercase
        /// <see cref="ProjectVisibility" /> value.
        /// </summary>
        /// <param name="reader">
        /// The JSON reader positioned on the enumeration value.
        /// </param>
        /// <returns>
        /// The deserialized enumeration value, or <see langword="null" />.
        /// </returns>
        /// <exception cref="JsonException">
        /// Thrown when a non-null value is not a string or is not an exact uppercase
        /// representation of a defined literal.
        /// </exception>
        internal static ProjectVisibility? DeSerializeOrNull(
            ref Utf8JsonReader reader)
        {
            return reader.TokenType == JsonTokenType.Null
                ? null
                : DeSerialize(ref reader);
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
