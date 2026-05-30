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

using DEMAConsulting.VHDLTest.Run;

namespace DEMAConsulting.VHDLTest.Tests.Run;

/// <summary>
///     Unit tests for the <see cref="RunProgram"/> class.
/// </summary>
public class RunProgramTests
{
    /// <summary>
    ///     Verifies that running a valid program returns exit code 0 and non-empty output.
    /// </summary>
    [Fact]
    public void RunProgram_Run_ValidProgram_ReturnsZeroExitCodeAndNonEmptyOutput()
    {
        // Arrange: dotnet is always available; "help" exits 0 and produces usage output

        // Act: launch dotnet help through RunProgram
        var exitCode = RunProgram.Run(out var output, "dotnet", "", "help");

        // Assert: exit code is 0 and output contains usage text
        Assert.Equal(0, exitCode);
        Assert.NotEmpty(output);
    }

    /// <summary>
    ///     Verifies that running a program that exits with a non-zero code returns that code.
    /// </summary>
    [Fact]
    public void RunProgram_Run_ProgramWithNonZeroExit_ReturnsNonZeroExitCode()
    {
        // Arrange: dotnet with an unknown command exits non-zero

        // Act: launch dotnet with an unrecognized command
        var exitCode = RunProgram.Run(out _, "dotnet", "", "unknown-command");

        // Assert: the non-zero exit code is returned to the caller
        Assert.NotEqual(0, exitCode);
    }

    /// <summary>
    ///     Verifies that stderr output from the launched program is captured and included in
    ///     the combined output string.
    /// </summary>
    [Fact]
    public void RunProgram_Run_ProgramWithStderr_CapturesBothStreamsInOutput()
    {
        // Arrange: dotnet with an unrecognized command writes a recognizable error to stderr

        // Act: launch dotnet with an unknown command that produces stderr output
        RunProgram.Run(out var output, "dotnet", "", "unknown-command");

        // Assert: the combined output includes text from stderr; the unknown command name
        // appears in dotnet's error message confirming stderr was captured
        Assert.Contains("unknown", output, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    ///     Verifies that passing an empty string as the working directory causes the launched
    ///     process to inherit the caller's current working directory.
    /// </summary>
    [Fact]
    public void RunProgram_Run_EmptyWorkingDirectory_UsesCallerCurrentDirectory()
    {
        // Arrange: record the caller's current working directory and choose a platform-appropriate
        // program that prints its working directory to stdout
        var expectedCwd = Directory.GetCurrentDirectory();
        string application;
        string[] arguments;
        if (OperatingSystem.IsWindows())
        {
            application = "cmd.exe";
            arguments = ["/c", "cd"];
        }
        else
        {
            application = "pwd";
            arguments = [];
        }

        // Act: launch the program with an empty working directory so it inherits the caller's CWD
        var exitCode = RunProgram.Run(out var output, application, "", arguments);

        // Assert: the program ran successfully and its printed directory matches the caller's CWD
        Assert.Equal(0, exitCode);
        Assert.Contains(expectedCwd, output.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    ///     Verifies that attempting to run a non-existent executable propagates an exception
    ///     to the caller rather than returning a silent error code.
    /// </summary>
    [Fact]
    public void RunProgram_Run_MissingExecutable_ThrowsException()
    {
        // Arrange: use a path that cannot correspond to any real executable
        const string missingApplication = "this-executable-does-not-exist-vhdltest-test";

        // Act + Assert: launching a missing executable must throw an exception
        Assert.ThrowsAny<Exception>(() => RunProgram.Run(out _, missingApplication, ""));
    }
}
