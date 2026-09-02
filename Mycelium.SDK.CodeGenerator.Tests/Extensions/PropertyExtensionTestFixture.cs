// ------------------------------------------------------------------------------------------------
//  <copyright file="PropertyExtensionTestFixture.cs" company="Starion Group S.A.">
// 
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
// 
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Tests.Extensions
{
    using Mycelium.SDK.CodeGenerator.Extensions;
    using Mycelium.SDK.CodeGenerator.Tests.Xmi;

    using uml4net.Classification;
    using uml4net.SimpleClassifiers;
    using uml4net.StructuredClassifiers;
    using uml4net.Values;

    [TestFixture]
    public class PropertyExtensionTestFixture
    {
        private IClass[] classes = [];
        private IProperty roleProperty;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var result = XmiLoadingTestFixture.ReadFunctionalData();
            var functionalData = XmiLoadingTestFixture.QueryFunctionalDataPackage(result);

            this.classes = functionalData.PackagedElement
                .OfType<IClass>()
                .ToArray();

            this.roleProperty = this.QueryProperty("ProjectMember", "role");
        }

        [Test]
        public void Verify_that_QueryPropertyName_preserves_the_FunctionalData_contract()
        {
            Assert.That(this.roleProperty.QueryPropertyName(), Is.EqualTo("Role"));
        }

        [Test]
        public void Verify_that_QueryPropertyName_rejects_a_null_property()
        {
            IProperty property = null;

            Assert.That(() => property.QueryPropertyName(), Throws.ArgumentNullException);
        }

        [Test]
        public void Verify_that_QueryPocoTypeName_applies_multiplicity_and_nullability()
        {
            var optionalInteger = new Property
            {
                XmiId = "optional-integer",
                Name = "optionalInteger",
                Type = new PrimitiveType
                {
                    XmiId = "integer-type",
                    Name = "Integer"
                }
            };

            optionalInteger.LowerValue.Add(new LiteralInteger { Value = 0 });

            var optionalEnumeration = new Property
            {
                XmiId = "optional-enumeration",
                Name = "optionalEnumeration",
                Type = new Enumeration
                {
                    XmiId = "enumeration-type",
                    Name = "OptionalKind"
                }
            };

            optionalEnumeration.LowerValue.Add(new LiteralInteger { Value = 0 });

            var optionalReference = new Property
            {
                XmiId = "optional-reference",
                Name = "optionalReference",
                Type = new Class
                {
                    XmiId = "referenced-class",
                    Name = "ReferencedClass"
                }
            };

            optionalReference.LowerValue.Add(new LiteralInteger { Value = 0 });

            var manyStrings = new Property
            {
                XmiId = "many-strings",
                Name = "manyStrings",
                Type = new PrimitiveType
                {
                    XmiId = "string-type",
                    Name = "String"
                }
            };

            manyStrings.UpperValue.Add(
                new LiteralUnlimitedNatural { Value = "*" });

            var guidClassReference = new Property
            {
                XmiId = "guid-class-reference",
                Name = "guidClassReference",
                Type = new Class
                {
                    XmiId = "guid-class",
                    Name = "Guid"
                }
            };

            using (Assert.EnterMultipleScope())
            {
                Assert.That(optionalInteger.QueryPocoTypeName(), Is.EqualTo("int?"));
                Assert.That(optionalEnumeration.QueryPocoTypeName(), Is.EqualTo("OptionalKind?"));
                Assert.That(optionalReference.QueryPocoTypeName(), Is.EqualTo("IReferencedClass"));
                Assert.That(manyStrings.QueryPocoTypeName(), Is.EqualTo("List<string>"));
                Assert.That(guidClassReference.QueryPocoTypeName(), Is.EqualTo("IGuid"));
            }
        }

        [Test]
        public void Verify_that_QueryPocoTypeName_maps_the_FunctionalData_contract()
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(
                    this.QueryProperty("ProjectMember", "activeOwnership").QueryPocoTypeName(),
                    Is.EqualTo("IOwnership"));

                Assert.That(
                    this.QueryProperty("ProjectMember", "owns").QueryPocoTypeName(),
                    Is.EqualTo("List<IOwnership>"));

                Assert.That(
                    this.QueryProperty("ProjectMember", "role").QueryPocoTypeName(),
                    Is.EqualTo("ProjectMemberRole"));

                Assert.That(
                    this.QueryProperty("BranchProtectionRule", "mergeAllowedFor").QueryPocoTypeName(),
                    Is.EqualTo("List<ProjectMemberRole>"));

                Assert.That(
                    this.QueryProperty("Thing", "id").QueryPocoTypeName(),
                    Is.EqualTo("Guid"));

                Assert.That(
                    this.QueryProperty("AuditableThing", "createdOn").QueryPocoTypeName(),
                    Is.EqualTo("DateTime"));

                Assert.That(
                    this.QueryProperty("BranchProtectionRule", "minimumRequiredApproval").QueryPocoTypeName(),
                    Is.EqualTo("int"));

                Assert.That(
                    this.QueryProperty("BranchProtectionRule", "name").QueryPocoTypeName(),
                    Is.EqualTo("string"));

                Assert.That(
                    this.QueryProperty("FunctionalProject", "sharedPreferences").QueryPocoTypeName(),
                    Is.EqualTo("Dictionary<string,string>"));
            }
        }

        [Test]
        public void Verify_that_QueryPocoTypeName_rejects_a_null_property()
        {
            IProperty property = null;

            Assert.That(() => property.QueryPocoTypeName(), Throws.ArgumentNullException);
        }

        [Test]
        public void Verify_that_QueryPocoTypeName_rejects_an_unsupported_type()
        {
            var property = new Property
            {
                XmiId = "property-id",
                Name = "unsupported",
                Type = new DataType
                {
                    XmiId = "unsupported-type",
                    Name = "UnsupportedType"
                }
            };

            var exception = Assert.Throws<InvalidOperationException>(() => property.QueryPocoTypeName());

            Assert.That(exception.Message, Is.EqualTo("Property 'unsupported' has unsupported UML type 'UnsupportedType'."));
        }

        /// <summary>
        /// Queries a directly owned property from the loaded FunctionalData model.
        /// </summary>
        /// <param name="className">
        /// The owning UML class name.
        /// </param>
        /// <param name="propertyName">
        /// The UML property name.
        /// </param>
        /// <returns>
        /// The matching directly owned UML property.
        /// </returns>
        private IProperty QueryProperty(string className, string propertyName)
        {
            return this.classes
                .Single(umlClass => umlClass.Name == className)
                .OwnedAttribute
                .Single(property => property.Name == propertyName);
        }
    }
}
