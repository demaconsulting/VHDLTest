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

namespace DEMAConsulting.VHDLTest.Tests.Simulators;

/// <summary>
/// Tests for ModelSim simulator
/// </summary>
[Collection("SimulatorEnvVarTests")]
public class ModelSimSimulatorTests
{
    /// <summary>
    /// Check name of ModelSim simulator
    /// </summary>
    [Fact]
    public void ModelSimSimulator_SimulatorName_ReturnsModelSim()
    {
        // Act / Assert: simulator name is the fixed string "ModelSim"
        Assert.Equal("ModelSim", ModelSimSimulator.Instance.SimulatorName);
    }

    /// <summary>
    /// Test ModelSim simulator compile with clean output
    /// </summary>
    [Fact]
    public void ModelSimSimulator_CompileProcessor_CleanOutput_ReturnsTextResult()
    {
        // Arrange: output with no diagnostic patterns
        // Act: parse two plain-text lines
        var results = ModelSimSimulator.CompileProcessor.Parse(
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
    /// Test ModelSim simulator compile with an error message
    /// </summary>
    [Fact]
    public void ModelSimSimulator_CompileProcessor_ErrorOutput_ReturnsErrorResult()
    {
        // Arrange: output containing the ModelSim error pattern
        // Act: parse an error line
        var results = ModelSimSimulator.CompileProcessor.Parse(
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
    /// Test ModelSim simulator test with clean output
    /// </summary>
    [Fact]
    public void ModelSimSimulator_TestProcessor_CleanOutput_ReturnsTextResult()
    {
        // Arrange: output with no diagnostic patterns
        // Act: parse two plain-text lines
        var results = ModelSimSimulator.TestProcessor.Parse(
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
    /// Test ModelSim simulator test with an info message
    /// </summary>
    [Fact]
    public void ModelSimSimulator_TestProcessor_InfoOutput_ReturnsInfoResult()
    {
        // Arrange: output containing the ModelSim note pattern
        // Act: parse an info line
        var results = ModelSimSimulator.TestProcessor.Parse(
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
    /// Test ModelSim simulator test with a warning message
    /// </summary>
    [Fact]
    public void ModelSimSimulator_TestProcessor_WarningOutput_ReturnsWarningResult()
    {
        // Arrange: output containing the ModelSim warning pattern
        // Act: parse a warning line
        var results = ModelSimSimulator.TestProcessor.Parse(
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
    /// Test ModelSim simulator test with an error message
    /// </summary>
    [Fact]
    public void ModelSimSimulator_TestProcessor_ErrorOutput_ReturnsErrorResult()
    {
        // Arrange: output containing the ModelSim error pattern
        // Act: parse an error line
        var results = ModelSimSimulator.TestProcessor.Parse(
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
    /// Test ModelSim simulator test with a failure message
    /// </summary>
    [Fact]
    public void ModelSimSimulator_TestProcessor_FailureOutput_ReturnsErrorResult()
    {
        // Arrange: output containing the ModelSim failure pattern
        // Act: parse a failure line
        var results = ModelSimSimulator.TestProcessor.Parse(
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
    ///     Verifies that calling Compile when ModelSim is not installed throws
    ///     <see cref="InvalidOperationException"/> with the expected message.
    ///     This test is skipped in environments where ModelSim is installed.
    /// </summary>
    [Fact]
    public void ModelSimSimulator_Compile_SimulatorNotAvailable_ThrowsInvalidOperationException()
    {
        // Skip this test when ModelSim is available — we can only test the unavailable path
        // when SimulatorPath is null
        if (ModelSimSimulator.Instance.Available())
        {
            Assert.Skip("ModelSim is installed; unavailable-path test not applicable in this environment");
        }

        // Arrange: simulator is not available (SimulatorPath is null)
        using var context = Context.Create(["--silent"]);
        var options = new Options(Directory.GetCurrentDirectory(), new ConfigDocument());

        // Act / Assert: Compile throws when ModelSim is not installed
        var ex = Assert.Throws<InvalidOperationException>(
            () => ModelSimSimulator.Instance.Compile(context, options));
        Assert.Contains("ModelSim Simulator not available", ex.Message);
    }

    /// <summary>
    ///     Verifies that calling Test when ModelSim is not installed throws
    ///     <see cref="InvalidOperationException"/> with the expected message.
    ///     This test is skipped in environments where ModelSim is installed.
    /// </summary>
    [Fact]
    public void ModelSimSimulator_Test_SimulatorNotAvailable_ThrowsInvalidOperationException()
    {
        // Skip this test when ModelSim is available — we can only test the unavailable path
        // when SimulatorPath is null
        if (ModelSimSimulator.Instance.Available())
        {
            Assert.Skip("ModelSim is installed; unavailable-path test not applicable in this environment");
        }

        // Arrange: simulator is not available (SimulatorPath is null)
        using var context = Context.Create(["--silent"]);
        var options = new Options(Directory.GetCurrentDirectory(), new ConfigDocument());

        // Act / Assert: Test throws when ModelSim is not installed
        var ex = Assert.Throws<InvalidOperationException>(
            () => ModelSimSimulator.Instance.Test(context, options, "test_tb"));
        Assert.Contains("ModelSim Simulator not available", ex.Message);
    }

    /// <summary>
    ///     Verifies that FindPath returns the value of the VHDLTEST_MODELSIM_PATH
    ///     environment variable when it is set.
    /// </summary>
    [Fact]
    public void ModelSimSimulator_FindPath_WithEnvVar_ReturnsEnvVarValue()
    {
        // Arrange: set the VHDLTEST_MODELSIM_PATH environment variable to a known value
        var expectedPath = "/custom/modelsim/path";
        var previousValue = Environment.GetEnvironmentVariable("VHDLTEST_MODELSIM_PATH");
        Environment.SetEnvironmentVariable("VHDLTEST_MODELSIM_PATH", expectedPath);

        try
        {
            // Act: call FindPath()
            var result = ModelSimSimulator.FindPath();

            // Assert: result is the env var value
            Assert.Equal(expectedPath, result);
        }
        finally
        {
            // Restore the environment variable
            Environment.SetEnvironmentVariable("VHDLTEST_MODELSIM_PATH", previousValue);
        }
    }
}
