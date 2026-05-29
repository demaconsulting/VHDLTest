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

#### Test Scenarios

**SimulatorName_ReturnsModelSim**: Verifies that `ModelSimSimulator.Instance.SimulatorName`
is `"ModelSim"`, confirming the instance is registered with the correct name.
This scenario is tested by the simulator name test in `ModelSimSimulatorTests.cs`.

**CompileProcessor_CleanOutput_ReturnsTextResult**: Verifies that clean ModelSim compilation
output produces a `RunLineType.Text` summary.
This scenario is tested by the compile clean output test in `ModelSimSimulatorTests.cs`.

**CompileProcessor_ErrorOutput_ReturnsErrorResult**: Verifies that a ModelSim error line
is classified as `RunLineType.Error`.
This scenario is tested by the compile error test in `ModelSimSimulatorTests.cs`.

**TestProcessor_CleanOutput_ReturnsTextResult**: Verifies that clean ModelSim simulation
output produces a `RunLineType.Text` summary.
This scenario is tested by the test clean output test in `ModelSimSimulatorTests.cs`.

**TestProcessor_InfoOutput_ReturnsInfoResult**: Verifies that a ModelSim note in simulation
output is classified as `RunLineType.Info`.
This scenario is tested by the test info output test in `ModelSimSimulatorTests.cs`.

**TestProcessor_WarningOutput_ReturnsWarningResult**: Verifies that a ModelSim warning in
simulation output is classified as `RunLineType.Warning`.
This scenario is tested by the test warning output test in `ModelSimSimulatorTests.cs`.

**TestProcessor_ErrorOutput_ReturnsErrorResult**: Verifies that a ModelSim error in
simulation output is classified as `RunLineType.Error`.
This scenario is tested by the test error output test in `ModelSimSimulatorTests.cs`.

**TestProcessor_FailureOutput_ReturnsErrorResult**: Verifies that a ModelSim failure in
simulation output is classified as `RunLineType.Error`, confirming that VHDL assertion
failure lines are treated as errors.
This scenario is tested by `ModelSimSimulator_TestProcessor_FailureOutput_ReturnsErrorResult`
in `ModelSimSimulatorTests.cs`.
