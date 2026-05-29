### GhdlSimulator

#### Verification Approach

`GhdlSimulator` is verified through unit tests in
`test/DEMAConsulting.VHDLTest.Tests/Simulators/GhdlSimulatorTests.cs`. The compile and test
`RunProcessor` instances (`CompileProcessor` and `TestProcessor`) are exercised by calling
`Parse` with pre-captured output strings, covering clean output, warnings, info messages,
and errors without requiring GHDL to be installed. GHDL integration with real VHDL source
files is verified by CI jobs in environments with GHDL installed, using the self-validation
path (`--validate --simulator ghdl`).

#### Test Environment

Automated unit tests: standard test environment, no GHDL installation required.
Live simulator integration: CI environment with GHDL installed on PATH.

#### Acceptance Criteria

- All unit tests in `GhdlSimulatorTests.cs` pass with zero failures.
- `GhdlSimulator.Instance.SimulatorName` returns `"GHDL"`.
- The compile processor correctly classifies clean, warning, and error output patterns.
- The test processor correctly classifies clean, info (report note), warning (report warning),
  and error (report error/failure) output patterns.
- `GhdlSimulator.FindPath()` returns the `VHDLTEST_GHDL_PATH` environment variable value when set.

#### Test Scenarios

**SimulatorName_ReturnsGHDL**: Verifies that `GhdlSimulator.Instance.SimulatorName` is
`"GHDL"`, confirming the instance is registered with the correct name.
This scenario is tested by `GhdlSimulator_SimulatorName_ReturnsGHDL`.

**CompileProcessor_CleanOutput_ReturnsTextResult**: Verifies that clean GHDL analysis output
with no diagnostic lines produces a `RunLineType.Text` summary and correctly classified lines.
This scenario is tested by `GhdlSimulator_CompileProcessor_CleanOutput_ReturnsTextResult`.

**CompileProcessor_WarningOutput_ReturnsWarningResult**: Verifies that a GHDL warning line
matching the `.*:\d+:\d+:warning:` pattern is classified as `RunLineType.Warning` and elevates
the summary accordingly.
This scenario is tested by `GhdlSimulator_CompileProcessor_WarningOutput_ReturnsWarningResult`.

**CompileProcessor_ErrorOutput_ReturnsErrorResult**: Verifies that a GHDL error line matching
the `.*:error:` pattern is classified as `RunLineType.Error` and elevates the summary.
This scenario is tested by `GhdlSimulator_CompileProcessor_ErrorOutput_ReturnsErrorResult`.

**TestProcessor_CleanOutput_ReturnsTextResult**: Verifies that clean GHDL simulation output
produces a `RunLineType.Text` summary.
This scenario is tested by `GhdlSimulator_TestProcessor_CleanOutput_ReturnsTextResult`.

**TestProcessor_InfoOutput_ReturnsInfoResult**: Verifies that a GHDL `(report note)` line
is classified as `RunLineType.Info`.
This scenario is tested by `GhdlSimulator_TestProcessor_InfoOutput_ReturnsInfoResult`.

**TestProcessor_WarningOutput_ReturnsWarningResult**: Verifies that a GHDL `(report warning)`
line is classified as `RunLineType.Warning`.
This scenario is tested by `GhdlSimulator_TestProcessor_WarningOutput_ReturnsWarningResult`.

**TestProcessor_ErrorOutput_ReturnsErrorResult**: Verifies that a GHDL `(report error)` line
is classified as `RunLineType.Error`.
This scenario is tested by `GhdlSimulator_TestProcessor_ErrorOutput_ReturnsErrorResult`.

**CompileProcessor_LineColError_ReturnsErrorResult**: Verifies that a GHDL line/column error
line matching the `.*:\d+:\d+:` pattern (e.g. `test.vhd:10:5: error:`) is classified as
`RunLineType.Error`.
This scenario is tested by `GhdlSimulator_CompileProcessor_LineColError_ReturnsErrorResult`.

**CompileProcessor_CannotOpenError_ReturnsErrorResult**: Verifies that a GHDL "cannot open"
line matching the `.*: cannot open` pattern is classified as `RunLineType.Error`.
This scenario is tested by `GhdlSimulator_CompileProcessor_CannotOpenError_ReturnsErrorResult`.

**TestProcessor_AssertionNoteOutput_ReturnsInfoResult**: Verifies that a GHDL `(assertion note)`
line is classified as `RunLineType.Info`, confirming the assertion note pattern is handled.
This scenario is tested by `GhdlSimulator_TestProcessor_AssertionNoteOutput_ReturnsInfoResult`.

**TestProcessor_AssertionWarningOutput_ReturnsWarningResult**: Verifies that a GHDL
`(assertion warning)` line is classified as `RunLineType.Warning`.
This scenario is tested by `GhdlSimulator_TestProcessor_AssertionWarningOutput_ReturnsWarningResult`.

**TestProcessor_AssertionErrorOutput_ReturnsErrorResult**: Verifies that a GHDL
`(assertion error)` line is classified as `RunLineType.Error`.
This scenario is tested by `GhdlSimulator_TestProcessor_AssertionErrorOutput_ReturnsErrorResult`.

**TestProcessor_AssertionFailureOutput_ReturnsErrorResult**: Verifies that a GHDL
`(assertion failure)` line is classified as `RunLineType.Error`.
This scenario is tested by `GhdlSimulator_TestProcessor_AssertionFailureOutput_ReturnsErrorResult`.

**TestProcessor_ReportFailureOutput_ReturnsErrorResult**: Verifies that a GHDL
`(report failure)` line is classified as `RunLineType.Error`.
This scenario is tested by `GhdlSimulator_TestProcessor_ReportFailureOutput_ReturnsErrorResult`.

**TestProcessor_ColonErrorOutput_ReturnsErrorResult**: Verifies that a GHDL `:error:` pattern
line (matching `.*:error:`) is classified as `RunLineType.Error` by the TestProcessor.
This scenario is tested by `GhdlSimulator_TestProcessor_ColonErrorOutput_ReturnsErrorResult`.

**FindPath_WithEnvVar_ReturnsEnvVarValue**: Verifies that `GhdlSimulator.FindPath()` returns the
value of `VHDLTEST_GHDL_PATH` when it is set, confirming the environment variable override
takes priority over PATH search.
This scenario is tested by `GhdlSimulator_FindPath_WithEnvVar_ReturnsEnvVarValue`.

**FindPath_WithoutEnvVar_ReturnsNullOrPath**: Verifies that `GhdlSimulator.FindPath()` does not
throw when `VHDLTEST_GHDL_PATH` is not set, returning either a valid path string (when GHDL is
installed on PATH) or null (when not installed).
This scenario is tested by `GhdlSimulator_FindPath_WithoutEnvVar_ReturnsNullOrPath`.
