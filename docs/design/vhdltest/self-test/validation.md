# Validation Unit Design

## Overview

`SelfTest/Validation.cs` implements the self-validation test runner for VHDLTest.
It uses a set of embedded VHDL files to verify the tool is correctly installed
and that the configured simulator is functioning correctly.

## Responsibilities

- Execute a set of built-in VHDL tests using the available simulator
- Report validation results via the `Context` output channels
- Optionally save results to a TRX or JUnit file
- Include system information (OS, .NET runtime) in the validation report

## Interactions

- Called by `Program.cs` when `--validate` is present in the `Context`
- Uses `SimulatorFactory` to obtain the active simulator
- Uses `Results/TestResults.cs` to build and serialize the results report
