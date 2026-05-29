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

namespace DEMAConsulting.VHDLTest.Tests.Cli;

/// <summary>
/// Tests for configuration documents
/// </summary>
public class ConfigDocumentTests
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
    [Fact]
    public void ConfigDocument_ReadFile_MissingFile_ThrowsFileNotFoundException()
    {
        // Act + Assert: reading a non-existent file should throw FileNotFoundException
        Assert.Throws<FileNotFoundException>(() => ConfigDocument.ReadFile("invalid-file"));
    }

    /// <summary>
    /// Test reading a valid configuration file
    /// </summary>
    [Fact]
    public void ConfigDocument_ReadFile_ValidFile_ReadsSuccessfully()
    {
        try
        {
            // Arrange: write the config file
            File.WriteAllText(ConfigFile, ConfigContent);

            // Act: read the configuration
            var config = ConfigDocument.ReadFile(ConfigFile);

            // Assert: check the content
            Assert.NotNull(config);
            Assert.Equal(2, config.Files.Length);
            Assert.Equal("file1.vhd", config.Files[0]);
            Assert.Equal("file2.vhd", config.Files[1]);
            Assert.Equal(2, config.Tests.Length);
            Assert.Equal("test1", config.Tests[0]);
            Assert.Equal("test2", config.Tests[1]);
        }
        finally
        {
            // Delete the config file
            File.Delete(ConfigFile);
        }
    }

    /// <summary>
    /// Test reading a configuration file with null content (should throw InvalidOperationException)
    /// </summary>
    [Fact]
    public void ConfigDocument_ReadFile_InvalidContent_ThrowsInvalidOperationException()
    {
        // Arrange: write a file that deserializes to null
        var invalidFile = Path.GetTempFileName();
        try
        {
            File.WriteAllText(invalidFile, "null\n");

            // Act + Assert: reading the file should throw InvalidOperationException
            Assert.Throws<InvalidOperationException>(() => ConfigDocument.ReadFile(invalidFile));
        }
        finally
        {
            File.Delete(invalidFile);
        }
    }
}
