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
- The compile processor correctly classifies clean, warning, and error output patterns.
- The test processor correctly classifies clean, warning, and error output patterns.

#### Test Scenarios

**SimulatorName_ReturnsQuestaSim**: Verifies that `QuestaSimSimulator.Instance.SimulatorName`
is `"QuestaSim"`, confirming the instance is registered with the correct name.
This scenario is tested by the simulator name test in `QuestaSimSimulatorTests.cs`.

**CompileProcessor_CleanOutput_ReturnsTextResult**: Verifies that clean QuestaSim compilation
output produces a `RunLineType.Text` summary.
This scenario is tested by the compile clean output test in `QuestaSimSimulatorTests.cs`.

**CompileProcessor_WarningOutput_ReturnsWarningResult**: Verifies that a QuestaSim warning
line is classified as `RunLineType.Warning`.
This scenario is tested by the compile warning test in `QuestaSimSimulatorTests.cs`.

**CompileProcessor_ErrorOutput_ReturnsErrorResult**: Verifies that a QuestaSim error line
is classified as `RunLineType.Error`.
This scenario is tested by the compile error test in `QuestaSimSimulatorTests.cs`.

**TestProcessor_CleanOutput_ReturnsTextResult**: Verifies that clean QuestaSim simulation
output produces a `RunLineType.Text` summary.
This scenario is tested by the test clean output test in `QuestaSimSimulatorTests.cs`.

**TestProcessor_WarningOutput_ReturnsWarningResult**: Verifies that a QuestaSim warning in
simulation output is classified as `RunLineType.Warning`.
This scenario is tested by the test warning output test in `QuestaSimSimulatorTests.cs`.

**TestProcessor_ErrorOutput_ReturnsErrorResult**: Verifies that a QuestaSim error in
simulation output is classified as `RunLineType.Error`.
This scenario is tested by the test error output test in `QuestaSimSimulatorTests.cs`.
