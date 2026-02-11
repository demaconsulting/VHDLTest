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
public class ContextTests
{
    /// <summary>
    /// Test parsing arguments with no arguments
    /// </summary>
    [TestMethod]
    public void Context_Create_NoArguments_ReturnsDefaultContext()
    {
        // Arrange - Prepare empty arguments array

        // Act - Parse the arguments
        var arguments = Context.Create([]);

        // Assert - Verify default context is returned with all properties set to defaults
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
    public void Context_Create_UnknownArgument_ThrowsInvalidOperationException()
    {
        // Act & Assert - Verify InvalidOperationException is thrown for unknown argument
        Assert.ThrowsExactly<InvalidOperationException>(() => Context.Create(["--unknown"]));
    }

    /// <summary>
    /// Test parsing arguments with a config file
    /// </summary>
    [TestMethod]
    public void Context_Create_WithConfigFile_SetsConfigFile()
    {
        // Arrange - Prepare arguments with config file option

        // Act - Parse the arguments
        var arguments = Context.Create(["-c", "config.json"]);

        // Assert - Verify config file is set and other properties are defaults
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
    public void Context_Create_MissingConfigValue_ThrowsInvalidOperationException()
    {
        // Act & Assert - Verify InvalidOperationException is thrown for missing config value
        Assert.ThrowsExactly<InvalidOperationException>(() => Context.Create(["-c"]));
    }

    /// <summary>
    /// Test parsing arguments with a results file
    /// </summary>
    [TestMethod]
    public void Context_Create_WithResultsFile_SetsResultsFile()
    {
        // Arrange - Prepare arguments with results file option

        // Act - Parse the arguments
        var arguments = Context.Create(["-r", "results.trx"]);

        // Assert - Verify results file is set and other properties are defaults
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
    public void Context_Create_MissingResultsValue_ThrowsInvalidOperationException()
    {
        // Act & Assert - Verify InvalidOperationException is thrown for missing results value
        Assert.ThrowsExactly<InvalidOperationException>(() => Context.Create(["-r"]));
    }

    /// <summary>
    /// Test parsing arguments with a specified simulator
    /// </summary>
    [TestMethod]
    public void Context_Create_WithSimulator_SetsSimulator()
    {
        // Arrange - Prepare arguments with simulator option

        // Act - Parse the arguments
        var arguments = Context.Create(["-s", "GHDL"]);

        // Assert - Verify simulator is set and other properties are defaults
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
    public void Context_Create_MissingSimulatorValue_ThrowsInvalidOperationException()
    {
        // Act & Assert - Verify InvalidOperationException is thrown for missing simulator value
        Assert.ThrowsExactly<InvalidOperationException>(() => Context.Create(["-s"]));
    }

    /// <summary>
    /// Test parsing arguments with verbose
    /// </summary>
    [TestMethod]
    public void Context_Create_WithVerbose_SetsVerboseFlag()
    {
        // Arrange - Prepare arguments with verbose flag

        // Act - Parse the arguments
        var arguments = Context.Create(["--verbose"]);

        // Assert - Verify verbose flag is set and other properties are defaults
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
    public void Context_Create_WithExitZero_SetsExitZeroFlag()
    {
        // Arrange - Prepare arguments with exit-zero flag

        // Act - Parse the arguments
        var arguments = Context.Create(["--exit-0"]);

        // Assert - Verify exit-zero flag is set and other properties are defaults
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
    public void Context_Create_WithValidate_SetsValidateFlag()
    {
        // Arrange - Prepare arguments with validate flag

        // Act - Parse the arguments
        var arguments = Context.Create(["--validate"]);

        // Assert - Verify validate flag is set and other properties are defaults
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
    public void Context_Create_WithCustomTest_SetsCustomTest()
    {
        // Arrange - Prepare arguments with single custom test name

        // Act - Parse the arguments
        var arguments = Context.Create(["custom_test"]);

        // Assert - Verify custom test is set and other properties are defaults
        Assert.IsNotNull(arguments);
        Assert.IsNull(arguments.ConfigFile);
        Assert.IsNull(arguments.ResultsFile);
        Assert.IsNull(arguments.Simulator);
        Assert.IsFalse(arguments.Verbose);
        Assert.IsFalse(arguments.ExitZero);
        Assert.IsFalse(arguments.Validate);
        Assert.IsNotNull(arguments.CustomTests);
        Assert.HasCount(1, arguments.CustomTests);
        Assert.AreEqual("custom_test", arguments.CustomTests[0]);
    }

    /// <summary>
    /// Test parsing arguments with multiple custom tests
    /// </summary>
    [TestMethod]
    public void Context_Create_WithCustomTests_SetsCustomTests()
    {
        // Arrange - Prepare arguments with multiple custom test names

        // Act - Parse the arguments
        var arguments = Context.Create(["--", "custom_test1", "custom_test2"]);

        // Assert - Verify all custom tests are set and other properties are defaults
        Assert.IsNotNull(arguments);
        Assert.IsNull(arguments.ConfigFile);
        Assert.IsNull(arguments.ResultsFile);
        Assert.IsNull(arguments.Simulator);
        Assert.IsFalse(arguments.Verbose);
        Assert.IsFalse(arguments.ExitZero);
        Assert.IsFalse(arguments.Validate);
        Assert.IsNotNull(arguments.CustomTests);
        Assert.HasCount(2, arguments.CustomTests);
        Assert.AreEqual("custom_test1", arguments.CustomTests[0]);
        Assert.AreEqual("custom_test2", arguments.CustomTests[1]);
    }
}
