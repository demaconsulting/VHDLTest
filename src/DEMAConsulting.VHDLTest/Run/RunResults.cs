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

using System.Collections.ObjectModel;
using DEMAConsulting.VHDLTest.Cli;

namespace DEMAConsulting.VHDLTest.Run;

/// <summary>
///     Immutable record holding the complete outcome of a single simulator execution. It is
///     the primary return value from <see cref="RunProcessor"/> and the data source for
///     simulator pass/fail decisions, result serialization, and console output display.
/// </summary>
/// <param name="Summary">
///     Highest-severity <see cref="RunLineType"/> across all classified output lines. Elevated to at least
///     <see cref="RunLineType.Error"/> when <paramref name="ExitCode"/> is non-zero, ensuring downstream
///     pass/fail decisions always reflect a simulator-reported error.
/// </param>
/// <param name="Start">
///     Wall-clock timestamp recorded immediately before the simulator process was launched. Used to compute
///     <paramref name="Duration"/> and stored in test-result reports for traceability.
/// </param>
/// <param name="Duration">
///     Elapsed time in seconds between <paramref name="Start"/> and process exit. Must be non-negative.
///     Formatted to one decimal place in summary output.
/// </param>
/// <param name="ExitCode">
///     Raw process exit code returned by the simulator. Zero indicates success; any non-zero value indicates
///     failure and causes <paramref name="Summary"/> to be elevated to at least <see cref="RunLineType.Error"/>.
/// </param>
/// <param name="Output">
///     Full combined stdout and stderr text captured from the simulator process, unmodified. May be empty;
///     never null.
/// </param>
/// <param name="Lines">
///     Ordered collection of classified output lines produced by <see cref="RunProcessor.Parse"/>. Each entry
///     pairs the original text with its assigned <see cref="RunLineType"/>. May be empty; never null.
/// </param>
public sealed record RunResults(
    RunLineType Summary,
    DateTime Start,
    double Duration,
    int ExitCode,
    string Output,
    ReadOnlyCollection<RunLine> Lines)
{
    /// <summary>
    ///     Iterates over <see cref="Lines"/> and writes each line to the console using a color
    ///     determined by its <see cref="RunLineType"/>. Text-classified lines are suppressed
    ///     unless verbose output is enabled.
    /// </summary>
    /// <remarks>
    ///     <c>Print</c> writes to <paramref name="context"/> as a side effect; the number of
    ///     lines written depends on <see cref="Lines"/> content and the value of
    ///     <c>context.Verbose</c>. <see cref="RunResults"/> is immutable and thread-safe for
    ///     concurrent reads; multiple callers may read its properties simultaneously without
    ///     synchronization. Concurrent calls to <c>Print</c> that share the same
    ///     <see cref="Context"/> instance are subject to <see cref="Context"/>'s own
    ///     thread-safety contract.
    /// </remarks>
    /// <param name="context">
    ///     Context used for colored console output. Must not be null. The <c>Verbose</c>
    ///     property controls whether <see cref="RunLineType.Text"/> lines are written.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="context"/> is null.</exception>
    public void Print(Context context)
    {
        ArgumentNullException.ThrowIfNull(context);

        // Filter and write all lines
        foreach (var line in Lines.Where(l => context.Verbose || l.Type != RunLineType.Text))
        {
            // Pick the desired color
            var color = line.Type switch
            {
                RunLineType.Info => ConsoleColor.White,
                RunLineType.Warning => ConsoleColor.Yellow,
                RunLineType.Error => ConsoleColor.Red,
                _ => ConsoleColor.Gray
            };

            // Write the line with its severity color, then emit the newline separately without
            // color to prevent console color from bleeding into the line separator
            context.Write(color, line.Text);
            context.WriteLine("");
        }
    }
}
