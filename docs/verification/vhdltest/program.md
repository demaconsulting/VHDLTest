## Program

### Verification Approach

`Program` is verified through unit tests in `test/DEMAConsulting.VHDLTest.Tests/ProgramTests.cs`.
The `Program.Run` method is called directly with a `Context` constructed from a controlled
argument array. Early-dispatch tests (version flag and help flag) exercise only the dispatch
logic and do not invoke a simulator. The no-config test exercises the usage-print and error-code
path. Tests for validation dispatch, run-tests, failure-exit-code, exit-zero, and save-results
use a mock `ISimulator` implementation that returns controlled `RunResults` without launching
any external process. No external mocking framework is used; the `Context` class and the mock
simulator are the real implementations configured through their own factories and interfaces.

### Test Environment

N/A - standard test environment.

### Acceptance Criteria

- All unit tests in `ProgramTests.cs` pass with zero failures.
- `context.ExitCode` reflects 0 on version, help, and successful run paths.
- `context.ExitCode` reflects non-zero on missing config and failing test paths.
- `context.ExitCode` reflects 0 when `--exit-zero` is specified even if tests fail.
- The usage text `"Usage: VHDLTest"` appears in the log output when no config is given.
- The `--validate` flag dispatches to the validation path without running tests.
- The `--results` flag produces a results file on disk.

### Test Scenarios

**VersionIsNotEmpty**: Verifies that `Program.Version` is a non-null, non-empty string,
confirming the assembly informational version attribute is populated at build time.
This scenario is tested by `Program_Version_WhenAssemblyBuilt_IsNotEmpty`.

**Run_WithVersionFlag_ReturnsZeroExitCode**: Verifies that calling `Program.Run` with
`--version --silent` results in `context.ExitCode == 0`, confirming the version dispatch
path terminates cleanly.
This scenario is tested by `Program_Run_WithVersionFlag_ReturnsZeroExitCode`.

**Run_WithHelpFlag_ReturnsZeroExitCode**: Verifies that calling `Program.Run` with
`--help --silent` results in `context.ExitCode == 0`, confirming the help dispatch path
terminates cleanly.
This scenario is tested by `Program_Run_WithHelpFlag_ReturnsZeroExitCode`.

**Run_WithNoConfigFileArgument_DisplaysUsage**: Verifies that calling `Program.Run`
without a config file argument writes `"Usage: VHDLTest"` to the log output, confirming
usage guidance is provided when the configuration argument is missing.
This scenario is tested by `Program_Run_WithNoConfigFileArgument_DisplaysUsage`.

**Run_WithNoConfig_ReturnsNonZeroExitCode**: Verifies that calling `Program.Run` without
any config file argument results in a non-zero `context.ExitCode`, confirming the error
path sets the exit code correctly.
This scenario is tested by `Program_Run_WithNoConfig_ReturnsNonZeroExitCode`.

**Run_WithVersionFlag_DisplaysVersion**: Verifies that calling `Program.Run` with
`--version --silent` writes the version string to the log output, confirming the version
dispatch path produces visible output.
This scenario is tested by `Program_Run_WithVersionFlag_DisplaysVersion`.

**Run_WithHelpFlag_DisplaysHelp**: Verifies that calling `Program.Run` with `--help --silent`
writes help text to the log output, confirming the help dispatch path produces visible output.
This scenario is tested by `Program_Run_WithHelpFlag_DisplaysHelp`.

**Run_WithValidateFlag_DispatchesToValidation**: Verifies that calling `Program.Run`
without a config file argument and with the `--validate` flag completes with
`context.ExitCode == 0`, confirming the validation dispatch path is exercised.
This scenario is tested by `Program_Run_WithValidateFlag_DispatchesToValidation`.

**Run_RunTests_WithMockSimulator_RunsSuccessfully**: Verifies that calling `Program.Run`
with a valid config and a mock simulator that returns a passing result completes with
`context.ExitCode == 0`, confirming the run-tests path is exercised end-to-end.
This scenario is tested by `Program_Run_RunTests_WithMockSimulator_RunsSuccessfully`.

**Run_WithInvalidSimulator_WritesErrorAndExits**: Verifies that calling `Program.Run` with
a config that references an unknown simulator identifier writes an error message and returns
a non-zero `context.ExitCode`, confirming the invalid-simulator error path is handled.
This scenario is tested by `Program_Run_WithInvalidSimulator_WritesErrorAndExits`.

**Run_FailureExitCode_WithFailingTests_ReturnsNonZeroExitCode**: Verifies that calling
`Program.Run` with a mock simulator that returns failing tests results in a non-zero
`context.ExitCode`, confirming the failure-exit-code path is exercised.
This scenario is tested by `Program_Run_FailureExitCode_WithFailingTests_ReturnsNonZeroExitCode`.

**Run_ExitZero_WithFailingTestsAndExitZeroFlag_ReturnsZeroExitCode**: Verifies that calling
`Program.Run` with a mock simulator that returns failing tests and the `--exit-zero` flag
results in `context.ExitCode == 0`, confirming the exit-zero flag overrides failure exit code.
This scenario is tested by `Program_Run_ExitZero_WithFailingTestsAndExitZeroFlag_ReturnsZeroExitCode`.

**Run_SaveResults_WithResultsFile_CreatesResultsFile**: Verifies that calling `Program.Run`
with a mock simulator and a `--results` path creates the specified results file on disk,
confirming the save-results path is exercised.
This scenario is tested by `Program_Run_SaveResults_WithResultsFile_CreatesResultsFile`.

**Run_WithTestFilter_RunsOnlyMatchingTests**: Verifies that calling `Program.Run` with a
configuration file containing multiple tests and a test name filter argument runs only the
named test, confirming that `context.CustomTests` overrides the full test list from the
configuration file.
This scenario is tested by `Program_Run_WithTestFilter_RunsOnlyMatchingTests`.
