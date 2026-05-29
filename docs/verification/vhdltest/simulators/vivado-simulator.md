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
- The compile processor correctly classifies clean, warning, and error output patterns.
- The test processor correctly classifies clean, warning, and error output patterns.

#### Test Scenarios

**SimulatorName_ReturnsVivado**: Verifies that `VivadoSimulator.Instance.SimulatorName`
is `"Vivado"`, confirming the instance is registered with the correct name.
This scenario is tested by the simulator name test in `VivadoSimulatorTests.cs`.

**CompileProcessor_CleanOutput_ReturnsTextResult**: Verifies that clean Vivado compilation
output produces a `RunLineType.Text` summary.
This scenario is tested by the compile clean output test in `VivadoSimulatorTests.cs`.

**CompileProcessor_WarningOutput_ReturnsWarningResult**: Verifies that a Vivado warning
line is classified as `RunLineType.Warning`.
This scenario is tested by the compile warning test in `VivadoSimulatorTests.cs`.

**CompileProcessor_ErrorOutput_ReturnsErrorResult**: Verifies that a Vivado error line is
classified as `RunLineType.Error`.
This scenario is tested by the compile error test in `VivadoSimulatorTests.cs`.

**TestProcessor_CleanOutput_ReturnsTextResult**: Verifies that clean Vivado simulation
output produces a `RunLineType.Text` summary.
This scenario is tested by the test clean output test in `VivadoSimulatorTests.cs`.

**TestProcessor_WarningOutput_ReturnsWarningResult**: Verifies that a Vivado warning in
simulation output is classified as `RunLineType.Warning`.
This scenario is tested by the test warning output test in `VivadoSimulatorTests.cs`.

**TestProcessor_ErrorOutput_ReturnsErrorResult**: Verifies that a Vivado error in
simulation output is classified as `RunLineType.Error`.
This scenario is tested by the test error output test in `VivadoSimulatorTests.cs`.
