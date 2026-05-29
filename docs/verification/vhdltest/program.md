## Program

### Verification Approach

`Program` is verified through unit tests in `test/DEMAConsulting.VHDLTest.Tests/ProgramTests.cs`.
The `Program.Run` method is called directly with a `Context` constructed from a controlled
argument array. No simulator is invoked in these unit tests; the test for the version flag and
help flag exercises only the dispatch logic. The test for no-config exercises the usage-print
and error-code path. No mocking framework is used; the `Context` class is the real implementation
configured through its own `Create` factory.

### Test Environment

N/A - standard test environment.

### Acceptance Criteria

- All unit tests in `ProgramTests.cs` pass with zero failures.
- `context.ExitCode` reflects 0 on version and help flag paths and non-zero on missing config.
- The usage text `"Usage: VHDLTest"` appears in the log output when no config is given.

### Test Scenarios

**VersionIsNotEmpty**: Verifies that `Program.Version` is a non-null, non-empty string,
confirming the assembly informational version attribute is populated at build time.
This scenario is tested by `Program_Version_IsNotEmpty`.

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
