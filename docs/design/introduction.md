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

The following topics are out of scope:

- External library internals
- Build pipeline configuration
- Deployment and packaging
- Test projects

## Software Structure

The following tree shows how the VHDLTest software items are organized across the system, subsystem, and unit
levels:

```text
VHDLTest (System)
├── Program (Unit)
├── Cli (Subsystem)
│   ├── Context (Unit)
│   ├── ConfigDocument (Unit)
│   └── Options (Unit)
├── Simulators (Subsystem)
│   ├── Simulator (Unit)
│   ├── SimulatorFactory (Unit)
│   ├── GhdlSimulator (Unit)
│   ├── NvcSimulator (Unit)
│   ├── ModelSimSimulator (Unit)
│   ├── QuestaSimSimulator (Unit)
│   ├── VivadoSimulator (Unit)
│   ├── ActiveHdlSimulator (Unit)
│   └── MockSimulator (Unit)
├── Run (Subsystem)
│   ├── RunProcessor (Unit)
│   ├── RunProgram (Unit)
│   ├── RunResults (Unit)
│   ├── RunLine (Unit)
│   ├── RunLineRule (Unit)
│   └── RunLineType (Unit)
├── Results (Subsystem)
│   ├── TestResult (Unit)
│   └── TestResults (Unit)
└── SelfTest (Subsystem)
    └── Validation (Unit)

OTS Dependencies:
├── YamlDotNet (OTS)
└── DemaConsulting.TestResults (OTS)
```

Each unit is described in detail in its own chapter within this document.

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
│   ├── NvcSimulator.cs         — NVC simulator integration
│   ├── ModelSimSimulator.cs    — ModelSim simulator integration
│   ├── QuestaSimSimulator.cs   — QuestaSim simulator integration
│   ├── VivadoSimulator.cs      — Vivado simulator integration
│   ├── ActiveHdlSimulator.cs   — Active-HDL simulator integration
│   └── MockSimulator.cs        — Mock simulator for self-validation
├── Run/
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

test/DEMAConsulting.VHDLTest.Tests/
├── VHDLTestTests.cs            — system-level integration tests
├── Cli/
│   ├── ContextTests.cs         — Context unit tests
│   ├── ConfigDocumentTests.cs  — ConfigDocument unit tests
│   └── OptionsTests.cs         — Options unit tests
├── Simulators/
│   ├── SimulatorTests.cs           — Simulator unit tests
│   ├── SimulatorFactoryTests.cs    — SimulatorFactory unit tests
│   ├── GhdlSimulatorTests.cs       — GhdlSimulator unit tests
│   ├── NvcSimulatorTests.cs        — NvcSimulator unit tests
│   ├── ModelSimSimulatorTests.cs   — ModelSimSimulator unit tests
│   ├── QuestaSimSimulatorTests.cs  — QuestaSimSimulator unit tests
│   ├── VivadoSimulatorTests.cs     — VivadoSimulator unit tests
│   ├── ActiveHdlSimulatorTests.cs  — ActiveHdlSimulator unit tests
│   ├── MockSimulatorTests.cs       — MockSimulator unit tests
│   └── SimulatorsSubsystemTests.cs — Simulators subsystem integration tests
├── Run/
│   ├── RunProcessorTests.cs    — RunProcessor unit tests
│   ├── RunProgramTests.cs      — RunProgram unit tests
│   ├── RunLineRuleTests.cs     — RunLineRule unit tests
│   ├── RunResultsTests.cs      — RunResults unit tests
│   └── RunSubsystemTests.cs    — Run subsystem integration tests
├── Results/
│   ├── TestResultTests.cs      — TestResult unit tests
│   └── TestResultsTests.cs     — TestResults unit tests
└── SelfTest/
    └── ValidationTests.cs      — Validation unit tests
```

The design documentation mirrors this under `docs/design/vhdltest/` and the test project mirrors the same layout
under `test/DEMAConsulting.VHDLTest.Tests/`.

## Companion Artifact Structure

Each local software item has corresponding artifacts in parallel directory trees:

- Requirements: `docs/reqstream/vhdltest/vhdltest.yaml`,
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

- [VHDLTest releases](https://github.com/demaconsulting/VHDLTest/releases)
