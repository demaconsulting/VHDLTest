using System.Text.RegularExpressions;

namespace DEMAConsulting.VHDLTest.Tests;

[TestClass]
public class TestVersion
{
    [TestMethod]
    public void VersionShort()
    {
        // Query version
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            "DEMAConsulting.VHDLTest.dll",
            "-v");

        // Verify success
        Assert.AreEqual(0, exitCode);

        // Verify version reported
        Assert.IsTrue(Regex.IsMatch(output, @"\d+\.\d+\.\d+"));
    }

    [TestMethod]
    public void VersionLong()
    {
        // Query version
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            "DEMAConsulting.VHDLTest.dll",
            "--version");

        // Verify success
        Assert.AreEqual(0, exitCode);

        // Verify version reported
        Assert.IsTrue(Regex.IsMatch(output, @"\d+\.\d+\.\d+"));
    }
}