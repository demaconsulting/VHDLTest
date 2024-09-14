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

[TestClass]
public class TestUsage
{
    [TestMethod]
    public void UsageNoArguments()
    {
        // Run the application
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            "DEMAConsulting.VHDLTest.dll");

        // Verify error
        Assert.AreNotEqual(0, exitCode);

        // Verify usage reported
        StringAssert.Contains(output, "No arguments specified");
        StringAssert.Contains(output, "Usage: VHDLTest");
    }

    [TestMethod]
    public void UsageShort()
    {
        // Run the application
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            "DEMAConsulting.VHDLTest.dll",
            "-h");

        // Verify no error
        Assert.AreEqual(0, exitCode);

        // Verify usage reported
        StringAssert.Contains(output, "Usage: VHDLTest");
    }

    [TestMethod]
    public void UsageQuestionMark()
    {
        // Run the application
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            "DEMAConsulting.VHDLTest.dll",
            "-?");

        // Verify no error
        Assert.AreEqual(0, exitCode);

        // Verify usage reported
        StringAssert.Contains(output, "Usage: VHDLTest");
    }


    [TestMethod]
    public void UsageLong()
    {
        // Run the application
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            "DEMAConsulting.VHDLTest.dll",
            "--help");

        // Verify no error
        Assert.AreEqual(0, exitCode);

        // Verify usage reported
        StringAssert.Contains(output, "Usage: VHDLTest");
    }
}