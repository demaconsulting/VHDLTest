### ModelSimSimulator

#### Verification Approach

`ModelSimSimulator` is verified through unit tests in
`test/DEMAConsulting.VHDLTest.Tests/Simulators/ModelSimSimulatorTests.cs`. The compile and
test `RunProcessor` instances are exercised by calling `Parse` with pre-captured output
strings, covering clean output, warnings, and errors without requiring ModelSim to be
installed. ModelSim integration with real VHDL source files is verified by CI jobs in
environments with ModelSim installed, using the self-validation path
(`--validate --simulator modelsim`).

#### Test Environment

Automated unit tests: standard test environment, no ModelSim installation required.
Live simulator integration: CI environment with ModelSim installed on PATH.

#### Acceptance Criteria

- All unit tests in `ModelSimSimulatorTests.cs` pass with zero failures.
- `ModelSimSimulator.Instance.SimulatorName` returns `"ModelSim"`.
- The compile processor correctly classifies clean and error output patterns.
- The test processor correctly classifies clean, info, warning, error, and failure output patterns.
- Calling `Compile()` or `Test()` when the simulator is not installed throws `InvalidOperationException`.
- `FindPath()` returns the value of `VHDLTEST_MODELSIM_PATH` when that environment variable is set.

#### Test Scenarios

**SimulatorName_ReturnsModelSim**: Verifies that `ModelSimSimulator.Instance.SimulatorName`
is `"ModelSim"`, confirming the instance is registered with the correct name.
This scenario is tested by `ModelSimSimulator_SimulatorName_ReturnsModelSim` in `ModelSimSimulatorTests.cs`.

**CompileProcessor_CleanOutput_ReturnsTextResult**: Verifies that clean ModelSim compilation
output produces a `RunLineType.Text` summary.
This scenario is tested by `ModelSimSimulator_CompileProcessor_CleanOutput_ReturnsTextResult` in `ModelSimSimulatorTests.cs`.

**CompileProcessor_ErrorOutput_ReturnsErrorResult**: Verifies that a ModelSim error line
is classified as `RunLineType.Error`.
This scenario is tested by `ModelSimSimulator_CompileProcessor_ErrorOutput_ReturnsErrorResult` in `ModelSimSimulatorTests.cs`.

**TestProcessor_CleanOutput_ReturnsTextResult**: Verifies that clean ModelSim simulation
output produces a `RunLineType.Text` summary.
This scenario is tested by `ModelSimSimulator_TestProcessor_CleanOutput_ReturnsTextResult` in `ModelSimSimulatorTests.cs`.

**TestProcessor_InfoOutput_ReturnsInfoResult**: Verifies that a ModelSim note in simulation
output is classified as `RunLineType.Info`.
This scenario is tested by `ModelSimSimulator_TestProcessor_InfoOutput_ReturnsInfoResult` in `ModelSimSimulatorTests.cs`.

**TestProcessor_WarningOutput_ReturnsWarningResult**: Verifies that a ModelSim warning in
simulation output is classified as `RunLineType.Warning`.
This scenario is tested by `ModelSimSimulator_TestProcessor_WarningOutput_ReturnsWarningResult` in `ModelSimSimulatorTests.cs`.

**TestProcessor_ErrorOutput_ReturnsErrorResult**: Verifies that a ModelSim error in
simulation output is classified as `RunLineType.Error`.
This scenario is tested by `ModelSimSimulator_TestProcessor_ErrorOutput_ReturnsErrorResult` in `ModelSimSimulatorTests.cs`.

**TestProcessor_FailureOutput_ReturnsErrorResult**: Verifies that a ModelSim failure in
simulation output is classified as `RunLineType.Error`, confirming that VHDL assertion
failure lines are treated as errors.
This scenario is tested by `ModelSimSimulator_TestProcessor_FailureOutput_ReturnsErrorResult`
in `ModelSimSimulatorTests.cs`.

**Compile_SimulatorNotAvailable_ThrowsInvalidOperationException**: Verifies that `Compile()`
throws `InvalidOperationException` with message containing `"ModelSim Simulator not available"`
when `SimulatorPath` is null. This test is skipped in environments where ModelSim is installed.
This scenario is tested by `ModelSimSimulator_Compile_SimulatorNotAvailable_ThrowsInvalidOperationException`
in `ModelSimSimulatorTests.cs`.

**Test_SimulatorNotAvailable_ThrowsInvalidOperationException**: Verifies that `Test()`
throws `InvalidOperationException` with message containing `"ModelSim Simulator not available"`
when `SimulatorPath` is null. This test is skipped in environments where ModelSim is installed.
This scenario is tested by `ModelSimSimulator_Test_SimulatorNotAvailable_ThrowsInvalidOperationException`
in `ModelSimSimulatorTests.cs`.

**FindPath_WithEnvVar_ReturnsEnvVarValue**: Verifies that `FindPath()` returns the value of the
`VHDLTEST_MODELSIM_PATH` environment variable when it is set, confirming that the env var
override takes precedence over PATH discovery.
This scenario is tested by `ModelSimSimulator_FindPath_WithEnvVar_ReturnsEnvVarValue`
in `ModelSimSimulatorTests.cs`.

**FindPath_WithoutEnvVar_ReturnsNullOrPath**: Verifies that `FindPath()` does not throw when
`VHDLTEST_MODELSIM_PATH` is not set, and returns either a non-empty path string (ModelSim
installed) or null (ModelSim not installed). This confirms that PATH-based discovery is safe
to call in environments without ModelSim.
This scenario is tested by `ModelSimSimulator_FindPath_WithoutEnvVar_ReturnsNullOrPath`
in `ModelSimSimulatorTests.cs`.

**Compile_WithValidConfig_InvokesVcom**: Verifies that `ModelSimSimulator.Compile()` invokes the
`vcom` executable (directly or via `cmd /c` on Windows) with the expected arguments,
using `CreateForTesting` with a `FakeProcessInvoker` to capture the invocation without
launching a real process.
This scenario is tested by `ModelSimSimulator_Compile_WithValidConfig_InvokesVcom`
in `ModelSimSimulatorTests.cs`.

**Test_WithValidConfig_InvokesVsim**: Verifies that `ModelSimSimulator.Test()` invokes the
`vsim` executable (directly or via `cmd /c` on Windows) with the expected simulation arguments
for the specified test bench, using `CreateForTesting` with a `FakeProcessInvoker`.
This scenario is tested by `ModelSimSimulator_Test_WithValidConfig_InvokesVsim`
in `ModelSimSimulatorTests.cs`.

**CompileAndTest_WithValidConfig_WritesDoScript**: Verifies that `ModelSimSimulator.Compile()`
and `Test()` write the TCL do-scripts to the expected paths in the working directory.
Uses `CreateForTesting` with a `FakeProcessInvoker`.
This scenario is tested by `ModelSimSimulator_CompileAndTest_WithValidConfig_WritesDoScript`
in `ModelSimSimulatorTests.cs`.
