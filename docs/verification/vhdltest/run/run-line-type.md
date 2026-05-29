### RunLineType

#### Verification Approach

`RunLineType` is an enumeration verified indirectly through all tests that assert on
`RunResults.Summary` and `RunLine.Type` values. Every simulator processor test and
`RunProcessor` test asserts against specific `RunLineType` values. The severity ordering
(used by `RunProcessor.Parse` to compute the summary) is verified by any test that produces
multiple line types and asserts that the summary reflects the highest-severity type present.

#### Test Environment

N/A - standard test environment. `RunLineType` is an enum with no runtime dependencies.

#### Acceptance Criteria

- `RunLineType` values are correctly used in all processor and results tests with zero failures.
- The ordinal ordering `Text < Info < Warning < Error` is correctly exploited by
  `RunProcessor.Parse` to select the highest-severity summary type.
- A non-zero exit code forces the summary to at least `RunLineType.Error`.

#### Test Scenarios

**SeverityOrdering_TextIsLowest**: Verifies that clean output with no diagnostic lines
produces `Summary == RunLineType.Text`, confirming `Text` is the lowest severity and does
not elevate the summary.
This scenario is tested by (for example) `GhdlSimulator_CompileProcessor_CleanOutput_ReturnsTextResult`.

**SeverityOrdering_ErrorIsHighest**: Verifies that an error line elevates the summary to
`RunLineType.Error` even when other lower-severity lines are also present in the output.
This scenario is tested by (for example) `GhdlSimulator_CompileProcessor_ErrorOutput_ReturnsErrorResult`.

**NonZeroExitCode_ForcesErrorSummary**: Verifies that a non-zero exit code elevates the
`RunResults.Summary` to `RunLineType.Error` regardless of the line content, confirming
the exit-code threshold in `RunProcessor.Parse`.
This scenario is tested by `RunProcessor_Execute_ProgramWithError_ReturnsErrorResult`.
