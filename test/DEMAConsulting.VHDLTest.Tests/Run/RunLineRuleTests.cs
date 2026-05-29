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

using DEMAConsulting.VHDLTest.Run;

namespace DEMAConsulting.VHDLTest.Tests.Run;

/// <summary>
///     Unit tests for the <see cref="RunLineRule"/> class.
/// </summary>
public class RunLineRuleTests
{
    /// <summary>
    ///     Verifies that RunLineRule.Create rejects an invalid regex pattern.
    /// </summary>
    [Fact]
    public void RunLineRule_Create_InvalidPattern_ThrowsArgumentException()
    {
        // Arrange: prepare an invalid regex pattern
        const string invalidPattern = "[invalid";

        // Act / Assert: creating a rule with an invalid pattern must throw ArgumentException
        Assert.ThrowsAny<ArgumentException>(() => RunLineRule.Create(RunLineType.Error, invalidPattern));
    }

    /// <summary>
    ///     Verifies that RunLineRule.Create with a valid pattern returns a rule whose Type and
    ///     Pattern are correctly set and whose Pattern can match a sample line.
    /// </summary>
    [Fact]
    public void RunLineRule_Create_ValidPattern_ReturnsRuleWithMatchingProperties()
    {
        // Arrange: define a valid pattern and the expected type
        const string pattern = "Error:";
        const RunLineType expectedType = RunLineType.Error;

        // Act: create the rule
        var rule = RunLineRule.Create(expectedType, pattern);

        // Assert: rule has correct Type and Pattern that matches the expected text
        Assert.Equal(expectedType, rule.Type);
        Assert.NotNull(rule.Pattern);
        Assert.True(rule.Pattern.IsMatch("Error: something went wrong"),
            "Pattern should match a line containing 'Error:'");
        Assert.False(rule.Pattern.IsMatch("Info: all is well"),
            "Pattern should not match a line not containing 'Error:'");
    }
}
