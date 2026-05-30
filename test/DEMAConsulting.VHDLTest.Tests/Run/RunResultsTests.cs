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
using DEMAConsulting.VHDLTest.Run;

namespace DEMAConsulting.VHDLTest.Tests.Run;

/// <summary>
///     Unit tests for the <see cref="RunResults"/> class.
/// </summary>
public class RunResultsTests
{
    /// <summary>
    ///     Verifies that Print writes color-coded output for lines of all severity types,
    ///     confirming that Info, Warning, Error, and Text lines are all written to the log.
    /// </summary>
    [Fact]
    public void RunResults_Print_WithMixedLines_WritesColorCodedOutput()
    {
        // Arrange: construct a RunResults with one line of each type and a temp log path
        var logPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        var lines = new ReadOnlyCollection<RunLine>(new List<RunLine>
        {
            new(RunLineType.Text, "text line"),
            new(RunLineType.Info, "info line"),
            new(RunLineType.Warning, "warning line"),
            new(RunLineType.Error, "error line"),
        });
        var results = new RunResults(RunLineType.Error, DateTime.Now, 0.0, 0, "output", lines);

        try
        {
            string output;
            using (var context = Context.Create(["--verbose", "--log", logPath, "--silent"]))
            {
                // Act: print results — exercises color selection and output paths for all line types
                results.Print(context);
            }

            // Assert: log contains each line type's text, confirming all branches were written
            output = File.ReadAllText(logPath);
            Assert.Contains("text line", output);
            Assert.Contains("info line", output);
            Assert.Contains("warning line", output);
            Assert.Contains("error line", output);
        }
        finally
        {
            File.Delete(logPath);
        }
    }

    /// <summary>
    ///     Verifies that Print suppresses Text-classified lines when verbose output is disabled,
    ///     and that non-Text lines are still written.
    /// </summary>
    [Fact]
    public void RunResults_Print_WithVerboseDisabled_SuppressesTextLines()
    {
        // Arrange: construct a RunResults with Text and Info lines; verbose is disabled
        var logPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        var lines = new ReadOnlyCollection<RunLine>(new List<RunLine>
        {
            new(RunLineType.Text, "text line — should be suppressed"),
            new(RunLineType.Info, "info line — should be written"),
        });
        var results = new RunResults(RunLineType.Info, DateTime.Now, 0.0, 0, "output", lines);

        try
        {
            string output;
            using (var context = Context.Create(["--log", logPath, "--silent"])) // Verbose = false
            {
                // Act: print results with verbose disabled — Text lines must not be written
                results.Print(context);
            }

            // Assert: log does not contain the Text line but does contain the Info line
            output = File.ReadAllText(logPath);
            Assert.DoesNotContain("text line — should be suppressed", output);
            Assert.Contains("info line — should be written", output);
        }
        finally
        {
            File.Delete(logPath);
        }
    }

    /// <summary>
    ///     Verifies that passing null as the context to Print throws ArgumentNullException,
    ///     confirming that the null-guard on Print is enforced before any output is attempted.
    /// </summary>
    [Fact]
    public void RunResults_Print_WithNullContext_ThrowsArgumentNullException()
    {
        // Arrange: construct a minimal RunResults with no lines
        var results = new RunResults(
            RunLineType.Info,
            DateTime.Now,
            0.0,
            0,
            string.Empty,
            new System.Collections.ObjectModel.ReadOnlyCollection<RunLine>([]));

        // Act / Assert: passing null as context must throw ArgumentNullException
        Assert.Throws<ArgumentNullException>(() => results.Print(null!));
    }
}
