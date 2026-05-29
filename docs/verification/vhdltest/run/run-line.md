### RunLine

#### Verification Approach

`RunLine` is an immutable positional record verified indirectly through `RunProcessor` and
simulator processor tests. The `Lines` collection of every `RunResults` instance is composed
of `RunLine` elements. All tests that assert on `results.Lines[i].Type` and
`results.Lines[i].Text` are verifying `RunLine` construction and content. No direct unit
tests exist for `RunLine` because it is a simple positional record with no logic beyond the
record-generated constructor.

#### Test Environment

N/A - standard test environment.

#### Acceptance Criteria

- `RunLine` instances created by `RunProcessor.Parse` carry the correct `Type` and `Text`
  values as confirmed by all simulator processor tests.
- Each line's `Text` is the unmodified original output string.
- Each line's `Type` is determined by the first matching `RunLineRule`, or `RunLineType.Text`
  if no rule matches.

#### Test Scenarios

**CompileProcessor_CleanOutput_Lines** (via simulator tests): Verifies that each line in the
`RunResults.Lines` collection for clean output carries `RunLineType.Text` and the original
unmodified text, confirming `RunLine` construction is correct.
This scenario is tested by (for example) `GhdlSimulator_CompileProcessor_CleanOutput_ReturnsTextResult`.

**CompileProcessor_ErrorOutput_Lines** (via simulator tests): Verifies that a classified
error line has `Type == RunLineType.Error` and `Text` matching the original output line.
This scenario is tested by (for example) `GhdlSimulator_CompileProcessor_ErrorOutput_ReturnsErrorResult`.
