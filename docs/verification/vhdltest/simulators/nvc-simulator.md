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
- The compile processor correctly classifies clean, warning, and error output patterns.
- The test processor correctly classifies clean, info, warning, and error output patterns.

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
