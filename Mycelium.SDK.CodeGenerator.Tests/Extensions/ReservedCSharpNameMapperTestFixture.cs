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
    /// Verifies the shared reserved-keyword mapping policy used by generated identifiers.
    /// </summary>
    [TestFixture]
    public class ReservedCSharpNameMapperTestFixture
    {
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("ProjectVisibility")]
        [TestCase("ChangesRequested")]
        [TestCase("_")]
        [TestCase("record")]
        [TestCase("1Status")]
        [TestCase("Invalid-Type")]
        [TestCase("two words")]
        [TestCase("@class")]
        [TestCase(@"\u0063lass")]
        public void Verify_that_Map_preserves_non_keyword_modeled_names(string identifier)
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

        [Test]
        public void Verify_that_Map_rejects_null_input()
        {
            Assert.That(() => ReservedCSharpNameMapper.Map(null), Throws.ArgumentNullException);
        }

        [TestCase("class", true)]
        [TestCase("event", true)]
        [TestCase("record", false)]
        [TestCase("ProjectVisibility", false)]
        [TestCase("", false)]
        [TestCase("Invalid-Type", false)]
        public void Verify_that_QueryIsReserved_identifies_reserved_keywords(string identifier, bool expected)
        {
            Assert.That(ReservedCSharpNameMapper.QueryIsReserved(identifier), Is.EqualTo(expected));
        }

        [Test]
        public void Verify_that_QueryIsReserved_rejects_null_input()
        {
            Assert.That(() => ReservedCSharpNameMapper.QueryIsReserved(null), Throws.ArgumentNullException);
        }
    }
}
