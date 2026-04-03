# VHDLTest System Design

## Overview

VHDLTest is a .NET command-line tool for running VHDL unit tests and generating test
reports. It accepts command-line arguments, loads a YAML configuration file, invokes a
VHDL simulator, processes the simulation output, and reports test pass/fail results.

## Responsibilities

- Provide a command-line interface for VHDL test execution
- Support multiple VHDL simulator back-ends
- Parse simulation output to determine test outcomes
- Report test results in standard formats (TRX, JUnit)
- Provide self-validation tests

## Top-Level Unit

| Unit    | File         | Responsibility                                        |
| ------- | ------------ | ----------------------------------------------------- |
| Program | `Program.cs` | Entry point, command dispatch, and process exit code  |

## Subsystems

| Subsystem  | Folder        | Responsibility                                       |
| ---------- | ------------- | ---------------------------------------------------- |
| Cli        | `Cli/`        | Command-line argument parsing and I/O channels       |
| Simulators | `Simulators/` | VHDL simulator integrations                          |
| Run        | `Run/`        | Simulation program execution and output processing   |
| Results    | `Results/`    | Test result data model and report generation         |
| SelfTest   | `SelfTest/`   | Self-validation test runner                          |

## Execution Flow

1. `Program.Main` creates a `Context` from the raw command-line argument array
   (Cli subsystem)
2. `Program.Run` dispatches based on the context flags:
   - **Version flag** — prints the version string and returns
   - **Help flag** — prints version banner and usage information, then returns
   - **Validate flag** — delegates to `Validation.Run` (SelfTest subsystem) and returns
   - **Config file specified** — executes VHDL tests:
     1. `SimulatorFactory.Get` selects the named simulator (Simulators subsystem)
     2. `Options.Parse` reads the YAML configuration file (Cli subsystem)
     3. `TestResults.Execute` drives the simulator through the Run subsystem and
        collects test outcomes (Results subsystem)
     4. `TestResults.PrintSummary` reports the summary to the context
     5. If a results file is specified, `TestResults.SaveResults` writes the TRX/JUnit output
   - **No config file** — writes an error and prints usage, then returns
3. `Program.Main` sets `Environment.ExitCode` from `context.ExitCode`

## Interactions

The `Program` unit is the sole entry point and acts as the orchestrator. It depends on
all subsystems but has no reverse dependencies — no subsystem calls back into `Program`.
