// ------------------------------------------------------------------------------------------------
//  <copyright file="FunctionalDataModelValidationTestFixture.cs" company="Starion Group S.A.">
// 
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
// 
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Tests.Xmi
{
    using Mycelium.SDK.CodeGenerator.Tests.Expected;

    using uml4net.Classification;
    using uml4net.CommonStructure;
    using uml4net.SimpleClassifiers;
    using uml4net.StructuredClassifiers;

    [TestFixture]
    public class FunctionalDataModelValidationTestFixture
    {
        private const int ExpectedClassCount = 13;
        private const int ExpectedEnumerationCount = 8;
        private const int ExpectedAssociationCount = 20;
        private const int ExpectedAbstractClassCount = 2;
        private const int ExpectedConcreteClassCount = 11;

        private const string LifecycleEnumerationName =
            "ProjectLifecyleKind";

        private static readonly string[] ExpectedAbstractClassNames =
        [
            "Thing",
            "AuditableThing"
        ];

        private IClass[] classes = [];
        private IEnumeration[] enumerations = [];
        private IAssociation[] associations = [];

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var result = XmiLoadingTestFixture.ReadFunctionalData();

            var functionalData = XmiLoadingTestFixture.QueryFunctionalDataPackage(result);

            this.classes = functionalData.PackagedElement
                .OfType<IClass>()
                .ToArray();

            this.enumerations = functionalData.PackagedElement
                .OfType<IEnumeration>()
                .ToArray();

            this.associations = functionalData.PackagedElement
                .OfType<IAssociation>()
                .ToArray();
        }

        [Test]
        public void Verify_that_abstract_and_concrete_classes_are_correct()
        {
            var abstractClassNames = this.classes
                .Where(umlClass => umlClass.IsAbstract)
                .Select(umlClass => umlClass.Name)
                .ToArray();

            var concreteClassNames = this.classes
                .Where(umlClass => !umlClass.IsAbstract)
                .Select(umlClass => umlClass.Name)
                .ToArray();

            var expectedConcreteClassNames = new ExpectedClasses()
                .Except(ExpectedAbstractClassNames, StringComparer.Ordinal)
                .ToArray();

            Assert.Multiple(() =>
            {
                Assert.That(
                    abstractClassNames,
                    Has.Length.EqualTo(ExpectedAbstractClassCount));

                Assert.That(
                    abstractClassNames,
                    Is.EquivalentTo(ExpectedAbstractClassNames));

                Assert.That(
                    concreteClassNames,
                    Has.Length.EqualTo(ExpectedConcreteClassCount));

                Assert.That(
                    concreteClassNames,
                    Is.EquivalentTo(expectedConcreteClassNames));
            });
        }

        [Test]
        public void Verify_that_association_ends_and_multiplicities_are_correct()
        {
            var actualSignatures = new List<string>();

            Assert.Multiple(() =>
            {
                foreach (var association in this.associations)
                {
                    var description = DescribeAssociation(association);

                    Assert.That(
                        association.MemberEnd,
                        Has.Count.EqualTo(2),
                        $"Association '{description}' must have exactly two member ends.");

                    if (association.MemberEnd.Count != 2)
                    {
                        continue;
                    }

                    foreach (var associationEnd in association.MemberEnd)
                    {
                        Assert.That(
                            associationEnd.Type,
                            Is.Not.Null,
                            $"Association end '{DescribeAssociationEnd(associationEnd)}' has no resolved type.");

                        AssertValidMultiplicity(
                            associationEnd,
                            $"Association end '{DescribeAssociationEnd(associationEnd)}'");
                    }

                    var firstEnd = association.MemberEnd[0];
                    var secondEnd = association.MemberEnd[1];
                    var firstType = firstEnd.Type;
                    var secondType = secondEnd.Type;

                    if (firstType is null || secondType is null)
                    {
                        continue;
                    }

                    actualSignatures.Add(
                        ExpectedAssociations.CreateSignature(
                            firstType.Name,
                            firstEnd.Name,
                            secondType.Name,
                            secondEnd.Name));
                }

                Assert.That(
                    actualSignatures,
                    Has.Count.EqualTo(ExpectedAssociationCount),
                    "Every association must produce a semantic signature.");

                Assert.That(
                    actualSignatures,
                    Is.Unique,
                    "Association signatures must be unique.");

                Assert.That(
                    actualSignatures,
                    Is.EquivalentTo(
                        new ExpectedAssociations().ToArray()));
            });
        }

        [Test]
        public void Verify_that_class_names_are_expected_and_unique()
        {
            var actualNames = this.classes
                .Select(umlClass => umlClass.Name)
                .ToArray();

            var expectedNames = new ExpectedClasses().ToArray();

            Assert.Multiple(() =>
            {
                Assert.That(actualNames, Is.Unique, "UML class names must be unique.");

                Assert.That(actualNames, Is.EquivalentTo(expectedNames));
            });
        }

        [Test]
        public void Verify_that_enumeration_names_are_expected_unique_and_exactly_spelled()
        {
            var actualNames = this.enumerations
                .Select(enumeration => enumeration.Name)
                .ToArray();

            var expectedNames = new ExpectedEnumerations().ToArray();

            Assert.Multiple(() =>
            {
                Assert.That(actualNames, Is.Unique, "UML enumeration names must be unique.");

                Assert.That(actualNames, Is.EquivalentTo(expectedNames));

                Assert.That(actualNames, Does.Contain(LifecycleEnumerationName));

                Assert.That(actualNames,
                    Does.Not.Contain("ProjectLifecycleKind"),
                    $"The exported model intentionally uses '{LifecycleEnumerationName}'.");
            });
        }

        [Test]
        public void Verify_that_generalizations_and_property_types_are_resolved()
        {
            Assert.Multiple(() =>
            {
                foreach (var umlClass in this.classes)
                {
                    foreach (var generalization in umlClass.Generalization)
                    {
                        Assert.That(
                            generalization.General,
                            Is.Not.Null,
                            $"Class '{umlClass.Name}' has an unresolved generalization.");
                    }

                    foreach (var property in umlClass.OwnedAttribute)
                    {
                        Assert.That(
                            property.Type,
                            Is.Not.Null,
                            $"Property '{umlClass.Name}.{property.Name}' has no resolved type.");
                    }
                }
            });
        }

        [Test]
        public void Verify_that_model_element_counts_are_exact()
        {
            Assert.Multiple(() =>
            {
                Assert.That(this.classes, Has.Length.EqualTo(ExpectedClassCount));

                Assert.That(this.enumerations, Has.Length.EqualTo(ExpectedEnumerationCount));

                Assert.That(this.associations, Has.Length.EqualTo(ExpectedAssociationCount));
            });
        }

        [Test]
        public void Verify_that_property_multiplicities_are_valid()
        {
            Assert.Multiple(() =>
            {
                foreach (var umlClass in this.classes)
                {
                    foreach (var property in umlClass.OwnedAttribute)
                    {
                        AssertValidMultiplicity(
                            property,
                            $"Property '{umlClass.Name}.{property.Name}'");
                    }
                }
            });
        }

        private static void AssertValidMultiplicity(IMultiplicityElement multiplicity, string description)
        {
            Assert.That(
                multiplicity.Lower,
                Is.GreaterThanOrEqualTo(0),
                $"{description} has an invalid lower multiplicity of '{multiplicity.Lower}'.");

            if (multiplicity.Upper == "*")
            {
                return;
            }

            var hasNumericUpperBound = int.TryParse(multiplicity.Upper, out var upper);

            Assert.That(
                hasNumericUpperBound,
                Is.True,
                $"{description} has an invalid upper multiplicity of '{multiplicity.Upper}'.");

            if (!hasNumericUpperBound)
            {
                return;
            }

            Assert.That(
                upper,
                Is.GreaterThanOrEqualTo(multiplicity.Lower),
                $"{description} has upper multiplicity '{upper}' below lower multiplicity '{multiplicity.Lower}'.");
        }

        private static string DescribeAssociation(IAssociation association)
        {
            var description = string.Join(" <-> ", association.MemberEnd.Select(DescribeAssociationEnd));

            return string.IsNullOrEmpty(description)
                ? "<unnamed association>"
                : description;
        }

        private static string DescribeAssociationEnd(IProperty associationEnd)
        {
            var typeName = associationEnd.Type?.Name ?? "<unresolved>";

            var roleName = associationEnd.Name ?? "<unnamed>";

            return $"{typeName}:{roleName}";
        }
    }
}
