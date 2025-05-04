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
/// Tests for configuration documents
/// </summary>
[TestClass]
public class TestConfigDocument
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
    /// Test reading a missing configuration file
    /// </summary>
    [TestMethod]
    public void Test_ConfigDocument_Missing()
    {
        Assert.ThrowsExactly<FileNotFoundException>(() => ConfigDocument.ReadFile("invalid-file"));
    }

    /// <summary>
    /// Test reading a valid configuration file
    /// </summary>
    [TestMethod]
    public void Test_ConfigDocument_Valid()
    {
        try
        {
            // Write the config file
            File.WriteAllText(ConfigFile, ConfigContent);

            // Read the configuration
            var config = ConfigDocument.ReadFile(ConfigFile);

            // Check the content
            Assert.IsNotNull(config);
            Assert.AreEqual(2, config.Files.Length);
            Assert.AreEqual("file1.vhd", config.Files[0]);
            Assert.AreEqual("file2.vhd", config.Files[1]);
            Assert.AreEqual(2, config.Tests.Length);
            Assert.AreEqual("test1", config.Tests[0]);
            Assert.AreEqual("test2", config.Tests[1]);
        }
        finally
        {
            // Delete the config file
            File.Delete(ConfigFile);
        }
    }
}