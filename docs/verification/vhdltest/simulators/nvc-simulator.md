### NvcSimulator

#### Verification Approach

`NvcSimulator` is verified through unit tests in
`test/DEMAConsulting.VHDLTest.Tests/Simulators/NvcSimulatorTests.cs`. The compile and test
`RunProcessor` instances are exercised by calling `Parse` with pre-captured output strings,
covering clean output, warnings, info messages, and errors without requiring NVC to be
installed. NVC integration with real VHDL source files is verified by CI jobs in environments
with NVC installed, using the self-validation path (`--validate --simulator nvc`).

#### Test Environment

Automated unit tests: standard test environment, no NVC installation required.
Live simulator integration: CI environment with NVC installed on PATH.

#### Acceptance Criteria

- All unit tests in `NvcSimulatorTests.cs` pass with zero failures.
- `NvcSimulator.Instance.SimulatorName` returns `"NVC"`.
- The compile processor correctly classifies clean, info, warning, error, failure, and fatal output patterns.
- The test processor correctly classifies clean, info, warning, error, failure, and fatal output patterns.
- `NvcSimulator.FindPath()` returns the `VHDLTEST_NVC_PATH` environment variable value when set.

#### Test Scenarios

**SimulatorName_ReturnsNVC**: Verifies that `NvcSimulator.Instance.SimulatorName` is `"NVC"`,
confirming the instance is registered with the correct name for factory lookup.
This scenario is tested by the simulator name test in `NvcSimulatorTests.cs`.

**CompileProcessor_CleanOutput_ReturnsTextResult**: Verifies that clean NVC analysis output
produces a `RunLineType.Text` summary with correctly classified lines.
This scenario is tested by the compile clean output test in `NvcSimulatorTests.cs`.

**CompileProcessor_WarningOutput_ReturnsWarningResult**: Verifies that an NVC warning line
is classified as `RunLineType.Warning`.
This scenario is tested by the compile warning test in `NvcSimulatorTests.cs`.

**CompileProcessor_ErrorOutput_ReturnsErrorResult**: Verifies that an NVC error line is
classified as `RunLineType.Error`.
This scenario is tested by the compile error test in `NvcSimulatorTests.cs`.

**TestProcessor_CleanOutput_ReturnsTextResult**: Verifies that clean NVC simulation output
produces a `RunLineType.Text` summary.
This scenario is tested by the test clean output test in `NvcSimulatorTests.cs`.

**TestProcessor_InfoOutput_ReturnsInfoResult**: Verifies that an NVC info line is classified
as `RunLineType.Info`.
This scenario is tested by the test info output test in `NvcSimulatorTests.cs`.

**TestProcessor_WarningOutput_ReturnsWarningResult**: Verifies that an NVC warning line in
simulation output is classified as `RunLineType.Warning`.
This scenario is tested by the test warning output test in `NvcSimulatorTests.cs`.

**TestProcessor_ErrorOutput_ReturnsErrorResult**: Verifies that an NVC error line in
simulation output is classified as `RunLineType.Error`.
This scenario is tested by the test error output test in `NvcSimulatorTests.cs`.

**CompileProcessor_InfoOutput_ReturnsInfoResult**: Verifies that an NVC info (note) line in
compile output is classified as `RunLineType.Info`, confirming the `.* Note:` pattern works.
This scenario is tested by `NvcSimulator_CompileProcessor_InfoOutput_ReturnsInfoResult`.

**CompileProcessor_FailureOutput_ReturnsErrorResult**: Verifies that an NVC failure line
matching `.* Failure:` in compile output is classified as `RunLineType.Error`.
This scenario is tested by `NvcSimulator_CompileProcessor_FailureOutput_ReturnsErrorResult`.

**CompileProcessor_FatalOutput_ReturnsErrorResult**: Verifies that an NVC fatal line
matching `.* Fatal:` in compile output is classified as `RunLineType.Error`.
This scenario is tested by `NvcSimulator_CompileProcessor_FatalOutput_ReturnsErrorResult`.

**TestProcessor_FailureOutput_ReturnsErrorResult**: Verifies that an NVC failure line
matching `.* Failure:` in simulation output is classified as `RunLineType.Error`.
This scenario is tested by `NvcSimulator_TestProcessor_FailureOutput_ReturnsErrorResult`.

**TestProcessor_FatalOutput_ReturnsErrorResult**: Verifies that an NVC fatal line
matching `.* Fatal:` in simulation output is classified as `RunLineType.Error`.
This scenario is tested by `NvcSimulator_TestProcessor_FatalOutput_ReturnsErrorResult`.

**FindPath_WithEnvVar_ReturnsEnvVarValue**: Verifies that `NvcSimulator.FindPath()` returns the
value of `VHDLTEST_NVC_PATH` when it is set, confirming the environment variable override.
This scenario is tested by `NvcSimulator_FindPath_WithEnvVar_ReturnsEnvVarValue`.

**FindPath_WithoutEnvVar_ReturnsNullOrPath**: Verifies that `NvcSimulator.FindPath()` does not
throw when `VHDLTEST_NVC_PATH` is not set, returning either a valid path string (when NVC is
installed) or null (when not installed).
This scenario is tested by `NvcSimulator_FindPath_WithoutEnvVar_ReturnsNullOrPath`.
