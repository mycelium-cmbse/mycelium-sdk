// ------------------------------------------------------------------------------------------------
//  <copyright file="ClassHelperTestFixture.cs" company="Starion Group S.A.">
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

    using uml4net.StructuredClassifiers;

    [TestFixture]
    public class ClassHelperTestFixture
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
            this.pocoHandlebars.RegisterPocoClassHelper();
        }

        [Test]
        public void Verify_that_Dto_and_Poco_class_helpers_can_be_registered_independently()
        {
            var dtoHandlebars = Handlebars.CreateSharedEnvironment();
            HandlebarsHelpers.Register(dtoHandlebars);
            dtoHandlebars.RegisterDtoClassHelper();

            var dtoTemplate = dtoHandlebars.Compile("{{ #Class.WriteDtoInterfaceIdentifier this }}");
            var pocoTemplate = this.pocoHandlebars.Compile("{{ #Class.WritePocoInterfaceIdentifier this }}");

            var projectMember = this.QueryClass("ProjectMember");

            Assert.Multiple(() =>
            {
                Assert.That(dtoTemplate(projectMember), Is.EqualTo("IProjectMember"));
                Assert.That(pocoTemplate(projectMember), Is.EqualTo("IProjectMember"));
            });
        }

        [Test]
        public void Verify_that_Poco_class_helpers_require_an_IClass_argument()
        {
            var template = this.pocoHandlebars.Compile("{{ #Class.WritePocoInterfaceIdentifier this }}");

            var exception = Assert.Throws<HandlebarsException>(() => template(new object()));

            Assert.That(exception.Message, Is.EqualTo("{{Class.WritePocoInterfaceIdentifier}} requires an IClass argument."));
        }

        [Test]
        public void Verify_that_Poco_class_helpers_reject_multiple_arguments()
        {
            var template = this.pocoHandlebars.Compile("{{ #Class.WritePocoInterfaceIdentifier this this }}");
            var exception = Assert.Throws<HandlebarsException>(() => template(new object()));

            Assert.That(exception.Message, Is.EqualTo("{{Class.WritePocoInterfaceIdentifier}} requires exactly one argument."));
        }

        [Test]
        public void Verify_that_Poco_identifier_helpers_write_identifiers_and_direct_generalizations()
        {
            var template = this.pocoHandlebars.Compile("{{ #Class.WritePocoInterfaceIdentifier this }}" +
                                                       "{{ #Class.WritePocoInterfaceGeneralizations this }}");

            Assert.Multiple(() =>
            {
                Assert.That(template(this.QueryClass("ProjectMember")), Is.EqualTo("IProjectMember : IAuditableThing"));
                Assert.That(template(this.QueryClass("Thing")), Is.EqualTo("IThing"));
            });
        }

        [Test]
        public void Verify_that_Poco_property_helpers_expose_the_FunctionalData_contract()
        {
            var projectMember = this.QueryClass("ProjectMember");

            var template = this.pocoHandlebars.Compile(
                "{{ #each (Class.QueryPocoInterfaceProperties this) as | property | }}" +
                "{{ property.Name }};" +
                "{{ /each }}|" +
                "{{ #each (Class.QueryPocoImplementationProperties this) as | property | }}" +
                "{{ property.Name }};" +
                "{{ /each }}");

            var result = template(projectMember);

            Assert.That(
                result,
                Is.EqualTo(
                    "activeOwnership;isOutsideCollaborator;isPartOf;owns;role;user;|" +
                    "id;activeOwnership;createdBy;createdOn;isOutsideCollaborator;" +
                    "isPartOf;owns;role;updatedBy;updatedOn;user;"));
        }

        [Test]
        public void Verify_that_RegisterPocoClassHelper_rejects_a_null_environment()
        {
            IHandlebars handlebars = null;

            Assert.That(() => handlebars.RegisterPocoClassHelper(), Throws.ArgumentNullException);
        }

        [Test]
        public void Verify_that_WritePocoInterfaceIdentifier_rejects_a_missing_class_name()
        {
            var umlClass = new Class
            {
                XmiId = "class-id",
                Name = " "
            };

            var template = this.pocoHandlebars.Compile("{{ #Class.WritePocoInterfaceIdentifier this }}");

            var exception = Assert.Throws<InvalidOperationException>(() => template(umlClass));

            Assert.That(exception.Message, Is.EqualTo("Class 'class-id' has no name."));
        }

        [Test]
        public void Verify_that_WritePocoInterfaceIdentifier_rejects_an_illegal_identifier()
        {
            var umlClass = new Class
            {
                XmiId = "class-id",
                Name = "Invalid-Type"
            };

            var template = this.pocoHandlebars.Compile("{{ #Class.WritePocoInterfaceIdentifier this }}");
            var exception = Assert.Throws<ArgumentException>(() => template(umlClass));

            Assert.That(exception.Message, Does.Contain("'IInvalid-Type' is not a valid C# identifier"));
        }

        private IClass QueryClass(string className)
        {
            return this.classes.Single(umlClass => umlClass.Name == className);
        }
    }
}
