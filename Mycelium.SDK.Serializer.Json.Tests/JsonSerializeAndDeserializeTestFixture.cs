// ------------------------------------------------------------------------------------------------
//  <copyright file="JsonSerializeAndDeserializeTestFixture.cs" company="Starion Group S.A.">
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

    using Mycelium.SDK.DTO;

    /// <summary>
    /// Verifies representative semantic round trips through the public JSON facades.
    /// </summary>
    [TestFixture]
    public class JsonSerializeAndDeserializeTestFixture
    {
        private static readonly DeSerializer JsonDeSerializer = new();

        private static readonly Serializer JsonSerializer = new();

        /// <summary>
        /// Verifies representative DTO-to-JSON-to-DTO semantic round trips for object and sequence
        /// payloads.
        /// </summary>
        [Test]
        public void Verify_that_JSON_facades_round_trip_representative_DTO_payloads()
        {
            var reply = Guid.Parse("81000000-0000-0000-0000-000000000001");

            var comment = new Comment
            {
                Id = Guid.Parse("80000000-0000-0000-0000-000000000001"),
                Author = Guid.Parse("80000000-0000-0000-0000-000000000002"),
                CommentStatus = CommentStatus.Open,
                Content = "Semantic round trip",
                CreatedBy = Guid.Parse("80000000-0000-0000-0000-000000000003"),
                CreatedOn = new DateTime(2026, 9, 10, 11, 12, 13, DateTimeKind.Utc),
                Quotes = null,
                Replies =
                [
                    reply,
                ],
                TargetElementId = Guid.Parse("80000000-0000-0000-0000-000000000004"),
                UpdatedBy = Guid.Parse("80000000-0000-0000-0000-000000000005"),
                UpdatedOn = new DateTime(2026, 9, 11, 12, 13, 14, DateTimeKind.Utc),
            };

            var ownership = Guid.Parse("82000000-0000-0000-0000-000000000001");

            var projectMember = new ProjectMember
            {
                Id = Guid.Parse("82000000-0000-0000-0000-000000000002"),
                ActiveOwnership = ownership,
                CreatedBy = Guid.Parse("82000000-0000-0000-0000-000000000003"),
                CreatedOn = new DateTime(2026, 8, 9, 10, 11, 12, DateTimeKind.Utc),
                IsPartOf = Guid.Parse("82000000-0000-0000-0000-000000000004"),
                Owns =
                [
                    ownership,
                ],
                Role = ProjectMemberRole.Participant,
                UpdatedBy = Guid.Parse("82000000-0000-0000-0000-000000000005"),
                UpdatedOn = new DateTime(2026, 8, 10, 11, 12, 13, DateTimeKind.Utc),
                User = Guid.Parse("82000000-0000-0000-0000-000000000006"),
            };

            var functionalProject = new FunctionalProject
            {
                Id = Guid.Parse("83000000-0000-0000-0000-000000000001"),
                BelongsTo = Guid.Parse("83000000-0000-0000-0000-000000000002"),
                BranchRules =
                [
                    Guid.Parse("83000000-0000-0000-0000-000000000003"),
                ],
                CreatedBy = Guid.Parse("83000000-0000-0000-0000-000000000004"),
                CreatedOn = new DateTime(2026, 7, 8, 9, 10, 11, DateTimeKind.Utc),
                CurrentMode = ProjectMode.Concurrent,
                Defines =
                [
                    Guid.Parse("83000000-0000-0000-0000-000000000005"),
                ],
                Description = "Representative project",
                EngineeringProjectId = Guid.Parse("83000000-0000-0000-0000-000000000006"),
                Involves =
                [
                    projectMember.Id,
                ],
                Lifecycle = ProjectLifecycleKind.Open,
                Name = "Dictionary round trip",
                Policy = Guid.Parse("83000000-0000-0000-0000-000000000007"),
                Reviews =
                [
                    Guid.Parse("83000000-0000-0000-0000-000000000008"),
                ],
                SharedPreferences = new Dictionary<string, string>(StringComparer.Ordinal) { ["Theme"] = "LIGHT", ["theme"] = "dark", },
                UpdatedBy = Guid.Parse("83000000-0000-0000-0000-000000000009"),
                UpdatedOn = new DateTime(2026, 7, 9, 10, 11, 12, DateTimeKind.Utc),
                Visibility = ProjectVisibility.Organization,
            };

            using var objectStream = new MemoryStream();

            JsonSerializer.Serialize(comment, objectStream, default);

            objectStream.Position = 0;

            var objectRoundTrip = (Comment)JsonDeSerializer.DeSerialize(objectStream)
                .Single();

            using var sequenceStream = new MemoryStream();

            JsonSerializer.Serialize(new IThing[] { comment, projectMember, functionalProject, }, sequenceStream, default);

            sequenceStream.Position = 0;

            var sequenceRoundTrip = JsonDeSerializer.DeSerialize(sequenceStream)
                .ToArray();

            var functionalProjectRoundTrip = (FunctionalProject)sequenceRoundTrip[2];

            using (Assert.EnterMultipleScope())
            {
                Assert.That(objectRoundTrip, Is.EqualTo(comment)
                    .UsingPropertiesComparer());

                Assert.That(sequenceRoundTrip, Has.Length.EqualTo(3));

                Assert.That(sequenceRoundTrip[0], Is.EqualTo(comment)
                    .UsingPropertiesComparer());

                Assert.That(sequenceRoundTrip[1], Is.EqualTo(projectMember)
                    .UsingPropertiesComparer());

                Assert.That(functionalProjectRoundTrip, Is.EqualTo(functionalProject)
                    .UsingPropertiesComparer(configuration => configuration.Excluding(nameof(FunctionalProject.SharedPreferences))));

                Assert.That(functionalProjectRoundTrip.SharedPreferences, Is.EquivalentTo(functionalProject.SharedPreferences));

                Assert.That(objectStream.CanRead, Is.True);

                Assert.That(sequenceStream.CanRead, Is.True);
            }
        }
    }
}
