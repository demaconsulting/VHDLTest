### RunLine

#### Verification Approach

`RunLine` is an immutable positional record with a direct unit test in `RunLineTests.cs` that
exercises its constructor. In addition, `RunLine` is verified indirectly through `RunProcessor`
and simulator processor tests: the `Lines` collection of every `RunResults` instance is composed
of `RunLine` elements, and all tests that assert on `results.Lines[i].Type` and
`results.Lines[i].Text` are verifying `RunLine` construction and content.

#### Test Environment

N/A - standard test environment.

#### Acceptance Criteria

- `RunLine_Constructor_SetsTypeAndTextProperties` passes, confirming that the `Type` and `Text`
  properties are correctly set at construction.
- `RunLine` instances created by `RunProcessor.Parse` carry the correct `Type` and `Text`
  values as confirmed by all simulator processor tests.
- Each line's `Text` is the unmodified original output string.
- Each line's `Type` is determined by the first matching `RunLineRule`, or `RunLineType.Text`
  if no rule matches.

#### Test Scenarios

**Constructor_SetsTypeAndTextProperties**: Verifies that constructing a `RunLine` with a given
`RunLineType` and text string correctly sets both the `Type` and `Text` properties, confirming
that the positional record constructor assigns all fields as expected.
This scenario is tested by `RunLine_Constructor_SetsTypeAndTextProperties`.

**CompileProcessor_CleanOutput_Lines** (via simulator tests): Verifies that each line in the
`RunResults.Lines` collection for clean output carries `RunLineType.Text` and the original
unmodified text, confirming `RunLine` construction is correct.
This scenario is tested by (for example) `GhdlSimulator_CompileProcessor_CleanOutput_ReturnsTextResult`.

**CompileProcessor_ErrorOutput_Lines** (via simulator tests): Verifies that a classified
error line has `Type == RunLineType.Error` and `Text` matching the original output line.
This scenario is tested by (for example) `GhdlSimulator_CompileProcessor_ErrorOutput_ReturnsErrorResult`.
