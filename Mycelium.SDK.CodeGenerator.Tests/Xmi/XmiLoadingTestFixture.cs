// ------------------------------------------------------------------------------------------------
//  <copyright file="XmiLoadingTestFixture.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Tests.Xmi
{
    using System.Xml.Linq;

    using Mycelium.SDK.CodeGenerator.Extensions;
    using Mycelium.SDK.CodeGenerator.Tests.Generators.UmlHandleBarsGenerators;

    using uml4net.Classification;
    using uml4net.Packages;
    using uml4net.SimpleClassifiers;
    using uml4net.StructuredClassifiers;
    using uml4net.xmi.Resources;

    [TestFixture]
    public class XmiLoadingTestFixture
    {
        private static DirectoryInfo ResourcesDirectory => GeneratorSetupFixture.ResourcesDirectory;

        [Test]
        public void Verify_that_FunctionalData_is_loaded_through_the_canonical_path()
        {
            var result = GeneratorSetupFixture.ReadFunctionalData();
            var package = GeneratorSetupFixture.QueryFunctionalDataPackage(result);

            Assert.That(package.Name, Is.EqualTo(XmiResources.FunctionalDataPackageName));
        }

        [Test]
        public void Verify_that_reader_uses_local_reference_settings()
        {
            var settings = ResourcesDirectory.CreateReaderSettings(useStrictReading: true);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(settings.LocalReferenceBasePath, Is.EqualTo(ResourcesDirectory.FullName));

                Assert.That(settings.PathMaps, Is.Empty);

                Assert.That(settings.UseStrictReading, Is.True);
            }
        }

        [TestCase(XmiResources.PrimitiveTypesFileName, true)]
        [TestCase(XmiResources.CSharpPrimitivesFileName, false)]
        public void Verify_that_primitive_inventories_are_loaded_without_path_maps(string resourceFileName, bool isEmbeddedResource)
        {
            var result = GeneratorSetupFixture.ReadFunctionalData();
            var resourceDocument = XDocument.Load(Path.Combine(ResourcesDirectory.FullName, resourceFileName));
            var xmiNamespace = resourceDocument.Root.GetNamespaceOfPrefix("xmi");
            var resourcePackage = resourceDocument.Root.Elements()
                .Single(element => (string)element.Attribute(xmiNamespace + "type") == "uml:Package");
            var expectedPrimitives = resourcePackage.Elements()
                .Where(element => (string)element.Attribute(xmiNamespace + "type") == "uml:PrimitiveType")
                .Select(element => (Id: (string)element.Attribute(xmiNamespace + "id"), Name: (string)element.Attribute("name")))
                .ToArray();
            var package = result.QueryRoot(null, (string)resourcePackage.Attribute("name"));
            var actualPrimitives = package.PackagedElement
                .OfType<IPrimitiveType>()
                .Select(primitive => (Id: primitive.XmiId, Name: primitive.Name))
                .ToArray();
            var localResult = ResourcesDirectory.ReadModel(resourceFileName, useStrictReading: true);
            var localPackage = localResult.QueryPackage((string)resourcePackage.Attribute("name"));
            var localPrimitives = localPackage.PackagedElement
                .OfType<IPrimitiveType>()
                .Select(primitive => (Id: primitive.XmiId, Name: primitive.Name))
                .ToArray();
            var resourceLoader = new ResourceLoader();
            var isKnownResource = resourceLoader.TryLoadKnownResource(package.DocumentName, out var embeddedStream);

            using (embeddedStream)
            using (Assert.EnterMultipleScope())
            {
                Assert.That(expectedPrimitives, Is.Not.Empty);
                Assert.That(actualPrimitives, Is.EquivalentTo(expectedPrimitives));
                Assert.That(localPrimitives, Is.EquivalentTo(expectedPrimitives));
                Assert.That(isKnownResource, Is.EqualTo(isEmbeddedResource));

                if (isEmbeddedResource)
                {
                    Assert.That(embeddedStream, Is.Not.Null);
                }
                else
                {
                    Assert.That(package.DocumentName, Is.EqualTo(resourceFileName));
                }
            }
        }

        [TestCase("BranchProtectionRule", "minimumRequiredApproval")]
        [TestCase("AuditableThing", "createdOn")]
        [TestCase("ProjectMember", "role")]
        public void Verify_that_representative_property_types_are_resolved_without_path_maps(string className, string propertyName)
        {
            var package = GeneratorSetupFixture.QueryFunctionalDataPackage(GeneratorSetupFixture.ReadFunctionalData());
            var property = QueryProperty(className, propertyName, package);
            var typeIdentifier = property.SingleValueReferencePropertyIdentifiers["type"];

            Assert.That(property.Type, Is.Not.Null);
            Assert.That(property.Type.XmiId, Is.EqualTo(typeIdentifier.Split('#')[^1]));
        }

        private static IProperty QueryProperty(string className, string propertyName, IPackage package)
        {
            var umlClass = package.PackagedElement
                .OfType<IClass>()
                .Single(candidate => candidate.Name == className);

            var property = umlClass.OwnedAttribute.Single(candidate => candidate.Name == propertyName);
            return property;
        }

        [Test]
        public void Verify_that_representative_generalization_and_association_references_are_resolved_without_path_maps()
        {
            var package = GeneratorSetupFixture.QueryFunctionalDataPackage(GeneratorSetupFixture.ReadFunctionalData());
            var umlClass = package.PackagedElement
                .OfType<IClass>()
                .Single(candidate => candidate.Name == "ProjectMember");
            var generalization = umlClass.Generalization.Single();
            var property = QueryProperty("ProjectMember", "owns", package);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(generalization.General, Is.Not.Null);
                Assert.That(property.Association, Is.Not.Null);
            }

            using (Assert.EnterMultipleScope())
            {
                Assert.That(generalization.General.XmiId, Is.EqualTo(generalization.SingleValueReferencePropertyIdentifiers["general"].Split('#')[^1]));
                Assert.That(property.Association.XmiId, Is.EqualTo(property.SingleValueReferencePropertyIdentifiers["association"].Split('#')[^1]));
                Assert.That(property.Association.MemberEnd, Does.Contain(property));
                Assert.That(property.Association.MemberEnd.Select(end => end.XmiId),
                    Is.EquivalentTo(property.Association.MultiValueReferencePropertyIdentifiers["memberEnd"].Select(identifier => identifier.Split('#')[^1])));

                foreach (var end in property.Association.MemberEnd)
                {
                    Assert.That(end.Type, Is.Not.Null);
                    Assert.That(end.Association, Is.SameAs(property.Association));
                }
            }
        }

        [TestCase("role", 1, "1")]
        [TestCase("activeOwnership", 0, "1")]
        [TestCase("owns", 0, "*")]
        public void Verify_that_representative_multiplicities_are_preserved_without_path_maps(string propertyName, int expectedLower, string expectedUpper)
        {
            var package = GeneratorSetupFixture.QueryFunctionalDataPackage(GeneratorSetupFixture.ReadFunctionalData());
            var property = QueryProperty("ProjectMember", propertyName, package);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(property.Lower, Is.EqualTo(expectedLower));
                Assert.That(property.Upper, Is.EqualTo(expectedUpper));
            }
        }
    }
}
