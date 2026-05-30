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

using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DEMAConsulting.VHDLTest.Tests;

/// <summary>
/// System-level integration tests for VHDLTest.
/// These tests run the VHDLTest tool as a whole and verify end-to-end behavior.
/// </summary>
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
    [Fact]
    public void VHDLTest_NoArguments_DisplaysUsageAndReturnsError()
    {
        // Arrange: (none — application is invoked directly)

        // Act: run the application with no arguments
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            "DEMAConsulting.VHDLTest.dll");

        // Assert: verify error exit code and usage message
        Assert.NotEqual(0, exitCode);
        Assert.Contains("Error: Missing arguments", output);
        Assert.Contains("Usage: VHDLTest", output);
    }

    /// <summary>
    /// Test usage information is reported when the '-h' parameter is specified
    /// </summary>
    [Fact]
    public void VHDLTest_HelpShortFlag_DisplaysUsageAndReturnsSuccess()
    {
        // Arrange: (none — application is invoked directly)

        // Act: run the application with -h flag
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            "DEMAConsulting.VHDLTest.dll",
            "-h");

        // Assert: verify success and usage message
        Assert.Equal(0, exitCode);
        Assert.Contains("Usage: VHDLTest", output);
    }

    /// <summary>
    /// Test usage information is reported when the '-?' parameter is specified
    /// </summary>
    [Fact]
    public void VHDLTest_HelpQuestionFlag_DisplaysUsageAndReturnsSuccess()
    {
        // Arrange: (none — application is invoked directly)

        // Act: run the application with -? flag
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            "DEMAConsulting.VHDLTest.dll",
            "-?");

        // Assert: verify success and usage message
        Assert.Equal(0, exitCode);
        Assert.Contains("Usage: VHDLTest", output);
    }

    /// <summary>
    /// Test usage information is reported when the '--help' parameter is specified
    /// </summary>
    [Fact]
    public void VHDLTest_HelpLongFlag_DisplaysUsageAndReturnsSuccess()
    {
        // Arrange: (none — application is invoked directly)

        // Act: run the application with --help flag
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            "DEMAConsulting.VHDLTest.dll",
            "--help");

        // Assert: verify success and usage message
        Assert.Equal(0, exitCode);
        Assert.Contains("Usage: VHDLTest", output);
    }

    /// <summary>
    /// Test version information is reported when the '-v' parameter is specified
    /// </summary>
    [Fact]
    public void VHDLTest_VersionShortFlag_DisplaysVersionAndReturnsSuccess()
    {
        // Arrange: (none — application is invoked directly)

        // Act: query version with -v flag
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            "DEMAConsulting.VHDLTest.dll",
            "-v");

        // Assert: verify success and version reported
        Assert.Equal(0, exitCode);
        Assert.Matches(VersionRegex(), output);
    }

    /// <summary>
    /// Test version information is reported when the '--version' parameter is specified
    /// </summary>
    [Fact]
    public void VHDLTest_VersionLongFlag_DisplaysVersionAndReturnsSuccess()
    {
        // Arrange: (none — application is invoked directly)

        // Act: query version with --version flag
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            "DEMAConsulting.VHDLTest.dll",
            "--version");

        // Assert: verify success and version reported
        Assert.Equal(0, exitCode);
        Assert.Matches(VersionRegex(), output);
    }

    /// <summary>
    /// Test non-zero exit code with compile errors
    /// </summary>
    [Fact]
    public void VHDLTest_CompileError_ReturnsNonZeroExitCode()
    {
        // Arrange: create a config file pointing to a file that will trigger a compile error
        var configFile = CreateConfigurationFile("test_compile_error", "file_error_test.vhd", "file_error_test_tb");

        try
        {
            // Act: run the application using the mock simulator
            var exitCode = Runner.Run(
                out _,
                "dotnet",
                "DEMAConsulting.VHDLTest.dll",
                "--simulator", "mock",
                "--config", configFile);

            // Assert: verify non-zero exit code
            Assert.NotEqual(0, exitCode);
        }
        finally
        {
            DeleteFileIfExists(configFile);
        }
    }

    /// <summary>
    /// Test non-zero exit code with test-execution errors
    /// </summary>
    [Fact]
    public void VHDLTest_TestExecutionError_ReturnsNonZeroExitCode()
    {
        // Arrange: create a config file for a test that will fail during execution
        var configFile = CreateConfigurationFile("test_execution_error", "file_test.vhd", "file_error_test_tb");

        try
        {
            // Act: run the application using the mock simulator
            var exitCode = Runner.Run(
                out _,
                "dotnet",
                "DEMAConsulting.VHDLTest.dll",
                "--simulator", "mock",
                "--config", configFile);

            // Assert: verify non-zero exit code
            Assert.NotEqual(0, exitCode);
        }
        finally
        {
            DeleteFileIfExists(configFile);
        }
    }

    /// <summary>
    /// Test zero exit code is returned when exit-0 is specified and tests fail
    /// </summary>
    [Fact]
    public void VHDLTest_TestExecutionErrorWithExit0_ReturnsZeroExitCode()
    {
        // Arrange: create a config file for a test that will fail during execution
        var configFile = CreateConfigurationFile("test_execution_error_exit0", "file_test.vhd", "file_error_test_tb");

        try
        {
            // Act: run the application using the mock simulator with --exit-0
            var exitCode = Runner.Run(
                out _,
                "dotnet",
                "DEMAConsulting.VHDLTest.dll",
                "--simulator", "mock",
                "--config", configFile,
                "--exit-0");

            // Assert: verify error suppressed
            Assert.Equal(0, exitCode);
        }
        finally
        {
            DeleteFileIfExists(configFile);
        }
    }

    /// <summary>
    /// Test self-validation passes and returns zero exit code
    /// </summary>
    [Fact]
    public void VHDLTest_Validate_ReturnsZeroExitCode()
    {
        // Arrange: (none — validation uses embedded mock simulator resources)

        // Act: run the application with --validate using the mock simulator
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            "DEMAConsulting.VHDLTest.dll",
            "--validate",
            "--simulator", "mock");

        // Assert: verify success and validation passed message
        Assert.Equal(0, exitCode);
        Assert.Contains("Validation Passed", output);
    }

    /// <summary>
    /// Test self-validation with --depth renders headings at the specified depth
    /// </summary>
    [Fact]
    public void VHDLTest_ValidateWithDepth_RendersDepthHeadings()
    {
        // Arrange: (none — validation uses embedded mock simulator resources)

        // Act: run the application with --validate --depth 2 using the mock simulator
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            "DEMAConsulting.VHDLTest.dll",
            "--validate",
            "--simulator", "mock",
            "--depth", "2");

        // Assert: verify success and that headings are at depth 2 (##)
        Assert.Equal(0, exitCode);
        Assert.Contains("##", output);
    }

    /// <summary>
    /// Test zero exit code is returned when all tests pass
    /// </summary>
    [Fact]
    public void VHDLTest_TestsPassed_ReturnsZeroExitCode()
    {
        // Arrange: create a config file for a test that will pass
        var configFile = CreateConfigurationFile("test_execution_pass", "file_test.vhd", "file_test_tb");

        try
        {
            // Act: run the application using the mock simulator
            var exitCode = Runner.Run(
                out _,
                "dotnet",
                "DEMAConsulting.VHDLTest.dll",
                "--simulator", "mock",
                "--config", configFile);

            // Assert: verify no error
            Assert.Equal(0, exitCode);
        }
        finally
        {
            DeleteFileIfExists(configFile);
        }
    }

    /// <summary>
    /// Test the test filter positional argument restricts execution to the named test bench
    /// </summary>
    [Fact]
    public void VHDLTest_WithTestFilter_OnlyRunsMatchingTest()
    {
        // Arrange: create a config with two tests — one that passes and one that fails
        var configFile = CreateConfigurationFile(
            "test_filter",
            "file_test.vhd",
            "file_test_tb",
            "file_error_test_tb");

        try
        {
            // Act: run with a filter that selects only the passing test
            var exitCode = Runner.Run(
                out var output,
                "dotnet",
                "DEMAConsulting.VHDLTest.dll",
                "--simulator", "mock",
                "--config", configFile,
                "--", "file_test_tb");

            // Assert: only the passing test ran — exit code is zero
            Assert.Equal(0, exitCode);

            // The matching test should appear in the results
            Assert.Contains("Passed file_test_tb", output);

            // The excluded test should not appear in the results
            Assert.DoesNotContain("file_error_test_tb", output);
        }
        finally
        {
            DeleteFileIfExists(configFile);
        }
    }

    /// <summary>
    /// Test the verbose flag enables additional diagnostic output
    /// </summary>
    [Fact]
    public void VHDLTest_VerboseFlag_DisplaysDetailedOutput()
    {
        // Arrange: create a config file for a passing test
        var configFile = CreateConfigurationFile("test_verbose", "file_test.vhd", "file_test_tb");

        try
        {
            // Act: run normal and verbose executions for comparison
            var normalExitCode = Runner.Run(
                out var normalOutput,
                "dotnet",
                "DEMAConsulting.VHDLTest.dll",
                "--simulator", "mock",
                "--config", configFile);

            var verboseExitCode = Runner.Run(
                out var verboseOutput,
                "dotnet",
                "DEMAConsulting.VHDLTest.dll",
                "--verbose",
                "--simulator", "mock",
                "--config", configFile);

            // Assert: verify both succeeded and verbose has additional output
            Assert.Equal(0, normalExitCode);
            Assert.Equal(0, verboseExitCode);

            // Verify verbose-only details are emitted
            Assert.DoesNotContain("Starting Mock compile...", normalOutput);
            Assert.DoesNotContain("Starting Mock test file_test_tb...", normalOutput);
            Assert.Contains("Starting Mock compile...", verboseOutput);
            Assert.Contains("Starting Mock test file_test_tb...", verboseOutput);
        }
        finally
        {
            DeleteFileIfExists(configFile);
        }
    }

    /// <summary>
    /// Test the silent flag suppresses non-essential output
    /// </summary>
    [Fact]
    public void VHDLTest_SilentFlag_SuppressesOutput()
    {
        // Arrange: create a config file for a passing test
        var configFile = CreateConfigurationFile("test_silent", "file_test.vhd", "file_test_tb");

        try
        {
            // Act: run with silent mode enabled
            var exitCode = Runner.Run(
                out var output,
                "dotnet",
                "DEMAConsulting.VHDLTest.dll",
                "--silent",
                "--simulator", "mock",
                "--config", configFile);

            // Assert: verify success and suppressed output
            Assert.Equal(0, exitCode);
            Assert.DoesNotContain("Building with Mock...", output);
            Assert.DoesNotContain("Passed 1 of 1 tests", output);
            Assert.True(string.IsNullOrWhiteSpace(output));
        }
        finally
        {
            DeleteFileIfExists(configFile);
        }
    }

    /// <summary>
    /// Test the log flag writes output to the requested log file
    /// </summary>
    [Fact]
    public void VHDLTest_LogFlag_WritesLogFile()
    {
        // Arrange: create a config file and a unique log file path
        var configFile = CreateConfigurationFile("test_log", "file_test.vhd", "file_test_tb");
        var logFile = Path.Combine(Environment.CurrentDirectory, $"test_log_{Guid.NewGuid():N}.log");

        try
        {
            // Act: run with log file output enabled
            var exitCode = Runner.Run(
                out _,
                "dotnet",
                "DEMAConsulting.VHDLTest.dll",
                "--log", logFile,
                "--simulator", "mock",
                "--config", configFile);

            // Assert: verify success and log file content
            Assert.Equal(0, exitCode);
            Assert.True(File.Exists(logFile));
            var logContent = File.ReadAllText(logFile);
            Assert.NotEmpty(logContent);
            Assert.Contains("Passed file_test_tb", logContent);
        }
        finally
        {
            DeleteFileIfExists(logFile);
            DeleteFileIfExists(configFile);
        }
    }

    /// <summary>
    /// Test the results flag writes a TRX results file
    /// </summary>
    [Fact]
    public void VHDLTest_ResultsFlag_WritesTrxFile()
    {
        // Arrange: create a config file and a unique TRX results file path
        var configFile = CreateConfigurationFile("test_results_trx", "file_test.vhd", "file_test_tb");
        var resultsFile = Path.Combine(Environment.CurrentDirectory, $"test_results_{Guid.NewGuid():N}.trx");

        try
        {
            // Act: run with TRX results output enabled
            var exitCode = Runner.Run(
                out _,
                "dotnet",
                "DEMAConsulting.VHDLTest.dll",
                "--results", resultsFile,
                "--simulator", "mock",
                "--config", configFile);

            // Assert: verify success and TRX file content
            Assert.Equal(0, exitCode);
            Assert.True(File.Exists(resultsFile));
            var resultsContent = File.ReadAllText(resultsFile);
            Assert.NotEmpty(resultsContent);
            Assert.Contains("<TestRun", resultsContent);
        }
        finally
        {
            DeleteFileIfExists(resultsFile);
            DeleteFileIfExists(configFile);
        }
    }

    /// <summary>
    /// Test the results flag writes a JUnit XML results file
    /// </summary>
    [Fact]
    public void VHDLTest_ResultsFlag_WritesJUnitFile()
    {
        // Arrange: create a config file and a unique JUnit XML results file path
        var configFile = CreateConfigurationFile("test_results_xml", "file_test.vhd", "file_test_tb");
        var resultsFile = Path.Combine(Environment.CurrentDirectory, $"test_results_{Guid.NewGuid():N}.xml");

        try
        {
            // Act: run with JUnit XML results output enabled
            var exitCode = Runner.Run(
                out _,
                "dotnet",
                "DEMAConsulting.VHDLTest.dll",
                "--results", resultsFile,
                "--simulator", "mock",
                "--config", configFile);

            // Assert: verify success and JUnit file content
            Assert.Equal(0, exitCode);
            Assert.True(File.Exists(resultsFile));
            var resultsContent = File.ReadAllText(resultsFile);
            Assert.NotEmpty(resultsContent);
            Assert.Contains("<testsuite", resultsContent);
        }
        finally
        {
            DeleteFileIfExists(resultsFile);
            DeleteFileIfExists(configFile);
        }
    }

    /// <summary>
    /// Test the simulator path override environment variable is honored
    /// </summary>
    [Fact]
    public void VHDLTest_SimulatorEnvPath_OverridesPath()
    {
        // Arrange: create a config file and a fake GHDL simulator directory
        var configFile = CreateConfigurationFile("test_env_path", "fake_test.vhd", "fake_test_tb");
        var simulatorDirectory = Path.Combine(Environment.CurrentDirectory, $"fake_ghdl_{Guid.NewGuid():N}");

        try
        {
            // Create a fake simulator executable that mimics the GHDL command surface used by the test.
            // On Windows the runnable file must be a .cmd batch file; on Unix it must be an executable
            // shell script named exactly "ghdl" (no extension) so that RunProcessor can invoke it directly.
            Directory.CreateDirectory(simulatorDirectory);
            if (OperatingSystem.IsWindows())
            {
                var simulatorScript = Path.Combine(simulatorDirectory, "ghdl.cmd");
                File.WriteAllText(
                    simulatorScript,
                    """
                    @echo off
                    if "%1"=="-a" exit /b 0
                    if "%1"=="-e" exit /b 0
                    if "%1"=="-r" echo fake_test.vhd:^(report note^): simulation passed & exit /b 0
                    exit /b 0
                    """);
            }
            else
            {
                var simulatorScript = Path.Combine(simulatorDirectory, "ghdl");
                File.WriteAllText(
                    simulatorScript,
                    """
                    #!/bin/sh
                    case "$1" in
                      -a) exit 0;;
                      -e) exit 0;;
                      -r) echo 'fake_test.vhd:(report note): simulation passed'; exit 0;;
                    esac
                    exit 0
                    """);
                File.SetUnixFileMode(
                    simulatorScript,
                    UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute |
                    UnixFileMode.GroupRead | UnixFileMode.GroupExecute |
                    UnixFileMode.OtherRead | UnixFileMode.OtherExecute);
            }

            // Act: run with environment variable override for GHDL path
            var exitCode = Runner.Run(
                out var output,
                "dotnet",
                new Dictionary<string, string>
                {
                    ["VHDLTEST_GHDL_PATH"] = simulatorDirectory
                },
                "DEMAConsulting.VHDLTest.dll",
                "--simulator", "ghdl",
                "--config", configFile);

            // Assert: verify the override allowed the run to complete successfully
            Assert.Equal(0, exitCode);
            Assert.Contains("Passed fake_test_tb", output);
        }
        finally
        {
            DeleteDirectoryIfExists(simulatorDirectory);
            DeleteFileIfExists(configFile);
        }
    }

    /// <summary>
    /// Test that each enumerated simulator name is recognized and does not produce an unknown-simulator error
    /// </summary>
    [Fact]
    public void VHDLTest_SimulatorSelect_AcceptsEnumeratedValues()
    {
        var configFile = CreateConfigurationFile("test_simulator_select", "file_test.vhd", "file_test_tb");

        try
        {
            // Arrange: the six simulator names specified in the --simulator option contract
            var simulatorNames = new[] { "ghdl", "nvc", "modelsim", "questasim", "vivado", "activehdl" };

            Assert.Multiple(() =>
            {
                foreach (var simulatorName in simulatorNames)
                {
                    // Act: run VHDLTest with the named simulator (simulator may not be installed)
                    Runner.Run(
                        out var output,
                        "dotnet",
                        "DEMAConsulting.VHDLTest.dll",
                        "--simulator", simulatorName,
                        "--config", configFile);

                    // Assert: the simulator name is recognized — "Simulator not found" indicates an
                    // unrecognized name; any other error means the name was accepted
                    Assert.DoesNotContain(
                        "Simulator not found",
                        output,
                        StringComparison.OrdinalIgnoreCase);
                }
            });
        }
        finally
        {
            DeleteFileIfExists(configFile);
        }
    }

    /// <summary>
    /// Create a configuration file for an integration test
    /// </summary>
    /// <param name="prefix">Filename prefix</param>
    /// <param name="sourceFileName">VHDL source file name</param>
    /// <param name="testNames">Configured test bench names</param>
    /// <returns>Path to the created configuration file</returns>
    private static string CreateConfigurationFile(string prefix, string sourceFileName, params string[] testNames)
    {
        var configFile = Path.Combine(Environment.CurrentDirectory, $"{prefix}_{Guid.NewGuid():N}.yaml");
        var builder = new StringBuilder();
        builder.AppendLine("files:");
        builder.AppendLine($" - {sourceFileName}");
        builder.AppendLine();
        builder.AppendLine("tests:");

        foreach (var testName in testNames)
        {
            builder.AppendLine($" - {testName}");
        }

        File.WriteAllText(configFile, builder.ToString());
        return configFile;
    }

    /// <summary>
    /// Delete a file when it exists
    /// </summary>
    /// <param name="path">Path to delete</param>
    private static void DeleteFileIfExists(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    /// <summary>
    /// Delete a directory when it exists
    /// </summary>
    /// <param name="path">Path to delete</param>
    private static void DeleteDirectoryIfExists(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }
    }
}
