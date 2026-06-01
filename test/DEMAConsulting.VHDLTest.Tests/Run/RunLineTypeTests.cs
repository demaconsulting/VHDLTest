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
///     Unit tests for the <see cref="RunLineType"/> enum, verifying the intentional
///     ordinal severity ordering used by <see cref="RunProcessor"/> for summary computation.
/// </summary>
public class RunLineTypeTests
{
    /// <summary>
    ///     Verifies that Text has a lower ordinal than Info, confirming it is the lowest severity.
    /// </summary>
    [Fact]
    public void RunLineType_Ordinal_Text_IsLessThan_Info()
    {
        // Arrange / Act: compare ordinal values directly
        // Assert: Text < Info in the severity ordering
        Assert.True(RunLineType.Text < RunLineType.Info,
            "Text should have lower severity than Info");
    }

    /// <summary>
    ///     Verifies that Info has a lower ordinal than Warning.
    /// </summary>
    [Fact]
    public void RunLineType_Ordinal_Info_IsLessThan_Warning()
    {
        // Arrange / Act: compare ordinal values directly
        // Assert: Info < Warning in the severity ordering
        Assert.True(RunLineType.Info < RunLineType.Warning,
            "Info should have lower severity than Warning");
    }

    /// <summary>
    ///     Verifies that Warning has a lower ordinal than Error.
    /// </summary>
    [Fact]
    public void RunLineType_Ordinal_Warning_IsLessThan_Error()
    {
        // Arrange / Act: compare ordinal values directly
        // Assert: Warning < Error in the severity ordering
        Assert.True(RunLineType.Warning < RunLineType.Error,
            "Warning should have lower severity than Error");
    }

    /// <summary>
    ///     Verifies that Error has the highest ordinal, confirming it is the highest severity.
    /// </summary>
    [Fact]
    public void RunLineType_Ordinal_Error_IsGreaterThan_Warning()
    {
        // Arrange / Act: compare ordinal values directly
        // Assert: Error > Warning in the severity ordering
        Assert.True(RunLineType.Error > RunLineType.Warning,
            "Error should have higher severity than Warning");
    }
}
