### XilinxArgText

#### Verification Approach

`XilinxArgText` is verified through unit tests in
`test/DEMAConsulting.VHDLTest.Tests/Simulators/XilinxArgTextTests.cs`. Each test calls
`XilinxArgText.Quote` with a value exercising a specific special character (space, embedded
double quote, embedded backslash, or both) and asserts the exact quoted string returned. No
process is invoked; `Quote` is a pure function.

#### Test Environment

N/A - standard test environment.

#### Acceptance Criteria

- All unit tests in `XilinxArgTextTests.cs` pass with zero failures.
- `XilinxArgText.Quote` always wraps the value in double quotes, regardless of whether the value
  contains whitespace or special characters.
- `XilinxArgText.Quote` backslash-escapes embedded `\` and `"` characters, escaping the backslash
  first so escaping backslashes are not themselves re-escaped.
- `XilinxArgText.Quote` throws `ArgumentNullException` when `value` is null.

#### Test Scenarios

**Quote_PlainValue_ReturnsQuotedLiteral**: Verifies that a plain value with no special characters
is wrapped unchanged in double quotes.
This scenario is tested by `XilinxArgText_Quote_PlainValue_ReturnsQuotedLiteral`.

**Quote_ValueWithSpace_ReturnsQuotedLiteral**: Verifies that a value containing a space is
double-quoted verbatim, preserving the space.
This scenario is tested by `XilinxArgText_Quote_ValueWithSpace_ReturnsQuotedLiteral`.

**Quote_ValueWithDoubleQuote_EscapesDoubleQuote**: Verifies that a value containing an embedded
double quote is escaped with a backslash.
This scenario is tested by `XilinxArgText_Quote_ValueWithDoubleQuote_EscapesDoubleQuote`.

**Quote_ValueWithBackslash_EscapesBackslash**: Verifies that a value containing an embedded
backslash is escaped with a backslash.
This scenario is tested by `XilinxArgText_Quote_ValueWithBackslash_EscapesBackslash`.

**Quote_ValueWithBackslashAndDoubleQuote_EscapesBoth**: Verifies that a value containing both an
embedded backslash and an embedded double quote escapes both characters, escaping the backslash
first so the escaping backslashes themselves are not re-escaped.
This scenario is tested by `XilinxArgText_Quote_ValueWithBackslashAndDoubleQuote_EscapesBoth`.

**Quote_NullValue_ThrowsArgumentNullException**: Verifies that passing a null value throws
`ArgumentNullException` rather than a less-clear `NullReferenceException` from iterating the
value.
This scenario is tested by `XilinxArgText_Quote_NullValue_ThrowsArgumentNullException`.
