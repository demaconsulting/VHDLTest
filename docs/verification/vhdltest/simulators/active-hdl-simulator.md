### ActiveHdlSimulator

#### Verification Approach

`ActiveHdlSimulator` is verified through unit tests in
`test/DEMAConsulting.VHDLTest.Tests/Simulators/ActiveHdlSimulatorTests.cs`. The compile
and test `RunProcessor` instances are exercised by calling `Parse` with pre-captured output
strings, covering clean output, warnings, and errors without requiring Active-HDL to be
installed. Active-HDL integration with real VHDL source files is verified by CI jobs in
environments with Active-HDL installed, using the self-validation path
(`--validate --simulator activehdl`).

#### Test Environment

Automated unit tests: standard test environment, no Active-HDL installation required.
Live simulator integration: CI environment with Active-HDL installed on PATH.

#### Acceptance Criteria

- All unit tests in `ActiveHdlSimulatorTests.cs` pass with zero failures.
- `ActiveHdlSimulator.Instance.SimulatorName` returns `"ActiveHDL"`.
- The compile processor correctly classifies clean, warning, and error output patterns.
- The test processor correctly classifies clean, warning, and error output patterns.

#### Test Scenarios

**SimulatorName_ReturnsActiveHDL**: Verifies that `ActiveHdlSimulator.Instance.SimulatorName`
is `"ActiveHDL"`, confirming the instance is registered with the correct name for factory
lookup.
This scenario is tested by the simulator name test in `ActiveHdlSimulatorTests.cs`.

**CompileProcessor_CleanOutput_ReturnsTextResult**: Verifies that clean Active-HDL
compilation output produces a `RunLineType.Text` summary.
This scenario is tested by the compile clean output test in `ActiveHdlSimulatorTests.cs`.

**CompileProcessor_WarningOutput_ReturnsWarningResult**: Verifies that an Active-HDL warning
line is classified as `RunLineType.Warning`.
This scenario is tested by the compile warning test in `ActiveHdlSimulatorTests.cs`.

**CompileProcessor_ErrorOutput_ReturnsErrorResult**: Verifies that an Active-HDL error line
is classified as `RunLineType.Error`.
This scenario is tested by the compile error test in `ActiveHdlSimulatorTests.cs`.

**TestProcessor_CleanOutput_ReturnsTextResult**: Verifies that clean Active-HDL simulation
output produces a `RunLineType.Text` summary.
This scenario is tested by the test clean output test in `ActiveHdlSimulatorTests.cs`.

**TestProcessor_WarningOutput_ReturnsWarningResult**: Verifies that an Active-HDL warning
in simulation output is classified as `RunLineType.Warning`.
This scenario is tested by the test warning output test in `ActiveHdlSimulatorTests.cs`.

**TestProcessor_ErrorOutput_ReturnsErrorResult**: Verifies that an Active-HDL error in
simulation output is classified as `RunLineType.Error`.
This scenario is tested by the test error output test in `ActiveHdlSimulatorTests.cs`.
