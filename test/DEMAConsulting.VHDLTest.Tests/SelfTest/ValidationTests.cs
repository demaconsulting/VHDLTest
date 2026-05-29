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

using DEMAConsulting.VHDLTest.Tests;

namespace DEMAConsulting.VHDLTest.Tests.SelfTest;

/// <summary>
///     Integration tests for the <c>Validation</c> class in the SelfTest subsystem. Each test runs
///     the VHDLTest executable end-to-end as a subprocess using <c>Runner</c> with
///     <c>--validate --simulator mock</c>, exercising the complete self-validation pipeline:
///     embedded VHDL file extraction, in-process VHDLTest invocation, log capture, result
///     reporting, heading-depth configuration, results-file serialization, system-information
///     table output (OS Version and .NET runtime), and graceful error handling when an
///     unrecognized simulator name is supplied.
/// </summary>
public class ValidationTests
{
    /// <summary>
    /// Test that VHDLTest self-validation passes successfully when using the mock simulator.
    /// </summary>
    [Fact]
    public void SelfTest_Validate_MockSimulator_ReturnsSuccess()
    {
        // Arrange: use the mock simulator for self-validation
        // Act: run validation with the mock simulator
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            "DEMAConsulting.VHDLTest.dll",
            "--simulator", "mock",
            "--validate");

        // Assert: exit code is 0 and validation passed
        Assert.Equal(0, exitCode);
        Assert.Contains("Validation Passed", output);
    }

    /// <summary>
    /// Test that configuring validation depth produces Markdown headings at the specified level.
    /// </summary>
    [Fact]
    public void SelfTest_ValidateWithDepth_MockSimulator_RendersDepthHeadings()
    {
        // Arrange: use the mock simulator with depth 3
        // Act: run validation with the mock simulator and depth 3
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            "DEMAConsulting.VHDLTest.dll",
            "--simulator", "mock",
            "--validate",
            "--depth", "3");

        // Assert: exit code is 0 and heading depth is applied
        Assert.Equal(0, exitCode);
        Assert.Contains("### DEMAConsulting.VHDLTest", output);
    }

    /// <summary>
    /// Test validation results can be saved to file
    /// </summary>
    [Fact]
    public void SelfTest_ValidateWithResultsFile_MockSimulator_SavesResults()
    {
        // Use a unique temp path to avoid interference from parallel test runs
        var resultsFile = Path.Combine(Path.GetTempPath(), $"validation_results_{Guid.NewGuid():N}.trx");
        try
        {
            // Arrange: use the mock simulator and specify a results file path
            // Act: run validation with results file
            var exitCode = Runner.Run(
                out _,
                "dotnet",
                "DEMAConsulting.VHDLTest.dll",
                "--simulator", "mock",
                "--validate",
                "--results", resultsFile);

            // Assert: exit code is 0 and results file was written with expected content
            Assert.Equal(0, exitCode);
            Assert.True(File.Exists(resultsFile));
            var text = File.ReadAllText(resultsFile);
            Assert.Contains("""<TestMethod codeBase="VHDLTest" className="VHDLTest.Validation" name="VHDLTest_TestPasses" />""", text);
            Assert.Contains("""<TestMethod codeBase="VHDLTest" className="VHDLTest.Validation" name="VHDLTest_TestFails" />""", text);
            Assert.Contains("""<Counters total="2" executed="2" passed="2" failed="0" />""", text);
        }
        finally
        {
            // Delete results file
            File.Delete(resultsFile);
        }
    }

    /// <summary>
    /// Test validation output contains OS Version field in the information table
    /// </summary>
    [Fact]
    public void SelfTest_Validate_MockSimulator_IncludesOSVersionInReport()
    {
        // Arrange: use the mock simulator for self-validation
        // Act: run validation with the mock simulator
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            "DEMAConsulting.VHDLTest.dll",
            "--simulator", "mock",
            "--validate");

        // Assert: exit code is 0 and OS Version field is present in output
        Assert.Equal(0, exitCode);
        Assert.Contains("| OS Version", output);
        Assert.Contains("| DotNet Runtime", output);
    }

    /// <summary>
    /// Test that VHDLTest self-validation fails gracefully when given an unrecognized simulator name.
    /// </summary>
    [Fact]
    public void SelfTest_Validate_InvalidSimulator_ReturnsFailure()
    {
        // Arrange: choose a simulator name that is not registered
        const string invalidSimulator = "totally_unknown_simulator_xyz";

        // Act: run validation with the invalid simulator name
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            "DEMAConsulting.VHDLTest.dll",
            "--simulator", invalidSimulator,
            "--validate");

        // Assert: VHDLTest exits with a non-zero code and output contains a descriptive error
        Assert.NotEqual(0, exitCode);
        Assert.Contains("Simulator not found", output, StringComparison.OrdinalIgnoreCase);
    }
}
