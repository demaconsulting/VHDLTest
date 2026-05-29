### RunResults

#### Verification Approach

`RunResults` is verified through direct unit tests in `RunResultsTests.cs` and indirectly
through `RunProcessor` tests and simulator processor tests. The `Parse` method of
`RunProcessor` constructs `RunResults` instances, and the tests in `RunProcessorTests.cs`,
`RunSubsystemTests.cs`, and simulator test classes assert the properties of the returned
`RunResults` — including `Summary`, `Start`, `Duration`, `ExitCode`, `Output`, and `Lines`.
The `Print` method is directly exercised by unit tests using a log file to capture and
assert on the written output.

#### Test Environment

N/A - standard test environment.

#### Acceptance Criteria

- `RunResults` instances are correctly populated by `RunProcessor.Parse` in all tests.
- `Summary` reflects the highest-severity line type, elevated to `RunLineType.Error` when
  the exit code is non-zero.
- `Duration` is non-negative and reflects elapsed time between start and end timestamps.
- `Lines` contains one entry per output line with the correct type classification.
- `Print` writes all line types (Info, Warning, Error) to the log output when verbose is enabled.
- `Print` suppresses Text-classified lines when verbose is disabled.
- `Print` writes non-Text lines even when verbose is disabled.

#### Test Scenarios

**Print_WithMixedLines_WritesColorCodedOutput**: Verifies that `Print` writes lines of all
four severity types (Text, Info, Warning, Error) to the log when verbose is enabled,
confirming the color-selection and output code paths for all RunLineType values.
This scenario is tested by `RunResults_Print_WithMixedLines_WritesColorCodedOutput`.

**Print_WithVerboseDisabled_SuppressesTextLines**: Verifies that Text-classified lines do
not appear in the log when verbose is disabled, and that Info-classified lines are still
written, confirming the verbose-suppression logic.
This scenario is tested by `RunResults_Print_WithVerboseDisabled_SuppressesTextLines`.

**Summary_WhenExitCodeNonZero_IsAtLeastError**: Verifies that a RunResults with a non-zero
exit code has Summary >= RunLineType.Error, confirming the SummaryElevation contract.
This scenario is tested by `RunResults_Summary_WhenExitCodeNonZero_IsAtLeastError`.

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
