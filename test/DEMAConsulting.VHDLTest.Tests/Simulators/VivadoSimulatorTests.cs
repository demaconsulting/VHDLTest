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
/// Tests for Vivado simulator
/// </summary>
/// <remarks>
///     Exercises the compile and test processor instances by calling <c>Parse</c> with
///     pre-captured output strings — no live Vivado installation is required. Env-var mutation
///     in <c>VivadoSimulator_FindPath_WithEnvVar_ReturnsEnvVarValue</c> is serialized through
///     the <c>SimulatorEnvVarTests</c> collection to prevent race conditions with other
///     env-var tests running in parallel.
/// </remarks>
[Collection("SimulatorEnvVarTests")]
public class VivadoSimulatorTests
{
    /// <summary>
    /// Check name of Vivado simulator
    /// </summary>
    /// <remarks>
    ///     Confirms that the registered simulator name matches the expected factory key "Vivado",
    ///     ensuring the simulator is discoverable by the factory and reported correctly in output.
    /// </remarks>
    [Fact]
    public void VivadoSimulator_SimulatorName_ReturnsVivado()
    {
        // Act / Assert: simulator name is "Vivado"
        Assert.Equal("Vivado", VivadoSimulator.Instance.SimulatorName);
    }

    /// <summary>
    /// Test Vivado simulator compile with clean output
    /// </summary>
    /// <remarks>
    ///     Validates the base-case path through the compile processor classification rules — when
    ///     xvhdl produces no error markers, all lines remain Text and the summary is Text.
    /// </remarks>
    [Fact]
    public void VivadoSimulator_CompileProcessor_CleanOutput_ReturnsTextResult()
    {
        // Arrange: define clean compile output with no severity markers
        var start = new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc);

        // Act: parse the output through the compile processor
        var results = VivadoSimulator.CompileProcessor.Parse(
            start,
            end,
            "Compile\nNo Issues",
            0);

        // Assert: summary is Text and all lines are classified correctly
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
    /// Test Vivado simulator compile with an error message
    /// </summary>
    /// <remarks>
    ///     Validates that lines prefixed with "Error:" are correctly escalated to Error severity,
    ///     confirming the compile processor correctly signals xvhdl compilation failures.
    /// </remarks>
    [Fact]
    public void VivadoSimulator_CompileProcessor_ErrorOutput_ReturnsErrorResult()
    {
        // Arrange: define compile output containing an Error marker
        var start = new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc);

        // Act: parse the output through the compile processor
        var results = VivadoSimulator.CompileProcessor.Parse(
            start,
            end,
            "Compile\nError: Compile Error",
            1);

        // Assert: summary is Error and the error line is classified correctly
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
    /// Test Vivado simulator test with clean output
    /// </summary>
    /// <remarks>
    ///     Validates the base-case path through the test processor classification rules — when
    ///     xelab produces no severity markers, all lines remain Text and the summary is Text.
    /// </remarks>
    [Fact]
    public void VivadoSimulator_TestProcessor_CleanOutput_ReturnsTextResult()
    {
        // Arrange: define clean test output with no severity markers
        var start = new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc);

        // Act: parse the output through the test processor
        var results = VivadoSimulator.TestProcessor.Parse(
            start,
            end,
            "Test\nNo Issues",
            0);

        // Assert: summary is Text and all lines are classified correctly
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
    /// Test Vivado simulator test with an info message
    /// </summary>
    /// <remarks>
    ///     Validates that lines prefixed with "Note:" are classified as Info severity, confirming
    ///     the test processor surfaces informational messages from xelab simulation output.
    /// </remarks>
    [Fact]
    public void VivadoSimulator_TestProcessor_InfoOutput_ReturnsInfoResult()
    {
        // Arrange: define test output containing a Note marker
        var start = new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc);

        // Act: parse the output through the test processor
        var results = VivadoSimulator.TestProcessor.Parse(
            start,
            end,
            "Test\nNote: Test Note",
            0);

        // Assert: summary is Info and the note line is classified correctly
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
    /// Test Vivado simulator test with a warning message
    /// </summary>
    /// <remarks>
    ///     Validates that lines prefixed with "Warning:" are classified as Warning severity,
    ///     confirming the test processor surfaces warnings from xelab simulation output.
    /// </remarks>
    [Fact]
    public void VivadoSimulator_TestProcessor_WarningOutput_ReturnsWarningResult()
    {
        // Arrange: define test output containing a Warning marker
        var start = new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc);

        // Act: parse the output through the test processor
        var results = VivadoSimulator.TestProcessor.Parse(
            start,
            end,
            "Test\nWarning: Test Warning",
            0);

        // Assert: summary is Warning and the warning line is classified correctly
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
    /// Test Vivado simulator test with an error message
    /// </summary>
    /// <remarks>
    ///     Validates that lines prefixed with "Error:" are classified as Error severity in test
    ///     output, confirming the test processor correctly signals simulation errors from xelab.
    /// </remarks>
    [Fact]
    public void VivadoSimulator_TestProcessor_ErrorOutput_ReturnsErrorResult()
    {
        // Arrange: define test output containing an Error marker
        var start = new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc);

        // Act: parse the output through the test processor
        var results = VivadoSimulator.TestProcessor.Parse(
            start,
            end,
            "Test\nError: Test Error",
            1);

        // Assert: summary is Error and the error line is classified correctly
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
    /// Test Vivado simulator test with a failure message
    /// </summary>
    /// <remarks>
    ///     Validates that lines prefixed with "Failure:" are classified as Error severity,
    ///     confirming that VHDL assertion failures in xelab simulation output are escalated to Error.
    /// </remarks>
    [Fact]
    public void VivadoSimulator_TestProcessor_FailureOutput_ReturnsErrorResult()
    {
        // Arrange: define test output containing a Failure marker
        var start = new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc);

        // Act: parse the output through the test processor
        var results = VivadoSimulator.TestProcessor.Parse(start, end, "Test\nFailure: Test Failure", 1);

        // Assert: summary is Error and the failure line is classified as Error
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
    ///     Verifies that calling Compile when Vivado is not installed throws
    ///     <see cref="InvalidOperationException"/> with the expected message.
    ///     This test is skipped in environments where Vivado is installed.
    /// </summary>
    [Fact]
    public void VivadoSimulator_Compile_WhenSimulatorNotAvailable_ThrowsInvalidOperationException()
    {
        // Skip this test when Vivado is available — we can only test the unavailable path
        // when SimulatorPath is null
        if (VivadoSimulator.Instance.Available())
        {
            Assert.Skip("Vivado is installed; unavailable-path test not applicable in this environment");
        }

        // Arrange: simulator is not available (SimulatorPath is null)
        using var context = Context.Create(["--silent"]);
        var options = new Options(Directory.GetCurrentDirectory(), new ConfigDocument());

        // Act / Assert: Compile throws when Vivado is not installed
        var ex = Assert.Throws<InvalidOperationException>(
            () => VivadoSimulator.Instance.Compile(context, options));
        Assert.Equal("Vivado Simulator not available", ex.Message);
    }

    /// <summary>
    ///     Verifies that calling Test when Vivado is not installed throws
    ///     <see cref="InvalidOperationException"/> with the expected message.
    ///     This test is skipped in environments where Vivado is installed.
    /// </summary>
    [Fact]
    public void VivadoSimulator_Test_WhenSimulatorNotAvailable_ThrowsInvalidOperationException()
    {
        // Skip this test when Vivado is available — we can only test the unavailable path
        // when SimulatorPath is null
        if (VivadoSimulator.Instance.Available())
        {
            Assert.Skip("Vivado is installed; unavailable-path test not applicable in this environment");
        }

        // Arrange: simulator is not available (SimulatorPath is null)
        using var context = Context.Create(["--silent"]);
        var options = new Options(Directory.GetCurrentDirectory(), new ConfigDocument());

        // Act / Assert: Test throws when Vivado is not installed
        var ex = Assert.Throws<InvalidOperationException>(
            () => VivadoSimulator.Instance.Test(context, options, "test_tb"));
        Assert.Equal("Vivado Simulator not available", ex.Message);
    }

    /// <summary>
    ///     Verifies that FindPath returns the value of the VHDLTEST_VIVADO_PATH
    ///     environment variable when it is set.
    /// </summary>
    [Fact]
    public void VivadoSimulator_FindPath_WithEnvVar_ReturnsEnvVarValue()
    {
        // Arrange: set the VHDLTEST_VIVADO_PATH environment variable to a known value
        var expectedPath = "/custom/vivado/path";
        var previousValue = Environment.GetEnvironmentVariable("VHDLTEST_VIVADO_PATH");
        Environment.SetEnvironmentVariable("VHDLTEST_VIVADO_PATH", expectedPath);

        try
        {
            // Act: call FindPath()
            var result = VivadoSimulator.FindPath();

            // Assert: result is the env var value
            Assert.Equal(expectedPath, result);
        }
        finally
        {
            // Restore the environment variable
            Environment.SetEnvironmentVariable("VHDLTEST_VIVADO_PATH", previousValue);
        }
    }

    /// <summary>
    ///     Verifies that FindPath does not throw when VHDLTEST_VIVADO_PATH is not set.
    ///     Result is either a valid path (Vivado installed) or null (Vivado not installed).
    /// </summary>
    [Fact]
    public void VivadoSimulator_FindPath_WithoutEnvVar_ReturnsNullOrPath()
    {
        // Arrange: ensure VHDLTEST_VIVADO_PATH is not set for this test
        var previousValue = Environment.GetEnvironmentVariable("VHDLTEST_VIVADO_PATH");
        Environment.SetEnvironmentVariable("VHDLTEST_VIVADO_PATH", null);

        try
        {
            // Act: call FindPath() without the env var override
            var result = VivadoSimulator.FindPath();

            // Assert: result is either null (Vivado not installed) or a non-empty path string
            Assert.True(result == null || result.Length > 0);
        }
        finally
        {
            // Restore the environment variable
            Environment.SetEnvironmentVariable("VHDLTEST_VIVADO_PATH", previousValue);
        }
    }

    /// <summary>
    ///     Verifies that VivadoSimulator.Compile invokes xvhdl with argument-file arguments.
    /// </summary>
    [Fact]
    public void VivadoSimulator_Compile_WithValidConfig_InvokesXvhdlWithArgumentFile()
    {
        // Arrange
        var invoker = new FakeProcessInvoker();
        var tempDir = Path.Combine(Path.GetTempPath(), $"vhdltest_{Path.GetRandomFileName()}");
        Directory.CreateDirectory(tempDir);
        try
        {
            var sim = VivadoSimulator.CreateForTesting(tempDir, invoker);
            using var context = Context.Create(["--silent"]);
            var options = new Options(tempDir, new ConfigDocument());

            // Act
            sim.Compile(context, options);

            // Assert: at least one invocation occurred
            Assert.True(invoker.AllCalls.Count > 0);
            var allArgs = invoker.AllCalls.SelectMany(c => c.Arguments).ToList();
            Assert.Contains("-file", allArgs);
            Assert.Contains("compile.do", allArgs);
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    /// <summary>
    ///     Verifies that VivadoSimulator.Compile writes an argument file containing -2008 and -nolog.
    /// </summary>
    [Fact]
    public void VivadoSimulator_Compile_WithValidConfig_ScriptContains2008AndNolog()
    {
        // Arrange
        var invoker = new FakeProcessInvoker();
        var tempDir = Path.Combine(Path.GetTempPath(), $"vhdltest_{Path.GetRandomFileName()}");
        Directory.CreateDirectory(tempDir);
        try
        {
            var sim = VivadoSimulator.CreateForTesting(tempDir, invoker);
            using var context = Context.Create(["--silent"]);
            var options = new Options(tempDir, new ConfigDocument());

            // Act
            sim.Compile(context, options);

            // Assert: the generated compile script contains expected content
            var scriptPath = Path.Combine(tempDir, "VHDLTest.out", "Vivado", "compile.do");
            Assert.True(File.Exists(scriptPath));
            var content = File.ReadAllText(scriptPath);
            Assert.Contains("-2008", content);
            Assert.Contains("-nolog", content);
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    /// <summary>
    ///     Verifies that VivadoSimulator.Test writes an argument file containing the test name.
    /// </summary>
    [Fact]
    public void VivadoSimulator_Test_WithValidConfig_ScriptContainsTestName()
    {
        // Arrange
        var invoker = new FakeProcessInvoker();
        var tempDir = Path.Combine(Path.GetTempPath(), $"vhdltest_{Path.GetRandomFileName()}");
        Directory.CreateDirectory(tempDir);
        try
        {
            // Pre-create the Vivado output directory
            Directory.CreateDirectory(Path.Combine(tempDir, "VHDLTest.out", "Vivado"));

            var sim = VivadoSimulator.CreateForTesting(tempDir, invoker);
            using var context = Context.Create(["--silent"]);
            var options = new Options(tempDir, new ConfigDocument());

            // Act
            sim.Test(context, options, "my_tb");

            // Assert: the generated test script contains expected content
            var scriptPath = Path.Combine(tempDir, "VHDLTest.out", "Vivado", "test.do");
            Assert.True(File.Exists(scriptPath));
            var content = File.ReadAllText(scriptPath);
            Assert.Contains("-nolog", content);
            Assert.Contains("-standalone", content);
            Assert.Contains("my_tb", content);
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }
}
