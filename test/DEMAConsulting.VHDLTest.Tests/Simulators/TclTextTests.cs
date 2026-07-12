// Copyright (c) 2023 DEMA Consulting
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using DEMAConsulting.VHDLTest.Simulators;

namespace DEMAConsulting.VHDLTest.Tests.Simulators;

/// <summary>
///     Unit tests for the <see cref="TclText"/> helper class, covering the brace-quoting
///     common path and the backslash-escaping fallback path.
/// </summary>
/// <remarks>
///     Unit under test: <see cref="TclText.Quote"/>.
/// </remarks>
public class TclTextTests
{
    /// <summary>
    ///     Verifies that a plain value with no metacharacters is wrapped unchanged in braces.
    /// </summary>
    [Fact]
    public void TclText_Quote_PlainValue_ReturnsBracedLiteral()
    {
        // Act
        var result = TclText.Quote("plain_value");

        // Assert
        Assert.Equal("{plain_value}", result);
    }

    /// <summary>
    ///     Verifies that a value containing a space is brace-quoted verbatim, preserving the space.
    /// </summary>
    [Fact]
    public void TclText_Quote_ValueWithSpace_ReturnsBracedLiteral()
    {
        // Act
        var result = TclText.Quote("my file.vhd");

        // Assert
        Assert.Equal("{my file.vhd}", result);
    }

    /// <summary>
    ///     Verifies that a value containing a square bracket is brace-quoted verbatim (command
    ///     substitution is suppressed by braces).
    /// </summary>
    [Fact]
    public void TclText_Quote_ValueWithSquareBrackets_ReturnsBracedLiteral()
    {
        // Act
        var result = TclText.Quote("my file [1].vhd");

        // Assert
        Assert.Equal("{my file [1].vhd}", result);
    }

    /// <summary>
    ///     Verifies that a value containing a dollar sign is brace-quoted verbatim (variable
    ///     substitution is suppressed by braces).
    /// </summary>
    [Fact]
    public void TclText_Quote_ValueWithDollarSign_ReturnsBracedLiteral()
    {
        // Act
        var result = TclText.Quote("value$with$dollar");

        // Assert
        Assert.Equal("{value$with$dollar}", result);
    }

    /// <summary>
    ///     Verifies that a value containing a double quote is brace-quoted verbatim.
    /// </summary>
    [Fact]
    public void TclText_Quote_ValueWithDoubleQuote_ReturnsBracedLiteral()
    {
        // Act
        var result = TclText.Quote("value\"with\"quote");

        // Assert
        Assert.Equal("{value\"with\"quote}", result);
    }

    /// <summary>
    ///     Verifies that a value containing a backslash is brace-quoted verbatim (backslash
    ///     substitution is suppressed by braces).
    /// </summary>
    [Fact]
    public void TclText_Quote_ValueWithBackslash_ReturnsBracedLiteral()
    {
        // Act
        var result = TclText.Quote(@"value\with\backslash");

        // Assert
        Assert.Equal("{value\\with\\backslash}", result);
    }

    /// <summary>
    ///     Verifies that a value containing balanced braces falls back to the double-quoted,
    ///     backslash-escaped form, with each metacharacter (including the braces themselves)
    ///     individually escaped.
    /// </summary>
    [Fact]
    public void TclText_Quote_ValueWithBalancedBraces_ReturnsEscapedDoubleQuotedFallback()
    {
        // Act
        var result = TclText.Quote("value{with}braces");

        // Assert
        Assert.Equal("\"value\\{with\\}braces\"", result);
    }

    /// <summary>
    ///     Verifies that the fallback path escapes every TCL metacharacter (backslash, double
    ///     quote, dollar, square brackets, and braces) when a brace forces the fallback, and
    ///     escapes the backslash first so escaping backslashes are not themselves re-escaped.
    /// </summary>
    [Fact]
    public void TclText_Quote_ValueWithBracesAndOtherMetacharacters_EscapesAllMetacharacters()
    {
        // Act
        var result = TclText.Quote("a{b}c\\d\"e$f[g]h");

        // Assert
        Assert.Equal("\"a\\{b\\}c\\\\d\\\"e\\$f\\[g\\]h\"", result);
    }

    /// <summary>
    ///     Verifies that passing a null value throws <see cref="ArgumentNullException"/> rather
    ///     than a less-clear <see cref="NullReferenceException"/> from dereferencing the value.
    /// </summary>
    [Fact]
    public void TclText_Quote_NullValue_ThrowsArgumentNullException()
    {
        // Act / Assert
        Assert.Throws<ArgumentNullException>(() => TclText.Quote(null!));
    }
}
