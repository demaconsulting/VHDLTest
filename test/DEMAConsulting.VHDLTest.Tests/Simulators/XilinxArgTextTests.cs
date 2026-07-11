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
///     Unit tests for the <see cref="XilinxArgText"/> helper class, covering the double-quote
///     wrapping and backslash-escaping behavior used for Xilinx argument files.
/// </summary>
/// <remarks>
///     Unit under test: <see cref="XilinxArgText.Quote"/>.
/// </remarks>
public class XilinxArgTextTests
{
    /// <summary>
    ///     Verifies that a plain value with no special characters is wrapped unchanged in
    ///     double quotes.
    /// </summary>
    [Fact]
    public void XilinxArgText_Quote_PlainValue_ReturnsQuotedLiteral()
    {
        // Act
        var result = XilinxArgText.Quote("plain_value");

        // Assert
        Assert.Equal("\"plain_value\"", result);
    }

    /// <summary>
    ///     Verifies that a value containing a space is double-quoted verbatim, preserving the
    ///     space.
    /// </summary>
    [Fact]
    public void XilinxArgText_Quote_ValueWithSpace_ReturnsQuotedLiteral()
    {
        // Act
        var result = XilinxArgText.Quote("my file.vhd");

        // Assert
        Assert.Equal("\"my file.vhd\"", result);
    }

    /// <summary>
    ///     Verifies that a value containing an embedded double quote is escaped with a
    ///     backslash.
    /// </summary>
    [Fact]
    public void XilinxArgText_Quote_ValueWithDoubleQuote_EscapesDoubleQuote()
    {
        // Act
        var result = XilinxArgText.Quote("value\"with\"quote");

        // Assert
        Assert.Equal("\"value\\\"with\\\"quote\"", result);
    }

    /// <summary>
    ///     Verifies that a value containing an embedded backslash is escaped with a backslash.
    /// </summary>
    [Fact]
    public void XilinxArgText_Quote_ValueWithBackslash_EscapesBackslash()
    {
        // Act
        var result = XilinxArgText.Quote(@"value\with\backslash");

        // Assert
        Assert.Equal("\"value\\\\with\\\\backslash\"", result);
    }

    /// <summary>
    ///     Verifies that a value containing both an embedded backslash and an embedded double
    ///     quote escapes both characters, escaping the backslash first so the escaping
    ///     backslashes themselves are not re-escaped.
    /// </summary>
    [Fact]
    public void XilinxArgText_Quote_ValueWithBackslashAndDoubleQuote_EscapesBoth()
    {
        // Act
        var result = XilinxArgText.Quote("a\\b\"c");

        // Assert
        Assert.Equal("\"a\\\\b\\\"c\"", result);
    }

    /// <summary>
    ///     Verifies that passing a null value throws <see cref="ArgumentNullException"/> rather
    ///     than a less-clear <see cref="NullReferenceException"/> from iterating the value.
    /// </summary>
    [Fact]
    public void XilinxArgText_Quote_NullValue_ThrowsArgumentNullException()
    {
        // Act / Assert
        Assert.Throws<ArgumentNullException>(() => XilinxArgText.Quote(null!));
    }
}
