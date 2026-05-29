### Context

#### Verification Approach

`Context` is verified through unit tests in `test/DEMAConsulting.VHDLTest.Tests/Cli/ContextTests.cs`.
The `Context.Create` static factory is called directly with controlled argument arrays. No mocking
or external I/O is required; all assertions target the properties of the returned `Context` instance.
Error paths verify that `InvalidOperationException` is thrown for unknown flags and missing values.

#### Test Environment

N/A - standard test environment.

#### Acceptance Criteria

- All unit tests in `ContextTests.cs` pass with zero failures.
- All recognised flags are correctly parsed into typed properties.
- `InvalidOperationException` is thrown for unknown flags and missing value arguments.

#### Test Scenarios

**Create_NoArguments_ReturnsDefaultContext**: Verifies that `Context.Create([])` returns a
context with all option properties at their default values (null paths, false flags, null custom
tests), confirming the default state is correct.
This scenario is tested by `Context_Create_NoArguments_ReturnsDefaultContext`.

**Create_UnknownArgument_ThrowsInvalidOperationException**: Verifies that an unrecognised option
flag causes `Context.Create` to throw `InvalidOperationException`, ensuring unknown flags do not
silently pass through.
This scenario is tested by `Context_Create_UnknownArgument_ThrowsInvalidOperationException`.

**Create_WithConfigFile_SetsConfigFile**: Verifies that `-c config.json` sets `ConfigFile` to
`"config.json"` and leaves all other properties at their defaults.
This scenario is tested by `Context_Create_WithConfigFile_SetsConfigFile`.

**Create_MissingConfigValue_ThrowsInvalidOperationException**: Verifies that `-c` without a
following value throws `InvalidOperationException`, preventing silent null configuration.
This scenario is tested by `Context_Create_MissingConfigValue_ThrowsInvalidOperationException`.

**Create_WithResultsFile_SetsResultsFile**: Verifies that `-r results.trx` sets `ResultsFile`
to `"results.trx"` and leaves all other properties at their defaults.
This scenario is tested by `Context_Create_WithResultsFile_SetsResultsFile`.

**Create_MissingResultsValue_ThrowsInvalidOperationException**: Verifies that `-r` without a
following value throws `InvalidOperationException`.
This scenario is tested by `Context_Create_MissingResultsValue_ThrowsInvalidOperationException`.

**Create_WithSimulator_SetsSimulator**: Verifies that `-s GHDL` sets `Simulator` to `"GHDL"`
and leaves all other properties at their defaults.
This scenario is tested by `Context_Create_WithSimulator_SetsSimulator`.

**Create_MissingSimulatorValue_ThrowsInvalidOperationException**: Verifies that `-s` without a
following value throws `InvalidOperationException`.
This scenario is tested by `Context_Create_MissingSimulatorValue_ThrowsInvalidOperationException`.

**Create_WithVerbose_SetsVerboseFlag**: Verifies that `--verbose` sets `Verbose` to true.
This scenario is tested by `Context_Create_WithVerbose_SetsVerboseFlag`.

**Create_WithExitZero_SetsExitZeroFlag**: Verifies that `--exit-0` sets `ExitZero` to true.
This scenario is tested by `Context_Create_WithExitZero_SetsExitZeroFlag`.

**Create_WithValidate_SetsValidateFlag**: Verifies that `--validate` sets `Validate` to true.
This scenario is tested by `Context_Create_WithValidate_SetsValidateFlag`.

**Create_WithDepth_SetsDepth**: Verifies that `--depth 2` sets `Depth` to 2.
This scenario is tested by `Context_Create_WithDepth_SetsDepth`.

**Create_WithInvalidDepth_ThrowsInvalidOperationException**: Verifies that a non-integer
`--depth` value throws `InvalidOperationException`.
This scenario is tested by `Context_Create_WithInvalidDepth_ThrowsInvalidOperationException`.

**Create_WithZeroDepth_ThrowsInvalidOperationException**: Verifies that `--depth 0` throws
`InvalidOperationException` because depth must be at least 1.
This scenario is tested by `Context_Create_WithZeroDepth_ThrowsInvalidOperationException`.

**Create_WithNegativeDepth_ThrowsInvalidOperationException**: Verifies that `--depth -1` throws
`InvalidOperationException` because depth must be positive.
This scenario is tested by `Context_Create_WithNegativeDepth_ThrowsInvalidOperationException`.

**Create_WithCustomTest_SetsCustomTest**: Verifies that a single positional argument is
collected into `CustomTests` as a single-element list.
This scenario is tested by `Context_Create_WithCustomTest_SetsCustomTest`.

**Create_WithCustomTests_SetsCustomTests**: Verifies that multiple positional arguments after
`--` are collected into `CustomTests` as a multi-element list.
This scenario is tested by `Context_Create_WithCustomTests_SetsCustomTests`.
