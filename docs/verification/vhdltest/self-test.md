## SelfTest

### Verification Approach

The SelfTest subsystem contains a single unit, `Validation`, which is verified through
integration tests in `test/DEMAConsulting.VHDLTest.Tests/SelfTest/ValidationTests.cs`. All
tests run the VHDLTest executable end-to-end via the `Runner` helper with `--validate` and
`--simulator mock`, exercising the full validation pipeline — embedded file extraction,
in-process VHDLTest invocation, log capture, result reporting, and optional file output.

### Test Environment

The standard test environment is sufficient. The `MockSimulator` is used so no external VHDL
simulator installation is required.

### Acceptance Criteria

- All integration tests in `ValidationTests.cs` pass with zero failures.
- `--validate --simulator mock` exits with code 0 and output contains `"Validation Passed"`.
- `--depth` is honoured in the validation output heading.
- The OS Version field appears in the system information table.
- A results file is written when `--results` is specified and contains the expected test entries.

### Test Scenarios

**ValidateFlag_PerformsValidationAndReturnsSuccess**: Verifies that running VHDLTest with
`--validate --simulator mock` produces exit code 0 and output containing `"Validation Passed"`,
confirming the embedded VHDL files are correctly exercised by the mock simulator and the
overall self-validation pipeline functions end-to-end.
This scenario is tested by `IntegrationTest_ValidateFlag_PerformsValidationAndReturnsSuccess`.

**ValidateFlagWithDepth_PerformsValidationWithDepth**: Verifies that `--depth 3` causes the
validation output to contain `"### DEMAConsulting.VHDLTest"`, confirming the heading depth
setting is passed through and applied correctly.
This scenario is tested by `IntegrationTest_ValidateFlagWithDepth_PerformsValidationWithDepth`.

**ValidateFlagWithResultsFile_SavesValidationResults**: Verifies that `--results
validation_results.trx` causes a TRX file to be written containing both
`VHDLTest_TestPasses` and `VHDLTest_TestFails` test entries with 2 passed and 0 failed,
confirming the result serialization path of the validation subsystem works correctly.
This scenario is tested by `IntegrationTest_ValidateFlagWithResultsFile_SavesValidationResults`.

**ValidateFlag_IncludesOSVersionInReport**: Verifies that the validation report contains
the `"| OS Version"` row in the system information table, confirming environment metadata
is captured and reported correctly.
This scenario is tested by `IntegrationTest_ValidateFlag_IncludesOSVersionInReport`.
