// ------------------------------------------------------------------------------------------------
//  <copyright file="DeSerializerTestFixture.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.Serializer.Json.Tests
{
    using System;
    using System.Text;
    using System.Text.Json;

    using Mycelium.SDK.DTO;

    /// <summary>
    /// Verifies the runtime behavior of generated JSON deserializers.
    /// </summary>
    [TestFixture]
    public class DeSerializerTestFixture
    {
        /// <summary>
        /// Verifies representative scalar, nullable, enumeration, collection,
        /// dictionary, reference and inherited-property mappings.
        /// </summary>
        [Test]
        public void Verify_that_DTO_deserialization_applies_representative_mappings()
        {
            var branchProtectionRule = DeSerialize<BranchProtectionRule>("""
                                                                         {
                                                                           "mergeAllowedFor": [
                                                                             "VIEWER",
                                                                             "ADMINISTRATOR"
                                                                           ],
                                                                           "reviewRequired": true,
                                                                           "minimumRequiredApproval": 2,
                                                                           "defaultReviewers": [
                                                                             "21000000-0000-0000-0000-000000000001",
                                                                             "21000000-0000-0000-0000-000000000002"
                                                                           ],
                                                                           "engineeringBranchId": "22000000-0000-0000-0000-000000000001",
                                                                           "@id": "20000000-0000-0000-0000-000000000001",
                                                                           "@type": "BranchProtectionRule"
                                                                         }
                                                                         """, BranchProtectionRuleDeSerializer.DeSerialize);

            var projectMember = DeSerialize<ProjectMember>("""
                                                           {
                                                             "activeOwnership": null,
                                                             "owns": [
                                                               "31000000-0000-0000-0000-000000000001"
                                                             ],
                                                             "role": "PARTICIPANT",
                                                             "user": "32000000-0000-0000-0000-000000000001",
                                                             "isOutsideCollaborator": true,
                                                             "@id": "30000000-0000-0000-0000-000000000001",
                                                             "@type": "ProjectMember"
                                                           }
                                                           """, ProjectMemberDeSerializer.DeSerialize);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(branchProtectionRule.Id, Is.EqualTo(Guid.Parse("20000000-0000-0000-0000-000000000001")));

                Assert.That(branchProtectionRule.DefaultReviewers, Is.EqualTo([
                    Guid.Parse("21000000-0000-0000-0000-000000000001"),
                    Guid.Parse("21000000-0000-0000-0000-000000000002"),
                ]));

                Assert.That(branchProtectionRule.EngineeringBranchId, Is.EqualTo(Guid.Parse("22000000-0000-0000-0000-000000000001")));

                Assert.That(branchProtectionRule.MergeAllowedFor, Is.EqualTo([
                    ProjectMemberRole.Viewer,
                    ProjectMemberRole.Administrator,
                ]));

                Assert.That(branchProtectionRule.MinimumRequiredApproval, Is.EqualTo(2));

                Assert.That(branchProtectionRule.ReviewRequired, Is.True);

                Assert.That(projectMember.Id, Is.EqualTo(Guid.Parse("30000000-0000-0000-0000-000000000001")));

                Assert.That(projectMember.ActiveOwnership, Is.Null);

                Assert.That(projectMember.Owns, Is.EqualTo([
                    Guid.Parse("31000000-0000-0000-0000-000000000001"),
                ]));

                Assert.That(projectMember.Role, Is.EqualTo(ProjectMemberRole.Participant));

                Assert.That(projectMember.User, Is.EqualTo(Guid.Parse("32000000-0000-0000-0000-000000000001")));
            }
        }

        /// <summary>
        /// Verifies mandatory, valid and unique metadata.
        /// </summary>
        [Test]
        public void Verify_that_invalid_metadata_is_rejected()
        {
            var missingType = """
                              {
                                "@id": "50000000-0000-0000-0000-000000000001"
                              }
                              """;

            var missingId = """
                            {
                              "@type": "Comment"
                            }
                            """;

            var nullType = """
                           {
                             "@type": null,
                             "@id": "50000000-0000-0000-0000-000000000001"
                           }
                           """;

            var incorrectType = """
                                {
                                  "@type": "comment",
                                  "@id": "50000000-0000-0000-0000-000000000001"
                                }
                                """;

            var duplicateType = """
                                {
                                  "@type": "Comment",
                                  "@type": "Comment",
                                  "@id": "50000000-0000-0000-0000-000000000001"
                                }
                                """;

            var duplicateId = """
                              {
                                "@type": "Comment",
                                "@id": "50000000-0000-0000-0000-000000000001",
                                "@id": "50000000-0000-0000-0000-000000000002"
                              }
                              """;

            var invalidId = """
                            {
                              "@type": "Comment",
                              "@id": "not-a-guid"
                            }
                            """;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(() => DeSerialize<Comment>(missingType, CommentDeSerializer.DeSerialize), Throws.TypeOf<JsonException>());

                Assert.That(() => DeSerialize<Comment>(missingId, CommentDeSerializer.DeSerialize), Throws.TypeOf<JsonException>());

                Assert.That(() => DeSerialize<Comment>(nullType, CommentDeSerializer.DeSerialize), Throws.TypeOf<JsonException>());

                Assert.That(() => DeSerialize<Comment>(incorrectType, CommentDeSerializer.DeSerialize), Throws.TypeOf<NotSupportedException>());

                Assert.That(() => DeSerialize<Comment>(duplicateType, CommentDeSerializer.DeSerialize), Throws.TypeOf<JsonException>());

                Assert.That(() => DeSerialize<Comment>(duplicateId, CommentDeSerializer.DeSerialize), Throws.TypeOf<JsonException>());

                Assert.That(() => DeSerialize<Comment>(invalidId, CommentDeSerializer.DeSerialize), Throws.TypeOf<JsonException>());
            }
        }

        /// <summary>
        /// Verifies representative invalid property and dictionary values.
        /// </summary>
        [Test]
        public void Verify_that_invalid_property_values_are_rejected()
        {
            var invalidJsonValues = new[]
            {
                """
                {
                  "@type": "FunctionalProject",
                  "@id": "60000000-0000-0000-0000-000000000001",
                  "belongsTo": "not-a-guid"
                }
                """,
                """
                {
                  "@type": "FunctionalProject",
                  "@id": "60000000-0000-0000-0000-000000000001",
                  "branchRules": {}
                }
                """,
                """
                {
                  "@type": "FunctionalProject",
                  "@id": "60000000-0000-0000-0000-000000000001",
                  "branchRules": [
                    null
                  ]
                }
                """,
                """
                {
                  "@type": "FunctionalProject",
                  "@id": "60000000-0000-0000-0000-000000000001",
                  "currentMode": "concurrent"
                }
                """,
                """
                {
                  "@type": "FunctionalProject",
                  "@id": "60000000-0000-0000-0000-000000000001",
                  "name": null
                }
                """,
                """
                {
                  "@type": "FunctionalProject",
                  "@id": "60000000-0000-0000-0000-000000000001",
                  "sharedPreferences": []
                }
                """,
                """
                {
                  "@type": "FunctionalProject",
                  "@id": "60000000-0000-0000-0000-000000000001",
                  "sharedPreferences": {
                    "key": null
                  }
                }
                """,
                """
                {
                  "@type": "FunctionalProject",
                  "@id": "60000000-0000-0000-0000-000000000001",
                  "sharedPreferences": {
                    "key": 1
                  }
                }
                """,
                """
                {
                  "@type": "FunctionalProject",
                  "@id": "60000000-0000-0000-0000-000000000001",
                  "sharedPreferences": {
                    "key": "first",
                    "key": "second"
                  }
                }
                """,
                """
                {
                  "@type": "FunctionalProject",
                  "@id": "60000000-0000-0000-0000-000000000001",
                  "branchRules": [
                    "61000000-0000-0000-0000-000000000001"
                  ]
                """,
            };

            using (Assert.EnterMultipleScope())
            {
                foreach (var json in invalidJsonValues)
                {
                    Assert.That(() => DeSerialize<FunctionalProject>(json, FunctionalProjectDeSerializer.DeSerialize), Throws.InstanceOf<JsonException>(), json);
                }
            }
        }

        /// <summary>
        /// Verifies order-independent reading, case-sensitive property matching,
        /// ignored unknown properties and construction defaults.
        /// </summary>
        [Test]
        public void Verify_that_property_reading_is_order_independent_case_sensitive_and_preserves_defaults()
        {
            var project = DeSerialize<FunctionalProject>("""
                                                         {
                                                           "name": "Expected name",
                                                           "Name": "Ignored because casing differs",
                                                           "unknown": {
                                                             "nested": [
                                                               1,
                                                               true,
                                                               {
                                                                 "value": "ignored"
                                                               }
                                                             ]
                                                           },
                                                           "sharedPreferences": {
                                                             "Key": "FIRST",
                                                             "key": "second"
                                                           },
                                                           "branchRules": [
                                                             "41000000-0000-0000-0000-000000000001",
                                                             "41000000-0000-0000-0000-000000000002"
                                                           ],
                                                           "currentMode": "CONCURRENT",
                                                           "lifecycle": "OPEN",
                                                           "visibility": "ORGANIZATION",
                                                           "description": "Runtime contract",
                                                           "belongsTo": "42000000-0000-0000-0000-000000000001",
                                                           "engineeringProjectId": "43000000-0000-0000-0000-000000000001",
                                                           "policy": "44000000-0000-0000-0000-000000000001",
                                                           "createdBy": "45000000-0000-0000-0000-000000000001",
                                                           "createdOn": "2026-05-06T07:08:09.0000000Z",
                                                           "@id": "40000000-0000-0000-0000-000000000001",
                                                           "@type": "FunctionalProject"
                                                         }
                                                         """, FunctionalProjectDeSerializer.DeSerialize);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(project.Id, Is.EqualTo(Guid.Parse("40000000-0000-0000-0000-000000000001")));

                Assert.That(project.Name, Is.EqualTo("Expected name"));

                Assert.That(project.Description, Is.EqualTo("Runtime contract"));

                Assert.That(project.BelongsTo, Is.EqualTo(Guid.Parse("42000000-0000-0000-0000-000000000001")));

                Assert.That(project.EngineeringProjectId, Is.EqualTo(Guid.Parse("43000000-0000-0000-0000-000000000001")));

                Assert.That(project.Policy, Is.EqualTo(Guid.Parse("44000000-0000-0000-0000-000000000001")));

                Assert.That(project.CreatedBy, Is.EqualTo(Guid.Parse("45000000-0000-0000-0000-000000000001")));

                Assert.That(project.CreatedOn, Is.EqualTo(new DateTime(2026, 5, 6, 7, 8, 9, DateTimeKind.Utc)));

                Assert.That(project.CurrentMode, Is.EqualTo(ProjectMode.Concurrent));

                Assert.That(project.Lifecycle, Is.EqualTo(ProjectLifecycleKind.Open));

                Assert.That(project.Visibility, Is.EqualTo(ProjectVisibility.Organization));

                Assert.That(project.BranchRules, Is.EqualTo([
                    Guid.Parse("41000000-0000-0000-0000-000000000001"),
                    Guid.Parse("41000000-0000-0000-0000-000000000002"),
                ]));

                Assert.That(project.SharedPreferences, Has.Count.EqualTo(2));

                Assert.That(project.SharedPreferences.Comparer, Is.SameAs(StringComparer.Ordinal));

                Assert.That(project.SharedPreferences["Key"], Is.EqualTo("FIRST"));

                Assert.That(project.SharedPreferences["key"], Is.EqualTo("second"));

                Assert.That(project.Defines, Is.Empty);

                Assert.That(project.Involves, Is.Empty);

                Assert.That(project.Reviews, Is.Empty);

                Assert.That(project.UpdatedBy, Is.EqualTo(Guid.Empty));

                Assert.That(project.UpdatedOn, Is.EqualTo(default(DateTime)));
            }
        }

        /// <summary>
        /// Verifies exact provider dispatch and exact-uppercase enumeration parsing.
        /// </summary>
        [Test]
        public void Verify_that_provider_and_enumeration_matching_is_exact()
        {
            var comment = DeSerialize<Comment>("""
                                               {
                                                 "@type": "Comment",
                                                 "@id": "10000000-0000-0000-0000-000000000001"
                                               }
                                               """, DeSerializationProvider.Provide("Comment"));

            using (Assert.EnterMultipleScope())
            {
                Assert.That(comment.Id, Is.EqualTo(Guid.Parse("10000000-0000-0000-0000-000000000001")));

                Assert.That(Read("\"ADMINISTRATOR\"", ProjectMemberRoleDeSerializer.DeSerialize), Is.EqualTo(ProjectMemberRole.Administrator));

                Assert.That(Read("null", ProjectMemberRoleDeSerializer.DeSerializeOrNull), Is.Null);

                Assert.That(() => DeSerializationProvider.Provide(null), Throws.TypeOf<ArgumentNullException>());

                Assert.That(() => DeSerializationProvider.Provide("comment"), Throws.TypeOf<NotSupportedException>());

                Assert.That(() => DeSerializationProvider.Provide("Unknown"), Throws.TypeOf<NotSupportedException>());

                Assert.That(() => Read("\"administrator\"", ProjectMemberRoleDeSerializer.DeSerialize), Throws.TypeOf<JsonException>());

                Assert.That(() => Read("\"UNKNOWN\"", ProjectMemberRoleDeSerializer.DeSerialize), Throws.TypeOf<JsonException>());

                Assert.That(() => Read("null", ProjectMemberRoleDeSerializer.DeSerialize), Throws.TypeOf<JsonException>());

                Assert.That(() => Read("1", ProjectMemberRoleDeSerializer.DeSerialize), Throws.TypeOf<JsonException>());
            }
        }

        /// <summary>
        /// Represents an operation that reads a value from a positioned JSON reader.
        /// </summary>
        private delegate T ReaderOperation<out T>(ref Utf8JsonReader reader);

        /// <summary>
        /// Reads one JSON value using the supplied operation.
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
        /// Deserializes one complete DTO object using a generated operation.
        /// </summary>
        private static T DeSerialize<T>(string json, DeSerializationProvider.DeSerializerAction operation) where T : IThing
        {
            var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(json));

            if (!reader.Read())
            {
                throw new InvalidOperationException("The test JSON contains no value.");
            }

            var result = operation(ref reader);

            Assert.That(reader.TokenType, Is.EqualTo(JsonTokenType.EndObject));

            return (T)result;
        }
    }
}
