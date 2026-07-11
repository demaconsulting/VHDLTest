# Introduction

This document defines the design for each software item in VHDLTest — full architectural and detailed design
for local items (systems, subsystems, and units), and integration/usage design for OTS software items. VHDLTest
is a .NET command-line tool that accepts command-line arguments, loads a YAML configuration file, invokes a VHDL
simulator, processes the simulation output, and reports test pass/fail results.

## Purpose

The purpose of this document is to describe the internal design of each software item that comprises VHDLTest.
It captures data models, algorithms, key methods, and inter-unit interactions at a level of detail sufficient
for formal code review, compliance verification, and future maintenance. A reviewer should be able to understand
how each item satisfies its requirements without reading source code. The document does not restate requirements;
it explains how they are realized.

## Scope

This document covers the detailed design of the following software items:

Local items:

- **VHDLTest**: system, subsystem, and unit design.

OTS items:

- **YamlDotNet**: integration and usage design.
- **DemaConsulting.TestResults**: integration and usage design.
- **xUnit**: integration and usage design.
- **DemaConsulting.ReqStream**: integration and usage design.
- **DemaConsulting.BuildMark**: integration and usage design.
- **DemaConsulting.VersionMark**: integration and usage design.
- **DemaConsulting.ReviewMark**: integration and usage design.
- **DemaConsulting.SysML2Tools**: integration and usage design.
- **DemaConsulting.PandocTool**: integration and usage design.
- **DemaConsulting.WeasyprintTool**: integration and usage design.
- **DemaConsulting.FileAssert**: integration and usage design.
- **DemaConsulting.SarifMark**: integration and usage design.
- **DemaConsulting.SonarMark**: integration and usage design.
- **dotnet-sonarscanner**: integration and usage design.

The following topics are out of scope:

- External library internals
- Build pipeline configuration
- Deployment and packaging
- Test projects

## Software Structure

The software structure is modeled in SysML2 under `docs/sysml2/` and rendered to the
diagram below by SysML2Tools as part of the build pipeline. AI agents should query the
SysML2 model directly (see the `sysml2tools-query` skill) rather than parsing this
diagram or hand-maintained prose.

![Software Structure](SoftwareStructureView.svg)

Within the Simulators subsystem, the simulator units are exposed in auto-discovery priority order
(the order in which `SimulatorFactory` tests for an available simulator when no `--simulator`
option is supplied): `Simulator`, `SimulatorFactory`, `GhdlSimulator`, `ModelSimSimulator`,
`QuestaSimSimulator`, `VivadoSimulator`, `ActiveHdlSimulator`, `NvcSimulator`, `MockSimulator`.
`MockSimulator` is excluded from auto-discovery and is only accessible via the explicit name
`mock`.

## Folder Layout

The source code folder structure mirrors the top-level subsystem breakdown above, giving reviewers an explicit
navigation aid from design to code:

```text
src/DEMAConsulting.VHDLTest/
├── Program.cs                  — entry point and execution orchestrator
├── Cli/
│   ├── Context.cs              — command-line argument parser and I/O owner
│   ├── ConfigDocument.cs       — YAML configuration document parser
│   └── Options.cs              — parsed options holder
├── Simulators/
│   ├── Simulator.cs            — base class for all simulators
│   ├── SimulatorFactory.cs     — creates simulator instances
│   ├── GhdlSimulator.cs        — GHDL simulator integration
│   ├── ModelSimSimulator.cs    — ModelSim simulator integration
│   ├── QuestaSimSimulator.cs   — QuestaSim simulator integration
│   ├── VivadoSimulator.cs      — Vivado simulator integration
│   ├── ActiveHdlSimulator.cs   — Active-HDL simulator integration
│   ├── NvcSimulator.cs         — NVC simulator integration
│   └── MockSimulator.cs        — Mock simulator for self-validation
├── Run/
│   ├── IProcessInvoker.cs      — process invoker interface
│   ├── ProcessInvoker.cs       — default process invoker implementation
│   ├── RunProcessor.cs         — processes simulation run output
│   ├── RunProgram.cs           — executes simulation programs
│   ├── RunResults.cs           — simulation run results
│   ├── RunLine.cs              — individual output line
│   ├── RunLineRule.cs          — output line matching rule
│   └── RunLineType.cs          — output line type classification
├── Results/
│   ├── TestResult.cs           — individual test result
│   └── TestResults.cs          — collection of test results
└── SelfTest/
    └── Validation.cs           — self-validation test runner
```

The design documentation mirrors this under `docs/design/vhdltest/`. The test project,
`test/DEMAConsulting.VHDLTest.Tests/`, mirrors the same subsystem/unit layout but is outside the
design documentation scope of this document; see the Companion Artifact Structure section below.

## Companion Artifact Structure

Each local software item has corresponding artifacts in parallel directory trees:

- Requirements: `docs/reqstream/vhdltest/vhdltest.yaml`, `docs/reqstream/vhdltest/platform-requirements.yaml`,
  `docs/reqstream/vhdltest[/{subsystem-name}...]/{item}.yaml`
- Design: `docs/design/vhdltest.md`,
  `docs/design/vhdltest[/{subsystem-name}...]/{item}.md`
- Verification: `docs/verification/vhdltest.md`,
  `docs/verification/vhdltest[/{subsystem-name}...]/{item}.md`
- Source: `src/DEMAConsulting.VHDLTest[/{SubsystemName}...]/{Item}.cs`
- Tests: `test/DEMAConsulting.VHDLTest.Tests[/{SubsystemName}...]/{Item}Tests.cs`

OTS items have integration/usage design documentation parallel to system folders:

- Requirements: `docs/reqstream/ots/{ots-name}.yaml`
- Design: `docs/design/ots/{ots-name}.md`
- Verification: `docs/verification/ots/{ots-name}.md`

Review-sets: defined in `.reviewmark.yaml`

## References

None.
