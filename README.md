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
