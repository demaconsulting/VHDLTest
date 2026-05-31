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
- `NvcSimulator.Compile()` throws `InvalidOperationException` when the NVC simulator is not available.
- `NvcSimulator.Test()` throws `InvalidOperationException` when the NVC simulator is not available.

#### Test Scenarios

**SimulatorName_ReturnsNVC**: Verifies that `NvcSimulator.Instance.SimulatorName` is `"NVC"`,
confirming the instance is registered with the correct name for factory lookup.
This scenario is tested by `NvcSimulator_SimulatorName_ReturnsNVC`.

**Compile_WhenNotAvailable_ThrowsInvalidOperationException**: Verifies that `NvcSimulator.Compile()`
throws `InvalidOperationException` when the NVC simulator is not available (i.e., `SimulatorPath` is null),
preventing silent incorrect behavior when the simulator is not installed.
This scenario is tested by `NvcSimulator_Compile_WhenNotAvailable_ThrowsInvalidOperationException`.

**Test_WhenNotAvailable_ThrowsInvalidOperationException**: Verifies that `NvcSimulator.Test()`
throws `InvalidOperationException` when the NVC simulator is not available (i.e., `SimulatorPath` is null),
preventing silent incorrect behavior when the simulator is not installed.
This scenario is tested by `NvcSimulator_Test_WhenNotAvailable_ThrowsInvalidOperationException`.

**CompileProcessor_CleanOutput_ReturnsTextResult**: Verifies that clean NVC analysis output
produces a `RunLineType.Text` summary with correctly classified lines.
This scenario is tested by `NvcSimulator_CompileProcessor_CleanOutput_ReturnsTextResult`.

**CompileProcessor_InfoOutput_ReturnsInfoResult**: Verifies that an NVC info (note) line in
compile output is classified as `RunLineType.Info`, confirming the `.* Note:` pattern works.
This scenario is tested by `NvcSimulator_CompileProcessor_InfoOutput_ReturnsInfoResult`.

**CompileProcessor_WarningOutput_ReturnsWarningResult**: Verifies that an NVC warning line
is classified as `RunLineType.Warning`.
This scenario is tested by `NvcSimulator_CompileProcessor_WarningOutput_ReturnsWarningResult`.

**CompileProcessor_ErrorOutput_ReturnsErrorResult**: Verifies that an NVC error line is
classified as `RunLineType.Error`.
This scenario is tested by `NvcSimulator_CompileProcessor_ErrorOutput_ReturnsErrorResult`.

**CompileProcessor_FailureOutput_ReturnsErrorResult**: Verifies that an NVC failure line
matching `.* Failure:` in compile output is classified as `RunLineType.Error`.
This scenario is tested by `NvcSimulator_CompileProcessor_FailureOutput_ReturnsErrorResult`.

**CompileProcessor_FatalOutput_ReturnsErrorResult**: Verifies that an NVC fatal line
matching `.* Fatal:` in compile output is classified as `RunLineType.Error`.
This scenario is tested by `NvcSimulator_CompileProcessor_FatalOutput_ReturnsErrorResult`.

**TestProcessor_CleanOutput_ReturnsTextResult**: Verifies that clean NVC simulation output
produces a `RunLineType.Text` summary.
This scenario is tested by `NvcSimulator_TestProcessor_CleanOutput_ReturnsTextResult`.

**TestProcessor_InfoOutput_ReturnsInfoResult**: Verifies that an NVC info line is classified
as `RunLineType.Info`.
This scenario is tested by `NvcSimulator_TestProcessor_InfoOutput_ReturnsInfoResult`.

**TestProcessor_WarningOutput_ReturnsWarningResult**: Verifies that an NVC warning line in
simulation output is classified as `RunLineType.Warning`.
This scenario is tested by `NvcSimulator_TestProcessor_WarningOutput_ReturnsWarningResult`.

**TestProcessor_ErrorOutput_ReturnsErrorResult**: Verifies that an NVC error line in
simulation output is classified as `RunLineType.Error`.
This scenario is tested by `NvcSimulator_TestProcessor_ErrorOutput_ReturnsErrorResult`.

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

**Compile_WithValidConfig_InvokesNvc**: Verifies that `NvcSimulator.Compile()` invokes the
`nvc` executable (directly or via `cmd /c` on Windows) with the expected analysis arguments,
using `CreateForTesting` with a `FakeProcessInvoker` to capture the invocation without
launching a real process.
This scenario is tested by `NvcSimulator_Compile_WithValidConfig_InvokesNvc`.

**Test_WithValidConfig_InvokesNvc**: Verifies that `NvcSimulator.Test()` invokes the
`nvc` executable (directly or via `cmd /c` on Windows) with elaboration and run arguments
for the specified test bench, using `CreateForTesting` with a `FakeProcessInvoker`.
This scenario is tested by `NvcSimulator_Test_WithValidConfig_InvokesNvc`.
