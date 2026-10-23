# mycelium-sdk

The mycelium software development toolkit

## Installation

The packages are available on Nuget at:

project                                                     | Nuget
------------------------------------------------------------ | ------------
[Mycelium.SDK](https://www.nuget.org/packages/Mycelium.SDK) | ![NuGet Version](https://img.shields.io/nuget/v/Mycelium.SDK)

## MessagePack compatibility

MessagePack DTO fields and concrete-type payload groups use positional arrays. Their positions are generated from the model. Deterministic ordering does not guarantee compatibility across model revisions.

A change to any DTO field position or count, or payload group position or count, is a breaking wire-format change. Producers and consumers must use matching reader and writer versions for the same model layout; incompatible layouts are unsupported. Exact field-count checks may not detect a changed layout with the same count. Payloads have no in-band schema identifier or automatic version negotiation.

A positional wire-layout change requires a breaking package-version increase. After the first stable MessagePack package release, it requires a major-version increase. Release notes must identify the incompatibility so producers and consumers can coordinate upgrades to matching versions.

## PostgreSQL schema generation

The SDK generates the initial PostgreSQL 18 schema for [Fabric issue #33](https://github.com/mycelium-cmbse/mycelium-fabric/issues/33). The authoritative inputs are `model/mycelium-fabric.xmi` from `Mycelium.Model.Fabric 0.2.0` (UML package `Fabric`) and `model/mycelium-commonprimitives.xmi` from `Mycelium.Model.CommonPrimitives 0.4.0`. Generation reuses the strict canonical loader and packaged model resources.

The generator, SQL helper and template live in `Mycelium.SDK.CodeGenerator`:

- `Generators/UmlHandleBarsGenerators/UmlSqlSchemaGenerator.cs`
- `HandleBarHelpers/SqlSchemaHelper.cs`
- `Templates/Uml/sql-schema-uml-template.hbs`

The complete consumer artifact is [Mycelium.SDK.CodeGenerator/AutoGenSql/schema.sql](Mycelium.SDK.CodeGenerator/AutoGenSql/schema.sql). SQL remains generator-owned: change the model or generator source and regenerate rather than editing the script by hand.

### Generate and review

From the repository root, run:

```powershell
dotnet test Mycelium.SDK.CodeGenerator.Tests/Mycelium.SDK.CodeGenerator.Tests.csproj --configuration Debug --filter FullyQualifiedName~UmlSqlSchemaGeneratorTestFixture
```

The fixture builds the producer, generates through its production rendering path and reports retained staging directories. It does not execute SQL or automatically update committed output or goldens. With the current Debug/net10.0 configuration, the directories are:

| Output | Staging directory |
|---|---|
| Complete `schema.sql` | `Mycelium.SDK.CodeGenerator.Tests/bin/Debug/net10.0/UML/_Mycelium.SDK.CodeGenerator.AutoGenSql` |
| Representative class fragments | `Mycelium.SDK.CodeGenerator.Tests/bin/Debug/net10.0/UML/_Mycelium.SDK.CodeGenerator.AutoGenSql.Representative` |

The complete schema is compared with committed production output. Separate class fragments, selected through the existing `RepresentativeClasses`, are compared with `Mycelium.SDK.CodeGenerator.Tests/Expected/UML/AutoGenSql`. These goldens are bounded review contracts, not standalone database scripts.

SQL uses UTF-8 without BOM, LF line endings and a final newline. Comparisons do not normalize whitespace or line endings. The SQL-only `.gitattributes` rules preserve LF on checkout.

When a model or generator change causes a comparison failure:

1. Review the complete staged schema against the committed schema and the approved SQL mappings.
2. Deliberately copy the reviewed staged `schema.sql` to `Mycelium.SDK.CodeGenerator/AutoGenSql/schema.sql`, preserving its bytes.
3. Separately review affected representative fragments and copy approved files to `Mycelium.SDK.CodeGenerator.Tests/Expected/UML/AutoGenSql`, preserving their bytes. Do not refresh goldens merely to make tests pass.
4. Rerun the generation command above, including its build so updated baselines are copied into test output, then run the complete generator suite:

```powershell
dotnet test Mycelium.SDK.CodeGenerator.Tests/Mycelium.SDK.CodeGenerator.Tests.csproj --configuration Debug --no-build
```

### Provide the schema to Fabric

Fabric explicitly copies and reviews the complete SDK `schema.sql`, records the producing SDK revision and model-package versions, and embeds its reviewed copy in the consumer assembly. If producer changes are uncommitted, identify them in the handoff; a base commit alone does not reproduce the result. Include the generation command and verification results. The SQL header also records the model-package versions.

The Fabric destination path and owning assembly are decided in the Fabric stage. Fabric does not need the SDK checkout at runtime, a sibling-checkout project reference or package publication. The two repository copies are not automatically synchronized: a new SDK artifact requires a deliberate Fabric review and update. Change SQL content in the producer and regenerate; do not hand-edit the consumer copy.

The database and connection must already exist. The script creates the `"Fabric"` schema and its tables, keys, indexes, functions and triggers. It is an initial-creation script, is not safe to rerun against existing application tables, and does not upgrade existing databases. It includes no database creation, credentials, domain seed records or transaction-control commands. Fabric supplies the transaction and determines whether initialization is appropriate, including handling existing or partial state and concurrent startup.

Generator tests verify rendering and exact output comparisons. Actual PostgreSQL execution, rollback and restart preservation require Fabric integration verification. Schema initialization does not connect Fabric's existing SysML2.NET runtime interfaces to this storage; DAO generation and CRUD belong to #42.

## Build Status

GitHub actions are used to build and test the library

Branch | Build Status
------- | :------------
Main | ![Build Status](https://github.com/mycelium-cmbse/mycelium-sdk/actions/workflows/CodeQuality.yml/badge.svg?branch=main)
Development | ![Build Status](https://github.com/mycelium-cmbse/mycelium-sdk/actions/workflows/CodeQuality.yml/badge.svg?branch=development)

# Code Quality

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=mycelium-cmbse_mycelium-sdk&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=mycelium-cmbse_mycelium-sdk)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=mycelium-cmbse_mycelium-sdk&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=mycelium-cmbse_mycelium-sdk)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=mycelium-cmbse_mycelium-sdk&metric=coverage)](https://sonarcloud.io/summary/new_code?id=mycelium-cmbse_mycelium-sdk)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=mycelium-cmbse_mycelium-sdk&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=mycelium-cmbse_mycelium-sdk)
[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=mycelium-cmbse_mycelium-sdk&metric=ncloc)](https://sonarcloud.io/summary/new_code?id=mycelium-cmbse_mycelium-sdk)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=mycelium-cmbse_mycelium-sdk&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=mycelium-cmbse_mycelium-sdk)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=mycelium-cmbse_mycelium-sdk&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=mycelium-cmbse_mycelium-sdk)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=mycelium-cmbse_mycelium-sdk&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=mycelium-cmbse_mycelium-sdk)
[![Technical Debt](https://sonarcloud.io/api/project_badges/measure?project=mycelium-cmbse_mycelium-sdk&metric=sqale_index)](https://sonarcloud.io/summary/new_code?id=mycelium-cmbse_mycelium-sdk)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=mycelium-cmbse_mycelium-sdk&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=mycelium-cmbse_mycelium-sdk)

# Software Bill of Materials (SBOM)

As part of our commitment to security, transparency and traceability, the nuget packages contain a Software Bill of Materials (SBOM). It is generated automatically during the build, providing detailed insight into the components, their licenses and versions. What is included:

  - A comprehensive list of all open-source and third-party components included in the nuget packages.
  - Tracking of software dependencies, licenses and versions.
  - Support for vulnerability management by allowing users to quickly identify potential risks tied to specific components.

# License

The mycelium-sdk libraries are provided to the community under the Apache License 2.0.

# Contributions

Contributions to the code-base are welcome. Please read [CONTRIBUTING](.github/CONTRIBUTING.md) before submitting a pull request.
