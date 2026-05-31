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
using DEMAConsulting.VHDLTest.Tests.Run;

namespace DEMAConsulting.VHDLTest.Tests.Simulators;

/// <summary>
///     Verifies the correct behavior of <see cref="NvcSimulator"/> — output classification,
///     availability guards, path discovery, and simulator identity.
/// </summary>
/// <remarks>
///     Unit under test: <see cref="NvcSimulator"/>.
///     <para>
///         The testing strategy is parse-based: <see cref="NvcSimulator.CompileProcessor"/> and
///         <see cref="NvcSimulator.TestProcessor"/> are exercised by calling <c>Parse</c> with
///         pre-captured output strings, covering clean, info, warning, error, failure, and fatal
///         output categories without requiring a live NVC installation. This approach keeps the
///         tests fast, hermetic, and runnable in any CI environment.
///     </para>
///     <para>
///         <c>[Collection("SimulatorEnvVarTests")]</c> serialization is required because
///         <see cref="NvcSimulator_FindPath_WithEnvVar_ReturnsEnvVarValue"/> mutates the
///         <c>VHDLTEST_NVC_PATH</c> process-level environment variable. Concurrent mutation of
///         the same environment variable by parallel test classes would create race conditions,
///         producing non-deterministic test results. Serializing via the shared
///         <c>SimulatorEnvVarTests</c> collection prevents this.
///     </para>
/// </remarks>
// All tests in this class are serialized via the SimulatorEnvVarTests collection because
// NvcSimulator_FindPath_WithEnvVar_ReturnsEnvVarValue modifies the
// VHDLTEST_NVC_PATH process-level environment variable.
// The DisableParallelization = true collection definition in SimulatorTestCollections.cs
// ensures these tests run sequentially with other env-var tests, preventing race conditions.
[Collection("SimulatorEnvVarTests")]
public class NvcSimulatorTests
{
    /// <summary>
    ///     Validates that <see cref="NvcSimulator.Instance"/> returns <c>"NVC"</c> as the simulator
    ///     name, confirming the singleton is registered with the expected identity for factory
    ///     lookup and reporting.
    /// </summary>
    [Fact]
    public void NvcSimulator_SimulatorName_ReturnsNVC()
    {
        // Arrange: no setup required — NvcSimulator.Instance is initialized at class load time

        // Act / Assert: simulator name is the fixed string "NVC"
        Assert.Equal("NVC", NvcSimulator.Instance.SimulatorName);
    }

    /// <summary>
    ///     Validates that clean NVC analysis output produces <see cref="RunLineType.Text"/>-classified
    ///     lines, confirming the processor does not generate false-positive diagnostics on normal output.
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
    ///     Validates that an NVC note line matching <c>.* Note:</c> in compile output is classified
    ///     as <see cref="RunLineType.Info"/>, confirming the note pattern is correctly detected.
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
    ///     Validates that an NVC warning line matching <c>.* Warning:</c> in compile output is
    ///     classified as <see cref="RunLineType.Warning"/>, confirming the warning pattern is
    ///     correctly detected.
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
    ///     Validates that an NVC error line matching <c>.* Error:</c> in compile output is classified
    ///     as <see cref="RunLineType.Error"/>, confirming the error pattern is correctly detected.
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
    ///     Validates that an NVC failure line matching <c>.* Failure:</c> in compile output is
    ///     classified as <see cref="RunLineType.Error"/>, confirming the failure pattern is
    ///     correctly mapped to Error severity.
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
        Assert.Equal(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.Equal(5.0, results.Duration, 1);
        Assert.Equal(1, results.ExitCode);
        Assert.Equal("Compile\nCompile Failure: Compile Failure", results.Output);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Compile", results.Lines[0].Text);
        Assert.Equal(RunLineType.Error, results.Lines[1].Type);
        Assert.Equal("Compile Failure: Compile Failure", results.Lines[1].Text);
    }

    /// <summary>
    ///     Validates that an NVC fatal line matching <c>.* Fatal:</c> in compile output is classified
    ///     as <see cref="RunLineType.Error"/>, confirming the fatal pattern is correctly mapped to
    ///     Error severity.
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
        Assert.Equal(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.Equal(5.0, results.Duration, 1);
        Assert.Equal(1, results.ExitCode);
        Assert.Equal("Compile\nCompile Fatal: Compile Fatal", results.Output);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Compile", results.Lines[0].Text);
        Assert.Equal(RunLineType.Error, results.Lines[1].Type);
        Assert.Equal("Compile Fatal: Compile Fatal", results.Lines[1].Text);
    }

    /// <summary>
    ///     Validates that clean NVC simulation output produces <see cref="RunLineType.Text"/>-classified
    ///     lines, confirming the processor does not generate false-positive diagnostics on normal
    ///     simulation output.
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
    ///     Validates that an NVC note line matching <c>.* Note:</c> in simulation output is classified
    ///     as <see cref="RunLineType.Info"/>, confirming the note pattern is correctly detected in
    ///     test output.
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
    ///     Validates that an NVC warning line matching <c>.* Warning:</c> in simulation output is
    ///     classified as <see cref="RunLineType.Warning"/>, confirming the warning pattern is
    ///     correctly detected in test output.
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
    ///     Validates that an NVC error line matching <c>.* Error:</c> in simulation output is
    ///     classified as <see cref="RunLineType.Error"/>, confirming the error pattern is correctly
    ///     detected in test output.
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
    ///     Validates that an NVC failure line matching <c>.* Failure:</c> in simulation output is
    ///     classified as <see cref="RunLineType.Error"/>, confirming the failure pattern maps to
    ///     Error severity in test output.
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
        Assert.Equal(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.Equal(5.0, results.Duration, 1);
        Assert.Equal(1, results.ExitCode);
        Assert.Equal("Test\nTest Failure: Test Failure", results.Output);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Test", results.Lines[0].Text);
        Assert.Equal(RunLineType.Error, results.Lines[1].Type);
        Assert.Equal("Test Failure: Test Failure", results.Lines[1].Text);
    }

    /// <summary>
    ///     Validates that an NVC fatal line matching <c>.* Fatal:</c> in simulation output is
    ///     classified as <see cref="RunLineType.Error"/>, confirming the fatal pattern maps to
    ///     Error severity in test output.
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
        Assert.Equal(new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc), results.Start);
        Assert.Equal(5.0, results.Duration, 1);
        Assert.Equal(1, results.ExitCode);
        Assert.Equal("Test\nTest Fatal: Test Fatal", results.Output);
        Assert.Equal(2, results.Lines.Count);
        Assert.Equal(RunLineType.Text, results.Lines[0].Type);
        Assert.Equal("Test", results.Lines[0].Text);
        Assert.Equal(RunLineType.Error, results.Lines[1].Type);
        Assert.Equal("Test Fatal: Test Fatal", results.Lines[1].Text);
    }

    /// <summary>
    ///     Validates that <see cref="NvcSimulator.FindPath()"/> returns the <c>VHDLTEST_NVC_PATH</c>
    ///     environment variable value when set, confirming the override takes precedence over
    ///     PATH search.
    /// </summary>
    [Fact]
    public void NvcSimulator_FindPath_WithEnvVar_ReturnsEnvVarValue()
    {
        // Arrange: set the VHDLTEST_NVC_PATH environment variable to a known value
        var expectedPath = "/custom/nvc/path";
        var previousValue = Environment.GetEnvironmentVariable("VHDLTEST_NVC_PATH");
        Environment.SetEnvironmentVariable("VHDLTEST_NVC_PATH", expectedPath);

        try
        {
            // Act: call FindPath()
            var result = NvcSimulator.FindPath();

            // Assert: result is the env var value
            Assert.Equal(expectedPath, result);
        }
        finally
        {
            // Restore the environment variable
            Environment.SetEnvironmentVariable("VHDLTEST_NVC_PATH", previousValue);
        }
    }

    /// <summary>
    ///     Validates that <see cref="NvcSimulator.FindPath()"/> returns either a valid path or null
    ///     without throwing when <c>VHDLTEST_NVC_PATH</c> is not set, confirming graceful behavior
    ///     in both installed and not-installed environments.
    /// </summary>
    [Fact]
    public void NvcSimulator_FindPath_WithoutEnvVar_ReturnsNullOrPath()
    {
        // Arrange: ensure VHDLTEST_NVC_PATH is not set for this test
        var previousValue = Environment.GetEnvironmentVariable("VHDLTEST_NVC_PATH");
        Environment.SetEnvironmentVariable("VHDLTEST_NVC_PATH", null);

        try
        {
            // Act: call FindPath() without the env var override
            var result = NvcSimulator.FindPath();

            // Assert: result is either null (NVC not installed) or a non-empty path string
            Assert.True(result == null || result.Length > 0);
        }
        finally
        {
            // Restore the environment variable
            Environment.SetEnvironmentVariable("VHDLTEST_NVC_PATH", previousValue);
        }
    }

    /// <summary>
    ///     Validates that <see cref="NvcSimulator.Compile"/> throws
    ///     <see cref="InvalidOperationException"/> when the simulator is not available
    ///     (i.e., <see cref="Simulator.SimulatorPath"/> is <c>null</c>).
    /// </summary>
    [Fact]
    public void NvcSimulator_Compile_WhenNotAvailable_ThrowsInvalidOperationException()
    {
        // Arrange: create a fresh NvcSimulator instance with a null SimulatorPath by
        // temporarily clearing the NVC env var and using a PATH that contains no NVC
        // executable, so FindPath() returns null during construction.
        var previousNvcPath = Environment.GetEnvironmentVariable("VHDLTEST_NVC_PATH");
        var previousPath = Environment.GetEnvironmentVariable("PATH");
        Environment.SetEnvironmentVariable("VHDLTEST_NVC_PATH", null);
        Environment.SetEnvironmentVariable("PATH", Path.GetTempPath());

        NvcSimulator simulator;
        try
        {
            // Use reflection to invoke the private constructor so FindPath() returns null
            var ctor = typeof(NvcSimulator)
                .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
                .Single(c => c.GetParameters().Length == 0);
            simulator = (NvcSimulator)ctor.Invoke([]);
        }
        finally
        {
            // Restore environment before running the actual assertion
            Environment.SetEnvironmentVariable("VHDLTEST_NVC_PATH", previousNvcPath);
            Environment.SetEnvironmentVariable("PATH", previousPath);
        }

        // Verify the constructed instance has a null SimulatorPath (NVC not found)
        if (simulator.SimulatorPath != null)
        {
            // Skip the assertion: NVC was found in the temp directory (unexpected but possible)
            Assert.Skip("NVC was unexpectedly found in the test environment; skipping availability guard test.");
        }

        using var context = Context.Create(["--silent"]);
        var options = new Options(Directory.GetCurrentDirectory(), new ConfigDocument());

        // Act / Assert: Compile throws when NVC is not installed
        var ex = Assert.Throws<InvalidOperationException>(() => simulator.Compile(context, options));
        Assert.Equal("NVC Simulator not available", ex.Message);
    }

    /// <summary>
    ///     Validates that <see cref="NvcSimulator.Test"/> throws
    ///     <see cref="InvalidOperationException"/> when the simulator is not available
    ///     (i.e., <see cref="Simulator.SimulatorPath"/> is <c>null</c>).
    /// </summary>
    [Fact]
    public void NvcSimulator_Test_WhenNotAvailable_ThrowsInvalidOperationException()
    {
        // Arrange: create a fresh NvcSimulator instance with a null SimulatorPath by
        // temporarily clearing the NVC env var and using a PATH that contains no NVC
        // executable, so FindPath() returns null during construction.
        var previousNvcPath = Environment.GetEnvironmentVariable("VHDLTEST_NVC_PATH");
        var previousPath = Environment.GetEnvironmentVariable("PATH");
        Environment.SetEnvironmentVariable("VHDLTEST_NVC_PATH", null);
        Environment.SetEnvironmentVariable("PATH", Path.GetTempPath());

        NvcSimulator simulator;
        try
        {
            // Use reflection to invoke the private constructor so FindPath() returns null
            var ctor = typeof(NvcSimulator)
                .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
                .Single(c => c.GetParameters().Length == 0);
            simulator = (NvcSimulator)ctor.Invoke([]);
        }
        finally
        {
            // Restore environment before running the actual assertion
            Environment.SetEnvironmentVariable("VHDLTEST_NVC_PATH", previousNvcPath);
            Environment.SetEnvironmentVariable("PATH", previousPath);
        }

        // Verify the constructed instance has a null SimulatorPath (NVC not found)
        if (simulator.SimulatorPath != null)
        {
            // Skip the assertion: NVC was found in the temp directory (unexpected but possible)
            Assert.Skip("NVC was unexpectedly found in the test environment; skipping availability guard test.");
        }

        using var context = Context.Create(["--silent"]);
        var options = new Options(Directory.GetCurrentDirectory(), new ConfigDocument());

        // Act / Assert: Test throws when NVC is not installed
        var ex = Assert.Throws<InvalidOperationException>(() =>
            simulator.Test(context, options, "test_bench"));
        Assert.Equal("NVC Simulator not available", ex.Message);
    }

    /// <summary>
    ///     Verifies that NvcSimulator.Compile invokes the nvc executable with analysis arguments.
    /// </summary>
    [Fact]
    public void NvcSimulator_Compile_WithValidConfig_InvokesNvcWithAnalysisArguments()
    {
        // Arrange
        var invoker = new FakeProcessInvoker();
        var tempDir = Path.Combine(Path.GetTempPath(), $"vhdltest_{Path.GetRandomFileName()}");
        Directory.CreateDirectory(tempDir);
        try
        {
            var sim = NvcSimulator.CreateForTesting(tempDir, invoker);
            using var context = Context.Create(["--silent"]);
            var options = new Options(tempDir, new ConfigDocument());

            // Act
            sim.Compile(context, options);

            // Assert: at least one invocation occurred
            Assert.True(invoker.AllCalls.Count > 0);

            // Assert: nvc path appears in the invocation
            var allStrings = invoker.AllCalls
                .SelectMany(c => new[] { c.Application }.Concat(c.Arguments))
                .ToList();
            Assert.Contains(Path.Combine(tempDir, "nvc"), allStrings);

            // Assert: key flags are present
            var allArgs = invoker.AllCalls.SelectMany(c => c.Arguments).ToList();
            Assert.Contains("--std=2008", allArgs);
            Assert.Contains("-a", allArgs);
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    /// <summary>
    ///     Verifies that NvcSimulator.Test invokes the nvc executable with elaboration and run arguments.
    /// </summary>
    [Fact]
    public void NvcSimulator_Test_WithValidConfig_InvokesNvcWithRunArguments()
    {
        // Arrange
        var invoker = new FakeProcessInvoker();
        var tempDir = Path.Combine(Path.GetTempPath(), $"vhdltest_{Path.GetRandomFileName()}");
        Directory.CreateDirectory(tempDir);
        try
        {
            // Pre-create the NVC output directory
            Directory.CreateDirectory(Path.Combine(tempDir, "VHDLTest.out", "NVC"));

            var sim = NvcSimulator.CreateForTesting(tempDir, invoker);
            using var context = Context.Create(["--silent"]);
            var options = new Options(tempDir, new ConfigDocument());

            // Act
            sim.Test(context, options, "my_tb");

            // Assert: at least one invocation occurred
            Assert.True(invoker.AllCalls.Count > 0);

            var allArgs = invoker.AllCalls.SelectMany(c => c.Arguments).ToList();
            Assert.Contains("--std=2008", allArgs);
            Assert.Contains("-e", allArgs);
            Assert.Contains("-r", allArgs);
            Assert.Contains("my_tb", allArgs);
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }
}
