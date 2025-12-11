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
/// Tests for NVC simulator
/// </summary>
[TestClass]
public class NvcSimulatorTests
{
    /// <summary>
    /// Check name of NVC simulator
    /// </summary>
    [TestMethod]
    public void Test_NvcSimulator_Name()
    {
        Assert.AreEqual("NVC", NvcSimulator.Instance.SimulatorName);
    }

    /// <summary>
    /// Test NVC simulator compile with clean output
    /// </summary>
    [TestMethod]
    public void Test_NvcSimulator_Compile_Clean()
    {
        var results = NvcSimulator.CompileProcessor.Parse(
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
    /// Test NVC simulator compile with an info message
    /// </summary>
    [TestMethod]
    public void Test_NvcSimulator_Compile_Info()
    {
        var results = NvcSimulator.CompileProcessor.Parse(
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc),
            "Compile\nCompile Note: Compile Note",
            0);

        Assert.AreEqual(RunLineType.Info, results.Summary);
        Assert.AreEqual(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.AreEqual(5.0, results.Duration, 0.1);
        Assert.AreEqual(0, results.ExitCode);
        Assert.AreEqual("Compile\nCompile Note: Compile Note", results.Output);
        Assert.HasCount(2, results.Lines);
        Assert.AreEqual(RunLineType.Text, results.Lines[0].Type);
        Assert.AreEqual("Compile", results.Lines[0].Text);
        Assert.AreEqual(RunLineType.Info, results.Lines[1].Type);
        Assert.AreEqual("Compile Note: Compile Note", results.Lines[1].Text);
    }

    /// <summary>
    /// Test NVC simulator compile with a warning message
    /// </summary>
    [TestMethod]
    public void Test_NvcSimulator_Compile_Warning()
    {
        var results = NvcSimulator.CompileProcessor.Parse(
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc),
            "Compile\nCompile Warning: Compile Warning",
            0);

        Assert.AreEqual(RunLineType.Warning, results.Summary);
        Assert.AreEqual(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.AreEqual(5.0, results.Duration, 0.1);
        Assert.AreEqual(0, results.ExitCode);
        Assert.AreEqual("Compile\nCompile Warning: Compile Warning", results.Output);
        Assert.HasCount(2, results.Lines);
        Assert.AreEqual(RunLineType.Text, results.Lines[0].Type);
        Assert.AreEqual("Compile", results.Lines[0].Text);
        Assert.AreEqual(RunLineType.Warning, results.Lines[1].Type);
        Assert.AreEqual("Compile Warning: Compile Warning", results.Lines[1].Text);
    }

    /// <summary>
    /// Test NVC simulator compile with an error message
    /// </summary>
    [TestMethod]
    public void Test_NvcSimulator_Compile_Error()
    {
        var results = NvcSimulator.CompileProcessor.Parse(
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc),
            "Compile\nCompile Error: Compile Error",
            1);

        Assert.AreEqual(RunLineType.Error, results.Summary);
        Assert.AreEqual(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.AreEqual(5.0, results.Duration, 0.1);
        Assert.AreEqual(1, results.ExitCode);
        Assert.AreEqual("Compile\nCompile Error: Compile Error", results.Output);
        Assert.HasCount(2, results.Lines);
        Assert.AreEqual(RunLineType.Text, results.Lines[0].Type);
        Assert.AreEqual("Compile", results.Lines[0].Text);
        Assert.AreEqual(RunLineType.Error, results.Lines[1].Type);
        Assert.AreEqual("Compile Error: Compile Error", results.Lines[1].Text);
    }

    /// <summary>
    /// Test NVC simulator test with clean output
    /// </summary>
    [TestMethod]
    public void Test_NvcSimulator_Test_Clean()
    {
        var results = NvcSimulator.TestProcessor.Parse(
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
    /// Test NVC simulator test with an info message
    /// </summary>
    [TestMethod]
    public void Test_NvcSimulator_Test_Info()
    {
        var results = NvcSimulator.TestProcessor.Parse(
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc),
            "Test\nTest Note: Test Note",
            0);

        Assert.AreEqual(RunLineType.Info, results.Summary);
        Assert.AreEqual(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.AreEqual(5.0, results.Duration, 0.1);
        Assert.AreEqual(0, results.ExitCode);
        Assert.AreEqual("Test\nTest Note: Test Note", results.Output);
        Assert.HasCount(2, results.Lines);
        Assert.AreEqual(RunLineType.Text, results.Lines[0].Type);
        Assert.AreEqual("Test", results.Lines[0].Text);
        Assert.AreEqual(RunLineType.Info, results.Lines[1].Type);
        Assert.AreEqual("Test Note: Test Note", results.Lines[1].Text);
    }

    /// <summary>
    /// Test NVC simulator test with a warning message
    /// </summary>
    [TestMethod]
    public void Test_NvcSimulator_Test_Warning()
    {
        var results = NvcSimulator.TestProcessor.Parse(
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc),
            "Test\nTest Warning: Test Warning",
            0);

        Assert.AreEqual(RunLineType.Warning, results.Summary);
        Assert.AreEqual(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.AreEqual(5.0, results.Duration, 0.1);
        Assert.AreEqual(0, results.ExitCode);
        Assert.AreEqual("Test\nTest Warning: Test Warning", results.Output);
        Assert.HasCount(2, results.Lines);
        Assert.AreEqual(RunLineType.Text, results.Lines[0].Type);
        Assert.AreEqual("Test", results.Lines[0].Text);
        Assert.AreEqual(RunLineType.Warning, results.Lines[1].Type);
        Assert.AreEqual("Test Warning: Test Warning", results.Lines[1].Text);
    }

    /// <summary>
    /// Test NVC simulator test with an error message
    /// </summary>
    [TestMethod]
    public void Test_NvcSimulator_Test_Error()
    {
        var results = NvcSimulator.TestProcessor.Parse(
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc),
            "Test\nTest Error: Test Error",
            1);

        Assert.AreEqual(RunLineType.Error, results.Summary);
        Assert.AreEqual(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.AreEqual(5.0, results.Duration, 0.1);
        Assert.AreEqual(1, results.ExitCode);
        Assert.AreEqual("Test\nTest Error: Test Error", results.Output);
        Assert.HasCount(2, results.Lines);
        Assert.AreEqual(RunLineType.Text, results.Lines[0].Type);
        Assert.AreEqual("Test", results.Lines[0].Text);
        Assert.AreEqual(RunLineType.Error, results.Lines[1].Type);
        Assert.AreEqual("Test Error: Test Error", results.Lines[1].Text);
    }
}