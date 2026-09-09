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
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    using Mycelium.SDK.DTO;
    using Mycelium.SDK.Serializer.Json;

    [TestFixture]
    public class SerializerTestFixture
    {
        private static readonly Serializer JsonSerializer = new();

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

        private static readonly string[] ExpectedMergeAllowedForLiterals =
        [
            "VIEWER",
            "ADMINISTRATOR",
        ];

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

        [Test]
        public async Task Verify_that_facade_roots_options_flushing_stream_ownership_and_async_parity_follow_the_contract()
        {
            var firstDto = CreateComment(
                Guid.Parse("10000000-0000-0000-0000-000000000001"));

            var secondDto = CreateComment(
                Guid.Parse("10000000-0000-0000-0000-000000000002"));

            var dtos = new IThing[]
            {
                firstDto,
                secondDto,
            };

            var writerOptions = new JsonWriterOptions
            {
                Indented = true,
            };

            using var synchronousDtoStream =
                new RecordingMemoryStream();

            JsonSerializer.Serialize(
                firstDto,
                synchronousDtoStream,
                writerOptions);

            using var asynchronousDtoStream =
                new RecordingMemoryStream();

            await JsonSerializer.SerializeAsync(
                firstDto,
                asynchronousDtoStream,
                writerOptions,
                CancellationToken.None);

            using var synchronousSequenceStream =
                new RecordingMemoryStream();

            JsonSerializer.Serialize(
                dtos,
                synchronousSequenceStream,
                writerOptions);

            using var asynchronousSequenceStream =
                new RecordingMemoryStream();

            await JsonSerializer.SerializeAsync(
                dtos,
                asynchronousSequenceStream,
                writerOptions,
                CancellationToken.None);

            var synchronousDtoBytes =
                synchronousDtoStream.ToArray();

            var asynchronousDtoBytes =
                asynchronousDtoStream.ToArray();

            var synchronousSequenceBytes =
                synchronousSequenceStream.ToArray();

            var asynchronousSequenceBytes =
                asynchronousSequenceStream.ToArray();

            using var dtoDocument =
                JsonDocument.Parse(synchronousDtoBytes);

            using var sequenceDocument =
                JsonDocument.Parse(synchronousSequenceBytes);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(
                    dtoDocument.RootElement.ValueKind,
                    Is.EqualTo(JsonValueKind.Object));

                Assert.That(
                    sequenceDocument.RootElement.ValueKind,
                    Is.EqualTo(JsonValueKind.Array));

                Assert.That(
                    sequenceDocument.RootElement.GetArrayLength(),
                    Is.EqualTo(2));

                Assert.That(
                    asynchronousDtoBytes,
                    Is.EqualTo(synchronousDtoBytes));

                Assert.That(
                    asynchronousSequenceBytes,
                    Is.EqualTo(synchronousSequenceBytes));

                Assert.That(
                    Encoding.UTF8.GetString(synchronousDtoBytes),
                    Does.Contain("\n"));

                Assert.That(
                    Encoding.UTF8.GetString(synchronousSequenceBytes),
                    Does.Contain("\n"));

                Assert.That(
                    synchronousDtoStream.WasFlushedSynchronously,
                    Is.True);

                Assert.That(
                    synchronousSequenceStream.WasFlushedSynchronously,
                    Is.True);

                Assert.That(
                    asynchronousDtoStream.WasFlushedAsynchronously,
                    Is.True);

                Assert.That(
                    asynchronousSequenceStream.WasFlushedAsynchronously,
                    Is.True);

                Assert.That(
                    synchronousDtoStream.CanWrite,
                    Is.True);

                Assert.That(
                    asynchronousDtoStream.CanWrite,
                    Is.True);

                Assert.That(
                    synchronousSequenceStream.CanWrite,
                    Is.True);

                Assert.That(
                    asynchronousSequenceStream.CanWrite,
                    Is.True);
            }
        }

        [Test]
        public async Task Verify_that_facade_rejects_null_and_unsupported_values()
        {
            var supportedDto = CreateComment(
                Guid.Parse("20000000-0000-0000-0000-000000000001"));

            var unsupportedDto = new DerivedComment();

            var writerOptions =
                default(JsonWriterOptions);

            using var synchronousArgumentStream =
                new MemoryStream();

            Assert.That(
                () => JsonSerializer.Serialize(
                    (IThing)null,
                    synchronousArgumentStream,
                    writerOptions),
                Throws.TypeOf<ArgumentNullException>());

            Assert.That(
                () => JsonSerializer.Serialize(
                    (IEnumerable<IThing>)null,
                    synchronousArgumentStream,
                    writerOptions),
                Throws.TypeOf<ArgumentNullException>());

            Assert.That(
                () => JsonSerializer.Serialize(
                    supportedDto,
                    null,
                    writerOptions),
                Throws.TypeOf<ArgumentNullException>());

            Assert.That(
                () => JsonSerializer.Serialize(
                    new IThing[]
                    {
                        supportedDto,
                    },
                    null,
                    writerOptions),
                Throws.TypeOf<ArgumentNullException>());

            using var synchronousNullElementStream =
                new MemoryStream();

            Assert.That(
                () => JsonSerializer.Serialize(
                    new IThing[]
                    {
                        supportedDto,
                        null,
                    },
                    synchronousNullElementStream,
                    writerOptions),
                Throws.TypeOf<ArgumentNullException>());

            using var synchronousUnsupportedStream =
                new MemoryStream();

            Assert.That(
                () => JsonSerializer.Serialize(
                    unsupportedDto,
                    synchronousUnsupportedStream,
                    writerOptions),
                Throws.TypeOf<NotSupportedException>());

            Assert.That(
                () => JsonSerializer.Serialize(
                    new IThing[]
                    {
                        supportedDto,
                        unsupportedDto,
                    },
                    synchronousUnsupportedStream,
                    writerOptions),
                Throws.TypeOf<NotSupportedException>());

            using var asynchronousArgumentStream =
                new MemoryStream();

            await Assert.ThatAsync(
                () => JsonSerializer.SerializeAsync(
                    (IThing)null,
                    asynchronousArgumentStream,
                    writerOptions,
                    CancellationToken.None),
                Throws.TypeOf<ArgumentNullException>());

            await Assert.ThatAsync(
                () => JsonSerializer.SerializeAsync(
                    (IEnumerable<IThing>)null,
                    asynchronousArgumentStream,
                    writerOptions,
                    CancellationToken.None),
                Throws.TypeOf<ArgumentNullException>());

            await Assert.ThatAsync(
                () => JsonSerializer.SerializeAsync(
                    supportedDto,
                    null,
                    writerOptions,
                    CancellationToken.None),
                Throws.TypeOf<ArgumentNullException>());

            await Assert.ThatAsync(
                () => JsonSerializer.SerializeAsync(
                    new IThing[]
                    {
                        supportedDto,
                    },
                    null,
                    writerOptions,
                    CancellationToken.None),
                Throws.TypeOf<ArgumentNullException>());

            using var asynchronousNullElementStream =
                new MemoryStream();

            await Assert.ThatAsync(
                () => JsonSerializer.SerializeAsync(
                    new IThing[]
                    {
                        supportedDto,
                        null,
                    },
                    asynchronousNullElementStream,
                    writerOptions,
                    CancellationToken.None),
                Throws.TypeOf<ArgumentNullException>());

            using var asynchronousUnsupportedStream =
                new MemoryStream();

            await Assert.ThatAsync(
                () => JsonSerializer.SerializeAsync(
                    unsupportedDto,
                    asynchronousUnsupportedStream,
                    writerOptions,
                    CancellationToken.None),
                Throws.TypeOf<NotSupportedException>());

            await Assert.ThatAsync(
                () => JsonSerializer.SerializeAsync(
                    new IThing[]
                    {
                        supportedDto,
                        unsupportedDto,
                    },
                    asynchronousUnsupportedStream,
                    writerOptions,
                    CancellationToken.None),
                Throws.TypeOf<NotSupportedException>());
        }

        [Test]
        public async Task Verify_that_async_facade_observes_cancellation()
        {
            var dto = CreateComment(
                Guid.Parse("30000000-0000-0000-0000-000000000001"));

            using var cancellationTokenSource =
                new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            using var dtoStream =
                new MemoryStream();

            await Assert.ThatAsync(
                () => JsonSerializer.SerializeAsync(
                    dto,
                    dtoStream,
                    default,
                    cancellationTokenSource.Token),
                Throws.TypeOf<OperationCanceledException>());

            using var sequenceStream =
                new MemoryStream();

            await Assert.ThatAsync(
                () => JsonSerializer.SerializeAsync(
                    new IThing[]
                    {
                        dto,
                    },
                    sequenceStream,
                    default,
                    cancellationTokenSource.Token),
                Throws.TypeOf<OperationCanceledException>());
        }

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
                    root.GetProperty("createdOn").GetString(),
                    Is.EqualTo(
                        createdOn.ToString(
                            "o",
                            CultureInfo.InvariantCulture)));

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
                    root.GetProperty("updatedOn").GetString(),
                    Is.EqualTo(
                        updatedOn.ToString(
                            "o",
                            CultureInfo.InvariantCulture)));

                Assert.That(
                    defaultReviewers,
                    Has.Length.EqualTo(2));
            }

            AssertReferenceValue(
                root.GetProperty("createdBy"),
                createdBy);

            AssertReferenceValue(
                defaultReviewers[0],
                firstReviewer);

            AssertReferenceValue(
                defaultReviewers[1],
                secondReviewer);

            AssertReferenceValue(
                root.GetProperty("updatedBy"),
                updatedBy);
        }

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

            AssertReferenceValue(
                rootWithReference.GetProperty("activeOwnership"),
                activeOwnership);

            AssertReferenceValue(
                rootWithReference.GetProperty("createdBy"),
                createdBy);

            AssertReferenceValue(
                rootWithReference.GetProperty("isPartOf"),
                project);

            AssertReferenceValue(
                ownerships[0],
                firstOwnership);

            AssertReferenceValue(
                ownerships[1],
                secondOwnership);

            AssertReferenceValue(
                rootWithReference.GetProperty("updatedBy"),
                updatedBy);

            AssertReferenceValue(
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

        [Test]
        public void Verify_that_dictionary_serialization_preserves_entries_without_imposing_order()
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

            var serialization = Serialize(dto);

            using var document =
                JsonDocument.Parse(serialization);

            var root = document.RootElement;

            var sharedPreferences =
                root.GetProperty("sharedPreferences");

            var dictionaryEntries = sharedPreferences
                .EnumerateObject()
                .ToDictionary(
                    entry => entry.Name,
                    entry => entry.Value.GetString(),
                    StringComparer.Ordinal);

            using (Assert.EnterMultipleScope())
            {
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
                    dictionaryEntries,
                    Is.EquivalentTo(dto.SharedPreferences));
            }
        }

        private static byte[] Serialize(IThing dto)
        {
            using var stream = new MemoryStream();

            JsonSerializer.Serialize(
                dto,
                stream,
                default);

            return stream.ToArray();
        }

        private static Comment CreateComment(Guid id)
        {
            return new Comment
            {
                Id = id,
                Author =
                    Guid.Parse("40000000-0000-0000-0000-000000000001"),
                CommentStatus = CommentStatus.Open,
                Content = "Facade serialization",
                CreatedBy =
                    Guid.Parse("40000000-0000-0000-0000-000000000002"),
                CreatedOn =
                    new DateTime(
                        2026,
                        7,
                        8,
                        9,
                        10,
                        11,
                        DateTimeKind.Utc),
                Quotes = null,
                Replies =
                [
                    Guid.Parse("40000000-0000-0000-0000-000000000003"),
                ],
                TargetElementId =
                    Guid.Parse("40000000-0000-0000-0000-000000000004"),
                UpdatedBy =
                    Guid.Parse("40000000-0000-0000-0000-000000000005"),
                UpdatedOn =
                    new DateTime(
                        2026,
                        8,
                        9,
                        10,
                        11,
                        12,
                        DateTimeKind.Utc),
            };
        }

        private static void AssertReferenceValue(
            JsonElement reference,
            Guid expectedId)
        {
            Assert.That(
                reference.ValueKind,
                Is.EqualTo(JsonValueKind.String));

            Assert.That(
                reference.GetGuid(),
                Is.EqualTo(expectedId));
        }

        private sealed class RecordingMemoryStream : MemoryStream
        {
            public bool WasFlushedSynchronously { get; private set; }

            public bool WasFlushedAsynchronously { get; private set; }

            public override void Flush()
            {
                this.WasFlushedSynchronously = true;
                base.Flush();
            }

            public override Task FlushAsync(
                CancellationToken cancellationToken)
            {
                this.WasFlushedAsynchronously = true;

                return base.FlushAsync(cancellationToken);
            }
        }

        private sealed class DerivedComment : Comment
        {
        }
    }
}
