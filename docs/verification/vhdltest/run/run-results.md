### RunResults

#### Verification Approach

`RunResults` is an immutable record verified indirectly through `RunProcessor` tests and
simulator processor tests. The `Parse` method of `RunProcessor` constructs `RunResults`
instances, and the tests in `RunProcessorTests.cs`, `RunSubsystemTests.cs`, and simulator
test classes assert the properties of the returned `RunResults` — including `Summary`,
`Start`, `Duration`, `ExitCode`, `Output`, and `Lines`. The `Print` method is exercised
indirectly through simulator integration paths.

#### Test Environment

N/A - standard test environment.

#### Acceptance Criteria

- `RunResults` instances are correctly populated by `RunProcessor.Parse` in all tests.
- `Summary` reflects the highest-severity line type, elevated to `RunLineType.Error` when
  the exit code is non-zero.
- `Duration` is non-negative and reflects elapsed time between start and end timestamps.
- `Lines` contains one entry per output line with the correct type classification.

#### Test Scenarios

**CompileProcessor_CleanOutput_ReturnsTextResult** (via simulator tests): Verifies that a
`RunResults` constructed from clean output has `Summary == RunLineType.Text`, `ExitCode == 0`,
correct `Start` and `Duration`, and correctly typed `Lines` entries.
This scenario is tested by (for example) `GhdlSimulator_CompileProcessor_CleanOutput_ReturnsTextResult`.

**CompileProcessor_ErrorOutput_ReturnsErrorResult** (via simulator tests): Verifies that a
`RunResults` constructed from error output has `Summary == RunLineType.Error` and the error
line is classified correctly in `Lines`.
This scenario is tested by (for example) `GhdlSimulator_CompileProcessor_ErrorOutput_ReturnsErrorResult`.

**Execute_ProgramWithSuccess_ReturnsInfoResult** (via RunProcessor): Verifies that the
`RunResults` returned from a real process execution has non-empty `Output`, a non-empty
`Lines` collection, non-negative `Duration`, and the correct `Summary` type.
This scenario is tested by `RunSubsystem_ExecuteRealProgram_WithClassificationRules_ProducesClassifiedRunResults`.
