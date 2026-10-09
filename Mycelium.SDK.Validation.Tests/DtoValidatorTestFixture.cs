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
            var dto = CreateComment();
            var validator = new CommentValidator();

            Assert.That(validator.Validate(dto).IsValid, Is.True);

            dto.Quotes = ReferenceId;
            dto.Replies = [ReferenceId];

            Assert.That(validator.Validate(dto).IsValid, Is.True);

            dto.Author = Guid.Empty;
            dto.CreatedBy = Guid.Empty;
            dto.Quotes = Guid.Empty;
            dto.Replies = [ReferenceId, Guid.Empty];

            var result = validator.Validate(dto);

            Assert.That(result.Errors.Select(failure => failure.PropertyName), Is.EquivalentTo(new[]
            {
                nameof(Comment.Author),
                nameof(Comment.CreatedBy),
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

        [TestCase(false)]
        [TestCase(true)]
        public void VerifyThatHandwrittenRulesRunAlongsideGeneratedRules(bool useComposition)
        {
            var validator = useComposition ? (IValidator<Comment>)new ComposedCommentValidator() : new RestrictedCommentValidator();
            var dto = CreateComment();
            dto.Author = Guid.Empty;
            dto.Content = "restricted";

            var result = validator.Validate(dto);

            Assert.That(result.Errors.Select(failure => failure.PropertyName), Is.EquivalentTo(new[] { nameof(Comment.Author), nameof(Comment.Content) }));

            dto.Author = ReferenceId;

            Assert.That(validator.Validate(dto).Errors.Select(failure => failure.PropertyName), Is.EqualTo(new[] { nameof(Comment.Content) }));

            dto.Content = "Allowed comment";

            Assert.That(validator.Validate(dto).IsValid, Is.True);
        }

        [Test]
        public void VerifyThatValidationInvocationUsesTheApprovedExceptionContract()
        {
            var dto = CreateComment();
            dto.Content = null;

            var validator = new CommentValidator();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(() => validator.ValidateAndThrow(dto), Throws.TypeOf<ValidationException>().And.Matches<ValidationException>(
                    exception => exception.Errors.Select(failure => failure.PropertyName).SequenceEqual(new[] { nameof(Comment.Content) })));

                Assert.That(() => validator.Validate((Comment)null), Throws.TypeOf<ArgumentNullException>());
            }
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

        private sealed class RestrictedCommentValidator : CommentValidator
        {
            public RestrictedCommentValidator()
            {
                this.RuleFor(dto => dto.Content).NotEqual("restricted");
            }
        }

        private sealed class ComposedCommentValidator : AbstractValidator<Comment>
        {
            public ComposedCommentValidator()
            {
                this.Include(new CommentValidator());
                this.RuleFor(dto => dto.Content).NotEqual("restricted");
            }
        }
    }
}
