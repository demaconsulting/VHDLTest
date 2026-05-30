# VHDLTest

VHDLTest is a .NET command-line tool that accepts command-line arguments, loads a YAML configuration file,
invokes a VHDL simulator, processes the simulation output, and reports test pass/fail results.

## Architecture

VHDLTest is structured as a single system containing five subsystems and one top-level unit. The `Program`
unit is the sole entry point and orchestrates all subsystems. The one exception is `Validation` (in the
`SelfTest` subsystem), which calls back into `Program.Run` in-process as a re-entrant call to execute each
embedded validation test scenario; this is the only circular dependency in the design.

```mermaid
flowchart TD
    Program

    subgraph Cli
        Context
        ConfigDocument
        Options
    end

    subgraph Simulators
        Simulator
        SimulatorFactory
        GhdlSimulator
        NvcSimulator
        ModelSimSimulator
        QuestaSimSimulator
        VivadoSimulator
        ActiveHdlSimulator
        MockSimulator
    end

    subgraph Run
        RunProcessor
        RunProgram
        RunResults
        RunLine
        RunLineRule
        RunLineType
    end

    subgraph Results
        TestResult
        TestResults
    end

    subgraph SelfTest
        Validation
    end

    Program --> Cli
    Program --> Simulators
    Program --> Results
    Program --> SelfTest
    Results --> Run
    Results --> Simulators
    Simulators --> Run
    SelfTest --> Program
```

## External Interfaces

**Command-Line Interface**: Arguments and options passed to the tool on invocation.

- *Type*: CLI
- *Role*: Consumer (the operator invokes this tool)
- *Contract*: Accepts options `-h/--help`, `-v/--version`, `--validate`, `--silent`, `--verbose`,
  `--depth`, `-l/--log`, `-c/--config`, `-r/--result/--results`, `-s/--simulator`, `-0/--exit-0`,
  and a positional test-filter list. Writes results and diagnostics to stdout/stderr.
- *Constraints*: Unknown options cause an error; missing config file causes an error and prints usage.

**Configuration File**: YAML file specifying simulator settings and test list.

- *Type*: File (YAML)
- *Role*: Consumer (reads file from disk)
- *Contract*: Parsed by `ConfigDocument` into an `Options` instance; fields include simulator name,
  test names, compile files, and simulator-specific arguments.
- *Constraints*: File must be valid YAML conforming to the VHDLTest configuration schema.

**Environment Variables**: Simulator executable path overrides.

- *Type*: Environment variables
- *Role*: Consumer (reads process environment)
- *Contract*: Variables following the `VHDLTEST_<SIMULATOR>_PATH` naming convention override
  default simulator path discovery for the named simulator.
- *Constraints*: Optional. When set, the value must be the full path to the directory containing
  the simulator executable.

**Simulator Process**: External VHDL simulator executable invoked as a child process.

- *Type*: Process (stdin/stdout/stderr pipes)
- *Role*: Consumer (spawns and reads output from the simulator)
- *Contract*: Each simulator subclass (`GhdlSimulator`, `NvcSimulator`, etc.) constructs the
  appropriate command-line arguments and interprets stdout/stderr via `RunProcessor` rules to
  classify each output line as pass, fail, or informational.
- *Constraints*: Simulator must be installed and on the system PATH. Exit codes and output format
  are simulator-specific and handled per-subclass.

**Results File**: Optional TRX or JUnit XML file written on completion.

- *Type*: File (TRX/JUnit XML)
- *Role*: Provider (writes file to disk)
- *Contract*: Written by `TestResults.SaveResults` using the `DemaConsulting.TestResults` library.
  Path specified via `--results` option.
- *Constraints*: Format is determined by file extension (.trx → TRX, .xml → JUnit).

## Dependencies

- **YamlDotNet**: used for YAML deserialization of the configuration file — see *YamlDotNet
  Integration Design*
- **DemaConsulting.TestResults**: used for writing TRX and JUnit test results files — see
  *DemaConsulting.TestResults Integration Design*

## Risk Control Measures

N/A - not a safety-classified software item.

## Data Flow

Data moves through VHDLTest in the following sequence:

1. `Program.Main` receives raw command-line arguments and creates a `Context` (Cli subsystem).
2. `Program.Run` inspects `Context` flags; for a test run it calls `SimulatorFactory.Get` to
   select the active simulator (Simulators subsystem).
3. `Options.Parse` reads and deserializes the YAML configuration file into an `Options` instance
   (Cli subsystem, using YamlDotNet).
4. `TestResults.Execute` iterates over the configured tests; for each test it delegates to the
   active simulator which invokes `RunProgram` to spawn the simulator process and `RunProcessor`
   to classify each output line (Run subsystem).
5. Each classified line is accumulated into `RunResults`, from which pass/fail `TestResult` records
   are created and collected into `TestResults` (Results subsystem).
6. `TestResults.PrintSummary` writes the summary to the `Context` output stream.
7. If a results file path is present, `TestResults.SaveResults` writes the TRX or JUnit file.
8. `Program.Main` sets `Environment.ExitCode` from `context.ExitCode`.

## Design Constraints

- **Platform**: targets .NET 8, .NET 9, and .NET 10 on Linux, Windows, and macOS.
- **Distribution**: packaged and distributed as a .NET global tool (`dotnet tool install`).
- **Simulator coupling**: each simulator integration is self-contained in its own unit; adding a
  new simulator requires only a new subclass registered in `SimulatorFactory`.
- **No unsafe code**: all code must compile without unsafe blocks; pointer arithmetic is prohibited.
- **Nullable reference types**: enabled throughout; all public APIs must be null-annotated.

## Error Handling

| Error Condition | Detection | Response |
| --- | --- | --- |
| Unknown CLI option | `Context.Create` parser encounters unrecognised flag | Writes error message, prints usage, exits non-zero |
| Missing config file path | `Context.ConfigFile` is null | Writes "Error: Missing arguments", prints usage, exits non-zero |
| Missing config file on disk | `ConfigDocument.ReadFile` throws `FileNotFoundException` | Caught in `Program.Run`; writes "Error: ..." message, exits non-zero |
| Invalid configuration YAML | `ConfigDocument.ReadFile` throws `InvalidOperationException` | Caught in `Program.Run`; writes "Error: ..." message, exits non-zero |
| Unknown simulator name | `SimulatorFactory.Get` returns null | `Program.Run` throws `InvalidOperationException("Simulator not found")`, exits non-zero |
| Simulator executable absent | Simulator `Compile` or `Test` throws `InvalidOperationException` | Caught in `Program.Run`; writes "Error: ..." message, exits non-zero |
| Results file write failure | `TestResults.SaveResults` throws any exception | Caught in `Program.Run`; writes "Error: Failed to write results file: ..." message, exits non-zero |
