### TclText

#### Verification Approach

`TclText` is verified through unit tests in
`test/DEMAConsulting.VHDLTest.Tests/Simulators/TclTextTests.cs`. Each test calls
`TclText.Quote` with a value exercising a specific TCL metacharacter or the balanced-braces
fallback trigger, and asserts the exact quoted string returned. No process is invoked;
`Quote` is a pure function.

#### Test Environment

N/A - standard test environment.

#### Acceptance Criteria

- All unit tests in `TclTextTests.cs` pass with zero failures.
- `TclText.Quote` brace-quotes a value unchanged when it contains no literal `{` or `}`
  characters, regardless of spaces or other TCL metacharacters present.
- `TclText.Quote` falls back to a double-quoted form with every metacharacter (`\`, `"`,
  `$`, `[`, `]`, `{`, `}`) individually backslash-escaped when the value contains literal
  braces.

#### Test Scenarios

**Quote_PlainValue_ReturnsBracedLiteral**: Verifies that a plain value with no
metacharacters is wrapped unchanged in braces.
This scenario is tested by `TclText_Quote_PlainValue_ReturnsBracedLiteral`.

**Quote_ValueWithSpace_ReturnsBracedLiteral**: Verifies that a value containing a space is
brace-quoted verbatim, preserving the space.
This scenario is tested by `TclText_Quote_ValueWithSpace_ReturnsBracedLiteral`.

**Quote_ValueWithSquareBrackets_ReturnsBracedLiteral**: Verifies that a value containing a
square bracket is brace-quoted verbatim, confirming command substitution is suppressed by
braces.
This scenario is tested by `TclText_Quote_ValueWithSquareBrackets_ReturnsBracedLiteral`.

**Quote_ValueWithDollarSign_ReturnsBracedLiteral**: Verifies that a value containing a
dollar sign is brace-quoted verbatim, confirming variable substitution is suppressed by
braces.
This scenario is tested by `TclText_Quote_ValueWithDollarSign_ReturnsBracedLiteral`.

**Quote_ValueWithDoubleQuote_ReturnsBracedLiteral**: Verifies that a value containing a
double quote is brace-quoted verbatim.
This scenario is tested by `TclText_Quote_ValueWithDoubleQuote_ReturnsBracedLiteral`.

**Quote_ValueWithBackslash_ReturnsBracedLiteral**: Verifies that a value containing a
backslash is brace-quoted verbatim, confirming backslash substitution is suppressed by
braces.
This scenario is tested by `TclText_Quote_ValueWithBackslash_ReturnsBracedLiteral`.

**Quote_ValueWithBalancedBraces_ReturnsEscapedDoubleQuotedFallback**: Verifies that a value
containing balanced braces falls back to the double-quoted, backslash-escaped form, with
each metacharacter (including the braces themselves) individually escaped.
This scenario is tested by
`TclText_Quote_ValueWithBalancedBraces_ReturnsEscapedDoubleQuotedFallback`.

**Quote_ValueWithBracesAndOtherMetacharacters_EscapesAllMetacharacters**: Verifies that the
fallback path escapes every TCL metacharacter (backslash, double quote, dollar, square
brackets, and braces) when a brace forces the fallback, and escapes the backslash first so
escaping backslashes are not themselves re-escaped.
This scenario is tested by
`TclText_Quote_ValueWithBracesAndOtherMetacharacters_EscapesAllMetacharacters`.
