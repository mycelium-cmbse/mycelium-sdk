// ------------------------------------------------------------------------------------------------
//  <copyright file="SerializationProvider.cs" company="Starion Group S.A.">
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
    using System.Collections.Generic;
    using System.Text.Json;

    using Mycelium.SDK.DTO;

    /// <summary>
    /// Provides an exact-runtime-type serializer for each supported concrete DTO.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    internal static class SerializationProvider
    {
        /// <summary>
        /// Maps exact concrete DTO types to their generated serializers.
        /// </summary>
        private static readonly IReadOnlyDictionary<Type, Action<object, Utf8JsonWriter>> SerializerActionMap =
        new Dictionary<Type, Action<object, Utf8JsonWriter>>
        {
            [typeof(BranchProtectionRule)] = BranchProtectionRuleSerializer.Serialize,
            [typeof(Comment)] = CommentSerializer.Serialize,
            [typeof(FunctionalProject)] = FunctionalProjectSerializer.Serialize,
            [typeof(FunctionalProjectPolicy)] = FunctionalProjectPolicySerializer.Serialize,
            [typeof(Organization)] = OrganizationSerializer.Serialize,
            [typeof(OrganizationMember)] = OrganizationMemberSerializer.Serialize,
            [typeof(OrganizationPolicy)] = OrganizationPolicySerializer.Serialize,
            [typeof(Ownership)] = OwnershipSerializer.Serialize,
            [typeof(ProjectMember)] = ProjectMemberSerializer.Serialize,
            [typeof(Review)] = ReviewSerializer.Serialize,
            [typeof(User)] = UserSerializer.Serialize,
        };

        /// <summary>
        /// Provides the generated serializer registered for an exact concrete DTO type.
        /// </summary>
        /// <param name="runtimeType">
        /// The exact runtime DTO type.
        /// </param>
        /// <returns>
        /// The serializer registered for <paramref name="runtimeType" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="runtimeType" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Thrown when the exact runtime type is not supported.
        /// </exception>
        internal static Action<object, Utf8JsonWriter> Provide(Type runtimeType)
        {
            if (runtimeType == null)
            {
                throw new ArgumentNullException(nameof(runtimeType));
            }

            if (SerializerActionMap.TryGetValue(runtimeType, out var serializer))
            {
                return serializer;
            }

            throw new NotSupportedException(
            $"Runtime DTO type '{runtimeType.FullName}' is not supported by the serialization provider.");
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
