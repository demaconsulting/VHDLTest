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
///     Tests for <see cref="RunProcessor"/> class.
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
    ///     Verifies that an output line matching an error rule is classified as Error
    ///     when the exit code is zero, isolating pattern-classification from exit-code escalation.
    /// </summary>
    [Fact]
    public void RunProcessor_Parse_OutputWithErrorPattern_ReturnsErrorClassification()
    {
        // Arrange: create a processor with an error rule and synthetic output with zero exit code
        var processor = new RunProcessor(
        [
            RunLineRule.Create(RunLineType.Error, "Error:")
        ]);

        // Act: parse output containing the error pattern with exit code 0 to isolate pattern classification
        var result = processor.Parse(DateTime.Now, DateTime.Now, "** Error: something went wrong", 0);

        // Assert: the matched line drives the summary to Error even with a zero exit code
        Assert.Multiple(
            () => Assert.Equal(RunLineType.Error, result.Summary),
            () => Assert.Contains(result.Lines, l => l.Type == RunLineType.Error));
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

        // Assert: the summary is Error; both pattern matching and exit-code escalation
        // can contribute — this test verifies the combined outcome
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

        // Assert: individual lines are classified as Info and the summary reflects that
        Assert.Multiple(
            () => Assert.Equal(RunLineType.Info, result.Summary),
            () => Assert.Contains(result.Lines, l => l.Type == RunLineType.Info));
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

        // Assert: the unmatched line is classified as Text and the summary is Text
        Assert.Multiple(
            () => Assert.Equal(RunLineType.Text, result.Summary),
            () => Assert.Contains(result.Lines, l => l.Type == RunLineType.Text));
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

    /// <summary>
    ///     Verifies that <c>Execute(Context, ...)</c> writes a verbose log entry for the run
    ///     command (including the application name) independently of the working-directory entry.
    /// </summary>
    [Fact]
    public void RunProcessor_Execute_WithVerboseContext_LogsCommand()
    {
        // Arrange: use a unique per-test temp log file to capture verbose context output
        var logFile = Path.Combine(Path.GetTempPath(), $"rp_cmd_test_{Guid.NewGuid():N}.log");
        try
        {
            var processor = new RunProcessor(
            [
                RunLineRule.Create(RunLineType.Info, "Usage")
            ]);
            using (var context = Context.Create(["--verbose", "--log", logFile, "--silent"]))
            {
                // Act: invoke Execute(Context, ...) with a known application
                processor.Execute(context, "dotnet", "", "help");
            }

            // Assert: the verbose log contains the "Run Command" entry with the application name
            var logContent = File.ReadAllText(logFile);
            Assert.Multiple(
                () => Assert.Contains("Run Command", logContent),
                () => Assert.Contains("dotnet", logContent));
        }
        finally
        {
            if (File.Exists(logFile))
            {
                File.Delete(logFile);
            }
        }
    }

    /// <summary>
    ///     Verifies that RunProcessor.Parse elevates the RunResults Summary to at least
    ///     RunLineType.Error when the exit code is non-zero and no rule matches the output,
    ///     confirming the SummaryElevation invariant is enforced independently of pattern matching.
    /// </summary>
    [Fact]
    public void RunProcessor_Parse_WithNonZeroExitCode_ElevatesSummaryToAtLeastError()
    {
        // Arrange: create a processor with no rules so no line elevates the summary via pattern
        // matching; use fixed timestamps so the duration calculation is deterministic
        var processor = new RunProcessor([]);
        var start = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Local);
        var end = new DateTime(2024, 1, 1, 0, 0, 1, DateTimeKind.Local);

        // Act: parse output with exit code 1 — the non-zero exit code must escalate the summary
        // even though no error pattern exists in the rule set
        var results = processor.Parse(start, end, "no error pattern present", exitCode: 1);

        // Assert: summary is at least Error solely due to the non-zero exit code
        Assert.True(results.Summary >= RunLineType.Error,
            $"Expected Summary >= RunLineType.Error for non-zero ExitCode, but got {results.Summary}");
    }

    /// <summary>
    ///     Verifies that <c>Execute(Context, ...)</c> wraps the command with <c>cmd /c</c> on
    ///     Windows so that batch files and other shell-dispatched programs can be launched
    ///     without requiring the caller to know about the Windows shell requirement.
    /// </summary>
    [Fact]
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public void RunProcessor_Execute_WithContext_OnWindows_WrapsCommandWithCmdSlashC()
    {
        // Skip this test on non-Windows platforms — the cmd /c wrapping only occurs on Windows
        if (!OperatingSystem.IsWindows())
        {
            Assert.Skip("This test only applies to Windows");
        }

        // Arrange: use a unique per-test temp log file to capture the verbose command log
        var logFile = Path.Combine(Path.GetTempPath(), $"rp_win_test_{Guid.NewGuid():N}.log");
        try
        {
            var processor = new RunProcessor(
            [
                RunLineRule.Create(RunLineType.Info, "Usage")
            ]);
            using (var context = Context.Create(["--verbose", "--log", logFile, "--silent"]))
            {
                // Act: invoke Execute(Context, ...) with a known-good application; on Windows
                // the implementation wraps the command with cmd /c before launching
                processor.Execute(context, "dotnet", "", "help");
            }

            // Assert: the verbose log contains the cmd /c wrapper added on Windows
            var logContent = File.ReadAllText(logFile);
            Assert.Contains("cmd /c", logContent);
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
