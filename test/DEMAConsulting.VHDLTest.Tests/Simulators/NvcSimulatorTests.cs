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
/// Tests for NVC simulator
/// </summary>
public class NvcSimulatorTests
{
    /// <summary>
    /// Check name of NVC simulator
    /// </summary>
    [Fact]
    public void NvcSimulator_SimulatorName_ReturnsNVC()
    {
        // Act / Assert: simulator name is the fixed string "NVC"
        Assert.Equal("NVC", NvcSimulator.Instance.SimulatorName);
    }

    /// <summary>
    /// Test NVC simulator compile with clean output
    /// </summary>
    [Fact]
    public void NvcSimulator_CompileProcessor_CleanOutput_ReturnsTextResult()
    {
        // Arrange: output with no diagnostic patterns
        // Act: parse two plain-text lines
        var results = NvcSimulator.CompileProcessor.Parse(
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
    /// Test NVC simulator compile with an info message
    /// </summary>
    [Fact]
    public void NvcSimulator_CompileProcessor_InfoOutput_ReturnsInfoResult()
    {
        // Arrange: output containing the NVC note pattern
        // Act: parse an info line
        var results = NvcSimulator.CompileProcessor.Parse(
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc),
            "Compile\nCompile Note: Compile Note",
            0);

        // Assert: summary is Info and the diagnostic line is classified as Info
        Assert.Equal(RunLineType.Info, results.Summary);
        Assert.Equal(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.Equal(5.0, results.Duration, 1);
        Assert.Equal(0, results.ExitCode);
        Assert.Equal("Compile\nCompile Note: Compile Note", results.Output);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Compile", results.Lines[0].Text);
        Assert.Equal(RunLineType.Info, results.Lines[1].Type);
        Assert.Equal("Compile Note: Compile Note", results.Lines[1].Text);
    }

    /// <summary>
    /// Test NVC simulator compile with a warning message
    /// </summary>
    [Fact]
    public void NvcSimulator_CompileProcessor_WarningOutput_ReturnsWarningResult()
    {
        // Arrange: output containing the NVC warning pattern
        // Act: parse a warning line
        var results = NvcSimulator.CompileProcessor.Parse(
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc),
            "Compile\nCompile Warning: Compile Warning",
            0);

        // Assert: summary is Warning and the diagnostic line is classified as Warning
        Assert.Equal(RunLineType.Warning, results.Summary);
        Assert.Equal(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.Equal(5.0, results.Duration, 1);
        Assert.Equal(0, results.ExitCode);
        Assert.Equal("Compile\nCompile Warning: Compile Warning", results.Output);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Compile", results.Lines[0].Text);
        Assert.Equal(RunLineType.Warning, results.Lines[1].Type);
        Assert.Equal("Compile Warning: Compile Warning", results.Lines[1].Text);
    }

    /// <summary>
    /// Test NVC simulator compile with an error message
    /// </summary>
    [Fact]
    public void NvcSimulator_CompileProcessor_ErrorOutput_ReturnsErrorResult()
    {
        // Arrange: output containing the NVC error pattern
        // Act: parse an error line
        var results = NvcSimulator.CompileProcessor.Parse(
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc),
            "Compile\nCompile Error: Compile Error",
            1);

        // Assert: summary is Error and the diagnostic line is classified as Error
        Assert.Equal(RunLineType.Error, results.Summary);
        Assert.Equal(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.Equal(5.0, results.Duration, 1);
        Assert.Equal(1, results.ExitCode);
        Assert.Equal("Compile\nCompile Error: Compile Error", results.Output);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Compile", results.Lines[0].Text);
        Assert.Equal(RunLineType.Error, results.Lines[1].Type);
        Assert.Equal("Compile Error: Compile Error", results.Lines[1].Text);
    }

    /// <summary>
    /// Test NVC simulator compile with a failure message
    /// </summary>
    [Fact]
    public void NvcSimulator_CompileProcessor_FailureOutput_ReturnsErrorResult()
    {
        // Arrange: output containing the NVC failure pattern
        // Act: parse a failure line
        var results = NvcSimulator.CompileProcessor.Parse(
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc),
            "Compile\nCompile Failure: Compile Failure",
            1);

        // Assert: summary is Error and the diagnostic line is classified as Error
        Assert.Equal(RunLineType.Error, results.Summary);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Compile", results.Lines[0].Text);
        Assert.Equal(RunLineType.Error, results.Lines[1].Type);
        Assert.Equal("Compile Failure: Compile Failure", results.Lines[1].Text);
    }

    /// <summary>
    /// Test NVC simulator compile with a fatal message
    /// </summary>
    [Fact]
    public void NvcSimulator_CompileProcessor_FatalOutput_ReturnsErrorResult()
    {
        // Arrange: output containing the NVC fatal pattern
        // Act: parse a fatal line
        var results = NvcSimulator.CompileProcessor.Parse(
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc),
            "Compile\nCompile Fatal: Compile Fatal",
            1);

        // Assert: summary is Error and the diagnostic line is classified as Error
        Assert.Equal(RunLineType.Error, results.Summary);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Compile", results.Lines[0].Text);
        Assert.Equal(RunLineType.Error, results.Lines[1].Type);
        Assert.Equal("Compile Fatal: Compile Fatal", results.Lines[1].Text);
    }

    /// <summary>
    /// Test NVC simulator test with clean output
    /// </summary>
    [Fact]
    public void NvcSimulator_TestProcessor_CleanOutput_ReturnsTextResult()
    {
        // Arrange: output with no diagnostic patterns
        // Act: parse two plain-text lines
        var results = NvcSimulator.TestProcessor.Parse(
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
    /// Test NVC simulator test with an info message
    /// </summary>
    [Fact]
    public void NvcSimulator_TestProcessor_InfoOutput_ReturnsInfoResult()
    {
        // Arrange: output containing the NVC note pattern
        // Act: parse an info line
        var results = NvcSimulator.TestProcessor.Parse(
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc),
            "Test\nTest Note: Test Note",
            0);

        // Assert: summary is Info and the diagnostic line is classified as Info
        Assert.Equal(RunLineType.Info, results.Summary);
        Assert.Equal(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.Equal(5.0, results.Duration, 1);
        Assert.Equal(0, results.ExitCode);
        Assert.Equal("Test\nTest Note: Test Note", results.Output);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Test", results.Lines[0].Text);
        Assert.Equal(RunLineType.Info, results.Lines[1].Type);
        Assert.Equal("Test Note: Test Note", results.Lines[1].Text);
    }

    /// <summary>
    /// Test NVC simulator test with a warning message
    /// </summary>
    [Fact]
    public void NvcSimulator_TestProcessor_WarningOutput_ReturnsWarningResult()
    {
        // Arrange: output containing the NVC warning pattern
        // Act: parse a warning line
        var results = NvcSimulator.TestProcessor.Parse(
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc),
            "Test\nTest Warning: Test Warning",
            0);

        // Assert: summary is Warning and the diagnostic line is classified as Warning
        Assert.Equal(RunLineType.Warning, results.Summary);
        Assert.Equal(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.Equal(5.0, results.Duration, 1);
        Assert.Equal(0, results.ExitCode);
        Assert.Equal("Test\nTest Warning: Test Warning", results.Output);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Test", results.Lines[0].Text);
        Assert.Equal(RunLineType.Warning, results.Lines[1].Type);
        Assert.Equal("Test Warning: Test Warning", results.Lines[1].Text);
    }

    /// <summary>
    /// Test NVC simulator test with an error message
    /// </summary>
    [Fact]
    public void NvcSimulator_TestProcessor_ErrorOutput_ReturnsErrorResult()
    {
        // Arrange: output containing the NVC error pattern
        // Act: parse an error line
        var results = NvcSimulator.TestProcessor.Parse(
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc),
            "Test\nTest Error: Test Error",
            1);

        // Assert: summary is Error and the diagnostic line is classified as Error
        Assert.Equal(RunLineType.Error, results.Summary);
        Assert.Equal(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.Equal(5.0, results.Duration, 1);
        Assert.Equal(1, results.ExitCode);
        Assert.Equal("Test\nTest Error: Test Error", results.Output);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Test", results.Lines[0].Text);
        Assert.Equal(RunLineType.Error, results.Lines[1].Type);
        Assert.Equal("Test Error: Test Error", results.Lines[1].Text);
    }

    /// <summary>
    /// Test NVC simulator test with a failure message
    /// </summary>
    [Fact]
    public void NvcSimulator_TestProcessor_FailureOutput_ReturnsErrorResult()
    {
        // Arrange: output containing the NVC failure pattern
        // Act: parse a failure line
        var results = NvcSimulator.TestProcessor.Parse(
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc),
            "Test\nTest Failure: Test Failure",
            1);

        // Assert: summary is Error and the diagnostic line is classified as Error
        Assert.Equal(RunLineType.Error, results.Summary);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Test", results.Lines[0].Text);
        Assert.Equal(RunLineType.Error, results.Lines[1].Type);
        Assert.Equal("Test Failure: Test Failure", results.Lines[1].Text);
    }

    /// <summary>
    /// Test NVC simulator test with a fatal message
    /// </summary>
    [Fact]
    public void NvcSimulator_TestProcessor_FatalOutput_ReturnsErrorResult()
    {
        // Arrange: output containing the NVC fatal pattern
        // Act: parse a fatal line
        var results = NvcSimulator.TestProcessor.Parse(
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc),
            "Test\nTest Fatal: Test Fatal",
            1);

        // Assert: summary is Error and the diagnostic line is classified as Error
        Assert.Equal(RunLineType.Error, results.Summary);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Test", results.Lines[0].Text);
        Assert.Equal(RunLineType.Error, results.Lines[1].Type);
        Assert.Equal("Test Fatal: Test Fatal", results.Lines[1].Text);
    }
}
