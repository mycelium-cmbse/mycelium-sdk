// ------------------------------------------------------------------------------------------------
//  <copyright file="PropertyHelperTestFixture.cs" company="Starion Group S.A.">
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
    using Mycelium.SDK.CodeGenerator.Tests.Xmi;

    using uml4net.Classification;
    using uml4net.SimpleClassifiers;
    using uml4net.StructuredClassifiers;
    using uml4net.Values;

    [TestFixture]
    public class PropertyHelperTestFixture
    {
        private IClass[] classes = [];
        private IHandlebars pocoHandlebars;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var result = XmiLoadingTestFixture.ReadFunctionalData();
            var functionalData = XmiLoadingTestFixture.QueryFunctionalDataPackage(result);

            this.classes = functionalData.PackagedElement
                .OfType<IClass>()
                .ToArray();

            this.pocoHandlebars = Handlebars.CreateSharedEnvironment();
            HandlebarsHelpers.Register(this.pocoHandlebars);
            this.pocoHandlebars.RegisterPocoPropertyHelper();
        }

        [Test]
        public void Verify_that_Poco_interface_declarations_match_the_FunctionalData_contract()
        {
            Assert.Multiple(() =>
            {
                Assert.That(
                    this.RenderPocoInterface(this.QueryProperty("ProjectMember", "activeOwnership")),
                    Is.EqualTo("IOwnership ActiveOwnership { get; set; }"));

                Assert.That(
                    this.RenderPocoInterface(this.QueryProperty("ProjectMember", "owns")),
                    Is.EqualTo("List<IOwnership> Owns { get; set; }"));

                Assert.That(
                    this.RenderPocoInterface(this.QueryProperty("ProjectMember", "role")),
                    Is.EqualTo("ProjectMemberRole Role { get; set; }"));

                Assert.That(
                    this.RenderPocoInterface(this.QueryProperty("ProjectMember", "isOutsideCollaborator")),
                    Is.EqualTo("bool IsOutsideCollaborator { get; }"));

                Assert.That(
                    this.RenderPocoInterface(this.QueryProperty("Thing", "id")),
                    Is.EqualTo("Guid Id { get; set; }"));
            });
        }

        [Test]
        public void Verify_that_Poco_implementation_declarations_match_the_FunctionalData_contract()
        {
            Assert.Multiple(() =>
            {
                Assert.That(
                    this.RenderPocoImplementation(this.QueryProperty("ProjectMember", "activeOwnership")),
                    Is.EqualTo("public IOwnership ActiveOwnership { get; set; }"));

                Assert.That(
                    this.RenderPocoImplementation(this.QueryProperty("ProjectMember", "owns")),
                    Is.EqualTo("public List<IOwnership> Owns { get; set; } = [];"));

                Assert.That(
                    this.RenderPocoImplementation(this.QueryProperty("ProjectMember", "isOutsideCollaborator")),
                    Is.EqualTo("public bool IsOutsideCollaborator { get; }"));

                Assert.That(
                    this.RenderPocoImplementation(this.QueryProperty("FunctionalProject", "sharedPreferences")),
                    Is.EqualTo(
                        "public Dictionary<string,string> SharedPreferences { get; set; } = [];"));

                Assert.That(
                    this.RenderPocoImplementation(this.QueryProperty("Thing", "id")),
                    Is.EqualTo("public Guid Id { get; set; }"));
            });
        }

        [Test]
        public void Verify_that_Poco_declarations_preserve_nullable_value_types()
        {
            var property = new Property
            {
                XmiId = "optional-count",
                Name = "optionalCount",
                Type = new PrimitiveType
                {
                    XmiId = "integer-type",
                    Name = "Integer"
                }
            };

            property.LowerValue.Add(new LiteralInteger { Value = 0 });

            Assert.Multiple(() =>
            {
                Assert.That(
                    this.RenderPocoInterface(property),
                    Is.EqualTo("int? OptionalCount { get; set; }"));

                Assert.That(
                    this.RenderPocoImplementation(property),
                    Is.EqualTo("public int? OptionalCount { get; set; }"));
            });
        }

        [Test]
        public void Verify_that_Dto_and_Poco_property_helpers_can_be_registered_independently()
        {
            var dtoHandlebars = Handlebars.CreateSharedEnvironment();
            HandlebarsHelpers.Register(dtoHandlebars);
            dtoHandlebars.RegisterDtoPropertyHelper();

            var property = this.QueryProperty("ProjectMember", "activeOwnership");

            var dtoTemplate = dtoHandlebars.Compile("{{ #Property.WriteDtoInterfaceDeclaration this }}");

            Assert.Multiple(() =>
            {
                Assert.That(
                    dtoTemplate(property),
                    Is.EqualTo("Guid? ActiveOwnership { get; set; }"));

                Assert.That(
                    this.RenderPocoInterface(property),
                    Is.EqualTo("IOwnership ActiveOwnership { get; set; }"));
            });
        }

        [Test]
        public void Verify_that_RegisterPocoPropertyHelper_rejects_a_null_environment()
        {
            IHandlebars handlebars = null;

            Assert.That(() => handlebars.RegisterPocoPropertyHelper(), Throws.ArgumentNullException);
        }

        [Test]
        public void Verify_that_Poco_property_helpers_require_an_IProperty_argument()
        {
            var template = this.pocoHandlebars.Compile("{{ #Property.WritePocoInterfaceDeclaration this }}");
            var exception = Assert.Throws<HandlebarsException>(() => template(new object()));

            Assert.That(
                exception.Message,
                Is.EqualTo(
                    "{{Property.WritePocoInterfaceDeclaration}} requires an IProperty argument."));
        }

        [Test]
        public void Verify_that_Poco_property_helpers_reject_multiple_arguments()
        {
            var template = this.pocoHandlebars.Compile("{{ #Property.WritePocoInterfaceDeclaration this this }}");
            var exception = Assert.Throws<HandlebarsException>(() => template(new object()));

            Assert.That(
                exception.Message,
                Is.EqualTo(
                    "{{Property.WritePocoInterfaceDeclaration}} requires exactly one argument."));
        }

        private string RenderPocoInterface(IProperty property)
        {
            var template = this.pocoHandlebars.Compile("{{ #Property.WritePocoInterfaceDeclaration this }}");

            return template(property);
        }

        private string RenderPocoImplementation(IProperty property)
        {
            var template = this.pocoHandlebars.Compile("{{ #Property.WritePocoImplementationDeclaration this }}");

            return template(property);
        }

        private IProperty QueryProperty(string className, string propertyName)
        {
            return this.classes
                .Single(umlClass => umlClass.Name == className)
                .OwnedAttribute
                .Single(property => property.Name == propertyName);
        }
    }
}
