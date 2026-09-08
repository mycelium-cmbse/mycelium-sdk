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
        private IHandlebars jsonSerializerHandlebars;
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

            this.jsonSerializerHandlebars =
                Handlebars.CreateSharedEnvironment();

            HandlebarsHelpers.Register(
                this.jsonSerializerHandlebars);

            this.jsonSerializerHandlebars
                .RegisterJsonSerializerPropertyHelper();

            this.jsonSerializerHandlebars
                .RegisterSafeContextHelper();
        }

        [Test]
        public void Verify_that_Poco_interface_declarations_match_the_FunctionalData_contract()
        {
            using (Assert.EnterMultipleScope())
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
            }
        }

        [Test]
        public void Verify_that_Poco_implementation_declarations_match_the_FunctionalData_contract()
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(
                    this.RenderPocoImplementation(this.QueryProperty("ProjectMember", "activeOwnership")),
                    Is.EqualTo("public IOwnership ActiveOwnership { get; set; }"));

                Assert.That(
                    this.RenderPocoImplementation(this.QueryProperty("ProjectMember", "owns")),
                    Is.EqualTo("public List<IOwnership> Owns { get; set; } = [];"));

                Assert.That(
                    this.RenderPocoImplementation(this.QueryProperty("ProjectMember", "isOutsideCollaborator")),
                    Is.EqualTo("public bool IsOutsideCollaborator => this.ComputeIsOutsideCollaborator();"));

                Assert.That(
                    this.RenderPocoImplementation(this.QueryProperty("FunctionalProject", "sharedPreferences")),
                    Is.EqualTo(
                        "public Dictionary<string,string> SharedPreferences { get; set; } = [];"));

                Assert.That(
                    this.RenderPocoImplementation(this.QueryProperty("Thing", "id")),
                    Is.EqualTo("public Guid Id { get; set; }"));
            }
        }

        [Test]
        public void Verify_that_Poco_derived_union_declarations_delegate_to_computation()
        {
            var property = new Property
            {
                XmiId = "derived-union",
                Name = "derivedUnion",
                IsDerivedUnion = true,
                Type = new PrimitiveType
                {
                    XmiId = "boolean-type",
                    Name = "Boolean"
                }
            };

            property.LowerValue.Add(new LiteralInteger { Value = 1 });

            using (Assert.EnterMultipleScope())
            {
                Assert.That(
                    this.RenderPocoInterface(property),
                    Is.EqualTo("bool DerivedUnion { get; }"));

                Assert.That(
                    this.RenderPocoImplementation(property),
                    Is.EqualTo(
                        "public bool DerivedUnion => this.ComputeDerivedUnion();"));
            }
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

            using (Assert.EnterMultipleScope())
            {
                Assert.That(
                    this.RenderPocoInterface(property),
                    Is.EqualTo("int? OptionalCount { get; set; }"));

                Assert.That(
                    this.RenderPocoImplementation(property),
                    Is.EqualTo("public int? OptionalCount { get; set; }"));
            }
        }

        [Test]
        public void Verify_that_Dto_and_Poco_property_helpers_can_be_registered_independently()
        {
            var dtoHandlebars = Handlebars.CreateSharedEnvironment();
            HandlebarsHelpers.Register(dtoHandlebars);
            dtoHandlebars.RegisterDtoPropertyHelper();

            var property = this.QueryProperty("ProjectMember", "activeOwnership");
            var dtoTemplate = dtoHandlebars.Compile("{{ #Property.WriteDtoInterfaceDeclaration this }}");

            using (Assert.EnterMultipleScope())
            {
                Assert.That(
                    dtoTemplate(property),
                    Is.EqualTo("Guid? ActiveOwnership { get; set; }"));

                Assert.That(
                    this.RenderPocoInterface(property),
                    Is.EqualTo("IOwnership ActiveOwnership { get; set; }"));
            }
        }

        [Test]
        public void Verify_that_Json_serializer_property_helpers_match_the_FunctionalData_contract()
        {
            var identifier =
                this.QueryProperty("Thing", "id");

            var ordinaryProperty =
                this.QueryProperty("BranchProtectionRule", "name");

            var dictionary =
                this.QueryProperty("FunctionalProject", "sharedPreferences");

            using (Assert.EnterMultipleScope())
            {
                Assert.That(
                    this.RenderJsonSerializerProperty(
                        "{{#if (Property.QueryIsIdentifier this)}}identifier{{else}}ordinary{{/if}}",
                        identifier),
                    Is.EqualTo("identifier"));

                Assert.That(
                    this.RenderJsonSerializerProperty(
                        "{{#if (Property.QueryIsIdentifier this)}}identifier{{else}}ordinary{{/if}}",
                        ordinaryProperty),
                    Is.EqualTo("ordinary"));

                Assert.That(
                    this.RenderJsonSerializerProperty(
                        "{{#if (Property.QueryIsStringDictionary this)}}dictionary{{else}}ordinary{{/if}}",
                        dictionary),
                    Is.EqualTo("dictionary"));

                Assert.That(
                    this.RenderJsonSerializerProperty(
                        "{{#if (Property.QueryIsStringDictionary this)}}dictionary{{else}}ordinary{{/if}}",
                        ordinaryProperty),
                    Is.EqualTo("ordinary"));

                Assert.That(
                    this.RenderJsonSerializerProperty(
                        "{{Property.WritePropertyName this}}",
                        this.QueryProperty("ProjectMember", "role")),
                    Is.EqualTo("Role"));
            }
        }

        [Test]
        public void Verify_that_safe_context_exposes_the_property_and_owning_class()
        {
            var classContext =
                this.classes.Single(umlClass => umlClass.Name == "ProjectMember");

            var property =
                this.QueryProperty("ProjectMember", "role");

            var template = this.jsonSerializerHandlebars.Compile(
                "{{#withPropertyClassContext property classContext}}" +
                "{{property.Name}}:{{classContext.Name}}" +
                "{{/withPropertyClassContext}}");

            var renderedContext = template(
                new
                {
                    property,
                    classContext
                });

            Assert.That(
                renderedContext,
                Is.EqualTo("role:ProjectMember"));
        }

        [Test]
        public void Verify_that_RegisterPocoPropertyHelper_rejects_a_null_environment()
        {
            IHandlebars handlebars = null;

            Assert.That(
                () => handlebars.RegisterPocoPropertyHelper(),
                Throws.ArgumentNullException);
        }

        [Test]
        public void Verify_that_RegisterJsonSerializerPropertyHelper_rejects_a_null_environment()
        {
            IHandlebars handlebars = null;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(
                    () => handlebars.RegisterJsonSerializerPropertyHelper(),
                    Throws.ArgumentNullException);

                Assert.That(
                    () => handlebars.RegisterSafeContextHelper(),
                    Throws.ArgumentNullException);
            }
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

        private string RenderJsonSerializerProperty(
            string templateText,
            IProperty property)
        {
            var template =
                this.jsonSerializerHandlebars.Compile(templateText);

            return template(property);
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
