// Copyright (c) 2024 DEMA Consulting
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
///     Unit tests for the <see cref="MockSimulator"/> class, verifying that its deterministic
///     compile and test simulation behaviors produce the correct output classifications and
///     overall result severity from filename and test-name patterns.
/// </summary>
/// <remarks>
///     Tests cover: <see cref="Simulator.SimulatorName"/> and <see cref="Simulator.Available"/>
///     registration properties; <see cref="MockSimulator.CompileProcessor"/> and
///     <see cref="MockSimulator.TestProcessor"/> line-classification rules; and the
///     <see cref="MockSimulator.Compile"/> and <see cref="MockSimulator.Test"/> pattern-matching
///     algorithms end-to-end.
/// </remarks>
public class MockSimulatorTests
{
    /// <summary>
    ///     Verifies that <see cref="MockSimulator.Instance"/> has <see cref="Simulator.SimulatorName"/>
    ///     equal to <c>"Mock"</c>, confirming it is registered under the name the factory uses to
    ///     locate it when the caller passes <c>--simulator mock</c>.
    /// </summary>
    [Fact]
    public void MockSimulator_SimulatorName_ReturnsMock()
    {
        // Arrange: use the shared Mock simulator instance
        var simulator = MockSimulator.Instance;

        // Act: read the simulator name
        var name = simulator.SimulatorName;

        // Assert: name is "Mock"
        Assert.Equal("Mock", name);
    }

    /// <summary>
    ///     Verifies that <see cref="MockSimulator.Instance"/> reports unavailable, confirming
    ///     that <see cref="Simulator.SimulatorPath"/> is null and auto-discovery by the
    ///     simulator factory never selects MockSimulator during normal runs.
    /// </summary>
    [Fact]
    public void MockSimulator_Available_WithNullPath_ReturnsFalse()
    {
        // Arrange: use the shared Mock simulator instance (constructed with null path)
        var simulator = MockSimulator.Instance;

        // Act: check availability
        var available = simulator.Available();

        // Assert: simulator is not available because SimulatorPath is null
        Assert.False(available);
    }

    /// <summary>
    ///     Verifies that clean output through the mock compile processor produces a
    ///     <see cref="RunLineType.Text"/> summary with correct line count and timing fields,
    ///     confirming the minimum-severity baseline classification when no severity markers are present.
    /// </summary>
    [Fact]
    public void MockSimulator_CompileProcessor_CleanOutput_ReturnsTextResult()
    {
        // Arrange: define clean compile output with no severity markers
        var start = new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc);

        // Act: parse the output through the compile processor
        var results = MockSimulator.CompileProcessor.Parse(start, end, "Compile\nNo Issues", 0);

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
    ///     Verifies that a line prefixed <c>"Info:"</c> is classified as <see cref="RunLineType.Info"/>
    ///     by the mock compile processor, confirming the Info classification rule is present and
    ///     takes precedence over the default Text classification.
    /// </summary>
    [Fact]
    public void MockSimulator_CompileProcessor_InfoOutput_ReturnsInfoResult()
    {
        // Arrange: define compile output containing an Info marker
        var start = new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc);

        // Act: parse the output through the compile processor
        var results = MockSimulator.CompileProcessor.Parse(start, end, "Compile\nInfo: Compile Info", 0);

        // Assert: summary is Info and the info line is classified correctly
        Assert.Equal(RunLineType.Info, results.Summary);
        Assert.Equal(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.Equal(5.0, results.Duration, 1);
        Assert.Equal(0, results.ExitCode);
        Assert.Equal("Compile\nInfo: Compile Info", results.Output);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Compile", results.Lines[0].Text);
        Assert.Equal(RunLineType.Info, results.Lines[1].Type);
        Assert.Equal("Info: Compile Info", results.Lines[1].Text);
    }

    /// <summary>
    ///     Verifies that a line prefixed <c>"Warning:"</c> is classified as
    ///     <see cref="RunLineType.Warning"/> by the mock compile processor, confirming the Warning
    ///     classification rule correctly elevates severity above the Text baseline.
    /// </summary>
    [Fact]
    public void MockSimulator_CompileProcessor_WarningOutput_ReturnsWarningResult()
    {
        // Arrange: define compile output containing a Warning marker
        var start = new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc);

        // Act: parse the output through the compile processor
        var results = MockSimulator.CompileProcessor.Parse(start, end, "Compile\nWarning: Compile Warning", 0);

        // Assert: summary is Warning and the warning line is classified correctly
        Assert.Equal(RunLineType.Warning, results.Summary);
        Assert.Equal(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.Equal(5.0, results.Duration, 1);
        Assert.Equal(0, results.ExitCode);
        Assert.Equal("Compile\nWarning: Compile Warning", results.Output);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Compile", results.Lines[0].Text);
        Assert.Equal(RunLineType.Warning, results.Lines[1].Type);
        Assert.Equal("Warning: Compile Warning", results.Lines[1].Text);
    }

    /// <summary>
    ///     Verifies that a line prefixed <c>"Error:"</c> is classified as
    ///     <see cref="RunLineType.Error"/> by the mock compile processor, confirming the Error
    ///     classification rule produces the highest severity in the compile output path.
    /// </summary>
    [Fact]
    public void MockSimulator_CompileProcessor_ErrorOutput_ReturnsErrorResult()
    {
        // Arrange: define compile output containing an Error marker
        var start = new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc);

        // Act: parse the output through the compile processor
        var results = MockSimulator.CompileProcessor.Parse(start, end, "Compile\nError: Compile Error", 1);

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
    ///     Verifies that clean output through the mock test processor produces a
    ///     <see cref="RunLineType.Text"/> summary with correct line count and timing fields,
    ///     confirming the minimum-severity baseline classification when no severity markers are present.
    /// </summary>
    [Fact]
    public void MockSimulator_TestProcessor_CleanOutput_ReturnsTextResult()
    {
        // Arrange: define clean test output with no severity markers
        var start = new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc);

        // Act: parse the output through the test processor
        var results = MockSimulator.TestProcessor.Parse(start, end, "Test\nNo Issues", 0);

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
    ///     Verifies that a line prefixed <c>"Info:"</c> is classified as <see cref="RunLineType.Info"/>
    ///     by the mock test processor, confirming the Info classification rule is present and
    ///     takes precedence over the default Text classification.
    /// </summary>
    [Fact]
    public void MockSimulator_TestProcessor_InfoOutput_ReturnsInfoResult()
    {
        // Arrange: define test output containing an Info marker
        var start = new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc);

        // Act: parse the output through the test processor
        var results = MockSimulator.TestProcessor.Parse(start, end, "Test\nInfo: Test Info", 0);

        // Assert: summary is Info and the info line is classified correctly
        Assert.Equal(RunLineType.Info, results.Summary);
        Assert.Equal(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.Equal(5.0, results.Duration, 1);
        Assert.Equal(0, results.ExitCode);
        Assert.Equal("Test\nInfo: Test Info", results.Output);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Test", results.Lines[0].Text);
        Assert.Equal(RunLineType.Info, results.Lines[1].Type);
        Assert.Equal("Info: Test Info", results.Lines[1].Text);
    }

    /// <summary>
    ///     Verifies that a line prefixed <c>"Warning:"</c> is classified as
    ///     <see cref="RunLineType.Warning"/> by the mock test processor, confirming the Warning
    ///     classification rule correctly elevates severity above the Text baseline.
    /// </summary>
    [Fact]
    public void MockSimulator_TestProcessor_WarningOutput_ReturnsWarningResult()
    {
        // Arrange: define test output containing a Warning marker
        var start = new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc);

        // Act: parse the output through the test processor
        var results = MockSimulator.TestProcessor.Parse(start, end, "Test\nWarning: Test Warning", 0);

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
    ///     Verifies that a line prefixed <c>"Failure:"</c> is classified as
    ///     <see cref="RunLineType.Error"/> by the mock test processor, confirming that VHDL
    ///     assertion-failure lines are treated as errors even when the process exit code is zero.
    /// </summary>
    [Fact]
    public void MockSimulator_TestProcessor_FailureOutput_ReturnsErrorResult()
    {
        // Arrange: define test output containing a Failure marker
        var start = new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc);

        // Act: parse the output through the test processor
        var results = MockSimulator.TestProcessor.Parse(start, end, "Test\nFailure: Test Failure", 0);

        // Assert: summary is Error (Failure maps to Error) and line is classified correctly
        Assert.Equal(RunLineType.Error, results.Summary);
        Assert.Equal(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.Equal(5.0, results.Duration, 1);
        Assert.Equal(0, results.ExitCode);
        Assert.Equal("Test\nFailure: Test Failure", results.Output);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Test", results.Lines[0].Text);
        Assert.Equal(RunLineType.Error, results.Lines[1].Type);
        Assert.Equal("Failure: Test Failure", results.Lines[1].Text);
    }

    /// <summary>
    ///     Verifies that a line prefixed <c>"Error:"</c> is classified as
    ///     <see cref="RunLineType.Error"/> by the mock test processor, confirming the Error
    ///     classification rule produces the highest severity in the test execution output path.
    /// </summary>
    [Fact]
    public void MockSimulator_TestProcessor_ErrorOutput_ReturnsErrorResult()
    {
        // Arrange: define test output containing an Error marker
        var start = new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2024, 08, 10, 0, 0, 5, DateTimeKind.Utc);

        // Act: parse the output through the test processor
        var results = MockSimulator.TestProcessor.Parse(start, end, "Test\nError: Test Error", 1);

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
    ///     Verifies that <see cref="MockSimulator.Compile"/> returns an <see cref="RunLineType.Error"/>
    ///     summary when a source file name contains <c>_error_</c>, confirming the error filename
    ///     pattern triggers error classification and a non-zero exit code.
    /// </summary>
    [Fact]
    public void MockSimulator_Compile_WithErrorFile_ReturnsErrorResult()
    {
        // Arrange: create a context and options with a file containing '_error_' in the name
        using var context = Context.Create(["--silent"]);
        var options = new Options(
            Directory.GetCurrentDirectory(),
            new ConfigDocument { Files = ["my_error_module.vhd"] });

        // Act: compile with the mock simulator
        var results = MockSimulator.Instance.Compile(context, options);

        // Assert: result summary is Error due to the _error_ filename pattern
        Assert.Equal(RunLineType.Error, results.Summary);
    }

    /// <summary>
    ///     Verifies that <see cref="MockSimulator.Compile"/> returns a <see cref="RunLineType.Warning"/>
    ///     summary when a source file name contains <c>_warning_</c>, confirming the warning filename
    ///     pattern exercises the compile warning reporting path without triggering a failure.
    /// </summary>
    [Fact]
    public void MockSimulator_Compile_WithWarningFile_ReturnsWarningResult()
    {
        // Arrange: create a context and options with a file containing '_warning_' in the name
        using var context = Context.Create(["--silent"]);
        var options = new Options(
            Directory.GetCurrentDirectory(),
            new ConfigDocument { Files = ["my_warning_module.vhd"] });

        // Act: compile with the mock simulator
        var results = MockSimulator.Instance.Compile(context, options);

        // Assert: result summary is Warning due to the _warning_ filename pattern
        Assert.Equal(RunLineType.Warning, results.Summary);
    }

    /// <summary>
    ///     Verifies that <see cref="MockSimulator.Compile"/> returns a <see cref="RunLineType.Info"/>
    ///     summary when a source file name contains <c>_info_</c>, confirming the info filename
    ///     pattern exercises the compile info reporting path without escalating to a warning.
    /// </summary>
    [Fact]
    public void MockSimulator_Compile_WithInfoFile_ReturnsInfoResult()
    {
        // Arrange: create a context and options with a file containing '_info_' in the name
        using var context = Context.Create(["--silent"]);
        var options = new Options(
            Directory.GetCurrentDirectory(),
            new ConfigDocument { Files = ["my_info_module.vhd"] });

        // Act: compile with the mock simulator
        var results = MockSimulator.Instance.Compile(context, options);

        // Assert: result summary is Info due to the _info_ filename pattern
        Assert.Equal(RunLineType.Info, results.Summary);
    }

    /// <summary>
    ///     Verifies that <see cref="MockSimulator.Compile"/> returns a <see cref="RunLineType.Text"/>
    ///     summary when a source file name contains no special pattern, confirming the clean compile
    ///     path produces no severity escalation.
    /// </summary>
    [Fact]
    public void MockSimulator_Compile_WithCleanFile_ReturnsSuccessResult()
    {
        // Arrange: create a context and options with a clean filename
        using var context = Context.Create(["--silent"]);
        var options = new Options(
            Directory.GetCurrentDirectory(),
            new ConfigDocument { Files = ["my_adder.vhd"] });

        // Act: compile with the mock simulator
        var results = MockSimulator.Instance.Compile(context, options);

        // Assert: result summary is Text because the filename has no special pattern
        Assert.Equal(RunLineType.Text, results.Summary);
    }

    /// <summary>
    ///     Verifies that <see cref="MockSimulator.Test"/> returns a <see cref="RunLineType.Error"/>
    ///     summary when the test bench name contains <c>_error_</c>, confirming the error pattern
    ///     generates an <c>"Error:"</c> output line and sets a non-zero exit code.
    /// </summary>
    [Fact]
    public void MockSimulator_Test_WithErrorPattern_ReturnsErrorResult()
    {
        // Arrange: create a context and options; test name contains '_error_'
        using var context = Context.Create(["--silent"]);
        var options = new Options(
            Directory.GetCurrentDirectory(),
            new ConfigDocument());

        // Act: run a test whose name contains '_error_'
        var result = MockSimulator.Instance.Test(context, options, "adder_error_tb");

        // Assert: the test result summary is Error
        Assert.Equal(RunLineType.Error, result.RunResults.Summary);
    }

    /// <summary>
    ///     Verifies that <see cref="MockSimulator.Test"/> returns a <see cref="RunLineType.Error"/>
    ///     summary when the test bench name contains <c>_fail_</c>, confirming that a
    ///     <c>"Failure:"</c> output line is classified as Error by the test processor.
    /// </summary>
    [Fact]
    public void MockSimulator_Test_WithFailPattern_ReturnsErrorResult()
    {
        // Arrange: create a context and options; test name contains '_fail_'
        using var context = Context.Create(["--silent"]);
        var options = new Options(
            Directory.GetCurrentDirectory(),
            new ConfigDocument());

        // Act: run a test whose name contains '_fail_'
        var result = MockSimulator.Instance.Test(context, options, "adder_fail_tb");

        // Assert: the test result summary is Error (Failure maps to Error)
        Assert.Equal(RunLineType.Error, result.RunResults.Summary);
    }

    /// <summary>
    ///     Verifies that <see cref="MockSimulator.Test"/> returns a <see cref="RunLineType.Warning"/>
    ///     summary when the test bench name contains <c>_warning_</c>, confirming the warning pattern
    ///     exercises the test warning reporting path without triggering a failure.
    /// </summary>
    [Fact]
    public void MockSimulator_Test_WithWarningPattern_ReturnsWarningResult()
    {
        // Arrange: create a context and options; test name contains '_warning_'
        using var context = Context.Create(["--silent"]);
        var options = new Options(
            Directory.GetCurrentDirectory(),
            new ConfigDocument());

        // Act: run a test whose name contains '_warning_'
        var result = MockSimulator.Instance.Test(context, options, "adder_warning_tb");

        // Assert: the test result summary is Warning
        Assert.Equal(RunLineType.Warning, result.RunResults.Summary);
    }

    /// <summary>
    ///     Verifies that <see cref="MockSimulator.Test"/> returns a <see cref="RunLineType.Info"/>
    ///     summary when the test bench name contains <c>_info_</c>, confirming the info pattern
    ///     exercises the test info reporting path without escalating to a warning.
    /// </summary>
    [Fact]
    public void MockSimulator_Test_WithInfoPattern_ReturnsInfoResult()
    {
        // Arrange: create a context and options; test name contains '_info_'
        using var context = Context.Create(["--silent"]);
        var options = new Options(
            Directory.GetCurrentDirectory(),
            new ConfigDocument());

        // Act: run a test whose name contains '_info_'
        var result = MockSimulator.Instance.Test(context, options, "adder_info_tb");

        // Assert: the test result summary is Info
        Assert.Equal(RunLineType.Info, result.RunResults.Summary);
    }

    /// <summary>
    ///     Verifies that <see cref="MockSimulator.Test"/> returns a <see cref="RunLineType.Text"/>
    ///     summary when the test bench name contains no special pattern, confirming the clean
    ///     pass path produces no severity escalation.
    /// </summary>
    [Fact]
    public void MockSimulator_Test_WithCleanName_ReturnsSuccessResult()
    {
        // Arrange: create a context and options; test name has no special pattern
        using var context = Context.Create(["--silent"]);
        var options = new Options(
            Directory.GetCurrentDirectory(),
            new ConfigDocument());

        // Act: run a test with a clean name
        var result = MockSimulator.Instance.Test(context, options, "adder_pass_tb");

        // Assert: the test result summary is Text (clean pass)
        Assert.Equal(RunLineType.Text, result.RunResults.Summary);
    }
}
