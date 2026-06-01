### TestResults

#### Verification Approach

`TestResults` is verified through unit tests in
`test/DEMAConsulting.VHDLTest.Tests/Results/TestResultsTests.cs`. Tests exercise
`Execute` with MockSimulator (both success and build-failure paths), `PrintSummary` via
a log-file capture, `SaveResults` for TRX format (`.trx` extension), JUnit XML format
(`.xml` extension), unknown extension defaulting to TRX, and `ArgumentException` for null,
empty, and whitespace-only file names. The `SaveToTrx` backward-compatibility wrapper is
also tested. Each test that writes a file uses a temp path and cleans up in a `finally` block.
`RunResults` instances are constructed from in-memory data.

#### Test Environment

N/A - standard test environment.

#### Acceptance Criteria

- All unit tests in `TestResultsTests.cs` pass with zero failures.
- `Execute` with a valid config and MockSimulator returns a populated `TestResults` with
  build results and at least one test outcome.
- `Execute` throws `InvalidOperationException` with message "Build Failed" when the build step fails.
- `PrintSummary` writes per-test outcome lines and aggregate pass/fail counts to the output.
- `SaveResults` with a `.trx` extension creates a file containing valid TRX XML.
- `SaveResults` with a `.xml` extension creates a file containing valid JUnit XML.
- `SaveResults` with an unknown extension defaults to TRX format.
- `SaveResults` throws `ArgumentException` for null, empty, or whitespace-only file names.
- `SaveToTrx` creates a TRX file equivalent to `SaveResults` with a `.trx` extension.
- `PrintSummary` throws `ArgumentNullException` when `context` is null.
- `PrintSummary` with an empty test collection writes only separator lines and no count lines.
- `PrintSummary` with all-passing tests writes only the "Passed" count line (failed count suppressed).
- `PrintSummary` with all-failing tests writes only the "Failed" count line (passed count suppressed).

#### Test Scenarios

**Execute_WithMockSimulator_CollectsTestOutcomes**: Verifies that `Execute` with MockSimulator
returns a `TestResults` with populated `BuildResults` and `Tests` collection.
This scenario is tested by `TestResults_Execute_WithMockSimulator_CollectsTestOutcomes`.

**Execute_WithBuildFailure_ThrowsInvalidOperationException**: Verifies that `Execute` throws
`InvalidOperationException` with message "Build Failed" when the build step produces an error.
This scenario is tested by `TestResults_Execute_WithBuildFailure_ThrowsInvalidOperationException`.

**PrintSummary_WithMixedResults_WritesSummaryToOutput**: Verifies that `PrintSummary` writes
"Passed" and "Failed" indicators to the log output for a mixed result set.
This scenario is tested by `TestResults_PrintSummary_WithMixedResults_WritesSummaryToOutput`.

**SaveResults_WithTrxExtension_CreatesTrxFile**: Verifies that calling `SaveResults` with
a `.trx` file name creates the file and its content contains `"<?xml"` and the run name,
confirming TRX format output.
This scenario is tested by `TestResults_SaveResults_WithTrxExtension_CreatesTrxFile`.

**SaveResults_WithXmlExtension_CreatesJUnitFile**: Verifies that calling `SaveResults` with
a `.xml` file name creates a file containing valid JUnit XML with a `testsuites` root element.
This scenario is tested by `TestResults_SaveResults_WithXmlExtension_CreatesJUnitFile`.

**SaveResults_WithFailedTest_CreatesJUnitFileWithFailure**: Verifies that a failed test
result is serialized to JUnit XML containing `"failure"` and the error text.
This scenario is tested by `TestResults_SaveResults_WithFailedTest_CreatesJUnitFileWithFailure`.

**SaveToTrx_WithTestResults_CreatesTrxFile**: Verifies that the `SaveToTrx` backward
compatibility wrapper creates a valid TRX file.
This scenario is tested by `TestResults_SaveToTrx_WithTestResults_CreatesTrxFile`.

**SaveResults_WithNullFileName_ThrowsArgumentException**: Verifies that `SaveResults`
throws `ArgumentException` for null, empty, and whitespace-only file names.
This scenario is tested by `TestResults_SaveResults_WithNullFileName_ThrowsArgumentException`.

**SaveResults_WithUnknownExtension_CreatesTrxFile**: Verifies that an unrecognized file
extension defaults to TRX format output.
This scenario is tested by `TestResults_SaveResults_WithUnknownExtension_CreatesTrxFile`.

**PrintSummary_NullContext_ThrowsArgumentNullException**: Verifies that `PrintSummary` throws
`ArgumentNullException` when `context` is null.
This scenario is tested by `TestResults_PrintSummary_NullContext_ThrowsArgumentNullException`.

**PrintSummary_EmptyTests_WritesOnlySeparators**: Verifies that `PrintSummary` with an empty
test collection writes only separator lines and no count lines.
This scenario is tested by `TestResults_PrintSummary_EmptyTests_WritesOnlySeparators`.

**PrintSummary_AllPassTests_WritesPassedCountOnly**: Verifies that `PrintSummary` with an
all-passing result set writes only the "Passed" count line, suppressing the "Failed" count.
This scenario is tested by `TestResults_PrintSummary_AllPassTests_WritesPassedCountOnly`.

**PrintSummary_AllFailTests_WritesFailedCountOnly**: Verifies that `PrintSummary` with an
all-failing result set writes only the "Failed" count line, suppressing the "Passed" count.
This scenario is tested by `TestResults_PrintSummary_AllFailTests_WritesFailedCountOnly`.
