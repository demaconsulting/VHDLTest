### VivadoSimulator

#### Verification Approach

`VivadoSimulator` is verified through unit tests in
`test/DEMAConsulting.VHDLTest.Tests/Simulators/VivadoSimulatorTests.cs`. The compile and
test `RunProcessor` instances are exercised by calling `Parse` with pre-captured output
strings, covering clean output, warnings, and errors without requiring Vivado to be
installed. Vivado integration with real VHDL source files is verified by CI jobs in
environments with Vivado installed, using the self-validation path
(`--validate --simulator vivado`).

#### Test Environment

Automated unit tests: standard test environment, no Vivado installation required.
Live simulator integration: CI environment with Vivado installed on PATH.

#### Acceptance Criteria

- All unit tests in `VivadoSimulatorTests.cs` pass with zero failures.
- `VivadoSimulator.Instance.SimulatorName` returns `"Vivado"`.
- The compile processor correctly classifies clean and error output patterns.
- The test processor correctly classifies clean, info, warning, error, and failure output patterns.
- Calling `Compile()` or `Test()` when the simulator is not installed throws `InvalidOperationException`.
- `FindPath()` returns the value of `VHDLTEST_VIVADO_PATH` when that environment variable is set.

#### Test Scenarios

**SimulatorName_ReturnsVivado**: Verifies that `VivadoSimulator.Instance.SimulatorName`
is `"Vivado"`, confirming the instance is registered with the correct name.
This scenario is tested by `VivadoSimulator_SimulatorName_ReturnsVivado` in `VivadoSimulatorTests.cs`.

**CompileProcessor_CleanOutput_ReturnsTextResult**: Verifies that clean Vivado compilation
output produces a `RunLineType.Text` summary.
This scenario is tested by `VivadoSimulator_CompileProcessor_CleanOutput_ReturnsTextResult` in `VivadoSimulatorTests.cs`.

**CompileProcessor_ErrorOutput_ReturnsErrorResult**: Verifies that a Vivado error line is
classified as `RunLineType.Error`.
This scenario is tested by `VivadoSimulator_CompileProcessor_ErrorOutput_ReturnsErrorResult` in `VivadoSimulatorTests.cs`.

**TestProcessor_CleanOutput_ReturnsTextResult**: Verifies that clean Vivado simulation
output produces a `RunLineType.Text` summary.
This scenario is tested by `VivadoSimulator_TestProcessor_CleanOutput_ReturnsTextResult` in `VivadoSimulatorTests.cs`.

**TestProcessor_InfoOutput_ReturnsInfoResult**: Verifies that a Vivado note in simulation
output is classified as `RunLineType.Info`.
This scenario is tested by `VivadoSimulator_TestProcessor_InfoOutput_ReturnsInfoResult` in `VivadoSimulatorTests.cs`.

**TestProcessor_WarningOutput_ReturnsWarningResult**: Verifies that a Vivado warning in
simulation output is classified as `RunLineType.Warning`.
This scenario is tested by `VivadoSimulator_TestProcessor_WarningOutput_ReturnsWarningResult` in `VivadoSimulatorTests.cs`.

**TestProcessor_ErrorOutput_ReturnsErrorResult**: Verifies that a Vivado error in
simulation output is classified as `RunLineType.Error`.
This scenario is tested by `VivadoSimulator_TestProcessor_ErrorOutput_ReturnsErrorResult` in `VivadoSimulatorTests.cs`.

**TestProcessor_FailureOutput_ReturnsErrorResult**: Verifies that a Vivado failure in
simulation output is classified as `RunLineType.Error`, confirming that assertion failure
lines are treated as errors.
This scenario is tested by `VivadoSimulator_TestProcessor_FailureOutput_ReturnsErrorResult`
in `VivadoSimulatorTests.cs`.

**Compile_WhenSimulatorNotAvailable_ThrowsInvalidOperationException**: Verifies that
`Compile()` throws `InvalidOperationException` with message `"Vivado Simulator not available"`
when `SimulatorPath` is null. This test is skipped in environments with Vivado installed.
This scenario is tested by `VivadoSimulator_Compile_WhenSimulatorNotAvailable_ThrowsInvalidOperationException`
in `VivadoSimulatorTests.cs`.

**Test_WhenSimulatorNotAvailable_ThrowsInvalidOperationException**: Verifies that `Test()`
throws `InvalidOperationException` with message `"Vivado Simulator not available"` when
`SimulatorPath` is null. This test is skipped in environments with Vivado installed.
This scenario is tested by `VivadoSimulator_Test_WhenSimulatorNotAvailable_ThrowsInvalidOperationException`
in `VivadoSimulatorTests.cs`.

**FindPath_WithEnvVar_ReturnsEnvVarValue**: Verifies that `FindPath()` returns the value of the
`VHDLTEST_VIVADO_PATH` environment variable when it is set, confirming that the env var
override takes precedence over PATH discovery.
This scenario is tested by `VivadoSimulator_FindPath_WithEnvVar_ReturnsEnvVarValue`
in `VivadoSimulatorTests.cs`.
