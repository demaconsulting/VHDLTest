### TestResult

#### Verification Approach

`TestResult` is verified through unit tests in
`test/DEMAConsulting.VHDLTest.Tests/Results/TestResultTests.cs`. Each test constructs a
`TestResult` with a `RunResults` instance built from in-memory data and asserts on the
`Passed`, `Failed`, `ClassName`, `TestName`, and `RunResults.Summary` properties. The
`PrintSummary` tests use a Context with a temp log file to capture and assert on the
written output, including pass/fail label, test name, and duration. No external I/O or
simulator invocation is required.

#### Test Environment

N/A - standard test environment.

#### Acceptance Criteria

- All unit tests in `TestResultTests.cs` pass with zero failures.
- `Passed` is true when `RunResults.Summary < RunLineType.Error`.
- `Failed` is true when `RunResults.Summary >= RunLineType.Error`.
- `Passed` is true when `RunResults.Summary == RunLineType.Warning` (boundary: Warning < Error).
- `ClassName` and `TestName` match the values supplied at construction.
- `PrintSummary` writes "Passed" for passing tests and "Failed" for failing tests to the log.
- `PrintSummary` includes the test name and the execution duration in the log output.
- Each `TestResult` instance has a unique `TestId` and `ExecutionId`.

#### Test Scenarios

**Constructor_WithInfoResult_CreatesPassedTest**: Verifies that constructing a `TestResult`
with a `RunResults` whose `Summary` is `RunLineType.Info` results in `Passed == true` and
`Failed == false`, confirming the pass threshold is correctly applied.
This scenario is tested by `TestResult_Constructor_WithInfoResult_CreatesPassedTest`.

**Constructor_WithErrorResult_CreatesFailedTest**: Verifies that constructing a `TestResult`
with a `RunResults` whose `Summary` is `RunLineType.Error` results in `Passed == false` and
`Failed == true`, confirming the failure threshold is correctly applied.
This scenario is tested by `TestResult_Constructor_WithErrorResult_CreatesFailedTest`.

**Constructor_WithWarningResult_CreatesPassedTest**: Verifies the boundary case that
`RunLineType.Warning` (ordinal 2) is below the `RunLineType.Error` threshold, so a
Warning-level result is considered passed (`Passed == true`).
This scenario is tested by `TestResult_Constructor_WithWarningResult_CreatesPassedTest`.

**PrintSummary_PassedResult_WritesPassLine**: Verifies that `PrintSummary` writes "Passed",
the test name, and the duration to the log output for a passing test result.
This scenario is tested by `TestResult_PrintSummary_PassedResult_WritesPassLine`.

**PrintSummary_FailedResult_WritesFailLine**: Verifies that `PrintSummary` writes "Failed",
the test name, and the duration to the log output for a failing test result.
This scenario is tested by `TestResult_PrintSummary_FailedResult_WritesFailLine`.

**Constructor_CreatesUniqueTestAndExecutionIds**: Verifies that each `TestResult` construction
produces distinct `TestId` and `ExecutionId` values, confirming fresh GUID initialization.
This scenario is tested by `TestResult_Constructor_CreatesUniqueTestAndExecutionIds`.
