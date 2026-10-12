// ------------------------------------------------------------------------------------------------
//  <copyright file="DtoValidatorTestFixture.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.Validation.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using FluentValidation;

    using Mycelium.SDK.DTO;

    [TestFixture]
    public class DtoValidatorTestFixture
    {
        private static readonly Guid ReferenceId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        [Test]
        public void VerifyThatValidPrimitiveDefaultsAreAccepted()
        {
            var dto = new BranchProtectionRule
            {
                CreatedBy = ReferenceId,
                UpdatedBy = ReferenceId,
                Name = "Main",
                MergeAllowedFor = [default]
            };

            var result = new BranchProtectionRuleValidator().Validate(dto);

            Assert.That(result.IsValid, Is.True);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void VerifyThatRequiredStringsRejectAbsentOrBlankValuesWithoutChangingThem(string content)
        {
            var dto = CreateComment();
            dto.Content = content;

            var result = new CommentValidator().Validate(dto);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Errors.Select(failure => failure.PropertyName), Is.EqualTo(new[] { nameof(Comment.Content) }));
                Assert.That(dto.Content, Is.EqualTo(content));
            }
        }

        [Test]
        public void VerifyThatReferencesRejectEmptyValuesAndIdentifyCollectionItems()
        {
            IComment dto = CreateComment();
            IValidator<IComment> validator = new CommentValidator();

            Assert.That(validator.Validate(dto).IsValid, Is.True);

            dto.Quotes = ReferenceId;
            dto.Replies = [ReferenceId];

            Assert.That(validator.Validate(dto).IsValid, Is.True);

            dto.Author = Guid.Empty;
            dto.CreatedBy = Guid.Empty;
            dto.UpdatedBy = Guid.Empty;
            dto.Quotes = Guid.Empty;
            dto.Replies = [ReferenceId, Guid.Empty];

            var result = validator.Validate(dto);

            Assert.That(result.Errors.Select(failure => failure.PropertyName), Is.EquivalentTo(new[]
            {
                nameof(Comment.Author),
                nameof(Comment.CreatedBy),
                nameof(Comment.UpdatedBy),
                nameof(Comment.Quotes),
                $"{nameof(Comment.Replies)}[1]"
            }));
        }

        [Test]
        public void VerifyThatCollectionMultiplicityTreatsNullAsZeroWithoutChangingTheDto()
        {
            var dto = new Organization
            {
                CreatedBy = ReferenceId,
                UpdatedBy = ReferenceId,
                Policy = ReferenceId,
                Name = "Organization",
                Description = "Representative organization",
                InvolvedUser = null,
                Projects = null
            };

            var validator = new OrganizationValidator();
            var expectedProperties = new[] { nameof(Organization.InvolvedUser) };
            var result = validator.Validate(dto);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Errors.Select(failure => failure.PropertyName), Is.EqualTo(expectedProperties));
                Assert.That(dto.InvolvedUser, Is.Null);
                Assert.That(dto.Projects, Is.Null);
            }

            dto.InvolvedUser = [];

            Assert.That(validator.Validate(dto).Errors.Select(failure => failure.PropertyName), Is.EqualTo(expectedProperties));

            dto.InvolvedUser = [ReferenceId];

            Assert.That(validator.Validate(dto).IsValid, Is.True);
        }

        [Test]
        public void VerifyThatDictionaryValidationChecksPresenceRatherThanEntryCount()
        {
            var dto = new FunctionalProject
            {
                BelongsTo = ReferenceId,
                CreatedBy = ReferenceId,
                UpdatedBy = ReferenceId,
                Policy = ReferenceId,
                Name = "Project",
                Description = "Representative project",
                SharedPreferences = null
            };

            var validator = new FunctionalProjectValidator();
            var result = validator.Validate(dto);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Errors.Select(failure => failure.PropertyName), Is.EqualTo(new[] { nameof(FunctionalProject.SharedPreferences) }));
                Assert.That(dto.SharedPreferences, Is.Null);
            }

            dto.SharedPreferences = [];

            Assert.That(validator.Validate(dto).IsValid, Is.True);

            dto.SharedPreferences = new Dictionary<string, string> { ["theme"] = "dark", ["language"] = "en" };

            Assert.That(validator.Validate(dto).IsValid, Is.True);
        }

        [Test]
        public void VerifyThatActiveOwnershipMustBelongToOwns()
        {
            var dto = CreateProjectMember();
            dto.Owns = null;

            var validator = new ProjectMemberValidator();
            var activeOwnership = Guid.Parse("22222222-2222-2222-2222-222222222222");
            var expectedProperties = new[] { nameof(ProjectMember.ActiveOwnership) };

            Assert.That(validator.Validate(dto).IsValid, Is.True);

            dto.ActiveOwnership = activeOwnership;
            dto.Owns = [ReferenceId, activeOwnership];

            Assert.That(validator.Validate(dto).IsValid, Is.True);

            dto.Owns = [ReferenceId];

            Assert.That(validator.Validate(dto).Errors.Select(failure => failure.PropertyName), Is.EqualTo(expectedProperties));

            dto.Owns = [];

            Assert.That(validator.Validate(dto).Errors.Select(failure => failure.PropertyName), Is.EqualTo(expectedProperties));

            dto.Owns = null;

            var result = validator.Validate(dto);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Errors.Select(failure => failure.PropertyName), Is.EqualTo(expectedProperties));
                Assert.That(dto.ActiveOwnership, Is.EqualTo(activeOwnership));
                Assert.That(dto.Owns, Is.Null);
            }
        }

        [Test]
        public void VerifyThatHandwrittenRulesRunAlongsideGeneratedRules()
        {
            IProjectMember dto = CreateProjectMember();
            dto.ActiveOwnership = ReferenceId;
            dto.Owns = [Guid.Empty];

            IValidator<IProjectMember> validator = new ProjectMemberValidator();
            var result = validator.Validate(dto);

            Assert.That(result.Errors.Select(failure => failure.PropertyName), Is.EquivalentTo(new[]
            {
                nameof(ProjectMember.ActiveOwnership),
                $"{nameof(ProjectMember.Owns)}[0]"
            }));

            dto.Owns = [Guid.Empty, ReferenceId];

            Assert.That(validator.Validate(dto).Errors.Select(failure => failure.PropertyName), Is.EqualTo(new[] { $"{nameof(ProjectMember.Owns)}[0]" }));

            dto.ActiveOwnership = Guid.Empty;

            Assert.That(validator.Validate(dto).Errors.Select(failure => failure.PropertyName), Is.EquivalentTo(new[]
            {
                nameof(ProjectMember.ActiveOwnership),
                $"{nameof(ProjectMember.Owns)}[0]"
            }));
        }

        private static Comment CreateComment()
        {
            return new Comment
            {
                Author = ReferenceId,
                CreatedBy = ReferenceId,
                UpdatedBy = ReferenceId,
                Content = "Representative comment"
            };
        }

        private static ProjectMember CreateProjectMember()
        {
            return new ProjectMember
            {
                CreatedBy = ReferenceId,
                UpdatedBy = ReferenceId,
                IsPartOf = ReferenceId,
                User = ReferenceId
            };
        }
    }
}
