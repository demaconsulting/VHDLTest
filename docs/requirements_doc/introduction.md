# Introduction

This document lists all requirements for VHDLTest.

## Purpose

To provide a complete, traceable record of all requirements for VHDLTest, including requirements at the system,
subsystem, and unit levels.

VHDLTest is a VHDL test bench automation tool that executes test benches across multiple HDL simulators and generates
standardized test results. This requirements document defines the functional and non-functional requirements that the
tool must satisfy, covering the command-line interface, configuration file processing, simulator support, test
execution, result output, self-validation, exit code handling, and cross-platform support.

## Scope

This document covers all requirements defined in `docs/reqstream/` for VHDLTest, spanning:

- Command-line interface and user interaction
- YAML-based configuration file processing
- Simulator support (ActiveHDL, GHDL, ModelSim, NVC, QuestaSim, Vivado)
- Test execution and output processing
- TRX format test result generation
- Self-validation capabilities
- Exit code handling
- Cross-platform support (Windows, Linux) and multi-version .NET support

## References

N/A
