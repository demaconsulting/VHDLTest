### RunLineType

#### Verification Approach

`RunLineType` is an enumeration with a direct unit test suite in `RunLineTypeTests.cs` that
explicitly verifies the ordinal severity ordering. The four ordinal comparison tests are the
primary verification source for the ordering invariant on which `RunProcessor.Parse` relies.
In addition, `RunLineType` is verified indirectly through all tests that assert on
`RunResults.Summary` and `RunLine.Type` values. Every simulator processor test and
`RunProcessor` test asserts against specific `RunLineType` values.

#### Test Environment

N/A - standard test environment. `RunLineType` is an enum with no runtime dependencies.

#### Acceptance Criteria

- `RunLineType` ordinal ordering `Text < Info < Warning < Error` is confirmed by the four
  direct ordinal tests in `RunLineTypeTests.cs`.
- `RunLineType` values are correctly used in all processor and results tests with zero failures.
- The ordinal ordering is correctly exploited by `RunProcessor.Parse` to select the
  highest-severity summary type.
- A non-zero exit code forces the summary to at least `RunLineType.Error`.

#### Test Scenarios

**Ordinal_Text_IsLessThan_Info**: Verifies that `RunLineType.Text` has a lower ordinal value
than `RunLineType.Info`, confirming `Text` is the lowest severity in the ordering.
This scenario is tested by `RunLineType_Ordinal_Text_IsLessThan_Info`.

**Ordinal_Info_IsLessThan_Warning**: Verifies that `RunLineType.Info` has a lower ordinal value
than `RunLineType.Warning`, confirming the Info–Warning ordering step.
This scenario is tested by `RunLineType_Ordinal_Info_IsLessThan_Warning`.

**Ordinal_Warning_IsLessThan_Error**: Verifies that `RunLineType.Warning` has a lower ordinal
value than `RunLineType.Error`, confirming the Warning–Error boundary used by pass/fail logic.
This scenario is tested by `RunLineType_Ordinal_Warning_IsLessThan_Error`.

**Ordinal_Error_IsGreaterThan_Warning**: Verifies that `RunLineType.Error` has the highest
ordinal value relative to `Warning`, confirming `Error` is the highest severity.
This scenario is tested by `RunLineType_Ordinal_Error_IsGreaterThan_Warning`.

**SeverityOrdering_TextIsLowest**: Verifies that clean output with no diagnostic lines
produces `Summary == RunLineType.Text`, confirming `Text` is the lowest severity and does
not elevate the summary.
This scenario is tested by (for example) `GhdlSimulator_CompileProcessor_CleanOutput_ReturnsTextResult`.

**SeverityOrdering_ErrorIsHighest**: Verifies that an error line elevates the summary to
`RunLineType.Error` even when other lower-severity lines are also present in the output.
This scenario is tested by (for example) `GhdlSimulator_CompileProcessor_ErrorOutput_ReturnsErrorResult`.

**NonZeroExitCode_ForcesErrorSummary**: Verifies that a non-zero exit code elevates the
`RunResults.Summary` to `RunLineType.Error` regardless of the line content, confirming
the exit-code threshold in `RunProcessor.Parse`. The primary isolation evidence for this
scenario is `RunProcessor_Parse_WithNonZeroExitCode_ElevatesSummaryToAtLeastError`, which
calls `RunProcessor.Parse` directly with a non-zero exit code and Info-only rules and
asserts Summary >= Error. Supplementary integration evidence is provided by
`RunProcessor_Execute_ProgramWithError_ReturnsErrorResult`.
