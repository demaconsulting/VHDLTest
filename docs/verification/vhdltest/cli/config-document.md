### ConfigDocument

#### Verification Approach

`ConfigDocument` is verified through unit tests in
`test/DEMAConsulting.VHDLTest.Tests/Cli/ConfigDocumentTests.cs`. Each test writes a
temporary YAML configuration file to disk, calls `ConfigDocument.ReadFile`, and verifies
the returned document's properties. Cleanup is performed in a `finally` block. The missing
file path is tested without creating any file.

#### Test Environment

N/A - standard test environment.

#### Acceptance Criteria

- All unit tests in `ConfigDocumentTests.cs` pass with zero failures.
- `ConfigDocument.ReadFile` deserializes `files` and `tests` arrays correctly.
- `FileNotFoundException` is thrown when the specified file does not exist.

#### Test Scenarios

**ReadFile_MissingFile_ThrowsFileNotFoundException**: Verifies that calling
`ConfigDocument.ReadFile` with a path that does not exist on disk throws
`FileNotFoundException`, ensuring missing configuration is surfaced as a typed error.
This scenario is tested by `ConfigDocument_ReadFile_MissingFile_ThrowsFileNotFoundException`.

**ReadFile_ValidFile_ReadsSuccessfully**: Verifies that a well-formed YAML configuration
file with two files and two tests is deserialized correctly into a `ConfigDocument` with
`Files` and `Tests` arrays of length 2 containing the expected values.
This scenario is tested by `ConfigDocument_ReadFile_ValidFile_ReadsSuccessfully`.
