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

        // Verify no error
        Assert.AreEqual(0, exitCode);

        // Verify usage reported
        Assert.IsTrue(output.Contains("Usage: VHDLTest"));
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
        Assert.IsTrue(output.Contains("Usage: VHDLTest"));
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
        Assert.IsTrue(output.Contains("Usage: VHDLTest"));
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
        Assert.IsTrue(output.Contains("Usage: VHDLTest"));
    }
}