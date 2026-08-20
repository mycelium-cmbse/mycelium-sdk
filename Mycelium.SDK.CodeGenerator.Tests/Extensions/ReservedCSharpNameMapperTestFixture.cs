// ------------------------------------------------------------------------------------------------
//  <copyright file="ReservedCSharpNameMapperTestFixture.cs" company="Starion Group S.A.">
// 
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
// 
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Tests.Extensions
{
    using Mycelium.SDK.CodeGenerator.Extensions;

    /// <summary>
    /// Verifies the shared C# identifier policy used by enum names and literals.
    /// </summary>
    [TestFixture]
    public class ReservedCSharpNameMapperTestFixture
    {
        [TestCase("ProjectVisibility")]
        [TestCase("ChangesRequested")]
        [TestCase("_")]
        [TestCase("record")]
        public void Verify_that_Map_preserves_legal_identifiers(string identifier)
        {
            Assert.That(ReservedCSharpNameMapper.Map(identifier), Is.EqualTo(identifier));
        }

        [TestCase("class", "@class")]
        [TestCase("event", "@event")]
        [TestCase("namespace", "@namespace")]
        [TestCase("public", "@public")]
        public void Verify_that_Map_escapes_reserved_CSharp_keywords(string identifier, string expectedIdentifier)
        {
            Assert.That(ReservedCSharpNameMapper.Map(identifier), Is.EqualTo(expectedIdentifier));
        }

        [TestCase(" ")]
        [TestCase("1Status")]
        [TestCase("Invalid-Type")]
        [TestCase("two words")]
        [TestCase("@class")]
        [TestCase(@"\u0063lass")]
        public void Verify_that_Map_rejects_identifiers_that_would_change_the_modeled_name(string identifier)
        {
            var exception = Assert.Throws<ArgumentException>(() => ReservedCSharpNameMapper.Map(identifier));

            using (Assert.EnterMultipleScope())
            {
                Assert.That(exception.ParamName, Is.EqualTo("input"));
                Assert.That(exception.Message, Does.Contain($"'{identifier}' is not a valid C# identifier"));
            }
        }

        [Test]
        public void Verify_that_Map_rejects_null_and_empty_input()
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(() => ReservedCSharpNameMapper.Map(null), Throws.ArgumentNullException);
                Assert.That(() => ReservedCSharpNameMapper.Map(string.Empty), Throws.TypeOf<ArgumentException>());
            }
        }

        [TestCase("class", true)]
        [TestCase("event", true)]
        [TestCase("record", false)]
        [TestCase("ProjectVisibility", false)]
        public void Verify_that_QueryIsReserved_identifies_reserved_keywords(string identifier, bool expected)
        {
            Assert.That(ReservedCSharpNameMapper.QueryIsReserved(identifier), Is.EqualTo(expected));
        }

        [Test]
        public void Verify_that_QueryIsReserved_rejects_null_and_empty_input()
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(() => ReservedCSharpNameMapper.QueryIsReserved(null), Throws.ArgumentNullException);
                Assert.That(() => ReservedCSharpNameMapper.QueryIsReserved(string.Empty), Throws.TypeOf<ArgumentException>());
            }
        }
    }
}
