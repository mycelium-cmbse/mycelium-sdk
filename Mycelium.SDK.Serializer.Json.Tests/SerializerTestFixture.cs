// ------------------------------------------------------------------------------------------------
//  <copyright file="SerializerTestFixture.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.Serializer.Json.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;

    using Mycelium.SDK.DTO;
    using Mycelium.SDK.Serializer.Json;

    /// <summary>
    /// Verifies the generated JSON DTO serialization runtime contract.
    /// </summary>
    [TestFixture]
    public class SerializerTestFixture
    {
        /// <summary>
        /// Expected modeled property order for a branch protection rule.
        /// </summary>
        private static readonly string[] ExpectedBranchProtectionRulePropertyNames =
        [
            "@type",
            "@id",
            "createdBy",
            "createdOn",
            "defaultReviewers",
            "engineeringBranchId",
            "mergeAllowedFor",
            "minimumRequiredApproval",
            "name",
            "reviewRequired",
            "updatedBy",
            "updatedOn",
        ];

        /// <summary>
        /// Expected invariant-uppercase enumeration literals for merge roles.
        /// </summary>
        private static readonly string[] ExpectedMergeAllowedForLiterals =
        [
            "VIEWER",
            "ADMINISTRATOR",
        ];

        /// <summary>
        /// Expected modeled property order for a project member.
        /// </summary>
        private static readonly string[] ExpectedProjectMemberPropertyNames =
        [
            "@type",
            "@id",
            "activeOwnership",
            "createdBy",
            "createdOn",
            "isPartOf",
            "owns",
            "role",
            "updatedBy",
            "updatedOn",
            "user",
        ];

        /// <summary>
        /// Expected ordinally ordered shared-preference keys.
        /// </summary>
        private static readonly string[] ExpectedSharedPreferenceKeys =
        [
            "A-first",
            "a-middle",
            "z-last",
        ];

        /// <summary>
        /// Expected shared-preference values corresponding to the ordinal keys.
        /// </summary>
        private static readonly string[] ExpectedSharedPreferenceValues =
        [
            "FIRST value",
            "MiXeD value",
            "last value",
        ];

        /// <summary>
        /// Expected property set for an exact JSON reference envelope.
        /// </summary>
        private static readonly string[] ExpectedReferenceEnvelopePropertyNames =
        [
            "@id",
        ];

        /// <summary>
        /// Verifies that serializer lookup supports only exact concrete DTO
        /// runtime types and does not use interface or inheritance fallback.
        /// </summary>
        [Test]
        public void Verify_that_provider_dispatches_only_exact_concrete_runtime_types()
        {
            Assert.That(
                SerializationProvider.Provide(typeof(Comment)),
                Is.Not.Null);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(
                    () => SerializationProvider.Provide(typeof(IComment)),
                    Throws.TypeOf<NotSupportedException>());

                Assert.That(
                    () => SerializationProvider.Provide(typeof(DerivedComment)),
                    Throws.TypeOf<NotSupportedException>());
            }
        }

        /// <summary>
        /// Verifies metadata, scalar, enumeration, collection, reference, and
        /// inherited-property serialization.
        /// </summary>
        [Test]
        public void Verify_that_scalar_collection_and_inherited_properties_follow_the_JSON_contract()
        {
            var id =
                Guid.Parse("11111111-1111-1111-1111-111111111111");

            var createdBy =
                Guid.Parse("22222222-2222-2222-2222-222222222222");

            var firstReviewer =
                Guid.Parse("33333333-3333-3333-3333-333333333333");

            var secondReviewer =
                Guid.Parse("44444444-4444-4444-4444-444444444444");

            var engineeringBranchId =
                Guid.Parse("55555555-5555-5555-5555-555555555555");

            var updatedBy =
                Guid.Parse("66666666-6666-6666-6666-666666666666");

            var createdOn =
                new DateTime(
                    2026,
                    1,
                    2,
                    3,
                    4,
                    5,
                    DateTimeKind.Utc);

            var updatedOn =
                new DateTime(
                    2026,
                    2,
                    3,
                    4,
                    5,
                    6,
                    DateTimeKind.Utc);

            var dto = new BranchProtectionRule
            {
                Id = id,
                CreatedBy = createdBy,
                CreatedOn = createdOn,
                DefaultReviewers =
                [
                    firstReviewer,
                    secondReviewer,
                ],
                EngineeringBranchId = engineeringBranchId,
                MergeAllowedFor =
                [
                    ProjectMemberRole.Viewer,
                    ProjectMemberRole.Administrator,
                ],
                MinimumRequiredApproval = 2,
                Name = "main",
                ReviewRequired = true,
                UpdatedBy = updatedBy,
                UpdatedOn = updatedOn,
            };

            var bytes = Serialize(dto);

            using var document = JsonDocument.Parse(bytes);

            var root = document.RootElement;

            var propertyNames = root
                .EnumerateObject()
                .Select(property => property.Name)
                .ToArray();

            var defaultReviewers = root
                .GetProperty("defaultReviewers")
                .EnumerateArray()
                .ToArray();

            var mergeAllowedFor = root
                .GetProperty("mergeAllowedFor")
                .EnumerateArray()
                .Select(element => element.GetString())
                .ToArray();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(
                    root.ValueKind,
                    Is.EqualTo(JsonValueKind.Object));

                Assert.That(
                    propertyNames,
                    Is.EqualTo(ExpectedBranchProtectionRulePropertyNames));

                Assert.That(
                    root.GetProperty("@type").GetString(),
                    Is.EqualTo("BranchProtectionRule"));

                Assert.That(
                    root.GetProperty("@id").GetGuid(),
                    Is.EqualTo(id));

                Assert.That(
                    root.GetProperty("createdOn").GetDateTime(),
                    Is.EqualTo(createdOn));

                Assert.That(
                    root.GetProperty("engineeringBranchId").GetGuid(),
                    Is.EqualTo(engineeringBranchId));

                Assert.That(
                    mergeAllowedFor,
                    Is.EqualTo(ExpectedMergeAllowedForLiterals));

                Assert.That(
                    root.GetProperty("minimumRequiredApproval").GetInt32(),
                    Is.EqualTo(2));

                Assert.That(
                    root.GetProperty("name").GetString(),
                    Is.EqualTo("main"));

                Assert.That(
                    root.GetProperty("reviewRequired").GetBoolean(),
                    Is.True);

                Assert.That(
                    root.GetProperty("updatedOn").GetDateTime(),
                    Is.EqualTo(updatedOn));

                Assert.That(
                    defaultReviewers,
                    Has.Length.EqualTo(2));
            }

            AssertReferenceEnvelope(
                root.GetProperty("createdBy"),
                createdBy);

            AssertReferenceEnvelope(
                defaultReviewers[0],
                firstReviewer);

            AssertReferenceEnvelope(
                defaultReviewers[1],
                secondReviewer);

            AssertReferenceEnvelope(
                root.GetProperty("updatedBy"),
                updatedBy);
        }

        /// <summary>
        /// Verifies nullable reference envelopes and confirms that the
        /// POCO-only derived property is not represented in DTO JSON.
        /// </summary>
        [Test]
        public void Verify_that_nullable_references_and_derived_property_exclusion_follow_the_JSON_contract()
        {
            var activeOwnership =
                Guid.Parse("11111111-aaaa-1111-aaaa-111111111111");

            var createdBy =
                Guid.Parse("22222222-bbbb-2222-bbbb-222222222222");

            var project =
                Guid.Parse("33333333-cccc-3333-cccc-333333333333");

            var firstOwnership =
                Guid.Parse("44444444-dddd-4444-dddd-444444444444");

            var secondOwnership =
                Guid.Parse("55555555-eeee-5555-eeee-555555555555");

            var updatedBy =
                Guid.Parse("66666666-ffff-6666-ffff-666666666666");

            var user =
                Guid.Parse("77777777-aaaa-7777-aaaa-777777777777");

            var dto = new ProjectMember
            {
                Id = Guid.Parse(
                    "88888888-bbbb-8888-bbbb-888888888888"),
                ActiveOwnership = activeOwnership,
                CreatedBy = createdBy,
                CreatedOn = new DateTime(
                    2026,
                    3,
                    4,
                    5,
                    6,
                    7,
                    DateTimeKind.Utc),
                IsPartOf = project,
                Owns =
                [
                    firstOwnership,
                    secondOwnership,
                ],
                Role = ProjectMemberRole.Participant,
                UpdatedBy = updatedBy,
                UpdatedOn = new DateTime(
                    2026,
                    4,
                    5,
                    6,
                    7,
                    8,
                    DateTimeKind.Utc),
                User = user,
            };

            var bytesWithReference = Serialize(dto);

            using var documentWithReference =
                JsonDocument.Parse(bytesWithReference);

            var rootWithReference =
                documentWithReference.RootElement;

            var propertyNames = rootWithReference
                .EnumerateObject()
                .Select(property => property.Name)
                .ToArray();

            var ownerships = rootWithReference
                .GetProperty("owns")
                .EnumerateArray()
                .ToArray();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(
                    propertyNames,
                    Is.EqualTo(ExpectedProjectMemberPropertyNames));

                Assert.That(
                    rootWithReference.GetProperty("@type").GetString(),
                    Is.EqualTo("ProjectMember"));

                Assert.That(
                    rootWithReference.GetProperty("role").GetString(),
                    Is.EqualTo("PARTICIPANT"));

                Assert.That(
                    ownerships,
                    Has.Length.EqualTo(2));

                Assert.That(
                    rootWithReference.TryGetProperty(
                        "isOutsideCollaborator",
                        out _),
                    Is.False);
            }

            AssertReferenceEnvelope(
                rootWithReference.GetProperty("activeOwnership"),
                activeOwnership);

            AssertReferenceEnvelope(
                rootWithReference.GetProperty("createdBy"),
                createdBy);

            AssertReferenceEnvelope(
                rootWithReference.GetProperty("isPartOf"),
                project);

            AssertReferenceEnvelope(
                ownerships[0],
                firstOwnership);

            AssertReferenceEnvelope(
                ownerships[1],
                secondOwnership);

            AssertReferenceEnvelope(
                rootWithReference.GetProperty("updatedBy"),
                updatedBy);

            AssertReferenceEnvelope(
                rootWithReference.GetProperty("user"),
                user);

            dto.ActiveOwnership = null;

            var bytesWithoutReference = Serialize(dto);

            using var documentWithoutReference =
                JsonDocument.Parse(bytesWithoutReference);

            Assert.That(
                documentWithoutReference.RootElement
                    .GetProperty("activeOwnership")
                    .ValueKind,
                Is.EqualTo(JsonValueKind.Null));
        }

        /// <summary>
        /// Verifies deterministic output, invariant-uppercase scalar
        /// enumeration values, and ordinal dictionary member ordering.
        /// </summary>
        [Test]
        public void Verify_that_dictionary_serialization_is_ordinal_and_deterministic()
        {
            var id =
                Guid.Parse("aaaaaaaa-1111-2222-3333-444444444444");

            var dto = new FunctionalProject
            {
                Id = id,
                BelongsTo = Guid.Parse(
                    "bbbbbbbb-1111-2222-3333-444444444444"),
                CreatedBy = Guid.Parse(
                    "cccccccc-1111-2222-3333-444444444444"),
                CreatedOn = new DateTime(
                    2026,
                    5,
                    6,
                    7,
                    8,
                    9,
                    DateTimeKind.Utc),
                CurrentMode = ProjectMode.Concurrent,
                Description = "Deterministic project",
                EngineeringProjectId = Guid.Parse(
                    "dddddddd-1111-2222-3333-444444444444"),
                Lifecycle = ProjectLifecycleKind.Archived,
                Name = "Serialization contract",
                Policy = Guid.Parse(
                    "eeeeeeee-1111-2222-3333-444444444444"),
                SharedPreferences =
                    new Dictionary<string, string>
                    {
                        ["z-last"] = "last value",
                        ["a-middle"] = "MiXeD value",
                        ["A-first"] = "FIRST value",
                    },
                UpdatedBy = Guid.Parse(
                    "ffffffff-1111-2222-3333-444444444444"),
                UpdatedOn = new DateTime(
                    2026,
                    6,
                    7,
                    8,
                    9,
                    10,
                    DateTimeKind.Utc),
                Visibility = ProjectVisibility.Organization,
            };

            var firstSerialization = Serialize(dto);
            var secondSerialization = Serialize(dto);

            using var document =
                JsonDocument.Parse(firstSerialization);

            var root = document.RootElement;

            var sharedPreferences =
                root.GetProperty("sharedPreferences");

            var dictionaryEntries = sharedPreferences
                .EnumerateObject()
                .ToArray();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(
                    secondSerialization,
                    Is.EqualTo(firstSerialization),
                    "Repeated serialization of the same DTO must be byte-for-byte deterministic.");

                Assert.That(
                    root.GetProperty("@type").GetString(),
                    Is.EqualTo("FunctionalProject"));

                Assert.That(
                    root.GetProperty("@id").GetGuid(),
                    Is.EqualTo(id));

                Assert.That(
                    root.GetProperty("currentMode").GetString(),
                    Is.EqualTo("CONCURRENT"));

                Assert.That(
                    root.GetProperty("lifecycle").GetString(),
                    Is.EqualTo("ARCHIVED"));

                Assert.That(
                    root.GetProperty("visibility").GetString(),
                    Is.EqualTo("ORGANIZATION"));

                Assert.That(
                    sharedPreferences.ValueKind,
                    Is.EqualTo(JsonValueKind.Object));

                Assert.That(
                    dictionaryEntries
                        .Select(entry => entry.Name)
                        .ToArray(),
                    Is.EqualTo(ExpectedSharedPreferenceKeys));

                Assert.That(
                    dictionaryEntries
                        .Select(entry => entry.Value.GetString())
                        .ToArray(),
                    Is.EqualTo(ExpectedSharedPreferenceValues));
            }
        }

        /// <summary>
        /// Serializes a DTO through the exact-runtime-type provider.
        /// </summary>
        /// <param name="dto">
        /// The concrete DTO to serialize.
        /// </param>
        /// <returns>
        /// The generated UTF-8 JSON bytes.
        /// </returns>
        private static byte[] Serialize(object dto)
        {
            using var stream = new MemoryStream();

            using (var writer = new Utf8JsonWriter(stream))
            {
                var serializer =
                    SerializationProvider.Provide(dto.GetType());

                serializer(dto, writer);
                writer.Flush();
            }

            return stream.ToArray();
        }

        /// <summary>
        /// Verifies the exact shape and identifier of a JSON reference
        /// envelope.
        /// </summary>
        /// <param name="reference">
        /// The JSON reference envelope.
        /// </param>
        /// <param name="expectedId">
        /// The expected referenced identifier.
        /// </param>
        private static void AssertReferenceEnvelope(
            JsonElement reference,
            Guid expectedId)
        {
            Assert.That(
                reference.ValueKind,
                Is.EqualTo(JsonValueKind.Object));

            var properties = reference
                .EnumerateObject()
                .ToArray();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(
                    properties
                        .Select(property => property.Name)
                        .ToArray(),
                    Is.EqualTo(ExpectedReferenceEnvelopePropertyNames));

                Assert.That(
                    properties[0].Value.GetGuid(),
                    Is.EqualTo(expectedId));
            }
        }

        /// <summary>
        /// Test-only subtype used to verify that serialization dispatch does
        /// not fall back through inheritance.
        /// </summary>
        private sealed class DerivedComment : Comment
        {
        }
    }
}
