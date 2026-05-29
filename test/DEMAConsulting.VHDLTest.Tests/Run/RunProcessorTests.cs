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

namespace DEMAConsulting.VHDLTest.Tests.Run;

/// <summary>
/// Tests for <see cref="RunProcessor"/> class.
/// </summary>
public class RunProcessorTests
{
    /// <summary>
    ///     Verifies that attempting to execute a non-existent program raises an exception.
    /// </summary>
    [Fact]
    public void RunProcessor_Execute_MissingProgram_ThrowsException()
    {
        // Arrange: create a processor with a single error-matching rule
        var processor = new RunProcessor(
        [
            RunLineRule.Create(RunLineType.Error, "Error")
        ]);

        // Act / Assert: executing an unknown program must throw
        Assert.ThrowsAny<Exception>(() => processor.Execute("unknown-program"));
    }

    /// <summary>
    ///     Verifies that output containing an error pattern produces an Error summary.
    /// </summary>
    [Fact]
    public void RunProcessor_Execute_ProgramWithError_ReturnsErrorResult()
    {
        // Arrange: create a processor with an error rule matching "Error"
        var processor = new RunProcessor(
        [
            RunLineRule.Create(RunLineType.Error, "Error")
        ]);

        // Act: run dotnet with an unknown command, which produces error output
        var result = processor.Execute("dotnet", "", "unknown-command");

        // Assert: the error pattern in the output drives the summary to Error
        Assert.Equal(RunLineType.Error, result.Summary);
    }

    /// <summary>
    ///     Verifies that output containing an info pattern produces an Info summary.
    /// </summary>
    [Fact]
    public void RunProcessor_Execute_ProgramWithSuccess_ReturnsInfoResult()
    {
        // Arrange: create a processor with an info rule matching "Usage"
        var processor = new RunProcessor(
        [
            RunLineRule.Create(RunLineType.Info, "Usage")
        ]);

        // Act: run dotnet help, which outputs usage information
        var result = processor.Execute("dotnet", "", "help");

        // Assert: the Info-matched lines drive the summary to Info
        Assert.Equal(RunLineType.Info, result.Summary);
    }

    /// <summary>
    ///     Verifies that a non-zero exit code forces the summary to at least Error even when
    ///     no error pattern is present in the output.
    /// </summary>
    [Fact]
    public void RunProcessor_Execute_ProgramWithNonZeroExitCode_ReturnsErrorResult()
    {
        // Arrange: create a processor with no rules so no line will match an error pattern
        var processor = new RunProcessor([]);

        // Act: run dotnet with an unknown command, which exits non-zero
        var result = processor.Execute("dotnet", "", "unknown-command");

        // Assert: the non-zero exit code elevates the summary to Error regardless of patterns
        Assert.True(result.Summary >= RunLineType.Error);
    }

    /// <summary>
    ///     Verifies that an output line matching a Warning rule is classified as Warning
    ///     and that the summary reflects the highest severity.
    /// </summary>
    [Fact]
    public void RunProcessor_Parse_OutputWithWarningPattern_ReturnsWarningSummary()
    {
        // Arrange: create a processor with a Warning rule and fixed output
        var processor = new RunProcessor(
        [
            RunLineRule.Create(RunLineType.Warning, "warning")
        ]);

        // Act: parse output containing the warning keyword
        var result = processor.Parse(DateTime.Now, DateTime.Now, "this is a warning line", 0);

        // Assert: the matched line is Warning and the summary reflects that
        Assert.Multiple(
            () => Assert.Equal(RunLineType.Warning, result.Summary),
            () => Assert.Contains(result.Lines, l => l.Type == RunLineType.Warning));
    }

    /// <summary>
    ///     Verifies that an output line matching no rule is classified as Text.
    /// </summary>
    [Fact]
    public void RunProcessor_Parse_OutputWithNoMatchingPattern_ReturnsTextClassification()
    {
        // Arrange: create a processor with an Info rule that will not match the output
        var processor = new RunProcessor(
        [
            RunLineRule.Create(RunLineType.Info, "info")
        ]);

        // Act: parse output that contains no matching keyword
        var result = processor.Parse(DateTime.Now, DateTime.Now, "this line does not match", 0);

        // Assert: the unmatched line is classified as Text
        Assert.Contains(result.Lines, l => l.Type == RunLineType.Text);
    }

    /// <summary>
    ///     Verifies that <c>Execute(Context, ...)</c> writes verbose log entries for the working
    ///     directory and command to the context before executing the program.
    /// </summary>
    [Fact]
    public void RunProcessor_Execute_WithContext_LogsRunInfo()
    {
        // Arrange: use a unique per-test temp log file to capture verbose context output
        var logFile = Path.Combine(Path.GetTempPath(), $"rp_test_{Guid.NewGuid():N}.log");
        try
        {
            var processor = new RunProcessor(
            [
                RunLineRule.Create(RunLineType.Info, "Usage")
            ]);
            RunResults results;
            using (var context = Context.Create(["--verbose", "--log", logFile, "--silent"]))
            {
                // Act: invoke the Execute overload that accepts a Context
                results = processor.Execute(context, "dotnet", "", "help");
            }

            // Assert: results are valid
            Assert.NotNull(results);
            Assert.Equal(0, results.ExitCode);

            // Assert: context captured the verbose log lines written before execution
            var logContent = File.ReadAllText(logFile);
            Assert.Multiple(
                () => Assert.Contains("Run Directory", logContent),
                () => Assert.Contains("Run Command", logContent));
        }
        finally
        {
            if (File.Exists(logFile))
            {
                File.Delete(logFile);
            }
        }
    }
}
