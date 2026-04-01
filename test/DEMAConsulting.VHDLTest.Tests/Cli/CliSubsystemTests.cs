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
/// Subsystem integration tests for the Cli subsystem.
/// These tests verify that <see cref="Context"/>, <see cref="ConfigDocument"/>, and
/// <see cref="Options"/> work together as a complete command-line configuration pipeline.
/// </summary>
[TestClass]
public class CliSubsystemTests
{
    /// <summary>
    /// Configuration file name
    /// </summary>
    private const string ConfigFile = "cli-subsystem-test.yaml";

    /// <summary>
    /// Configuration file contents
    /// </summary>
    private const string ConfigContent =
        """
        files:
        - src/design.vhd
        - src/testbench.vhd

        tests:
        - test_entity
        """;

    /// <summary>
    /// Test that command-line arguments, config file loading, and options parsing
    /// work together to produce a correctly configured Options object.
    /// </summary>
    [TestMethod]
    public void CliSubsystem_ParseArgsAndLoadConfig_WithValidConfig_ProducesCorrectOptions()
    {
        try
        {
            // Arrange - write a valid config file
            File.WriteAllText(ConfigFile, ConfigContent);

            // Act - run the full Cli pipeline: args -> Context -> Options
            var context = Context.Create(["-c", ConfigFile, "--verbose"]);
            var options = Options.Parse(context);

            // Assert - verify the subsystem produced the correct options
            Assert.IsNotNull(context);
            Assert.IsTrue(context.Verbose);
            Assert.AreEqual(ConfigFile, context.ConfigFile);
            Assert.IsNotNull(options);
            Assert.HasCount(2, options.Config.Files);
            Assert.AreEqual("src/design.vhd", options.Config.Files[0]);
            Assert.AreEqual("src/testbench.vhd", options.Config.Files[1]);
            Assert.HasCount(1, options.Config.Tests);
            Assert.AreEqual("test_entity", options.Config.Tests[0]);
        }
        finally
        {
            File.Delete(ConfigFile);
        }
    }

    /// <summary>
    /// Test that the Cli subsystem correctly propagates a missing config file error
    /// through the Context and Options pipeline.
    /// </summary>
    [TestMethod]
    public void CliSubsystem_ParseArgsAndLoadConfig_WithMissingConfig_ThrowsFileNotFoundException()
    {
        // Arrange - create context specifying a non-existent config file
        var context = Context.Create(["-c", "missing-config.yaml"]);

        // Act & Assert - verify the subsystem surfaces the missing file error from Options.Parse
        Assert.ThrowsExactly<FileNotFoundException>(() => Options.Parse(context));
    }
}
