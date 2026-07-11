### ActiveHdlSimulator

#### Verification Approach

`ActiveHdlSimulator` is verified through unit tests in
`test/DEMAConsulting.VHDLTest.Tests/Simulators/ActiveHdlSimulatorTests.cs`. The compile
and test `RunProcessor` instances are exercised by calling `Parse` with pre-captured output
strings, covering clean output, warnings, and errors without requiring Active-HDL to be
installed. Active-HDL integration with real VHDL source files is verified by CI jobs in
environments with Active-HDL installed, using the self-validation path
(`--validate --simulator activehdl`).

#### Test Environment

Automated unit tests: standard test environment, no Active-HDL installation required.
Live simulator integration: CI environment with Active-HDL installed on PATH.

#### Acceptance Criteria

- All unit tests in `ActiveHdlSimulatorTests.cs` pass with zero failures.
- `ActiveHdlSimulator.Instance.SimulatorName` returns `"ActiveHdl"`.
- The compile processor correctly classifies clean, warning, and error output patterns.
- The test processor correctly classifies clean, info, warning, error, and Lattice Edition
  suppression patterns.
- Calling `Compile()` or `Test()` when the simulator is not installed throws `InvalidOperationException`.
- `FindPath()` returns the value of `VHDLTEST_ACTIVEHDL_PATH` when that environment variable is set.

#### Test Scenarios

**SimulatorName_ReturnsActiveHdl**: Verifies that `ActiveHdlSimulator.Instance.SimulatorName`
is `"ActiveHdl"`, confirming the instance is registered with the correct name for factory
lookup.
This scenario is tested by `ActiveHdlSimulator_SimulatorName_ReturnsActiveHdl` in `ActiveHdlSimulatorTests.cs`.

**CompileProcessor_CleanOutput_ReturnsTextResult**: Verifies that clean Active-HDL
compilation output produces a `RunLineType.Text` summary.
This scenario is tested by `ActiveHdlSimulator_CompileProcessor_CleanOutput_ReturnsTextResult` in `ActiveHdlSimulatorTests.cs`.

**CompileProcessor_WarningOutput_ReturnsWarningResult**: Verifies that an Active-HDL warning
line (`KERNEL: Warning:`) is classified as `RunLineType.Warning`.
This scenario is tested by `ActiveHdlSimulator_CompileProcessor_WarningOutput_ReturnsWarningResult` in `ActiveHdlSimulatorTests.cs`.

**CompileProcessor_ErrorOutput_ReturnsErrorResult**: Verifies that an Active-HDL error line
is classified as `RunLineType.Error`.
This scenario is tested by `ActiveHdlSimulator_CompileProcessor_ErrorOutput_ReturnsErrorResult` in `ActiveHdlSimulatorTests.cs`.

**CompileProcessor_FatalRuntimeError_ReturnsErrorResult**: Verifies that a `RUNTIME: Fatal Error`
line in compile output is classified as `RunLineType.Error`.
This scenario is tested by `ActiveHdlSimulator_CompileProcessor_FatalRuntimeError_ReturnsErrorResult`
in `ActiveHdlSimulatorTests.cs`.

**TestProcessor_CleanOutput_ReturnsTextResult**: Verifies that clean Active-HDL simulation
output produces a `RunLineType.Text` summary.
This scenario is tested by `ActiveHdlSimulator_TestProcessor_CleanOutput_ReturnsTextResult` in `ActiveHdlSimulatorTests.cs`.

**TestProcessor_InfoOutput_ReturnsInfoResult**: Verifies that an `EXECUTION:: NOTE` line
is classified as `RunLineType.Info`.
This scenario is tested by `ActiveHdlSimulator_TestProcessor_InfoOutput_ReturnsInfoResult`
in `ActiveHdlSimulatorTests.cs`.

**TestProcessor_WarningOutput_ReturnsWarningResult**: Verifies that an `EXECUTION:: WARNING`
line is classified as `RunLineType.Warning`.
This scenario is tested by `ActiveHdlSimulator_TestProcessor_WarningOutput_ReturnsWarningResult` in `ActiveHdlSimulatorTests.cs`.

**TestProcessor_ErrorOutput_ReturnsErrorResult**: Verifies that an `EXECUTION:: ERROR` line
is classified as `RunLineType.Error`.
This scenario is tested by `ActiveHdlSimulator_TestProcessor_ErrorOutput_ReturnsErrorResult` in `ActiveHdlSimulatorTests.cs`.

**TestProcessor_LatticeSuppression1_ReturnsTextResult**: Verifies that the first Active-HDL
Lattice Edition advisory (`KERNEL: Warning: You are using the Active-HDL Lattice Edition`)
is suppressed and classified as `RunLineType.Text` rather than `RunLineType.Warning`.
This scenario is tested by `ActiveHdlSimulator_TestProcessor_LatticeSuppression1_ReturnsTextResult`
in `ActiveHdlSimulatorTests.cs`.

**TestProcessor_LatticeSuppression2_ReturnsTextResult**: Verifies that the second Active-HDL
Lattice Edition advisory (`KERNEL: Warning: Contact Aldec for available upgrade options`)
is suppressed and classified as `RunLineType.Text` rather than `RunLineType.Warning`.
This scenario is tested by `ActiveHdlSimulator_TestProcessor_LatticeSuppression2_ReturnsTextResult`
in `ActiveHdlSimulatorTests.cs`.

**TestProcessor_KernelWarning_ReturnsWarningResult**: Verifies that a `KERNEL: Warning:` line
that is not an Aldec advisory is classified as `RunLineType.Warning`.
This scenario is tested by `ActiveHdlSimulator_TestProcessor_KernelWarning_ReturnsWarningResult`
in `ActiveHdlSimulatorTests.cs`.

**TestProcessor_KernelWarningUpper_ReturnsWarningResult**: Verifies that a `KERNEL: WARNING:`
(uppercase) line is classified as `RunLineType.Warning`.
This scenario is tested by `ActiveHdlSimulator_TestProcessor_KernelWarningUpper_ReturnsWarningResult`
in `ActiveHdlSimulatorTests.cs`.

**TestProcessor_ExecutionFailure_ReturnsErrorResult**: Verifies that an `EXECUTION:: FAILURE`
line is classified as `RunLineType.Error`.
This scenario is tested by `ActiveHdlSimulator_TestProcessor_ExecutionFailure_ReturnsErrorResult`
in `ActiveHdlSimulatorTests.cs`.

**TestProcessor_KernelError_ReturnsErrorResult**: Verifies that a `KERNEL: ERROR` line is
classified as `RunLineType.Error`.
This scenario is tested by `ActiveHdlSimulator_TestProcessor_KernelError_ReturnsErrorResult`
in `ActiveHdlSimulatorTests.cs`.

**TestProcessor_RuntimeFatalError_ReturnsErrorResult**: Verifies that a `RUNTIME: Fatal Error:`
line is classified as `RunLineType.Error`.
This scenario is tested by `ActiveHdlSimulator_TestProcessor_RuntimeFatalError_ReturnsErrorResult`
in `ActiveHdlSimulatorTests.cs`.

**TestProcessor_VsimError_ReturnsErrorResult**: Verifies that a `VSIM: Error:` line in simulation
output is classified as `RunLineType.Error`.
This scenario is tested by `ActiveHdlSimulator_TestProcessor_VsimError_ReturnsErrorResult`
in `ActiveHdlSimulatorTests.cs`.

**Test_WithCleanConfig_AppendsTclExitCode**: Verifies that the TCL test script written by
`ActiveHdlSimulator.Test()` contains `exit -code 0` to signal successful simulation
completion to vsimsa. Uses `CreateForTesting` with a `FakeProcessInvoker` to avoid
launching a real process; the test captures the written do-script content via a temp directory.
This scenario is tested by `ActiveHdlSimulator_Test_WithCleanConfig_AppendsTclExitCode`
in `ActiveHdlSimulatorTests.cs`.

**Compile_WithValidConfig_InvokesVsimsaWithDoScript**: Verifies that `ActiveHdlSimulator.Compile()`
invokes the `vsimsa` executable (directly or via `cmd /c` on Windows) with the `-do` argument
and the path to the generated do-script, confirming the correct process invocation.
Uses `CreateForTesting` with a `FakeProcessInvoker`.
This scenario is tested by `ActiveHdlSimulator_Compile_WithValidConfig_InvokesVsimsaWithDoScript`
in `ActiveHdlSimulatorTests.cs`.

**FindPath_WithEnvVar_ReturnsEnvVarValue**: Verifies that `ActiveHdlSimulator.FindPath()`
returns the value of the `VHDLTEST_ACTIVEHDL_PATH` environment variable when it is set,
confirming the environment variable override path works correctly.
This scenario is tested by `ActiveHdlSimulator_FindPath_WithEnvVar_ReturnsEnvVarValue`
in `ActiveHdlSimulatorTests.cs`.

**FindPath_WithoutEnvVar_ReturnsNullOrPath**: Verifies that `ActiveHdlSimulator.FindPath()`
does not throw when `VHDLTEST_ACTIVEHDL_PATH` is not set, and returns either null (Active-HDL
not installed) or a non-empty string (Active-HDL found on PATH), confirming the PATH-based
discovery path handles both the installed and not-installed cases without error.
This scenario is tested by `ActiveHdlSimulator_FindPath_WithoutEnvVar_ReturnsNullOrPath`
in `ActiveHdlSimulatorTests.cs`.

**Compile_SimulatorNotAvailable_ThrowsInvalidOperationException**: Verifies that `Compile()`
throws `InvalidOperationException` with message containing `"ActiveHDL Simulator not available"`
when `SimulatorPath` is null. This test is skipped in environments where Active-HDL is installed.
This scenario is tested by `ActiveHdlSimulator_Compile_SimulatorNotAvailable_ThrowsInvalidOperationException`
in `ActiveHdlSimulatorTests.cs`.

**Test_SimulatorNotAvailable_ThrowsInvalidOperationException**: Verifies that `Test()`
throws `InvalidOperationException` with message containing `"ActiveHDL Simulator not available"`
when `SimulatorPath` is null. This test is skipped in environments where Active-HDL is installed.
This scenario is tested by `ActiveHdlSimulator_Test_SimulatorNotAvailable_ThrowsInvalidOperationException`
in `ActiveHdlSimulatorTests.cs`.

**Compile_WithFileNameContainingSpaceAndMetacharacter_QuotesFileNameInScript**: Verifies that a
file path containing a space and a TCL metacharacter (`my file [1].vhd`) round-trips correctly
through `Compile()`: it appears verbatim inside the brace-quoted form produced by
`TclText.Quote` in the generated `compile.do`, and the invocation's captured arguments still
resolve correctly, proving the quoting does not break the surrounding invocation.
This scenario is tested by
`ActiveHdlSimulator_Compile_WithFileNameContainingSpaceAndMetacharacter_QuotesFileNameInScript`
in `ActiveHdlSimulatorTests.cs`.

**Test_WithTestNameContainingSpaceAndMetacharacter_QuotesTestNameInScript**: Verifies that a
test bench name containing a space and a TCL metacharacter (`lib.my tb`) round-trips correctly
through `Test()`: it appears verbatim inside the brace-quoted form produced by `TclText.Quote`
in the generated `test.do`, and the invocation's captured arguments still resolve correctly.
This scenario is tested by
`ActiveHdlSimulator_Test_WithTestNameContainingSpaceAndMetacharacter_QuotesTestNameInScript`
in `ActiveHdlSimulatorTests.cs`.
