## Cli

### Verification Approach

The Cli subsystem is verified through two complementary test strategies. Unit tests for each
class (`ContextTests.cs`, `ConfigDocumentTests.cs`, `OptionsTests.cs`) test the individual
units in isolation, exercising normal paths, boundary conditions, and error paths. A subsystem
integration test class (`CliSubsystemTests.cs`) exercises the full Cli pipeline — from raw
command-line arguments through `Context`, `ConfigDocument`, and `Options` — to verify that
the three units work together correctly. A real YAML configuration file is written to disk
and cleaned up after each test; no mocking is used.

### Test Environment

N/A - standard test environment.

### Acceptance Criteria

- All unit tests in `ContextTests.cs`, `ConfigDocumentTests.cs`, and `OptionsTests.cs` pass
  with zero failures.
- All integration tests in `CliSubsystemTests.cs` pass with zero failures.
- Error paths throw the expected exception types (`InvalidOperationException`,
  `FileNotFoundException`).

### Test Scenarios

**ParseArgsAndLoadConfig_WithValidConfig_ProducesCorrectOptions**: Verifies that the full
Cli pipeline, from `-c config.yaml --verbose` through `Context.Create` and `Options.Parse`,
produces an `Options` instance reflecting the file's contents and a `Context` with `Verbose`
set to true, confirming the three units integrate correctly.
This scenario is tested by `CliSubsystem_ParseArgsAndLoadConfig_WithValidConfig_ProducesCorrectOptions`.

**ParseArgsAndLoadConfig_WithMissingConfig_ThrowsFileNotFoundException**: Verifies that
specifying a non-existent configuration file through the Cli pipeline surfaces a
`FileNotFoundException` from `Options.Parse`, confirming the error propagates through the
subsystem boundary correctly.
This scenario is tested by `CliSubsystem_ParseArgsAndLoadConfig_WithMissingConfig_ThrowsFileNotFoundException`.
