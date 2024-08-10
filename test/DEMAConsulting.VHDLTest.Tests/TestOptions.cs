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

[TestClass]
public class TestOptions
{
    private const string ConfigFile = "options-test.yaml";

    private const string ConfigContent = 
        "files:\n" +
        "- file1.vhd\n" +
        "- file2.vhd\n" +
        "\n" +
        "tests:\n" +
        "- test1\n" +
        "- test2\n";

    [TestMethod]
    public void Test_Options_None()
    {
        Assert.ThrowsException<InvalidOperationException>(() => Options.Parse(Array.Empty<string>()));
    }

    [TestMethod]
    public void Test_Options_ConfigFile()
    {
        try
        {
            // Write the config file
            File.WriteAllText(ConfigFile, ConfigContent);

            // Parse the options
            var options = Options.Parse(new[] { "-c", ConfigFile });

            // Check the options
            Assert.IsNotNull(options);
            Assert.AreEqual(2, options.Config.Files.Count);
            Assert.AreEqual("file1.vhd", options.Config.Files[0]);
            Assert.AreEqual("file2.vhd", options.Config.Files[1]);
            Assert.AreEqual(2, options.Config.Tests.Count);
            Assert.AreEqual("test1", options.Config.Tests[0]);
            Assert.AreEqual("test2", options.Config.Tests[1]);
            Assert.IsNull(options.TestResultsFile);
            Assert.IsNull(options.Simulator);
            Assert.IsFalse(options.Verbose);
            Assert.IsFalse(options.ExitZero);
            Assert.IsNull(options.CustomTests);
        }
        finally
        {
            // Delete the config file
            File.Delete(ConfigFile);
        }
    }

    [TestMethod]
    public void Test_Options_ResultsFile()
    {
        try
        {
            // Write the config file
            File.WriteAllText(ConfigFile, ConfigContent);

            // Parse the options
            var options = Options.Parse(new[] { "-c", ConfigFile, "-r", "results.rtd" });

            // Check the options
            Assert.IsNotNull(options);
            Assert.IsNull(options.Simulator);
            Assert.IsFalse(options.Verbose);
            Assert.IsFalse(options.ExitZero);
            Assert.IsNull(options.CustomTests);
        }
        finally
        {
            // Delete the config file
            File.Delete(ConfigFile);
        }
    }

    [TestMethod]
    public void Test_Options_Simulator()
    {
        try
        {
            // Write the config file
            File.WriteAllText(ConfigFile, ConfigContent);

            // Parse the options
            var options = Options.Parse(new[] { "-c", ConfigFile, "-s", "GHDL" });

            // Check the options
            Assert.IsNotNull(options);
            Assert.IsNull(options.TestResultsFile);
            Assert.AreEqual("GHDL", options.Simulator);
            Assert.IsFalse(options.Verbose);
            Assert.IsFalse(options.ExitZero);
            Assert.IsNull(options.CustomTests);
        }
        finally
        {
            // Delete the config file
            File.Delete(ConfigFile);
        }
    }

    [TestMethod]
    public void Test_Options_Verbose()
    {
        try
        {
            // Write the config file
            File.WriteAllText(ConfigFile, ConfigContent);

            // Parse the options
            var options = Options.Parse(new[] { "-c", ConfigFile, "--verbose" });

            // Check the options
            Assert.IsNotNull(options);
            Assert.IsNull(options.TestResultsFile);
            Assert.IsNull(options.Simulator);
            Assert.IsTrue(options.Verbose);
            Assert.IsFalse(options.ExitZero);
            Assert.IsNull(options.CustomTests);
        }
        finally
        {
            // Delete the config file
            File.Delete(ConfigFile);
        }
    }

    [TestMethod]
    public void Test_Options_ExitZero()
    {
        try
        {
            // Write the config file
            File.WriteAllText(ConfigFile, ConfigContent);

            // Parse the options
            var options = Options.Parse(new[] { "-c", ConfigFile, "-0" });

            // Check the options
            Assert.IsNotNull(options);
            Assert.IsNull(options.TestResultsFile);
            Assert.IsNull(options.Simulator);
            Assert.IsFalse(options.Verbose);
            Assert.IsTrue(options.ExitZero);
            Assert.IsNull(options.CustomTests);
        }
        finally
        {
            // Delete the config file
            File.Delete(ConfigFile);
        }
    }

    [TestMethod]
    public void Test_Options_CustomTest()
    {
        try
        {
            // Write the config file
            File.WriteAllText(ConfigFile, ConfigContent);

            // Parse the options
            var options = Options.Parse(new[] { "-c", ConfigFile, "custom_test" });

            // Check the options
            Assert.IsNotNull(options);
            Assert.IsNull(options.TestResultsFile);
            Assert.IsNull(options.Simulator);
            Assert.IsFalse(options.Verbose);
            Assert.IsFalse(options.ExitZero);
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

    [TestMethod]
    public void Test_Options_CustomTests()
    {
        try
        {
            // Write the config file
            File.WriteAllText(ConfigFile, ConfigContent);

            // Parse the options
            var options = Options.Parse(new[] { "-c", ConfigFile, "--", "custom_test1", "custom_test2" });

            // Check the options
            Assert.IsNotNull(options);
            Assert.IsNull(options.TestResultsFile);
            Assert.IsNull(options.Simulator);
            Assert.IsFalse(options.Verbose);
            Assert.IsFalse(options.ExitZero);
            Assert.IsNotNull(options.CustomTests);
            Assert.AreEqual(2, options.CustomTests.Count);
            Assert.AreEqual("custom_test1", options.CustomTests[0]);
            Assert.AreEqual("custom_test2", options.CustomTests[1]);
        }
        finally
        {
            // Delete the config file
            File.Delete(ConfigFile);
        }
    }
}