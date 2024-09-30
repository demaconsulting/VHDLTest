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

namespace DEMAConsulting.VHDLTest.Tests;

/// <summary>
/// Tests for argument parsing
/// </summary>
[TestClass]
public class TestArguments
{
    /// <summary>
    /// Test parsing arguments with no arguments
    /// </summary>
    [TestMethod]
    public void Test_Arguments_None()
    {
        // Parse the arguments
        var arguments = Arguments.Parse([]);

        // Check the arguments
        Assert.IsNotNull(arguments);
        Assert.IsNull(arguments.ConfigFile);
        Assert.IsNull(arguments.ResultsFile);
        Assert.IsNull(arguments.Simulator);
        Assert.IsFalse(arguments.Verbose);
        Assert.IsFalse(arguments.ExitZero);
        Assert.IsFalse(arguments.Validate);
        Assert.IsNull(arguments.CustomTests);
    }

    /// <summary>
    /// Test parsing arguments with unknown argument
    /// </summary>
    [TestMethod]
    public void Test_Arguments_Unknown()
    {
        // Verify the exception is thrown
        Assert.ThrowsException<InvalidOperationException>(() => Arguments.Parse(["--unknown"]));
    }

    /// <summary>
    /// Test parsing arguments with a config file
    /// </summary>
    [TestMethod]
    public void Test_Arguments_ConfigFile()
    {
        // Parse the arguments
        var arguments = Arguments.Parse(["-c", "config.json"]);

        // Check the arguments
        Assert.IsNotNull(arguments);
        Assert.AreEqual("config.json", arguments.ConfigFile);
        Assert.IsNull(arguments.ResultsFile);
        Assert.IsNull(arguments.Simulator);
        Assert.IsFalse(arguments.Verbose);
        Assert.IsFalse(arguments.ExitZero);
        Assert.IsFalse(arguments.Validate);
        Assert.IsNull(arguments.CustomTests);
    }

    /// <summary>
    /// Test parsing arguments with a missing config file
    /// </summary>
    [TestMethod]
    public void Test_Arguments_ConfigFile_Missing()
    {
        // Verify the exception is thrown
        Assert.ThrowsException<InvalidOperationException>(() => Arguments.Parse(["-c"]));
    }

    /// <summary>
    /// Test parsing arguments with a results file
    /// </summary>
    [TestMethod]
    public void Test_Arguments_ResultsFile()
    {
        // Parse the arguments
        var arguments = Arguments.Parse(["-r", "results.trx"]);

        // Check the arguments
        Assert.IsNotNull(arguments);
        Assert.IsNull(arguments.ConfigFile);
        Assert.AreEqual("results.trx", arguments.ResultsFile);
        Assert.IsNull(arguments.Simulator);
        Assert.IsFalse(arguments.Verbose);
        Assert.IsFalse(arguments.ExitZero);
        Assert.IsFalse(arguments.Validate);
        Assert.IsNull(arguments.CustomTests);
    }

    /// <summary>
    /// Test parsing arguments with a missing results file
    /// </summary>
    [TestMethod]
    public void Test_Arguments_ResultsFile_Missing()
    {
        // Verify the exception is thrown
        Assert.ThrowsException<InvalidOperationException>(() => Arguments.Parse(["-r"]));
    }

    /// <summary>
    /// Test parsing arguments with a specified simulator
    /// </summary>
    [TestMethod]
    public void Test_Arguments_Simulator()
    {
        // Parse the arguments
        var arguments = Arguments.Parse(["-s", "GHDL"]);

        // Check the arguments
        Assert.IsNotNull(arguments);
        Assert.IsNull(arguments.ConfigFile);
        Assert.IsNull(arguments.ResultsFile);
        Assert.AreEqual("GHDL", arguments.Simulator);
        Assert.IsFalse(arguments.Verbose);
        Assert.IsFalse(arguments.ExitZero);
        Assert.IsFalse(arguments.Validate);
        Assert.IsNull(arguments.CustomTests);
    }

    /// <summary>
    /// Test parsing arguments with a missing simulator
    /// </summary>
    [TestMethod]
    public void Test_Arguments_Simulator_Missing()
    {
        // Verify the exception is thrown
        Assert.ThrowsException<InvalidOperationException>(() => Arguments.Parse(["-s"]));
    }

    /// <summary>
    /// Test parsing arguments with verbose
    /// </summary>
    [TestMethod]
    public void Test_Arguments_Verbose()
    {
        // Parse the arguments
        var arguments = Arguments.Parse(["--verbose"]);

        // Check the arguments
        Assert.IsNotNull(arguments);
        Assert.IsNull(arguments.ConfigFile);
        Assert.IsNull(arguments.ResultsFile);
        Assert.IsNull(arguments.Simulator);
        Assert.IsTrue(arguments.Verbose);
        Assert.IsFalse(arguments.ExitZero);
        Assert.IsFalse(arguments.Validate);
        Assert.IsNull(arguments.CustomTests);
    }

    /// <summary>
    /// Test parsing arguments with exit-zero flag
    /// </summary>
    [TestMethod]
    public void Test_Arguments_ExitZero()
    {
        // Parse the arguments
        var arguments = Arguments.Parse(["--exit-0"]);

        // Check the arguments
        Assert.IsNotNull(arguments);
        Assert.IsNull(arguments.ConfigFile);
        Assert.IsNull(arguments.ResultsFile);
        Assert.IsNull(arguments.Simulator);
        Assert.IsFalse(arguments.Verbose);
        Assert.IsTrue(arguments.ExitZero);
        Assert.IsFalse(arguments.Validate);
        Assert.IsNull(arguments.CustomTests);
    }

    /// <summary>
    /// Test parsing arguments with validate flag
    /// </summary>
    [TestMethod]
    public void Test_Arguments_Validate()
    {
        // Parse the arguments
        var arguments = Arguments.Parse(["--validate"]);

        // Check the arguments
        Assert.IsNotNull(arguments);
        Assert.IsNull(arguments.ConfigFile);
        Assert.IsNull(arguments.ResultsFile);
        Assert.IsNull(arguments.Simulator);
        Assert.IsFalse(arguments.Verbose);
        Assert.IsFalse(arguments.ExitZero);
        Assert.IsTrue(arguments.Validate);
        Assert.IsNull(arguments.CustomTests);
    }

    /// <summary>
    /// Test parsing arguments with a custom test
    /// </summary>
    [TestMethod]
    public void Test_Arguments_CustomTest()
    {
        // Parse the arguments
        var arguments = Arguments.Parse(["custom_test"]);

        // Check the arguments
        Assert.IsNotNull(arguments);
        Assert.IsNull(arguments.ConfigFile);
        Assert.IsNull(arguments.ResultsFile);
        Assert.IsNull(arguments.Simulator);
        Assert.IsFalse(arguments.Verbose);
        Assert.IsFalse(arguments.ExitZero);
        Assert.IsFalse(arguments.Validate);
        Assert.IsNotNull(arguments.CustomTests);
        Assert.AreEqual(1, arguments.CustomTests.Count);
        Assert.AreEqual("custom_test", arguments.CustomTests[0]);
    }

    /// <summary>
    /// Test parsing arguments with multiple custom tests
    /// </summary>
    [TestMethod]
    public void Test_Arguments_CustomTests()
    {
        // Parse the arguments
        var arguments = Arguments.Parse(["--", "custom_test1", "custom_test2"]);

        // Check the arguments
        Assert.IsNotNull(arguments);
        Assert.IsNull(arguments.ConfigFile);
        Assert.IsNull(arguments.ResultsFile);
        Assert.IsNull(arguments.Simulator);
        Assert.IsFalse(arguments.Verbose);
        Assert.IsFalse(arguments.ExitZero);
        Assert.IsFalse(arguments.Validate);
        Assert.IsNotNull(arguments.CustomTests);
        Assert.AreEqual(2, arguments.CustomTests.Count);
        Assert.AreEqual("custom_test1", arguments.CustomTests[0]);
        Assert.AreEqual("custom_test2", arguments.CustomTests[1]);
    }
}