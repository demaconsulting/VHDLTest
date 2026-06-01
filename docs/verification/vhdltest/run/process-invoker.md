### ProcessInvoker

#### Verification Approach

`IProcessInvoker` and `ProcessInvoker` are verified indirectly through the simulator unit tests
that inject `FakeProcessInvoker` via `CreateForTesting`. The default `ProcessInvoker.Instance`
singleton is exercised by the existing `RunProcessor` tests that launch `dotnet` as the
controlled external program. No dedicated `ProcessInvoker` unit tests are required because the
singleton is a thin one-line delegation to `RunProgram.Run`, which is independently verified in
`RunProgramTests.cs`.

`FakeProcessInvoker` is verified implicitly: any simulator test that asserts on the captured
`AllCalls` list confirms that the fake correctly records invocations.

#### Test Environment

N/A — standard test environment. No simulator installation required for
`FakeProcessInvoker`-based tests.

#### Acceptance Criteria

- `IProcessInvoker.Execute` contract: returns `(int ExitCode, string Output)` tuple.
- `ProcessInvoker.Instance` is non-null and callable.
- `FakeProcessInvoker` records every `Execute` call in `AllCalls` with correct
  `WorkingDirectory`, `Application`, and `Arguments`.
- `FakeProcessInvoker.ExitCodeToReturn` and `OutputToReturn` control the return values
  for all recorded calls.

#### Test Scenarios

**ProcessInvoker_Instance_DelegatesTo_RunProgram**: The `ProcessInvoker.Instance` singleton is
exercised by `RunProcessor` tests that call `Execute` with the `dotnet` executable.
This confirms that the default implementation delegates to `RunProgram.Run` and returns the
correct exit code and output.
This scenario is covered indirectly by `RunProcessor_Execute_ProgramWithSuccess_ReturnsInfoResult`
and `RunProcessor_Execute_ProgramWithError_ReturnsErrorResult`.

**FakeProcessInvoker_RecordsAllCalls**: Each simulator `CreateForTesting`-based test asserts that
`FakeProcessInvoker.AllCalls` contains at least one entry after a `Compile()` or `Test()` call.
This confirms the fake correctly captures every invocation.
This scenario is covered indirectly by all simulator `Compile_WithValidConfig_*` and
`Test_WithValidConfig_*` tests.
