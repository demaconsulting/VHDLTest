### RunProcessor

#### Verification Approach

`RunProcessor` is verified through unit tests in
`test/DEMAConsulting.VHDLTest.Tests/Run/RunProcessorTests.cs`. Tests exercise the
`Execute(string, string, string[])` and `Execute(Context, ...)` overloads directly using
the `dotnet` executable as a controlled external process, and `Parse` is also exercised
in isolation with pre-built output strings. Tests cover: successful output classification
with a matching info rule; warning-pattern matching; no-matching-pattern fallback to text;
non-zero exit code elevation; verbose-mode logging of working directory and run command;
missing-program exception propagation; and Windows cmd-wrapping detection.

#### Test Environment

N/A - standard test environment. `dotnet` must be available on PATH.

#### Acceptance Criteria

- All unit tests in `RunProcessorTests.cs` pass with zero failures.
- `RunProcessor.Execute` classifies output lines using the provided `RunLineRule` patterns.
- A non-zero exit code from the process elevates the summary to `RunLineType.Error`.
- Executing an unknown program raises an exception that propagates to the caller.
- On Windows, `Execute(Context, ...)` throws `Win32Exception` for a missing program,
  consistent with the non-Windows path and the `Execute(string, ...)` overload, and the
  thrown exception's `NativeErrorCode` is set to `ERROR_FILE_NOT_FOUND` (2).
- On Windows, an application name that already carries its own extension (e.g. `tool.exe`)
  never resolves to an unrelated `PATHEXT`-qualified file (e.g. `tool.exe.cmd`).
- Mutating the caller's original rules array after construction does not affect a
  `RunProcessor` instance's classification behavior.
- Verbose logging writes the working directory and run command when `context.Verbose` is set.
- On Windows, the command is wrapped with `cmd /c` before being launched.

#### Test Scenarios

**Execute_MissingProgram_ThrowsException**: Verifies that calling `Execute` with a program
name that cannot be located throws an exception, confirming that missing simulator executables
are surfaced as exceptions rather than silent failures.
This scenario is tested by `RunProcessor_Execute_MissingProgram_ThrowsException`.

**Parse_OutputWithErrorPattern_ReturnsErrorClassification**: Verifies that parsing an output
string that matches an error-type rule with a zero exit code produces a `RunResults.Summary`
of `RunLineType.Error` and contains at least one line classified as `RunLineType.Error`,
confirming that the error pattern classification path operates independently of exit-code
escalation.
This scenario is tested by `RunProcessor_Parse_OutputWithErrorPattern_ReturnsErrorClassification`.

**Execute_ProgramWithError_ReturnsErrorResult**: Verifies that executing `dotnet` with an
unknown command produces a `RunResults.Summary` of `RunLineType.Error`, confirming that
error conditions are surfaced through the execute pipeline.
This scenario is tested by `RunProcessor_Execute_ProgramWithError_ReturnsErrorResult`.

**Execute_ProgramWithSuccess_ReturnsInfoResult**: Verifies that executing `dotnet help`
and matching `"Usage"` lines against an info rule produces a `RunResults.Summary` of
`RunLineType.Info`, confirming that classification rules are applied to output lines.
This scenario is tested by `RunProcessor_Execute_ProgramWithSuccess_ReturnsInfoResult`.

**Execute_ProgramWithNonZeroExitCode_ReturnsErrorResult**: Verifies that executing `dotnet`
with an argument that produces a non-zero exit code causes the `RunResults.Summary` to be
`RunLineType.Error` regardless of the output classification, confirming the exit-code
escalation path.
This scenario is tested by `RunProcessor_Execute_ProgramWithNonZeroExitCode_ReturnsErrorResult`.

**Parse_OutputWithWarningPattern_ReturnsWarningSummary**: Verifies that parsing an output
string that matches a warning-type rule produces a `RunResults.Summary` of
`RunLineType.Warning`, confirming that warning classification rules are applied correctly.
This scenario is tested by `RunProcessor_Parse_OutputWithWarningPattern_ReturnsWarningSummary`.

**Parse_OutputWithNoMatchingPattern_ReturnsTextClassification**: Verifies that parsing an
output string that matches no classification rule results in lines classified as
`RunLineType.Text` and a summary of `RunLineType.Text`, confirming the default classification
fallback.
This scenario is tested by `RunProcessor_Parse_OutputWithNoMatchingPattern_ReturnsTextClassification`.

**Parse_WithNonZeroExitCode_ElevatesSummaryToAtLeastError**: Verifies that calling `Parse`
directly with a non-zero exit code and output that would otherwise produce a non-error
summary results in a `RunResults.Summary` of at least `RunLineType.Error`, confirming
the `Parse`-level exit-code escalation logic.
This scenario is tested by `RunProcessor_Parse_WithNonZeroExitCode_ElevatesSummaryToAtLeastError`.

**Execute_WithContext_LogsRunInfo**: Verifies that calling the `Execute(Context, ...)` overload
with a verbose context writes both the working directory and the run command to the context
log, confirming that both `VHDLTest-Run-RunProcessor-VerboseLogging` and
`VHDLTest-Run-RunProcessor-VerboseLogging-Command` behaviors are exercised.
This scenario is tested by `RunProcessor_Execute_WithContext_LogsRunInfo`.

**Execute_WithContext_OnWindows_WrapsCommandWithCmdSlashC**: Verifies that on Windows,
the `Execute(Context, ...)` overload wraps the application command with `cmd /c` before
launching the process, confirming the Windows cmd-wrapping requirement. This test only
runs on Windows (`[SupportedOSPlatform("windows")]`).
This scenario is tested by `RunProcessor_Execute_WithContext_OnWindows_WrapsCommandWithCmdSlashC`.

**Execute_WithContext_MissingProgram_ThrowsWin32ExceptionConsistently**: Verifies that on
Windows, `Execute(Context, ...)` throws `Win32Exception` for a missing program, with
`NativeErrorCode` set to `ERROR_FILE_NOT_FOUND` (2) — proving the `Execute(Context, ...)`
overload's pre-flight resolution step now matches the already-verified missing-program
behavior of the direct `Execute(string, ...)` overload, closing the prior inconsistency
where `cmd /c` silently swallowed a missing program into a non-throwing,
non-zero-exit `RunResults`. This test only runs on Windows (`[SupportedOSPlatform("windows")]`).
This scenario is tested by
`RunProcessor_Execute_WithContext_MissingProgram_ThrowsWin32ExceptionConsistently`.

**Execute_WithContext_ExtensionQualifiedNameNotFound_DoesNotMatchDoubleExtensionFile**:
Verifies that requesting an already extension-qualified application name (e.g. `tool.exe`)
does not resolve to an unrelated `PATHEXT`-qualified file (e.g. `tool.exe.cmd`) present in
the working directory, confirming resolution matches `cmd.exe`'s own semantics for
extension-qualified names rather than over-appending further extensions. This test only
runs on Windows (`[SupportedOSPlatform("windows")]`).
This scenario is tested by
`RunProcessor_Execute_WithContext_ExtensionQualifiedNameNotFound_DoesNotMatchDoubleExtensionFile`.

**Execute_WithContext_ValidProgram_StillInvokesSuccessfully**: Verifies that a valid Windows
application (`dotnet`) is unaffected by the new pre-flight executable resolution step — a
regression guard proving the resolution logic does not break the existing working path. This
test only runs on Windows (`[SupportedOSPlatform("windows")]`).
This scenario is tested by `RunProcessor_Execute_WithContext_ValidProgram_StillInvokesSuccessfully`.

**Constructor_MutatingOriginalRulesArrayAfterConstruction_DoesNotAffectClassification**:
Verifies that mutating the caller's original `RunLineRule[]` array after constructing a
`RunProcessor` does not affect the instance's classification behavior, confirming the
constructor takes a defensive copy rather than capturing the caller's array reference.
This scenario is tested by
`RunProcessor_Constructor_MutatingOriginalRulesArrayAfterConstruction_DoesNotAffectClassification`.

**Constructor_NullRules_ThrowsArgumentNullException**: Verifies that constructing a
`RunProcessor` with a null `rules` array throws `ArgumentNullException` rather than a
less-clear exception surfacing later from the defensive-copy collection expression.
This scenario is tested by `RunProcessor_Constructor_NullRules_ThrowsArgumentNullException`.
