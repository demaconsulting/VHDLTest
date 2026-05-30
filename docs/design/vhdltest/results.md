## Results

The Results subsystem provides the data model for storing VHDL test run outcomes and serializing
them to standard report formats (TRX and JUnit XML).

### Overview

The Results subsystem encapsulates two tightly related units: `TestResult`, which records the
outcome of a single test bench execution, and `TestResults`, which holds the complete collection and
provides build/test execution, summary printing, and file serialization. The subsystem boundary
includes all data capture and reporting logic; it does not perform any simulation.

### Interfaces

**TestResultsPublicApi**: The public-facing in-process .NET API of the Results subsystem.

- *Type*: in-process .NET public API.
- *Role*: Provider.
- *Contract*: Callers obtain a `TestResults` instance via `TestResults.Execute`, print a summary via
  `PrintSummary`, and optionally persist results via `SaveResults`.
- *Properties*:
  - `Passes` (`IEnumerable<TestResult>`): lazily-enumerated, LINQ-queryable view over `Tests`
    containing only outcomes where the test bench ran without error (i.e., `TestResult.Passed` is
    true). Filters `Tests` by pass outcome; consumers may call `.Count()` or iterate directly.
  - `Fails` (`IEnumerable<TestResult>`): lazily-enumerated, LINQ-queryable view over `Tests`
    containing only outcomes where the test bench encountered an error (i.e., `TestResult.Failed`
    is true). Filters `Tests` by fail outcome; consumers may call `.Count()` or iterate directly.
- *Constraints*: `SaveResults` requires a non-null, non-empty file path; an empty or unrecognised
  extension defaults to TRX format.

**Consumed Interfaces**:

- **`Run.RunResults`** — consumed by `TestResults.PrintSummary` for line-by-line test output;
  provides the raw simulation output and severity summary used to format each summary line.
- **`Run.RunLine`** and **`Run.RunLineType`** — consumed transitively through `RunResults`; directly
  referenced in `TestResults.SaveResults` when filtering error lines for failure messages, and
  required when constructing `RunResults` fixtures for testing.
- **`Cli.Context`** — consumed by `TestResults.Execute` and `TestResults.PrintSummary` for writing
  progress messages and formatted output to the configured output channels.
- **`Cli.Options`** — consumed by `TestResults.Execute` for the working directory, simulator name,
  and test list configuration.
- **`Simulators.Simulator`** — consumed by `TestResults.Execute` to delegate VHDL source
  compilation and individual test bench execution.

### Design

1. `Program` calls `TestResults.Execute` with a `Context`, `Options`, and `Simulator`.
2. `TestResults.Execute` invokes `simulator.Compile` to build the VHDL sources and stores the
   `RunResults` as `BuildResults`.
3. For each configured test, `TestResults.Execute` calls `simulator.Test`, wraps the `RunResults` in
   a `TestResult`, and appends it to `Tests`.
4. `Program` calls `TestResults.PrintSummary` to write a pass/fail table to the context.
5. If a results file path was supplied, `Program` calls `TestResults.SaveResults`, which formats the
   collected `TestResult` instances as TRX or JUnit XML and writes the file.
