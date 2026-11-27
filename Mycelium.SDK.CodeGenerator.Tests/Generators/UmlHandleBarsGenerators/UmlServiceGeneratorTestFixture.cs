// ------------------------------------------------------------------------------------------------
//  <copyright file="UmlServiceGeneratorTestFixture.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Tests.Generators.UmlHandleBarsGenerators
{
    using Mycelium.SDK.CodeGenerator.Extensions;
    using Mycelium.SDK.CodeGenerator.Generators.UmlHandleBarsGenerators;
    using Mycelium.SDK.CodeGenerator.Tests.Expected;

    using uml4net.Classification;
    using uml4net.Extensions;
    using uml4net.SimpleClassifiers;
    using uml4net.StructuredClassifiers;
    using uml4net.xmi.Readers;

    [TestFixture]
    public class UmlServiceGeneratorTestFixture
    {
        private const string ServiceProviderFileName = "ThingServiceProvider.cs";

        private Dictionary<string, IClass> classes;

        private DirectoryInfo expectedDirectory;

        private DirectoryInfo stagingDirectory;

        private XmiReaderResult xmiReaderResult;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.expectedDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Expected", "UML", "AutoGenServiceImplementations"));

            this.stagingDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "UML", "_Mycelium.Fabric.AutoGenServiceImplementations"));

            if (this.stagingDirectory.Exists)
            {
                this.stagingDirectory.Delete(true);
            }

            this.xmiReaderResult = GeneratorSetupFixture.ReadFunctionalData();

            this.classes = QueryClasses(this.xmiReaderResult).ToDictionary(umlClass => umlClass.Name, StringComparer.Ordinal);

            await new UmlServiceGenerator().GenerateAsync(this.xmiReaderResult, this.stagingDirectory);
        }

        [Test]
        [Category("Expected")]
        public async Task VerifyThatRepresentativeServicesMatchTheirGoldens()
        {
            Assert.That(this.expectedDirectory.Exists, Is.True, "The representative service golden directory Expected/UML/AutoGenServiceImplementations is missing.");

            if (!this.expectedDirectory.Exists)
            {
                return;
            }

            var expectedFileNames = new RepresentativeClasses()
                .Where(className => this.classes.TryGetValue(className, out var umlClass) && !umlClass.IsAbstract)
                .Select(className => $"{className}Service.cs")
                .Append(ServiceProviderFileName)
                .OrderBy(fileName => fileName, StringComparer.Ordinal)
                .ToArray();

            var goldenFileNames = GeneratedOutput.QueryRelativeFileNames(this.expectedDirectory);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(goldenFileNames, Is.EqualTo(expectedFileNames), "The service golden set must contain exactly the concrete representative classes and the service provider.");

                foreach (var fileName in expectedFileNames.Intersect(goldenFileNames, StringComparer.Ordinal))
                {
                    var stagedSource = await GeneratorSetupFixture.ReadSourceAsync(Path.Combine(this.stagingDirectory.FullName, fileName));
                    var goldenSource = await GeneratorSetupFixture.ReadSourceAsync(Path.Combine(this.expectedDirectory.FullName, fileName));

                    Assert.That(stagedSource, Is.EqualTo(goldenSource), $"Generated service file '{fileName}' differs from its reviewed golden.");
                }
            }
        }

        [Test]
        public async Task VerifyThatACreatedPartReceivesTheFirstLiteralOfARequiredEnumerationWithoutDefault()
        {
            var partsWithEnumeration = this.classes.Values
                .Where(umlClass => !umlClass.IsAbstract)
                .SelectMany(umlClass => umlClass.QueryRequiredPartProperties().Select(property => (Owner: umlClass, Part: property)))
                .SelectMany(ownedPart => ownedPart.Part.QueryCreatedPartValueProperties()
                    .Where(partProperty => partProperty.Type is IEnumeration && !partProperty.QueryHasDefaultValue())
                    .Select(partProperty => (ownedPart.Owner, PartProperty: partProperty, Enumeration: (IEnumeration)partProperty.Type)))
                .OrderBy(candidate => candidate.Owner.Name, StringComparer.Ordinal)
                .ToArray();

            Assert.That(partsWithEnumeration, Is.Not.Empty, "The packaged model must contain a created part with a required enumeration without default for this check to be meaningful.");

            if (partsWithEnumeration.Length == 0)
            {
                return;
            }

            var (owner, partProperty, enumeration) = partsWithEnumeration[0];

            var source = await GeneratorSetupFixture.ReadSourceAsync(Path.Combine(this.stagingDirectory.FullName, $"{owner.Name}Service.cs"));

            Assert.That(source, Does.Contain($"{partProperty.QueryPropertyName()} = {enumeration.Name}.{enumeration.OwnedLiteral.First().Name},"),
                $"The part created by the service of '{owner.Name}' does not receive the first literal of '{enumeration.Name}' for '{partProperty.Name}'.");
        }

        [Test]
        public async Task VerifyThatGenerationFailsForARequiredPartOfAbstractType()
        {
            var abstractModel = GeneratorSetupFixture.ReadFunctionalData();

            var (owner, partProperty) = QueryFirstRequiredPart(abstractModel);

            ((IClass)partProperty.Type).IsAbstract = true;

            var outputDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "UML", "_AbstractPartService"));

            if (outputDirectory.Exists)
            {
                outputDirectory.Delete(true);
            }

            using (Assert.EnterMultipleScope())
            {
                await Assert.ThatAsync(() => new UmlServiceGenerator().GenerateAsync(abstractModel, outputDirectory),
                    Throws.InvalidOperationException.With.Message.Contains(partProperty.Describe()).And.Message.Contains("abstract"),
                    $"A required part of abstract type in '{owner.Name}' must fail generation.");

                Assert.That(outputDirectory.Exists, Is.False, "A batch with a required part of abstract type must not be written.");
            }
        }

        [Test]
        public async Task VerifyThatGenerationFailsForARequiredValueOfACreatedPartWithoutSource()
        {
            var unsourcedModel = GeneratorSetupFixture.ReadFunctionalData();

            var (_, partProperty) = QueryFirstRequiredPart(unsourcedModel);

            var textType = QueryClasses(unsourcedModel)
                .SelectMany(umlClass => umlClass.QueryDtoInterfaceProperties())
                .Select(property => property.Type)
                .OfType<IPrimitiveType>()
                .First(primitiveType => primitiveType.QueryCSharpTypeName() == "string");

            var requiredValue = ((IClass)partProperty.Type).QueryDtoImplementationProperties()
                .First(property => property.Lower > 0 && !property.QueryIsManaged() && !property.QueryHasDefaultValue() && property.Type is IPrimitiveType { Name: not null } primitiveType &&
                    primitiveType.QueryCSharpTypeName() == "bool");

            requiredValue.Type = textType;

            var outputDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "UML", "_UnsourcedPartService"));

            if (outputDirectory.Exists)
            {
                outputDirectory.Delete(true);
            }

            using (Assert.EnterMultipleScope())
            {
                await Assert.ThatAsync(() => new UmlServiceGenerator().GenerateAsync(unsourcedModel, outputDirectory),
                    Throws.InvalidOperationException.With.Message.Contains(requiredValue.Describe()).And.Message.Contains(partProperty.Describe()));

                Assert.That(outputDirectory.Exists, Is.False, "A batch with a required value of a created part without source must not be written.");
            }
        }

        [Test]
        public async Task VerifyThatGenerationRejectsMissingArguments()
        {
            var generator = new UmlServiceGenerator();

            using (Assert.EnterMultipleScope())
            {
                await Assert.ThatAsync(() => generator.GenerateAsync((XmiReaderResult)null, this.stagingDirectory), Throws.ArgumentNullException);

                await Assert.ThatAsync(() => generator.GenerateAsync(this.xmiReaderResult, null), Throws.ArgumentNullException);
            }
        }

        private static IEnumerable<IClass> QueryClasses(XmiReaderResult model)
        {
            return GeneratorSetupFixture.QueryFunctionalDataPackage(model).QueryPackages()
                .SelectMany(package => package.PackagedElement.OfType<IClass>());
        }

        private static (IClass Owner, IProperty PartProperty) QueryFirstRequiredPart(XmiReaderResult model)
        {
            return QueryClasses(model)
                .Where(umlClass => !umlClass.IsAbstract)
                .OrderBy(umlClass => umlClass.Name, StringComparer.Ordinal)
                .SelectMany(umlClass => umlClass.QueryRequiredPartProperties()
                    .Where(property => ((IClass)property.Type).QueryDtoImplementationProperties()
                        .Any(partProperty => !partProperty.QueryHasDefaultValue() && partProperty.Type is IPrimitiveType primitiveType && primitiveType.QueryCSharpTypeName() == "bool"))
                    .Select(property => (Owner: umlClass, PartProperty: property)))
                .First();
        }
    }
}
