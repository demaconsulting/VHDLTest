using DEMAConsulting.VHDLTest.Run;

namespace DEMAConsulting.VHDLTest.Tests;

[TestClass]
public class TestTestResult
{
    [TestMethod]
    public void Test_TestResult_Info()
    {
        // Construct the result
        var result = new Results.TestResult(
            "test",
            "test",
            new RunResults(
                RunLineType.Info,
                new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
                5.0,
                0,
                "Test\nNo Issues",
                new[]
                {
                    new RunLine(RunLineType.Text, "Test"),
                    new RunLine(RunLineType.Text, "No Issues")
                }
            ));

        Assert.AreEqual("test", result.ClassName);
        Assert.AreEqual("test", result.TestName);
        Assert.AreEqual(RunLineType.Info, result.RunResults.Summary);
        Assert.IsTrue(result.Passed);
        Assert.IsFalse(result.Failed);
    }

    [TestMethod]
    public void Test_TestResult_Error()
    {
        // Construct the result
        var result = new Results.TestResult(
            "test",
            "test",
            new RunResults(
                RunLineType.Error,
                new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
                5.0,
                0,
                "Test\nError: Some Error",
                new[]
                {
                    new RunLine(RunLineType.Text, "Test"),
                    new RunLine(RunLineType.Error, "Error: Some Error")
                }
            ));

        Assert.AreEqual("test", result.ClassName);
        Assert.AreEqual("test", result.TestName);
        Assert.AreEqual(RunLineType.Error, result.RunResults.Summary);
        Assert.IsFalse(result.Passed);
        Assert.IsTrue(result.Failed);
    }
}