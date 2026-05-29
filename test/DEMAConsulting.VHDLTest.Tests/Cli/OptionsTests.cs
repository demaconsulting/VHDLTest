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
/// Tests for parsing options
/// </summary>
public class OptionsTests
{
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
    [Fact]
    public void Options_Parse_NoConfigProvided_ThrowsInvalidOperationException()
    {
        // Arrange: create a context with no configuration file
        var arguments = Context.Create([]);

        // Act + Assert: parsing without a config file should throw
        Assert.Throws<InvalidOperationException>(() => Options.Parse(arguments));
    }

    /// <summary>
    /// Test parsing options with missing configuration file
    /// </summary>
    [Fact]
    public void Options_Parse_MissingConfigFile_ThrowsFileNotFoundException()
    {
        // Arrange: create a context referencing a non-existent configuration file
        var arguments = Context.Create(["-c", "missing-config.yaml"]);

        // Act + Assert: parsing with a missing config file should throw
        Assert.Throws<FileNotFoundException>(() => Options.Parse(arguments));
    }

    /// <summary>
    /// Test parsing options with configuration file
    /// </summary>
    [Fact]
    public void Options_Parse_ValidConfigFile_ParsesSuccessfully()
    {
        // Arrange: use a unique per-test temp file to avoid cross-test file conflicts
        var configFile = Path.Combine(Path.GetTempPath(), $"options_test_{Guid.NewGuid():N}.yaml");
        try
        {
            // Arrange: write the config file
            File.WriteAllText(configFile, ConfigContent);

            // Act: parse the options
            var arguments = Context.Create(["-c", configFile]);
            var options = Options.Parse(arguments);

            // Assert: check the options
            Assert.NotNull(options);
            Assert.Equal(2, options.Config.Files.Length);
            Assert.Equal("file1.vhd", options.Config.Files[0]);
            Assert.Equal("file2.vhd", options.Config.Files[1]);
            Assert.Equal(2, options.Config.Tests.Length);
            Assert.Equal("test1", options.Config.Tests[0]);
            Assert.Equal("test2", options.Config.Tests[1]);
            Assert.Equal(Path.GetDirectoryName(Path.GetFullPath(configFile)), options.WorkingDirectory);
        }
        finally
        {
            // Delete the config file
            File.Delete(configFile);
        }
    }

    /// <summary>
    /// Test parsing options with verbose flag
    /// </summary>
    [Fact]
    public void Options_Parse_WithVerboseFlag_ParsesSuccessfully()
    {
        // Arrange: use a unique per-test temp file to avoid cross-test file conflicts
        var configFile = Path.Combine(Path.GetTempPath(), $"options_test_{Guid.NewGuid():N}.yaml");
        try
        {
            // Arrange: write the config file
            File.WriteAllText(configFile, ConfigContent);

            // Act: parse the options
            var arguments = Context.Create(["-c", configFile, "--verbose"]);
            var options = Options.Parse(arguments);

            // Assert: verify options are non-null, working directory is absolute, and config is populated
            Assert.NotNull(options);
            Assert.True(Path.IsPathRooted(options.WorkingDirectory), "WorkingDirectory should be an absolute path");
            Assert.NotNull(options.Config);
            Assert.Equal(2, options.Config.Files.Length);
        }
        finally
        {
            // Delete the config file
            File.Delete(configFile);
        }
    }

    /// <summary>
    /// Test parsing options with custom test
    /// </summary>
    [Fact]
    public void Options_Parse_WithCustomTest_ParsesSuccessfully()
    {
        // Arrange: use a unique per-test temp file to avoid cross-test file conflicts
        var configFile = Path.Combine(Path.GetTempPath(), $"options_test_{Guid.NewGuid():N}.yaml");
        try
        {
            // Arrange: write the config file
            File.WriteAllText(configFile, ConfigContent);

            // Act: parse the options
            var arguments = Context.Create(["-c", configFile, "custom_test"]);
            var options = Options.Parse(arguments);

            // Assert: verify options are non-null, working directory is absolute, and config is populated
            Assert.NotNull(options);
            Assert.True(Path.IsPathRooted(options.WorkingDirectory), "WorkingDirectory should be an absolute path");
            Assert.NotNull(options.Config);
            Assert.Equal(2, options.Config.Tests.Length);
        }
        finally
        {
            // Delete the config file
            File.Delete(configFile);
        }
    }

    /// <summary>
    /// Test that a root config path throws InvalidOperationException
    /// </summary>
    [Fact]
    public void Options_ResolveWorkingDirectory_RootPath_ThrowsInvalidOperationException()
    {
        // Arrange: determine the root path for the current platform (e.g. "C:\" on Windows, "/" on Linux)
        var rootPath = Path.GetPathRoot(Path.GetFullPath("."))!;

        // Act + Assert: a root path has no parent directory and should throw
        Assert.Throws<InvalidOperationException>(() => Options.ResolveWorkingDirectory(rootPath));
    }
}
