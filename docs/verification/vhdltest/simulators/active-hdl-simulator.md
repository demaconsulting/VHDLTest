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
- `ActiveHdlSimulator.Instance.SimulatorName` returns `"ActiveHdl"`.
- The compile processor correctly classifies clean, warning, and error output patterns.
- The test processor correctly classifies clean, info, warning, error, and Lattice Edition
  suppression patterns.

#### Test Scenarios

**SimulatorName_ReturnsActiveHDL**: Verifies that `ActiveHdlSimulator.Instance.SimulatorName`
is `"ActiveHdl"`, confirming the instance is registered with the correct name for factory
lookup.
This scenario is tested by the simulator name test in `ActiveHdlSimulatorTests.cs`.

**CompileProcessor_CleanOutput_ReturnsTextResult**: Verifies that clean Active-HDL
compilation output produces a `RunLineType.Text` summary.
This scenario is tested by the compile clean output test in `ActiveHdlSimulatorTests.cs`.

**CompileProcessor_WarningOutput_ReturnsWarningResult**: Verifies that an Active-HDL warning
line (`KERNEL: Warning:`) is classified as `RunLineType.Warning`.
This scenario is tested by the compile warning test in `ActiveHdlSimulatorTests.cs`.

**CompileProcessor_ErrorOutput_ReturnsErrorResult**: Verifies that an Active-HDL error line
is classified as `RunLineType.Error`.
This scenario is tested by the compile error test in `ActiveHdlSimulatorTests.cs`.

**CompileProcessor_FatalRuntimeError_ReturnsErrorResult**: Verifies that a `RUNTIME: Fatal Error`
line in compile output is classified as `RunLineType.Error`.
This scenario is tested by `ActiveHdlSimulator_CompileProcessor_FatalRuntimeError_ReturnsErrorResult`
in `ActiveHdlSimulatorTests.cs`.

**TestProcessor_CleanOutput_ReturnsTextResult**: Verifies that clean Active-HDL simulation
output produces a `RunLineType.Text` summary.
This scenario is tested by the test clean output test in `ActiveHdlSimulatorTests.cs`.

**TestProcessor_InfoOutput_ReturnsInfoResult**: Verifies that an `EXECUTION:: NOTE` line
is classified as `RunLineType.Info`.
This scenario is tested by `ActiveHdlSimulator_TestProcessor_InfoOutput_ReturnsInfoResult`
in `ActiveHdlSimulatorTests.cs`.

**TestProcessor_WarningOutput_ReturnsWarningResult**: Verifies that an `EXECUTION:: WARNING`
line is classified as `RunLineType.Warning`.
This scenario is tested by the test warning output test in `ActiveHdlSimulatorTests.cs`.

**TestProcessor_ErrorOutput_ReturnsErrorResult**: Verifies that an `EXECUTION:: ERROR` line
is classified as `RunLineType.Error`.
This scenario is tested by the test error output test in `ActiveHdlSimulatorTests.cs`.

**TestProcessor_LatticeSuppression1_ReturnsTextResult**: Verifies that the first Active-HDL
Lattice Edition advisory (`KERNEL: Warning: You are using the Active-HDL Lattice Edition`)
is suppressed and classified as `RunLineType.Text` rather than `RunLineType.Warning`.
This scenario is tested by `ActiveHdlSimulator_TestProcessor_LatticeSuppression1_ReturnsTextResult`
in `ActiveHdlSimulatorTests.cs`.

**TestProcessor_LatticeSuppression2_ReturnsTextResult**: Verifies that the second Active-HDL
Lattice Edition advisory (`KERNEL: Warning: Contact Aldec for available upgrade options`)
is suppressed and classified as `RunLineType.Text` rather than `RunLineType.Warning`.
This scenario is tested by `ActiveHdlSimulator_TestProcessor_LatticeSuppression2_ReturnsTextResult`
in `ActiveHdlSimulatorTests.cs`.

**TestProcessor_KernelWarning_ReturnsWarningResult**: Verifies that a `KERNEL: Warning:` line
that is not an Aldec advisory is classified as `RunLineType.Warning`.
This scenario is tested by `ActiveHdlSimulator_TestProcessor_KernelWarning_ReturnsWarningResult`
in `ActiveHdlSimulatorTests.cs`.

**TestProcessor_KernelWarningUpper_ReturnsWarningResult**: Verifies that a `KERNEL: WARNING:`
(uppercase) line is classified as `RunLineType.Warning`.
This scenario is tested by `ActiveHdlSimulator_TestProcessor_KernelWarningUpper_ReturnsWarningResult`
in `ActiveHdlSimulatorTests.cs`.

**TestProcessor_ExecutionFailure_ReturnsErrorResult**: Verifies that an `EXECUTION:: FAILURE`
line is classified as `RunLineType.Error`.
This scenario is tested by `ActiveHdlSimulator_TestProcessor_ExecutionFailure_ReturnsErrorResult`
in `ActiveHdlSimulatorTests.cs`.

**TestProcessor_KernelError_ReturnsErrorResult**: Verifies that a `KERNEL: ERROR` line is
classified as `RunLineType.Error`.
This scenario is tested by `ActiveHdlSimulator_TestProcessor_KernelError_ReturnsErrorResult`
in `ActiveHdlSimulatorTests.cs`.

**TestProcessor_RuntimeFatalError_ReturnsErrorResult**: Verifies that a `RUNTIME: Fatal Error:`
line is classified as `RunLineType.Error`.
This scenario is tested by `ActiveHdlSimulator_TestProcessor_RuntimeFatalError_ReturnsErrorResult`
in `ActiveHdlSimulatorTests.cs`.

**Test_WithCleanConfig_AppendsTclExitCode**: Verifies that the TCL test script written by
`ActiveHdlSimulator.Test()` contains `exit -code 0` to signal successful simulation
completion to vsimsa.
This scenario is tested by `ActiveHdlSimulator_Test_WithCleanConfig_AppendsTclExitCode`
in `ActiveHdlSimulatorTests.cs`.
