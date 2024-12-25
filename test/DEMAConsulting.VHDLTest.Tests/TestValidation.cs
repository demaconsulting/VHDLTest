﻿// Copyright (c) 2023 DEMA Consulting
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

namespace DEMAConsulting.VHDLTest.Tests;

/// <summary>
/// Tests for validation
/// </summary>
[TestClass]
public class TestValidation
{
    /// <summary>
    /// Test usage information is reported when no arguments are specified
    /// </summary>
    [TestMethod]
    public void Validation()
    {
        // Run the application
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            "DEMAConsulting.VHDLTest.dll",
            "--simulator", "mock",
            "--validate");

        // Verify success
        Assert.AreEqual(0, exitCode);

        // Verify validation passed
        StringAssert.Contains(output, "Validation Passed");
    }

    /// <summary>
    /// Test usage information is reported when no arguments are specified
    /// </summary>
    [TestMethod]
    public void Validation_Depth()
    {
        // Run the application
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            "DEMAConsulting.VHDLTest.dll",
            "--simulator", "mock",
            "--validate",
            "--depth", "3");

        // Verify success
        Assert.AreEqual(0, exitCode);

        // Verify validation depth
        StringAssert.Contains(output, "### DEMAConsulting.VHDLTest");
    }

    /// <summary>
    /// Test validation results can be saved to file
    /// </summary>
    [TestMethod]
    public void Validation_Results()
    {
        try
        {
            // Run the application
            var exitCode = Runner.Run(
                out _,
                "dotnet",
                "DEMAConsulting.VHDLTest.dll",
                "--simulator", "mock",
                "--validate",
                "--results", "validation_results.trx");

            // Verify success
            Assert.AreEqual(0, exitCode);

            // Verify results file written
            Assert.IsTrue(File.Exists("validation_results.trx"));

            // Read the results file.
            var text = File.ReadAllText("validation_results.trx");
            StringAssert.Contains(text, """<TestMethod codeBase="VHDLTest" className="VHDLTest.Validation" name="VHDLTest_TestPasses" />""");
            StringAssert.Contains(text, """<TestMethod codeBase="VHDLTest" className="VHDLTest.Validation" name="VHDLTest_TestFails" />""");
            StringAssert.Contains(text, """<Counters total="2" executed="2" passed="2" failed="0" />""");
        }
        finally
        {
            // Delete results file
            File.Delete("validation_results.trx");
        }
    }
}