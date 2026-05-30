### Validation

#### Purpose

`Validation.cs` implements the self-validation test runner for VHDLTest. It extracts embedded VHDL
reference files to a temporary folder, runs VHDLTest against them in-process, checks the captured
log for expected pass/fail markers, and reports the outcomes. It is used to verify that the tool is
correctly installed and that the configured simulator is functioning.

#### Data Model

N/A - `Validation` is a static class with no instance state. The constant `ValidationFolder`
(`"validation.tmp"`) holds the temporary working directory name used during test execution.
Note that `RunValidation` is not safe to call concurrently from the same working directory
because both callers would attempt to create and delete the same fixed-name temporary folder.

#### Key Methods

**Run** (public static): Executes the full self-validation sequence and reports results.

- *Parameters*: `Context context` — the program context providing flags, I/O, and simulator name.
- *Returns*: void.
- *Preconditions*: `context` is not null.
- *Postconditions*: validation results have been written to the context; `context.Errors` is greater
  than 0 if any validation failed; optionally a results file has been written.

Writes a system information table (VHDLTest version, machine name, OS Version, .NET runtime,
UTC timestamp), constructs a `TestResults` instance, calls `ValidateTestPasses` and
`ValidateTestFails`, optionally calls `results.SaveResults`, and writes a final pass/fail summary.

**ValidateTestPasses** (public static): Checks that VHDLTest correctly reports passing tests as
passed.

- *Parameters*: `Context context`, `TestResults results`.
- *Returns*: void.
- *Preconditions*: `context` and `results` are not null.
- *Postconditions*: a `TestResult` for `VHDLTest_TestPasses` has been added to `results`.

Calls `RunValidation`, then checks that the exit code is 0, the output contains
`"Passed full_adder_pass_tb"`, and the output contains `"Passed half_adder_pass_tb"`.
Because `RunValidation` always passes `--exit-0` to the inner VHDLTest invocation (see
`RunValidation`), an exit code of 0 confirms there was no tool-level error; pass/fail
determination is made solely from the log output.
Each call to `ValidateTestPasses` and `ValidateTestFails` invokes `RunValidation` independently,
performing a separate subprocess run so that each validation scenario is fully isolated from the
other.

**ValidateTestFails** (public static): Checks that VHDLTest correctly reports failing tests as
failed.

- *Parameters*: `Context context`, `TestResults results`.
- *Returns*: void.
- *Preconditions*: `context` and `results` are not null.
- *Postconditions*: a `TestResult` for `VHDLTest_TestFails` has been added to `results`.

Calls `RunValidation`, then checks that the exit code is 0, the output contains
`"Failed full_adder_fail_tb"`, and the output contains `"Failed half_adder_fail_tb"`. Pass/fail
determination is made solely from the log output; see `RunValidation` for the `--exit-0` flag
rationale.

**RunValidation** (public static): Runs VHDLTest on embedded validation files and returns the
captured log.

- *Parameters*: `out string results` — the captured log file content on return.
- *Parameters*: `string? simulator` — optional simulator name override; null uses the default.
- *Returns*: `int` — exit code from VHDLTest.
- *Preconditions*: the filesystem is writable.
- *Postconditions*: `ValidationFolder` has been deleted; `results` contains the captured log, or an
  empty string if the log file was not written.

Creates `validation.tmp/`, calls `ExtractValidationFiles` to populate it, constructs VHDLTest
arguments (`--log output.log --silent --config validate.yaml --exit-0`, optionally
`--simulator <name>`), calls `RunVhdlTest` in the folder, reads the log, then deletes the folder in
a `finally` block. The `--exit-0` flag is always passed so that a non-zero exit code signals a
tool-level error (e.g., simulator not found) rather than a test-level failure; callers inspect the
log output to determine pass/fail status.

**ExtractValidationFiles** (private static): Extracts embedded validation resource files to a
directory.

- *Parameters*: `string path` — destination directory.
- *Returns*: void.
- *Preconditions*: `path` exists and is writable.
- *Postconditions*: all embedded resources with the prefix
  `DEMAConsulting.VHDLTest.ValidationFiles.` have been written to `path`.

Reflects over the assembly manifest resource names, filters by the
`DEMAConsulting.VHDLTest.ValidationFiles.` prefix, and copies each resource stream to a file named
by the suffix after the prefix in `path`. Throws `InvalidOperationException` if a resource stream
cannot be opened.

**RunVhdlTest** (internal static, two overloads): Runs VHDLTest in-process.

- *Overload 1*: `string[] args` — creates a `Context`, calls `Program.Run`, returns
  `context.ExitCode`.
- *Overload 2*: `string workingFolder`, `string[] args` — changes the working directory to
  `workingFolder`, delegates to overload 1, restores the working directory in a `finally` block.
  Uses `Directory.SetCurrentDirectory`, which is a process-global operation; concurrent calls
  from multiple threads are unsafe.

**ReportTestResult** (private static): Records and reports a single validation test outcome.

- *Parameters*: `Context context`, `TestResults results`, `string testName`, `DateTime start`,
  `double duration`, `int exitCode`, `string output`, `bool succeeded`.
- *Returns*: void.
- *Preconditions*: all reference-type parameters (`context`, `results`, `testName`, `output`) are not null.
- *Postconditions*: a check or cross symbol line has been written to the context; a `TestResult` has
  been appended to `results.Tests`.

Writes `"✓ VHDLTest_{testName} - Passed"` or `"✗ VHDLTest_{testName} - Failed"` (with indented
exit code and output lines on failure) to the context. Constructs a `RunResults` from a single
outcome `RunLine` and wraps it in a `TestResult` with class name `"VHDLTest.Validation"`.

#### Error Handling

`Run` validates that `context` is not null via `ArgumentNullException.ThrowIfNull`. `RunValidation`
uses a `finally` block for best-effort cleanup of the temporary folder and swallows
`Directory.Delete` exceptions to avoid masking earlier failures. `ExtractValidationFiles` throws
`InvalidOperationException` if an embedded resource stream cannot be opened.

#### Dependencies

- **Context** — provides I/O channels, simulator name, depth setting, results file path, and error
  counter.
- **Program** — invoked in-process by `RunVhdlTest` to execute the validation tests.
- **TestResults** — collects and serializes the validation test outcomes.
- **TestResult** — represents each individual validation outcome.
- **RunResults** — wraps the outcome of each validation scenario.
- **RunLine** — represents the single outcome line passed into `RunResults`.

#### Callers

- **Program** — calls `Validation.Run(context)` when `context.Validate` is true.
