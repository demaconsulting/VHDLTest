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

using System.Text.RegularExpressions;

namespace DEMAConsulting.VHDLTest.Run;

/// <summary>
///     Run Line Rule Record
/// </summary>
/// <param name="Type">Run Line Type</param>
/// <param name="Pattern">Text Match Pattern</param>
public record RunLineRule(RunLineType Type, Regex Pattern)
{
    /// <summary>
    /// Create a new RunLineRule
    /// </summary>
    /// <param name="type">Type</param>
    /// <param name="pattern">Pattern</param>
    /// <returns>New RunLineRule</returns>
    public static RunLineRule Create(RunLineType type, string pattern)
    {
        return new RunLineRule(
            type,
            new Regex(pattern, RegexOptions.None, TimeSpan.FromMilliseconds(100)));
    }
}
