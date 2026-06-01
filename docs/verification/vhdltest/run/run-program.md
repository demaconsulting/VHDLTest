### RunProgram

#### Verification Approach

`RunProgram` is verified through five direct unit tests in `RunProgramTests.cs`. These tests
call `RunProgram.Run` directly using the `dotnet` executable as the controlled external program,
verifying that the method correctly launches the process, captures combined stdout and stderr,
returns the correct exit code, and propagates exceptions when the executable is not found.

No mocking is used; `dotnet` is used as the controlled external program because it is always
available in any .NET SDK environment.

#### Test Environment

N/A - standard test environment. `dotnet` must be available on PATH (it is in any .NET
SDK environment).

#### Acceptance Criteria

- `RunProgram.Run` launches the specified program and returns its exit code with zero failures.
- Combined stdout and stderr output is captured and returned without data loss.
- The exit code returned by `RunProgram.Run` matches the process exit code.
- An empty working directory causes the launched process to run in the caller's current working directory.
- A missing executable causes an exception to propagate to the caller.

#### Test Scenarios

**Run_ValidProgram_ReturnsZeroExitCodeAndNonEmptyOutput**: Verifies that `RunProgram.Run`
launches the `dotnet help` command, returns exit code 0, and produces non-empty combined
output, confirming successful process execution and output capture.
This scenario is tested by
`RunProgram_Run_ValidProgram_ReturnsZeroExitCodeAndNonEmptyOutput`.

**Run_ProgramWithNonZeroExit_ReturnsNonZeroExitCode**: Verifies that `RunProgram.Run`
returns the non-zero exit code when the launched program exits with an error, confirming
that the exit code is faithfully propagated to the caller.
This scenario is tested by
`RunProgram_Run_ProgramWithNonZeroExit_ReturnsNonZeroExitCode`.

**Run_ProgramWithStderr_CapturesBothStreamsInOutput**: Verifies that `RunProgram.Run`
captures output written to stderr and includes it in the combined output string returned
to the caller, confirming that neither output stream is discarded.
This scenario is tested by
`RunProgram_Run_ProgramWithStderr_CapturesBothStreamsInOutput`.

**Run_EmptyWorkingDirectory_UsesCallerCurrentDirectory**: Verifies that `RunProgram.Run`
launches the process in the caller's current working directory when an empty string is
supplied as the working directory.
This scenario is tested by `RunProgram_Run_EmptyWorkingDirectory_UsesCallerCurrentDirectory`.

**Run_MissingExecutable_ThrowsException**: Verifies that `RunProgram.Run` propagates an
exception when passed a path to a non-existent executable, confirming that launch failures
are not silently swallowed.
This scenario is tested by `RunProgram_Run_MissingExecutable_ThrowsException`.
