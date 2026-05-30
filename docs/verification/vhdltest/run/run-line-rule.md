### RunLineRule

#### Verification Approach

`RunLineRule` is verified through unit tests in
`test/DEMAConsulting.VHDLTest.Tests/Run/RunLineRuleTests.cs` and indirectly through all
simulator processor tests that apply classification rules via `RunProcessor.Parse`. The
`RunLineRule.Create` factory is exercised by constructing processors with known patterns and
verifying that the correct `RunLineType` is assigned to matching output lines. Invalid
pattern strings are tested to confirm that `ArgumentException` is thrown.

#### Test Environment

N/A - standard test environment.

#### Acceptance Criteria

- All unit tests in `RunLineRuleTests.cs` pass with zero failures.
- `RunLineRule.Create` compiles valid pattern strings and returns a usable rule.
- Lines matching the pattern are classified with the assigned `RunLineType`.
- Lines not matching any pattern default to `RunLineType.Text`.

#### Test Scenarios

**Create_WithValidPattern_ClassifiesMatchingLine**: Verifies that a `RunLineRule` created
with a valid pattern correctly classifies lines that match the pattern as the specified
`RunLineType`, and leaves non-matching lines as `RunLineType.Text`.
This scenario is directly tested by `RunLineRule_Create_ValidPattern_ReturnsRuleWithMatchingProperties`,
which confirms the `Type` and compiled `Pattern` properties are set correctly and that the pattern
matches expected text. Additional indirect coverage is provided by all simulator processor tests
(e.g., `GhdlSimulator_CompileProcessor_WarningOutput_ReturnsWarningResult`).

**Create_WithErrorPattern_ClassifiesErrorLine**: Verifies that a rule with `RunLineType.Error`
correctly elevates the `RunResults.Summary` when a matching error line is present in the output.
This scenario is tested by `RunProcessor_Execute_ProgramWithError_ReturnsErrorResult`.

**Create_InvalidPattern_ThrowsArgumentException**: Verifies that `RunLineRule.Create` throws
`ArgumentException` when given a syntactically invalid regular expression string. This confirms
the validation guard in `Create` fires before any `Regex` object is constructed.
This scenario is tested by `RunLineRule_Create_InvalidPattern_ThrowsArgumentException`.

**Create_NullPattern_ThrowsArgumentNullException**: Verifies that `RunLineRule.Create` throws
`ArgumentNullException` when `pattern` is null, confirming the null guard is enforced before
any regex compilation is attempted.
This scenario is tested by `RunLineRule_Create_NullPattern_ThrowsArgumentNullException`.

**Execute_ProgramWithSuccess_ReturnsInfoResult**: Verifies that a rule with `RunLineType.Info`
correctly classifies matching lines and drives the `RunResults.Summary` to `Info` when
info-matching output is produced.
This scenario is tested by `RunProcessor_Execute_ProgramWithSuccess_ReturnsInfoResult`.

**Parse_OutputWithWarningPattern_ReturnsWarningSummary**: Verifies that a rule with
`RunLineType.Warning` correctly classifies lines matching the warning pattern and that the
summary reflects the highest-severity matched type.
This scenario is tested by `RunProcessor_Parse_OutputWithWarningPattern_ReturnsWarningSummary`.

**Parse_OutputWithNoMatchingPattern_ReturnsTextClassification**: Verifies that output lines
that do not match any rule in the rule set are classified as `RunLineType.Text`, confirming
the default fallback classification.
This scenario is tested by `RunProcessor_Parse_OutputWithNoMatchingPattern_ReturnsTextClassification`.
