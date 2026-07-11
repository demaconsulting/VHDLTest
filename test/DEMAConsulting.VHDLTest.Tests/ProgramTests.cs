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

namespace DEMAConsulting.VHDLTest.Tests;

/// <summary>
/// Unit tests for <see cref="Program"/> class.
/// </summary>
public class ProgramTests
{
    /// <summary>
    /// Configuration file contents for a run that should pass.
    /// </summary>
    private const string PassingConfigContent =
        """
        files:
        - file1.vhd

        tests:
        - test1
        """;

    /// <summary>
    /// Configuration file contents for a run that should fail (test name contains _fail_).
    /// </summary>
    private const string FailingConfigContent =
        """
        files:
        - file1.vhd

        tests:
        - test_fail_1
        """;

    /// <summary>
    /// Verifies that the version string is non-null and non-empty when the assembly carries an informational version attribute.
    /// </summary>
    [Fact]
    public void Program_Version_WhenAssemblyBuilt_IsNotEmpty()
    {
        // Arrange: no setup required — Program.Version is a static property initialized at startup

        // Act: read the version string
        var version = Program.Version;

        // Assert - verify version is populated
        Assert.False(string.IsNullOrEmpty(version));
    }

    /// <summary>
    ///     Test that Run returns zero exit code when version flag is given
    /// </summary>
    [Fact]
    public void Program_Run_WithVersionFlag_ReturnsZeroExitCode()
    {
        // Arrange: create a silent context with version flag
        using var context = Context.Create(["--version", "--silent"]);

        // Act: run the program
        Program.Run(context);

        // Assert: verify zero exit code
        Assert.Equal(0, context.ExitCode);
    }

    /// <summary>
    ///     Test that Run returns zero exit code when help flag is given
    /// </summary>
    [Fact]
    public void Program_Run_WithHelpFlag_ReturnsZeroExitCode()
    {
        // Arrange: create a silent context with help flag
        using var context = Context.Create(["--help", "--silent"]);

        // Act: run the program
        Program.Run(context);

        // Assert: verify zero exit code
        Assert.Equal(0, context.ExitCode);
    }

    /// <summary>
    ///     Test that Run displays usage information when no configuration file argument is provided
    /// </summary>
    [Fact]
    public void Program_Run_WithNoConfigFileArgument_DisplaysUsage()
    {
        // Arrange: create a log file to capture output; no config file argument is passed
        var logFile = Path.GetTempFileName();
        try
        {
            using (var context = Context.Create(["--log", logFile, "--silent"]))
            {
                // Act: run the program without a configuration file argument
                Program.Run(context);
            }

            // Assert: verify usage text was written to the log
            var output = File.ReadAllText(logFile);
            Assert.True(output.Contains("Usage: VHDLTest"), $"Expected 'Usage: VHDLTest' in output:\n{output}");
        }
        finally
        {
            File.Delete(logFile);
        }
    }

    /// <summary>
    ///     Test that Run displays version information when version flag is given
    /// </summary>
    [Fact]
    public void Program_Run_WithVersionFlag_DisplaysVersion()
    {
        // Arrange: create a log file to capture output; pass the version flag
        var logFile = Path.GetTempFileName();
        try
        {
            using (var context = Context.Create(["--version", "--log", logFile, "--silent"]))
            {
                // Act: run the program with the version flag
                Program.Run(context);
            }

            // Assert: verify the version string was written to the log
            var output = File.ReadAllText(logFile);
            Assert.True(output.Contains(Program.Version), $"Expected version '{Program.Version}' in output:\n{output}");
        }
        finally
        {
            File.Delete(logFile);
        }
    }

    /// <summary>
    ///     Test that Run displays help information when help flag is given
    /// </summary>
    [Fact]
    public void Program_Run_WithHelpFlag_DisplaysHelp()
    {
        // Arrange: create a log file to capture output; pass the help flag
        var logFile = Path.GetTempFileName();
        try
        {
            using (var context = Context.Create(["--help", "--log", logFile, "--silent"]))
            {
                // Act: run the program with the help flag
                Program.Run(context);
            }

            // Assert: verify help text was written to the log
            var output = File.ReadAllText(logFile);
            Assert.True(output.Contains("Usage: VHDLTest"), $"Expected 'Usage: VHDLTest' in output:\n{output}");
        }
        finally
        {
            File.Delete(logFile);
        }
    }

    /// <summary>
    ///     Test that Run returns non-zero exit code when no config is given
    /// </summary>
    [Fact]
    public void Program_Run_WithNoConfig_ReturnsNonZeroExitCode()
    {
        // Arrange: create a silent context with no config file
        using var context = Context.Create(["--silent"]);

        // Act: run the program
        Program.Run(context);

        // Assert: verify non-zero exit code
        Assert.NotEqual(0, context.ExitCode);
    }

    /// <summary>
    ///     Verifies that Run dispatches to the self-validation runner when the validate flag is given,
    ///     and that self-validation actually executed and reported its results (not merely that the
    ///     process exited with code zero, which would also be true for a no-op dispatch).
    /// </summary>
    [Fact]
    public void Program_Run_WithValidateFlag_DispatchesToValidation()
    {
        // Arrange: create a log file to capture output; pass the validate flag and the mock
        // simulator so that no external simulator binary is required during the unit test
        var logFile = Path.GetTempFileName();
        try
        {
            using (var context = Context.Create(["--validate", "--simulator", "mock", "--log", logFile, "--silent"]))
            {
                // Act: run the program with the validate flag — must not throw an unhandled exception
                Program.Run(context);

                // Assert: the validate dispatch path completed; exit code is zero for the mock simulator
                Assert.Equal(0, context.ExitCode);
            }

            // Assert: the self-validation run actually executed and reported its results, proving
            // Validation.Run was dispatched to (rather than a no-op that happens to exit zero)
            var output = File.ReadAllText(logFile);
            Assert.True(output.Contains("Total Tests:"), $"Expected 'Total Tests:' in output:\n{output}");
            Assert.True(output.Contains("VHDLTest_TestPasses - Passed"), $"Expected 'VHDLTest_TestPasses - Passed' in output:\n{output}");
            Assert.True(output.Contains("VHDLTest_TestFails - Passed"), $"Expected 'VHDLTest_TestFails - Passed' in output:\n{output}");
            Assert.True(output.Contains("Validation Passed"), $"Expected 'Validation Passed' in output:\n{output}");
        }
        finally
        {
            File.Delete(logFile);
        }
    }

    /// <summary>
    ///     Verifies that Run executes the test suite successfully when a valid configuration file
    ///     and the mock simulator are provided.
    /// </summary>
    [Fact]
    public void Program_Run_RunTests_WithMockSimulator_RunsSuccessfully()
    {
        // Arrange: write a passing configuration file and create a silent context
        var configFile = Path.GetTempFileName();
        try
        {
            File.WriteAllText(configFile, PassingConfigContent);
            using var context = Context.Create(["-c", configFile, "--simulator", "mock", "--silent"]);

            // Act: run the program with the valid configuration
            Program.Run(context);

            // Assert: the test suite ran without errors and exit code is zero
            Assert.Equal(0, context.ExitCode);
        }
        finally
        {
            File.Delete(configFile);
        }
    }

    /// <summary>
    ///     Verifies that Run returns a non-zero exit code when tests fail and --exit-0 is not set.
    /// </summary>
    [Fact]
    public void Program_Run_FailureExitCode_WithFailingTests_ReturnsNonZeroExitCode()
    {
        // Arrange: write a configuration file whose test names trigger mock failures
        var configFile = Path.GetTempFileName();
        try
        {
            File.WriteAllText(configFile, FailingConfigContent);
            using var context = Context.Create(["-c", configFile, "--simulator", "mock", "--silent"]);

            // Act: run the program with failing tests and no --exit-0 flag
            Program.Run(context);

            // Assert: the exit code is non-zero because tests failed
            Assert.NotEqual(0, context.ExitCode);
        }
        finally
        {
            File.Delete(configFile);
        }
    }

    /// <summary>
    ///     Verifies that Run returns a zero exit code when tests fail but --exit-0 is set.
    /// </summary>
    [Fact]
    public void Program_Run_ExitZero_WithFailingTestsAndExitZeroFlag_ReturnsZeroExitCode()
    {
        // Arrange: write a configuration file whose test names trigger mock failures
        var configFile = Path.GetTempFileName();
        try
        {
            File.WriteAllText(configFile, FailingConfigContent);
            using var context = Context.Create(["-c", configFile, "--simulator", "mock", "--exit-0", "--silent"]);

            // Act: run the program with failing tests and the --exit-0 flag
            Program.Run(context);

            // Assert: the exit code is zero because --exit-0 suppresses failure exit codes
            Assert.Equal(0, context.ExitCode);
        }
        finally
        {
            File.Delete(configFile);
        }
    }

    /// <summary>
    ///     Verifies that Run writes an error and returns a non-zero exit code when an
    ///     invalid simulator name is given, exercising the InvalidOperationException catch
    ///     branch inside Run that handles operational errors such as a missing simulator.
    /// </summary>
    [Fact]
    public void Program_Run_WithInvalidSimulator_WritesErrorAndExits()
    {
        // Arrange: write a valid configuration file and specify an unrecognized simulator
        // name so that SimulatorFactory.Get returns null and Run throws
        // InvalidOperationException("Simulator not found") in its inner try/catch
        var configFile = Path.GetTempFileName();
        try
        {
            File.WriteAllText(configFile, PassingConfigContent);
            using var context = Context.Create(
                ["-c", configFile, "--simulator", "invalid-simulator-name", "--silent"]);

            // Act: run the program — must not throw; the exception is handled inside Run
            Program.Run(context);

            // Assert: an error was recorded and the exit code is non-zero
            Assert.NotEqual(0, context.ExitCode);
        }
        finally
        {
            File.Delete(configFile);
        }
    }

    /// <summary>
    ///     Verifies that Run creates the results file when the --results flag is given.
    /// </summary>
    [Fact]
    public void Program_Run_SaveResults_WithResultsFile_CreatesResultsFile()
    {
        // Arrange: write a passing configuration file and specify a results output file
        var configFile = Path.GetTempFileName();
        var resultsFile = Path.Combine(Path.GetTempPath(), $"results_{Guid.NewGuid():N}.trx");
        try
        {
            File.WriteAllText(configFile, PassingConfigContent);
            using var context = Context.Create(
                ["-c", configFile, "--simulator", "mock", "--results", resultsFile, "--silent"]);

            // Act: run the program with the --results flag
            Program.Run(context);

            // Assert: the results file was created
            Assert.True(File.Exists(resultsFile), $"Expected results file to be created at: {resultsFile}");
        }
        finally
        {
            File.Delete(configFile);
            if (File.Exists(resultsFile))
            {
                File.Delete(resultsFile);
            }
        }
    }

    /// <summary>
    ///     Verifies that Run executes only the named test when test names are supplied as
    ///     positional arguments, filtering out tests whose names were not specified.
    /// </summary>
    [Fact]
    public void Program_Run_WithTestFilter_RunsOnlyMatchingTests()
    {
        // Arrange: write a configuration file containing both a passing and a failing test;
        // the failing test name contains "_fail_" which triggers mock simulator failure
        const string mixedConfigContent =
            """
            files:
            - file1.vhd

            tests:
            - test1
            - test_fail_1
            """;
        var configFile = Path.GetTempFileName();
        var logFile = Path.Combine(Path.GetTempPath(), $"program_filter_test_{Guid.NewGuid():N}.log");
        try
        {
            File.WriteAllText(configFile, mixedConfigContent);

            // Arrange: pass only "test1" as the filter so "test_fail_1" is not executed; also
            // capture the run log so we can prove which tests actually ran, not merely that the
            // exit code was zero (a completely broken filter that runs zero tests would also
            // produce ExitCode == 0, since no failures would have occurred)
            int exitCode;
            using (var context = Context.Create(
                ["-c", configFile, "--simulator", "mock", "--log", logFile, "--silent", "test1"]))
            {
                // Act: run the program with the test filter
                Program.Run(context);
                exitCode = context.ExitCode;
            }

            // Assert: exit code is zero because only the passing test ran
            Assert.Equal(0, exitCode);

            // Assert: the log proves test1 actually ran and passed, and test_fail_1 was not
            // executed at all — closing the "zero tests ran" false-positive gap that a bare
            // ExitCode == 0 assertion alone would not catch
            var logContent = File.ReadAllText(logFile);
            Assert.Multiple(
                () => Assert.Contains("Passed test1", logContent),
                () => Assert.DoesNotContain("test_fail_1", logContent));
        }
        finally
        {
            File.Delete(configFile);
            if (File.Exists(logFile))
            {
                File.Delete(logFile);
            }
        }
    }
}

