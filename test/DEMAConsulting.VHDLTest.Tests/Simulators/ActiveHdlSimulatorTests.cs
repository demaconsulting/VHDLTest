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
///     Tests for the ActiveHDL simulator
/// </summary>
/// <remarks>
///     Tests in this class are serialized via the <c>SimulatorEnvVarTests</c> collection because
///     <c>ActiveHdlSimulator_FindPath_WithEnvVar_ReturnsEnvVarValue</c> modifies the
///     <c>VHDLTEST_ACTIVEHDL_PATH</c> process-level environment variable, requiring serialization
///     to prevent race conditions with other environment-variable tests running in parallel.
/// </remarks>
[Collection("SimulatorEnvVarTests")]
public class ActiveHdlSimulatorTests
{
    /// <summary>
    ///     Check name of ActiveHDL simulator name
    /// </summary>
    [Fact]
    public void ActiveHdlSimulator_SimulatorName_ReturnsActiveHdl()
    {
        // Arrange: N/A - uses the pre-initialized singleton instance

        // Act / Assert: simulator name is "ActiveHdl"
        Assert.Equal("ActiveHdl", ActiveHdlSimulator.Instance.SimulatorName);
    }

    /// <summary>
    ///     Test ActiveHDL simulator compile with clean output
    /// </summary>
    [Fact]
    public void ActiveHdlSimulator_CompileProcessor_CleanOutput_ReturnsTextResult()
    {
        // Arrange: define clean compile output with no severity markers
        var start = new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc);

        // Act: parse the output through the compile processor
        var results = ActiveHdlSimulator.CompileProcessor.Parse(
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
    ///     Test ActiveHDL simulator compile with a warning message
    /// </summary>
    [Fact]
    public void ActiveHdlSimulator_CompileProcessor_WarningOutput_ReturnsWarningResult()
    {
        // Arrange: define compile output containing a KERNEL Warning marker
        var start = new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc);

        // Act: parse the output through the compile processor
        var results = ActiveHdlSimulator.CompileProcessor.Parse(
            start,
            end,
            "Compile\nKERNEL: Warning: Compile Warning",
            0);

        // Assert: summary is Warning and the warning line is classified correctly
        Assert.Equal(RunLineType.Warning, results.Summary);
        Assert.Equal(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.Equal(5.0, results.Duration, 1);
        Assert.Equal(0, results.ExitCode);
        Assert.Equal("Compile\nKERNEL: Warning: Compile Warning", results.Output);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Compile", results.Lines[0].Text);
        Assert.Equal(RunLineType.Warning, results.Lines[1].Type);
        Assert.Equal("KERNEL: Warning: Compile Warning", results.Lines[1].Text);
    }

    /// <summary>
    ///     Test ActiveHDL simulator compile with an error message
    /// </summary>
    [Fact]
    public void ActiveHdlSimulator_CompileProcessor_ErrorOutput_ReturnsErrorResult()
    {
        // Arrange: define compile output containing an Error marker
        var start = new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc);

        // Act: parse the output through the compile processor
        var results = ActiveHdlSimulator.CompileProcessor.Parse(
            start,
            end,
            "Compile\nKERNEL: Fatal Error: Compile Error",
            1);

        // Assert: summary is Error and the error line is classified correctly
        Assert.Equal(RunLineType.Error, results.Summary);
        Assert.Equal(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.Equal(5.0, results.Duration, 1);
        Assert.Equal(1, results.ExitCode);
        Assert.Equal("Compile\nKERNEL: Fatal Error: Compile Error", results.Output);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Compile", results.Lines[0].Text);
        Assert.Equal(RunLineType.Error, results.Lines[1].Type);
        Assert.Equal("KERNEL: Fatal Error: Compile Error", results.Lines[1].Text);
    }

    /// <summary>
    ///     Test ActiveHDL simulator compile with a fatal runtime error message
    /// </summary>
    [Fact]
    public void ActiveHdlSimulator_CompileProcessor_FatalRuntimeError_ReturnsErrorResult()
    {
        // Arrange: define compile output with a RUNTIME Fatal Error marker
        var start = new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc);

        // Act: parse the output through the compile processor
        var results = ActiveHdlSimulator.CompileProcessor.Parse(
            start, end, "Compile\nRUNTIME: Fatal Error Compile Error", 1);

        // Assert: summary is Error and the error line is classified correctly
        Assert.Equal(RunLineType.Error, results.Summary);
        Assert.Equal(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.Equal(5.0, results.Duration, 1);
        Assert.Equal(1, results.ExitCode);
        Assert.Equal("Compile\nRUNTIME: Fatal Error Compile Error", results.Output);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Compile", results.Lines[0].Text);
        Assert.Equal(RunLineType.Error, results.Lines[1].Type);
        Assert.Equal("RUNTIME: Fatal Error Compile Error", results.Lines[1].Text);
    }

    /// <summary>
    ///     Test ActiveHDL simulator test with clean output
    /// </summary>
    [Fact]
    public void ActiveHdlSimulator_TestProcessor_CleanOutput_ReturnsTextResult()
    {
        // Arrange: define clean test output with no severity markers
        var start = new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc);

        // Act: parse the output through the test processor
        var results = ActiveHdlSimulator.TestProcessor.Parse(
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
    ///     Test ActiveHDL simulator test with an info message
    /// </summary>
    [Fact]
    public void ActiveHdlSimulator_TestProcessor_InfoOutput_ReturnsInfoResult()
    {
        // Arrange: define test output containing an EXECUTION NOTE marker
        var start = new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc);

        // Act: parse the output through the test processor
        var results = ActiveHdlSimulator.TestProcessor.Parse(
            start,
            end,
            "Test\nEXECUTION:: NOTE Test Note",
            0);

        // Assert: summary is Info and the note line is classified correctly
        Assert.Equal(RunLineType.Info, results.Summary);
        Assert.Equal(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.Equal(5.0, results.Duration, 1);
        Assert.Equal(0, results.ExitCode);
        Assert.Equal("Test\nEXECUTION:: NOTE Test Note", results.Output);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Test", results.Lines[0].Text);
        Assert.Equal(RunLineType.Info, results.Lines[1].Type);
        Assert.Equal("EXECUTION:: NOTE Test Note", results.Lines[1].Text);
    }

    /// <summary>
    ///     Test ActiveHDL simulator test with a warning message
    /// </summary>
    [Fact]
    public void ActiveHdlSimulator_TestProcessor_WarningOutput_ReturnsWarningResult()
    {
        // Arrange: define test output containing an EXECUTION WARNING marker
        var start = new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc);

        // Act: parse the output through the test processor
        var results = ActiveHdlSimulator.TestProcessor.Parse(
            start,
            end,
            "Test\nEXECUTION:: WARNING Test Warning",
            0);

        // Assert: summary is Warning and the warning line is classified correctly
        Assert.Equal(RunLineType.Warning, results.Summary);
        Assert.Equal(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.Equal(5.0, results.Duration, 1);
        Assert.Equal(0, results.ExitCode);
        Assert.Equal("Test\nEXECUTION:: WARNING Test Warning", results.Output);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Test", results.Lines[0].Text);
        Assert.Equal(RunLineType.Warning, results.Lines[1].Type);
        Assert.Equal("EXECUTION:: WARNING Test Warning", results.Lines[1].Text);
    }

    /// <summary>
    ///     Test ActiveHDL simulator test with an error message
    /// </summary>
    [Fact]
    public void ActiveHdlSimulator_TestProcessor_ErrorOutput_ReturnsErrorResult()
    {
        // Arrange: define test output containing an EXECUTION ERROR marker
        var start = new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc);

        // Act: parse the output through the test processor
        var results = ActiveHdlSimulator.TestProcessor.Parse(
            start,
            end,
            "Test\nEXECUTION:: ERROR Test Error",
            1);

        // Assert: summary is Error and the error line is classified correctly
        Assert.Equal(RunLineType.Error, results.Summary);
        Assert.Equal(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.Equal(5.0, results.Duration, 1);
        Assert.Equal(1, results.ExitCode);
        Assert.Equal("Test\nEXECUTION:: ERROR Test Error", results.Output);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Test", results.Lines[0].Text);
        Assert.Equal(RunLineType.Error, results.Lines[1].Type);
        Assert.Equal("EXECUTION:: ERROR Test Error", results.Lines[1].Text);
    }

    /// <summary>
    ///     Test ActiveHDL simulator test that the first Lattice Edition warning is suppressed to Text
    /// </summary>
    [Fact]
    public void ActiveHdlSimulator_TestProcessor_LatticeSuppression1_ReturnsTextResult()
    {
        // Arrange: define test output with the first Lattice Edition suppression marker
        var start = new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc);

        // Act: parse the output through the test processor
        var results = ActiveHdlSimulator.TestProcessor.Parse(
            start, end, "Test\nKERNEL: Warning: You are using the Active-HDL Lattice Edition", 0);

        // Assert: summary is Text (suppressed) and the line is classified as Text
        Assert.Equal(RunLineType.Text, results.Summary);
        Assert.Equal(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.Equal(5.0, results.Duration, 1);
        Assert.Equal(0, results.ExitCode);
        Assert.Equal("Test\nKERNEL: Warning: You are using the Active-HDL Lattice Edition", results.Output);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Test", results.Lines[0].Text);
        Assert.Equal(RunLineType.Text, results.Lines[1].Type);
        Assert.Equal("KERNEL: Warning: You are using the Active-HDL Lattice Edition", results.Lines[1].Text);
    }

    /// <summary>
    ///     Test ActiveHDL simulator test that the second Lattice Edition warning is suppressed to Text
    /// </summary>
    [Fact]
    public void ActiveHdlSimulator_TestProcessor_LatticeSuppression2_ReturnsTextResult()
    {
        // Arrange: define test output with the second Lattice Edition suppression marker
        var start = new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc);

        // Act: parse the output through the test processor
        var results = ActiveHdlSimulator.TestProcessor.Parse(
            start, end, "Test\nKERNEL: Warning: Contact Aldec for available upgrade options", 0);

        // Assert: summary is Text (suppressed) and the line is classified as Text
        Assert.Equal(RunLineType.Text, results.Summary);
        Assert.Equal(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.Equal(5.0, results.Duration, 1);
        Assert.Equal(0, results.ExitCode);
        Assert.Equal("Test\nKERNEL: Warning: Contact Aldec for available upgrade options", results.Output);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Test", results.Lines[0].Text);
        Assert.Equal(RunLineType.Text, results.Lines[1].Type);
        Assert.Equal("KERNEL: Warning: Contact Aldec for available upgrade options", results.Lines[1].Text);
    }

    /// <summary>
    ///     Test ActiveHDL simulator test with a KERNEL Warning message
    /// </summary>
    [Fact]
    public void ActiveHdlSimulator_TestProcessor_KernelWarning_ReturnsWarningResult()
    {
        // Arrange: define test output with a KERNEL Warning marker
        var start = new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc);

        // Act: parse the output through the test processor
        var results = ActiveHdlSimulator.TestProcessor.Parse(
            start, end, "Test\nKERNEL: Warning: Test Warning", 0);

        // Assert: summary is Warning and the warning line is classified correctly
        Assert.Equal(RunLineType.Warning, results.Summary);
        Assert.Equal(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.Equal(5.0, results.Duration, 1);
        Assert.Equal(0, results.ExitCode);
        Assert.Equal("Test\nKERNEL: Warning: Test Warning", results.Output);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Test", results.Lines[0].Text);
        Assert.Equal(RunLineType.Warning, results.Lines[1].Type);
        Assert.Equal("KERNEL: Warning: Test Warning", results.Lines[1].Text);
    }

    /// <summary>
    ///     Test ActiveHDL simulator test with a KERNEL WARNING (uppercase) message
    /// </summary>
    [Fact]
    public void ActiveHdlSimulator_TestProcessor_KernelWarningUpper_ReturnsWarningResult()
    {
        // Arrange: define test output with a KERNEL WARNING (uppercase) marker
        var start = new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc);

        // Act: parse the output through the test processor
        var results = ActiveHdlSimulator.TestProcessor.Parse(
            start, end, "Test\nKERNEL: WARNING: Test Warning", 0);

        // Assert: summary is Warning and the warning line is classified correctly
        Assert.Equal(RunLineType.Warning, results.Summary);
        Assert.Equal(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.Equal(5.0, results.Duration, 1);
        Assert.Equal(0, results.ExitCode);
        Assert.Equal("Test\nKERNEL: WARNING: Test Warning", results.Output);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Test", results.Lines[0].Text);
        Assert.Equal(RunLineType.Warning, results.Lines[1].Type);
        Assert.Equal("KERNEL: WARNING: Test Warning", results.Lines[1].Text);
    }

    /// <summary>
    ///     Test ActiveHDL simulator test with an EXECUTION FAILURE message
    /// </summary>
    [Fact]
    public void ActiveHdlSimulator_TestProcessor_ExecutionFailure_ReturnsErrorResult()
    {
        // Arrange: define test output with an EXECUTION FAILURE marker
        var start = new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc);

        // Act: parse the output through the test processor
        var results = ActiveHdlSimulator.TestProcessor.Parse(
            start, end, "Test\nEXECUTION:: FAILURE Test Failure", 1);

        // Assert: summary is Error and the failure line is classified as Error
        Assert.Equal(RunLineType.Error, results.Summary);
        Assert.Equal(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.Equal(5.0, results.Duration, 1);
        Assert.Equal(1, results.ExitCode);
        Assert.Equal("Test\nEXECUTION:: FAILURE Test Failure", results.Output);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Test", results.Lines[0].Text);
        Assert.Equal(RunLineType.Error, results.Lines[1].Type);
        Assert.Equal("EXECUTION:: FAILURE Test Failure", results.Lines[1].Text);
    }

    /// <summary>
    ///     Test ActiveHDL simulator test with a KERNEL ERROR message
    /// </summary>
    [Fact]
    public void ActiveHdlSimulator_TestProcessor_KernelError_ReturnsErrorResult()
    {
        // Arrange: define test output with a KERNEL ERROR marker
        var start = new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc);

        // Act: parse the output through the test processor
        var results = ActiveHdlSimulator.TestProcessor.Parse(
            start, end, "Test\nKERNEL: ERROR Test Error", 1);

        // Assert: summary is Error and the error line is classified correctly
        Assert.Equal(RunLineType.Error, results.Summary);
        Assert.Equal(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.Equal(5.0, results.Duration, 1);
        Assert.Equal(1, results.ExitCode);
        Assert.Equal("Test\nKERNEL: ERROR Test Error", results.Output);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Test", results.Lines[0].Text);
        Assert.Equal(RunLineType.Error, results.Lines[1].Type);
        Assert.Equal("KERNEL: ERROR Test Error", results.Lines[1].Text);
    }

    /// <summary>
    ///     Test ActiveHDL simulator test with a RUNTIME Fatal Error message
    /// </summary>
    [Fact]
    public void ActiveHdlSimulator_TestProcessor_RuntimeFatalError_ReturnsErrorResult()
    {
        // Arrange: define test output with a RUNTIME Fatal Error marker
        var start = new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc);

        // Act: parse the output through the test processor
        var results = ActiveHdlSimulator.TestProcessor.Parse(
            start, end, "Test\nRUNTIME: Fatal Error: Test Error", 1);

        // Assert: summary is Error and the error line is classified correctly
        Assert.Equal(RunLineType.Error, results.Summary);
        Assert.Equal(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.Equal(5.0, results.Duration, 1);
        Assert.Equal(1, results.ExitCode);
        Assert.Equal("Test\nRUNTIME: Fatal Error: Test Error", results.Output);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Test", results.Lines[0].Text);
        Assert.Equal(RunLineType.Error, results.Lines[1].Type);
        Assert.Equal("RUNTIME: Fatal Error: Test Error", results.Lines[1].Text);
    }

    /// <summary>
    ///     Test ActiveHDL simulator test with a VSIM Error message
    /// </summary>
    [Fact]
    public void ActiveHdlSimulator_TestProcessor_VsimError_ReturnsErrorResult()
    {
        // Arrange: define test output with a VSIM Error marker
        var start = new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc);

        // Act: parse the output through the test processor
        var results = ActiveHdlSimulator.TestProcessor.Parse(
            start, end, "Test\nVSIM: Error: Test Error", 1);

        // Assert: summary is Error and the error line is classified correctly
        Assert.Equal(RunLineType.Error, results.Summary);
        Assert.Equal(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.Equal(5.0, results.Duration, 1);
        Assert.Equal(1, results.ExitCode);
        Assert.Equal("Test\nVSIM: Error: Test Error", results.Output);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Test", results.Lines[0].Text);
        Assert.Equal(RunLineType.Error, results.Lines[1].Type);
        Assert.Equal("VSIM: Error: Test Error", results.Lines[1].Text);
    }

    /// <summary>
    ///     Verifies that the TCL test script written by ActiveHdlSimulator.Test contains
    ///     <c>exit -code 0</c> to signal successful simulation completion to vsimsa.
    /// </summary>
    [Fact]
    public void ActiveHdlSimulator_Test_WithCleanConfig_AppendsTclExitCode()
    {
        // Arrange
        var invoker = new FakeProcessInvoker();
        var tempDir = Path.Combine(Path.GetTempPath(), $"vhdltest_{Path.GetRandomFileName()}");
        Directory.CreateDirectory(tempDir);
        try
        {
            // Pre-create the ActiveHDL output directory
            Directory.CreateDirectory(Path.Combine(tempDir, "VHDLTest.out", "ActiveHDL"));

            var sim = ActiveHdlSimulator.CreateForTesting(tempDir, invoker);
            using var context = Context.Create(["--silent"]);
            var options = new Options(tempDir, new ConfigDocument());

            // Act
            sim.Test(context, options, "my_tb");

            // Assert: the generated TCL test script contains "exit -code 0"
            var scriptPath = Path.Combine(tempDir, "VHDLTest.out", "ActiveHDL", "test.do");
            Assert.True(File.Exists(scriptPath), "TCL test script file was not created");
            var content = File.ReadAllText(scriptPath);
            Assert.Contains("exit -code 0", content);
            Assert.Contains("asim {my_tb}", content);
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    /// <summary>
    ///     Verifies that <see cref="ActiveHdlSimulator.FindPath"/> returns the value of the
    ///     <c>VHDLTEST_ACTIVEHDL_PATH</c> environment variable when it is set.
    /// </summary>
    [Fact]
    public void ActiveHdlSimulator_FindPath_WithEnvVar_ReturnsEnvVarValue()
    {
        // Arrange: set the VHDLTEST_ACTIVEHDL_PATH environment variable to a known value
        var expectedPath = "/custom/activehdl/path";
        var previousValue = Environment.GetEnvironmentVariable("VHDLTEST_ACTIVEHDL_PATH");
        Environment.SetEnvironmentVariable("VHDLTEST_ACTIVEHDL_PATH", expectedPath);

        try
        {
            // Act: call FindPath()
            var result = ActiveHdlSimulator.FindPath();

            // Assert: result is the env var value
            Assert.Equal(expectedPath, result);
        }
        finally
        {
            // Restore the environment variable
            Environment.SetEnvironmentVariable("VHDLTEST_ACTIVEHDL_PATH", previousValue);
        }
    }

    /// <summary>
    ///     Verifies that <see cref="ActiveHdlSimulator.FindPath"/> does not throw when
    ///     <c>VHDLTEST_ACTIVEHDL_PATH</c> is not set. Result is either a valid path
    ///     (Active-HDL installed) or null (Active-HDL not installed).
    /// </summary>
    [Fact]
    public void ActiveHdlSimulator_FindPath_WithoutEnvVar_ReturnsNullOrPath()
    {
        // Arrange: ensure VHDLTEST_ACTIVEHDL_PATH is not set for this test
        var previousValue = Environment.GetEnvironmentVariable("VHDLTEST_ACTIVEHDL_PATH");
        Environment.SetEnvironmentVariable("VHDLTEST_ACTIVEHDL_PATH", null);

        try
        {
            // Act: call FindPath() without the env var override
            var result = ActiveHdlSimulator.FindPath();

            // Assert: result is either null (Active-HDL not installed) or a non-empty path string
            Assert.True(result == null || result.Length > 0);
        }
        finally
        {
            // Restore the environment variable
            Environment.SetEnvironmentVariable("VHDLTEST_ACTIVEHDL_PATH", previousValue);
        }
    }

    /// <summary>
    ///     Verifies that calling Compile when Active-HDL is not installed throws
    ///     <see cref="InvalidOperationException"/> with the expected message.
    ///     This test is skipped in environments where Active-HDL is installed.
    /// </summary>
    [Fact]
    public void ActiveHdlSimulator_Compile_SimulatorNotAvailable_ThrowsInvalidOperationException()
    {
        // Skip this test when Active-HDL is available — we can only test the unavailable path
        // when SimulatorPath is null
        if (ActiveHdlSimulator.Instance.Available())
        {
            Assert.Skip("Active-HDL is installed; unavailable-path test not applicable in this environment");
        }

        // Arrange: simulator is not available (SimulatorPath is null)
        using var context = Context.Create(["--silent"]);
        var options = new Options(Directory.GetCurrentDirectory(), new ConfigDocument());

        // Act / Assert: Compile throws when Active-HDL is not installed
        var ex = Assert.Throws<InvalidOperationException>(
            () => ActiveHdlSimulator.Instance.Compile(context, options));
        Assert.Contains("ActiveHDL Simulator not available", ex.Message);
    }

    /// <summary>
    ///     Verifies that calling Test when Active-HDL is not installed throws
    ///     <see cref="InvalidOperationException"/> with the expected message.
    ///     This test is skipped in environments where Active-HDL is installed.
    /// </summary>
    [Fact]
    public void ActiveHdlSimulator_Test_SimulatorNotAvailable_ThrowsInvalidOperationException()
    {
        // Skip this test when Active-HDL is available — we can only test the unavailable path
        // when SimulatorPath is null
        if (ActiveHdlSimulator.Instance.Available())
        {
            Assert.Skip("Active-HDL is installed; unavailable-path test not applicable in this environment");
        }

        // Arrange: simulator is not available (SimulatorPath is null)
        using var context = Context.Create(["--silent"]);
        var options = new Options(Directory.GetCurrentDirectory(), new ConfigDocument());

        // Act / Assert: Test throws when Active-HDL is not installed
        var ex = Assert.Throws<InvalidOperationException>(
            () => ActiveHdlSimulator.Instance.Test(context, options, "test_tb"));
        Assert.Contains("ActiveHDL Simulator not available", ex.Message);
    }

    /// <summary>
    ///     Verifies that ActiveHdlSimulator.Compile invokes vsimsa with a do-script argument.
    /// </summary>
    [Fact]
    public void ActiveHdlSimulator_Compile_WithValidConfig_InvokesVsimsaWithDoScript()
    {
        // Arrange
        var invoker = new FakeProcessInvoker();
        var tempDir = Path.Combine(Path.GetTempPath(), $"vhdltest_{Path.GetRandomFileName()}");
        Directory.CreateDirectory(tempDir);
        try
        {
            var sim = ActiveHdlSimulator.CreateForTesting(tempDir, invoker);
            using var context = Context.Create(["--silent"]);
            var options = new Options(tempDir, new ConfigDocument());

            // Act
            sim.Compile(context, options);

            // Assert: at least one invocation occurred
            Assert.True(invoker.AllCalls.Count > 0);
            var allArgs = invoker.AllCalls.SelectMany(c => c.Arguments).ToList();
            Assert.Contains("-do", allArgs);
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
    public void ActiveHdlSimulator_Compile_WithFileNameContainingSpaceAndMetacharacter_QuotesFileNameInScript()
    {
        // Arrange
        var invoker = new FakeProcessInvoker();
        var tempDir = Path.Combine(Path.GetTempPath(), $"vhdltest_{Path.GetRandomFileName()}");
        Directory.CreateDirectory(tempDir);
        try
        {
            var sim = ActiveHdlSimulator.CreateForTesting(tempDir, invoker);
            using var context = Context.Create(["--silent"]);
            var config = new ConfigDocument { Files = ["my file [1].vhd"] };
            var options = new Options(tempDir, config);

            // Act
            sim.Compile(context, options);

            // Assert: the generated compile script contains the file name verbatim, brace-quoted
            var scriptPath = Path.Combine(tempDir, "VHDLTest.out", "ActiveHDL", "compile.do");
            var content = File.ReadAllText(scriptPath);
            Assert.Contains("acom -2008 -dbg {my file [1].vhd}", content);

            // Assert: the invocation's arguments still resolve correctly (round-trip proof)
            Assert.True(invoker.AllCalls.Count > 0);
            var allArgs = invoker.AllCalls.SelectMany(c => c.Arguments).ToList();
            Assert.Contains("-do", allArgs);
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
    public void ActiveHdlSimulator_Test_WithTestNameContainingSpaceAndMetacharacter_QuotesTestNameInScript()
    {
        // Arrange
        var invoker = new FakeProcessInvoker();
        var tempDir = Path.Combine(Path.GetTempPath(), $"vhdltest_{Path.GetRandomFileName()}");
        Directory.CreateDirectory(tempDir);
        try
        {
            Directory.CreateDirectory(Path.Combine(tempDir, "VHDLTest.out", "ActiveHDL"));

            var sim = ActiveHdlSimulator.CreateForTesting(tempDir, invoker);
            using var context = Context.Create(["--silent"]);
            var options = new Options(tempDir, new ConfigDocument());

            // Act
            sim.Test(context, options, "lib.my tb");

            // Assert: the generated test script contains the test name verbatim, brace-quoted
            var scriptPath = Path.Combine(tempDir, "VHDLTest.out", "ActiveHDL", "test.do");
            var content = File.ReadAllText(scriptPath);
            Assert.Contains("asim {lib.my tb}", content);

            // Assert: the invocation's arguments still resolve correctly (round-trip proof)
            Assert.True(invoker.AllCalls.Count > 0);
            var allArgs = invoker.AllCalls.SelectMany(c => c.Arguments).ToList();
            Assert.Contains("-do", allArgs);
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }
}
