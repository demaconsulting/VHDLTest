﻿// Copyright (c) 2023 DEMA Consulting
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

namespace DEMAConsulting.VHDLTest.Results;

/// <summary>
/// Test result class
/// </summary>
/// <param name="ClassName">Class name</param>
/// <param name="TestName">Test name</param>
/// <param name="RunResults">Test run results</param>
public sealed record TestResult(string ClassName, string TestName, RunResults RunResults)
{
    /// <summary>
    ///     Test ID
    /// </summary>
    public Guid TestId { get; init; } = Guid.NewGuid();

    /// <summary>
    ///     Test Execution ID
    /// </summary>
    public Guid ExecutionId { get; init; } = Guid.NewGuid();

    /// <summary>
    ///     Gets a value indicating whether the test passed
    /// </summary>
    public bool Passed => RunResults.Summary < RunLineType.Error;

    /// <summary>
    ///     Gets a value indicating whether the test failed
    /// </summary>
    public bool Failed => RunResults.Summary >= RunLineType.Error;

    /// <summary>
    ///     Print a summary line to the console
    /// </summary>
    /// <param name="context">Program context</param>
    public void PrintSummary(Context context)
    {
        // Print the colored summary word
        context.Write(
            Passed ? ConsoleColor.Green : ConsoleColor.Red,
            Passed ? "Passed" : "Failed");

        // Print test name and duration
        context.WriteLine($" {TestName} ({RunResults.Duration:F1} seconds)");
    }
}