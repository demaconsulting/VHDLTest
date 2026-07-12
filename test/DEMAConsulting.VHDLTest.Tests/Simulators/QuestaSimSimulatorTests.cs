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
using DEMAConsulting.VHDLTest.Simulators;
using DEMAConsulting.VHDLTest.Tests.Run;

namespace DEMAConsulting.VHDLTest.Tests.Simulators;

/// <summary>
///     Tests for the QuestaSim simulator.
/// </summary>
/// <remarks>
///     All tests in this class are serialized via the SimulatorEnvVarTests collection because
///     QuestaSimSimulator_FindPath_WithEnvVar_ReturnsEnvVarValue modifies the
///     VHDLTEST_QUESTASIM_PATH process-level environment variable.
///     The DisableParallelization = true collection definition in SimulatorTestCollections.cs
///     ensures these tests run sequentially with other env-var tests, preventing race conditions.
/// </remarks>
[Collection("SimulatorEnvVarTests")]
public class QuestaSimSimulatorTests
{
    /// <summary>
    /// Check name of QuestaSim simulator
    /// </summary>
    [Fact]
    public void QuestaSimSimulator_SimulatorName_ReturnsQuestaSim()
    {
        // Act / Assert: simulator name is the fixed string "QuestaSim"
        Assert.Equal("QuestaSim", QuestaSimSimulator.Instance.SimulatorName);
    }

    /// <summary>
    /// Test QuestaSim simulator compile with clean output
    /// </summary>
    [Fact]
    public void QuestaSimSimulator_CompileProcessor_CleanOutput_ReturnsTextResult()
    {
        // Arrange: output with no diagnostic patterns
        // Act: parse two plain-text lines
        var results = QuestaSimSimulator.CompileProcessor.Parse(
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc),
            "Compile\nNo Issues",
            0);

        // Assert: summary is Text and all lines are classified as Text
        Assert.Equal(RunLineType.Text, results.Summary);
        Assert.Equal(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.Equal(5.0, results.Duration, 1);
        Assert.Equal(0, results.ExitCode);
        Assert.Equal("Compile\nNo Issues", results.Output);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Compile", results.Lines[0].Text);
        Assert.Equal(RunLineType.Text, results.Lines[1].Type);
        Assert.Equal("No Issues", results.Lines[1].Text);
    }

    /// <summary>
    /// Test QuestaSim simulator compile with an error message
    /// </summary>
    [Fact]
    public void QuestaSimSimulator_CompileProcessor_ErrorOutput_ReturnsErrorResult()
    {
        // Arrange: output containing the QuestaSim error pattern
        // Act: parse an error line
        var results = QuestaSimSimulator.CompileProcessor.Parse(
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc),
            "Compile\nError: Compile Error",
            1);

        // Assert: summary is Error and the diagnostic line is classified as Error
        Assert.Equal(RunLineType.Error, results.Summary);
        Assert.Equal(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.Equal(5.0, results.Duration, 1);
        Assert.Equal(1, results.ExitCode);
        Assert.Equal("Compile\nError: Compile Error", results.Output);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Compile", results.Lines[0].Text);
        Assert.Equal(RunLineType.Error, results.Lines[1].Type);
        Assert.Equal("Error: Compile Error", results.Lines[1].Text);
    }

    /// <summary>
    /// Test QuestaSim simulator test with clean output
    /// </summary>
    [Fact]
    public void QuestaSimSimulator_TestProcessor_CleanOutput_ReturnsTextResult()
    {
        // Arrange: output with no diagnostic patterns
        // Act: parse two plain-text lines
        var results = QuestaSimSimulator.TestProcessor.Parse(
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc),
            "Test\nNo Issues",
            0);

        // Assert: summary is Text and all lines are classified as Text
        Assert.Equal(RunLineType.Text, results.Summary);
        Assert.Equal(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.Equal(5.0, results.Duration, 1);
        Assert.Equal(0, results.ExitCode);
        Assert.Equal("Test\nNo Issues", results.Output);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Test", results.Lines[0].Text);
        Assert.Equal(RunLineType.Text, results.Lines[1].Type);
        Assert.Equal("No Issues", results.Lines[1].Text);
    }

    /// <summary>
    /// Test QuestaSim simulator test with an info message
    /// </summary>
    [Fact]
    public void QuestaSimSimulator_TestProcessor_InfoOutput_ReturnsInfoResult()
    {
        // Arrange: output containing the QuestaSim note pattern
        // Act: parse an info line
        var results = QuestaSimSimulator.TestProcessor.Parse(
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc),
            "Test\nNote: Test Note",
            0);

        // Assert: summary is Info and the diagnostic line is classified as Info
        Assert.Equal(RunLineType.Info, results.Summary);
        Assert.Equal(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.Equal(5.0, results.Duration, 1);
        Assert.Equal(0, results.ExitCode);
        Assert.Equal("Test\nNote: Test Note", results.Output);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Test", results.Lines[0].Text);
        Assert.Equal(RunLineType.Info, results.Lines[1].Type);
        Assert.Equal("Note: Test Note", results.Lines[1].Text);
    }

    /// <summary>
    /// Test QuestaSim simulator test with a warning message
    /// </summary>
    [Fact]
    public void QuestaSimSimulator_TestProcessor_WarningOutput_ReturnsWarningResult()
    {
        // Arrange: output containing the QuestaSim warning pattern
        // Act: parse a warning line
        var results = QuestaSimSimulator.TestProcessor.Parse(
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc),
            "Test\nWarning: Test Warning",
            0);

        // Assert: summary is Warning and the diagnostic line is classified as Warning
        Assert.Equal(RunLineType.Warning, results.Summary);
        Assert.Equal(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.Equal(5.0, results.Duration, 1);
        Assert.Equal(0, results.ExitCode);
        Assert.Equal("Test\nWarning: Test Warning", results.Output);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Test", results.Lines[0].Text);
        Assert.Equal(RunLineType.Warning, results.Lines[1].Type);
        Assert.Equal("Warning: Test Warning", results.Lines[1].Text);
    }

    /// <summary>
    /// Test QuestaSim simulator test with an error message
    /// </summary>
    [Fact]
    public void QuestaSimSimulator_TestProcessor_ErrorOutput_ReturnsErrorResult()
    {
        // Arrange: output containing the QuestaSim error pattern
        // Act: parse an error line
        var results = QuestaSimSimulator.TestProcessor.Parse(
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc),
            "Test\nError: Test Error",
            1);

        // Assert: summary is Error and the diagnostic line is classified as Error
        Assert.Equal(RunLineType.Error, results.Summary);
        Assert.Equal(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.Equal(5.0, results.Duration, 1);
        Assert.Equal(1, results.ExitCode);
        Assert.Equal("Test\nError: Test Error", results.Output);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Test", results.Lines[0].Text);
        Assert.Equal(RunLineType.Error, results.Lines[1].Type);
        Assert.Equal("Error: Test Error", results.Lines[1].Text);
    }

    /// <summary>
    /// Test QuestaSim simulator test with a failure message
    /// </summary>
    [Fact]
    public void QuestaSimSimulator_TestProcessor_FailureOutput_ReturnsErrorResult()
    {
        // Arrange: output containing the QuestaSim failure pattern
        // Act: parse a failure line
        var results = QuestaSimSimulator.TestProcessor.Parse(
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc),
            "Test\nFailure: Test Failure",
            1);

        // Assert: summary is Error and the diagnostic line is classified as Error
        Assert.Equal(RunLineType.Error, results.Summary);
        Assert.Equal(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.Equal(5.0, results.Duration, 1);
        Assert.Equal(1, results.ExitCode);
        Assert.Equal("Test\nFailure: Test Failure", results.Output);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Test", results.Lines[0].Text);
        Assert.Equal(RunLineType.Error, results.Lines[1].Type);
        Assert.Equal("Failure: Test Failure", results.Lines[1].Text);
    }

    /// <summary>
    ///     Verifies that calling Compile when QuestaSim is not installed throws
    ///     <see cref="InvalidOperationException"/> indicating the simulator is not available.
    ///     This test is skipped in environments where QuestaSim is installed.
    /// </summary>
    [Fact]
    public void QuestaSimSimulator_Compile_SimulatorNotAvailable_ThrowsInvalidOperationException()
    {
        // Skip this test when QuestaSim is available — we can only test the unavailable path
        // when SimulatorPath is null
        if (QuestaSimSimulator.Instance.Available())
        {
            Assert.Skip("QuestaSim is installed; unavailable-path test not applicable in this environment");
        }

        // Arrange: simulator is not available (SimulatorPath is null)
        using var context = Context.Create(["--silent"]);
        var options = new Options(Directory.GetCurrentDirectory(), new ConfigDocument());

        // Act / Assert: Compile throws when QuestaSim is not installed
        var ex = Assert.Throws<InvalidOperationException>(
            () => QuestaSimSimulator.Instance.Compile(context, options));
        Assert.Contains("QuestaSim Simulator not available", ex.Message);
    }

    /// <summary>
    ///     Verifies that calling Test when QuestaSim is not installed throws
    ///     <see cref="InvalidOperationException"/> with the expected message.
    ///     This test is skipped in environments where QuestaSim is installed.
    /// </summary>
    [Fact]
    public void QuestaSimSimulator_Test_SimulatorNotAvailable_ThrowsInvalidOperationException()
    {
        // Skip this test when QuestaSim is available — we can only test the unavailable path
        // when SimulatorPath is null
        if (QuestaSimSimulator.Instance.Available())
        {
            Assert.Skip("QuestaSim is installed; unavailable-path test not applicable in this environment");
        }

        // Arrange: simulator is not available (SimulatorPath is null)
        using var context = Context.Create(["--silent"]);
        var options = new Options(Directory.GetCurrentDirectory(), new ConfigDocument());

        // Act / Assert: Test throws when QuestaSim is not installed
        var ex = Assert.Throws<InvalidOperationException>(
            () => QuestaSimSimulator.Instance.Test(context, options, "test_tb"));
        Assert.Contains("QuestaSim Simulator not available", ex.Message);
    }

    /// <summary>
    ///     Verifies that FindPath returns the value of the VHDLTEST_QUESTASIM_PATH
    ///     environment variable when it is set.
    /// </summary>
    [Fact]
    public void QuestaSimSimulator_FindPath_WithEnvVar_ReturnsEnvVarValue()
    {
        // Arrange: set the VHDLTEST_QUESTASIM_PATH environment variable to a known value
        var expectedPath = "/custom/questasim/path";
        var previousValue = Environment.GetEnvironmentVariable("VHDLTEST_QUESTASIM_PATH");
        Environment.SetEnvironmentVariable("VHDLTEST_QUESTASIM_PATH", expectedPath);

        try
        {
            // Act: call FindPath()
            var result = QuestaSimSimulator.FindPath();

            // Assert: result is the env var value
            Assert.Equal(expectedPath, result);
        }
        finally
        {
            // Restore the environment variable
            Environment.SetEnvironmentVariable("VHDLTEST_QUESTASIM_PATH", previousValue);
        }
    }

    /// <summary>
    ///     Verifies that FindPath does not throw when VHDLTEST_QUESTASIM_PATH is not set.
    ///     Result is either a valid path (QuestaSim installed) or null (QuestaSim not installed).
    /// </summary>
    [Fact]
    public void QuestaSimSimulator_FindPath_WithoutEnvVar_ReturnsNullOrPath()
    {
        // Arrange: ensure VHDLTEST_QUESTASIM_PATH is not set for this test
        var previousValue = Environment.GetEnvironmentVariable("VHDLTEST_QUESTASIM_PATH");
        Environment.SetEnvironmentVariable("VHDLTEST_QUESTASIM_PATH", null);

        try
        {
            // Act: call FindPath() without the env var override
            var result = QuestaSimSimulator.FindPath();

            // Assert: result is either null (QuestaSim not installed) or a non-empty path string
            Assert.True(result == null || result.Length > 0);
        }
        finally
        {
            // Restore the environment variable
            Environment.SetEnvironmentVariable("VHDLTEST_QUESTASIM_PATH", previousValue);
        }
    }

    /// <summary>
    ///     Verifies that QuestaSimSimulator.Compile invokes vsim with do-script arguments.
    /// </summary>
    [Fact]
    public void QuestaSimSimulator_Compile_WithValidConfig_InvokesVsimWithDoScript()
    {
        // Arrange
        var invoker = new FakeProcessInvoker();
        var tempDir = Path.Combine(Path.GetTempPath(), $"vhdltest_{Path.GetRandomFileName()}");
        Directory.CreateDirectory(tempDir);
        try
        {
            var sim = QuestaSimSimulator.CreateForTesting(tempDir, invoker);
            using var context = Context.Create(["--silent"]);
            var options = new Options(tempDir, new ConfigDocument());

            // Act
            sim.Compile(context, options);

            // Assert: at least one invocation occurred
            Assert.True(invoker.AllCalls.Count > 0);
            var allArgs = invoker.AllCalls.SelectMany(c => c.Arguments).ToList();
            Assert.Contains("-c", allArgs);
            Assert.Contains("-do", allArgs);
            Assert.Contains("compile.do", allArgs);
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    /// <summary>
    ///     Verifies that QuestaSimSimulator.Compile writes a TCL script containing vcom -2008.
    /// </summary>
    [Fact]
    public void QuestaSimSimulator_Compile_WithValidConfig_ScriptContainsVcom2008()
    {
        // Arrange
        var invoker = new FakeProcessInvoker();
        var tempDir = Path.Combine(Path.GetTempPath(), $"vhdltest_{Path.GetRandomFileName()}");
        Directory.CreateDirectory(tempDir);
        try
        {
            var sim = QuestaSimSimulator.CreateForTesting(tempDir, invoker);
            using var context = Context.Create(["--silent"]);
            var config = new ConfigDocument { Files = ["test.vhd"] };
            var options = new Options(tempDir, config);

            // Act
            sim.Compile(context, options);

            // Assert: the generated compile script contains expected content
            var scriptPath = Path.Combine(tempDir, "VHDLTest.out", "QuestaSim", "compile.do");
            Assert.True(File.Exists(scriptPath));
            var content = File.ReadAllText(scriptPath);
            Assert.Contains("vcom -2008", content);
            Assert.Contains("exit -code 0", content);
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    /// <summary>
    ///     Verifies that QuestaSimSimulator.Test writes a TCL script with vsim and exit code.
    /// </summary>
    [Fact]
    public void QuestaSimSimulator_Test_WithValidConfig_ScriptContainsExitCode()
    {
        // Arrange
        var invoker = new FakeProcessInvoker();
        var tempDir = Path.Combine(Path.GetTempPath(), $"vhdltest_{Path.GetRandomFileName()}");
        Directory.CreateDirectory(tempDir);
        try
        {
            // Pre-create the QuestaSim output directory
            Directory.CreateDirectory(Path.Combine(tempDir, "VHDLTest.out", "QuestaSim"));

            var sim = QuestaSimSimulator.CreateForTesting(tempDir, invoker);
            using var context = Context.Create(["--silent"]);
            var options = new Options(tempDir, new ConfigDocument());

            // Act
            sim.Test(context, options, "my_tb");

            // Assert: the generated test script contains expected content
            var scriptPath = Path.Combine(tempDir, "VHDLTest.out", "QuestaSim", "test.do");
            Assert.True(File.Exists(scriptPath));
            var content = File.ReadAllText(scriptPath);
            Assert.Contains("vsim -quiet {my_tb}", content);
            Assert.Contains("exit -code 0", content);
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    /// <summary>
    ///     Verifies that a file path containing a space and a TCL metacharacter round-trips
    ///     correctly through Compile: it is written into compile.do inside a brace-quoted form,
    ///     and the compile invocation still succeeds (proving the quoting does not break the
    ///     surrounding invocation arguments).
    /// </summary>
    [Fact]
    public void QuestaSimSimulator_Compile_WithFileNameContainingSpaceAndMetacharacter_QuotesFileNameInScript()
    {
        // Arrange
        var invoker = new FakeProcessInvoker();
        var tempDir = Path.Combine(Path.GetTempPath(), $"vhdltest_{Path.GetRandomFileName()}");
        Directory.CreateDirectory(tempDir);
        try
        {
            var sim = QuestaSimSimulator.CreateForTesting(tempDir, invoker);
            using var context = Context.Create(["--silent"]);
            var config = new ConfigDocument { Files = ["my file [1].vhd"] };
            var options = new Options(tempDir, config);

            // Act
            sim.Compile(context, options);

            // Assert: the generated compile script contains the file name verbatim, brace-quoted
            var scriptPath = Path.Combine(tempDir, "VHDLTest.out", "QuestaSim", "compile.do");
            var content = File.ReadAllText(scriptPath);
            Assert.Contains("vcom -2008 {../../my file [1].vhd}", content);

            // Assert: the invocation's arguments still resolve correctly (round-trip proof)
            Assert.True(invoker.AllCalls.Count > 0);
            var allArgs = invoker.AllCalls.SelectMany(c => c.Arguments).ToList();
            Assert.Contains("compile.do", allArgs);
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    /// <summary>
    ///     Verifies that a test bench name containing a space and a TCL metacharacter round-trips
    ///     correctly through Test: it is written into test.do inside a brace-quoted form, and the
    ///     test invocation still succeeds (proving the quoting does not break the surrounding
    ///     invocation arguments).
    /// </summary>
    [Fact]
    public void QuestaSimSimulator_Test_WithTestNameContainingSpaceAndMetacharacter_QuotesTestNameInScript()
    {
        // Arrange
        var invoker = new FakeProcessInvoker();
        var tempDir = Path.Combine(Path.GetTempPath(), $"vhdltest_{Path.GetRandomFileName()}");
        Directory.CreateDirectory(tempDir);
        try
        {
            Directory.CreateDirectory(Path.Combine(tempDir, "VHDLTest.out", "QuestaSim"));

            var sim = QuestaSimSimulator.CreateForTesting(tempDir, invoker);
            using var context = Context.Create(["--silent"]);
            var options = new Options(tempDir, new ConfigDocument());

            // Act
            sim.Test(context, options, "lib.my tb");

            // Assert: the generated test script contains the test name verbatim, brace-quoted
            var scriptPath = Path.Combine(tempDir, "VHDLTest.out", "QuestaSim", "test.do");
            var content = File.ReadAllText(scriptPath);
            Assert.Contains("vsim -quiet {lib.my tb}", content);

            // Assert: the invocation's arguments still resolve correctly (round-trip proof)
            Assert.True(invoker.AllCalls.Count > 0);
            var allArgs = invoker.AllCalls.SelectMany(c => c.Arguments).ToList();
            Assert.Contains("test.do", allArgs);
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }
}
