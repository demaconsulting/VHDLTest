## DemaConsulting.TestResults Verification

### Verification Approach

DemaConsulting.TestResults is verified through unit and integration tests in
`test/DEMAConsulting.VHDLTest.Tests/Results/TestResultsTests.cs`. The
`TestResults.SaveResults` method under test calls the DemaConsulting.TestResults library
to write TRX or JUnit XML files; passing tests confirm that the library correctly
serializes test result data into the appropriate format. The real library is exercised —
no mocking of the library is performed. Integration-level evidence is additionally
provided by `test/DEMAConsulting.VHDLTest.Tests/IntegrationTests.cs`, which exercises
the full pipeline including results file generation via the `--results` flag.

### Test Environment

Standard .NET test runner environment — no additional setup required. Tests run on
Ubuntu, Windows, and macOS in the CI matrix. The test creates an output file in a
temporary location and verifies its contents.

### Acceptance Criteria

The relevant `TestResultsTests` and `IntegrationTests` tests pass under `dotnet test` on
all supported platforms. A passing test run constitutes evidence that DemaConsulting.TestResults
correctly serialized test result data into both the TRX format and the JUnit XML format
for all tested scenarios.

### Test Scenarios

- **TRX file is created with correct content**: `TestResults_SaveResults_WithTrxExtension_CreatesTrxFile`
  calls `TestResults.SaveResults` with a `.trx` path and verifies that the output file exists
  and contains valid TRX XML, including the `TestRun` root element.
- **Integration pipeline writes TRX file via results flag**: `VHDLTest_ResultsFlag_WritesTrxFile`
  runs VHDLTest end-to-end with the `--results` option and verifies that the TRX file is created
  and contains the expected `<TestRun` element, confirming that the full pipeline from CLI to
  TRX serialization is functional.
- **JUnit XML file is created with correct content**: `TestResults_SaveResults_WithXmlExtension_CreatesJUnitFile`
  calls `TestResults.SaveResults` with a `.xml` path and verifies that the output file exists
  and contains valid JUnit XML, including the `testsuites` root element.
- **JUnit XML file records failed tests correctly**: `TestResults_SaveResults_WithFailedTest_CreatesJUnitFileWithFailure`
  calls `TestResults.SaveResults` with a failed test result and a `.xml` path, and verifies that
  the output JUnit XML file correctly records the failure, confirming that failure serialization
  is functional.
