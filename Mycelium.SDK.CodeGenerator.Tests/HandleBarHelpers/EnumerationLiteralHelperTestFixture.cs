// ------------------------------------------------------------------------------------------------
//  <copyright file="EnumerationLiteralHelperTestFixture.cs" company="Starion Group S.A.">
// 
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
// 
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Tests.HandleBarHelpers
{
    using HandlebarsDotNet;
    using HandlebarsDotNet.Helpers;

    using Mycelium.SDK.CodeGenerator.HandleBarHelpers;

    using uml4net.SimpleClassifiers;

    [TestFixture]
    public class EnumerationLiteralHelperTestFixture
    {
        private IHandlebars handlebars = null!;

        [SetUp]
        public void SetUp()
        {
            this.handlebars = Handlebars.CreateSharedEnvironment();
            HandlebarsHelpers.Register(this.handlebars);
            this.handlebars.RegisterEnumerationLiteralHelper();
        }

        [TestCase("ChangesRequested", "ChangesRequested")]
        [TestCase("class", "@class")]
        public void Verify_that_EnumerationLiteral_Write_writes_the_mapped_identifier(string literalName, string expectedIdentifier)
        {
            var literal = new EnumerationLiteral
            {
                XmiId = "literal-id",
                Name = literalName
            };

            var template = this.handlebars.Compile("{{ #EnumerationLiteral.Write this }}");

            Assert.That(template(literal), Is.EqualTo(expectedIdentifier));
        }

        [Test]
        public void Verify_that_EnumerationLiteral_Write_requires_an_IEnumerationLiteral_argument()
        {
            var template = this.handlebars.Compile("{{ #EnumerationLiteral.Write this }}");
            var exception = Assert.Throws<HandlebarsException>(() => template(new object()));

            Assert.That(exception.Message, Is.EqualTo("{{EnumerationLiteral.Write}} requires an IEnumerationLiteral argument."));
        }

        [Test]
        public void Verify_that_EnumerationLiteral_Write_requires_exactly_one_argument()
        {
            var template = this.handlebars.Compile("{{ #EnumerationLiteral.Write this this }}");
            var exception = Assert.Throws<HandlebarsException>(() => template(new object()));

            Assert.That(exception.Message, Is.EqualTo("{{EnumerationLiteral.Write}} requires exactly one argument."));
        }

        [Test]
        public void Verify_that_RegisterEnumerationLiteralHelper_rejects_a_null_environment()
        {
            IHandlebars nullHandlebars = null;

            Assert.That(() => nullHandlebars.RegisterEnumerationLiteralHelper(), Throws.ArgumentNullException);
        }
    }
}
