### TestResults

#### Verification Approach

`TestResults` is verified through unit tests in
`test/DEMAConsulting.VHDLTest.Tests/Results/TestResultsTests.cs`. Tests exercise
`SaveResults` for TRX format (`.trx` extension), JUnit XML format (`.xml` extension),
unknown extension defaulting to TRX, and `ArgumentException` for null, empty, and
whitespace-only file names. The `SaveToTrx` backward-compatibility wrapper is also tested.
Each test that writes a file cleans up in a `finally` block. `RunResults` instances are
constructed from in-memory data.

#### Test Environment

N/A - standard test environment.

#### Acceptance Criteria

- All unit tests in `TestResultsTests.cs` pass with zero failures.
- `SaveResults` with a `.trx` extension creates a file containing valid TRX XML.
- `SaveResults` with a `.xml` extension creates a file containing valid JUnit XML.
- `SaveResults` with an unknown extension defaults to TRX format.
- `SaveResults` throws `ArgumentException` for null, empty, or whitespace-only file names.
- `SaveToTrx` creates a TRX file equivalent to `SaveResults` with a `.trx` extension.

#### Test Scenarios

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

**SaveResults_WithUnknownExtension_CreatesTrxFile**: Verifies that an unrecognised file
extension defaults to TRX format output.
This scenario is tested by `TestResults_SaveResults_WithUnknownExtension_CreatesTrxFile`.
