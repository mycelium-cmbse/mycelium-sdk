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
    using uml4net.Extensions;
    using uml4net.SimpleClassifiers;
    using uml4net.StructuredClassifiers;

    /// <summary>
    /// Verifies the structure and semantics of the FunctionalData UML model.
    /// </summary>
    [TestFixture]
    public class FunctionalDataModelValidationTestFixture
    {
        /// <summary>
        /// The expected number of FunctionalData UML classes.
        /// </summary>
        private const int ExpectedClassCount = 13;

        /// <summary>
        /// The expected number of FunctionalData UML enumerations.
        /// </summary>
        private const int ExpectedEnumerationCount = 8;

        /// <summary>
        /// The expected number of FunctionalData UML associations.
        /// </summary>
        private const int ExpectedAssociationCount = 21;

        /// <summary>
        /// The expected number of abstract FunctionalData UML classes.
        /// </summary>
        private const int ExpectedAbstractClassCount = 2;

        /// <summary>
        /// The expected number of concrete FunctionalData UML classes.
        /// </summary>
        private const int ExpectedConcreteClassCount = 11;

        /// <summary>
        /// The expected name of the project-lifecycle enumeration.
        /// </summary>
        private const string LifecycleEnumerationName = "ProjectLifecycleKind";

        /// <summary>
        /// The expected names of the abstract FunctionalData UML classes.
        /// </summary>
        private static readonly string[] ExpectedAbstractClassNames =
        [
            "Thing",
            "AuditableThing"
        ];

        /// <summary>
        /// The FunctionalData UML classes loaded for validation.
        /// </summary>
        private IClass[] classes = [];
        
        /// <summary>
        /// The FunctionalData UML enumerations loaded for validation.
        /// </summary>
        private IEnumeration[] enumerations = [];
        
        /// <summary>
        /// The FunctionalData UML associations loaded for validation.
        /// </summary>
        private IAssociation[] associations = [];

        /// <summary>
        /// Loads the FunctionalData UML elements used by the fixture's tests.
        /// </summary>
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

        /// <summary>
        /// Verifies that the expected UML classes are abstract or concrete.
        /// </summary>
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
                Assert.That(abstractClassNames, Has.Length.EqualTo(ExpectedAbstractClassCount));
                Assert.That(abstractClassNames, Is.EquivalentTo(ExpectedAbstractClassNames));
                Assert.That(concreteClassNames, Has.Length.EqualTo(ExpectedConcreteClassCount));
                Assert.That(concreteClassNames, Is.EquivalentTo(expectedConcreteClassNames));
            });
        }

        /// <summary>
        /// Verifies the association ends, semantic signatures, and multiplicities.
        /// </summary>
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

                        AssertValidMultiplicity(associationEnd, $"Association end '{DescribeAssociationEnd(associationEnd)}'");
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
        
        /// <summary>
        /// Verifies that named association ends are owned by UML classes and returned by property queries.
        /// </summary>
        [Test]
        public void Verify_that_named_association_ends_are_class_owned_and_queryable()
        {
            Assert.Multiple(() =>
            {
                foreach (var association in this.associations)
                {
                    foreach (var associationEnd in association.MemberEnd
                                 .Where(end => !string.IsNullOrWhiteSpace(end.Name)))
                    {
                        Assert.That(associationEnd.Owner,
                            Is.InstanceOf<IClass>(),
                            $"Named association end '{DescribeAssociationEnd(associationEnd)}' "
                            + "must be owned by a UML class.");

                        if (associationEnd.Owner is not IClass owningClass)
                        {
                            continue;
                        }

                        Assert.That(owningClass.QueryAllProperties().Select(property => property.XmiId),
                            Does.Contain(associationEnd.XmiId),
                            $"UML4NET QueryAllProperties() did not return " + $"'{owningClass.Name}.{associationEnd.Name}'.");
                    }
                }
            });
        }

        /// <summary>
        /// Verifies that the UML class names are expected and unique.
        /// </summary>
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

        /// <summary>
        /// Verifies that the UML enumeration names are expected, unique, and correctly spelled.
        /// </summary>
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
            });
        }

        /// <summary>
        /// Verifies that UML generalizations and property types are resolved.
        /// </summary>
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

        /// <summary>
        /// Verifies that the model contains the expected numbers of classes, enumerations, and associations.
        /// </summary>
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

        /// <summary>
        /// Verifies that every UML class property has a valid multiplicity.
        /// </summary>
        [Test]
        public void Verify_that_property_multiplicities_are_valid()
        {
            Assert.Multiple(() =>
            {
                foreach (var umlClass in this.classes)
                {
                    foreach (var property in umlClass.OwnedAttribute)
                    {
                        AssertValidMultiplicity(property, $"Property '{umlClass.Name}.{property.Name}'");
                    }
                }
            });
        }

        /// <summary>
        /// Asserts that a UML multiplicity has a nonnegative lower bound and a valid upper bound.
        /// </summary>
        /// <param name="multiplicity">
        /// The UML multiplicity element to validate.
        /// </param>
        /// <param name="description">
        /// The description included in assertion messages.
        /// </param>
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

        /// <summary>
        /// Creates a readable description of a UML association.
        /// </summary>
        /// <param name="association">
        /// The UML association to describe.
        /// </param>
        /// <returns>
        /// The readable association description.
        /// </returns>
        private static string DescribeAssociation(IAssociation association)
        {
            var description = string.Join(" <-> ", association.MemberEnd.Select(DescribeAssociationEnd));

            return string.IsNullOrEmpty(description) ? "<unnamed association>" : description;
        }

        /// <summary>
        /// Creates a readable description of a UML association end.
        /// </summary>
        /// <param name="associationEnd">
        /// The UML association end to describe.
        /// </param>
        /// <returns>
        /// The readable association-end description.
        /// </returns>
        private static string DescribeAssociationEnd(IProperty associationEnd)
        {
            var typeName = associationEnd.Type?.Name ?? "<unresolved>";
            var roleName = associationEnd.Name ?? "<unnamed>";

            return $"{typeName}:{roleName}";
        }
    }
}
