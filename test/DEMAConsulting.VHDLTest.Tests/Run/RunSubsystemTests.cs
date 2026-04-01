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
/// Subsystem integration tests for the Run subsystem.
/// These tests verify that <see cref="RunProcessor"/>, <see cref="RunProgram"/>, and
/// <see cref="RunResults"/> work together to execute programs and classify their output.
/// </summary>
[TestClass]
public class RunSubsystemTests
{
    /// <summary>
    /// Test that RunProcessor executes a real program via RunProgram, and produces a
    /// RunResults object with correctly classified output lines.
    /// </summary>
    [TestMethod]
    public void RunSubsystem_ExecuteRealProgram_WithClassificationRules_ProducesClassifiedRunResults()
    {
        // Arrange - create a processor with an Info classification rule
        var processor = new RunProcessor(
        [
            RunLineRule.Create(RunLineType.Info, "Usage")
        ]);

        // Act - execute a real program through the full Run pipeline
        var results = processor.Execute("dotnet", "", "help");

        // Assert - RunProgram ran the program and RunProcessor classified the output into RunResults
        Assert.IsNotNull(results);
        Assert.AreEqual(0, results.ExitCode);
        Assert.IsTrue(results.Output.Length > 0);
        Assert.IsTrue(results.Lines.Count > 0);
        Assert.IsTrue(results.Lines.Any(l => l.Type == RunLineType.Info));
        Assert.IsTrue(results.Duration >= 0.0);
    }

    /// <summary>
    /// Test that RunProcessor correctly surfaces a non-zero exit code from RunProgram
    /// as an Error summary in the RunResults.
    /// </summary>
    [TestMethod]
    public void RunSubsystem_ExecuteRealProgram_WithErrorExitCode_ProducesErrorRunResults()
    {
        // Arrange - create a processor with no special classification rules
        var processor = new RunProcessor([]);

        // Act - execute dotnet with an unknown command to produce a non-zero exit code
        var results = processor.Execute("dotnet", "", "unknown-command");

        // Assert - RunResults reflects the non-zero exit code as an error summary
        Assert.IsNotNull(results);
        Assert.AreNotEqual(0, results.ExitCode);
        Assert.AreEqual(RunLineType.Error, results.Summary);
    }
}
