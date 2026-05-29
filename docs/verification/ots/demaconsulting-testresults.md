## DemaConsulting.TestResults Verification

### Verification Approach

DemaConsulting.TestResults is verified through integration tests in
`test/DEMAConsulting.VHDLTest.Tests/Results/TestResultsTests.cs`. The
`TestResults.SaveResults` method under test calls the DemaConsulting.TestResults library
to write a TRX file; passing tests confirm that the library correctly serializes test
result data into the TRX format. The real library is exercised — no mocking of the
library is performed.

### Test Environment

Standard .NET test runner environment — no additional setup required. Tests run on
Ubuntu, Windows, and macOS in the CI matrix. The test creates a TRX output file in a
temporary location and verifies its contents.

### Acceptance Criteria

All `TestResultsTests` tests pass under `dotnet test` on all supported platforms. A
passing test run constitutes evidence that DemaConsulting.TestResults correctly serialized
test result data into the TRX format for all tested scenarios.

### Test Scenarios

- **Integration test with passing results produces zero exit code and valid TRX file**:
  `IntegrationTest_TestsPassed_ReturnsZeroExitCode` runs VHDLTest with the `--results`
  option and verifies that the TRX file is created and contains the expected test entries.
