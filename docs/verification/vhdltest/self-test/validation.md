### Validation

#### Verification Approach

`Validation` is verified through integration tests in
`test/DEMAConsulting.VHDLTest.Tests/SelfTest/ValidationTests.cs`. Each test runs the
VHDLTest executable end-to-end using the `Runner` helper with `--validate --simulator mock`,
exercising the complete `Validation.Run` method path including embedded file extraction, the
in-process VHDLTest invocation, log capture, system-information table output, result
reporting, and optional TRX file serialization. The `MockSimulator` is used to make
tests deterministic without requiring a real VHDL simulator.

Unit-level tests follow the `{ClassName}_{MethodUnderTest}_{Scenario}_{ExpectedBehavior}`
naming convention and are linked from the unit requirements. Because `Validation.Run` invokes
`Program.Run` in-process and the class is `internal`, direct method-call isolation is not
practical; the unit tests therefore exercise `Validation.Run` end-to-end via `Runner` with
the same assertions as the subsystem-level tests.

#### Test Environment

The standard test environment is sufficient. The `MockSimulator` eliminates the need for
any external VHDL simulator installation.

#### Acceptance Criteria

- All integration tests in `ValidationTests.cs` pass with zero failures.
- `Validation.Run` exits with code 0 when validation passes.
- Both `VHDLTest_TestPasses` and `VHDLTest_TestFails` scenarios are reported in the output.
- The system information table includes OS Version and .NET runtime fields.
- When `--results` is specified, a TRX file is written with correct test names and counters.
- The heading depth is configurable via `--depth`.

#### Test Scenarios

**ValidateFlag_PerformsValidationAndReturnsSuccess**: Verifies that `Validation.Run`
completes with exit code 0 and writes `"Validation Passed"` to the output, confirming
the embedded VHDL test files are correctly extracted, executed by the mock simulator,
and their pass/fail outcomes are correctly evaluated.
This scenario is tested by `Validation_Run_MockSimulator_ReturnsSuccess`.

**ValidateFlagWithDepth_PerformsValidationWithDepth**: Verifies that the `--depth 3` flag
causes the validation system information heading to use depth-3 Markdown headers
(`"### DEMAConsulting.VHDLTest"`), confirming the depth parameter is passed through
`Context` and applied to the output.
This scenario is tested by `Validation_Run_DepthParameter_RendersDepthHeadings`.

**ValidateFlagWithResultsFile_SavesValidationResults**: Verifies that specifying a
`--results <file>` path causes `Validation.Run` to write a TRX file containing
entries for `VHDLTest_TestPasses` and `VHDLTest_TestFails` with a counter of 2 passed and
0 failed, confirming the results collection and serialization path works correctly.
This scenario is tested by `Validation_Run_ResultsFile_SavesResults`.

**ValidateFlag_IncludesOSVersionInReport**: Verifies that the validation report contains the
`"| OS Version"` and `"| DotNet Runtime"` rows in the system information Markdown table,
confirming that environment metadata collection is functional and included in the validation
output.
This scenario is tested by `Validation_Run_MockSimulator_IncludesOSVersionInReport`.

**ValidateFlag_InvalidSimulator_ReturnsFailure**: Verifies that `Validation.Run` exits with a
non-zero code and produces a descriptive error message when given an unrecognized simulator
name, confirming that the tool fails gracefully rather than silently crashing.
This scenario is tested by `Validation_Run_InvalidSimulator_ReturnsFailure`.
