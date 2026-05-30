### MockSimulator

#### Verification Approach

`MockSimulator` is verified through unit tests in
`test/DEMAConsulting.VHDLTest.Tests/Simulators/MockSimulatorTests.cs`. The compile and test
`RunProcessor` instances are exercised by calling `Parse` with pre-captured output strings
covering text, info, warning, failure, and error classifications. Availability is checked to
confirm `MockSimulator.Available()` returns false (it has no real executable path). The mock
simulator is also exercised end-to-end through system integration tests and the self-validation
path that uses it as the simulator backend.

#### Test Environment

N/A - standard test environment. `MockSimulator` requires no external executable.

#### Acceptance Criteria

- All unit tests in `MockSimulatorTests.cs` pass with zero failures.
- `MockSimulator.Instance.SimulatorName` returns `"Mock"`.
- `MockSimulator.Instance.Available()` returns false (no path is set).
- The compile processor correctly classifies text, info, warning, and error output.
- The test processor correctly classifies text, info, warning, failure, and error output.
- `Compile()` produces error, warning, info, or clean results based on filename patterns.
- `Test()` produces error, fail, warning, info, or clean results based on test name patterns.

#### Test Scenarios

**SimulatorName_ReturnsMock**: Verifies that `MockSimulator.Instance.SimulatorName` is
`"Mock"`, confirming the instance is registered with the correct name.
This scenario is tested by `MockSimulator_SimulatorName_ReturnsMock`.

**Available_WithNullPath_ReturnsFalse**: Verifies that `MockSimulator.Instance.Available()`
returns false, confirming the mock has no real executable path and will not be auto-selected
when a real simulator is requested.
This scenario is tested by `MockSimulator_Available_WithNullPath_ReturnsFalse`.

**CompileProcessor_CleanOutput_ReturnsTextResult**: Verifies that clean output through the
mock compile processor produces a `RunLineType.Text` summary.
This scenario is tested by `MockSimulator_CompileProcessor_CleanOutput_ReturnsTextResult`.

**CompileProcessor_InfoOutput_ReturnsInfoResult**: Verifies that a line prefixed `"Info:"`
is classified as `RunLineType.Info` by the mock compile processor.
This scenario is tested by `MockSimulator_CompileProcessor_InfoOutput_ReturnsInfoResult`.

**CompileProcessor_WarningOutput_ReturnsWarningResult**: Verifies that a line prefixed
`"Warning:"` is classified as `RunLineType.Warning`.
This scenario is tested by `MockSimulator_CompileProcessor_WarningOutput_ReturnsWarningResult`.

**CompileProcessor_ErrorOutput_ReturnsErrorResult**: Verifies that a line prefixed `"Error:"`
is classified as `RunLineType.Error`.
This scenario is tested by `MockSimulator_CompileProcessor_ErrorOutput_ReturnsErrorResult`.

**TestProcessor_CleanOutput_ReturnsTextResult**: Verifies that clean output through the mock
test processor produces a `RunLineType.Text` summary.
This scenario is tested by `MockSimulator_TestProcessor_CleanOutput_ReturnsTextResult`.

**TestProcessor_InfoOutput_ReturnsInfoResult**: Verifies that a line prefixed `"Info:"` is
classified as `RunLineType.Info` by the mock test processor.
This scenario is tested by `MockSimulator_TestProcessor_InfoOutput_ReturnsInfoResult`.

**TestProcessor_WarningOutput_ReturnsWarningResult**: Verifies that a line prefixed
`"Warning:"` is classified as `RunLineType.Warning`.
This scenario is tested by `MockSimulator_TestProcessor_WarningOutput_ReturnsWarningResult`.

**TestProcessor_FailureOutput_ReturnsErrorResult**: Verifies that a line prefixed
`"Failure:"` is classified as `RunLineType.Error`, confirming VHDL assertion failure lines
are treated as errors.
This scenario is tested by `MockSimulator_TestProcessor_FailureOutput_ReturnsErrorResult`.

**TestProcessor_ErrorOutput_ReturnsErrorResult**: Verifies that a line prefixed `"Error:"`
is classified as `RunLineType.Error`.
This scenario is tested by `MockSimulator_TestProcessor_ErrorOutput_ReturnsErrorResult`.

**Compile_WithErrorFile_ReturnsErrorResult**: Verifies that `Compile()` returns an error
result when a source file name contains `_error_`.
This scenario is tested by `MockSimulator_Compile_WithErrorFile_ReturnsErrorResult`.

**Compile_WithWarningFile_ReturnsWarningResult**: Verifies that `Compile()` returns a
warning result when a source file name contains `_warning_`.
This scenario is tested by `MockSimulator_Compile_WithWarningFile_ReturnsWarningResult`.

**Compile_WithInfoFile_ReturnsInfoResult**: Verifies that `Compile()` returns an info result
when a source file name contains `_info_`.
This scenario is tested by `MockSimulator_Compile_WithInfoFile_ReturnsInfoResult`.

**Compile_WithCleanFile_ReturnsSuccessResult**: Verifies that `Compile()` returns a clean
text result when a source file name contains no special pattern.
This scenario is tested by `MockSimulator_Compile_WithCleanFile_ReturnsSuccessResult`.

**Test_WithErrorPattern_ReturnsErrorResult**: Verifies that `Test()` returns an error result
when the test bench name contains `_error_`.
This scenario is tested by `MockSimulator_Test_WithErrorPattern_ReturnsErrorResult`.

**Test_WithFailPattern_ReturnsFailResult**: Verifies that `Test()` returns an error result
(Failure maps to Error) when the test bench name contains `_fail_`.
This scenario is tested by `MockSimulator_Test_WithFailPattern_ReturnsErrorResult`.

**Test_WithWarningPattern_ReturnsWarningResult**: Verifies that `Test()` returns a warning
result when the test bench name contains `_warning_`.
This scenario is tested by `MockSimulator_Test_WithWarningPattern_ReturnsWarningResult`.

**Test_WithInfoPattern_ReturnsInfoResult**: Verifies that `Test()` returns an info result
when the test bench name contains `_info_`.
This scenario is tested by `MockSimulator_Test_WithInfoPattern_ReturnsInfoResult`.

**Test_WithCleanName_ReturnsSuccessResult**: Verifies that `Test()` returns a clean text
result when the test bench name contains no special pattern.
This scenario is tested by `MockSimulator_Test_WithCleanName_ReturnsSuccessResult`.
