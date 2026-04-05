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
[TestClass]
public class ProgramTests
{
    /// <summary>
    /// Test that the version string is not empty
    /// </summary>
    [TestMethod]
    public void Program_Version_IsNotEmpty()
    {
        // Assert - verify version is populated
        Assert.IsFalse(string.IsNullOrEmpty(Program.Version));
    }

    /// <summary>
    /// Test that Run returns zero exit code when version flag is given
    /// </summary>
    [TestMethod]
    public void Program_Run_WithVersionFlag_ReturnsZeroExitCode()
    {
        // Arrange - create a silent context with version flag
        using var context = Context.Create(["--version", "--silent"]);

        // Act - run the program
        Program.Run(context);

        // Assert - verify zero exit code
        Assert.AreEqual(0, context.ExitCode);
    }

    /// <summary>
    /// Test that Run returns zero exit code when help flag is given
    /// </summary>
    [TestMethod]
    public void Program_Run_WithHelpFlag_ReturnsZeroExitCode()
    {
        // Arrange - create a silent context with help flag
        using var context = Context.Create(["--help", "--silent"]);

        // Act - run the program
        Program.Run(context);

        // Assert - verify zero exit code
        Assert.AreEqual(0, context.ExitCode);
    }

    /// <summary>
    /// Test that Run displays usage information when no configuration file argument is provided
    /// </summary>
    [TestMethod]
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
            Assert.IsTrue(output.Contains("Usage: VHDLTest"), $"Expected 'Usage: VHDLTest' in output:\n{output}");
        }
        finally
        {
            File.Delete(logFile);
        }
    }

    /// <summary>
    /// Test that Run returns non-zero exit code when no config is given
    /// </summary>
    [TestMethod]
    public void Program_Run_WithNoConfig_ReturnsNonZeroExitCode()
    {
        // Arrange - create a silent context with no config file
        using var context = Context.Create(["--silent"]);

        // Act - run the program
        Program.Run(context);

        // Assert - verify non-zero exit code
        Assert.AreNotEqual(0, context.ExitCode);
    }
}
