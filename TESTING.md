# TESTING.md

Conventions for **every test project in this solution** — `Mycelium.SDK.Tests/` and `Mycelium.SDK.CodeGenerator.Tests/` today, and any `*.Tests` project added later. Read this before writing or modifying a test. The existing fixtures are the reference implementations — when in doubt, follow the closest one rather than inventing a new shape.

Sections 1–4 and 6–7 are **universal**: they bind every test project regardless of what it covers. Section 5 describes the verification model of the **code-generation pipeline** specifically, and applies where generated output is involved.

## 1. Framework

- **NUnit 4**, with `NUnit3TestAdapter` and `Microsoft.NET.Test.Sdk`.
- Attributes in use: `[TestFixture]`, `[Test]`, `[TestCase]`, `[TestCaseSource]`, `[Category]`, `[OneTimeSetUp]`.
- `NUnit.Framework` is imported globally via `<Using Include="NUnit.Framework"/>` in every test project — do not add a redundant `using NUnit.Framework;`, and carry the `<Using>` into any new project's csproj.
- `Moq` is referenced by `Mycelium.SDK.Tests`; add it to another project only when a real object genuinely cannot be built. Prefer real objects over mocks whenever the real object is cheap to construct — these suites are almost entirely mock-free by design.
- Coverage is collected by `coverlet` / `dotnet-coverage` in CI (`.github/workflows/CodeQuality.yml`).

### Adding a new test project

A new test project **inherits these conventions** — it does not get to invent its own naming, assertion style or fixture shape. Start from the baseline both existing projects already share:

- named `{ProjectUnderTest}.Tests`, in a folder of the same name, and added to `Mycelium.SDK.sln`
- `<TargetFramework>net10.0</TargetFramework>`, `<IsPackable>false</IsPackable>`, `<Nullable>disable</Nullable>`
- packages: `Microsoft.NET.Test.Sdk` 18.6.0, `NUnit` 4.6.1, `NUnit.Console` 3.22.0, `NUnit3TestAdapter` 6.2.0, `coverlet.collector` 10.0.1, `coverlet.msbuild` 10.0.1 (the last two with `PrivateAssets="all"`)
- `<Using Include="NUnit.Framework"/>`
- a `ProjectReference` to the project under test

`LangVersion` and `ImplicitUsings` are the project's own call — the two existing test projects deliberately differ on both.

## 2. Naming

Test methods are named `VerifyThat<TheBehaviourBeingAsserted>`, in PascalCase with **no underscores**, spelling out the assertion in full:

```csharp
[Test]
public void VerifyThatClassNamesAreExpectedAndUnique()

[Test]
public void VerifyThatBatchGenerationProducesExactly24DtoFiles()

[Test]
public async Task VerifyThatBatchGenerationWritesNoFilesWhenModelValidationFails()
```

The name states the *outcome*, not the method under test. Do not import the `Verify{MethodUnderTest}` form used in sibling Starion repositories — this repository's house style is the sentence form above.

**Existing fixtures use the older `Verify_that_…` snake-case form and are deliberately left alone.** Do not bulk-rename them: the churn buries real changes in review for no behavioural gain. The no-underscore rule applies to every test written from now on, including new tests added to an existing snake-case fixture.

Fixture classes are named `{SubjectUnderTest}TestFixture` and live in a folder mirroring the subject's folder (`Extensions/`, `HandleBarHelpers/`, `Generators/UmlHandleBarsGenerators/`, `Xmi/`, `Dto/`, `Poco/`, `Enumeration/`).

## 3. Assertions

- Use the **constraint model only**: `Assert.That(actual, Is.EqualTo(expected))`. Never the classic `Assert.AreEqual` / `Assert.IsTrue` forms.
- Wrap consecutive, independent assertions in a multiple scope so that one failure does not hide the rest:

  ```csharp
  using (Assert.EnterMultipleScope())
  {
      Assert.That(abstractClassNames, Has.Length.EqualTo(ExpectedAbstractClassCount));
      Assert.That(abstractClassNames, Is.EquivalentTo(ExpectedAbstractClassNames));
  }
  ```

  Use it only for asserts that are genuinely independent — if a later assert would throw when an earlier one fails, guard with an early `continue`/`return` inside the scope instead (see `FunctionalDataModelValidationTestFixture`).
- For asynchronous throw checks use `Assert.ThatAsync`:

  ```csharp
  await Assert.ThatAsync(
      () => invalidGenerator.GenerateAsync(invalidReaderResult, invalidOutputDirectory),
      Throws.TypeOf<InvalidOperationException>());
  ```

- **Every assertion inside a loop, and every collection comparison, carries an explanatory message** naming the element that failed:

  ```csharp
  Assert.That(
      property.Type,
      Is.Not.Null,
      $"Property '{umlClass.Name}.{property.Name}' has no resolved type.");
  ```

  Without it, a failure in a 300-element loop is unactionable.
- Prefer `Is.EqualTo` when order matters and `Is.EquivalentTo` when it does not — and be deliberate about which. File-name lists are ordered comparisons; model inventories are equivalence comparisons.

## 4. Fixture structure

- Expensive shared setup belongs in `[OneTimeSetUp]`, not in a per-test setup — currently that means reading the XMI and running a full batch generation.
- **Centralise expensive shared loading in one helper fixture and reuse it** rather than building a second loader with its own settings. The current instance is `Mycelium.SDK.CodeGenerator.Tests/Xmi/XmiLoadingTestFixture.cs` (`ReadFunctionalData()`, `QueryFunctionalDataPackage()`). **Model loading is read-only**: a fixture may mutate the in-memory graph it gets back — that is how the negative tests corrupt a property type to exercise the preflight — but no test ever writes to `Resources/`.
- Default to **one `[Test]` per behaviour**, packing the related scenarios into several `Assert.That` calls inside a multiple scope. Do not write one `[Test]` per assertion when the setup is shared.
- When the same behaviour must be checked across many inputs, fan out with `[TestCaseSource]` over a `static readonly` array — never by copy-pasting a fixture.
- **Keep expected values in one reviewed inventory type per subject**, not scattered as literal lists across fixtures. Add to the inventory; do not introduce a competing local list. The current instances are `ExpectedClasses`, `ExpectedEnumerations` and `ExpectedAssociations` under `Mycelium.SDK.CodeGenerator.Tests/Expected/`.

## 5. The four-tier verification model (code-generation pipeline)

**Scope**: this section applies to `Mycelium.SDK.CodeGenerator.Tests` and `Mycelium.SDK.Tests`, which between them verify generated output. A test project covering hand-written code does not need four tiers. But any project that verifies a *generated* artifact should reuse this tiering rather than invent another one.

The generated SDK is verified at four independent levels; a template change — or a new UML export landing in `Resources/` — usually touches several of them, and the agent must know which failure means what. (A model change only ever reaches this repository as a **new export**; `Resources/*.xmi` is read-only. See `CLAUDE.md`.)

### Tier 1 — Model inventory

`Mycelium.SDK.CodeGenerator.Tests/Xmi/FunctionalDataModelValidationTestFixture.cs` pins the exact shape of `Resources/FunctionalData.xmi`: class / enumeration / association counts, abstract vs. concrete classes, association-end names and multiplicities, enumeration literal order and spelling, and that every generalization and property type resolves. It compares against the reviewed inventories in `Expected/ExpectedClasses.cs`, `Expected/ExpectedEnumerations.cs` and `Expected/ExpectedAssociations.cs`.

*A failure here means a new UML export has landed.* Update the reviewed inventories to match the new export, deliberately and as a reviewed decision — never to "make the test pass". The fix is **never** the other direction: `Resources/*.xmi` is read-only and is not edited to match the inventories. If the export looks wrong, that is a UML proposal for the architect, not a local edit.

### Tier 2 — Golden files

`Mycelium.SDK.CodeGenerator.Tests/Expected/UML/AutoGen{DTO,POCO,Enum}/*.cs` hold a **representative, human-reviewed subset** of the generated output, compared byte-for-byte by the `[Category("Expected")]` tests (`VerifyThatRepresentativeDTOInterfaceMatchesReviewedGoldenFile`, `VerifyThatEveryGeneratedEnumerationMatchesItsGoldenExactly`, …). A companion test asserts that the golden set contains *exactly* the representative files, so a golden file cannot be quietly added or dropped.

*A failure here means the generated contract changed.* It requires human review before the golden files are updated.

### Tier 3 — Committed-source parity

`VerifyThatCompleteBatchMatchesCommittedSDKDTOs`, `VerifyThatCompleteBatchMatchesCommittedSDKPOCOs` and `VerifyThatCompleteStagedOutputMatchesCommittedAutoGenEnum` compare the **full** generated batch against the real shipped sources in `Mycelium.SDK/AutoGen*`, which `Mycelium.SDK.CodeGenerator.Tests.csproj` links into the test output as `Committed/Mycelium.SDK/AutoGen*`.

*A failure here means the committed SDK and the generator have drifted apart.* This is the gate for the regeneration loop described in `CLAUDE.md`; it must be green before the work is considered done.

### Tier 4 — Runtime contract

`Mycelium.SDK.Tests/Dto/`, `Mycelium.SDK.Tests/Poco/` and `Mycelium.SDK.Tests/Enumeration/` reflect over the *compiled* generated types to assert the public runtime contract: namespaces, interface coverage and inheritance, property accessor shape, that a derived property is getter-only, and that collection properties are initialised (never `null`) on a freshly constructed object.

These fixtures also exercise the **hand-coded POCO partials** as behaviour, not just shape — `PocoContractTestFixture` builds a real object graph and asserts both outcomes of `IsOutsideCollaborator` plus its `InvalidOperationException` contract for every incomplete graph. A `Compute*` companion added under `Mycelium.SDK/Poco/` needs the same treatment: both outcomes and every guard.

*A failure here means the generated code compiles but behaves wrongly.* Add to these fixtures whenever a template change alters the emitted member shape.

## 6. Failure-path coverage is mandatory

A fixture that only proves the happy path is incomplete, whatever the subject under test. Every documented throw needs a test that provokes it, and every guard clause needs a case that trips it — an `<exception>` tag with no corresponding test is an untested contract.

**Generators additionally must prove the all-or-nothing guarantee**, since a partially-written batch is the failure mode that matters most:

- `VerifyThatBatchGenerationWritesNoFilesWhenModelValidationFails` (DTO/POCO) — corrupts the in-memory model, asserts `InvalidOperationException`, and then asserts the output directory **was never created**.
- `VerifyThatSingleEnumIdentifierFailureOccursBeforeDestinationCreation` and `VerifyThatBatchPreflightFailureLeavesNoNewOrModifiedOutput` (Enum) — same contract for the enumeration path.
- `VerifyThatEnumAndLiteralKeywordsAreEscaped` — proves reserved-keyword escaping actually reaches the output.

When adding a generator or a new preflight check, add the equivalent negative test.

## 7. Determinism

A test that passes or fails depending on machine locale, filesystem ordering or a leftover file is worse than no test. These two rules are universal:

- Compare and sort with `StringComparer.Ordinal` / `StringComparison.Ordinal`, never with culture-sensitive defaults.
- Sort collections explicitly before comparing (`.OrderBy(fileName => fileName, StringComparer.Ordinal)`); never rely on `DirectoryInfo.GetFiles` — or any other source's — enumeration order.

These two apply specifically to tests that compare **generated text**:

- Delete and recreate the staging directory in `[OneTimeSetUp]` so a stale file from a previous run cannot mask a missing output.
- Golden and committed files are pinned to CRLF by `.gitattributes`; the generator normalises to CRLF and writes UTF-8 without BOM. Do not re-save those files with different line endings or a BOM — it will break every byte-for-byte comparison at once.
