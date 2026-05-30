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

using System.Reflection;
using DEMAConsulting.VHDLTest.Cli;
using DEMAConsulting.VHDLTest.Run;
using DEMAConsulting.VHDLTest.Simulators;

namespace DEMAConsulting.VHDLTest.Tests.Simulators;

/// <summary>
/// Tests for GHDL simulator
/// </summary>
// All tests in this class are serialized via the SimulatorEnvVarTests collection because
// GhdlSimulator_FindPath_WithEnvVar_ReturnsEnvVarValue modifies the
// VHDLTEST_GHDL_PATH process-level environment variable.
// The DisableParallelization = true collection definition in SimulatorTestCollections.cs
// ensures these tests run sequentially with other env-var tests, preventing race conditions.
[Collection("SimulatorEnvVarTests")]
public class GhdlSimulatorTests
{
    /// <summary>
    /// Check name of GHDL simulator
    /// </summary>
    [Fact]
    public void GhdlSimulator_SimulatorName_ReturnsGHDL()
    {
        // Arrange: no setup required — GhdlSimulator.Instance is the pre-initialized singleton

        // Act / Assert: simulator name is the fixed string "GHDL"
        Assert.Equal("GHDL", GhdlSimulator.Instance.SimulatorName);
    }

    /// <summary>
    /// Test GHDL simulator compile with clean output
    /// </summary>
    [Fact]
    public void GhdlSimulator_CompileProcessor_CleanOutput_ReturnsTextResult()
    {
        // Arrange: output with no diagnostic patterns
        // Act: parse two plain-text lines
        var results = GhdlSimulator.CompileProcessor.Parse(
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
    /// Test GHDL simulator compile with a warning message
    /// </summary>
    [Fact]
    public void GhdlSimulator_CompileProcessor_WarningOutput_ReturnsWarningResult()
    {
        // Arrange: output containing the GHDL warning pattern (file:line:col:warning:)
        // Act: parse a warning line
        var results = GhdlSimulator.CompileProcessor.Parse(
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc),
            "Compile\nCompile:1:1:warning: Compile Warning",
            0);

        // Assert: summary is Warning and the diagnostic line is classified as Warning
        Assert.Equal(RunLineType.Warning, results.Summary);
        Assert.Equal(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.Equal(5.0, results.Duration, 1);
        Assert.Equal(0, results.ExitCode);
        Assert.Equal("Compile\nCompile:1:1:warning: Compile Warning", results.Output);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Compile", results.Lines[0].Text);
        Assert.Equal(RunLineType.Warning, results.Lines[1].Type);
        Assert.Equal("Compile:1:1:warning: Compile Warning", results.Lines[1].Text);
    }

    /// <summary>
    /// Test GHDL simulator compile with an error message using the :error: pattern
    /// </summary>
    [Fact]
    public void GhdlSimulator_CompileProcessor_ErrorOutput_ReturnsErrorResult()
    {
        // Arrange: output containing the GHDL :error: pattern
        // Act: parse an error line
        var results = GhdlSimulator.CompileProcessor.Parse(
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc),
            "Compile\nCompile:error: Compile Error",
            1);

        // Assert: summary is Error and the diagnostic line is classified as Error
        Assert.Equal(RunLineType.Error, results.Summary);
        Assert.Equal(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.Equal(5.0, results.Duration, 1);
        Assert.Equal(1, results.ExitCode);
        Assert.Equal("Compile\nCompile:error: Compile Error", results.Output);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Compile", results.Lines[0].Text);
        Assert.Equal(RunLineType.Error, results.Lines[1].Type);
        Assert.Equal("Compile:error: Compile Error", results.Lines[1].Text);
    }

    /// <summary>
    /// Test GHDL simulator compile with a line/column error message (file:line:col: pattern)
    /// </summary>
    [Fact]
    public void GhdlSimulator_CompileProcessor_LineColError_ReturnsErrorResult()
    {
        // Arrange: output containing the GHDL line/column error pattern (file:line:col: message)
        // Act: parse a line/column error line
        var results = GhdlSimulator.CompileProcessor.Parse(
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc),
            "Compile\ntest.vhd:10:5: error: undefined identifier",
            1);

        // Assert: summary is Error and the diagnostic line is classified as Error
        Assert.Equal(RunLineType.Error, results.Summary);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Compile", results.Lines[0].Text);
        Assert.Equal(RunLineType.Error, results.Lines[1].Type);
        Assert.Equal("test.vhd:10:5: error: undefined identifier", results.Lines[1].Text);
    }

    /// <summary>
    /// Test GHDL simulator compile with a cannot-open error message
    /// </summary>
    [Fact]
    public void GhdlSimulator_CompileProcessor_CannotOpenError_ReturnsErrorResult()
    {
        // Arrange: output containing the GHDL "cannot open" pattern
        // Act: parse a cannot-open error line
        var results = GhdlSimulator.CompileProcessor.Parse(
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc),
            "Compile\nmissing.vhd: cannot open",
            1);

        // Assert: summary is Error and the diagnostic line is classified as Error
        Assert.Equal(RunLineType.Error, results.Summary);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Compile", results.Lines[0].Text);
        Assert.Equal(RunLineType.Error, results.Lines[1].Type);
        Assert.Equal("missing.vhd: cannot open", results.Lines[1].Text);
    }

    /// <summary>
    /// Test GHDL simulator test with clean output
    /// </summary>
    [Fact]
    public void GhdlSimulator_TestProcessor_CleanOutput_ReturnsTextResult()
    {
        // Arrange: output with no diagnostic patterns
        // Act: parse two plain-text lines
        var results = GhdlSimulator.TestProcessor.Parse(
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
    /// Test GHDL simulator test with an info message (report note pattern)
    /// </summary>
    [Fact]
    public void GhdlSimulator_TestProcessor_InfoOutput_ReturnsInfoResult()
    {
        // Arrange: output containing the GHDL report note pattern
        // Act: parse an info line
        var results = GhdlSimulator.TestProcessor.Parse(
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc),
            "Test\nTest:(report note): Test Note",
            0);

        // Assert: summary is Info and the diagnostic line is classified as Info
        Assert.Equal(RunLineType.Info, results.Summary);
        Assert.Equal(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.Equal(5.0, results.Duration, 1);
        Assert.Equal(0, results.ExitCode);
        Assert.Equal("Test\nTest:(report note): Test Note", results.Output);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Test", results.Lines[0].Text);
        Assert.Equal(RunLineType.Info, results.Lines[1].Type);
        Assert.Equal("Test:(report note): Test Note", results.Lines[1].Text);
    }

    /// <summary>
    /// Test GHDL simulator test with an assertion note info message
    /// </summary>
    [Fact]
    public void GhdlSimulator_TestProcessor_AssertionNoteOutput_ReturnsInfoResult()
    {
        // Arrange: output containing the GHDL assertion note pattern
        // Act: parse an assertion note line
        var results = GhdlSimulator.TestProcessor.Parse(
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc),
            "Test\nTest:(assertion note): Assertion Note",
            0);

        // Assert: summary is Info and the diagnostic line is classified as Info
        Assert.Equal(RunLineType.Info, results.Summary);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Test", results.Lines[0].Text);
        Assert.Equal(RunLineType.Info, results.Lines[1].Type);
        Assert.Equal("Test:(assertion note): Assertion Note", results.Lines[1].Text);
    }

    /// <summary>
    /// Test GHDL simulator test with a warning message (report warning pattern)
    /// </summary>
    [Fact]
    public void GhdlSimulator_TestProcessor_WarningOutput_ReturnsWarningResult()
    {
        // Arrange: output containing the GHDL report warning pattern
        // Act: parse a warning line
        var results = GhdlSimulator.TestProcessor.Parse(
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc),
            "Test\nTest:(report warning): Test Warning",
            0);

        // Assert: summary is Warning and the diagnostic line is classified as Warning
        Assert.Equal(RunLineType.Warning, results.Summary);
        Assert.Equal(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.Equal(5.0, results.Duration, 1);
        Assert.Equal(0, results.ExitCode);
        Assert.Equal("Test\nTest:(report warning): Test Warning", results.Output);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Test", results.Lines[0].Text);
        Assert.Equal(RunLineType.Warning, results.Lines[1].Type);
        Assert.Equal("Test:(report warning): Test Warning", results.Lines[1].Text);
    }

    /// <summary>
    /// Test GHDL simulator test with an assertion warning message
    /// </summary>
    [Fact]
    public void GhdlSimulator_TestProcessor_AssertionWarningOutput_ReturnsWarningResult()
    {
        // Arrange: output containing the GHDL assertion warning pattern
        // Act: parse an assertion warning line
        var results = GhdlSimulator.TestProcessor.Parse(
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc),
            "Test\nTest:(assertion warning): Assertion Warning",
            0);

        // Assert: summary is Warning and the diagnostic line is classified as Warning
        Assert.Equal(RunLineType.Warning, results.Summary);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Test", results.Lines[0].Text);
        Assert.Equal(RunLineType.Warning, results.Lines[1].Type);
        Assert.Equal("Test:(assertion warning): Assertion Warning", results.Lines[1].Text);
    }

    /// <summary>
    /// Test GHDL simulator test with an error message (report error pattern)
    /// </summary>
    [Fact]
    public void GhdlSimulator_TestProcessor_ErrorOutput_ReturnsErrorResult()
    {
        // Arrange: output containing the GHDL report error pattern
        // Act: parse an error line
        var results = GhdlSimulator.TestProcessor.Parse(
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc),
            "Test\nTest:(report error): Test Error",
            1);

        // Assert: summary is Error and the diagnostic line is classified as Error
        Assert.Equal(RunLineType.Error, results.Summary);
        Assert.Equal(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.Equal(5.0, results.Duration, 1);
        Assert.Equal(1, results.ExitCode);
        Assert.Equal("Test\nTest:(report error): Test Error", results.Output);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Test", results.Lines[0].Text);
        Assert.Equal(RunLineType.Error, results.Lines[1].Type);
        Assert.Equal("Test:(report error): Test Error", results.Lines[1].Text);
    }

    /// <summary>
    /// Test GHDL simulator test with an assertion error message
    /// </summary>
    [Fact]
    public void GhdlSimulator_TestProcessor_AssertionErrorOutput_ReturnsErrorResult()
    {
        // Arrange: output containing the GHDL assertion error pattern
        // Act: parse an assertion error line
        var results = GhdlSimulator.TestProcessor.Parse(
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc),
            "Test\nTest:(assertion error): Assertion Error",
            1);

        // Assert: summary is Error and the diagnostic line is classified as Error
        Assert.Equal(RunLineType.Error, results.Summary);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Test", results.Lines[0].Text);
        Assert.Equal(RunLineType.Error, results.Lines[1].Type);
        Assert.Equal("Test:(assertion error): Assertion Error", results.Lines[1].Text);
    }

    /// <summary>
    /// Test GHDL simulator test with an assertion failure message
    /// </summary>
    [Fact]
    public void GhdlSimulator_TestProcessor_AssertionFailureOutput_ReturnsErrorResult()
    {
        // Arrange: output containing the GHDL assertion failure pattern
        // Act: parse an assertion failure line
        var results = GhdlSimulator.TestProcessor.Parse(
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc),
            "Test\nTest:(assertion failure): Assertion Failure",
            1);

        // Assert: summary is Error and the diagnostic line is classified as Error
        Assert.Equal(RunLineType.Error, results.Summary);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Test", results.Lines[0].Text);
        Assert.Equal(RunLineType.Error, results.Lines[1].Type);
        Assert.Equal("Test:(assertion failure): Assertion Failure", results.Lines[1].Text);
    }

    /// <summary>
    /// Test GHDL simulator test with a report failure message
    /// </summary>
    [Fact]
    public void GhdlSimulator_TestProcessor_ReportFailureOutput_ReturnsErrorResult()
    {
        // Arrange: output containing the GHDL report failure pattern
        // Act: parse a report failure line
        var results = GhdlSimulator.TestProcessor.Parse(
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc),
            "Test\nTest:(report failure): Report Failure",
            1);

        // Assert: summary is Error and the diagnostic line is classified as Error
        Assert.Equal(RunLineType.Error, results.Summary);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Test", results.Lines[0].Text);
        Assert.Equal(RunLineType.Error, results.Lines[1].Type);
        Assert.Equal("Test:(report failure): Report Failure", results.Lines[1].Text);
    }

    /// <summary>
    /// Test GHDL simulator test with a :error: pattern message
    /// </summary>
    [Fact]
    public void GhdlSimulator_TestProcessor_ColonErrorOutput_ReturnsErrorResult()
    {
        // Arrange: output containing the GHDL :error: pattern
        // Act: parse a colon-error line
        var results = GhdlSimulator.TestProcessor.Parse(
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc),
            "Test\nTest:error: Test Error",
            1);

        // Assert: summary is Error and the diagnostic line is classified as Error
        Assert.Equal(RunLineType.Error, results.Summary);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Test", results.Lines[0].Text);
        Assert.Equal(RunLineType.Error, results.Lines[1].Type);
        Assert.Equal("Test:error: Test Error", results.Lines[1].Text);
    }

    /// <summary>
    /// Test that FindPath returns the env var value when VHDLTEST_GHDL_PATH is set.
    /// </summary>
    [Fact]
    public void GhdlSimulator_FindPath_WithEnvVar_ReturnsEnvVarValue()
    {
        // Arrange: set the VHDLTEST_GHDL_PATH environment variable to a known value
        var expectedPath = "/custom/ghdl/path";
        var previousValue = Environment.GetEnvironmentVariable("VHDLTEST_GHDL_PATH");
        Environment.SetEnvironmentVariable("VHDLTEST_GHDL_PATH", expectedPath);

        try
        {
            // Act: call FindPath()
            var result = GhdlSimulator.FindPath();

            // Assert: result is the env var value
            Assert.Equal(expectedPath, result);
        }
        finally
        {
            // Restore the environment variable
            Environment.SetEnvironmentVariable("VHDLTEST_GHDL_PATH", previousValue);
        }
    }

    /// <summary>
    /// Test that FindPath does not throw when the GHDL env var is not set.
    /// Result is either a valid path (GHDL installed) or null (GHDL not installed).
    /// </summary>
    [Fact]
    public void GhdlSimulator_FindPath_WithoutEnvVar_ReturnsNullOrPath()
    {
        // Arrange: ensure VHDLTEST_GHDL_PATH is not set for this test
        var previousValue = Environment.GetEnvironmentVariable("VHDLTEST_GHDL_PATH");
        Environment.SetEnvironmentVariable("VHDLTEST_GHDL_PATH", null);

        try
        {
            // Act: call FindPath() without the env var override
            var result = GhdlSimulator.FindPath();

            // Assert: result is either null (GHDL not installed) or a non-empty path string
            Assert.True(result == null || result.Length > 0);
        }
        finally
        {
            // Restore the environment variable
            Environment.SetEnvironmentVariable("VHDLTEST_GHDL_PATH", previousValue);
        }
    }

    /// <summary>
    ///     Validates that <see cref="GhdlSimulator.Compile"/> throws
    ///     <see cref="InvalidOperationException"/> when the simulator is not available
    ///     (i.e., <see cref="Simulator.SimulatorPath"/> is <c>null</c>).
    /// </summary>
    [Fact]
    public void GhdlSimulator_Compile_WhenNotAvailable_ThrowsInvalidOperationException()
    {
        // Arrange: create a fresh GhdlSimulator instance with a null SimulatorPath by
        // temporarily clearing the GHDL env var and using a PATH that contains no GHDL
        // executable, so FindPath() returns null during construction.
        var previousGhdlPath = Environment.GetEnvironmentVariable("VHDLTEST_GHDL_PATH");
        var previousPath = Environment.GetEnvironmentVariable("PATH");
        Environment.SetEnvironmentVariable("VHDLTEST_GHDL_PATH", null);
        Environment.SetEnvironmentVariable("PATH", Path.GetTempPath());

        GhdlSimulator simulator;
        try
        {
            // Use reflection to invoke the private constructor so FindPath() returns null
            var ctor = typeof(GhdlSimulator)
                .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
                .Single(c => c.GetParameters().Length == 0);
            simulator = (GhdlSimulator)ctor.Invoke([]);
        }
        finally
        {
            // Restore environment before running the actual assertion
            Environment.SetEnvironmentVariable("VHDLTEST_GHDL_PATH", previousGhdlPath);
            Environment.SetEnvironmentVariable("PATH", previousPath);
        }

        // Verify the constructed instance has a null SimulatorPath (GHDL not found)
        if (simulator.SimulatorPath != null)
        {
            // Skip the assertion: GHDL was found in the temp directory (unexpected but possible)
            Assert.Skip("GHDL executable found in temp directory; cannot construct unavailable simulator instance.");
        }

        using var context = Context.Create(["--silent"]);
        var options = new Options(Directory.GetCurrentDirectory(), new ConfigDocument());

        // Act / Assert: Compile throws when GHDL is not installed
        var ex = Assert.Throws<InvalidOperationException>(() => simulator.Compile(context, options));
        Assert.Equal("GHDL Simulator not available", ex.Message);
    }

    /// <summary>
    ///     Validates that <see cref="GhdlSimulator.Test"/> throws
    ///     <see cref="InvalidOperationException"/> when the simulator is not available
    ///     (i.e., <see cref="Simulator.SimulatorPath"/> is <c>null</c>).
    /// </summary>
    [Fact]
    public void GhdlSimulator_Test_WhenNotAvailable_ThrowsInvalidOperationException()
    {
        // Arrange: create a fresh GhdlSimulator instance with a null SimulatorPath by
        // temporarily clearing the GHDL env var and using a PATH that contains no GHDL
        // executable, so FindPath() returns null during construction.
        var previousGhdlPath = Environment.GetEnvironmentVariable("VHDLTEST_GHDL_PATH");
        var previousPath = Environment.GetEnvironmentVariable("PATH");
        Environment.SetEnvironmentVariable("VHDLTEST_GHDL_PATH", null);
        Environment.SetEnvironmentVariable("PATH", Path.GetTempPath());

        GhdlSimulator simulator;
        try
        {
            // Use reflection to invoke the private constructor so FindPath() returns null
            var ctor = typeof(GhdlSimulator)
                .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
                .Single(c => c.GetParameters().Length == 0);
            simulator = (GhdlSimulator)ctor.Invoke([]);
        }
        finally
        {
            // Restore environment before running the actual assertion
            Environment.SetEnvironmentVariable("VHDLTEST_GHDL_PATH", previousGhdlPath);
            Environment.SetEnvironmentVariable("PATH", previousPath);
        }

        // Verify the constructed instance has a null SimulatorPath (GHDL not found)
        if (simulator.SimulatorPath != null)
        {
            // Skip the assertion: GHDL was found in the temp directory (unexpected but possible)
            Assert.Skip("GHDL executable found in temp directory; cannot construct unavailable simulator instance.");
        }

        using var context = Context.Create(["--silent"]);
        var options = new Options(Directory.GetCurrentDirectory(), new ConfigDocument());

        // Act / Assert: Test throws when GHDL is not installed
        var ex = Assert.Throws<InvalidOperationException>(() => simulator.Test(context, options, "test_tb"));
        Assert.Equal("GHDL Simulator not available", ex.Message);
    }
}
