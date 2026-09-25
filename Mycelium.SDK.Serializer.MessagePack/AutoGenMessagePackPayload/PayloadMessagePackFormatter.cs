// ------------------------------------------------------------------------------------------------
//  <copyright file="PayloadMessagePackFormatter.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.Serializer.MessagePack
{
    using System;
    using System.CodeDom.Compiler;

    using global::MessagePack;
    using global::MessagePack.Formatters;

    using Mycelium.SDK.DTO;

    /// <summary>
    /// Serializes and deserializes the positional MessagePack payload envelope.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    internal sealed partial class PayloadMessagePackFormatter : IMessagePackFormatter<Payload?>
    {
        /// <summary>
        /// Serializes a payload with its timestamp and every concrete-type group.
        /// </summary>
        /// <param name="writer">
        /// The writer that receives the payload.
        /// </param>
        /// <param name="payload">
        /// The payload to serialize.
        /// </param>
        /// <param name="options">
        /// The MessagePack serializer options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="payload" /> or a group item is <see langword="null" />.
        /// </exception>
        public void Serialize(ref MessagePackWriter writer, Payload? payload, MessagePackSerializerOptions options)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            writer.WriteArrayHeader(12);
            writer.Write(payload.Created);

            writer.WriteArrayHeader(payload.BranchProtectionRule.Count);

            foreach (var dataItem in payload.BranchProtectionRule)
            {
                if (dataItem == null)
                {
                    throw new ArgumentNullException(nameof(payload), "A payload group contains a null DTO.");
                }

                options.Resolver.GetFormatterWithVerify<BranchProtectionRule?>().Serialize(ref writer, dataItem, options);
            }

            writer.WriteArrayHeader(payload.Comment.Count);

            foreach (var dataItem in payload.Comment)
            {
                if (dataItem == null)
                {
                    throw new ArgumentNullException(nameof(payload), "A payload group contains a null DTO.");
                }

                options.Resolver.GetFormatterWithVerify<Comment?>().Serialize(ref writer, dataItem, options);
            }

            writer.WriteArrayHeader(payload.FunctionalProject.Count);

            foreach (var dataItem in payload.FunctionalProject)
            {
                if (dataItem == null)
                {
                    throw new ArgumentNullException(nameof(payload), "A payload group contains a null DTO.");
                }

                options.Resolver.GetFormatterWithVerify<FunctionalProject?>().Serialize(ref writer, dataItem, options);
            }

            writer.WriteArrayHeader(payload.FunctionalProjectPolicy.Count);

            foreach (var dataItem in payload.FunctionalProjectPolicy)
            {
                if (dataItem == null)
                {
                    throw new ArgumentNullException(nameof(payload), "A payload group contains a null DTO.");
                }

                options.Resolver.GetFormatterWithVerify<FunctionalProjectPolicy?>().Serialize(ref writer, dataItem, options);
            }

            writer.WriteArrayHeader(payload.Organization.Count);

            foreach (var dataItem in payload.Organization)
            {
                if (dataItem == null)
                {
                    throw new ArgumentNullException(nameof(payload), "A payload group contains a null DTO.");
                }

                options.Resolver.GetFormatterWithVerify<Organization?>().Serialize(ref writer, dataItem, options);
            }

            writer.WriteArrayHeader(payload.OrganizationMember.Count);

            foreach (var dataItem in payload.OrganizationMember)
            {
                if (dataItem == null)
                {
                    throw new ArgumentNullException(nameof(payload), "A payload group contains a null DTO.");
                }

                options.Resolver.GetFormatterWithVerify<OrganizationMember?>().Serialize(ref writer, dataItem, options);
            }

            writer.WriteArrayHeader(payload.OrganizationPolicy.Count);

            foreach (var dataItem in payload.OrganizationPolicy)
            {
                if (dataItem == null)
                {
                    throw new ArgumentNullException(nameof(payload), "A payload group contains a null DTO.");
                }

                options.Resolver.GetFormatterWithVerify<OrganizationPolicy?>().Serialize(ref writer, dataItem, options);
            }

            writer.WriteArrayHeader(payload.Ownership.Count);

            foreach (var dataItem in payload.Ownership)
            {
                if (dataItem == null)
                {
                    throw new ArgumentNullException(nameof(payload), "A payload group contains a null DTO.");
                }

                options.Resolver.GetFormatterWithVerify<Ownership?>().Serialize(ref writer, dataItem, options);
            }

            writer.WriteArrayHeader(payload.ProjectMember.Count);

            foreach (var dataItem in payload.ProjectMember)
            {
                if (dataItem == null)
                {
                    throw new ArgumentNullException(nameof(payload), "A payload group contains a null DTO.");
                }

                options.Resolver.GetFormatterWithVerify<ProjectMember?>().Serialize(ref writer, dataItem, options);
            }

            writer.WriteArrayHeader(payload.Review.Count);

            foreach (var dataItem in payload.Review)
            {
                if (dataItem == null)
                {
                    throw new ArgumentNullException(nameof(payload), "A payload group contains a null DTO.");
                }

                options.Resolver.GetFormatterWithVerify<Review?>().Serialize(ref writer, dataItem, options);
            }

            writer.WriteArrayHeader(payload.User.Count);

            foreach (var dataItem in payload.User)
            {
                if (dataItem == null)
                {
                    throw new ArgumentNullException(nameof(payload), "A payload group contains a null DTO.");
                }

                options.Resolver.GetFormatterWithVerify<User?>().Serialize(ref writer, dataItem, options);
            }

        }

        /// <summary>
        /// Deserializes a payload with its timestamp and every concrete-type group.
        /// </summary>
        /// <param name="reader">
        /// The reader from which the payload is deserialized.
        /// </param>
        /// <param name="options">
        /// The MessagePack serializer options.
        /// </param>
        /// <returns>
        /// The payload with the received timestamp and DTO groups.
        /// </returns>
        /// <exception cref="MessagePackSerializationException">
        /// Thrown when the envelope is nil, has an incorrect field count or contains a null DTO item.
        /// </exception>
        public Payload? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                throw new MessagePackSerializationException("The MessagePack payload may not be nil.");
            }

            options.Security.DepthStep(ref reader);

            try
            {
                var fieldCount = reader.ReadArrayHeader();

                if (fieldCount != 12)
                {
                    throw new MessagePackSerializationException($"The payload contains {fieldCount} fields; exactly 12 fields are required.");
                }

                var payload = new Payload
                {
                    Created = reader.ReadDateTime()
                };

                var branchProtectionRuleCount = reader.ReadArrayHeader();

                for (var index = 0; index < branchProtectionRuleCount; index++)
                {
                    var dataItem = options.Resolver.GetFormatterWithVerify<BranchProtectionRule?>().Deserialize(ref reader, options);

                    if (dataItem == null)
                    {
                        throw new MessagePackSerializationException("The BranchProtectionRule group contains a null DTO.");
                    }

                    payload.BranchProtectionRule.Add(dataItem);
                }

                var commentCount = reader.ReadArrayHeader();

                for (var index = 0; index < commentCount; index++)
                {
                    var dataItem = options.Resolver.GetFormatterWithVerify<Comment?>().Deserialize(ref reader, options);

                    if (dataItem == null)
                    {
                        throw new MessagePackSerializationException("The Comment group contains a null DTO.");
                    }

                    payload.Comment.Add(dataItem);
                }

                var functionalProjectCount = reader.ReadArrayHeader();

                for (var index = 0; index < functionalProjectCount; index++)
                {
                    var dataItem = options.Resolver.GetFormatterWithVerify<FunctionalProject?>().Deserialize(ref reader, options);

                    if (dataItem == null)
                    {
                        throw new MessagePackSerializationException("The FunctionalProject group contains a null DTO.");
                    }

                    payload.FunctionalProject.Add(dataItem);
                }

                var functionalProjectPolicyCount = reader.ReadArrayHeader();

                for (var index = 0; index < functionalProjectPolicyCount; index++)
                {
                    var dataItem = options.Resolver.GetFormatterWithVerify<FunctionalProjectPolicy?>().Deserialize(ref reader, options);

                    if (dataItem == null)
                    {
                        throw new MessagePackSerializationException("The FunctionalProjectPolicy group contains a null DTO.");
                    }

                    payload.FunctionalProjectPolicy.Add(dataItem);
                }

                var organizationCount = reader.ReadArrayHeader();

                for (var index = 0; index < organizationCount; index++)
                {
                    var dataItem = options.Resolver.GetFormatterWithVerify<Organization?>().Deserialize(ref reader, options);

                    if (dataItem == null)
                    {
                        throw new MessagePackSerializationException("The Organization group contains a null DTO.");
                    }

                    payload.Organization.Add(dataItem);
                }

                var organizationMemberCount = reader.ReadArrayHeader();

                for (var index = 0; index < organizationMemberCount; index++)
                {
                    var dataItem = options.Resolver.GetFormatterWithVerify<OrganizationMember?>().Deserialize(ref reader, options);

                    if (dataItem == null)
                    {
                        throw new MessagePackSerializationException("The OrganizationMember group contains a null DTO.");
                    }

                    payload.OrganizationMember.Add(dataItem);
                }

                var organizationPolicyCount = reader.ReadArrayHeader();

                for (var index = 0; index < organizationPolicyCount; index++)
                {
                    var dataItem = options.Resolver.GetFormatterWithVerify<OrganizationPolicy?>().Deserialize(ref reader, options);

                    if (dataItem == null)
                    {
                        throw new MessagePackSerializationException("The OrganizationPolicy group contains a null DTO.");
                    }

                    payload.OrganizationPolicy.Add(dataItem);
                }

                var ownershipCount = reader.ReadArrayHeader();

                for (var index = 0; index < ownershipCount; index++)
                {
                    var dataItem = options.Resolver.GetFormatterWithVerify<Ownership?>().Deserialize(ref reader, options);

                    if (dataItem == null)
                    {
                        throw new MessagePackSerializationException("The Ownership group contains a null DTO.");
                    }

                    payload.Ownership.Add(dataItem);
                }

                var projectMemberCount = reader.ReadArrayHeader();

                for (var index = 0; index < projectMemberCount; index++)
                {
                    var dataItem = options.Resolver.GetFormatterWithVerify<ProjectMember?>().Deserialize(ref reader, options);

                    if (dataItem == null)
                    {
                        throw new MessagePackSerializationException("The ProjectMember group contains a null DTO.");
                    }

                    payload.ProjectMember.Add(dataItem);
                }

                var reviewCount = reader.ReadArrayHeader();

                for (var index = 0; index < reviewCount; index++)
                {
                    var dataItem = options.Resolver.GetFormatterWithVerify<Review?>().Deserialize(ref reader, options);

                    if (dataItem == null)
                    {
                        throw new MessagePackSerializationException("The Review group contains a null DTO.");
                    }

                    payload.Review.Add(dataItem);
                }

                var userCount = reader.ReadArrayHeader();

                for (var index = 0; index < userCount; index++)
                {
                    var dataItem = options.Resolver.GetFormatterWithVerify<User?>().Deserialize(ref reader, options);

                    if (dataItem == null)
                    {
                        throw new MessagePackSerializationException("The User group contains a null DTO.");
                    }

                    payload.User.Add(dataItem);
                }

                return payload;
            }
            finally
            {
                reader.Depth--;
            }
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
