# VHDLTest

VHDLTest is a .NET command-line tool that accepts command-line arguments, loads a YAML
configuration file, invokes a VHDL simulator, processes the simulation output, and reports
test pass/fail results.

## Verification Approach

VHDLTest is verified through a layered test strategy using the xUnit framework in
`test/DEMAConsulting.VHDLTest.Tests/`. Three test layers are applied:

- **Unit tests**: each unit is tested in isolation with any external dependencies
  replaced by the `MockSimulator` or by in-memory data. Unit test classes mirror the
  source structure (e.g., `ProgramTests.cs`, `Cli/ContextTests.cs`).
- **Subsystem integration tests**: each subsystem has a dedicated integration test class
  (e.g., `CliSubsystemTests.cs`, `SimulatorsSubsystemTests.cs`) that exercises the
  subsystem's units working together.
- **System integration tests**: `IntegrationTests.cs` runs the VHDLTest executable
  end-to-end via the `Runner` helper and verifies exit codes and console output for the
  complete CLI-to-results pipeline using the `mock` simulator.

The `MockSimulator` is used in all automated tests to avoid a dependency on any
installed VHDL simulator tool.

## Test Environment

Tests are executed with `dotnet test` on Linux and Windows, targeting .NET 8, .NET 9, and
.NET 10. No external VHDL simulator installation is required for automated tests; only the
.NET SDK is needed. CI runs on GitHub Actions.

## Acceptance Criteria

- All automated tests pass with zero failures.
- All subsystem integration tests pass with zero failures.
- All system integration tests pass with zero failures.
- Code coverage meets project thresholds as reported by the CI pipeline.

## Test Scenarios

**NoArguments_DisplaysUsageAndReturnsError**: Verifies that invoking the tool with no
arguments produces usage guidance on stdout and returns a non-zero exit code, ensuring
operators receive actionable feedback on incorrect invocation.
This scenario is tested by `IntegrationTest_NoArguments_DisplaysUsageAndReturnsError`.

**HelpShortFlag_DisplaysUsageAndReturnsSuccess**: Verifies that the `-h` short flag
displays usage information and returns exit code 0, confirming the help path is
accessible from the shortest option form.
This scenario is tested by `IntegrationTest_HelpShortFlag_DisplaysUsageAndReturnsSuccess`.

**HelpQuestionFlag_DisplaysUsageAndReturnsSuccess**: Verifies that the `-?` flag
displays usage information and returns exit code 0.
This scenario is tested by `IntegrationTest_HelpQuestionFlag_DisplaysUsageAndReturnsSuccess`.

**HelpLongFlag_DisplaysUsageAndReturnsSuccess**: Verifies that `--help` displays
usage information and returns exit code 0.
This scenario is tested by `IntegrationTest_HelpLongFlag_DisplaysUsageAndReturnsSuccess`.

**VersionShortFlag_DisplaysVersionAndReturnsSuccess**: Verifies that `-v` writes a
version string matching the semantic version pattern and returns exit code 0.
This scenario is tested by `IntegrationTest_VersionShortFlag_DisplaysVersionAndReturnsSuccess`.

**VersionLongFlag_DisplaysVersionAndReturnsSuccess**: Verifies that `--version` writes
a version string matching the semantic version pattern and returns exit code 0.
This scenario is tested by `IntegrationTest_VersionLongFlag_DisplaysVersionAndReturnsSuccess`.

**CompileError_ReturnsNonZeroExitCode**: Verifies that a configuration containing a
filename triggering a compile error in the mock simulator results in a non-zero exit
code, confirming compilation failures are surfaced correctly.
This scenario is tested by `IntegrationTest_CompileError_ReturnsNonZeroExitCode`.

**TestExecutionError_ReturnsNonZeroExitCode**: Verifies that a test bench name
triggering an error in the mock simulator results in a non-zero exit code.
This scenario is tested by `IntegrationTest_TestExecutionError_ReturnsNonZeroExitCode`.

**TestExecutionErrorWithExit0_ReturnsZeroExitCode**: Verifies that `--exit-0`
suppresses the non-zero exit code from test failures, enabling CI pipelines to collect
results without marking the build as failed.
This scenario is tested by `IntegrationTest_TestExecutionErrorWithExit0_ReturnsZeroExitCode`.

**TestsPassed_ReturnsZeroExitCode**: Verifies that a passing test configuration using
the mock simulator returns exit code 0, confirming the success path works end-to-end.
This scenario is tested by `IntegrationTest_TestsPassed_ReturnsZeroExitCode`.
