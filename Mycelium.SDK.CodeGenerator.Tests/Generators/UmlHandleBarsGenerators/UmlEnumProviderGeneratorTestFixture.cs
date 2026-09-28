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
    using System.Reflection;
    using System.Runtime.ExceptionServices;
    using System.Text;

    using Mycelium.SDK.CodeGenerator.Generators.UmlHandleBarsGenerators;
    using Mycelium.SDK.CodeGenerator.Tests.Expected;

    using uml4net.SimpleClassifiers;
    using uml4net.xmi.Readers;

    [TestFixture]
    public class UmlEnumProviderGeneratorTestFixture
    {
        private static readonly UTF8Encoding StrictUtf8WithoutBom = new(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

        private DirectoryInfo committedDirectory = null!;

        private DirectoryInfo expectedDirectory = null!;

        private DirectoryInfo stagingDirectory = null!;

        private XmiReaderResult xmiReaderResult = null!;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.committedDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Committed", "Mycelium.SDK.Extensions", "AutoGenEnumProvider"));

            this.expectedDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Expected", "UML", "AutoGenEnumProvider"));

            this.stagingDirectory = new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "UML", "_Mycelium.SDK.Extensions.AutoGenEnumProvider"));

            if (this.stagingDirectory.Exists)
            {
                this.stagingDirectory.Delete(true);
            }

            this.xmiReaderResult = GeneratorSetupFixture.ReadFunctionalData();

            var generator = new UmlEnumProviderGenerator();

            await generator.GenerateAsync(this.xmiReaderResult, this.stagingDirectory);
        }

        [Test]
        public async Task Verify_that_complete_staged_output_matches_committed_providers()
        {
            Assert.That(this.committedDirectory.Exists, Is.True, "The committed provider directory was not copied to the test output.");

            if (!this.committedDirectory.Exists)
            {
                return;
            }

            var stagedFileNames = QueryCSharpFileNames(this.stagingDirectory);
            var committedFileNames = QueryCSharpFileNames(this.committedDirectory);

            Assert.That(stagedFileNames, Is.EqualTo(committedFileNames), "The staged and committed provider file sets differ.");

            foreach (var fileName in stagedFileNames)
            {
                var stagedSource = await File.ReadAllTextAsync(Path.Combine(this.stagingDirectory.FullName, fileName), StrictUtf8WithoutBom);

                var committedSource = await File.ReadAllTextAsync(Path.Combine(this.committedDirectory.FullName, fileName), StrictUtf8WithoutBom);

                Assert.That(stagedSource, Is.EqualTo(committedSource), $"Staged provider '{fileName}' differs from committed output.");
            }
        }

        [Test]
        public async Task Verify_that_generated_providers_use_the_required_file_format()
        {
            var generatedFiles = this.stagingDirectory.GetFiles("*.cs", SearchOption.TopDirectoryOnly)
                .OrderBy(file => file.Name, StringComparer.Ordinal)
                .ToArray();

            Assert.That(generatedFiles, Is.Not.Empty, "Provider generation produced no C# files.");

            foreach (var generatedFile in generatedFiles)
            {
                var bytes = await File.ReadAllBytesAsync(generatedFile.FullName);

                var hasUtf8Bom = bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF;

                var source = StrictUtf8WithoutBom.GetString(bytes);

                var sourceWithoutCrLf = source.Replace("\r\n", string.Empty, StringComparison.Ordinal);

                using (Assert.EnterMultipleScope())
                {
                    Assert.That(hasUtf8Bom, Is.False, $"Generated provider '{generatedFile.Name}' contains a UTF-8 byte-order mark.");

                    Assert.That(source, Does.Contain("\r\n"), $"Generated provider '{generatedFile.Name}' contains no CRLF line endings.");

                    Assert.That(sourceWithoutCrLf, Does.Not.Contain("\r"), $"Generated provider '{generatedFile.Name}' contains a standalone carriage return.");

                    Assert.That(sourceWithoutCrLf, Does.Not.Contain("\n"), $"Generated provider '{generatedFile.Name}' contains a standalone line feed.");

                    Assert.That(source, Does.Contain("[GeneratedCode(\"Mycelium.SDK\", \"latest\")]"), $"Generated provider '{generatedFile.Name}' lacks the generated-code marker.");

                    Assert.That(source, Does.Contain("StringComparison.OrdinalIgnoreCase"), $"Generated provider '{generatedFile.Name}' does not use ordinal case-insensitive comparison.");
                }
            }
        }

        [TestCaseSource(typeof(RepresentativeEnumerations))]
        [Category("Expected")]
        public async Task Verify_that_representative_providers_match_reviewed_goldens(string enumerationName)
        {
            Assert.That(this.expectedDirectory.Exists, Is.True, "The representative provider-golden directory was not copied to the test output.");

            if (!this.expectedDirectory.Exists)
            {
                return;
            }

            var fileName = $"{enumerationName}Provider.cs";
            var expectedPath = Path.Combine(this.expectedDirectory.FullName, fileName);
            var stagedPath = Path.Combine(this.stagingDirectory.FullName, fileName);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(File.Exists(expectedPath), Is.True, $"Representative provider golden '{fileName}' is missing.");

                Assert.That(File.Exists(stagedPath), Is.True, $"Representative provider '{fileName}' was not generated.");
            }

            if (!File.Exists(expectedPath) || !File.Exists(stagedPath))
            {
                return;
            }

            var expectedSource = await File.ReadAllTextAsync(expectedPath, StrictUtf8WithoutBom);

            var stagedSource = await File.ReadAllTextAsync(stagedPath, StrictUtf8WithoutBom);

            Assert.That(stagedSource, Is.EqualTo(expectedSource), $"Generated provider '{fileName}' differs from its reviewed golden.");
        }

        [TestCaseSource(typeof(RepresentativeEnumerations))]
        public void Verify_that_representative_providers_preserve_the_Xmi_contract(string enumerationName)
        {
            var enumeration = QueryEnumeration(this.xmiReaderResult, enumerationName);

            var sdkAssemblyPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Mycelium.SDK.dll");

            var extensionsAssemblyPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Mycelium.SDK.Extensions.dll");

            using (Assert.EnterMultipleScope())
            {
                Assert.That(File.Exists(sdkAssemblyPath), Is.True, "The Mycelium.SDK assembly was not copied to the test output.");

                Assert.That(File.Exists(extensionsAssemblyPath), Is.True, "The Mycelium.SDK.Extensions assembly was not copied to the test output.");
            }

            if (!File.Exists(sdkAssemblyPath) || !File.Exists(extensionsAssemblyPath))
            {
                return;
            }

            var sdkAssembly = Assembly.LoadFrom(sdkAssemblyPath);
            var extensionsAssembly = Assembly.LoadFrom(extensionsAssemblyPath);

            var enumerationType = sdkAssembly.GetType($"Mycelium.SDK.{enumerationName}");

            var providerType = extensionsAssembly.GetType($"Mycelium.SDK.Extensions.{enumerationName}Provider");

            using (Assert.EnterMultipleScope())
            {
                Assert.That(enumerationType, Is.Not.Null, $"Generated enumeration '{enumerationName}' was not found.");

                Assert.That(providerType, Is.Not.Null, $"Generated provider '{enumerationName}Provider' was not found.");
            }

            if (enumerationType is null || providerType is null)
            {
                return;
            }

            InvokeProviderContractVerification(enumerationType, providerType, enumeration);
        }

        private static void InvokeProviderContractVerification(Type enumerationType, Type providerType, IEnumeration enumeration)
        {
            var verificationMethod = typeof(UmlEnumProviderGeneratorTestFixture).GetMethod(nameof(VerifyProviderContract), BindingFlags.NonPublic | BindingFlags.Static);

            if (verificationMethod is null)
            {
                throw new InvalidOperationException($"Method '{nameof(VerifyProviderContract)}' was not found.");
            }

            try
            {
                verificationMethod.MakeGenericMethod(enumerationType)
                    .Invoke(null, [providerType, enumeration]);
            }
            catch (TargetInvocationException exception) when (exception.InnerException is not null)
            {
                ExceptionDispatchInfo.Capture(exception.InnerException)
                    .Throw();

                throw;
            }
        }

        private static void VerifyProviderContract<TEnum>(Type providerType, IEnumeration enumeration) where TEnum : struct, Enum
        {
            var parse = QueryRequiredMethod(providerType, "Parse", typeof(ReadOnlySpan<char>))
                .CreateDelegate<Parser<TEnum>>();

            var tryParse = QueryRequiredMethod(providerType, "TryParse", typeof(ReadOnlySpan<char>), typeof(TEnum).MakeByRefType())
                .CreateDelegate<TryParser<TEnum>>();

            var format = QueryRequiredMethod(providerType, "Format", typeof(TEnum))
                .CreateDelegate<Func<TEnum, string>>();

            foreach (var literal in enumeration.OwnedLiteral)
            {
                var xmiLiteral = literal.Name;

                var expectedValue = Enum.Parse<TEnum>(xmiLiteral, false);

                var parsedValue = parse(xmiLiteral.AsSpan());

                var tryParseSucceeded = tryParse(xmiLiteral.AsSpan(), out var tryParseValue);

                using (Assert.EnterMultipleScope())
                {
                    Assert.That(parsedValue, Is.EqualTo(expectedValue), $"{enumeration.Name}.Parse rejected '{xmiLiteral}'.");

                    Assert.That(tryParseSucceeded, Is.True, $"{enumeration.Name}.TryParse rejected '{xmiLiteral}'.");

                    Assert.That(tryParseValue, Is.EqualTo(expectedValue), $"{enumeration.Name}.TryParse returned the wrong value for '{xmiLiteral}'.");

                    Assert.That(format(expectedValue), Is.EqualTo(xmiLiteral), $"{enumeration.Name}.Format did not preserve '{xmiLiteral}'.");
                }

                var casingVariation = QueryCasingVariation(xmiLiteral);

                var variedCaseParsedValue = parse(casingVariation.AsSpan());

                var variedCaseTryParseSucceeded = tryParse(casingVariation.AsSpan(), out var variedCaseTryParseValue);

                using (Assert.EnterMultipleScope())
                {
                    Assert.That(variedCaseParsedValue, Is.EqualTo(expectedValue), $"{enumeration.Name}.Parse did not accept casing variation '{casingVariation}'.");

                    Assert.That(variedCaseTryParseSucceeded, Is.True, $"{enumeration.Name}.TryParse did not accept casing variation '{casingVariation}'.");

                    Assert.That(variedCaseTryParseValue, Is.EqualTo(expectedValue), $"{enumeration.Name}.TryParse returned the wrong value for casing variation '{casingVariation}'.");
                }
            }

            const string UnknownLiteral = "__not_an_xmi_literal__";

            Assert.That(() => parse(UnknownLiteral.AsSpan()), Throws.TypeOf<ArgumentException>(), $"{enumeration.Name}.Parse accepted an unknown literal.");

            var unknownSucceeded = tryParse(UnknownLiteral.AsSpan(), out var unknownValue);

            var undefinedValue = (TEnum)Enum.ToObject(typeof(TEnum), int.MaxValue);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(unknownSucceeded, Is.False, $"{enumeration.Name}.TryParse accepted an unknown literal.");

                Assert.That(unknownValue, Is.EqualTo(default(TEnum)), $"{enumeration.Name}.TryParse did not return the default value.");

                Assert.That(() => format(undefinedValue), Throws.TypeOf<ArgumentOutOfRangeException>(), $"{enumeration.Name}.Format accepted an undefined value.");
            }
        }

        private static MethodInfo QueryRequiredMethod(Type providerType, string methodName, params Type[] parameterTypes)
        {
            return providerType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static, null, parameterTypes, null) ?? throw new InvalidOperationException($"Provider method '{providerType.FullName}.{methodName}' was not found.");
        }

        private static IEnumeration QueryEnumeration(XmiReaderResult xmiReaderResult, string enumerationName)
        {
            var functionalData = GeneratorSetupFixture.QueryFunctionalDataPackage(xmiReaderResult);

            return functionalData.PackagedElement.OfType<IEnumeration>()
                .Single(enumeration => string.Equals(enumeration.Name, enumerationName, StringComparison.Ordinal));
        }

        private static string QueryCasingVariation(string value)
        {
            var characters = value.ToCharArray();

            for (var index = 0; index < characters.Length; index++)
            {
                if (!char.IsLetter(characters[index]))
                {
                    continue;
                }

                characters[index] = char.IsUpper(characters[index]) ? char.ToLowerInvariant(characters[index]) : char.ToUpperInvariant(characters[index]);

                return new string(characters);
            }

            throw new InvalidOperationException($"XMI literal '{value}' has no character whose casing can be changed.");
        }

        private static string[] QueryCSharpFileNames(DirectoryInfo directory)
        {
            return directory.GetFiles("*.cs", SearchOption.TopDirectoryOnly)
                .Select(file => file.Name)
                .OrderBy(fileName => fileName, StringComparer.Ordinal)
                .ToArray();
        }

        private delegate TEnum Parser<TEnum>(ReadOnlySpan<char> value) where TEnum : struct, Enum;

        private delegate bool TryParser<TEnum>(ReadOnlySpan<char> value, out TEnum result) where TEnum : struct, Enum;
    }
}
