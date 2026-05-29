### RunProgram

#### Verification Approach

`RunProgram` is a static utility class with no standalone unit tests. It is verified
indirectly through the `RunProcessor` tests, which call `RunProcessor.Execute` and thereby
invoke `RunProgram.Run` to launch external processes. The tests in `RunProcessorTests.cs`
and `RunSubsystemTests.cs` confirm that `RunProgram.Run` correctly launches the `dotnet`
executable, captures combined stdout and stderr output, and returns the exit code to
`RunProcessor`. Missing-executable behavior is also exercised through these tests.

#### Test Environment

N/A - standard test environment. `dotnet` must be available on PATH.

#### Acceptance Criteria

- `RunProgram.Run` is exercised by all `RunProcessor` and Run subsystem tests with zero
  failures.
- Combined stdout and stderr output is captured and returned without data loss.
- The exit code returned by `RunProgram.Run` matches the process exit code.
- A missing executable causes an exception to propagate to the caller.

#### Test Scenarios

**Execute_ProgramWithSuccess_ReturnsInfoResult** (via RunProcessor): Verifies that
`RunProgram.Run` launches the `dotnet help` command and returns non-empty combined output
with a zero exit code, confirming successful process execution and output capture.
This scenario is tested by `RunProcessor_Execute_ProgramWithSuccess_ReturnsInfoResult`.

**Execute_ProgramWithError_ReturnsErrorResult** (via RunProcessor): Verifies that
`RunProgram.Run` returns a non-zero exit code when the process exits with an error.
This scenario is tested by `RunProcessor_Execute_ProgramWithError_ReturnsErrorResult`.

**Execute_MissingProgram_ThrowsException** (via RunProcessor): Verifies that `RunProgram.Run`
propagates the process-start exception when the executable cannot be found.
This scenario is tested by `RunProcessor_Execute_MissingProgram_ThrowsException`.
