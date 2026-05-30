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
- *Constructor*: `TestResults(string runName, string codeBase)` — initializes a new instance with the
  supplied run label and code-base path.
- *Properties*:
  - `RunId` (`Guid`): unique identifier for the test run; initialized to `Guid.NewGuid()` at
    construction; used to correlate TRX output back to a specific invocation.
  - `RunName` (`string`): human-readable label set at construction; used in generated reports.
  - `CodeBase` (`string`): path or label identifying the source tree under test; stored in TRX output.
  - `BuildResults` (`RunResults?`): null until `Execute` completes the build phase; set once
    compilation finishes regardless of pass or fail outcome.
  - `Tests` (`List<TestResult>`): ordered list of individual test bench outcomes, one per configured
    test bench; populated in order by `Execute`.
  - `Passes` (`IEnumerable<TestResult>`): lazily-enumerated, LINQ-queryable view over `Tests`
    containing only outcomes where the test bench ran without error (i.e., `TestResult.Passed` is
    true). Filters `Tests` by pass outcome; consumers may call `.Count()` or iterate directly.
  - `Fails` (`IEnumerable<TestResult>`): lazily-enumerated, LINQ-queryable view over `Tests`
    containing only outcomes where the test bench encountered an error (i.e., `TestResult.Failed`
    is true). Filters `Tests` by fail outcome; consumers may call `.Count()` or iterate directly.
- *Explicit Execute overload*: `Execute(Context context, string runName, string codeBase, Options options,
  Simulator simulator) → TestResults` — caller-supplied run name and code base; throws
  `InvalidOperationException` when the build step reports errors.
- *Constraints*: `SaveResults` requires a non-null, non-empty file path; an empty or unrecognised
  extension defaults to TRX format.

**Consumed Interfaces**:

- **`Run.RunResults`** — consumed by `TestResults.Execute` to display line-by-line simulator
  output via `RunResults.Print` (for both the build step and each test run), and to determine
  build failure from `BuildResults.Summary`; provides the severity summary and duration used by
  `TestResult.Passed`, `TestResult.Failed`, and per-test summary formatting in `PrintSummary`.
- **`Run.RunLine`** and **`Run.RunLineType`** — consumed transitively through `RunResults`; directly
  referenced in `TestResults.SaveResults` when filtering error lines for failure messages, and
  required when constructing `RunResults` fixtures for testing.
- **`Cli.Context`** — consumed by `TestResults.Execute` and `TestResults.PrintSummary` for writing
  progress messages and formatted output to the configured output channels. `Execute` also reads
  `context.CustomTests` when set, using it as the test list in place of `options.Config.Tests`.
- **`Cli.Options`** — consumed by `TestResults.Execute` for the working directory, simulator name,
  and test list configuration.
- **`Simulators.Simulator`** — consumed by `TestResults.Execute` to delegate VHDL source
  compilation and individual test bench execution.

### Design

1. `Program` calls `TestResults.Execute` with a `Context`, `Options`, and `Simulator`.
2. `TestResults.Execute` invokes `simulator.Compile` to build the VHDL sources and stores the
   `RunResults` as `BuildResults`. If `BuildResults.Summary >= RunLineType.Error`, `Execute` throws
   `InvalidOperationException` with the message `"Build Failed"` and returns without executing any tests.
3. For each configured test, `TestResults.Execute` calls `simulator.Test`, receives the returned
   `TestResult`, appends it to `Tests`, and calls `testResult.PrintSummary(context)` to write the
   per-test pass/fail line.
4. `Program` calls `TestResults.PrintSummary` to write a pass/fail table to the context.
5. If a results file path was supplied, `Program` calls `TestResults.SaveResults`, which formats the
   collected `TestResult` instances as TRX or JUnit XML and writes the file.
