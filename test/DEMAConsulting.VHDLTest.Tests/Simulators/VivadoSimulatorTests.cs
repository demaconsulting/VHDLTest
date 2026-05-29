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

using DEMAConsulting.VHDLTest.Run;
using DEMAConsulting.VHDLTest.Simulators;

namespace DEMAConsulting.VHDLTest.Tests.Simulators;

/// <summary>
/// Tests for Vivado simulator
/// </summary>
public class VivadoSimulatorTests
{
    /// <summary>
    /// Check name of Vivado simulator
    /// </summary>
    [Fact]
    public void VivadoSimulator_SimulatorName_ReturnsVivado()
    {
        // Act / Assert: simulator name is "Vivado"
        Assert.Equal("Vivado", VivadoSimulator.Instance.SimulatorName);
    }

    /// <summary>
    /// Test Vivado simulator compile with clean output
    /// </summary>
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
}
