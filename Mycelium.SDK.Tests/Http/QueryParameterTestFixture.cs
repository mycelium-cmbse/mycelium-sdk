// ------------------------------------------------------------------------------------------------
//  <copyright file="QueryParameterTestFixture.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.Tests.Http
{
    using System;

    using Mycelium.SDK.Http;

    [TestFixture]
    public class QueryParameterTestFixture
    {
        private static readonly object[] Combinations =
        [
            new object[] { false, false, string.Empty },
            new object[] { true, false, "?includeReferenced=true" },
            new object[] { false, true, "?includeComposed=true" },
            new object[] { true, true, "?includeReferenced=true&includeComposed=true" }
        ];

        private static readonly string[] DefaultInputs = [string.Empty, "   ", "?"];

        private static readonly string[] InvalidInputs =
        [
            "?includeComposed=true&includeComposed=true",
            "?includeComposed=false&includeComposed=false",
            "?includeComposed",
            "?includeComposed=",
            "?includeReferenced=yes",
            "?includeReferenced=1"
        ];

        [Test]
        public void VerifyThatDefaultQueryParameterHasBothParametersFalseAndWritesEmptyString()
        {
            var queryParameter = new QueryParameter();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(queryParameter.IncludeReferenced, Is.False);
                Assert.That(queryParameter.IncludeComposed, Is.False);
                Assert.That(queryParameter.ToString(), Is.Empty);
            }
        }

        [TestCaseSource(nameof(Combinations))]
        public void VerifyThatToStringWritesOnlyParametersSetToTrue(bool includeReferenced, bool includeComposed, string expected)
        {
            var queryParameter = new QueryParameter { IncludeReferenced = includeReferenced, IncludeComposed = includeComposed };

            Assert.That(queryParameter.ToString(), Is.EqualTo(expected));
        }

        [TestCaseSource(nameof(Combinations))]
        public void VerifyThatFromStringRoundTripsEveryCombination(bool includeReferenced, bool includeComposed, string expected)
        {
            var queryParameter = new QueryParameter { IncludeReferenced = includeReferenced, IncludeComposed = includeComposed };

            var parsed = QueryParameter.FromString(queryParameter.ToString());

            using (Assert.EnterMultipleScope())
            {
                Assert.That(parsed.IncludeReferenced, Is.EqualTo(includeReferenced));
                Assert.That(parsed.IncludeComposed, Is.EqualTo(includeComposed));
            }
        }

        [TestCaseSource(nameof(DefaultInputs))]
        public void VerifyThatFromStringReturnsDefaultsForEmptyWhitespaceOrQuestionMark(string query)
        {
            var parsed = QueryParameter.FromString(query);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(parsed.IncludeReferenced, Is.False);
                Assert.That(parsed.IncludeComposed, Is.False);
            }
        }

        [Test]
        public void VerifyThatFromStringAcceptsInputWithoutLeadingQuestionMark()
        {
            var parsed = QueryParameter.FromString("includeReferenced=true&includeComposed=false");

            using (Assert.EnterMultipleScope())
            {
                Assert.That(parsed.IncludeReferenced, Is.True);
                Assert.That(parsed.IncludeComposed, Is.False);
            }
        }

        [Test]
        public void VerifyThatFromStringAcceptsBooleanValuesInAnyCase()
        {
            var parsed = QueryParameter.FromString("?includeReferenced=TRUE&includeComposed=True");

            using (Assert.EnterMultipleScope())
            {
                Assert.That(parsed.IncludeReferenced, Is.True);
                Assert.That(parsed.IncludeComposed, Is.True);
            }
        }

        [Test]
        public void VerifyThatFromStringDecodesUrlEncodedNamesAndValues()
        {
            var parsed = QueryParameter.FromString("?include%52eferenced=tru%65");

            Assert.That(parsed.IncludeReferenced, Is.True);
        }

        [Test]
        public void VerifyThatFromStringIgnoresUnknownParameters()
        {
            var parsed = QueryParameter.FromString("?depth=deep&includeComposed=true&flag");

            using (Assert.EnterMultipleScope())
            {
                Assert.That(parsed.IncludeReferenced, Is.False);
                Assert.That(parsed.IncludeComposed, Is.True);
            }
        }

        [Test]
        public void VerifyThatFromStringMatchesNamesCaseSensitively()
        {
            var parsed = QueryParameter.FromString("?IncludeComposed=true&INCLUDEREFERENCED=true");

            using (Assert.EnterMultipleScope())
            {
                Assert.That(parsed.IncludeReferenced, Is.False);
                Assert.That(parsed.IncludeComposed, Is.False);
            }
        }

        [Test]
        public void VerifyThatFromStringThrowsArgumentNullExceptionForNull()
        {
            Assert.That(() => QueryParameter.FromString(null), Throws.TypeOf<ArgumentNullException>());
        }

        [TestCaseSource(nameof(InvalidInputs))]
        public void VerifyThatFromStringThrowsFormatExceptionForInvalidInput(string query)
        {
            Assert.That(() => QueryParameter.FromString(query), Throws.TypeOf<FormatException>().With.Message.Contains("include"));
        }
    }
}
