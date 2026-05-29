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
/// <param name="Summary">Result summary</param>
/// <param name="Start">Start time</param>
/// <param name="Duration">Duration</param>
/// <param name="ExitCode">Exit code</param>
/// <param name="Output">Output text</param>
/// <param name="Lines">Result lines</param>
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
    /// <param name="context">
    ///     Context used for colored console output. Must not be null. The <c>Verbose</c>
    ///     property controls whether <see cref="RunLineType.Text"/> lines are written.
    /// </param>
    public void Print(Context context)
    {
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

            // Write the line
            context.Write(color, line.Text);
            context.WriteLine("");
        }
    }
}
