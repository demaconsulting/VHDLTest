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
