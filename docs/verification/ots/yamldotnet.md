## YamlDotNet Verification

### Verification Approach

YamlDotNet is verified through integration tests in
`test/DEMAConsulting.VHDLTest.Tests/Cli/ConfigDocumentTests.cs`. The
`ConfigDocument.ReadFile` method under test calls the YamlDotNet deserializer directly;
passing tests confirm that YamlDotNet correctly deserializes YAML configuration files
into `ConfigDocument` instances. No mocking of YamlDotNet is performed — the real
library is exercised in all test scenarios.

### Test Environment

Standard .NET test runner environment — no additional setup required. Tests run on
Ubuntu, Windows, and macOS in the CI matrix.

### Acceptance Criteria

All `ConfigDocumentTests` tests pass under `dotnet test` on all supported platforms.
A passing test run constitutes evidence that YamlDotNet deserialized YAML content
correctly for all tested input scenarios.

### Test Scenarios

- **Valid configuration file is parsed successfully**: `Options_Parse_ValidConfigFile_ParsesSuccessfully`
  verifies that a well-formed YAML configuration file is deserialized into a `ConfigDocument`
  with all expected fields populated.
- **Verbose flag is parsed correctly**: `Options_Parse_WithVerboseFlag_ParsesSuccessfully`
  verifies that the optional `verbose` field is deserialized and mapped to the `Options` record.
- **Custom test entry is parsed correctly**: `Options_Parse_WithCustomTest_ParsesSuccessfully`
  verifies that custom test entries in the YAML are deserialized into the expected collection.
