## Run

### Verification Approach

The Run subsystem is verified through unit tests and a subsystem integration test. Unit tests
in `RunProcessorTests.cs` exercise `RunProcessor.Execute` directly against a real external
process (`dotnet`) to verify that clean output and error exit codes are correctly classified,
and that attempting to execute a missing program throws an exception. A subsystem integration
test in `RunSubsystemTests.cs` exercises `RunProcessor`, `RunProgram`, and `RunResults`
working together, verifying that a real process is executed, output is classified, and the
`RunResults` record is correctly populated including timing information.

No mocking is used for these tests; the `dotnet` tool is used as the controlled external
program to exercise the process execution pipeline.

### Test Environment

N/A - standard test environment. `dotnet` must be available on PATH (it is in any .NET
SDK environment).

### Acceptance Criteria

- All unit tests in `RunProcessorTests.cs` pass with zero failures.
- All integration tests in `RunSubsystemTests.cs` pass with zero failures.
- `RunProcessor.Execute` correctly classifies output using the supplied `RunLineRule` patterns.
- A non-zero exit code forces the `RunResults.Summary` to at least `RunLineType.Error`.
- Attempting to execute a missing program throws an exception.

### Test Scenarios

**ExecuteRealProgram_WithClassificationRules_ProducesClassifiedRunResults**: Verifies that
`RunProcessor`, `RunProgram`, and `RunResults` work together to execute a real program,
produce classified output lines, record a non-negative duration, and return a zero exit code.
This scenario is tested by
`RunSubsystem_ExecuteRealProgram_WithClassificationRules_ProducesClassifiedRunResults`.

**ExecuteRealProgram_WithErrorExitCode_ProducesErrorRunResults**: Verifies that a non-zero
exit code from the external process results in `RunResults.Summary` of `RunLineType.Error`,
confirming the error threshold logic in `RunProcessor.Parse`.
This scenario is tested by
`RunSubsystem_ExecuteRealProgram_WithErrorExitCode_ProducesErrorRunResults`.

**Execute_WithContext_LogsWorkingDirectoryAndCommandToContext**: Verifies that the
`Execute(Context, string, string, string[])` overload logs the working directory and run
command to the context before delegating to the process execution path. This scenario is
tested by `RunSubsystem_Execute_WithContext_LogsWorkingDirectoryAndCommandToContext`.

**ExecuteNoRules_FallsBackToTextClassification**: Verifies that when no classification rules
are configured, every output line produced by the external program is classified as
`RunLineType.Text`, confirming the Text-fallback path defined by `VHDLTest-Run-Classify`.
This scenario is tested by `RunSubsystem_ExecuteNoRules_FallsBackToTextClassification`.
