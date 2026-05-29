### TestResult

#### Verification Approach

`TestResult` is verified through unit tests in
`test/DEMAConsulting.VHDLTest.Tests/Results/TestResultTests.cs`. Each test constructs a
`TestResult` with a `RunResults` instance built from in-memory data and asserts on the
`Passed`, `Failed`, `ClassName`, `TestName`, and `RunResults.Summary` properties. No
external I/O or simulator invocation is required.

#### Test Environment

N/A - standard test environment.

#### Acceptance Criteria

- All unit tests in `TestResultTests.cs` pass with zero failures.
- `Passed` is true when `RunResults.Summary < RunLineType.Error`.
- `Failed` is true when `RunResults.Summary >= RunLineType.Error`.
- `ClassName` and `TestName` match the values supplied at construction.

#### Test Scenarios

**Constructor_WithInfoResult_CreatesPassedTest**: Verifies that constructing a `TestResult`
with a `RunResults` whose `Summary` is `RunLineType.Info` results in `Passed == true` and
`Failed == false`, confirming the pass threshold is correctly applied.
This scenario is tested by `TestResult_Constructor_WithInfoResult_CreatesPassedTest`.

**Constructor_WithErrorResult_CreatesFailedTest**: Verifies that constructing a `TestResult`
with a `RunResults` whose `Summary` is `RunLineType.Error` results in `Passed == false` and
`Failed == true`, confirming the failure threshold is correctly applied.
This scenario is tested by `TestResult_Constructor_WithErrorResult_CreatesFailedTest`.
