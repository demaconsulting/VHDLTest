### Simulator

#### Verification Approach

The abstract `Simulator` base class is verified indirectly through its concrete subclasses.
There are no direct unit tests for the abstract class itself; instead `SimulatorTests.cs`
defines a private `TestableSimulator` test double (not a production concrete simulator) whose
`Compile`/`Test` overrides both unconditionally throw `InvalidOperationException`, and uses
that test double solely to exercise `Where()` and `Available()` in isolation from any
production simulator's compile/test logic. Each concrete production simulator's own unit tests
separately exercise the `SimulatorName` and `Available()` properties, confirming that the base
class contract is correctly implemented.

`MockSimulator` is a test fixture (not a production software unit) whose sole purpose is to
exercise the `Simulator` abstract base class in isolation. `MockSimulatorTests` collectively
verify the base-class interface contract — name, availability, compile, and test-execution
behavior — covering 20 test cases. `MockSimulator` has its own verification document,
`mock-simulator.md`, covering its classifier and pattern-matching behavior in detail; its
tests are the exclusive source of happy-path coverage for the `Simulator` abstract base
class's `Compile`/`Test` contract, exercised below — `SimulatorTests.cs`'s `TestableSimulator`
does not contribute to Compile/Test coverage since both of its overrides always throw.

#### Test Environment

N/A - standard test environment.

#### Acceptance Criteria

- All tests in `SimulatorTests.cs` pass with zero failures.
- All relevant tests in `MockSimulatorTests.cs` pass with zero failures.
- `Available()` returns false when `SimulatorPath` is null.
- `SimulatorName` returns the value supplied at construction.
- The abstract `Compile`/`Test` contract's success path returns a clean-text `RunResults`/`TestResult`.

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

**Where_ExistingExecutable_ReturnsNonNull**: Verifies that `Where()` returns a non-null path
when searching for an executable that is present on the system PATH, confirming the PATH
search correctly locates installed tools.
This scenario is tested by `Simulator_Where_ExistingExecutable_ReturnsNonNull` in `SimulatorTests.cs`.

**Where_UnknownExecutable_ReturnsNull**: Verifies that `Where()` returns null when searching
for an executable name that does not exist on the system PATH, confirming the PATH search
returns null rather than throwing when no match is found.
This scenario is tested by `Simulator_Where_UnknownExecutable_ReturnsNull` in `SimulatorTests.cs`.

**Available_WithNonNullPath_ReturnsTrue**: Verifies that `Available()` returns true when
`SimulatorPath` is non-null, confirming the availability check correctly reflects the
constructed path.
This scenario is tested by `Simulator_Available_WithNonNullPath_ReturnsTrue` in `SimulatorTests.cs`.

**Compile_Test_HappyPath_ReturnsCleanTextResult**: Verifies that the abstract base class's
`Compile`/`Test` contract's success path returns a clean `RunLineType.Text` result when the
compile/test operation completes with no diagnostic output, exercised through `MockSimulator`
(the base class's own test double). No new test is required — this is already covered by the
existing `MockSimulator_Compile_WithCleanFile_ReturnsSuccessResult` and
`MockSimulator_Test_WithCleanName_ReturnsSuccessResult` tests documented in full in
`mock-simulator.md`.
This scenario is tested by `MockSimulator_Compile_WithCleanFile_ReturnsSuccessResult` and
`MockSimulator_Test_WithCleanName_ReturnsSuccessResult` in `MockSimulatorTests.cs`.
