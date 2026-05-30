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

namespace DEMAConsulting.VHDLTest.Tests.Cli;

/// <summary>
/// Tests for the Context class
/// </summary>
public class ContextTests
{
    /// <summary>
    /// Test parsing arguments with no arguments
    /// </summary>
    [Fact]
    public void Context_Create_NoArguments_ReturnsDefaultContext()
    {
        // Arrange: Prepare empty arguments array

        // Act: Parse the arguments
        using var arguments = Context.Create([]);

        // Assert: Verify default context is returned with all properties set to defaults
        Assert.NotNull(arguments);
        Assert.Null(arguments.ConfigFile);
        Assert.Null(arguments.ResultsFile);
        Assert.Null(arguments.Simulator);
        Assert.False(arguments.Version);
        Assert.False(arguments.Help);
        Assert.False(arguments.Silent);
        Assert.False(arguments.Verbose);
        Assert.False(arguments.ExitZero);
        Assert.False(arguments.Validate);
        Assert.Equal(1, arguments.Depth);
        Assert.Null(arguments.CustomTests);
    }

    /// <summary>
    /// Test parsing arguments with unknown argument
    /// </summary>
    [Fact]
    public void Context_Create_UnknownArgument_ThrowsInvalidOperationException()
    {
        // Arrange - no setup required; the invalid/missing argument is passed directly to the act step

        // Act & Assert - Verify InvalidOperationException is thrown for unknown argument
        Assert.Throws<InvalidOperationException>(() => Context.Create(["--unknown"]));
    }

    /// <summary>
    /// Test parsing arguments with a config file
    /// </summary>
    [Fact]
    public void Context_Create_WithConfigFile_SetsConfigFile()
    {
        // Arrange: Prepare arguments with config file option

        // Act: Parse the arguments
        using var arguments = Context.Create(["-c", "config.json"]);

        // Assert: Verify config file is set and other properties are defaults
        Assert.NotNull(arguments);
        Assert.Equal("config.json", arguments.ConfigFile);
        Assert.Null(arguments.ResultsFile);
        Assert.Null(arguments.Simulator);
        Assert.False(arguments.Verbose);
        Assert.False(arguments.ExitZero);
        Assert.False(arguments.Validate);
        Assert.Null(arguments.CustomTests);
    }

    /// <summary>
    /// Test parsing arguments with a missing config file
    /// </summary>
    [Fact]
    public void Context_Create_MissingConfigValue_ThrowsInvalidOperationException()
    {
        // Arrange - no setup required; the invalid/missing argument is passed directly to the act step

        // Act & Assert - Verify InvalidOperationException is thrown for missing config value
        Assert.Throws<InvalidOperationException>(() => Context.Create(["-c"]));
    }

    /// <summary>
    /// Test parsing arguments with a results file
    /// </summary>
    [Fact]
    public void Context_Create_WithResultsFile_SetsResultsFile()
    {
        // Arrange: Prepare arguments with results file option

        // Act: Parse the arguments
        using var arguments = Context.Create(["-r", "results.trx"]);

        // Assert: Verify results file is set and other properties are defaults
        Assert.NotNull(arguments);
        Assert.Null(arguments.ConfigFile);
        Assert.Equal("results.trx", arguments.ResultsFile);
        Assert.Null(arguments.Simulator);
        Assert.False(arguments.Verbose);
        Assert.False(arguments.ExitZero);
        Assert.False(arguments.Validate);
        Assert.Null(arguments.CustomTests);
    }

    /// <summary>
    /// Test parsing arguments with a missing results file
    /// </summary>
    [Fact]
    public void Context_Create_MissingResultsValue_ThrowsInvalidOperationException()
    {
        // Arrange - no setup required; the invalid/missing argument is passed directly to the act step

        // Act & Assert - Verify InvalidOperationException is thrown for missing results value
        Assert.Throws<InvalidOperationException>(() => Context.Create(["-r"]));
    }

    /// <summary>
    /// Test parsing arguments with a specified simulator
    /// </summary>
    [Fact]
    public void Context_Create_WithSimulator_SetsSimulator()
    {
        // Arrange: Prepare arguments with simulator option

        // Act: Parse the arguments
        using var arguments = Context.Create(["-s", "GHDL"]);

        // Assert: Verify simulator is set and other properties are defaults
        Assert.NotNull(arguments);
        Assert.Null(arguments.ConfigFile);
        Assert.Null(arguments.ResultsFile);
        Assert.Equal("GHDL", arguments.Simulator);
        Assert.False(arguments.Verbose);
        Assert.False(arguments.ExitZero);
        Assert.False(arguments.Validate);
        Assert.Null(arguments.CustomTests);
    }

    /// <summary>
    /// Test parsing arguments with a missing simulator
    /// </summary>
    [Fact]
    public void Context_Create_MissingSimulatorValue_ThrowsInvalidOperationException()
    {
        // Arrange - no setup required; the invalid/missing argument is passed directly to the act step

        // Act & Assert - Verify InvalidOperationException is thrown for missing simulator value
        Assert.Throws<InvalidOperationException>(() => Context.Create(["-s"]));
    }

    /// <summary>
    /// Test parsing arguments with verbose
    /// </summary>
    [Fact]
    public void Context_Create_WithVerbose_SetsVerboseFlag()
    {
        // Arrange: Prepare arguments with verbose flag

        // Act: Parse the arguments
        using var arguments = Context.Create(["--verbose"]);

        // Assert: Verify verbose flag is set and other properties are defaults
        Assert.NotNull(arguments);
        Assert.Null(arguments.ConfigFile);
        Assert.Null(arguments.ResultsFile);
        Assert.Null(arguments.Simulator);
        Assert.True(arguments.Verbose);
        Assert.False(arguments.ExitZero);
        Assert.False(arguments.Validate);
        Assert.Null(arguments.CustomTests);
    }

    /// <summary>
    /// Test parsing arguments with exit-zero flag
    /// </summary>
    [Fact]
    public void Context_Create_WithExitZero_SetsExitZeroFlag()
    {
        // Arrange: Prepare arguments with exit-zero flag

        // Act: Parse the arguments
        using var arguments = Context.Create(["--exit-0"]);

        // Assert: Verify exit-zero flag is set and other properties are defaults
        Assert.NotNull(arguments);
        Assert.Null(arguments.ConfigFile);
        Assert.Null(arguments.ResultsFile);
        Assert.Null(arguments.Simulator);
        Assert.False(arguments.Verbose);
        Assert.True(arguments.ExitZero);
        Assert.False(arguments.Validate);
        Assert.Null(arguments.CustomTests);
    }

    /// <summary>
    /// Test parsing arguments with validate flag
    /// </summary>
    [Fact]
    public void Context_Create_WithValidate_SetsValidateFlag()
    {
        // Arrange: Prepare arguments with validate flag

        // Act: Parse the arguments
        using var arguments = Context.Create(["--validate"]);

        // Assert: Verify validate flag is set and other properties are defaults
        Assert.NotNull(arguments);
        Assert.Null(arguments.ConfigFile);
        Assert.Null(arguments.ResultsFile);
        Assert.Null(arguments.Simulator);
        Assert.False(arguments.Verbose);
        Assert.False(arguments.ExitZero);
        Assert.True(arguments.Validate);
        Assert.Null(arguments.CustomTests);
    }

    /// <summary>
    /// Test parsing arguments with a valid depth value
    /// </summary>
    [Fact]
    public void Context_Create_WithDepth_SetsDepth()
    {
        // Arrange: no setup required

        // Act: Parse the arguments
        using var arguments = Context.Create(["--depth", "2"]);

        // Assert: Verify depth is set
        Assert.NotNull(arguments);
        Assert.Equal(2, arguments.Depth);
    }

    /// <summary>
    /// Test parsing arguments with a non-integer depth value
    /// </summary>
    [Fact]
    public void Context_Create_WithInvalidDepth_ThrowsInvalidOperationException()
    {
        // Arrange: no setup required

        // Act & Assert - Verify InvalidOperationException is thrown for non-integer depth
        Assert.Throws<InvalidOperationException>(() => Context.Create(["--depth", "abc"]));
    }

    /// <summary>
    /// Test parsing arguments with a zero depth value (out of range)
    /// </summary>
    [Fact]
    public void Context_Create_WithZeroDepth_ThrowsInvalidOperationException()
    {
        // Arrange: no setup required

        // Act & Assert - Verify InvalidOperationException is thrown for zero depth
        Assert.Throws<InvalidOperationException>(() => Context.Create(["--depth", "0"]));
    }

    /// <summary>
    /// Test parsing arguments with a negative depth value (out of range)
    /// </summary>
    [Fact]
    public void Context_Create_WithNegativeDepth_ThrowsInvalidOperationException()
    {
        // Arrange: no setup required

        // Act & Assert - Verify InvalidOperationException is thrown for negative depth
        Assert.Throws<InvalidOperationException>(() => Context.Create(["--depth", "-1"]));
    }

    /// <summary>
    /// Test parsing arguments with a custom test
    /// </summary>
    [Fact]
    public void Context_Create_WithCustomTest_SetsCustomTest()
    {
        // Arrange: Prepare arguments with single custom test name

        // Act: Parse the arguments
        using var arguments = Context.Create(["custom_test"]);

        // Assert: Verify custom test is set and other properties are defaults
        Assert.NotNull(arguments);
        Assert.Null(arguments.ConfigFile);
        Assert.Null(arguments.ResultsFile);
        Assert.Null(arguments.Simulator);
        Assert.False(arguments.Verbose);
        Assert.False(arguments.ExitZero);
        Assert.False(arguments.Validate);
        Assert.NotNull(arguments.CustomTests);
        Assert.Single(arguments.CustomTests);
        Assert.Equal("custom_test", arguments.CustomTests[0]);
    }

    /// <summary>
    /// Test parsing arguments with multiple custom tests
    /// </summary>
    [Fact]
    public void Context_Create_WithCustomTests_SetsCustomTests()
    {
        // Arrange: Prepare arguments with multiple custom test names

        // Act: Parse the arguments
        using var arguments = Context.Create(["--", "custom_test1", "custom_test2"]);

        // Assert: Verify all custom tests are set and other properties are defaults
        Assert.NotNull(arguments);
        Assert.Null(arguments.ConfigFile);
        Assert.Null(arguments.ResultsFile);
        Assert.Null(arguments.Simulator);
        Assert.False(arguments.Verbose);
        Assert.False(arguments.ExitZero);
        Assert.False(arguments.Validate);
        Assert.NotNull(arguments.CustomTests);
        Assert.Equal(2, arguments.CustomTests.Count);
        Assert.Equal("custom_test1", arguments.CustomTests[0]);
        Assert.Equal("custom_test2", arguments.CustomTests[1]);
    }

    /// <summary>
    /// Test parsing arguments with version flag (long form)
    /// </summary>
    [Fact]
    public void Context_Create_WithVersionFlag_SetsVersionFlag()
    {
        // Arrange: no setup required

        // Act: parse the arguments
        using var arguments = Context.Create(["--version"]);

        // Assert: verify version flag is set
        Assert.NotNull(arguments);
        Assert.True(arguments.Version);
    }

    /// <summary>
    /// Test parsing arguments with version flag (short form)
    /// </summary>
    [Fact]
    public void Context_Create_WithShortVersionFlag_SetsVersionFlag()
    {
        // Arrange: no setup required

        // Act: parse the arguments
        using var arguments = Context.Create(["-v"]);

        // Assert: verify version flag is set
        Assert.NotNull(arguments);
        Assert.True(arguments.Version);
    }

    /// <summary>
    /// Test parsing arguments with help flag (long form)
    /// </summary>
    [Fact]
    public void Context_Create_WithHelpFlag_SetsHelpFlag()
    {
        // Arrange: no setup required

        // Act: parse the arguments
        using var arguments = Context.Create(["--help"]);

        // Assert: verify help flag is set
        Assert.NotNull(arguments);
        Assert.True(arguments.Help);
    }

    /// <summary>
    /// Test parsing arguments with help flag (short form -h)
    /// </summary>
    [Fact]
    public void Context_Create_WithShortHelpFlag_SetsHelpFlag()
    {
        // Arrange: no setup required

        // Act: parse the arguments
        using var arguments = Context.Create(["-h"]);

        // Assert: verify help flag is set
        Assert.NotNull(arguments);
        Assert.True(arguments.Help);
    }

    /// <summary>
    /// Test parsing arguments with help flag (short form -?)
    /// </summary>
    [Fact]
    public void Context_Create_WithQuestionHelpFlag_SetsHelpFlag()
    {
        // Arrange: no setup required

        // Act: parse the arguments
        using var arguments = Context.Create(["-?"]);

        // Assert: verify help flag is set
        Assert.NotNull(arguments);
        Assert.True(arguments.Help);
    }

    /// <summary>
    /// Test parsing arguments with silent flag
    /// </summary>
    [Fact]
    public void Context_Create_WithSilentFlag_SetsSilentFlag()
    {
        // Arrange: no setup required

        // Act: parse the arguments
        using var arguments = Context.Create(["--silent"]);

        // Assert: verify silent flag is set
        Assert.NotNull(arguments);
        Assert.True(arguments.Silent);
    }

    /// <summary>
    /// Test that ExitCode returns zero when no errors have been reported
    /// </summary>
    [Fact]
    public void Context_ExitCode_NoErrors_ReturnsZero()
    {
        // Arrange: create a context with no arguments
        using var context = Context.Create([]);

        // Act: read the exit code without writing any errors
        var exitCode = context.ExitCode;

        // Assert: verify exit code is zero when no errors have been reported
        Assert.Equal(0, exitCode);
    }

    /// <summary>
    /// Test that the log option writes output to the specified log file
    /// </summary>
    [Fact]
    public void Context_Create_WithLogOption_WritesToLogFile()
    {
        // Arrange: create a temporary log file path
        var logFile = Path.GetTempFileName();
        try
        {
            using (var arguments = Context.Create(["-l", logFile]))
            {
                // Act: write a line through the context
                arguments.WriteLine("test output");
            }

            // Assert: verify the output was written to the log file
            var content = File.ReadAllText(logFile);
            Assert.Contains("test output", content);
        }
        finally
        {
            File.Delete(logFile);
        }
    }

    /// <summary>
    /// Test that the --log long-form option writes output to the specified log file
    /// </summary>
    [Fact]
    public void Context_Create_WithLongLogOption_WritesToLogFile()
    {
        // Arrange: create a temporary log file path
        var logFile = Path.GetTempFileName();
        try
        {
            using (var arguments = Context.Create(["--log", logFile]))
            {
                // Act: write a line through the context
                arguments.WriteLine("test output");
            }

            // Assert: verify the output was written to the log file
            var content = File.ReadAllText(logFile);
            Assert.Contains("test output", content);
        }
        finally
        {
            File.Delete(logFile);
        }
    }

    /// <summary>
    /// Test that WriteVerboseLine writes to the log file when verbose mode is active
    /// </summary>
    [Fact]
    public void Context_WriteVerboseLine_VerboseMode_WritesToLog()
    {
        // Arrange: create a context with verbose mode and a log file
        var logFile = Path.GetTempFileName();
        try
        {
            using (var arguments = Context.Create(["--verbose", "-l", logFile]))
            {
                // Act: write a verbose line
                arguments.WriteVerboseLine("verbose output");
            }

            // Assert: verify the verbose output was written to the log file
            var content = File.ReadAllText(logFile);
            Assert.Contains("verbose output", content);
        }
        finally
        {
            File.Delete(logFile);
        }
    }

    /// <summary>
    /// Test that WriteVerboseLine produces no output when verbose mode is not active
    /// </summary>
    [Fact]
    public void Context_WriteVerboseLine_NonVerboseMode_ProducesNoOutput()
    {
        // Arrange: create a context without verbose mode but with a log file
        var logFile = Path.GetTempFileName();
        try
        {
            using (var arguments = Context.Create(["-l", logFile]))
            {
                // Act: write a verbose line without verbose mode enabled
                arguments.WriteVerboseLine("verbose output");
            }

            // Assert: verify nothing was written to the log file
            var content = File.ReadAllText(logFile);
            Assert.Empty(content);
        }
        finally
        {
            File.Delete(logFile);
        }
    }

    /// <summary>
    /// Test that WriteError increments the error counter
    /// </summary>
    [Fact]
    public void Context_WriteError_WithMessage_IncrementsErrors()
    {
        // Arrange: create a silent context (to suppress console output)
        using var arguments = Context.Create(["--silent"]);

        // Act: write an error message
        arguments.WriteError("test error");

        // Assert: verify the error counter was incremented and exit code reflects the error
        Assert.Equal(1, arguments.Errors);
        Assert.NotEqual(0, arguments.ExitCode);
    }

    /// <summary>
    /// Test that WriteError writes to the log file even when silent mode is enabled
    /// </summary>
    [Fact]
    public void Context_WriteError_SilentMode_WritesToLogFile()
    {
        // Arrange: create a silent context with a log file
        var logFile = Path.GetTempFileName();
        try
        {
            using (var arguments = Context.Create(["--silent", "-l", logFile]))
            {
                // Act: write an error through the context
                arguments.WriteError("test error");
            }

            // Assert: verify the error was written to the log file despite silent mode
            var content = File.ReadAllText(logFile);
            Assert.Contains("test error", content);
        }
        finally
        {
            File.Delete(logFile);
        }
    }

    /// <summary>
    /// Test that Write sends output to the log file even when silent mode is enabled
    /// </summary>
    [Fact]
    public void Context_Write_SilentMode_WritesToLogFile()
    {
        // Arrange: create a silent context with a log file
        var logFile = Path.GetTempFileName();
        try
        {
            using (var arguments = Context.Create(["--silent", "-l", logFile]))
            {
                // Act: write colored text through the context
                arguments.Write(ConsoleColor.White, "silent output");
            }

            // Assert: verify the text was written to the log file despite silent mode
            var content = File.ReadAllText(logFile);
            Assert.Contains("silent output", content);
        }
        finally
        {
            File.Delete(logFile);
        }
    }

    /// <summary>
    /// Test that Write sends output to the log file
    /// </summary>
    [Fact]
    public void Context_Write_WithLogFile_WritesToLog()
    {
        // Arrange: create a context with a log file
        var logFile = Path.GetTempFileName();
        try
        {
            using (var arguments = Context.Create(["-l", logFile]))
            {
                // Act: write colored text through the context
                arguments.Write(ConsoleColor.White, "colored output");
            }

            // Assert: verify the text was written to the log file
            var content = File.ReadAllText(logFile);
            Assert.Contains("colored output", content);
        }
        finally
        {
            File.Delete(logFile);
        }
    }

    /// <summary>
    /// Test that Write outputs to the console when not in silent mode
    /// </summary>
    [Fact]
    public void Context_Write_NonSilentMode_WritesToConsole()
    {
        // Arrange: redirect standard output to capture console writes
        var writer = new StringWriter();
        var original = Console.Out;
        Console.SetOut(writer);
        try
        {
            using var context = Context.Create([]);

            // Act: write colored text without silent mode active
            context.Write(ConsoleColor.White, "console output");
        }
        finally
        {
            Console.SetOut(original);
        }

        // Assert: verify the output reached the console
        Assert.Contains("console output", writer.ToString());
    }

    /// <summary>
    /// Test that WriteLine outputs to the console when not in silent mode
    /// </summary>
    [Fact]
    public void Context_WriteLine_NonSilentMode_WritesToConsole()
    {
        // Arrange: redirect standard output to capture console writes
        var writer = new StringWriter();
        var original = Console.Out;
        Console.SetOut(writer);
        try
        {
            using var context = Context.Create([]);

            // Act: write a line without silent mode active
            context.WriteLine("console line output");
        }
        finally
        {
            Console.SetOut(original);
        }

        // Assert: verify the line reached the console
        Assert.Contains("console line output", writer.ToString());
    }

    /// <summary>
    /// Test that WriteError outputs to the console when not in silent mode
    /// </summary>
    [Fact]
    public void Context_WriteError_NonSilentMode_WritesToConsole()
    {
        // Arrange: redirect standard output to capture console writes
        var writer = new StringWriter();
        var original = Console.Out;
        Console.SetOut(writer);
        try
        {
            using var context = Context.Create([]);

            // Act: write an error without silent mode active
            context.WriteError("error output");
        }
        finally
        {
            Console.SetOut(original);
        }

        // Assert: verify the error reached the console
        Assert.Contains("error output", writer.ToString());
    }

    /// <summary>
    /// Test that WriteLine writes to the log file even when silent mode is enabled
    /// </summary>
    [Fact]
    public void Context_WriteLine_SilentMode_WritesToLogFile()
    {
        // Arrange: create a silent context with a log file
        var logFile = Path.GetTempFileName();
        try
        {
            using (var arguments = Context.Create(["--silent", "-l", logFile]))
            {
                // Act: write a line through the context while in silent mode
                arguments.WriteLine("silent line output");
            }

            // Assert: verify the line was written to the log file despite silent mode
            var content = File.ReadAllText(logFile);
            Assert.Contains("silent line output", content);
        }
        finally
        {
            File.Delete(logFile);
        }
    }

    /// <summary>
    /// Test that Context.Create throws InvalidOperationException when -l/--log is provided without a value
    /// </summary>
    [Fact]
    public void Context_Create_MissingLogValue_ThrowsInvalidOperationException()
    {
        // Arrange - no setup required; the missing value is the scenario under test

        // Act & Assert - verify InvalidOperationException is thrown for missing log value
        Assert.Throws<InvalidOperationException>(() => Context.Create(["-l"]));
    }

    /// <summary>
    /// Test that Context.Create throws InvalidOperationException when --depth is provided without a value
    /// </summary>
    [Fact]
    public void Context_Create_MissingDepthValue_ThrowsInvalidOperationException()
    {
        // Arrange - no setup required; the missing value is the scenario under test

        // Act & Assert - verify InvalidOperationException is thrown for missing depth value
        Assert.Throws<InvalidOperationException>(() => Context.Create(["--depth"]));
    }
}
