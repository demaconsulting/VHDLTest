### RunProcessor

#### Verification Approach

`RunProcessor` is verified through unit tests in
`test/DEMAConsulting.VHDLTest.Tests/Run/RunProcessorTests.cs`. Three test scenarios exercise
the `Execute(string, string, string[])` overload directly using the `dotnet` executable as
a controlled external process: one tests successful output with a matching info rule, one
tests error classification from a non-zero exit code, and one tests that a missing program
raises an exception. These tests exercise both the process execution path (via `RunProgram`)
and the output classification path (via `Parse`) together.

#### Test Environment

N/A - standard test environment. `dotnet` must be available on PATH.

#### Acceptance Criteria

- All unit tests in `RunProcessorTests.cs` pass with zero failures.
- `RunProcessor.Execute` classifies output lines using the provided `RunLineRule` patterns.
- A non-zero exit code from the process elevates the summary to `RunLineType.Error`.
- Executing an unknown program raises an exception that propagates to the caller.

#### Test Scenarios

**Execute_MissingProgram_ThrowsException**: Verifies that calling `Execute` with a program
name that cannot be located throws an exception, confirming that missing simulator executables
are surfaced as exceptions rather than silent failures.
This scenario is tested by `RunProcessor_Execute_MissingProgram_ThrowsException`.

**Execute_ProgramWithError_ReturnsErrorResult**: Verifies that executing `dotnet` with an
unknown command produces a non-zero exit code and a `RunResults.Summary` of
`RunLineType.Error`, confirming that non-zero exit codes elevate the summary.
This scenario is tested by `RunProcessor_Execute_ProgramWithError_ReturnsErrorResult`.

**Execute_ProgramWithSuccess_ReturnsInfoResult**: Verifies that executing `dotnet help`
and matching `"Usage"` lines against an info rule produces a `RunResults.Summary` of
`RunLineType.Info`, confirming that classification rules are applied to output lines.
This scenario is tested by `RunProcessor_Execute_ProgramWithSuccess_ReturnsInfoResult`.
