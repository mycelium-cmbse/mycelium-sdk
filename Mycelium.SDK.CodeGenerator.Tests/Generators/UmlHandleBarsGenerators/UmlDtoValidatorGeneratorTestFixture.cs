// ------------------------------------------------------------------------------------------------
//  <copyright file="UmlDtoValidatorGeneratorTestFixture.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Tests.Generators.UmlHandleBarsGenerators
{
    using System.Reflection;

    using FluentValidation;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    using Mycelium.SDK.CodeGenerator.Generators.UmlHandleBarsGenerators;
    using Mycelium.SDK.CodeGenerator.Tests.Expected;

    using uml4net.Classification;
    using uml4net.Extensions;
    using uml4net.SimpleClassifiers;
    using uml4net.StructuredClassifiers;
    using uml4net.Values;

    [TestFixture]
    public class UmlDtoValidatorGeneratorTestFixture : UmlClassGeneratorTestFixtureBase
    {
        private static readonly string[] ExpectedParentFailureProperties = ["Id", "ActiveOwnership", "CreatedBy", "User"];
        private static readonly string[] ExpectedChildFailureProperties = ["Id", "ActiveOwnership", "CreatedBy", "User", "Label"];

        private Dictionary<string, IClass> classes = [];
        private DirectoryInfo committedDirectory;
        private DirectoryInfo expectedDirectory;
        private DirectoryInfo stagingDirectory;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.committedDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Committed", "Mycelium.SDK.Validation", "AutoGenDtoValidator"));

            this.expectedDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Expected", "UML", "AutoGenDtoValidator"));

            this.stagingDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "UML", "_Mycelium.SDK.Validation.AutoGenDtoValidator"));

            if (this.stagingDirectory.Exists)
            {
                this.stagingDirectory.Delete(true);
            }

            var xmiReaderResult = GeneratorSetupFixture.ReadFunctionalData();
            var fabric = GeneratorSetupFixture.QueryFunctionalDataPackage(xmiReaderResult);

            this.classes = fabric.QueryPackages()
                .SelectMany(package => package.PackagedElement.OfType<IClass>())
                .ToDictionary(umlClass => umlClass.Name, StringComparer.Ordinal);

            var generator = new UmlDtoValidatorGenerator();

            await generator.GenerateAsync(xmiReaderResult, this.stagingDirectory);
        }

        [Test]
        public async Task VerifyThatCompleteBatchMatchesCommittedValidators()
        {
            Assert.That(this.committedDirectory.Exists, Is.True, "The committed validator directory was not copied to the test output.");

            var stagedFileNames = GeneratedOutput.QueryRelativeFileNames(this.stagingDirectory);
            var committedFileNames = GeneratedOutput.QueryRelativeFileNames(this.committedDirectory);

            Assert.That(stagedFileNames, Is.EqualTo(committedFileNames), "The staged and committed validator filename sets differ.");

            foreach (var fileName in stagedFileNames)
            {
                await AssertFilesMatchAsync(Path.Combine(this.stagingDirectory.FullName, fileName), Path.Combine(this.committedDirectory.FullName, fileName),
                    $"Generated validator '{fileName}'", "the committed production source");
            }
        }

        [Test]
        [Category("Expected")]
        public async Task VerifyThatRepresentativeValidatorsMatchReviewedGoldens()
        {
            Assert.That(this.expectedDirectory.Exists, Is.True, "The representative validator-golden directory is missing.");

            var representativeFileNames = this.QueryRepresentativeFileNames();
            var goldenFileNames = GeneratedOutput.QueryRelativeFileNames(this.expectedDirectory);

            Assert.That(goldenFileNames, Is.EqualTo(representativeFileNames), "The validator golden set must match the representative selection, including abstract classes.");

            foreach (var fileName in representativeFileNames)
            {
                await AssertFilesMatchAsync(Path.Combine(this.stagingDirectory.FullName, fileName), Path.Combine(this.expectedDirectory.FullName, fileName),
                    $"Generated validator '{fileName}'", "its reviewed golden");
            }
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task VerifyThatConcreteParentsAndChildrenCompileAndRegisterRulesOnce(bool concreteRoot)
        {
            var readerResult = GeneratorSetupFixture.ReadFunctionalData();
            var fabric = GeneratorSetupFixture.QueryFunctionalDataPackage(readerResult);
            var modelClasses = fabric.QueryPackages().SelectMany(package => package.PackagedElement.OfType<IClass>()).ToArray();
            var parent = modelClasses.Single(umlClass => umlClass.Name == "ProjectMember");
            modelClasses.Single(umlClass => umlClass.Name == "Thing").IsAbstract = !concreteRoot;

            var child = new Class { XmiId = "specialized-project-member", Name = "SpecializedProjectMember" };
            child.Generalization.Add(new Generalization { XmiId = "specialized-parent", Specific = child, General = parent });

            var label = new Property
            {
                XmiId = "specialized-label",
                Name = "label",
                Type = new PrimitiveType { XmiId = "specialized-string", Name = "String" }
            };

            label.LowerValue.Add(new LiteralInteger { Value = 1 });
            label.UpperValue.Add(new LiteralUnlimitedNatural { Value = "1" });
            child.OwnedAttribute.Add(label);
            fabric.PackagedElement.Add(child);

            var outputDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "UML", $"_ConcreteParentValidators_{concreteRoot}"));

            if (outputDirectory.Exists)
            {
                outputDirectory.Delete(true);
            }

            await new UmlDtoValidatorGenerator().GenerateAsync(readerResult, outputDirectory);

            var assembly = await CompileConcreteParentValidatorsAsync(outputDirectory);
            var parentValidator = (IValidator)Activator.CreateInstance(assembly.GetType("Mycelium.SDK.Validation.ProjectMemberValidator", throwOnError: true));
            var childValidator = (IValidator)Activator.CreateInstance(assembly.GetType("Mycelium.SDK.Validation.SpecializedProjectMemberValidator", throwOnError: true));
            var childType = assembly.GetType("Mycelium.SDK.DTO.SpecializedProjectMember", throwOnError: true);
            var parentDto = new Mycelium.SDK.DTO.ProjectMember();
            var childDto = (Mycelium.SDK.DTO.ProjectMember)Activator.CreateInstance(childType);
            var referenceId = Guid.Parse("11111111-1111-1111-1111-111111111111");

            foreach (var dto in new[] { parentDto, childDto })
            {
                dto.UpdatedBy = referenceId;
                dto.IsPartOf = referenceId;
            }

            using (Assert.EnterMultipleScope())
            {
                Assert.That(parentValidator.Validate(new ValidationContext<Mycelium.SDK.DTO.IProjectMember>(parentDto)).Errors.Select(failure => failure.PropertyName),
                    Is.EquivalentTo(ExpectedParentFailureProperties));
                Assert.That(childValidator.Validate(new ValidationContext<Mycelium.SDK.DTO.IProjectMember>(childDto)).Errors.Select(failure => failure.PropertyName),
                    Is.EquivalentTo(ExpectedChildFailureProperties));
            }

            foreach (var dto in new[] { parentDto, childDto })
            {
                dto.Id = referenceId;
                dto.CreatedBy = referenceId;
                dto.User = referenceId;
                dto.ActiveOwnership = referenceId;
            }

            childType.GetProperty("Label").SetValue(childDto, "Specialized member");

            using (Assert.EnterMultipleScope())
            {
                Assert.That(parentValidator.Validate(new ValidationContext<Mycelium.SDK.DTO.IProjectMember>(parentDto)).IsValid, Is.True);
                Assert.That(childValidator.Validate(new ValidationContext<Mycelium.SDK.DTO.IProjectMember>(childDto)).IsValid, Is.True);
            }
        }

        private static async Task<Assembly> CompileConcreteParentValidatorsAsync(DirectoryInfo outputDirectory)
        {
            const string supportSource = """
                namespace Mycelium.SDK.DTO
                {
                    public interface ISpecializedProjectMember : IProjectMember
                    {
                        string Label { get; set; }
                    }

                    public class SpecializedProjectMember : ProjectMember, ISpecializedProjectMember
                    {
                        public string Label { get; set; }
                    }
                }

                namespace Mycelium.SDK.Validation
                {
                    using FluentValidation;

                    public partial class ProjectMemberValidator<T> where T : Mycelium.SDK.DTO.IProjectMember
                    {
                        protected override void AddCustomValidation()
                        {
                            base.AddCustomValidation();

                            this.RuleFor(dto => dto.ActiveOwnership).NotNull();
                        }
                    }
                }
                """;

            var syntaxTrees = new List<SyntaxTree> { CSharpSyntaxTree.ParseText(supportSource) };

            foreach (var fileName in GeneratedOutput.QueryRelativeFileNames(outputDirectory))
            {
                syntaxTrees.Add(CSharpSyntaxTree.ParseText(await GeneratorSetupFixture.ReadSourceAsync(Path.Combine(outputDirectory.FullName, fileName))));
            }

            var referencePaths = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).Split(Path.PathSeparator)
                .Where(path => !string.Equals(Path.GetFileName(path), "Mycelium.SDK.Validation.dll", StringComparison.OrdinalIgnoreCase))
                .Append(typeof(Mycelium.SDK.DTO.IThing).Assembly.Location)
                .Append(typeof(IValidator).Assembly.Location)
                .Distinct(StringComparer.Ordinal);

            // Match the SDK's default assembly-version unification exclusions while treating other compiler warnings as errors.
            var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, generalDiagnosticOption: ReportDiagnostic.Error,
                specificDiagnosticOptions: new Dictionary<string, ReportDiagnostic> { ["CS1701"] = ReportDiagnostic.Suppress, ["CS1702"] = ReportDiagnostic.Suppress });

            var compilation = CSharpCompilation.Create($"ConcreteParentValidators_{Guid.NewGuid():N}", syntaxTrees,
                referencePaths.Select(path => MetadataReference.CreateFromFile(path)), compilationOptions);

            using var assemblyStream = new MemoryStream();
            var result = compilation.Emit(assemblyStream);

            Assert.That(result.Success, Is.True, string.Join(Environment.NewLine, result.Diagnostics));

            return Assembly.Load(assemblyStream.ToArray());
        }

        private string[] QueryRepresentativeFileNames()
        {
            return new RepresentativeClasses().Select(className => this.classes[className]).Select(umlClass => $"{umlClass.Name}Validator.cs")
                .OrderBy(fileName => fileName, StringComparer.Ordinal).ToArray();
        }
    }
}
