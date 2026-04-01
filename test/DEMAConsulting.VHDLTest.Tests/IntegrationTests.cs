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

using System.Text.RegularExpressions;

namespace DEMAConsulting.VHDLTest.Tests;

/// <summary>
/// System-level integration tests for VHDLTest.
/// These tests run the VHDLTest tool as a whole and verify end-to-end behavior.
/// </summary>
[TestClass]
public partial class IntegrationTests
{
    /// <summary>
    /// Regular expression to check for version
    /// </summary>
    /// <returns>Version regex</returns>
    [GeneratedRegex(@"\d+\.\d+\.\d+.*")]
    private static partial Regex VersionRegex();

    /// <summary>
    /// Test usage information is reported when no arguments are specified
    /// </summary>
    [TestMethod]
    public void IntegrationTest_NoArguments_DisplaysUsageAndReturnsError()
    {
        // Run the application
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            "DEMAConsulting.VHDLTest.dll");

        // Verify error
        Assert.AreNotEqual(0, exitCode);

        // Verify usage reported
        Assert.Contains("Error: Missing arguments", output);
        Assert.Contains("Usage: VHDLTest", output);
    }

    /// <summary>
    /// Test usage information is reported when the '-h' parameter is specified
    /// </summary>
    [TestMethod]
    public void IntegrationTest_HelpShortFlag_DisplaysUsageAndReturnsSuccess()
    {
        // Run the application
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            "DEMAConsulting.VHDLTest.dll",
            "-h");

        // Verify no error
        Assert.AreEqual(0, exitCode);

        // Verify usage reported
        Assert.Contains("Usage: VHDLTest", output);
    }

    /// <summary>
    /// Test usage information is reported when the '-?' parameter is specified
    /// </summary>
    [TestMethod]
    public void IntegrationTest_HelpQuestionFlag_DisplaysUsageAndReturnsSuccess()
    {
        // Run the application
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            "DEMAConsulting.VHDLTest.dll",
            "-?");

        // Verify no error
        Assert.AreEqual(0, exitCode);

        // Verify usage reported
        Assert.Contains("Usage: VHDLTest", output);
    }

    /// <summary>
    /// Test usage information is reported when the '--help' parameter is specified
    /// </summary>
    [TestMethod]
    public void IntegrationTest_HelpLongFlag_DisplaysUsageAndReturnsSuccess()
    {
        // Run the application
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            "DEMAConsulting.VHDLTest.dll",
            "--help");

        // Verify no error
        Assert.AreEqual(0, exitCode);

        // Verify usage reported
        Assert.Contains("Usage: VHDLTest", output);
    }

    /// <summary>
    /// Test version information is reported when the '-v' parameter is specified
    /// </summary>
    [TestMethod]
    public void IntegrationTest_VersionShortFlag_DisplaysVersionAndReturnsSuccess()
    {
        // Query version
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            "DEMAConsulting.VHDLTest.dll",
            "-v");

        // Verify success
        Assert.AreEqual(0, exitCode);

        // Verify version reported
        Assert.MatchesRegex(VersionRegex(), output);
    }

    /// <summary>
    /// Test version information is reported when the '--version' parameter is specified
    /// </summary>
    [TestMethod]
    public void IntegrationTest_VersionLongFlag_DisplaysVersionAndReturnsSuccess()
    {
        // Query version
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            "DEMAConsulting.VHDLTest.dll",
            "--version");

        // Verify success
        Assert.AreEqual(0, exitCode);

        // Verify version reported
        Assert.MatchesRegex(VersionRegex(), output);
    }

    /// <summary>
    /// Test non-zero exit code with compile errors
    /// </summary>
    [TestMethod]
    public void IntegrationTest_CompileError_ReturnsNonZeroExitCode()
    {
        try
        {
            // Write a config file
            File.WriteAllText("test_compile_error.yaml",
                """
                files:
                 - file_error_test.vhd
                
                tests:
                 - file_error_test_tb
                """
                );

            // Run the application using the mock simulator
            var exitCode = Runner.Run(
                out _,
                "dotnet",
                "DEMAConsulting.VHDLTest.dll",
                "--simulator", "mock",
                "--config", "test_compile_error.yaml");

            // Verify error reported
            Assert.AreNotEqual(0, exitCode);
        }
        finally
        {
            File.Delete("test_compile_error.yaml");
        }
    }

    /// <summary>
    /// Test non-zero exit code with test-execution errors
    /// </summary>
    [TestMethod]
    public void IntegrationTest_TestExecutionError_ReturnsNonZeroExitCode()
    {
        try
        {
            // Write a config file
            File.WriteAllText("test_execution_error.yaml",
                """
                files:
                 - file_test.vhd

                tests:
                 - file_error_test_tb
                """
            );

            // Run the application using the mock simulator
            var exitCode = Runner.Run(
                out _,
                "dotnet",
                "DEMAConsulting.VHDLTest.dll",
                "--simulator", "mock",
                "--config", "test_execution_error.yaml");

            // Verify error reported
            Assert.AreNotEqual(0, exitCode);
        }
        finally
        {
            File.Delete("test_execution_error.yaml");
        }
    }

    /// <summary>
    /// Test zero exit code is returned when exit-0 is specified and tests fail
    /// </summary>
    [TestMethod]
    public void IntegrationTest_TestExecutionErrorWithExit0_ReturnsZeroExitCode()
    {
        try
        {
            // Write a config file
            File.WriteAllText("test_execution_error.yaml",
                """
                files:
                 - file_test.vhd

                tests:
                 - file_error_test_tb
                """
            );

            // Run the application using the mock simulator
            var exitCode = Runner.Run(
                out _,
                "dotnet",
                "DEMAConsulting.VHDLTest.dll",
                "--simulator", "mock",
                "--config", "test_execution_error.yaml",
                "--exit-0");

            // Verify error suppressed
            Assert.AreEqual(0, exitCode);
        }
        finally
        {
            File.Delete("test_execution_error.yaml");
        }
    }

    /// <summary>
    /// Test zero exit code is returned when all tests pass
    /// </summary>
    [TestMethod]
    public void IntegrationTest_TestsPassed_ReturnsZeroExitCode()
    {
        try
        {
            // Write a config file
            File.WriteAllText("test_execution_pass.yaml",
                """
                files:
                 - file_test.vhd

                tests:
                 - file_test_tb
                """
            );

            // Run the application using the mock simulator
            var exitCode = Runner.Run(
                out _,
                "dotnet",
                "DEMAConsulting.VHDLTest.dll",
                "--simulator", "mock",
                "--config", "test_execution_pass.yaml");

            // Verify no error
            Assert.AreEqual(0, exitCode);
        }
        finally
        {
            File.Delete("test_execution_pass.yaml");
        }
    }
}
