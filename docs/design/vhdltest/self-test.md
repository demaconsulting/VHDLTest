## SelfTest

The SelfTest subsystem implements VHDLTest's self-validation capability, allowing users to verify
that the tool is correctly installed and functioning in their environment.

### Overview

The SelfTest subsystem contains a single unit, `Validation`, which executes a fixed set of embedded
VHDL test files using the available simulator, reports the outcomes via the context output channels,
and optionally saves the results to a TRX or JUnit file. It provides two specific validation
scenarios: verifying that passing tests are reported as passed, and verifying that failing tests are
reported as failed.

### Interfaces

**ValidationPublicApi**: The public-facing in-process .NET API of the SelfTest subsystem.

- *Type*: in-process .NET public API.
- *Role*: Provider.
- *Contract*: `Validation.Run(Context context)` is the single entry point. The caller provides a
  fully-initialised `Context`; `Validation.Run` writes all output to the context and increments
  `context.Errors` on failure.
- *Constraints*: Requires a writable filesystem for the temporary validation folder and a configured
  simulator. The caller must not dispose `context` before `Run` returns.

**Consumed Interfaces**:

- **`Cli.Context`** — consumed by `Validation.Run` for all output writes and flag reading; provides the
  I/O channels and `context.Validate`, `context.Simulator`, and `context.ResultsFile` flags.
- **`context.Depth`** — consumed by `Validation.Run` to control the Markdown heading level used in
  validation output.
  - *Type*: `int` property on `Cli.Context`.
  - *Role*: Consumer.
  - *Contract*: `context.Depth` is read once at the start of `Run` to produce the heading prefix
    (`new string('#', context.Depth)`) written before the system information table. The same depth
    value controls all Markdown heading levels throughout the validation output.
  - *Constraints*: Must be a positive integer; the value is supplied by the caller via the
    `--depth` command-line argument (e.g., `--depth 3` produces `###`-level headings).
- **`Program`** — invoked in-process via `Program.Run` to execute each embedded validation test scenario
  as a re-entrant call; `RunVhdlTest` supplies a fresh `Context` per validation scenario.
- **`Results.TestResults`** / **`Results.TestResult`** — used to collect individual validation pass/fail
  outcomes and optionally persist them to a results file via `SaveResults`.

### Design

1. `Program` calls `Validation.Run(context)` when `context.Validate` is true.
2. `Validation.Run` writes a Markdown-formatted system information table to the context (VHDLTest
   version, machine name, OS Version, .NET runtime, UTC timestamp). The Markdown heading level
   throughout the validation output is controlled by `context.Depth`; for example, `--depth 3`
   produces `###`-level headings (requirement `VHDLTest-SelfTest-Depth`).
3. It calls `ValidateTestPasses` and `ValidateTestFails`, each of which:
   - Calls `RunValidation` to extract embedded VHDL files to a temporary folder, execute VHDLTest
     on them, capture the log output, and clean up the folder.
   - Checks the exit code and output for expected pass/fail markers.
   - Calls `ReportTestResult` to write a check or cross symbol to the context and add a `TestResult`
     to the results collection.
4. If `context.ResultsFile` is set, `Validation.Run` calls `results.SaveResults` to persist the
   results.
5. `Validation.Run` writes a summary with total, passed, and failed counts and, if no errors
   occurred, a "Validation Passed" message.
