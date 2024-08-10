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

[TestClass]
public class TestActiveHdlSimulator
{
    /// <summary>
    /// Check name of ActiveHDL simulator
    /// </summary>
    [TestMethod]
    public void Test_ActiveHdlSimulator_Name()
    {
        Assert.AreEqual("ActiveHdl", ActiveHdlSimulator.Instance.SimulatorName);
    }

    [TestMethod]
    public void Test_ActiveHdlSimulator_Compile_Clean()
    {
        var results = ActiveHdlSimulator.CompileProcessor.Parse(
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc),
            "Compile\nNo Issues",
            0);

        Assert.AreEqual(RunLineType.Text, results.Summary);
        Assert.AreEqual(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.AreEqual(5.0, results.Duration, 0.1);
        Assert.AreEqual(0, results.ExitCode);
        Assert.AreEqual("Compile\nNo Issues", results.Output);
        Assert.AreEqual(2, results.Lines.Count);
        Assert.AreEqual(RunLineType.Text, results.Lines[0].Type);
        Assert.AreEqual("Compile", results.Lines[0].Text);
        Assert.AreEqual(RunLineType.Text, results.Lines[1].Type);
        Assert.AreEqual("No Issues", results.Lines[1].Text);
    }

    [TestMethod]
    public void Test_ActiveHdlSimulator_Compile_Warning()
    {
        var results = ActiveHdlSimulator.CompileProcessor.Parse(
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc),
            "Compile\nKERNEL: Warning: Compile Warning",
            0);

        Assert.AreEqual(RunLineType.Warning, results.Summary);
        Assert.AreEqual(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.AreEqual(5.0, results.Duration, 0.1);
        Assert.AreEqual(0, results.ExitCode);
        Assert.AreEqual("Compile\nKERNEL: Warning: Compile Warning", results.Output);
        Assert.AreEqual(2, results.Lines.Count);
        Assert.AreEqual(RunLineType.Text, results.Lines[0].Type);
        Assert.AreEqual("Compile", results.Lines[0].Text);
        Assert.AreEqual(RunLineType.Warning, results.Lines[1].Type);
        Assert.AreEqual("KERNEL: Warning: Compile Warning", results.Lines[1].Text);
    }

    [TestMethod]
    public void Test_ActiveHdlSimulator_Compile_Error()
    {
        var results = ActiveHdlSimulator.CompileProcessor.Parse(
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc),
            "Compile\nKERNEL: Fatal Error: Compile Error",
            1);

        Assert.AreEqual(RunLineType.Error, results.Summary);
        Assert.AreEqual(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.AreEqual(5.0, results.Duration, 0.1);
        Assert.AreEqual(1, results.ExitCode);
        Assert.AreEqual("Compile\nKERNEL: Fatal Error: Compile Error", results.Output);
        Assert.AreEqual(2, results.Lines.Count);
        Assert.AreEqual(RunLineType.Text, results.Lines[0].Type);
        Assert.AreEqual("Compile", results.Lines[0].Text);
        Assert.AreEqual(RunLineType.Error, results.Lines[1].Type);
        Assert.AreEqual("KERNEL: Fatal Error: Compile Error", results.Lines[1].Text);
    }

    [TestMethod]
    public void Test_ActiveHdlSimulator_Test_Clean()
    {
        var results = ActiveHdlSimulator.TestProcessor.Parse(
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc),
            "Test\nNo Issues",
            0);

        Assert.AreEqual(RunLineType.Text, results.Summary);
        Assert.AreEqual(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.AreEqual(5.0, results.Duration, 0.1);
        Assert.AreEqual(0, results.ExitCode);
        Assert.AreEqual("Test\nNo Issues", results.Output);
        Assert.AreEqual(2, results.Lines.Count);
        Assert.AreEqual(RunLineType.Text, results.Lines[0].Type);
        Assert.AreEqual("Test", results.Lines[0].Text);
        Assert.AreEqual(RunLineType.Text, results.Lines[1].Type);
        Assert.AreEqual("No Issues", results.Lines[1].Text);
    }

    [TestMethod]
    public void Test_ActiveHdlSimulator_Test_Info()
    {
        var results = ActiveHdlSimulator.TestProcessor.Parse(
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc),
            "Test\nEXECUTION:: NOTE Test Note",
            0);

        Assert.AreEqual(RunLineType.Info, results.Summary);
        Assert.AreEqual(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.AreEqual(5.0, results.Duration, 0.1);
        Assert.AreEqual(0, results.ExitCode);
        Assert.AreEqual("Test\nEXECUTION:: NOTE Test Note", results.Output);
        Assert.AreEqual(2, results.Lines.Count);
        Assert.AreEqual(RunLineType.Text, results.Lines[0].Type);
        Assert.AreEqual("Test", results.Lines[0].Text);
        Assert.AreEqual(RunLineType.Info, results.Lines[1].Type);
        Assert.AreEqual("EXECUTION:: NOTE Test Note", results.Lines[1].Text);
    }

    [TestMethod]
    public void Test_ActiveHdlSimulator_Test_Warning()
    {
        var results = ActiveHdlSimulator.TestProcessor.Parse(
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc),
            "Test\nEXECUTION:: WARNING Test Warning",
            0);

        Assert.AreEqual(RunLineType.Warning, results.Summary);
        Assert.AreEqual(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.AreEqual(5.0, results.Duration, 0.1);
        Assert.AreEqual(0, results.ExitCode);
        Assert.AreEqual("Test\nEXECUTION:: WARNING Test Warning", results.Output);
        Assert.AreEqual(2, results.Lines.Count);
        Assert.AreEqual(RunLineType.Text, results.Lines[0].Type);
        Assert.AreEqual("Test", results.Lines[0].Text);
        Assert.AreEqual(RunLineType.Warning, results.Lines[1].Type);
        Assert.AreEqual("EXECUTION:: WARNING Test Warning", results.Lines[1].Text);
    }

    [TestMethod]
    public void Test_ActiveHdlSimulator_Test_Error()
    {
        var results = ActiveHdlSimulator.TestProcessor.Parse(
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc),
            "Test\nEXECUTION:: ERROR Test Error",
            1);

        Assert.AreEqual(RunLineType.Error, results.Summary);
        Assert.AreEqual(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.AreEqual(5.0, results.Duration, 0.1);
        Assert.AreEqual(1, results.ExitCode);
        Assert.AreEqual("Test\nEXECUTION:: ERROR Test Error", results.Output);
        Assert.AreEqual(2, results.Lines.Count);
        Assert.AreEqual(RunLineType.Text, results.Lines[0].Type);
        Assert.AreEqual("Test", results.Lines[0].Text);
        Assert.AreEqual(RunLineType.Error, results.Lines[1].Type);
        Assert.AreEqual("EXECUTION:: ERROR Test Error", results.Lines[1].Text);
    }
}