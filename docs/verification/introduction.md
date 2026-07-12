# Introduction

This document describes how each software item in VHDLTest is verified.

## Purpose

This document describes how each software item in VHDLTest is verified — local items
(systems, subsystems, and units). For each item, it names the test scenarios that verify
its requirements. A reviewer should be able to confirm coverage completeness without reading
test code.

## Scope

Local items:

- **VHDLTest**: system, subsystem, and unit verification.

OTS items:

- **YamlDotNet**: integration and usage verification.
- **DemaConsulting.TestResults**: integration and usage verification.
- **xUnit**: integration and usage verification.
- **DemaConsulting.ReqStream**: integration and usage verification.
- **DemaConsulting.BuildMark**: integration and usage verification.
- **DemaConsulting.VersionMark**: integration and usage verification.
- **DemaConsulting.ReviewMark**: integration and usage verification.
- **DemaConsulting.SysML2Tools**: integration and usage verification.
- **DemaConsulting.PandocTool**: integration and usage verification.
- **DemaConsulting.WeasyprintTool**: integration and usage verification.
- **DemaConsulting.FileAssert**: integration and usage verification.
- **DemaConsulting.SarifMark**: integration and usage verification.
- **DemaConsulting.SonarMark**: integration and usage verification.
- **dotnet-sonarscanner**: integration and usage verification.

The following topics are out of scope:

- Test infrastructure (`test/DEMAConsulting.VHDLTest.Tests/`)
- Build pipeline configuration
- Deployment and packaging

## Companion Artifact Structure

Local items have parallel artifacts in:

- Requirements: `docs/reqstream/vhdltest.yaml`, `docs/reqstream/vhdltest[/{subsystem-name}...]/{item}.yaml`
- Design: `docs/design/vhdltest.md`, `docs/design/vhdltest[/{subsystem-name}...]/{item}.md`
- Verification: `docs/verification/vhdltest.md`, `docs/verification/vhdltest[/{subsystem-name}...]/{item}.md`
- Source: `src/DEMAConsulting.VHDLTest[/{SubsystemName}...]/{Item}.cs`
- Tests: `test/DEMAConsulting.VHDLTest.Tests[/{SubsystemName}...]/{Item}Tests.cs`

Review-sets: defined in `.reviewmark.yaml`

## References

- [VHDLTest releases](https://github.com/demaconsulting/VHDLTest/releases)
