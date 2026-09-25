// ------------------------------------------------------------------------------------------------
//  <copyright file="PayloadFactory.cs" company="Starion Group S.A.">
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
    using System.Collections.Generic;

    using Mycelium.SDK.DTO;

    /// <summary>
    /// Converts between DTO sequences and the grouped MessagePack payload.
    /// </summary>
    [GeneratedCode("Mycelium.SDK", "latest")]
    internal static partial class PayloadFactory
    {
        /// <summary>
        /// Creates a payload from concrete DTOs using exact runtime types.
        /// </summary>
        /// <param name="dataItems">
        /// The DTOs to group.
        /// </param>
        /// <returns>
        /// A new payload containing every supplied DTO in its concrete-type group.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="dataItems" /> or one of its items is <see langword="null" />.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Thrown when an item has no generated concrete DTO group.
        /// </exception>
        internal static Payload ToPayload(this IEnumerable<IThing> dataItems)
        {
            if (dataItems == null)
            {
                throw new ArgumentNullException(nameof(dataItems));
            }

            var payload = new Payload();

            foreach (var dataItem in dataItems)
            {
                if (dataItem == null)
                {
                    throw new ArgumentNullException(nameof(dataItems), "The DTO sequence contains a null item.");
                }

                var runtimeType = dataItem.GetType();

                if (runtimeType == typeof(BranchProtectionRule))
                {
                    payload.BranchProtectionRule.Add((BranchProtectionRule)dataItem);
                    continue;
                }

                if (runtimeType == typeof(Comment))
                {
                    payload.Comment.Add((Comment)dataItem);
                    continue;
                }

                if (runtimeType == typeof(FunctionalProject))
                {
                    payload.FunctionalProject.Add((FunctionalProject)dataItem);
                    continue;
                }

                if (runtimeType == typeof(FunctionalProjectPolicy))
                {
                    payload.FunctionalProjectPolicy.Add((FunctionalProjectPolicy)dataItem);
                    continue;
                }

                if (runtimeType == typeof(Organization))
                {
                    payload.Organization.Add((Organization)dataItem);
                    continue;
                }

                if (runtimeType == typeof(OrganizationMember))
                {
                    payload.OrganizationMember.Add((OrganizationMember)dataItem);
                    continue;
                }

                if (runtimeType == typeof(OrganizationPolicy))
                {
                    payload.OrganizationPolicy.Add((OrganizationPolicy)dataItem);
                    continue;
                }

                if (runtimeType == typeof(Ownership))
                {
                    payload.Ownership.Add((Ownership)dataItem);
                    continue;
                }

                if (runtimeType == typeof(ProjectMember))
                {
                    payload.ProjectMember.Add((ProjectMember)dataItem);
                    continue;
                }

                if (runtimeType == typeof(Review))
                {
                    payload.Review.Add((Review)dataItem);
                    continue;
                }

                if (runtimeType == typeof(User))
                {
                    payload.User.Add((User)dataItem);
                    continue;
                }

                throw new NotSupportedException($"No MessagePack payload group is registered for exact type '{runtimeType.FullName}'.");
            }

            return payload;
        }

        /// <summary>
        /// Materializes the DTOs from each concrete-type group in envelope order.
        /// </summary>
        /// <param name="payload">
        /// The payload whose DTOs are returned.
        /// </param>
        /// <returns>
        /// A materialized list of the payload's DTOs.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="payload" /> or one of its group items is <see langword="null" />.
        /// </exception>
        internal static IReadOnlyList<IThing> ToDataItems(this Payload payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            var dataItems = new List<IThing>();

            foreach (var dataItem in payload.BranchProtectionRule)
            {
                if (dataItem == null)
                {
                    throw new ArgumentNullException(nameof(payload), "A payload group contains a null DTO.");
                }

                dataItems.Add(dataItem);
            }

            foreach (var dataItem in payload.Comment)
            {
                if (dataItem == null)
                {
                    throw new ArgumentNullException(nameof(payload), "A payload group contains a null DTO.");
                }

                dataItems.Add(dataItem);
            }

            foreach (var dataItem in payload.FunctionalProject)
            {
                if (dataItem == null)
                {
                    throw new ArgumentNullException(nameof(payload), "A payload group contains a null DTO.");
                }

                dataItems.Add(dataItem);
            }

            foreach (var dataItem in payload.FunctionalProjectPolicy)
            {
                if (dataItem == null)
                {
                    throw new ArgumentNullException(nameof(payload), "A payload group contains a null DTO.");
                }

                dataItems.Add(dataItem);
            }

            foreach (var dataItem in payload.Organization)
            {
                if (dataItem == null)
                {
                    throw new ArgumentNullException(nameof(payload), "A payload group contains a null DTO.");
                }

                dataItems.Add(dataItem);
            }

            foreach (var dataItem in payload.OrganizationMember)
            {
                if (dataItem == null)
                {
                    throw new ArgumentNullException(nameof(payload), "A payload group contains a null DTO.");
                }

                dataItems.Add(dataItem);
            }

            foreach (var dataItem in payload.OrganizationPolicy)
            {
                if (dataItem == null)
                {
                    throw new ArgumentNullException(nameof(payload), "A payload group contains a null DTO.");
                }

                dataItems.Add(dataItem);
            }

            foreach (var dataItem in payload.Ownership)
            {
                if (dataItem == null)
                {
                    throw new ArgumentNullException(nameof(payload), "A payload group contains a null DTO.");
                }

                dataItems.Add(dataItem);
            }

            foreach (var dataItem in payload.ProjectMember)
            {
                if (dataItem == null)
                {
                    throw new ArgumentNullException(nameof(payload), "A payload group contains a null DTO.");
                }

                dataItems.Add(dataItem);
            }

            foreach (var dataItem in payload.Review)
            {
                if (dataItem == null)
                {
                    throw new ArgumentNullException(nameof(payload), "A payload group contains a null DTO.");
                }

                dataItems.Add(dataItem);
            }

            foreach (var dataItem in payload.User)
            {
                if (dataItem == null)
                {
                    throw new ArgumentNullException(nameof(payload), "A payload group contains a null DTO.");
                }

                dataItems.Add(dataItem);
            }

            return dataItems;
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
