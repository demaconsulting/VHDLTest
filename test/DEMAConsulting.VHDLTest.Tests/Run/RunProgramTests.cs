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
}
