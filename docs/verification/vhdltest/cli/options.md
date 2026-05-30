### Options

#### Verification Approach

`Options` is verified through unit tests in
`test/DEMAConsulting.VHDLTest.Tests/Cli/OptionsTests.cs`. Each test constructs a
`Context` from a controlled argument array and calls `Options.Parse`. Tests that require a
configuration file write a temporary YAML file to disk and clean it up in a `finally` block.
Error-path tests exercise the no-config and missing-file conditions.

#### Test Environment

N/A - standard test environment.

#### Acceptance Criteria

- All unit tests in `OptionsTests.cs` pass with zero failures.
- `Options.Parse` correctly populates `Config.Files` and `Config.Tests` from the YAML file.
- `InvalidOperationException` is thrown when no config file path is provided.
- `FileNotFoundException` is thrown when the config file path does not exist.

#### Test Scenarios

**Parse_NoConfigProvided_ThrowsInvalidOperationException**: Verifies that calling
`Options.Parse` with a context that has no `ConfigFile` set throws
`InvalidOperationException`, ensuring the absence of a config path is caught before any
file I/O is attempted.
This scenario is tested by `Options_Parse_NoConfigProvided_ThrowsInvalidOperationException`.

**Parse_MissingConfigFile_ThrowsFileNotFoundException**: Verifies that specifying a
non-existent config file path results in `FileNotFoundException` propagating from
`ConfigDocument.ReadFile` through `Options.Parse`.
This scenario is tested by `Options_Parse_MissingConfigFile_ThrowsFileNotFoundException`.

**Parse_ValidConfigFile_ParsesSuccessfully**: Verifies that a context with a valid config
file path results in an `Options` instance whose `Config.Files` and `Config.Tests` arrays
match the YAML content.
This scenario is tested by `Options_Parse_ValidConfigFile_ParsesSuccessfully`.

**Parse_WithVerboseFlag_ParsesSuccessfully**: Verifies that adding `--verbose` to the
context does not prevent `Options.Parse` from completing successfully.
This scenario is tested by `Options_Parse_WithVerboseFlag_ParsesSuccessfully`.

**Parse_WithCustomTest_ParsesSuccessfully**: Verifies that adding a positional custom test
argument to the context does not prevent `Options.Parse` from completing successfully.
This scenario is tested by `Options_Parse_WithCustomTest_ParsesSuccessfully`.

**Parse_NullArgs_ThrowsArgumentNullException**: Verifies that calling
`Options.Parse` with a null context argument throws `ArgumentNullException`,
confirming that the null guard is present before any context property is accessed.
This scenario is tested by `Options_Parse_NullArgs_ThrowsArgumentNullException`.

**ResolveWorkingDirectory_RootPath_ThrowsInvalidOperationException**: Verifies that calling
`Options.ResolveWorkingDirectory` with a root path (such as `/` or `C:\`) throws
`InvalidOperationException`, confirming that the defensive null guard for
`Path.GetDirectoryName` is in place.
This scenario is tested by `Options_ResolveWorkingDirectory_RootPath_ThrowsInvalidOperationException`.
