### Simulator

#### Verification Approach

The abstract `Simulator` base class is verified indirectly through its concrete subclasses.
There are no direct unit tests for the abstract class itself; instead `SimulatorTests.cs`
verifies the `Available()` method and the `Where()` PATH-search utility through the concrete
subclasses that inherit them. Each concrete simulator's unit tests exercise
`SimulatorName` and `Available()` properties, confirming that the base class contract
is correctly implemented.

#### Test Environment

N/A - standard test environment.

#### Acceptance Criteria

- All tests in `SimulatorTests.cs` pass with zero failures.
- All relevant tests in `MockSimulatorTests.cs` pass with zero failures.
- `Available()` returns false when `SimulatorPath` is null.
- `SimulatorName` returns the value supplied at construction.

#### Test Scenarios

**SimulatorName_ReturnsRegisteredName**: Verifies that each concrete simulator exposes its
registered name string via `SimulatorName`, confirming the base class property is set
correctly through the primary constructor.
This scenario is tested by the `SimulatorName` tests in each concrete simulator test class
(e.g., `GhdlSimulator_SimulatorName_ReturnsGHDL`, `MockSimulator_SimulatorName_ReturnsMock`).

**Available_WithNullPath_ReturnsFalse**: Verifies that `Available()` returns false when
no simulator executable is found on PATH, which is the expected state in the standard
test environment where real simulators are not installed.
This scenario is tested by `MockSimulator_Available_WithNullPath_ReturnsFalse`.

**Where_UnknownExecutable_ReturnsNull**: Verifies that `Where()` returns null when searching
for an executable name that does not exist on the system PATH, confirming the PATH search
returns null rather than throwing when no match is found.
This scenario is tested by `Simulator_Where_UnknownExecutable_ReturnsNull` in `SimulatorTests.cs`.

**Available_WithNonNullPath_ReturnsTrue**: Verifies that `Available()` returns true when
`SimulatorPath` is non-null, confirming the availability check correctly reflects the
constructed path.
This scenario is tested by `Simulator_Available_WithNonNullPath_ReturnsTrue` in `SimulatorTests.cs`.
