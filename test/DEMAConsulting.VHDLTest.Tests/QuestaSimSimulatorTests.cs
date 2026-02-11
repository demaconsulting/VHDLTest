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

namespace DEMAConsulting.VHDLTest.Tests;

/// <summary>
/// Tests for QuestaSim simulator
/// </summary>
[TestClass]
public class QuestaSimSimulatorTests
{
    /// <summary>
    /// Check name of QuestaSim simulator
    /// </summary>
    [TestMethod]
    public void QuestaSimSimulator_SimulatorName_ReturnsQuestaSim()
    {
        Assert.AreEqual("QuestaSim", QuestaSimSimulator.Instance.SimulatorName);
    }

    /// <summary>
    /// Test QuestaSim simulator compile with clean output
    /// </summary>
    [TestMethod]
    public void QuestaSimSimulator_CompileProcessor_CleanOutput_ReturnsTextResult()
    {
        var results = QuestaSimSimulator.CompileProcessor.Parse(
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc),
            "Compile\nNo Issues",
            0);

        Assert.AreEqual(RunLineType.Text, results.Summary);
        Assert.AreEqual(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.AreEqual(5.0, results.Duration, 0.1);
        Assert.AreEqual(0, results.ExitCode);
        Assert.AreEqual("Compile\nNo Issues", results.Output);
        Assert.HasCount(2, results.Lines);
        Assert.AreEqual(RunLineType.Text, results.Lines[0].Type);
        Assert.AreEqual("Compile", results.Lines[0].Text);
        Assert.AreEqual(RunLineType.Text, results.Lines[1].Type);
        Assert.AreEqual("No Issues", results.Lines[1].Text);
    }

    /// <summary>
    /// Test QuestaSim simulator compile with an error message
    /// </summary>
    [TestMethod]
    public void QuestaSimSimulator_CompileProcessor_ErrorOutput_ReturnsErrorResult()
    {
        var results = QuestaSimSimulator.CompileProcessor.Parse(
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc),
            "Compile\nError: Compile Error",
            1);

        Assert.AreEqual(RunLineType.Error, results.Summary);
        Assert.AreEqual(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.AreEqual(5.0, results.Duration, 0.1);
        Assert.AreEqual(1, results.ExitCode);
        Assert.AreEqual("Compile\nError: Compile Error", results.Output);
        Assert.HasCount(2, results.Lines);
        Assert.AreEqual(RunLineType.Text, results.Lines[0].Type);
        Assert.AreEqual("Compile", results.Lines[0].Text);
        Assert.AreEqual(RunLineType.Error, results.Lines[1].Type);
        Assert.AreEqual("Error: Compile Error", results.Lines[1].Text);
    }

    /// <summary>
    /// Test QuestaSim simulator test with clean output
    /// </summary>
    [TestMethod]
    public void QuestaSimSimulator_TestProcessor_CleanOutput_ReturnsTextResult()
    {
        var results = QuestaSimSimulator.TestProcessor.Parse(
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc),
            "Test\nNo Issues",
            0);

        Assert.AreEqual(RunLineType.Text, results.Summary);
        Assert.AreEqual(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.AreEqual(5.0, results.Duration, 0.1);
        Assert.AreEqual(0, results.ExitCode);
        Assert.AreEqual("Test\nNo Issues", results.Output);
        Assert.HasCount(2, results.Lines);
        Assert.AreEqual(RunLineType.Text, results.Lines[0].Type);
        Assert.AreEqual("Test", results.Lines[0].Text);
        Assert.AreEqual(RunLineType.Text, results.Lines[1].Type);
        Assert.AreEqual("No Issues", results.Lines[1].Text);
    }

    /// <summary>
    /// Test QuestaSim simulator test with an info message
    /// </summary>
    [TestMethod]
    public void QuestaSimSimulator_TestProcessor_InfoOutput_ReturnsInfoResult()
    {
        var results = QuestaSimSimulator.TestProcessor.Parse(
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc),
            "Test\nNote: Test Note",
            0);

        Assert.AreEqual(RunLineType.Info, results.Summary);
        Assert.AreEqual(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.AreEqual(5.0, results.Duration, 0.1);
        Assert.AreEqual(0, results.ExitCode);
        Assert.AreEqual("Test\nNote: Test Note", results.Output);
        Assert.HasCount(2, results.Lines);
        Assert.AreEqual(RunLineType.Text, results.Lines[0].Type);
        Assert.AreEqual("Test", results.Lines[0].Text);
        Assert.AreEqual(RunLineType.Info, results.Lines[1].Type);
        Assert.AreEqual("Note: Test Note", results.Lines[1].Text);
    }

    /// <summary>
    /// Test QuestaSim simulator test with a warning message
    /// </summary>
    [TestMethod]
    public void QuestaSimSimulator_TestProcessor_WarningOutput_ReturnsWarningResult()
    {
        var results = QuestaSimSimulator.TestProcessor.Parse(
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc),
            "Test\nWarning: Test Warning",
            0);

        Assert.AreEqual(RunLineType.Warning, results.Summary);
        Assert.AreEqual(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.AreEqual(5.0, results.Duration, 0.1);
        Assert.AreEqual(0, results.ExitCode);
        Assert.AreEqual("Test\nWarning: Test Warning", results.Output);
        Assert.HasCount(2, results.Lines);
        Assert.AreEqual(RunLineType.Text, results.Lines[0].Type);
        Assert.AreEqual("Test", results.Lines[0].Text);
        Assert.AreEqual(RunLineType.Warning, results.Lines[1].Type);
        Assert.AreEqual("Warning: Test Warning", results.Lines[1].Text);
    }

    /// <summary>
    /// Test QuestaSim simulator test with an error message
    /// </summary>
    [TestMethod]
    public void QuestaSimSimulator_TestProcessor_ErrorOutput_ReturnsErrorResult()
    {
        var results = QuestaSimSimulator.TestProcessor.Parse(
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc),
            "Test\nError: Test Error",
            1);

        Assert.AreEqual(RunLineType.Error, results.Summary);
        Assert.AreEqual(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.AreEqual(5.0, results.Duration, 0.1);
        Assert.AreEqual(1, results.ExitCode);
        Assert.AreEqual("Test\nError: Test Error", results.Output);
        Assert.HasCount(2, results.Lines);
        Assert.AreEqual(RunLineType.Text, results.Lines[0].Type);
        Assert.AreEqual("Test", results.Lines[0].Text);
        Assert.AreEqual(RunLineType.Error, results.Lines[1].Type);
        Assert.AreEqual("Error: Test Error", results.Lines[1].Text);
    }

    /// <summary>
    /// Test QuestaSim simulator test with a failure message
    /// </summary>
    [TestMethod]
    public void QuestaSimSimulator_TestProcessor_FailureOutput_ReturnsErrorResult()
    {
        var results = QuestaSimSimulator.TestProcessor.Parse(
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc),
            "Test\nFailure: Test Failure",
            1);

        Assert.AreEqual(RunLineType.Error, results.Summary);
        Assert.AreEqual(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.AreEqual(5.0, results.Duration, 0.1);
        Assert.AreEqual(1, results.ExitCode);
        Assert.AreEqual("Test\nFailure: Test Failure", results.Output);
        Assert.HasCount(2, results.Lines);
        Assert.AreEqual(RunLineType.Text, results.Lines[0].Type);
        Assert.AreEqual("Test", results.Lines[0].Text);
        Assert.AreEqual(RunLineType.Error, results.Lines[1].Type);
        Assert.AreEqual("Failure: Test Failure", results.Lines[1].Text);
    }
}
