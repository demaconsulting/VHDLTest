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
/// Tests for the ActiveHDL simulator
/// </summary>
public class ActiveHdlSimulatorTests
{
    /// <summary>
    /// Check name of ActiveHDL simulator name
    /// </summary>
    [Fact]
    public void ActiveHdlSimulator_SimulatorName_ReturnsActiveHDL()
    {
        // Act / Assert: simulator name is "ActiveHdl"
        Assert.Equal("ActiveHdl", ActiveHdlSimulator.Instance.SimulatorName);
    }

    /// <summary>
    /// Test ActiveHDL simulator compile with clean output
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
    /// Test ActiveHDL simulator compile with an info message
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
    /// Test ActiveHDL simulator compile with an error message
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
    /// Test ActiveHDL simulator compile with a fatal runtime error message
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
    /// Test ActiveHDL simulator test with clean output
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
    /// Test ActiveHDL simulator test with an info message
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
    /// Test ActiveHDL simulator test with a warning message
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
    /// Test ActiveHDL simulator test with an error message
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
    /// Test ActiveHDL simulator test that the first Lattice Edition warning is suppressed to Text
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
    /// Test ActiveHDL simulator test that the second Lattice Edition warning is suppressed to Text
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
    /// Test ActiveHDL simulator test with a KERNEL Warning message
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
    /// Test ActiveHDL simulator test with a KERNEL WARNING (uppercase) message
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
    /// Test ActiveHDL simulator test with an EXECUTION FAILURE message
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
    /// Test ActiveHDL simulator test with a KERNEL ERROR message
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
    /// Test ActiveHDL simulator test with a RUNTIME Fatal Error message
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
    /// Test ActiveHDL simulator test with a VSIM Error message
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
        // Arrange: create an isolated temporary working directory and a fake vsimsa executable
        // so that SimulatorPath is non-null and the script-write step is reached without
        // requiring a real Active-HDL installation.
        var tempDir = Path.Combine(Path.GetTempPath(), $"vhdltest_{Path.GetRandomFileName()}");
        Directory.CreateDirectory(tempDir);
        try
        {
            // Create a fake vsimsa stub so the simulator path resolves to a real directory
            if (OperatingSystem.IsWindows())
            {
                File.WriteAllText(Path.Combine(tempDir, "vsimsa.bat"), "@echo off\r\nexit /b 0\r\n");
            }
            else
            {
                File.WriteAllText(Path.Combine(tempDir, "vsimsa"), "#!/bin/sh\nexit 0\n");
            }

            // Pre-create the library output directory that Test() expects to exist
            var workDir = Path.Combine(tempDir, "work");
            Directory.CreateDirectory(Path.Combine(workDir, "VHDLTest.out", "ActiveHDL"));

            using var context = Context.Create(["--silent"]);
            var options = new Options(workDir, new ConfigDocument());

            // Create a test-only instance with tempDir as the simulator path.
            // Set VHDLTEST_ACTIVEHDL_PATH temporarily and invoke the private constructor
            // via reflection to bypass the singleton while keeping production code intact.
            var savedEnv = Environment.GetEnvironmentVariable("VHDLTEST_ACTIVEHDL_PATH");
            Environment.SetEnvironmentVariable("VHDLTEST_ACTIVEHDL_PATH", tempDir);
            ActiveHdlSimulator simulator;
            try
            {
                var ctor = typeof(ActiveHdlSimulator).GetConstructor(
                    BindingFlags.NonPublic | BindingFlags.Instance,
                    null,
                    Type.EmptyTypes,
                    null)!;
                simulator = (ActiveHdlSimulator)ctor.Invoke(null);
            }
            finally
            {
                Environment.SetEnvironmentVariable("VHDLTEST_ACTIVEHDL_PATH", savedEnv);
            }

            // Act: invoke Test so the script file is written.
            // Execution may fail because the stub vsimsa produces no expected output;
            // only the script file content matters so execution failures are suppressed.
            try
            {
                simulator.Test(context, options, "clean_tb");
            }
            catch (Exception)
            {
                // execution failure is expected with a stub executable
            }

            // Assert: the generated TCL test script contains "exit -code 0" to signal
            // successful simulation completion back to vsimsa
            var scriptPath = Path.Combine(workDir, "VHDLTest.out", "ActiveHDL", "test.do");
            Assert.True(File.Exists(scriptPath), "TCL test script file was not created");
            var scriptContent = File.ReadAllText(scriptPath);
            Assert.Contains("exit -code 0", scriptContent);
        }
        finally
        {
            // Cleanup: remove the temporary directory and all its contents
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, recursive: true);
            }
        }
    }
}
