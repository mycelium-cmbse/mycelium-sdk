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
    using Mycelium.SDK.CodeGenerator.Tests.Generators.UmlHandleBarsGenerators;

    using uml4net.Classification;
    using uml4net.SimpleClassifiers;
    using uml4net.StructuredClassifiers;
    using uml4net.Values;

    [TestFixture]
    public class ClassExtensionsTestFixture
    {
        private static readonly string[] ExpectedRegularPropertyNames = ["regular"];
        private static readonly string[] ExpectedAllPocoPropertyNames = ["regular", "derived", "derivedUnion"];
        private IClass[] classes = [];

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var result = GeneratorSetupFixture.ReadFunctionalData();
            var functionalData = GeneratorSetupFixture.QueryFunctionalDataPackage(result);

            this.classes = functionalData.PackagedElement
                .OfType<IClass>()
                .ToArray();
        }

        [Test]
        public void VerifyThatSqlReferencesStayWithTheirDeclaringClass()
        {
            var targetClass = new Class { XmiId = "target-class", Name = "Target" };
            var parentClass = new Class { XmiId = "parent-class", Name = "Parent" };
            var ownerClass = new Class { XmiId = "owner-class", Name = "Owner" };
            var ownedReference = new Property { XmiId = "owned-reference", Name = "reference", Type = targetClass };

            parentClass.OwnedAttribute.Add(new Property { XmiId = "inherited-reference", Name = "inheritedReference", Type = targetClass });

            ownerClass.Generalization.Add(new Generalization { XmiId = "owner-parent", Specific = ownerClass, General = parentClass });

            ownerClass.OwnedAttribute.Add(ownedReference);

            ownerClass.OwnedAttribute.Add(new Property { XmiId = "derived-reference", Name = "derivedReference", Type = targetClass, IsDerived = true });

            ownerClass.OwnedAttribute.Add(new Property { XmiId = "union-reference", Name = "unionReference", Type = targetClass, IsDerivedUnion = true });

            ownerClass.OwnedAttribute.Add(new Property { XmiId = "unnamed-reference", Type = targetClass });

            ownerClass.OwnedAttribute.Add(new Property { XmiId = "blank-reference", Name = " ", Type = targetClass });

            ownerClass.OwnedAttribute.Add(new Property { XmiId = "value-property", Name = "label", Type = new PrimitiveType { XmiId = "string-type", Name = "String" } });

            ownerClass.OwnedAttribute.Add(new Property { XmiId = "enum-property", Name = "kind", Type = new Enumeration { XmiId = "kind-type", Name = "Kind" } });

            Assert.That(ownerClass.QuerySqlReferenceProperties(), Is.EqualTo([ownedReference]),
                "Only named, non-derived references declared by Owner belong in its relational storage; inherited references and values must be excluded.");
        }

        [Test]
        public void VerifyThatSqlReferencesIncludeCollectionsWithScalarOpposites()
        {
            var ownerClass = new Class { XmiId = "owner-class", Name = "Owner" };
            var targetClass = new Class { XmiId = "target-class", Name = "Target" };
            var association = new Association { XmiId = "owner-target-association", Name = "OwnerTargets" };

            var reference = new Property { XmiId = "collection-reference", Name = "targets", Type = targetClass, Association = association };
            var opposite = new Property { XmiId = "unnamed-opposite", Type = ownerClass, Association = association };

            reference.UpperValue.Add(new LiteralUnlimitedNatural { Value = "*" });

            opposite.UpperValue.Add(new LiteralInteger { Value = 1 });

            ownerClass.OwnedAttribute.Add(reference);

            association.OwnedEnd.Add(opposite);
            association.MemberEnd.AddRange([reference, opposite]);

            Assert.That(ownerClass.QuerySqlReferenceProperties(), Is.EqualTo([reference]),
                "The owned collection needs relational storage even when its association opposite is scalar and unnamed.");
        }

        [Test]
        public void VerifyThatSqlReferencesAreDeduplicatedAndDeterministicallyOrdered()
        {
            var ownerClass = new Class { XmiId = "owner-class", Name = "Owner" };
            var targetClass = new Class { XmiId = "target-class", Name = "Target" };

            ownerClass.OwnedAttribute.Add(new Property { XmiId = "reference-zeta", Name = "zeta", Type = targetClass });

            ownerClass.OwnedAttribute.Add(new Property { XmiId = "reference-alpha-lower", Name = "alpha", Type = targetClass });

            ownerClass.OwnedAttribute.Add(new Property { XmiId = "reference-alpha-2", Name = "Alpha", Type = targetClass });

            ownerClass.OwnedAttribute.Add(new Property { XmiId = "reference-alpha-1", Name = "Alpha", Type = targetClass });

            ownerClass.OwnedAttribute.Add(new Property { XmiId = "reference-alpha-lower", Name = "duplicateReference", Type = targetClass });

            var references = ownerClass.QuerySqlReferenceProperties();

            Assert.That(references.Select(property => property.XmiId), Is.EqualTo([
                "reference-alpha-1",
                "reference-alpha-2",
                "reference-alpha-lower",
                "reference-zeta"
            ]), "SQL references must be unique by XMI identifier and ordered by case-insensitive name, ordinal name and XMI identifier.");
        }

        [Test]
        public void VerifyThatSqlReferenceQueriesRejectNullClasses()
        {
            IClass umlClass = null;

            Assert.That(() => umlClass.QuerySqlReferenceProperties(), Throws.ArgumentNullException);
        }

        [Test]
        public void Verify_that_Dto_queries_exclude_derived_and_derived_union_properties_only()
        {
            var umlClass = new Class { XmiId = "class-id", Name = "TestClass" };

            umlClass.OwnedAttribute.Add(new Property { XmiId = "regular-property", Name = "regular" });

            umlClass.OwnedAttribute.Add(new Property { XmiId = "derived-property", Name = "derived", IsDerived = true });

            umlClass.OwnedAttribute.Add(new Property { XmiId = "derived-union-property", Name = "derivedUnion", IsDerivedUnion = true });

            using (Assert.EnterMultipleScope())
            {
                Assert.That(umlClass.QueryDtoInterfaceProperties()
                    .Select(property => property.Name), Is.EqualTo(ExpectedRegularPropertyNames));

                Assert.That(umlClass.QueryDtoImplementationProperties()
                    .Select(property => property.Name), Is.EqualTo(ExpectedRegularPropertyNames));

                Assert.That(umlClass.QueryPocoInterfaceProperties()
                    .Select(property => property.Name), Is.EquivalentTo(ExpectedAllPocoPropertyNames));

                Assert.That(umlClass.QueryPocoImplementationProperties()
                    .Select(property => property.Name), Is.EquivalentTo(ExpectedAllPocoPropertyNames));
            }
        }

        [Test]
        public void Verify_that_Dto_queries_exclude_derived_properties_from_the_FunctionalData_contract()
        {
            var projectMember = this.classes.Single(umlClass => umlClass.Name == "ProjectMember");
            var interfaceProperties = projectMember.QueryDtoInterfaceProperties();
            var implementationProperties = projectMember.QueryDtoImplementationProperties();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(interfaceProperties.Select(property => property.Name), Is.EqualTo([
                    "activeOwnership",
                    "isPartOf",
                    "owns",
                    "role",
                    "user"
                ]));

                Assert.That(implementationProperties.Select(property => property.Name), Is.EqualTo([
                    "id",
                    "activeOwnership",
                    "createdBy",
                    "createdOn",
                    "isPartOf",
                    "owns",
                    "role",
                    "updatedBy",
                    "updatedOn",
                    "user"
                ]));

                Assert.That(projectMember.QueryPocoInterfaceProperties()
                    .Select(property => property.Name), Does.Contain("isOutsideCollaborator"));

                Assert.That(projectMember.QueryPocoImplementationProperties()
                    .Select(property => property.Name), Does.Contain("isOutsideCollaborator"));
            }
        }

        [Test]
        public void Verify_that_QueryGeneralizations_deduplicates_and_orders_generalizations()
        {
            var specificClass = new Class { XmiId = "specific-class", Name = "SpecificClass" };

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

                specificClass.Generalization.Add(new Generalization { XmiId = $"generalization-{index}", Specific = specificClass, General = generalClass });
            }

            var generalizations = specificClass.QueryGeneralizations();

            Assert.That(generalizations.Select(generalization => generalization.XmiId), Is.EqualTo([
                "general-alpha-1",
                "general-alpha-2",
                "general-alpha-lower",
                "general-zeta"
            ]));
        }

        [Test]
        public void Verify_that_QueryGeneralizations_rejects_a_null_class()
        {
            IClass umlClass = null;

            Assert.That(() => umlClass.QueryGeneralizations(), Throws.ArgumentNullException);
        }

        [Test]
        public void Verify_that_QueryGeneralizations_returns_only_direct_FunctionalData_generalizations()
        {
            var projectMember = this.classes.Single(umlClass => umlClass.Name == "ProjectMember");

            var generalizations = projectMember.QueryGeneralizations();

            Assert.That(generalizations.Select(generalization => generalization.Name), Is.EqualTo(["AuditableThing"]));
        }

        [Test]
        public void Verify_that_QueryPocoImplementationProperties_rejects_a_null_class()
        {
            IClass umlClass = null;

            Assert.That(() => umlClass.QueryPocoImplementationProperties(), Throws.ArgumentNullException);
        }

        [Test]
        public void Verify_that_QueryPocoImplementationProperties_returns_the_complete_FunctionalData_contract()
        {
            var projectMember = this.classes.Single(umlClass => umlClass.Name == "ProjectMember");

            var properties = projectMember.QueryPocoImplementationProperties();

            Assert.That(properties.Select(property => property.Name), Is.EqualTo([
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
            ]));
        }

        [Test]
        public void Verify_that_QueryPocoInterfaceProperties_deduplicates_and_orders_properties()
        {
            var umlClass = new Class { XmiId = "class-id", Name = "TestClass" };

            umlClass.OwnedAttribute.Add(new Property { XmiId = "property-alpha-lower", Name = "alpha" });

            umlClass.OwnedAttribute.Add(new Property { XmiId = "property-id", Name = "id" });

            umlClass.OwnedAttribute.Add(new Property { XmiId = "property-alpha-2", Name = "Alpha" });

            umlClass.OwnedAttribute.Add(new Property { XmiId = "property-alpha-1", Name = "Alpha" });

            umlClass.OwnedAttribute.Add(new Property { XmiId = "property-zeta", Name = "zeta" });

            umlClass.OwnedAttribute.Add(new Property { XmiId = "property-alpha-lower", Name = "shouldNotAppear" });

            var properties = umlClass.QueryPocoInterfaceProperties();

            Assert.That(properties.Select(property => property.XmiId), Is.EqualTo([
                "property-id",
                "property-alpha-1",
                "property-alpha-2",
                "property-alpha-lower",
                "property-zeta"
            ]));
        }

        [Test]
        public void Verify_that_QueryPocoInterfaceProperties_rejects_a_null_class()
        {
            IClass umlClass = null;

            Assert.That(() => umlClass.QueryPocoInterfaceProperties(), Throws.ArgumentNullException);
        }

        [Test]
        public void Verify_that_QueryPocoInterfaceProperties_returns_the_direct_FunctionalData_contract()
        {
            var projectMember = this.classes.Single(umlClass => umlClass.Name == "ProjectMember");
            var properties = projectMember.QueryPocoInterfaceProperties();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(properties.Select(property => property.Name), Is.EqualTo([
                    "activeOwnership",
                    "isOutsideCollaborator",
                    "isPartOf",
                    "owns",
                    "role",
                    "user"
                ]));

                Assert.That(properties.All(property => projectMember.OwnedAttribute.Contains(property)), Is.True);

                Assert.That(properties
                    .Where(property => property.Association is not null)
                    .Select(property => property.Name), Is.EquivalentTo([
                    "activeOwnership",
                    "isPartOf",
                    "owns",
                    "user"
                ]));
            }
        }
    }
}
