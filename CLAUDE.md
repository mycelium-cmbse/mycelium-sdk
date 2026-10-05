# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Mycelium.SDK is the .NET software development toolkit for the Mycelium platform. It currently ships the **FunctionalData** domain model — the organizations, projects, members, ownerships, reviews and policies that make up a Mycelium workspace — as generated DTOs, POCOs and enumerations. Current version: 0.0.1. The package is published to NuGet as `Mycelium.SDK`.

## Build & Test Commands

```bash
# Restore and build the entire solution
dotnet restore Mycelium.SDK.sln
dotnet build Mycelium.SDK.sln

# Run all tests
dotnet test Mycelium.SDK.sln

# Run tests for a specific project
dotnet test Mycelium.SDK.CodeGenerator.Tests/Mycelium.SDK.CodeGenerator.Tests.csproj
dotnet test Mycelium.SDK.Tests/Mycelium.SDK.Tests.csproj

# Run a single fixture by name
dotnet test Mycelium.SDK.CodeGenerator.Tests/Mycelium.SDK.CodeGenerator.Tests.csproj --filter "FullyQualifiedName~UmlDtoGeneratorTestFixture"

# Run with coverage (as CI does)
dotnet-coverage collect "dotnet test Mycelium.SDK.sln --no-build" -f xml -o coverage.xml
```

Test framework: **NUnit 4**. Test classes use `[TestFixture]` and `[Test]` attributes.

**When writing or modifying unit tests** in **any** `*.Tests/` project — the two that exist today or one added later: read `TESTING.md` at the repo root for the conventions (`Verify_that_…` naming, `Assert.That` everywhere, `Assert.EnterMultipleScope` for consecutive asserts, mandatory failure-path coverage, the four-tier verification model for generated output, and the baseline for a new test project).

## Architecture

### Code Generation

- Favour duplicated code in code generation so that generated members are **statically defined** and dispatched at compile time, rather than resolved through reflection. Repetition in generated output is acceptable when it buys that performance.
- Code generation is done by processing a UML model and rendering Handlebars templates.

### Code Generation Pipeline

Most code in `Mycelium.SDK/` is **auto-generated** — files carrying the banner

```
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
```

must never be edited directly. Change the **template or the helper** instead — not the model, which is read-only (see below).

The pipeline:

1. **Input** (read-only): `Resources/FunctionalData.xmi` — a UML/XMI export produced by Enterprise Architect, and the **single source of truth** for every generated DTO, POCO and enumeration. It is resolved against `Resources/PrimitiveTypes.xmi` (the OMG standard primitives: `Boolean`, `Integer`, `Real`, `String`, `UnlimitedNatural`) and `Resources/CSharp_Primitives.xmi` (the platform primitives: `DateTime`, `Guid`, `Uri`, `Dictionary<string,string>`). All three are external artifacts.
2. **Generator**: `Mycelium.SDK.CodeGenerator` reads the XMI via `uml4net.xmi` (with the Enterprise Architect extender) and renders Handlebars templates from `Mycelium.SDK.CodeGenerator/Templates/Uml/*.hbs`.
3. **Output**: `Mycelium.SDK/AutoGenDTO/`, `Mycelium.SDK/AutoGenPOCO/`, `Mycelium.SDK/AutoGenEnum/`.

There are three independent pipelines: this UML one, the OpenAPI one (Carter route modules) and the PIM one (Fabric service interfaces). Both of the latter two target the `mycelium-fabric` repository and generate nothing into `Mycelium.SDK/`.

Generator classes in `Mycelium.SDK.CodeGenerator/Generators/UmlHandleBarsGenerators/`:

| Generator | Output | Batch entry point | Single-artifact entry points |
|---|---|---|---|
| `UmlDtoGenerator` | `Mycelium.SDK/AutoGenDTO/` | `GenerateAsync` | `GenerateDataTransferObjectInterfaceAsync`, `GenerateDataTransferObjectClassAsync` |
| `UmlPocoGenerator` | `Mycelium.SDK/AutoGenPOCO/` | `GenerateAsync` | `GeneratePocoInterfaceAsync`, `GeneratePocoClassAsync` |
| `UmlEnumGenerator` | `Mycelium.SDK/AutoGenEnum/` | `GenerateAsync` | `GenerateEnumerationAsync` |

They sit on the shared bases `UmlClassHandleBarsGenerator` → `UmlHandleBarsGenerator` → `Generator`. Supporting code: Handlebars helpers in `Mycelium.SDK.CodeGenerator/HandleBarHelpers/`, UML queries in `Mycelium.SDK.CodeGenerator/Extensions/`.

XMI loading is centralised in `Mycelium.SDK.CodeGenerator.Tests/Generators/UmlHandleBarsGenerators/GeneratorSetupFixture.cs` (`ResourcesDirectory`, `ReadFunctionalData()`, `QueryFunctionalDataPackage()`). Reuse those helpers — do not hand-roll another `XmiReaderBuilder` with its own settings and path maps.

**Every model under `Resources/` is read through one pair of extension methods.** `Extensions/XmiReadingExtensions.cs` provides `DirectoryInfo.ReadModel(fileName, useStrictReading)` and `XmiReaderResult.QueryPackage(packageName)`; the file and package names are constants in `Extensions/XmiResources.cs`. Strict reading is the only setting that varies between pipelines, so a new model is one call, not a new loader class. The Enterprise Architect extender is applied unconditionally — it activates only on EA extension content, and the PIM goldens are byte-identical with it in place.

### OpenAPI-driven generation (Carter route modules)

A second, independent pipeline generates the API routes that `mycelium-fabric` must expose to comply with the OMG **Systems Modeling API and Services** specification.

- **Input** (read-only): `Resources/ptc-25-02-30.json` — the OMG OpenAPI 3.1.0 document, an external artifact on the same footing as the XMI exports.
- **Generator**: `Generators/OpenApiHandleBarsGenerators/OpenApiCarterModuleGenerator.cs` reads the document via `Microsoft.OpenApi`, groups operations by tag, and renders `Templates/OpenApi/carter-module-template.hbs`. Output is one `ICarterModule` per tag.
- **Output**: staged only, into the test project's `bin/.../OpenApi/_Mycelium.Fabric.AutoGenModules/`. The committed destination is `Mycelium.Fabric.ConcurrentServer/Modules/AutoGenModules/` in the **`mycelium-fabric` repository**, reached by the same reviewed manual copy the `AutoGen*` folders use. Nothing is generated into `Mycelium.SDK/`.

Two rules specific to this pipeline:

- **The generator emits `AddRoutes` and nothing else.** No handler declarations, no bound parameters. Each route is `app.MapX("<template>", <HandlerName>)` plus untyped metadata. Handlers are hand-written **`static`** members of the companion partial: a Carter module carries no dependencies of its own, so services reach the handler from DI through minimal-API parameter binding and query parameters are read off `HttpContext.Request.Query`. The unqualified method-group reference is what makes a missing handler a compile error. Never add generated parameter binding here.
- **The algorithm lives in the `.hbs`.** Following the SysML2.NET idiom, the template owns the loops and the branching; the C# side is `Query*` predicates/collections and `Write*` identifier emitters in `Extensions/OpenApi{Document,Operation,Tag}Extensions.cs`, registered through `HandleBarHelpers/OpenApi{Tag,Operation}Helper.cs`. Do not pre-compose C# statements in C#.

Document loading is centralised in `Mycelium.SDK.CodeGenerator.Tests/OpenApi/OpenApiLoadingTestFixture.cs` (`ReadSystemsModelingApiAsync()`). It reads with `ValidationRuleSet.GetEmptyRuleSet()` — the default rule set throws `UriFormatException` because the OMG export gives every schema a `$id` the validator cannot resolve as a `Uri`. Reuse that helper rather than building a second reader.

### PIM-driven generation (Fabric service and repository interfaces)

A third pipeline generates the service layer that the Carter modules delegate to, and the persistence layer that the services are written against, from the OMG **PIM** rather than from the REST binding. Both generators read the same model through the same loader and share the mapping helper; only their selection rule and their template differ.

- **Input** (read-only): `Resources/ptc-25-02-29.xmi` — "MOF XMI for Systems Modeling API PIM", fetched from <https://www.omg.org/spec/SystemsModelingAPI/20250201/Systems-Modeling-API.xmi>. It declares 6 service classes and **43** operations, and an `API_Model` package of **7** concrete record classes. (mycelium-fabric#10 says 88 operations; that figure is wrong.)
- **Generator**: `Generators/PimHandleBarsGenerators/PimServiceInterfaceGenerator.cs` reads the model via `uml4net.xmi` and renders `Templates/Pim/service-interface-template.hbs`. Output is one interface per service class, in namespace `Mycelium.Fabric.ConcurrentServer.Services`.
- **Output**: staged only, into the test project's `bin/.../Pim/_Mycelium.Fabric.AutoGenServices/`. The committed destination is `Mycelium.Fabric.ConcurrentServer/Services/AutoGenServices/` in the **`mycelium-fabric` repository**, reached by the same reviewed manual copy.
- **Generator**: `Generators/PimHandleBarsGenerators/PimRepositoryInterfaceGenerator.cs` renders `Templates/Pim/repository-interface-template.hbs`. Output is one interface per persisted record, in namespace `Mycelium.Fabric.ConcurrentServer.Persistence`.
- **Output**: staged only, into the test project's `bin/.../Pim/_Mycelium.Fabric.AutoGenRepositories/`. The committed destination is `Mycelium.Fabric.ConcurrentServer/Persistence/AutoGenRepositories/`, reached by the same reviewed manual copy.

Model loading is centralised in `Mycelium.SDK.CodeGenerator.Tests/Pim/PimXmiLoader.cs`, over the shared `Extensions/XmiReadingExtensions.cs`. It reads with `useStrictReading: false` — see the unresolvable reference below. Reuse that helper rather than building a second reader.

Rules specific to this pipeline:

- **A service is a class that owns operations**, not a class whose name ends in `Service`. Nothing is filtered: every operation is emitted, including the five with no REST endpoint.
- **Every generated interface extends `IBaseService`**, which Fabric **hand-writes** — it is a dependency-injection convenience, not a PIM element, so it is not generated here. The unqualified reference is what makes a missing base interface a compile error in Fabric, the same idiom the Carter modules use for their handlers.
- **The using block is a fixed literal; a foreign namespace that only a minority of the services need is written fully qualified instead of imported.** That is why the two KerML types and `SysML2.NET.Common.IData` appear qualified in the mapping tables — a fixed block stays correct without a predicate deciding which imports a given service requires. `System.Collections.Generic` is the deliberate exception: `IReadOnlyList` survives only on collection *inputs*, so four of the six files do not need it, but qualifying a BCL staple nine times reads far worse than an unused import.
- **Generated files open with `// <auto-generated />`**, before the copyright header. Roslyn only treats a file as generated when the marker is the file's *first* comment, so placing it inside the "THIS IS AN AUTOMATICALLY GENERATED FILE" banner lower down would be decorative and analyzers would still process the file. The other two pipelines do not carry the marker yet.
- **Signatures are `Task<ErrorOr<T>>`**; `[*]` becomes `IReadOnlyList<T>`, a void operation becomes `Task<ErrorOr<Success>>`, a collection-returning operation takes `SysML2.NET.PIM.QueryParameters`, and every operation ends in a `CancellationToken`. **Paging travels both ways.** The service takes `QueryParameters` so that persistence applies the page when it queries its store, and returns `SysML2.NET.PIM.PagedResult<T>` so the cursors come back with the rows. A plain `IReadOnlyList<T>` was tried and rejected: a `Page` is an epoch plus an identifier, and only 4 of the 15 collection-returning operations return a type carrying a timestamp (`Project`, `Commit`, `Branch`, `Tag`) — for the other 11 the module cannot derive the cursor from the rows, so it would need a second query. A collection *input* stays `IReadOnlyList<T>`; it is an argument, not a page. `[0..1]` becomes `T?` only when the mapped type is a value type — it is not turned into a C# optional parameter, because the PIM interleaves optional inputs with required ones and reordering them would change the declared signature.
- **The service layer is the DTO world, the PIM is the POCO world.** An input typed by a specialization of the abstract `Record` class names server-managed state the caller does not supply, so only its identifier travels: `Guid` / `Guid?` / `IReadOnlyList<Guid>`, with an `Id` / `Ids` suffix on the parameter name. An input typed by a `Data` realization or by a classifier with no identity at all (`Constraint`, `MergeResult`, `DataDifference`) carries content and keeps its `SysML2.NET.PIM.DTO.*` type. **Return types are never reduced.**
- **The decisions the PIM does not make live in the `.hbs`, as reviewed tables.** There are three, and all three are visible in the templates rather than compiled in: the §7.2 clause per service (editorial, not exported); the C# type behind the two unresolvable KerML `href`s; and the three inputs that are typed by a `Record` specialization yet carry content (`updateQuery.updateQuery`, `executeQuery.query`, `createCommit.change`) — the PIM declares every input with the same direction, so no rule derived from the model separates them. `HandleBarHelpers/PimMappingHelper.cs` exists only to look a key up in such a table and to **fail loudly** on a key it has no mapping for.
- **Type names come from uml4net's own mapping.** `IType.QueryCSharpTypeName()` covers every type kind, and the three data types the PIM declares for itself (`UUID`→`Guid`, `ISO8601DateTime`→`DateTime`, `IRI`→`Uri`) are registered through `TypeExtensions.AddOrOverwriteCSharpTypeMappings` in `PimParameterExtensions.RegisterPimCSharpTypeMappings()`. Do not hand-roll a second type table. Two consequences: that mapping is **process-wide**, so only register names that occur in no other model generated here; and `ReservedCSharpNameMapper` must be applied only to a name that came through *unmapped*, because a mapped alias such as `string` is already a legal C# type and escaping it yields `@string`.
- **`HandlebarsHelpers.Register` is required.** Without it a dotted helper name is resolved as a path expression and a helper used as the argument of a block helper cannot be reduced, so every subexpression in these templates silently renders empty or fails to compile.

Rules specific to the repository half:

- **A persisted record is a concrete class that specializes the abstract `Record` class**, found by walking the nested packages of the model rather than by naming the `API_Model` package. That yields `Project`, `Branch`, `Tag`, `Commit`, `DataVersion`, `DataIdentity` and `Query`. `Record`, `CommitReference` and `Constraint` are abstract, `DataDifference` and `MergeResult` are operation results, and `ExternalData` / `ExternalRelationship` / `ProjectUsage` realize `Data` — versioned payload reached through the `DataVersion` that wraps it, not through a repository of its own.
- **The generated file is a marker; the contract itself is hand-written once.** Fabric declares `IRecordRepository<TRecord>` with the six members every record needs — `CreateAsync`, `ReadByIdAsync`, `ReadAsync`, `ReadByFilterAsync`, `UpdateAsync` and `DeleteAsync` — and this pipeline emits `public interface IBranchRepository : IRecordRepository<Branch> { }` and six more like it. Named contracts survive for dependency injection and for mocking, without the same six signatures and their XML docs being generated seven times over. The unqualified `IRecordRepository<>` reference is what makes a missing base contract a compile error in Fabric, the same idiom as `IBaseService`.
- **The CRUD surface is uniform, and is not narrowed to what the API exposes**, even though the PIM declares `update` for `Project` and `Query` only and declares no write at all for `DataVersion` or `DataIdentity` outside a commit. The store is a store; which of its operations the specification reaches is the service layer's concern.
- **Narrowing a read takes the specification's own filter object, so nothing per-record is generated for it.** `ReadByFilterAsync` takes a `SearchFilter` — Fabric's hand-written counterpart of the §7.1.4 `Constraint` tree, a property/operator/value leaf plus an AND/OR composite — and the property is a plain string. Earlier cuts of this pipeline generated one `ReadBy{Reference}Async` per single-valued reference, then a `{Record}Filter` class per record; both made every new query dimension a generated member. Fabric carries its own filter rather than the SDK's `Constraint` because `SysML2.NET.PIM.DTO.PrimitiveConstraint` does not specialize `Constraint`, which leaves that tree with no leaf.
- **The record classes carry no documentation in the export**, so the one-line summary of a repository interface is authored in the `.hbs` with the record name interpolated, rather than taken from `{{Documentation}}` as the service interfaces are. The §7.1 clause per record is the reviewed table this template carries.

Two known specification defects are handled deliberately rather than worked around in the resource:

- **`getQueryById` declares two return parameters** (`queryId` and `query`). C# has one result, so the **last** return parameter in document order is the result and every earlier one becomes an input — which yields the PSM-correct signature and drops nothing.
- **Four `ElementNavigationService` results are typed by `href` into `KerML Abstract Syntax.xml`**, a companion export OMG does not publish; the `KerML.xmi` it does publish uses different `xmi:id`s and cannot resolve them. `uml4net` therefore leaves `Parameter.Type` null and the raw `href` is the only key, which the template maps onto `SysML2.NET.Core.DTO.Root.Elements.IElement` / `IRelationship`. Any *other* unresolved reference fails loudly.

The PIM interface `Data` maps to **`SysML2.NET.Common.IData`**, whose own documentation records the same four realizations the XMI declares (`Element`, `ExternalData`, `ExternalRelationship`, `ProjectUsage`). It is not in `SysML2.NET.PIM.DTO`, so look in `SysML2.NET.Common` before concluding a PIM type has no counterpart. Substituting `IElement` would narrow the contract to one of the four and is wrong.

Two pipeline invariants that must be preserved:

- **Batch generation is all-or-nothing.** `UmlClassHandleBarsGenerator.GenerateAsync` renders and validates the *complete* batch — including the duplicate-filename check — **before** it creates the output directory, so a model that cannot be generated leaves no partial output behind. Keep that ordering; there is a test that proves it.
- **Output is byte-for-byte deterministic on a given platform.** `Generator.CodeCleanup` Roslyn-formats the source and normalises line endings to `Environment.NewLine`, and `Generator.WriteAsync` writes UTF-8 **without** BOM. The line ending deliberately follows the platform rather than being pinned: a golden file is compared against output generated beside it, so both sides move together and the comparison holds on Windows and on `ubuntu-latest` with no `.gitattributes` entry to add for each new `AutoGen*` folder. Pinning CRLF leaves the generated side anchored while the checked-out golden follows the platform, which fails on Linux only. Deterministic ordering of properties and generalizations lives in `Mycelium.SDK.CodeGenerator/Extensions/ClassExtensions.cs` (`OrderProperties` puts `id` first, then orders case-insensitively, then ordinally, then by `XmiId`). Never introduce anything non-deterministic (timestamps, hash-ordered collections) into generated output — the whole verification model rests on exact comparison.

### The UML model is READ-ONLY

**Every file under `Resources/*.xmi` is produced by an external modelling tool (Enterprise Architect) and is consumed here read-only.** This covers `FunctionalData.xmi`, `CSharp_Primitives.xmi` and `PrimitiveTypes.xmi` alike. **Never edit one** — not a one-character change, and not temporarily to test a hypothesis.

Reason: this repository holds an *export*, not the master model. A local edit is silently destroyed by the next export, is invisible to the architect who owns the model, and desynchronises the SDK from the authoritative source. It also cannot be reviewed — nobody diffs a 217 KB XMI in a PR.

**When work genuinely requires a model change** — a new class, property, association, multiplicity or enumeration literal — stop and **write up the proposed UML improvement**: which element, what type and multiplicity, and why the generated code needs it. That proposal is shared with the **architect**, who owns the model. A developer does not update the model directly, and neither does the agent. Work resumes once a new export has been dropped into `Resources/`.

Corollaries:

- A generator that cannot handle the model must **fail loudly** (it already does). Never "work around" a modelling problem by touching the XMI — the fix is either a template/helper change or a model proposal.
- The one place a model is legitimately mutated is **in memory, inside a negative test**: `UmlDtoGeneratorTestFixture` and `UmlPocoGeneratorTestFixture` set `idProperty.Type = null!` on the object graph returned by the reader to exercise the all-or-nothing preflight. That mutates a freshly-read in-memory object, never the file on disk. It is correct as written — do not "fix" it, and do not cite it as precedent for editing `Resources/`.

### Regenerating the committed AutoGen files

**The CodeGenerator has no console entry point.** It is a library (`IsPackable=false`, no `OutputType`); there is no `dotnet run` for it. Generation happens only through the test fixtures. Do not invent a CLI command.

The loop runs when either of two things happens: **a new XMI export has been dropped into `Resources/`** by the architect, or **a `.hbs` template, Handlebars helper or generator has changed**. There is no step in which anyone edits an XMI.

1. Apply the template / helper / generator change — or, for a new export, start straight at step 2.
2. Run the code-generator tests:
   `dotnet test Mycelium.SDK.CodeGenerator.Tests/Mycelium.SDK.CodeGenerator.Tests.csproj`
3. **Look at the `Expected` golden-file comparisons first.** These are the human-reviewed representative files under `Mycelium.SDK.CodeGenerator.Tests/Expected/UML/AutoGen{DTO,POCO,Enum}/`, compared by the `[Category("Expected")]` tests. A failure there means the *reviewed contract* changed and needs a deliberate human decision — update the golden files only once the new output has been judged correct.
4. Once the `Expected` comparisons are green, **manually diff** the staged output against the committed SDK sources (WinMerge or an equivalent diff tool) to track exactly what changed. **This human review step is required and must not be skipped or simulated by the agent.**
   - staged output: `Mycelium.SDK.CodeGenerator.Tests/bin/<Configuration>/net10.0/UML/_Mycelium.SDK.AutoGen{DTO,POCO,Enum}/`
   - committed source: `Mycelium.SDK/AutoGen{DTO,POCO,Enum}/`
5. If the changes are accepted, copy the staged files from `bin` into the matching `Mycelium.SDK/AutoGen*` folder and re-run the tests. The `Verify_that_complete_batch_matches_committed_*` / `VerifyThatCompleteStagedOutputMatchesCommittedAutoGenEnum` fixtures are the gate and must be green.

The agent's role stops at step 3: it can change the template, helper or generator and run the tests, but the diff review and the copy into `Mycelium.SDK/AutoGen*` are the user's calls — and the model itself is never the agent's to change.

### DTO vs POCO Pattern

Every FunctionalData class is generated twice from the same UML class:

- **DTO** (Data Transfer Object) — namespace `Mycelium.SDK.DTO`, folder `Mycelium.SDK/AutoGenDTO/`. References to other elements become `Guid` / `Guid?` / `List<Guid>`. Derived and derived-union properties are **excluded** (`ClassExtensions.IsDtoProperty`). This is the transport/serialization shape.
- **POCO** (Plain Old CLR Object) — namespace `Mycelium.SDK.POCO`, folder `Mycelium.SDK/AutoGenPOCO/`. References become resolved interface references (`IOwnership`, `List<IOwnership>`). Derived properties **are** included, emitted as `public bool Foo => this.ComputeFoo();` (see `HandleBarHelpers/PropertyHelper.cs` → `QueryPocoImplementationSuffix`). This is the in-memory object graph.
- **Enumerations** are shared by both and generated once, into the root `Mycelium.SDK` namespace, under `Mycelium.SDK/AutoGenEnum/`.

Each form gets its own interface (`I{ClassName}`) in its own namespace; abstract UML classes produce an interface only, no concrete class. Collection properties on concrete POCOs are initialised to `[]`; optional scalars become `T?` only when the mapped type is a value type (`Extensions/PropertyExtension.cs` → `QueryPocoElementIsValueType`).

**Hand-coded POCO behaviour lives in `Mycelium.SDK/Poco/`**, never in `AutoGenPOCO/`. A derived property generated as `=> this.Compute{PropertyName}()` is satisfied by a `partial class` in `Mycelium.SDK/Poco/` supplying `private {Type} Compute{PropertyName}()`. The reference implementation is `Mycelium.SDK/Poco/ProjectMember.cs` (`ComputeIsOutsideCollaborator`) — it throws `InvalidOperationException` with a descriptive message for each missing link in the object graph rather than returning a silently wrong answer.

### Namespace Convention

- `Mycelium.SDK.DTO` — generated DTO interfaces and classes
- `Mycelium.SDK.POCO` — generated POCO interfaces and classes, plus their hand-coded partials
- `Mycelium.SDK` — generated enumerations

### Naming and identifier rules

`ReservedCSharpNameMapper.Map` (`Mycelium.SDK.CodeGenerator/Extensions/ReservedCSharpNameMapper.cs`) is the single gate for turning a modelled name into a C# identifier: it `@`-escapes reserved keywords, passes a valid identifier through unchanged, and throws `ArgumentException` when the name cannot be represented without changing the modelled value. Never hand-roll an escape or a sanitiser elsewhere.

UML class `Foo` → interface `IFoo` in both DTO and POCO namespaces. Property names are `CapitalizeFirstLetter()`-ed and then mapped.

### Project Dependency Graph

```
Mycelium.SDK (netstandard2.1; net10.0)
  ├── AutoGenDTO/   - generated DTO interfaces + classes (Mycelium.SDK.DTO)
  ├── AutoGenPOCO/  - generated POCO interfaces + classes (Mycelium.SDK.POCO)
  ├── AutoGenEnum/  - generated enumerations (Mycelium.SDK)
  └── Poco/         - hand-coded POCO partials (Compute* companions)

Mycelium.SDK.CodeGenerator (net10.0, not packaged)
  ├── Generators/UmlHandleBarsGenerators/ - DTO / POCO / Enum generators
  ├── HandleBarHelpers/                   - Handlebars helper registrations
  ├── Extensions/                         - UML class / property / name queries
  └── Templates/Uml/                      - *.hbs templates (copied to output)

Mycelium.SDK.Tests (net10.0)                - runtime contract of the generated SDK
Mycelium.SDK.CodeGenerator.Tests (net10.0)  - model validation, generation, golden files
```

Further `*.Tests` projects are expected; this list is not exhaustive. A new one follows the baseline in `TESTING.md` § "Adding a new test project".

### Target Frameworks

- Core library (`Mycelium.SDK`): `netstandard2.1;net10.0`, `LangVersion 14.0`
- `Mycelium.SDK.CodeGenerator` and **every** `*.Tests` project: `net10.0`

## Key Conventions

- **Paths are ALWAYS repo-relative — NEVER absolute.** This applies to every path the agent writes anywhere: code comments, XML doc `<see cref="…"/>` and prose, error and log messages, commit messages, PR bodies, GitHub issue bodies and plan files (e.g. say `Mycelium.SDK/Poco/ProjectMember.cs`, NOT `C:\CODE\Mycelium\mycelium-sdk\Mycelium.SDK\Poco\ProjectMember.cs` and NOT `/c/CODE/Mycelium/...`). Use forward slashes. Reason: absolute paths are user- and machine-specific — they leak the local filesystem into the repo and into communication with other contributors, break for everyone else, and go stale on rename or move. The ONLY exception is the `Read` / `Edit` / `Write` tool `file_path` parameter, which the tool implementation requires to be absolute; those tool arguments are not user-visible artifacts. Everything you author as content must be repo-relative.
- **Code style is defined in `.github/CONTRIBUTING.md`** — read it before writing C#. In short: 4 spaces (no tabs), no `_` prefix on members, explicit `this.` for instance members, `var` unless the inferred type is not obvious, C# type aliases (`int`, `string`), long descriptive names, braces on every block even single-line, `using` statements **inside** the namespace, no regions.
- Every source file carries the Starion Apache-2.0 file header exactly as `Mycelium.SDK.sln.DotSettings` defines it (`FileHeaderText`); ReSharper users get it automatically from that file.
- Commit messages use prefix tags: `[Add]`, `[Update]`, `[Remove]`, `[Fix]`.
- Branches: `main` is the release branch, `development` is the integration branch. **All feature work branches from and targets `development`**; `main` is downstream only.
- CI: GitHub Actions — `.github/workflows/CodeQuality.yml` (build, test, SonarCloud coverage), `codeql-analysis.yml`, `nuget-reference-check.yml`, plus Dependabot.
- License: Apache-2.0. An SBOM is generated during pack (`GenerateSBOM`).
- **`Resources/*.xmi` is read-only** — an external Enterprise Architect export, never edited in this repository. A needed model element is raised as a UML proposal to the architect, not applied locally; see "The UML model is READ-ONLY" above. Files under `AutoGen*` are likewise never hand-written.
- **Generated code destined for another solution is never this agent's to compile.** When a generator here emits source for a different solution — the Carter modules for `mycelium-fabric`, for example — verifying that the emitted C# compiles is the consuming solution's responsibility. Do not create scratch projects, probe builds or compile harnesses to type-check generated output. What this repository guarantees is that generation is deterministic and matches its reviewed goldens; compilation is proven downstream.
- **`Resources/ptc-25-02-29.xmi` is read-only** — the OMG Systems Modeling API and Services PIM export, on exactly the same footing as the other resources. Never edit it, not even to correct the `getQueryById` double return or to supply the missing `KerML Abstract Syntax.xml` references; both are handled in the template and documented above. A genuine defect is raised with OMG.
- **`Resources/ptc-25-02-30.json` is read-only** — the OMG Systems Modeling API and Services OpenAPI document, on exactly the same footing as the XMI exports. Never edit it, not even to work around a specification quirk. A generator that cannot handle the document must fail loudly; a genuine defect in the specification is raised with OMG, not patched locally. (One such quirk is live today: `datatypeId` is typed `format: uri` yet sits in a single path segment, which cannot match a URI containing a slash. The route is generated unconstrained and the question left open deliberately.)

## Quality rules

- Guard every public generator entry point with `ArgumentNullException.ThrowIfNull` / `ArgumentException.ThrowIfNullOrEmpty` and document the result with an `<exception>` XML doc tag — the pattern used throughout `Mycelium.SDK.CodeGenerator`.
- Throw `InvalidOperationException` with a message that names the offending element (`property.Describe()`, `umlClass.XmiId`) when the model is unusable. A generator must fail loudly on a bad model, never emit a plausible-but-wrong file.
- Prefer LINQ for projection, filtering and aggregation (`items.Where(...).Select(...).ToArray()`) over hand-rolled `foreach` + `if` + `.Add()` loops. The one exception is straightforward positional or range access on a concrete `List`/array, where the indexer/range syntax is both clearer and faster (`list[^1]` beats `list.Last()`).
- **Flatten a `foreach` with a leading-`if` filter by pushing the predicate into `.Where(...)`** on the iterated source, so the loop body is the unguarded action: `foreach (var end in association.MemberEnd.Where(end => !string.IsNullOrWhiteSpace(end.Name)))` rather than a `continue` guard inside the body. Same for `.OfType<T>()` instead of a runtime `is`-check plus cast. Exceptions: the predicate has observable side effects, or it is too long to read inline (extract it to a named local function and still call it from the `.Where(...)`).
- Prefer C# collection expressions (`[a, b, c]`, `[..xs]`, `[]`) over `new[] { … }`, `new List<T> { … }`, `new T[] { … }` — in production code *and* in tests. Fall back to explicit construction only when type inference cannot pick the right collection type.
- **Always use C# auto-properties** (`public T Foo { get; set; }`, `{ get; init; }`, `{ get; }`) — never pair a private backing field with a full-getter property when there is no non-trivial logic. Mere storage is not a justification for a backing field.
- Prefer switch expressions over if-else chains when mapping one value to another (the `property.Type switch { IClass => …, IEnumeration => …, … }` shape used across `Extensions/PropertyExtension.cs`).
- Prefer comparing `Count` to 0 rather than using `Any()`, both for clarity and for performance.
- Prefer `string.IsNullOrWhiteSpace` over `string.IsNullOrEmpty` when checking a non-nullable string.
- Prefer C# property patterns (`x is IType { Prop: value }`) over the declared-variable-plus-predicate form when the narrowed variable is consulted only once.
- Prefer method-group syntax over a lambda that merely forwards the call: `.Select(DescribeAssociationEnd)` rather than `.Select(end => DescribeAssociationEnd(end))`. Fall back to a lambda when the body does more than the bare call, when overload resolution is ambiguous, or when explicit type arguments are needed.
- Use meaningful variable names in every context — `umlClass`, `generatedFile`, `associationEnd`, never `c`, `f`, `e`.
- Use `NotSupportedException` (not `NotImplementedException`) for placeholder or stub members that require a manual implementation.
- Surround every braced block (`if`, `else if`, `while`, `for`, `foreach`, `switch`, `using`, `try`/`catch`/`finally`, `lock`, `do…while`) with a blank line on both sides. The rule does not apply at the very start or end of a method body, nor between a `}` and a continuation keyword (`else`, `catch`, `finally`) belonging to the same control flow.
- Use ordinal string comparison (`StringComparer.Ordinal`, `StringComparison.Ordinal`) for anything that feeds generated output or a file-name comparison. Culture-sensitive comparison would make generation machine-dependent.

## Branch & PR workflow (MANDATORY)

Direct pushes to `development` or `main` are forbidden. All work lives on a feature branch.

**Agent boundaries are strict and minimal**:

1. The agent **must NOT auto-commit, EVER.** `git commit` is the user's responsibility — no exceptions, no asking, no "for convenience". The user reviews `git diff` and commits manually.
2. The agent **must NOT push commits, open PRs, or merge by default.** Push, PR and merge are the user's job too. The agent performs push/PR only if the user explicitly asks for it in-conversation; otherwise it stays out of git remote operations entirely.
3. **When the agent creates a branch**, it must:
   - create it locally with `git switch -c <branch> origin/development`, AND
   - **immediately push the empty branch to `origin`** with `git push -u origin <branch>`, so the remote ref exists at the same commit as `origin/development` and the user's later push of the actual commit becomes a trivial fast-forward.

   This is the only push the agent performs by default. It is safe because the branch tip equals `origin/development`'s tip — no new commits, no force flags, no risk of overwriting.
4. **At the end of any task that creates a branch**, the agent stops with a final summary that includes:
   - the in-scope files modified, the test counts, and any verdicts,
   - a **pre-filled commit message** using the `[Add]` / `[Update]` / `[Remove]` / `[Fix]` prefix tag — single line, no body, no `Co-Authored-By` trailer, no "🤖 Generated with …" footer,
   - a handoff line telling the user how to stage, commit and push themselves. Example:
     > Review `git diff`, stage the in-scope files (`git add <path> …` — NEVER `-A` / `.`), commit with the message above, then `git push` (the remote branch already exists, so this is a fast-forward — no `-u` needed). Open the PR yourself via the GitHub UI or `gh pr create --base development`.
   - This is the end of the agent's involvement. **The agent does NOT proceed to push the commit, does NOT open the PR**, unless the user explicitly asks. Typical case: the user handles both.

**If the user does explicitly ask the agent to push or open the PR** (rare; user-initiated only):

- The agent verifies: the current branch is not `development`/`main`, and `git status --porcelain` is empty.
- Then `git push origin <branch>` — NEVER `--force`, NEVER `--force-with-lease`, NEVER `--no-verify`.
- Then `gh pr create --base development --head <branch> --title "…" --body-file <pr-body-tmp>` — NEVER `--base main`, NEVER `--draft` unless the user asked.

**Failure modes**:

- `git push -u origin <branch>` (step 3) fails because the branch already exists on origin → abort, surface to the user, do not force.
- Branch creation requested but the current branch is `development` or `main` AND the user asked for in-place work → REFUSE. Feature work must live on a feature branch first.
- If the user asks the agent to push a commit that the agent itself made (somehow), refuse and surface the policy violation. The agent's commits are forbidden by construction; if one exists, it is a bug that needs human review.

**Why this split**: the user is the reviewer of record. The commit is the review and the push is the delivery — both are the user's calls. The agent's git involvement is bounded to (a) creating the branch locally and pushing the empty ref, so the user's later push is frictionless, and (b) leaving the rest alone.
