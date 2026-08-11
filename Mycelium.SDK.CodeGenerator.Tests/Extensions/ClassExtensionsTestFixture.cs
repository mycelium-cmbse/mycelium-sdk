// ------------------------------------------------------------------------------------------------
//  <copyright file="ClassExtensionsTestFixture.cs" company="Starion Group S.A.">
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

    [TestFixture]
    public class ClassExtensionsTestFixture
    {
        private IClass[] classes = [];

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var result = XmiLoadingTestFixture.ReadFunctionalData();
            var functionalData = XmiLoadingTestFixture.QueryFunctionalDataPackage(result);

            this.classes = functionalData.PackagedElement
                .OfType<IClass>()
                .ToArray();
        }

        [Test]
        public void Verify_that_QueryPocoInterfaceProperties_returns_the_direct_FunctionalData_contract()
        {
            var projectMember = this.classes.Single(umlClass => umlClass.Name == "ProjectMember");
            var properties = projectMember.QueryPocoInterfaceProperties();

            Assert.Multiple(() =>
            {
                Assert.That(properties.Select(property => property.Name),
                    Is.EqualTo(new[]
                    {
                        "activeOwnership",
                        "isOutsideCollaborator",
                        "isPartOf",
                        "owns",
                        "role",
                        "user"
                    }));

                Assert.That(properties.All(property => projectMember.OwnedAttribute.Contains(property)), Is.True);

                Assert.That(properties
                        .Where(property => property.Association is not null)
                        .Select(property => property.Name),
                    Is.EquivalentTo(new[]
                    {
                        "activeOwnership",
                        "isPartOf",
                        "owns",
                        "user"
                    }));
            });
        }

        [Test]
        public void Verify_that_QueryPocoInterfaceProperties_deduplicates_and_orders_properties()
        {
            var umlClass = new Class
            {
                XmiId = "class-id",
                Name = "TestClass"
            };

            umlClass.OwnedAttribute.Add(new Property
            {
                XmiId = "property-alpha-lower",
                Name = "alpha"
            });

            umlClass.OwnedAttribute.Add(new Property
            {
                XmiId = "property-id",
                Name = "id"
            });

            umlClass.OwnedAttribute.Add(new Property
            {
                XmiId = "property-alpha-2",
                Name = "Alpha"
            });

            umlClass.OwnedAttribute.Add(new Property
            {
                XmiId = "property-alpha-1",
                Name = "Alpha"
            });

            umlClass.OwnedAttribute.Add(new Property
            {
                XmiId = "property-zeta",
                Name = "zeta"
            });

            umlClass.OwnedAttribute.Add(new Property
            {
                XmiId = "property-alpha-lower",
                Name = "shouldNotAppear"
            });

            var properties = umlClass.QueryPocoInterfaceProperties();

            Assert.That(properties.Select(property => property.XmiId),
                Is.EqualTo(new[]
                {
                    "property-id",
                    "property-alpha-1",
                    "property-alpha-2",
                    "property-alpha-lower",
                    "property-zeta"
                }));
        }

        [Test]
        public void Verify_that_QueryPocoInterfaceProperties_rejects_a_null_class()
        {
            IClass umlClass = null;

            Assert.That(() => umlClass.QueryPocoInterfaceProperties(), Throws.ArgumentNullException);
        }

        [Test]
        public void Verify_that_QueryPocoInterfaceProperties_rejects_a_missing_property_identifier()
        {
            var umlClass = new Class
            {
                XmiId = "class-id",
                Name = "TestClass"
            };

            umlClass.OwnedAttribute.Add(new Property
            {
                XmiId = " ",
                Name = "propertyName"
            });

            var exception = Assert.Throws<InvalidOperationException>(() => umlClass.QueryPocoInterfaceProperties());

            Assert.That(exception.Message, Is.EqualTo("A POCO property has no XMI identifier."));
        }

        [Test]
        public void Verify_that_QueryPocoInterfaceProperties_rejects_a_missing_property_name()
        {
            var umlClass = new Class
            {
                XmiId = "class-id",
                Name = "TestClass"
            };

            umlClass.OwnedAttribute.Add(new Property
            {
                XmiId = "property-id",
                Name = " "
            });

            var exception = Assert.Throws<InvalidOperationException>(() => umlClass.QueryPocoInterfaceProperties());

            Assert.That(exception.Message, Is.EqualTo("Property 'property-id' has no name."));
        }

        [Test]
        public void Verify_that_QueryPocoImplementationProperties_returns_the_complete_FunctionalData_contract()
        {
            var projectMember = this.classes.Single(umlClass => umlClass.Name == "ProjectMember");

            var properties = projectMember.QueryPocoImplementationProperties();

            Assert.That(properties.Select(property => property.Name),
                Is.EqualTo(new[]
                {
                    "id",
                    "activeOwnership",
                    "createdBy",
                    "createdOn",
                    "isOutsideCollaborator",
                    "isPartOf",
                    "owns",
                    "role",
                    "updatedBy",
                    "updatedOn",
                    "user"
                }));
        }
        
        [Test]
        public void Verify_that_QueryPocoImplementationProperties_rejects_a_null_class()
        {
            IClass umlClass = null;

            Assert.That(() => umlClass.QueryPocoImplementationProperties(), Throws.ArgumentNullException);
        }
        
        [Test]
        public void Verify_that_QueryPocoImplementationProperties_validates_ancestors_before_property_traversal()
        {
            var specificClass = new Class
            {
                XmiId = "specific-class",
                Name = "SpecificClass"
            };

            var generalClass = new Class
            {
                XmiId = "general-class",
                Name = "GeneralClass"
            };

            specificClass.Generalization.Add(new Generalization
            {
                XmiId = "specific-to-general",
                Specific = specificClass,
                General = generalClass
            });

            generalClass.Generalization.Add(new Generalization
            {
                XmiId = "unresolved-generalization",
                Specific = generalClass
            });

            specificClass.OwnedAttribute.Add(new Property
            {
                XmiId = " ",
                Name = "invalidProperty"
            });

            var exception = Assert.Throws<InvalidOperationException>(() => specificClass.QueryPocoImplementationProperties());

            Assert.That(exception.Message, Is.EqualTo("Class 'GeneralClass' has an unresolved or non-class generalization."));
        }
        
        [Test]
        public void Verify_that_QueryPocoImplementationProperties_rejects_a_non_class_ancestor()
        {
            var specificClass = new Class
            {
                XmiId = "specific-class",
                Name = "SpecificClass"
            };

            var generalClass = new Class
            {
                XmiId = "general-class",
                Name = "GeneralClass"
            };

            specificClass.Generalization.Add(new Generalization
            {
                XmiId = "specific-to-general",
                Specific = specificClass,
                General = generalClass
            });

            generalClass.Generalization.Add(new Generalization
            {
                XmiId = "non-class-generalization",
                Specific = generalClass,
                General = new DataType
                {
                    XmiId = "data-type",
                    Name = "DataType"
                }
            });

            var exception = Assert.Throws<InvalidOperationException>(() => specificClass.QueryPocoImplementationProperties());

            Assert.That(exception.Message, Is.EqualTo("Class 'GeneralClass' has an unresolved or non-class generalization."));
        }

        [Test]
        public void Verify_that_QueryPocoImplementationProperties_rejects_a_generalization_cycle()
        {
            var classA = new Class
            {
                XmiId = "class-a",
                Name = "ClassA"
            };

            var classB = new Class
            {
                XmiId = "class-b",
                Name = "ClassB"
            };

            classA.Generalization.Add(new Generalization
            {
                XmiId = "a-to-b",
                Specific = classA,
                General = classB
            });

            classB.Generalization.Add(new Generalization
            {
                XmiId = "b-to-a",
                Specific = classB,
                General = classA
            });

            var exception = Assert.Throws<InvalidOperationException>(() => classA.QueryPocoImplementationProperties());

            Assert.That(exception.Message, Is.EqualTo("Generalization cycle detected: 'ClassA' -> 'ClassB' -> 'ClassA'."));
        }
        
        [Test]
        public void Verify_that_QueryPocoGeneralizations_returns_only_direct_FunctionalData_generalizations()
        {
            var projectMember = this.classes.Single(umlClass => umlClass.Name == "ProjectMember");

            var generalizations = projectMember.QueryPocoGeneralizations();

            Assert.That(generalizations.Select(generalization => generalization.Name),
                Is.EqualTo(new[] { "AuditableThing" }));
        }

        [Test]
        public void Verify_that_QueryPocoGeneralizations_deduplicates_and_orders_generalizations()
        {
            var specificClass = new Class
            {
                XmiId = "specific-class",
                Name = "SpecificClass"
            };

            var generalClasses = new IClass[]
            {
                new Class { XmiId = "general-zeta", Name = "zeta" },
                new Class { XmiId = "general-alpha-lower", Name = "alpha" },
                new Class { XmiId = "general-alpha-2", Name = "Alpha" },
                new Class { XmiId = "general-alpha-1", Name = "Alpha" },
                new Class { XmiId = "general-alpha-lower", Name = "shouldNotAppear" }
            };

            for (var index = 0; index < generalClasses.Length; index++)
            {
                var generalClass = generalClasses[index];

                specificClass.Generalization.Add(new Generalization
                {
                    XmiId = $"generalization-{index}",
                    Specific = specificClass,
                    General = generalClass
                });
            }

            var generalizations = specificClass.QueryPocoGeneralizations();

            Assert.That(generalizations.Select(generalization => generalization.XmiId),
                Is.EqualTo(new[]
                {
                    "general-alpha-1",
                    "general-alpha-2",
                    "general-alpha-lower",
                    "general-zeta"
                }));
        }

        [Test]
        public void Verify_that_QueryPocoGeneralizations_rejects_a_null_class()
        {
            IClass umlClass = null;

            Assert.That(() => umlClass.QueryPocoGeneralizations(), Throws.ArgumentNullException);
        }

        [Test]
        public void Verify_that_QueryPocoGeneralizations_rejects_an_unresolved_generalization()
        {
            var specificClass = new Class
            {
                XmiId = "specific-class",
                Name = "SpecificClass"
            };

            specificClass.Generalization.Add(new Generalization
            {
                XmiId = "unresolved-generalization",
                Specific = specificClass
            });

            var exception = Assert.Throws<InvalidOperationException>(
                () => specificClass.QueryPocoGeneralizations());

            Assert.That(exception.Message, Is.EqualTo("Class 'SpecificClass' has an unresolved or non-class generalization."));
        }

        [Test]
        public void Verify_that_QueryPocoGeneralizations_rejects_a_non_class_generalization()
        {
            var specificClass = new Class
            {
                XmiId = "specific-class",
                Name = "SpecificClass"
            };

            specificClass.Generalization.Add(new Generalization
            {
                XmiId = "non-class-generalization",
                Specific = specificClass,
                General = new DataType
                {
                    XmiId = "data-type",
                    Name = "DataType"
                }
            });

            var exception = Assert.Throws<InvalidOperationException>(() => specificClass.QueryPocoGeneralizations());

            Assert.That(exception.Message, Is.EqualTo("Class 'SpecificClass' has an unresolved or non-class generalization."));
        }

        [Test]
        public void Verify_that_QueryPocoGeneralizations_rejects_a_general_class_without_an_identifier()
        {
            var specificClass = new Class
            {
                XmiId = "specific-class",
                Name = "SpecificClass"
            };

            specificClass.Generalization.Add(new Generalization
            {
                XmiId = "generalization",
                Specific = specificClass,
                General = new Class
                {
                    XmiId = " ",
                    Name = "GeneralClass"
                }
            });

            var exception = Assert.Throws<InvalidOperationException>(() => specificClass.QueryPocoGeneralizations());

            Assert.That(exception.Message, Is.EqualTo("Class 'GeneralClass' has no XMI identifier."));
        }
    }
}
