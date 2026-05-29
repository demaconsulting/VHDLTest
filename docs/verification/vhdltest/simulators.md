## Simulators

### Verification Approach

The Simulators subsystem is verified at two levels. Unit tests for each concrete simulator
class (`GhdlSimulatorTests.cs`, `NvcSimulatorTests.cs`, `ModelSimSimulatorTests.cs`,
`QuestaSimSimulatorTests.cs`, `VivadoSimulatorTests.cs`, `ActiveHdlSimulatorTests.cs`,
`MockSimulatorTests.cs`) exercise each simulator's output-classification processors
(compile and test `RunProcessor` instances) using `Parse` with pre-captured output strings,
covering clean, info, warning, and error output categories. `SimulatorFactoryTests.cs` tests
name-to-instance resolution. A subsystem integration test (`SimulatorsSubsystemTests.cs`)
verifies that the factory and a processor work together end-to-end.

Production simulators (GHDL, NVC, ModelSim, QuestaSim, Vivado, Active-HDL) are not
invoked live in the automated test suite; availability is verified by CI jobs that install
the respective simulator. The `MockSimulator` is used for all pipeline tests.

### Test Environment

N/A - standard test environment. Live simulator tests are run in separate CI environments
that have the respective VHDL simulator installed.

### Acceptance Criteria

- All unit tests for each simulator class pass with zero failures.
- All unit tests in `SimulatorFactoryTests.cs` pass with zero failures.
- All integration tests in `SimulatorsSubsystemTests.cs` pass with zero failures.
- Each simulator's compile and test processors correctly classify clean, info, warning, and
  error output patterns.

### Test Scenarios

**GetSimulatorAndProcessCompileOutput_WithErrorOutput_ClassifiesAsError**: Verifies that the
factory returns the GHDL simulator by name and that the GHDL compile processor correctly
classifies a line matching the GHDL error pattern as `RunLineType.Error`, confirming the
factory and processor interface work together.
This scenario is tested by
`SimulatorsSubsystem_GetSimulatorAndProcessCompileOutput_WithErrorOutput_ClassifiesAsError`.

**GetUnknownSimulatorAndProcessCleanOutput_ReturnsNullAndClassifiesText**: Verifies that
the factory returns null for an unrecognised name and that the NVC test processor classifies
clean output as `RunLineType.Text`, confirming null handling and processor correctness.
This scenario is tested by
`SimulatorsSubsystem_GetUnknownSimulatorAndProcessCleanOutput_ReturnsNullAndClassifiesText`.
