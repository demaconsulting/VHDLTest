### TestResults

#### Purpose

`TestResults.cs` is the aggregate container for a VHDLTest run. It drives the build and test
execution sequence through the simulator, collects `TestResult` instances, provides a formatted
summary, and serializes the collection to TRX or JUnit XML report files.

#### Data Model

**RunId**: `Guid` — Unique identifier for this test run; initialized to `Guid.NewGuid()` at
construction.

**RunName**: `string` — Human-readable run label, typically `"{user}@{machine} {timestamp}"` when
called from `Program`, or `"Validation"` when called from `Validation`.

**CodeBase**: `string` — Path or label identifying the code under test; set to the working directory
of the options by default.

**BuildResults**: `RunResults?` — The output and outcome of the VHDL compilation step; null until
`Execute` completes the build phase.

**Tests**: `List<TestResult>` — Ordered list of individual test outcomes, one per configured test
bench.

**Passes**: `IEnumerable<TestResult>` — Derived; filters `Tests` to results where `Passed` is true.

**Fails**: `IEnumerable<TestResult>` — Derived; filters `Tests` to results where `Failed` is true.

#### Key Methods

**Execute** (static, primary overload): Builds the VHDL sources and executes all configured tests.

- *Parameters*: `Context context`, `Options options`, `Simulator simulator`.
- *Returns*: `TestResults` — populated results instance.
- *Preconditions*: `context`, `options`, and `simulator` are not null; the working directory exists
  and contains valid VHDL sources.
- *Postconditions*: the returned `TestResults` has `BuildResults` set and `Tests` populated with one
  `TestResult` per configured test bench.

Delegates to the overload with explicit run name and code base, deriving the run name from
`"{user}@{machine} {timestamp}"` and the code base from `options.WorkingDirectory`.

**Execute** (static, explicit overload): Builds and tests with a caller-supplied run name and code
base.

- *Parameters*: `Context context`, `string runName`, `string codeBase`, `Options options`,
  `Simulator simulator`.
- *Returns*: `TestResults`.
- *Preconditions*: all parameters are not null.
- *Postconditions*: `BuildResults` is set; build failure throws `InvalidOperationException`; on
  success, `Tests` contains one entry per test.

Invokes `simulator.Compile` for the build step; throws `InvalidOperationException` with the message
"Build Failed" if `BuildResults.Summary >= RunLineType.Error`. Then iterates over
`context.CustomTests` (if set) or `options.Config.Tests`, calling `simulator.Test` for each and
appending the resulting `TestResult` to `Tests`.

**SaveResults**: Serializes the test collection to a file in TRX or JUnit XML format.

- *Parameters*: `string fileName` — destination path; extension determines format.
- *Returns*: void.
- *Preconditions*: `fileName` is not null or empty; the destination directory is writable.
- *Postconditions*: a file at `fileName` has been written; format is JUnit XML for the `.xml`
  extension, TRX for all other extensions.
- Throws `ArgumentException` when `fileName` is null, empty, or whitespace-only.

**SaveToTrx**: Backward-compatibility wrapper; delegates to `SaveResults`.

- *Parameters*: `string fileName`.
- *Returns*: void.

**PrintSummary**: Writes a summary table and pass/fail totals to the context.

- *Parameters*: `Context context`.
- *Returns*: void.
- *Preconditions*: `context` is not null.
- *Postconditions*: a formatted summary block has been written to the context output.

Iterates `Tests`, calling `TestResult.PrintSummary` for each. Then writes total, passed, and failed
test counts.

#### Error Handling

`Execute` throws `InvalidOperationException` with the message "Build Failed" when
`BuildResults.Summary >= RunLineType.Error`; `Program.Run` catches this and reports it as an error.
`SaveResults` throws `ArgumentException` when `fileName` is null or empty. All other exceptions from
the simulator propagate to the caller.

#### Dependencies

- **Context** — provides I/O channels for progress and summary output.
- **Options** — supplies the working directory, configuration, and test list.
- **Simulator** — executes the compile and test steps, returning `RunResults`.
- **TestResult** — represents each individual test outcome held in `Tests`.
- **RunResults** — holds raw simulator output; stored as `BuildResults` and wrapped inside each
  `TestResult`.
- **DemaConsulting.TestResults** (OTS) — provides the serializable test results and test result types
  used for TRX and JUnit output.
- **DemaConsulting.TestResults.IO** (OTS) — provides `TrxSerializer` and `JUnitSerializer` for file
  format serialization.

#### Callers

- **Program** — calls `Execute`, `PrintSummary`, and `SaveResults` for the main test execution path.
- **Validation** — constructs a `TestResults` instance directly to collect self-validation results
  and calls `SaveResults` if a results file is requested.
