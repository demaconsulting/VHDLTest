## Results

### Verification Approach

The Results subsystem is verified through unit tests and a subsystem integration test. Unit
tests in `TestResultTests.cs` exercise `TestResult` construction with passing and failing
`RunResults` objects. Unit tests in `TestResultsTests.cs` exercise `TestResults.SaveResults`
for TRX and JUnit XML formats, `SaveToTrx` backward compatibility, null/empty file name
validation, and unknown extension defaulting to TRX. A subsystem integration test in
`ResultsSubsystemTests.cs` verifies that `TestResult` and `TestResults` work together to
aggregate pass and fail counts and to save a combined result set to a TRX file.

No real simulator is invoked; `RunResults` instances are constructed directly from
in-memory data for all Results tests.

### Test Environment

N/A - standard test environment.

### Acceptance Criteria

- All unit tests in `TestResultTests.cs` and `TestResultsTests.cs` pass with zero failures.
- All integration tests in `ResultsSubsystemTests.cs` pass with zero failures.
- `TestResults.SaveResults` produces valid TRX XML for `.trx` extensions
  (verified at unit level in `TestResultsTests.cs`).
- `TestResults.SaveResults` produces valid JUnit XML for `.xml` extensions
  (verified at unit level in `TestResultsTests.cs`).
- `TestResults.SaveResults` throws `ArgumentException` for null or empty file names
  (verified at unit level in `TestResultsTests.cs`).
- `Execute` with a valid config and MockSimulator returns a `TestResults` instance with
  `BuildResults` populated and `Tests` containing at least one outcome.
- `PrintSummary` writes per-test names and aggregate "Passed N of M" and/or "Failed N of M"
  indicators to the output.

### Test Scenarios

**CollectAndSummarize_WithMixedResults_ReportsCorrectPassFailCounts**: Verifies that adding
two passing and one failing `TestResult` to a `TestResults` instance results in `Passes.Count()`
of 2 and `Fails.Count()` of 1, confirming the subsystem correctly aggregates outcomes.
This scenario is tested by
`ResultsSubsystem_CollectAndSummarize_WithMixedResults_ReportsCorrectPassFailCounts`.

**SaveMixedResults_ToTrxFile_CreatesTrxFileWithCorrectCounts**: Verifies that saving a
`TestResults` instance containing both passing and failing tests to a TRX file creates a
valid file whose XML content contains both test names, confirming the serialization
integration across `TestResult` and `TestResults`.
This scenario is tested by
`ResultsSubsystem_SaveMixedResults_ToTrxFile_CreatesTrxFileWithCorrectCounts`.

**Execute_WithMockSimulator_CollectsBuildAndTestOutcomes**: Verifies that `Execute` with
MockSimulator completes the full compile-and-test workflow, returning a `TestResults` instance
with `BuildResults` populated and `Tests` containing one outcome whose `TestName` matches the
configured test bench name.
This scenario is tested by
`ResultsSubsystem_Execute_WithMockSimulator_CollectsBuildAndTestOutcomes`.

**PrintSummary_WithMixedResults_WritesFormattedSummaryToOutput**: Verifies that `PrintSummary`
writes per-test names and aggregate pass/fail indicators (including test name strings and
pass/fail label words) to the log output for a mixed result set.
This scenario is tested by
`ResultsSubsystem_PrintSummary_WithMixedResults_WritesFormattedSummaryToOutput`.

**Execute_WithBuildFailure_ThrowsInvalidOperationException**: Verifies that `Execute` throws
`InvalidOperationException` with the message `"Build Failed"` when the simulator reports an
error-severity outcome during the build phase, confirming that the subsystem aborts test
execution before any test bench is run and propagates the failure clearly to the caller.
This scenario is tested by
`ResultsSubsystem_Execute_WithBuildFailure_ThrowsInvalidOperationException`.
