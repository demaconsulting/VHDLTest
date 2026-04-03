# Introduction

This document provides the detailed design for VHDLTest, a .NET command-line application for
running VHDL unit tests and generating test reports.

## Purpose

The purpose of this document is to describe the internal design of each software unit that
comprises VHDLTest. It captures data models, algorithms, key methods, and inter-unit
interactions at a level of detail sufficient for formal code review, compliance verification,
and future maintenance. The document does not restate requirements; it explains how they
are realized.

## Scope

This document covers the detailed design of the following software units:

- **Program** — entry point and execution orchestrator (`Program.cs`)
- **Context** — command-line argument parser and I/O owner (`Cli/Context.cs`)
- **ConfigDocument** — YAML configuration document parser (`Cli/ConfigDocument.cs`)
- **Options** — parsed options holder (`Cli/Options.cs`)
- **Simulators** — VHDL simulator integrations (`Simulators/`)
- **Run** — VHDL simulation run processing (`Run/`)
- **Results** — test result data model (`Results/`)
- **Validation** — self-validation test runner (`SelfTest/Validation.cs`)

The following topics are out of scope:

- External library internals
- Build pipeline configuration
- Deployment and packaging

## Software Structure

The following tree shows how the VHDLTest software items are organized across the
system, subsystem, and unit levels:

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
│   └── ActiveHdlSimulator (Unit)
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
```

Each unit is described in detail in its own chapter within this document.

## Folder Layout

The source code folder structure mirrors the top-level subsystem breakdown above, giving
reviewers an explicit navigation aid from design to code:

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
│   └── ActiveHdlSimulator.cs  — Active-HDL simulator integration
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
```

The design documentation mirrors this under `docs/design/vhdltest/` and the test project mirrors
the same layout under `test/DEMAConsulting.VHDLTest.Tests/`.

## Document Conventions

Throughout this document:

- Class names, method names, property names, and file names appear in `monospace` font.
- The word **shall** denotes a design constraint that the implementation must satisfy.
- Section headings within each unit chapter follow a consistent structure: overview, data model,
  methods/algorithms, and interactions with other units.
- Text tables are used in preference to diagrams, which may not render in all PDF viewers.

## References

- [VHDLTest User Guide][user-guide]
- [VHDLTest Repository][repo]

[user-guide]: ../user_guide/introduction.md
[repo]: https://github.com/demaconsulting/VHDLTest
