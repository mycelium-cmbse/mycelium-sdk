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

    /// <summary>
    /// Verifies the behavior of the named-element extension methods.
    /// </summary>
    [TestFixture]
    public class NamedElementExtensionsTestFixture
    {
        /// <summary>
        /// Verifies that <c>Describe</c> returns the UML class name when one is available.
        /// </summary>
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

        /// <summary>
        /// Verifies that <c>Describe</c> falls back to the property XMI identifier when its name is blank.
        /// </summary>
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

        /// <summary>
        /// Verifies that <c>Describe</c> rejects a <see langword="null" /> named element.
        /// </summary>
        [Test]
        public void Verify_that_Describe_rejects_a_null_named_element()
        {
            INamedElement namedElement = null;

            Assert.That(() => namedElement.Describe(), Throws.ArgumentNullException);
        }
    }
}
