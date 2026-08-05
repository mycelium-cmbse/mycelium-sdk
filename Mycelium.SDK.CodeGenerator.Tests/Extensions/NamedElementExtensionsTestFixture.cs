// ------------------------------------------------------------------------------------------------
//  <copyright file="NamedElementExtensionsTestFixture.cs" company="Starion Group S.A.">
// 
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
// 
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Tests.Extensions
{
    using Mycelium.SDK.CodeGenerator.Extensions;

    using uml4net.Classification;
    using uml4net.CommonStructure;
    using uml4net.StructuredClassifiers;

    [TestFixture]
    public class NamedElementExtensionsTestFixture
    {
        [Test]
        public void Verify_that_Describe_returns_the_class_name()
        {
            var umlClass = new Class
            {
                XmiId = "class-id",
                Name = "FunctionalProject"
            };

            Assert.That(umlClass.Describe(), Is.EqualTo("FunctionalProject"));
        }

        [Test]
        public void Verify_that_Describe_falls_back_to_the_property_Xmi_identifier()
        {
            var property = new Property
            {
                XmiId = "property-id",
                Name = " "
            };

            Assert.That(property.Describe(), Is.EqualTo("property-id"));
        }

        [Test]
        public void Verify_that_Describe_rejects_a_null_named_element()
        {
            INamedElement namedElement = null;

            Assert.That(() => namedElement.Describe(), Throws.ArgumentNullException);
        }
    }
}
