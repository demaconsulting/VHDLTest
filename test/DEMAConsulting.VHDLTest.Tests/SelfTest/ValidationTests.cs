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
///     Tests for the <c>Validation</c> class and the SelfTest subsystem.
///     The class contains two sets of tests that share the same assertions:
///     <list type="bullet">
///       <item>
///         <c>SelfTest_*</c> — subsystem-level tests linked to <c>VHDLTest-SelfTest-*</c> requirements.
///         These are thin delegating wrappers over the unit-level tests below.
///       </item>
///       <item>
///         <c>Validation_*</c> — unit-level tests linked to <c>VHDLTest-SelfTest-Validation-*</c>
///         requirements. Each test runs the VHDLTest executable end-to-end via <c>Runner</c> with
///         <c>--validate --simulator mock</c>, exercising the complete self-validation pipeline:
///         embedded VHDL file extraction, in-process VHDLTest invocation, log capture, result
///         reporting, heading-depth configuration, results-file serialization, system-information
///         table output (OS Version and .NET runtime), and graceful error handling when an
///         unrecognized simulator name is supplied.
///       </item>
///     </list>
/// </summary>
public class ValidationTests
{
    // -------------------------------------------------------------------------
    // Subsystem-level tests (VHDLTest-SelfTest-* requirements).
    // These delegate to the unit-level Validation_* tests below so that both
    // requirement levels share a single set of passing assertions while
    // keeping distinct method bodies required by static analysis (S4144).
    // -------------------------------------------------------------------------

    /// <summary>
    /// Subsystem test: VHDLTest self-validation passes successfully when using the mock simulator.
    /// </summary>
    [Fact]
    public void SelfTest_Validate_MockSimulator_ReturnsSuccess()
    {
        // Act / Assert: delegate to unit-level test — verifies self-validation passes with mock simulator
        Validation_Run_MockSimulator_ReturnsSuccess();
    }

    /// <summary>
    /// Subsystem test: configuring validation depth produces Markdown headings at the specified level.
    /// </summary>
    [Fact]
    public void SelfTest_ValidateWithDepth_MockSimulator_RendersDepthHeadings()
    {
        // Act / Assert: delegate to unit-level test — verifies depth parameter renders correct heading level
        Validation_Run_DepthParameter_RendersDepthHeadings();
    }

    /// <summary>
    /// Subsystem test: validation results are saved to a TRX file when a results path is specified.
    /// </summary>
    [Fact]
    public void SelfTest_ValidateWithResultsFile_MockSimulator_SavesResults()
    {
        // Act / Assert: delegate to unit-level test — verifies results file is written with expected entries
        Validation_Run_ResultsFile_SavesResults();
    }

    /// <summary>
    /// Subsystem test: validation output contains OS Version field in the system information table.
    /// </summary>
    [Fact]
    public void SelfTest_Validate_MockSimulator_IncludesOSVersionInReport()
    {
        // Act / Assert: delegate to unit-level test — verifies OS Version and DotNet Runtime appear in report
        Validation_Run_MockSimulator_IncludesOSVersionInReport();
    }

    /// <summary>
    /// Subsystem test: VHDLTest self-validation fails gracefully when given an unrecognized simulator name.
    /// </summary>
    [Fact]
    public void SelfTest_Validate_InvalidSimulator_ReturnsFailure()
    {
        // Act / Assert: delegate to unit-level test — verifies non-zero exit and descriptive error for bad simulator
        Validation_Run_InvalidSimulator_ReturnsFailure();
    }

    // -------------------------------------------------------------------------
    // Unit-level tests for the Validation class (VHDLTest-SelfTest-Validation-* requirements).
    // These tests exercise Validation.Run end-to-end via Runner because the class
    // is internal and invokes Program.Run in-process, making direct method-call
    // isolation impractical without restructuring the production code.
    // -------------------------------------------------------------------------

    /// <summary>
    /// Unit test: Validation.Run executes embedded test benches and reports success when using the mock simulator.
    /// </summary>
    [Fact]
    public void Validation_Run_MockSimulator_ReturnsSuccess()
    {
        // Arrange: use the mock simulator for self-validation
        // Act: invoke Validation.Run via the mock simulator
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            "DEMAConsulting.VHDLTest.dll",
            "--simulator", "mock",
            "--validate");

        // Assert: exit code is 0 and validation passed
        Assert.Multiple(() =>
        {
            Assert.Equal(0, exitCode);
            Assert.Contains("Validation Passed", output);
        });
    }

    /// <summary>
    /// Unit test: Validation.Run honours the depth parameter and renders headings at the specified level.
    /// </summary>
    [Fact]
    public void Validation_Run_DepthParameter_RendersDepthHeadings()
    {
        // Arrange: use the mock simulator with depth 3
        // Act: invoke Validation.Run with depth 3
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            "DEMAConsulting.VHDLTest.dll",
            "--simulator", "mock",
            "--validate",
            "--depth", "3");

        // Assert: exit code is 0 and heading depth is applied
        Assert.Multiple(() =>
        {
            Assert.Equal(0, exitCode);
            Assert.Contains("### DEMAConsulting.VHDLTest", output);
        });
    }

    /// <summary>
    /// Unit test: Validation.Run saves results to a TRX file when a results path is specified.
    /// </summary>
    [Fact]
    public void Validation_Run_ResultsFile_SavesResults()
    {
        // Arrange: create a unique temp path for the results file
        var resultsFile = Path.Combine(Path.GetTempPath(), $"validation_results_{Guid.NewGuid():N}.trx");
        try
        {
            // Arrange: use the mock simulator and specify a results file path
            // Act: invoke Validation.Run with results file
            var exitCode = Runner.Run(
                out _,
                "dotnet",
                "DEMAConsulting.VHDLTest.dll",
                "--simulator", "mock",
                "--validate",
                "--results", resultsFile);

            // Assert: exit code is 0 and results file was written with expected content
            Assert.Multiple(() =>
            {
                Assert.Equal(0, exitCode);
                Assert.True(File.Exists(resultsFile));
                var text = File.ReadAllText(resultsFile);
                Assert.Contains("""<TestMethod codeBase="VHDLTest" className="VHDLTest.Validation" name="VHDLTest_TestPasses" />""", text);
                Assert.Contains("""<TestMethod codeBase="VHDLTest" className="VHDLTest.Validation" name="VHDLTest_TestFails" />""", text);
                Assert.Contains("""<Counters total="2" executed="2" passed="2" failed="0" />""", text);
            });
        }
        finally
        {
            // Delete results file
            File.Delete(resultsFile);
        }
    }

    /// <summary>
    /// Unit test: Validation.Run includes OS Version and DotNet Runtime rows in the system information table.
    /// </summary>
    [Fact]
    public void Validation_Run_MockSimulator_IncludesOSVersionInReport()
    {
        // Arrange: use the mock simulator for self-validation
        // Act: invoke Validation.Run via the mock simulator
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            "DEMAConsulting.VHDLTest.dll",
            "--simulator", "mock",
            "--validate");

        // Assert: exit code is 0 and OS Version field is present in output
        Assert.Multiple(() =>
        {
            Assert.Equal(0, exitCode);
            Assert.Contains("| OS Version", output);
            Assert.Contains("| DotNet Runtime", output);
        });
    }

    /// <summary>
    /// Unit test: Validation.Run handles an invalid simulator name gracefully and exits with non-zero code.
    /// </summary>
    [Fact]
    public void Validation_Run_InvalidSimulator_ReturnsFailure()
    {
        // Arrange: choose a simulator name that is not registered
        const string invalidSimulator = "totally_unknown_simulator_xyz";

        // Act: invoke Validation.Run with the invalid simulator name
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            "DEMAConsulting.VHDLTest.dll",
            "--simulator", invalidSimulator,
            "--validate");

        // Assert: VHDLTest exits with a non-zero code and output contains a descriptive error
        Assert.Multiple(() =>
        {
            Assert.NotEqual(0, exitCode);
            Assert.Contains("Simulator not found", output, StringComparison.OrdinalIgnoreCase);
        });
    }
}
