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
    public void IntegrationTest_NoArguments_DisplaysUsageAndReturnsError()
    {
        // Run the application
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            "DEMAConsulting.VHDLTest.dll");

        // Verify error
        Assert.NotEqual(0, exitCode);

        // Verify usage reported
        Assert.Contains("Error: Missing arguments", output);
        Assert.Contains("Usage: VHDLTest", output);
    }

    /// <summary>
    /// Test usage information is reported when the '-h' parameter is specified
    /// </summary>
    [Fact]
    public void IntegrationTest_HelpShortFlag_DisplaysUsageAndReturnsSuccess()
    {
        // Run the application
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            "DEMAConsulting.VHDLTest.dll",
            "-h");

        // Verify no error
        Assert.Equal(0, exitCode);

        // Verify usage reported
        Assert.Contains("Usage: VHDLTest", output);
    }

    /// <summary>
    /// Test usage information is reported when the '-?' parameter is specified
    /// </summary>
    [Fact]
    public void IntegrationTest_HelpQuestionFlag_DisplaysUsageAndReturnsSuccess()
    {
        // Run the application
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            "DEMAConsulting.VHDLTest.dll",
            "-?");

        // Verify no error
        Assert.Equal(0, exitCode);

        // Verify usage reported
        Assert.Contains("Usage: VHDLTest", output);
    }

    /// <summary>
    /// Test usage information is reported when the '--help' parameter is specified
    /// </summary>
    [Fact]
    public void IntegrationTest_HelpLongFlag_DisplaysUsageAndReturnsSuccess()
    {
        // Run the application
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            "DEMAConsulting.VHDLTest.dll",
            "--help");

        // Verify no error
        Assert.Equal(0, exitCode);

        // Verify usage reported
        Assert.Contains("Usage: VHDLTest", output);
    }

    /// <summary>
    /// Test version information is reported when the '-v' parameter is specified
    /// </summary>
    [Fact]
    public void IntegrationTest_VersionShortFlag_DisplaysVersionAndReturnsSuccess()
    {
        // Query version
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            "DEMAConsulting.VHDLTest.dll",
            "-v");

        // Verify success
        Assert.Equal(0, exitCode);

        // Verify version reported
        Assert.Matches(VersionRegex(), output);
    }

    /// <summary>
    /// Test version information is reported when the '--version' parameter is specified
    /// </summary>
    [Fact]
    public void IntegrationTest_VersionLongFlag_DisplaysVersionAndReturnsSuccess()
    {
        // Query version
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            "DEMAConsulting.VHDLTest.dll",
            "--version");

        // Verify success
        Assert.Equal(0, exitCode);

        // Verify version reported
        Assert.Matches(VersionRegex(), output);
    }

    /// <summary>
    /// Test non-zero exit code with compile errors
    /// </summary>
    [Fact]
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
            Assert.NotEqual(0, exitCode);
        }
        finally
        {
            File.Delete("test_compile_error.yaml");
        }
    }

    /// <summary>
    /// Test non-zero exit code with test-execution errors
    /// </summary>
    [Fact]
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
            Assert.NotEqual(0, exitCode);
        }
        finally
        {
            File.Delete("test_execution_error.yaml");
        }
    }

    /// <summary>
    /// Test zero exit code is returned when exit-0 is specified and tests fail
    /// </summary>
    [Fact]
    public void IntegrationTest_TestExecutionErrorWithExit0_ReturnsZeroExitCode()
    {
        try
        {
            // Write a config file
            File.WriteAllText("test_execution_error_exit0.yaml",
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
                "--config", "test_execution_error_exit0.yaml",
                "--exit-0");

            // Verify error suppressed
            Assert.Equal(0, exitCode);
        }
        finally
        {
            File.Delete("test_execution_error_exit0.yaml");
        }
    }

    /// <summary>
    /// Test zero exit code is returned when all tests pass
    /// </summary>
    [Fact]
    public void IntegrationTest_TestsPassed_ReturnsZeroExitCode()
    {
        var configFile = CreateConfigurationFile("test_execution_pass", "file_test.vhd", "file_test_tb");

        try
        {
            // Run the application using the mock simulator
            var exitCode = Runner.Run(
                out _,
                "dotnet",
                "DEMAConsulting.VHDLTest.dll",
                "--simulator", "mock",
                "--config", configFile);

            // Verify no error
            Assert.Equal(0, exitCode);
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
    public void IntegrationTest_VerboseFlag_DisplaysDetailedOutput()
    {
        var configFile = CreateConfigurationFile("test_verbose", "file_test.vhd", "file_test_tb");

        try
        {
            // Run a normal execution for comparison
            var normalExitCode = Runner.Run(
                out var normalOutput,
                "dotnet",
                "DEMAConsulting.VHDLTest.dll",
                "--simulator", "mock",
                "--config", configFile);

            // Run a verbose execution
            var verboseExitCode = Runner.Run(
                out var verboseOutput,
                "dotnet",
                "DEMAConsulting.VHDLTest.dll",
                "--verbose",
                "--simulator", "mock",
                "--config", configFile);

            // Verify both executions succeeded
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
    public void IntegrationTest_SilentFlag_SuppressesOutput()
    {
        var configFile = CreateConfigurationFile("test_silent", "file_test.vhd", "file_test_tb");

        try
        {
            // Run the application with silent mode enabled
            var exitCode = Runner.Run(
                out var output,
                "dotnet",
                "DEMAConsulting.VHDLTest.dll",
                "--silent",
                "--simulator", "mock",
                "--config", configFile);

            // Verify the run succeeded
            Assert.Equal(0, exitCode);

            // Verify informational output was suppressed
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
    public void IntegrationTest_LogFlag_WritesLogFile()
    {
        var configFile = CreateConfigurationFile("test_log", "file_test.vhd", "file_test_tb");
        var logFile = Path.Combine(Environment.CurrentDirectory, $"test_log_{Guid.NewGuid():N}.log");

        try
        {
            // Run the application with log file output enabled
            var exitCode = Runner.Run(
                out _,
                "dotnet",
                "DEMAConsulting.VHDLTest.dll",
                "--log", logFile,
                "--simulator", "mock",
                "--config", configFile);

            // Verify the run succeeded and the log file was written
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
    public void IntegrationTest_ResultsFlag_WritesTrxFile()
    {
        var configFile = CreateConfigurationFile("test_results_trx", "file_test.vhd", "file_test_tb");
        var resultsFile = Path.Combine(Environment.CurrentDirectory, $"test_results_{Guid.NewGuid():N}.trx");

        try
        {
            // Run the application with TRX results output enabled
            var exitCode = Runner.Run(
                out _,
                "dotnet",
                "DEMAConsulting.VHDLTest.dll",
                "--results", resultsFile,
                "--simulator", "mock",
                "--config", configFile);

            // Verify the run succeeded and the TRX file was written
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
    public void IntegrationTest_ResultsFlag_WritesJUnitFile()
    {
        var configFile = CreateConfigurationFile("test_results_xml", "file_test.vhd", "file_test_tb");
        var resultsFile = Path.Combine(Environment.CurrentDirectory, $"test_results_{Guid.NewGuid():N}.xml");

        try
        {
            // Run the application with JUnit XML results output enabled
            var exitCode = Runner.Run(
                out _,
                "dotnet",
                "DEMAConsulting.VHDLTest.dll",
                "--results", resultsFile,
                "--simulator", "mock",
                "--config", configFile);

            // Verify the run succeeded and the JUnit XML file was written
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
    public void IntegrationTest_SimulatorEnvPath_OverridesPath()
    {
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

            // Run the application using the environment override for GHDL discovery
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

            // Verify the override allowed the run to complete successfully
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
