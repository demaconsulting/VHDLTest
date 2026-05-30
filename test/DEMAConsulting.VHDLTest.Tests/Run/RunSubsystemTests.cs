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
/// Subsystem integration tests for the Run subsystem.
/// These tests verify that <see cref="RunProcessor"/>, <see cref="RunProgram"/>, and
/// <see cref="RunResults"/> work together to execute programs and classify their output.
/// </summary>
public class RunSubsystemTests
{
    /// <summary>
    ///     Test that RunProcessor executes a real program via RunProgram, and produces a
    ///     RunResults object with correctly classified output lines.
    /// </summary>
    [Fact]
    public void RunSubsystem_ExecuteRealProgram_WithClassificationRules_ProducesClassifiedRunResults()
    {
        // Arrange - create a processor with an Info classification rule
        var processor = new RunProcessor(
        [
            RunLineRule.Create(RunLineType.Info, "Usage")
        ]);

        // Act - execute a real program through the full Run pipeline
        var results = processor.Execute("dotnet", "", "help");

        // Assert - RunProgram ran the program and RunProcessor classified the output into RunResults
        Assert.NotNull(results);
        Assert.Equal(0, results.ExitCode);
        Assert.True(results.Output.Length > 0);
        Assert.True(results.Lines.Count > 0);
        Assert.Contains(results.Lines, line => line.Type == RunLineType.Info);
        Assert.True(results.Duration >= 0.0);
    }

    /// <summary>
    ///     Test that RunProcessor correctly surfaces a non-zero exit code from RunProgram
    ///     as an Error summary in the RunResults.
    /// </summary>
    [Fact]
    public void RunSubsystem_ExecuteRealProgram_WithErrorExitCode_ProducesErrorRunResults()
    {
        // Arrange - create a processor with no special classification rules
        var processor = new RunProcessor([]);

        // Act - execute dotnet with an unknown command to produce a non-zero exit code
        var results = processor.Execute("dotnet", "", "unknown-command");

        // Assert - RunResults reflects the non-zero exit code as an error summary
        Assert.NotNull(results);
        Assert.NotEqual(0, results.ExitCode);
        Assert.Equal(RunLineType.Error, results.Summary);

        // Assert: with no classification rules, every output line falls back to Text type
        Assert.All(results.Lines, line => Assert.Equal(RunLineType.Text, line.Type));
    }

    /// <summary>
    ///     Verifies that the <c>Execute(Context, string, string, string[])</c> overload logs
    ///     the working directory and run command to the context before delegating to the
    ///     process execution path.
    /// </summary>
    [Fact]
    public void RunSubsystem_Execute_WithContext_LogsCommandToContext()
    {
        // Arrange: create a verbose context backed by a log file so that verbose output can
        // be read back after execution; use a unique filename to avoid cross-test conflicts
        var logFile = Path.Combine(Path.GetTempPath(), $"run_test_{Guid.NewGuid():N}.log");
        try
        {
            RunResults results;
            using (var context = Context.Create(["--verbose", "--log", logFile, "--silent"]))
            {
                var processor = new RunProcessor(
                [
                    RunLineRule.Create(RunLineType.Info, "Usage")
                ]);

                // Act: call the Execute overload that accepts a Context — this overload writes
                // verbose log lines for the working directory and command before running
                results = processor.Execute(context, "dotnet", "", "help");
            }

            // Assert: RunResults contain expected classified output from the real program
            Assert.NotNull(results);
            Assert.Equal(0, results.ExitCode);
            Assert.True(results.Lines.Count > 0, "Expected at least one classified output line");
            Assert.Contains(results.Lines, line => line.Type == RunLineType.Info);

            // Assert: the Context captured the verbose log lines written before execution
            var logContent = File.ReadAllText(logFile);
            Assert.Contains("Run Directory", logContent);
            Assert.Contains("Run Command", logContent);
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
