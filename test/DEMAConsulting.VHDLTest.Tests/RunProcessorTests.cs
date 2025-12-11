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

namespace DEMAConsulting.VHDLTest.Tests;

/// <summary>
/// Tests for <see cref="RunProcessor"/> class.
/// </summary>
[TestClass]
public class RunProcessorTests
{
    /// <summary>
    /// Test running a missing program.
    /// </summary>
    [TestMethod]
    public void Test_RunProcessor_Missing()
    {
        // Construct the processor
        var processor = new RunProcessor(
        [
            RunLineRule.Create(RunLineType.Error, "Error")
        ]);

        // Run unknown program
        try
        {
            processor.Execute("unknown-program");
            Assert.Fail("Expected exception");
        }
        catch (Exception)
        {
            // Expected
        }
    }

    /// <summary>
    /// Test running a program with an error.
    /// </summary>
    [TestMethod]
    public void Test_RunProcessor_Error()
    {
        // Construct the processor
        var processor = new RunProcessor(
        [
            RunLineRule.Create(RunLineType.Error, "Error")
        ]);

        // Run dotnet with unknown command
        var result = processor.Execute("dotnet", "", "unknown-command");

        // Check the result
        Assert.AreEqual(RunLineType.Error, result.Summary);
    }

    /// <summary>
    /// Test running a program producing passing output.
    /// </summary>
    [TestMethod]
    public void Test_RunProcessor_Pass()
    {
        // Construct the processor
        var processor = new RunProcessor(
        [
            RunLineRule.Create(RunLineType.Info, "Usage")
        ]);

        // Run dotnet with help command
        var result = processor.Execute("dotnet", "", "help");

        // Check the result
        Assert.AreEqual(RunLineType.Info, result.Summary);
    }
}