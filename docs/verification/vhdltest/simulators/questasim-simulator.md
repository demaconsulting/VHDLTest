### QuestaSimSimulator

#### Verification Approach

`QuestaSimSimulator` is verified through unit tests in
`test/DEMAConsulting.VHDLTest.Tests/Simulators/QuestaSimSimulatorTests.cs`. The compile
and test `RunProcessor` instances are exercised by calling `Parse` with pre-captured output
strings, covering clean output, warnings, and errors without requiring QuestaSim to be
installed. QuestaSim integration with real VHDL source files is verified by CI jobs in
environments with QuestaSim installed, using the self-validation path
(`--validate --simulator questasim`).

#### Test Environment

Automated unit tests: standard test environment, no QuestaSim installation required.
Live simulator integration: CI environment with QuestaSim installed on PATH.

#### Acceptance Criteria

- All unit tests in `QuestaSimSimulatorTests.cs` pass with zero failures.
- `QuestaSimSimulator.Instance.SimulatorName` returns `"QuestaSim"`.
- The compile processor correctly classifies clean and error output patterns.
- The test processor correctly classifies clean, info, warning, error, and failure output patterns.
- Calling `Compile()` when the simulator is not installed throws `InvalidOperationException`.
- Calling `Test()` when the simulator is not installed throws `InvalidOperationException`.
- `FindPath()` returns the value of `VHDLTEST_QUESTASIM_PATH` when that environment variable is set.

#### Test Scenarios

**SimulatorName_ReturnsQuestaSim**: Verifies that `QuestaSimSimulator.Instance.SimulatorName`
is `"QuestaSim"`, confirming the instance is registered with the correct name.
This scenario is tested by `QuestaSimSimulator_SimulatorName_ReturnsQuestaSim` in `QuestaSimSimulatorTests.cs`.

**CompileProcessor_CleanOutput_ReturnsTextResult**: Verifies that clean QuestaSim compilation
output produces a `RunLineType.Text` summary.
This scenario is tested by `QuestaSimSimulator_CompileProcessor_CleanOutput_ReturnsTextResult` in `QuestaSimSimulatorTests.cs`.

**CompileProcessor_ErrorOutput_ReturnsErrorResult**: Verifies that a QuestaSim error line
is classified as `RunLineType.Error`.
This scenario is tested by `QuestaSimSimulator_CompileProcessor_ErrorOutput_ReturnsErrorResult` in `QuestaSimSimulatorTests.cs`.

**TestProcessor_CleanOutput_ReturnsTextResult**: Verifies that clean QuestaSim simulation
output produces a `RunLineType.Text` summary.
This scenario is tested by `QuestaSimSimulator_TestProcessor_CleanOutput_ReturnsTextResult` in `QuestaSimSimulatorTests.cs`.

**TestProcessor_InfoOutput_ReturnsInfoResult**: Verifies that a QuestaSim note in simulation
output is classified as `RunLineType.Info`.
This scenario is tested by `QuestaSimSimulator_TestProcessor_InfoOutput_ReturnsInfoResult` in `QuestaSimSimulatorTests.cs`.

**TestProcessor_WarningOutput_ReturnsWarningResult**: Verifies that a QuestaSim warning in
simulation output is classified as `RunLineType.Warning`.
This scenario is tested by `QuestaSimSimulator_TestProcessor_WarningOutput_ReturnsWarningResult` in `QuestaSimSimulatorTests.cs`.

**TestProcessor_ErrorOutput_ReturnsErrorResult**: Verifies that a QuestaSim error in
simulation output is classified as `RunLineType.Error`.
This scenario is tested by `QuestaSimSimulator_TestProcessor_ErrorOutput_ReturnsErrorResult` in `QuestaSimSimulatorTests.cs`.

**TestProcessor_FailureOutput_ReturnsErrorResult**: Verifies that a QuestaSim failure in
simulation output is classified as `RunLineType.Error`, confirming that VHDL assertion
failure lines are treated as errors.
This scenario is tested by `QuestaSimSimulator_TestProcessor_FailureOutput_ReturnsErrorResult`
in `QuestaSimSimulatorTests.cs`.

**Compile_SimulatorNotAvailable_ThrowsInvalidOperationException**: Verifies that `Compile()`
throws `InvalidOperationException` when `SimulatorPath` is null (QuestaSim not installed).
This test is skipped in environments where QuestaSim is installed.
This scenario is tested by `QuestaSimSimulator_Compile_SimulatorNotAvailable_ThrowsInvalidOperationException`
in `QuestaSimSimulatorTests.cs`.

**Test_SimulatorNotAvailable_ThrowsInvalidOperationException**: Verifies that `Test()`
throws `InvalidOperationException` when `SimulatorPath` is null (QuestaSim not installed).
This test is skipped in environments where QuestaSim is installed.
This scenario is tested by `QuestaSimSimulator_Test_SimulatorNotAvailable_ThrowsInvalidOperationException`
in `QuestaSimSimulatorTests.cs`.

**FindPath_WithEnvVar_ReturnsEnvVarValue**: Verifies that `FindPath()` returns the value of the
`VHDLTEST_QUESTASIM_PATH` environment variable when it is set, confirming that the env var
override takes precedence over PATH discovery.
This scenario is tested by `QuestaSimSimulator_FindPath_WithEnvVar_ReturnsEnvVarValue`
in `QuestaSimSimulatorTests.cs`.

**FindPath_WithoutEnvVar_ReturnsNullOrPath**: Verifies that `FindPath()` does not throw when
`VHDLTEST_QUESTASIM_PATH` is not set, and that the result is either null (QuestaSim not
installed) or a non-empty path string (QuestaSim found on PATH).
This scenario is tested by `QuestaSimSimulator_FindPath_WithoutEnvVar_ReturnsNullOrPath`
in `QuestaSimSimulatorTests.cs`.

**Compile_WithValidConfig_InvokesVcom**: Verifies that `QuestaSimSimulator.Compile()` invokes the
`vcom` executable (directly or via `cmd /c` on Windows) with the expected arguments,
using `CreateForTesting` with a `FakeProcessInvoker` to capture the invocation without
launching a real process.
This scenario is tested by `QuestaSimSimulator_Compile_WithValidConfig_InvokesVcom`
in `QuestaSimSimulatorTests.cs`.

**Test_WithValidConfig_InvokesVsim**: Verifies that `QuestaSimSimulator.Test()` invokes the
`vsim` executable (directly or via `cmd /c` on Windows) with the expected simulation arguments
for the specified test bench, using `CreateForTesting` with a `FakeProcessInvoker`.
This scenario is tested by `QuestaSimSimulator_Test_WithValidConfig_InvokesVsim`
in `QuestaSimSimulatorTests.cs`.

**CompileAndTest_WithValidConfig_WritesDoScript**: Verifies that `QuestaSimSimulator.Compile()`
and `Test()` write the TCL do-scripts to the expected paths in the working directory.
Uses `CreateForTesting` with a `FakeProcessInvoker`.
This scenario is tested by `QuestaSimSimulator_CompileAndTest_WithValidConfig_WritesDoScript`
in `QuestaSimSimulatorTests.cs`.
