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
/// Tests for parsing options
/// </summary>
[TestClass]
public class TestOptions
{
    /// <summary>
    /// Configuration file name
    /// </summary>
    private const string ConfigFile = "options-test.yaml";

    /// <summary>
    /// Configuration file contents
    /// </summary>
    private const string ConfigContent =
        """
        files:
        - file1.vhd
        - file2.vhd

        tests:
        - test1
        - test2
        """;

    /// <summary>
    /// Test parsing options with no configuration file
    /// </summary>
    [TestMethod]
    public void Test_Options_NoConfig()
    {
        var arguments = Arguments.Parse([]);
        Assert.ThrowsException<InvalidOperationException>(() => Options.Parse(arguments));
    }

    /// <summary>
    /// Test parsing options with missing configuration file
    /// </summary>
    [TestMethod]
    public void Test_Options_MissingConfig()
    {
        var arguments = Arguments.Parse(["-c", "missing-config.yaml"]);
        Assert.ThrowsException<FileNotFoundException>(() => Options.Parse(arguments));
    }

    /// <summary>
    /// Test parsing options with configuration file
    /// </summary>
    [TestMethod]
    public void Test_Options_ConfigFile()
    {
        try
        {
            // Write the config file
            File.WriteAllText(ConfigFile, ConfigContent);

            // Parse the options
            var arguments = Arguments.Parse(["-c", ConfigFile]);
            var options = Options.Parse(arguments);

            // Check the options
            Assert.IsNotNull(options);
            Assert.AreEqual(2, options.Config.Files.Length);
            Assert.AreEqual("file1.vhd", options.Config.Files[0]);
            Assert.AreEqual("file2.vhd", options.Config.Files[1]);
            Assert.AreEqual(2, options.Config.Tests.Length);
            Assert.AreEqual("test1", options.Config.Tests[0]);
            Assert.AreEqual("test2", options.Config.Tests[1]);
            Assert.IsFalse(options.Verbose);
            Assert.IsNull(options.CustomTests);
        }
        finally
        {
            // Delete the config file
            File.Delete(ConfigFile);
        }
    }

    /// <summary>
    /// Test parsing options with verbose flag
    /// </summary>
    [TestMethod]
    public void Test_Options_Verbose()
    {
        try
        {
            // Write the config file
            File.WriteAllText(ConfigFile, ConfigContent);

            // Parse the options
            var arguments = Arguments.Parse(["-c", ConfigFile, "--verbose"]);
            var options = Options.Parse(arguments);

            // Check the options
            Assert.IsNotNull(options);
            Assert.IsTrue(options.Verbose);
            Assert.IsNull(options.CustomTests);
        }
        finally
        {
            // Delete the config file
            File.Delete(ConfigFile);
        }
    }

    /// <summary>
    /// Test parsing options with custom test
    /// </summary>
    [TestMethod]
    public void Test_Options_CustomTest()
    {
        try
        {
            // Write the config file
            File.WriteAllText(ConfigFile, ConfigContent);

            // Parse the options
            var arguments = Arguments.Parse(["-c", ConfigFile, "custom_test"]);
            var options = Options.Parse(arguments);

            // Check the options
            Assert.IsNotNull(options);
            Assert.IsFalse(options.Verbose);
            Assert.IsNotNull(options.CustomTests);
            Assert.AreEqual(1, options.CustomTests.Count);
            Assert.AreEqual("custom_test", options.CustomTests[0]);
        }
        finally
        {
            // Delete the config file
            File.Delete(ConfigFile);
        }
    }
}