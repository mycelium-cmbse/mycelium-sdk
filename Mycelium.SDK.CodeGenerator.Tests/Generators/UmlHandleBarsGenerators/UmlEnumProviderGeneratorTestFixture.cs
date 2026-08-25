// ------------------------------------------------------------------------------------------------
//  <copyright file="UmlEnumProviderGeneratorTestFixture.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.CodeGenerator.Tests.Generators.UmlHandleBarsGenerators
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Mycelium.SDK;
    using Mycelium.SDK.CodeGenerator.Generators.UmlHandleBarsGenerators;
    using Mycelium.SDK.CodeGenerator.Tests.Expected;
    using Mycelium.SDK.Extensions;

    using uml4net.SimpleClassifiers;
    using uml4net.StructuredClassifiers;
    using uml4net.xmi.Readers;

    [TestFixture]
    public class UmlEnumProviderGeneratorTestFixture
    {
        private const string TemplateName = "enumprovider-uml-template";

        private static readonly UTF8Encoding StrictUtf8WithoutBom = new(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

        private static readonly ProviderContract[] ProviderContracts =
        [
            CreateProviderContract<ActivationStatus>(
                nameof(ActivationStatus),
                ActivationStatusProvider.Parse,
                ActivationStatusProvider.TryParse,
                ActivationStatusProvider.Format),

            CreateProviderContract<CommentStatus>(
                nameof(CommentStatus),
                CommentStatusProvider.Parse,
                CommentStatusProvider.TryParse,
                CommentStatusProvider.Format),

            CreateProviderContract<OrganizationMembershipRole>(
                nameof(OrganizationMembershipRole),
                OrganizationMembershipRoleProvider.Parse,
                OrganizationMembershipRoleProvider.TryParse,
                OrganizationMembershipRoleProvider.Format),

            CreateProviderContract<ProjectLifecycleKind>(
                nameof(ProjectLifecycleKind),
                ProjectLifecycleKindProvider.Parse,
                ProjectLifecycleKindProvider.TryParse,
                ProjectLifecycleKindProvider.Format),

            CreateProviderContract<ProjectMemberRole>(
                nameof(ProjectMemberRole),
                ProjectMemberRoleProvider.Parse,
                ProjectMemberRoleProvider.TryParse,
                ProjectMemberRoleProvider.Format),

            CreateProviderContract<ProjectMode>(
                nameof(ProjectMode),
                ProjectModeProvider.Parse,
                ProjectModeProvider.TryParse,
                ProjectModeProvider.Format),

            CreateProviderContract<ProjectVisibility>(
                nameof(ProjectVisibility),
                ProjectVisibilityProvider.Parse,
                ProjectVisibilityProvider.TryParse,
                ProjectVisibilityProvider.Format),

            CreateProviderContract<ReviewStatus>(
                nameof(ReviewStatus),
                ReviewStatusProvider.Parse,
                ReviewStatusProvider.TryParse,
                ReviewStatusProvider.Format)
        ];

        private DirectoryInfo committedDirectory = null!;
        private DirectoryInfo expectedDirectory = null!;
        private DirectoryInfo stagingDirectory = null!;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.committedDirectory = new DirectoryInfo(
                Path.Combine(
                    TestContext.CurrentContext.TestDirectory,
                    "Committed",
                    "Mycelium.SDK.Extensions",
                    "AutoGenEnumProvider"));

            this.expectedDirectory = new DirectoryInfo(
                Path.Combine(
                    TestContext.CurrentContext.TestDirectory,
                    "Expected",
                    "UML",
                    "AutoGenEnumProvider"));

            this.stagingDirectory =
                GeneratorSetupFixture.QueryFreshOutputDirectory("_Mycelium.SDK.Extensions.AutoGenEnumProvider");

            var generator = new UmlEnumProviderGenerator();

            await generator.GenerateAsync(GeneratorSetupFixture.ResourcesDirectory, this.stagingDirectory);
        }

        [Test]
        public void Verify_that_golden_staged_and_committed_manifests_are_exact()
        {
            var expectedFileNames = QueryExpectedFileNames();
            var goldenFileNames = QueryRelativeFileNames(this.expectedDirectory);
            var stagedFileNames = QueryRelativeFileNames(this.stagingDirectory);

            var committedFileNames = this.committedDirectory.Exists
                ? QueryRelativeFileNames(this.committedDirectory)
                : Array.Empty<string>();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(
                    this.committedDirectory.Exists,
                    Is.True,
                    "The committed enumeration-provider directory was not copied to the test output.");

                Assert.That(expectedFileNames, Has.Length.EqualTo(8));

                Assert.That(
                    goldenFileNames,
                    Is.EqualTo(expectedFileNames),
                    "The reviewed provider golden manifest contains missing or extra files.");

                Assert.That(
                    stagedFileNames,
                    Is.EqualTo(expectedFileNames),
                    "The staged provider manifest contains missing or extra files.");

                Assert.That(
                    committedFileNames,
                    Is.EqualTo(expectedFileNames),
                    "The committed provider manifest contains missing or extra files.");
            }
        }

        [TestCaseSource(typeof(ExpectedEnumerations))]
        [Category("Expected")]
        public async Task Verify_that_every_generated_provider_matches_its_golden_exactly(string enumerationName)
        {
            var fileName = $"{enumerationName}Provider.cs";

            var expectedBytes = await File.ReadAllBytesAsync(
                Path.Combine(this.expectedDirectory.FullName, fileName));

            var generatedBytes = await File.ReadAllBytesAsync(
                Path.Combine(this.stagingDirectory.FullName, fileName));

            Assert.That(
                generatedBytes,
                Is.EqualTo(expectedBytes),
                $"Generated provider '{fileName}' differs byte-for-byte from its approved golden.");
        }

        [Test]
        public async Task Verify_that_complete_staged_output_matches_committed_providers()
        {
            Assert.That(
                this.committedDirectory.Exists,
                Is.True,
                "The committed enumeration-provider directory was not copied to the test output.");

            if (!this.committedDirectory.Exists)
            {
                return;
            }

            var stagedFileNames = QueryRelativeFileNames(this.stagingDirectory);
            var committedFileNames = QueryRelativeFileNames(this.committedDirectory);

            Assert.That(
                stagedFileNames,
                Is.EqualTo(committedFileNames),
                "The staged and committed provider manifests differ.");

            foreach (var fileName in stagedFileNames)
            {
                var stagedBytes = await File.ReadAllBytesAsync(
                    Path.Combine(this.stagingDirectory.FullName, fileName));

                var committedBytes = await File.ReadAllBytesAsync(
                    Path.Combine(this.committedDirectory.FullName, fileName));

                Assert.That(
                    stagedBytes,
                    Is.EqualTo(committedBytes),
                    $"Staged provider '{fileName}' differs byte-for-byte from the committed source.");
            }
        }

        [TestCaseSource(typeof(ExpectedEnumerations))]
        public async Task Verify_that_generated_providers_use_the_required_file_format(string enumerationName)
        {
            var fileName = $"{enumerationName}Provider.cs";

            var bytes = await File.ReadAllBytesAsync(Path.Combine(this.stagingDirectory.FullName, fileName));

            var hasUtf8Bom =
                bytes.Length >= 3
                && bytes[0] == 0xEF
                && bytes[1] == 0xBB
                && bytes[2] == 0xBF;

            var source = StrictUtf8WithoutBom.GetString(bytes);

            var sourceWithoutCrLf = source.Replace(
                "\r\n",
                string.Empty,
                StringComparison.Ordinal);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(
                    hasUtf8Bom,
                    Is.False,
                    $"Generated provider '{fileName}' contains a UTF-8 byte-order mark.");

                Assert.That(
                    source,
                    Does.Contain("\r\n"),
                    $"Generated provider '{fileName}' contains no CRLF line endings.");

                Assert.That(
                    sourceWithoutCrLf,
                    Does.Not.Contain("\r"),
                    $"Generated provider '{fileName}' contains a standalone carriage return.");

                Assert.That(
                    sourceWithoutCrLf,
                    Does.Not.Contain("\n"),
                    $"Generated provider '{fileName}' contains a standalone line feed.");

                Assert.That(
                    source,
                    Does.Contain("[GeneratedCode(\"Mycelium.SDK\", \"latest\")]"),
                    $"Generated provider '{fileName}' does not contain the generated-code marker.");

                Assert.That(
                    source,
                    Does.Contain("StringComparison.Ordinal"),
                    $"Generated provider '{fileName}' does not use ordinal comparisons.");

                Assert.That(
                    source,
                    Does.Not.Contain("StringComparison.OrdinalIgnoreCase"),
                    $"Generated provider '{fileName}' uses case-insensitive comparisons.");
            }
        }

        [Test]
        public void Verify_that_every_provider_round_trips_exact_Xmi_literals()
        {
            using (Assert.EnterMultipleScope())
            {
                foreach (var provider in ProviderContracts)
                {
                    foreach (var literal in provider.Literals)
                    {
                        var parsed = provider.Parse(literal.XmiLiteral.AsSpan());

                        Assert.That(
                            parsed,
                            Is.EqualTo(literal.Value),
                            $"{provider.EnumerationName} did not parse '{literal.XmiLiteral}'.");

                        var parsedSuccessfully = provider.TryParse(
                            literal.XmiLiteral.AsSpan(),
                            out var tryParseResult);

                        Assert.That(
                            parsedSuccessfully,
                            Is.True,
                            $"{provider.EnumerationName}.TryParse rejected '{literal.XmiLiteral}'.");

                        Assert.That(
                            tryParseResult,
                            Is.EqualTo(literal.Value),
                            $"{provider.EnumerationName}.TryParse returned the wrong value.");

                        Assert.That(
                            provider.Format(literal.Value),
                            Is.EqualTo(literal.XmiLiteral),
                            $"{provider.EnumerationName}.Format changed the XMI literal.");
                    }
                }
            }
        }

        [Test]
        public void Verify_that_every_provider_rejects_incorrect_literal_casing()
        {
            using (Assert.EnterMultipleScope())
            {
                foreach (var provider in ProviderContracts)
                {
                    foreach (var literal in provider.Literals)
                    {
                        var incorrectCase =
                            char.ToLowerInvariant(literal.XmiLiteral[0])
                            + literal.XmiLiteral[1..];

                        Assert.That(
                            () => provider.Parse(incorrectCase.AsSpan()),
                            Throws.TypeOf<ArgumentException>(),
                            $"{provider.EnumerationName}.Parse accepted '{incorrectCase}'.");

                        var parsedSuccessfully = provider.TryParse(
                            incorrectCase.AsSpan(),
                            out var result);

                        Assert.That(
                            parsedSuccessfully,
                            Is.False,
                            $"{provider.EnumerationName}.TryParse accepted '{incorrectCase}'.");

                        Assert.That(
                            result,
                            Is.EqualTo(Activator.CreateInstance(provider.EnumerationType)),
                            $"{provider.EnumerationName}.TryParse did not return its default value.");
                    }
                }
            }
        }

        [Test]
        public void Verify_that_every_provider_uses_the_required_failure_contract()
        {
            const string UnknownLiteral = "NotAnXmiLiteral";

            using (Assert.EnterMultipleScope())
            {
                foreach (var provider in ProviderContracts)
                {
                    Assert.That(
                        () => provider.Parse(UnknownLiteral.AsSpan()),
                        Throws.TypeOf<ArgumentException>(),
                        $"{provider.EnumerationName}.Parse accepted an unknown literal.");

                    var parsedSuccessfully = provider.TryParse(
                        UnknownLiteral.AsSpan(),
                        out var result);

                    Assert.That(
                        parsedSuccessfully,
                        Is.False,
                        $"{provider.EnumerationName}.TryParse accepted an unknown literal.");

                    Assert.That(
                        result,
                        Is.EqualTo(Activator.CreateInstance(provider.EnumerationType)),
                        $"{provider.EnumerationName}.TryParse did not return its default value.");

                    var undefinedValue = Enum.ToObject(provider.EnumerationType, int.MaxValue);

                    Assert.That(
                        () => provider.Format(undefinedValue),
                        Throws.TypeOf<ArgumentOutOfRangeException>(),
                        $"{provider.EnumerationName}.Format accepted an undefined value.");
                }
            }
        }

        [TestCase(ProviderPreflightFailure.InvalidEnumerationIdentifier)]
        [TestCase(ProviderPreflightFailure.InvalidLiteralIdentifier)]
        [TestCase(ProviderPreflightFailure.DuplicateLiteralIdentifier)]
        [TestCase(ProviderPreflightFailure.InvalidModelReference)]
        [TestCase(ProviderPreflightFailure.DuplicateFileName)]
        [TestCase(ProviderPreflightFailure.InvalidRenderedSyntax)]
        [TestCase(ProviderPreflightFailure.UnexpectedManifest)]
        public async Task Verify_that_provider_preflight_failure_leaves_destination_unchanged(
            ProviderPreflightFailure failure)
        {
            await AssertProviderPreflightFailureLeavesDestinationUntouched(
                failure,
                destinationExists: false);

            await AssertProviderPreflightFailureLeavesDestinationUntouched(
                failure,
                destinationExists: true);
        }

        private static void ApplyProviderPreflightFailure(
            ProviderPreflightFailure failure,
            XmiReaderResult xmiReaderResult,
            UmlEnumProviderGenerator generator)
        {
            switch (failure)
            {
                case ProviderPreflightFailure.InvalidEnumerationIdentifier:
                    QueryEnumeration(xmiReaderResult, "ReviewStatus")
                        .Name = "Review-Status";
                    break;

                case ProviderPreflightFailure.InvalidLiteralIdentifier:
                    QueryEnumeration(xmiReaderResult, "ReviewStatus")
                        .OwnedLiteral[0]
                        .Name = "Invalid-Literal";
                    break;

                case ProviderPreflightFailure.DuplicateLiteralIdentifier:
                    var reviewStatus = QueryEnumeration(xmiReaderResult, "ReviewStatus");

                    reviewStatus.OwnedLiteral[1].Name = reviewStatus.OwnedLiteral[0].Name;
                    break;

                case ProviderPreflightFailure.InvalidModelReference:
                    var functionalData = GeneratorSetupFixture.QueryFunctionalDataPackage(xmiReaderResult);

                    functionalData.PackagedElement
                        .OfType<IClass>()
                        .Single(umlClass => umlClass.Name == "Thing")
                        .OwnedAttribute
                        .Single(property => property.Name == "id")
                        .Type = null!;
                    break;

                case ProviderPreflightFailure.DuplicateFileName:
                    QueryEnumeration(xmiReaderResult, "ReviewStatus")
                        .Name = "ActivationStatus";
                    break;

                case ProviderPreflightFailure.InvalidRenderedSyntax:
                    generator.Templates[TemplateName] = (_, _) => "namespace Mycelium.SDK.Extensions\r\n{";
                    break;

                case ProviderPreflightFailure.UnexpectedManifest:
                    QueryEnumeration(xmiReaderResult, "ReviewStatus")
                        .Name = "UnexpectedStatus";
                    break;

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(failure),
                        failure,
                        "Unsupported enumeration-provider preflight failure.");
            }
        }

        private static ProviderContract CreateProviderContract<TEnum>(
            string enumerationName,
            Parser<TEnum> parse,
            TryParser<TEnum> tryParse,
            Func<TEnum, string> format)
            where TEnum : struct, Enum
        {
            var literals = ExpectedEnumerations
                .QueryLiteralNames(enumerationName)
                .Select(
                    xmiLiteral =>
                        new LiteralContract(xmiLiteral, Enum.Parse<TEnum>(xmiLiteral, ignoreCase: false)))
                .ToArray();

            return new ProviderContract(
                typeof(TEnum),
                enumerationName,
                literals,
                value => parse(value),
                (ReadOnlySpan<char> value, out object result) =>
                {
                    var parsedSuccessfully = tryParse(value, out var typedResult);

                    result = typedResult;

                    return parsedSuccessfully;
                },
                value => format((TEnum)value));
        }

        private static string[] QueryExpectedFileNames()
        {
            return new ExpectedEnumerations()
                .Select(enumerationName => $"{enumerationName}Provider.cs")
                .OrderBy(fileName => fileName, StringComparer.Ordinal)
                .ToArray();
        }

        private static IEnumeration QueryEnumeration(XmiReaderResult xmiReaderResult, string enumerationName)
        {
            var functionalData = GeneratorSetupFixture.QueryFunctionalDataPackage(xmiReaderResult);

            return functionalData.PackagedElement
                .OfType<IEnumeration>()
                .Single(
                    enumeration => string.Equals(
                        enumeration.Name,
                        enumerationName,
                        StringComparison.Ordinal));
        }

        private static string[] QueryRelativeFileNames(DirectoryInfo directory)
        {
            return directory
                .GetFiles("*", SearchOption.AllDirectories)
                .Select(
                    file => Path.GetRelativePath(directory.FullName, file.FullName))
                .OrderBy(fileName => fileName, StringComparer.Ordinal)
                .ToArray();
        }

        private static async Task AssertProviderPreflightFailureLeavesDestinationUntouched(
            ProviderPreflightFailure failure,
            bool destinationExists)
        {
            var state = destinationExists ? "Existing" : "Absent";

            var outputDirectory =
                GeneratorSetupFixture.QueryFreshOutputDirectory($"_Mycelium.SDK.InvalidAutoGenEnumProvider.{failure}.{state}");

            IReadOnlyDictionary<string, byte[]> expectedSnapshot = null;

            if (destinationExists)
            {
                outputDirectory.Create();

                await File.WriteAllBytesAsync(
                    Path.Combine(outputDirectory.FullName, "ActivationStatusProvider.cs"), new byte[] { 0x01, 0x02, 0x03 });

                var nestedDirectory = outputDirectory.CreateSubdirectory("preserve");

                await File.WriteAllTextAsync(
                    Path.Combine(nestedDirectory.FullName, "keep.txt"), "existing content", StrictUtf8WithoutBom);

                expectedSnapshot = await GeneratorSetupFixture.QueryDirectorySnapshotAsync(outputDirectory);
            }

            var xmiReaderResult = GeneratorSetupFixture.ReadFunctionalData();

            var generator = new UmlEnumProviderGenerator();

            ApplyProviderPreflightFailure(failure, xmiReaderResult, generator);

            if (failure is
                ProviderPreflightFailure.InvalidEnumerationIdentifier
                or ProviderPreflightFailure.InvalidLiteralIdentifier)
            {
                await Assert.ThatAsync(
                    () => generator.GenerateAsync(
                        xmiReaderResult,
                        outputDirectory),
                    Throws.TypeOf<ArgumentException>());
            }
            else
            {
                await Assert.ThatAsync(
                    () => generator.GenerateAsync(
                        xmiReaderResult,
                        outputDirectory),
                    Throws.TypeOf<InvalidOperationException>());
            }

            outputDirectory.Refresh();

            if (!destinationExists)
            {
                Assert.That(
                    outputDirectory.Exists,
                    Is.False,
                    $"Provider preflight failure '{failure}' created the destination directory.");

                return;
            }

            Assert.That(
                outputDirectory.Exists,
                Is.True,
                $"Provider preflight failure '{failure}' removed the existing destination.");

            await GeneratorSetupFixture.AssertDirectoryMatchesSnapshotAsync(
                outputDirectory,
                expectedSnapshot);
        }

        private delegate object ProviderParser(ReadOnlySpan<char> value);

        private delegate bool ProviderTryParser(ReadOnlySpan<char> value, out object result);

        private delegate TEnum Parser<TEnum>(ReadOnlySpan<char> value)
            where TEnum : struct, Enum;

        private delegate bool TryParser<TEnum>(ReadOnlySpan<char> value, out TEnum result)
            where TEnum : struct, Enum;

        private sealed record LiteralContract(string XmiLiteral, object Value);

        private sealed record ProviderContract(
            Type EnumerationType,
            string EnumerationName,
            IReadOnlyList<LiteralContract> Literals,
            ProviderParser Parse,
            ProviderTryParser TryParse,
            Func<object, string> Format);

        public enum ProviderPreflightFailure
        {
            InvalidEnumerationIdentifier,
            InvalidLiteralIdentifier,
            DuplicateLiteralIdentifier,
            InvalidModelReference,
            DuplicateFileName,
            InvalidRenderedSyntax,
            UnexpectedManifest
        }
    }
}