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

using DEMAConsulting.VHDLTest.Cli;
using DEMAConsulting.VHDLTest.Run;

namespace DEMAConsulting.VHDLTest.Results;

/// <summary>
///     Immutable record capturing the outcome of a single VHDL test bench execution.
/// </summary>
/// <remarks>
///     TestResult is a passive data container; it performs no I/O and throws no exceptions.
///     The pass/fail determination is derived solely from the severity summary of the
///     wrapped <see cref="RunResults"/>. Constructed by concrete <c>Simulator</c> implementations
///     and held by <see cref="TestResults"/>.
/// </remarks>
/// <param name="ClassName">
///     Fully qualified test class name used as the TRX class identifier. Must not be null.
/// </param>
/// <param name="TestName">
///     Test bench name used as the logical test identifier in reports. Must not be null.
/// </param>
/// <param name="RunResults">
///     The raw execution results from the simulator, including exit code, captured output
///     lines, duration, and the highest-severity line type. Must not be null.
/// </param>
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
    /// <remarks>
    ///     Writes the word "Passed" in green or "Failed" in red, followed by the test name
    ///     and the duration formatted to one decimal place in parentheses. The colored word
    ///     is written via <see cref="Context.Write(ConsoleColor, string)"/> and the remainder
    ///     via <see cref="Context.WriteLine"/>.
    /// </remarks>
    /// <param name="context">Output channel to write to. Must not be null.</param>
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
