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
    using System.Text;

    using Mycelium.SDK.CodeGenerator.Extensions;
    using Mycelium.SDK.CodeGenerator.Tests.Generators.UmlHandleBarsGenerators;

    using uml4net.Extensions;
    using uml4net.Packages;
    using uml4net.SimpleClassifiers;
    using uml4net.StructuredClassifiers;
    using uml4net.xmi.Readers;

    [TestFixture]
    public class XmiLoadingTestFixture
    {
        private const string PrimitiveTypesUri = "http://www.omg.org/spec/UML/20161101/PrimitiveTypes.xmi";

        private static readonly string[] StandardPrimitiveNames =
        [
            "Boolean",
            "Integer",
            "Real",
            "String",
            "UnlimitedNatural"
        ];

        private static readonly string[] CustomPrimitiveNames =
        [
            "DateTime",
            "Dictionary<string,string>",
            "Guid",
            "Uri"
        ];

        private static string ResourcesDirectory => GeneratorSetupFixture.ResourcesDirectory.FullName;

        [Test]
        public void Verify_that_Enterprise_Architect_documentation_is_loaded()
        {
            var result = ReadFunctionalData();
            var functionalData = QueryFunctionalDataPackage(result);

            var thing = functionalData.PackagedElement
                .OfType<IClass>()
                .Single(umlClass => umlClass.Name == "Thing");

            Assert.That(
                thing.QueryRawDocumentation(),
                Is.EqualTo(
                    "Base class of any entities defined in the Functional Data model."));
        }

        [Test]
        public void Verify_that_FunctionalData_package_is_selected_by_name()
        {
            var result = ReadFunctionalData();
            var package = QueryFunctionalDataPackage(result);

            Assert.That(package.Name, Is.EqualTo("FunctionalData"));
        }

        [Test]
        public void Verify_that_FunctionalData_package_selection_rejects_duplicate_name()
        {
            var result = ReadFunctionalData();

            result.Packages.Add(
                new Package
                {
                    XmiId = "duplicate-functional-data",
                    Name = "FunctionalData"
                });

            Assert.That(() => QueryFunctionalDataPackage(result), Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void Verify_that_Windows_1252_encoding_is_available()
        {
            _ = ReadFunctionalData();

            Assert.That(Encoding.GetEncoding(1252).CodePage, Is.EqualTo(1252));
        }

        [Test]
        public void Verify_that_reader_uses_local_reference_settings()
        {
            var settings =
                XmiReaderResultExtensions.CreateFunctionalDataReaderSettings(GeneratorSetupFixture.ResourcesDirectory);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(settings.LocalReferenceBasePath, Is.EqualTo(ResourcesDirectory));
                Assert.That(settings.PathMaps.TryGetValue(PrimitiveTypesUri, out var primitiveTypesPath), Is.True);
                Assert.That(primitiveTypesPath, Is.EqualTo(Path.Combine(ResourcesDirectory, "PrimitiveTypes.xmi")));
                Assert.That(settings.UseStrictReading, Is.False);
            }
        }

        [Test]
        public void Verify_that_referenced_identifiers_are_resolved()
        {
            var result = ReadFunctionalData();
            var functionalData = QueryFunctionalDataPackage(result);

            var classes = functionalData.PackagedElement
                .OfType<IClass>()
                .ToArray();

            var associations = functionalData.PackagedElement
                .OfType<IAssociation>()
                .ToArray();

            using (Assert.EnterMultipleScope())
            {
                foreach (var umlClass in classes)
                {
                    foreach (var property in umlClass.OwnedAttribute)
                    {
                        Assert.That(
                            property.Type,
                            Is.Not.Null,
                            $"Property '{umlClass.Name}.{property.Name}' has no resolved type.");
                    }

                    foreach (var generalization in umlClass.Generalization)
                    {
                        Assert.That(
                            generalization.General,
                            Is.Not.Null,
                            $"Class '{umlClass.Name}' has an unresolved generalization.");
                    }
                }

                foreach (var association in associations)
                {
                    Assert.That(association.MemberEnd, Has.Count.EqualTo(2));

                    foreach (var memberEnd in association.MemberEnd)
                    {
                        Assert.That(memberEnd.Type, Is.Not.Null, "An association end has no resolved type.");
                    }
                }
            }
        }

        [Test]
        public void Verify_that_referenced_primitive_models_are_loaded()
        {
            var result = ReadFunctionalData();

            var standardPrimitives = QueryPackage(result, "PrimitiveTypes")
                .PackagedElement
                .OfType<IPrimitiveType>()
                .Select(primitive => primitive.Name)
                .ToArray();

            var customPrimitives = QueryPackage(result, "Primitives")
                .PackagedElement
                .OfType<IPrimitiveType>()
                .Select(primitive => primitive.Name)
                .ToArray();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(standardPrimitives, Is.EquivalentTo(StandardPrimitiveNames));
                Assert.That(customPrimitives, Is.EquivalentTo(CustomPrimitiveNames));
            }
        }

        [TestCase("FunctionalData")]
        [TestCase("Primitives")]
        [TestCase("PrimitiveTypes")]
        public void Verify_that_Xmi_resources_can_be_read(string expectedPackageName)
        {
            var result = ReadFunctionalData();
            var package = QueryPackage(result, expectedPackageName);

            Assert.That(package.Name, Is.EqualTo(expectedPackageName));
        }

        internal static XmiReaderResult ReadFunctionalData()
        {
            return GeneratorSetupFixture.ReadFunctionalData();
        }

        internal static IPackage QueryFunctionalDataPackage(XmiReaderResult result)
        {
            return GeneratorSetupFixture.QueryFunctionalDataPackage(result);
        }

        private static IPackage QueryPackage(XmiReaderResult result, string packageName)
        {
            return result.Packages
                .SelectMany(package => package.QueryPackages())
                .Single(package => package.Name == packageName);
        }
    }
}
