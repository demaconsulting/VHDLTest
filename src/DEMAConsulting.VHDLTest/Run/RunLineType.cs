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

namespace DEMAConsulting.VHDLTest.Run;

/// <summary>
///     Classifies a simulator output line into one of four severity categories, used throughout the Run and
///     Results subsystems to drive color-coded output, verbose suppression, and pass/fail decisions.
/// </summary>
/// <remarks>
///     Ordinal values encode severity rank intentionally: higher ordinal means higher
///     severity. <see cref="RunProcessor"/> uses the <c>&gt;</c> operator to compute
///     the highest-severity summary across all output lines, and downstream code
///     compares values with <c>&gt;=</c> to determine pass/fail thresholds. As a value
///     type (enum), <see cref="RunLineType"/> is inherently thread-safe; values may be
///     read and compared concurrently without synchronization.
/// </remarks>
// Ordinal values are intentional: RunProcessor uses > to compute the highest-severity summary.
public enum RunLineType
{
    /// <summary>
    ///     Normal unclassified text. Lowest severity (ordinal 0); suppressed by <see cref="RunResults.Print"/>
    ///     when verbose output is disabled.
    /// </summary>
    Text,

    /// <summary>
    ///     Informational message. Severity level 1; always written by <see cref="RunResults.Print"/> in white.
    /// </summary>
    Info,

    /// <summary>
    ///     Warning message. Severity level 2; written by <see cref="RunResults.Print"/> in yellow. A result
    ///     with <see cref="Warning"/> summary is considered passed.
    /// </summary>
    Warning,

    /// <summary>
    ///     Error message. Highest severity (ordinal 3); written by <see cref="RunResults.Print"/> in red. A
    ///     result with <see cref="Error"/> summary is considered failed.
    /// </summary>
    Error
}
