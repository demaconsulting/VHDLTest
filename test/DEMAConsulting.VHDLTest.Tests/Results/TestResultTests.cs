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

using System.Collections.ObjectModel;
using DEMAConsulting.VHDLTest.Cli;
using DEMAConsulting.VHDLTest.Run;
using VHDLTestResult = DEMAConsulting.VHDLTest.Results.TestResult;

namespace DEMAConsulting.VHDLTest.Tests.Results;

/// <summary>
/// Tests for <see cref="DEMAConsulting.VHDLTest.Results.TestResult"/> class.
/// </summary>
public class TestResultTests
{
    /// <summary>
    /// Verifies that constructing a TestResult with an info-level RunResults produces Passed == true and Failed == false.
    /// </summary>
    [Fact]
    public void TestResult_Constructor_WithInfoResult_CreatesPassedTest()
    {
        // Arrange: define run results with info-level severity
        var runResults = new RunResults(
            RunLineType.Info,
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            5.0,
            0,
            "Test\nNo Issues",
            new ReadOnlyCollection<RunLine>([
                new RunLine(RunLineType.Text, "Test"),
                new RunLine(RunLineType.Text, "No Issues")
            ]));

        // Act: construct the TestResult record
        var result = new VHDLTestResult("test", "test", runResults);

        // Assert: properties reflect the info-level run result
        Assert.Equal("test", result.ClassName);
        Assert.Equal("test", result.TestName);
        Assert.Equal(RunLineType.Info, result.RunResults.Summary);
        Assert.True(result.Passed);
        Assert.False(result.Failed);
    }

    /// <summary>
    /// Verifies that constructing a TestResult with an error-level RunResults produces Passed == false and Failed == true.
    /// </summary>
    [Fact]
    public void TestResult_Constructor_WithErrorResult_CreatesFailedTest()
    {
        // Arrange: define run results with error-level severity
        var runResults = new RunResults(
            RunLineType.Error,
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            5.0,
            0,
            "Test\nError: Some Error",
            new ReadOnlyCollection<RunLine>([
                new RunLine(RunLineType.Text, "Test"),
                new RunLine(RunLineType.Error, "Error: Some Error")
            ]));

        // Act: construct the TestResult record
        var result = new VHDLTestResult("test", "test", runResults);

        // Assert: properties reflect the error-level run result
        Assert.Equal("test", result.ClassName);
        Assert.Equal("test", result.TestName);
        Assert.Equal(RunLineType.Error, result.RunResults.Summary);
        Assert.False(result.Passed);
        Assert.True(result.Failed);
    }

    /// <summary>
    ///     Validates that PrintSummary writes a "Passed" line for a passing test result.
    /// </summary>
    [Fact]
    public void TestResult_PrintSummary_PassedResult_WritesPassLine()
    {
        // Arrange: create a passing TestResult and a log path for capturing output
        var logPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        var result = new VHDLTestResult(
            "TestClass", "MyPassingTest",
            new RunResults(
                RunLineType.Info,
                new DateTime(2024, 8, 10, 0, 0, 0, DateTimeKind.Utc),
                3.0,
                0,
                "Passed",
                new ReadOnlyCollection<RunLine>([new RunLine(RunLineType.Info, "Passed")])));

        try
        {
            string output;
            using (var context = Context.Create(["--log", logPath, "--silent"]))
            {
                // Act: call PrintSummary on a passing test result
                result.PrintSummary(context);
            }

            // Assert: log contains "Passed" and the test name
            output = File.ReadAllText(logPath);
            Assert.Contains("Passed", output);
            Assert.Contains("MyPassingTest", output);
            Assert.Contains("3.0", output);  // duration formatted to one decimal place
        }
        finally
        {
            File.Delete(logPath);
        }
    }

    /// <summary>
    ///     Validates that PrintSummary writes a "Failed" line for a failing test result.
    /// </summary>
    [Fact]
    public void TestResult_PrintSummary_FailedResult_WritesFailLine()
    {
        // Arrange: create a failing TestResult and a log path for capturing output
        var logPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        var result = new VHDLTestResult(
            "TestClass", "MyFailingTest",
            new RunResults(
                RunLineType.Error,
                new DateTime(2024, 8, 10, 0, 0, 0, DateTimeKind.Utc),
                2.0,
                1,
                "Error: assertion failed",
                new ReadOnlyCollection<RunLine>([new RunLine(RunLineType.Error, "Error: assertion failed")])));

        try
        {
            string output;
            using (var context = Context.Create(["--log", logPath, "--silent"]))
            {
                // Act: call PrintSummary on a failing test result
                result.PrintSummary(context);
            }

            // Assert: log contains "Failed" and the test name
            output = File.ReadAllText(logPath);
            Assert.Contains("Failed", output);
            Assert.Contains("MyFailingTest", output);
            Assert.Contains("2.0", output);  // duration formatted to one decimal place
        }
        finally
        {
            File.Delete(logPath);
        }
    }

    /// <summary>
    ///     Verifies that passing null as the context to PrintSummary throws ArgumentNullException,
    ///     confirming the null guard is enforced before any output is attempted.
    /// </summary>
    [Fact]
    public void TestResult_PrintSummary_NullContext_ThrowsArgumentNullException()
    {
        // Arrange: create a minimal passing TestResult
        var result = new VHDLTestResult(
            "TestClass", "TestName",
            new RunResults(
                RunLineType.Info,
                new DateTime(2024, 8, 10, 0, 0, 0, DateTimeKind.Utc),
                1.0,
                0,
                "Passed",
                new ReadOnlyCollection<RunLine>([new RunLine(RunLineType.Info, "Passed")])));

        // Act / Assert: passing null as context must throw ArgumentNullException
        Assert.Throws<ArgumentNullException>(() => result.PrintSummary(null!));
    }

    /// <summary>
    ///     Verifies that constructing a TestResult with a Warning-level RunResults produces
    ///     Passed == true, confirming the pass threshold is strictly below Error.
    /// </summary>
    [Fact]
    public void TestResult_Constructor_WithWarningResult_CreatesPassedTest()
    {
        // Arrange: define run results with warning-level severity (boundary case: Warning < Error)
        var runResults = new RunResults(
            RunLineType.Warning,
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            5.0,
            0,
            "Warning: timing constraint not met",
            new ReadOnlyCollection<RunLine>([
                new RunLine(RunLineType.Warning, "Warning: timing constraint not met")
            ]));

        // Act: construct the TestResult record
        var result = new VHDLTestResult("test", "test", runResults);

        // Assert: warning-level result is considered passed (threshold is < Error)
        Assert.True(result.Passed, "Warning-level result should be Passed");
        Assert.False(result.Failed, "Warning-level result should not be Failed");
    }

    /// <summary>
    ///     Verifies that each TestResult construction produces unique TestId and ExecutionId values,
    ///     confirming that identifiers are initialized to fresh GUIDs per instance.
    /// </summary>
    [Fact]
    public void TestResult_Constructor_CreatesUniqueTestAndExecutionIds()
    {
        // Arrange: define a shared RunResults for two independent TestResult instances
        var runResults = new RunResults(
            RunLineType.Info,
            new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            1.0,
            0,
            "Passed",
            new ReadOnlyCollection<RunLine>([new RunLine(RunLineType.Info, "Passed")]));

        // Act: construct two separate TestResult instances
        var result1 = new VHDLTestResult("test", "test1", runResults);
        var result2 = new VHDLTestResult("test", "test2", runResults);

        // Assert: each instance has distinct TestId and ExecutionId
        Assert.NotEqual(result1.TestId, result2.TestId);
        Assert.NotEqual(result1.ExecutionId, result2.ExecutionId);
    }
}
