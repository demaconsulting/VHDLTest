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
using DEMAConsulting.VHDLTest.Results;
using DEMAConsulting.VHDLTest.Run;
using DEMAConsulting.VHDLTest.Simulators;
using VHDLTestResult = DEMAConsulting.VHDLTest.Results.TestResult;

namespace DEMAConsulting.VHDLTest.Tests.Results;

/// <summary>
/// Tests for <see cref="TestResults"/> class.
/// </summary>
public class TestResultsTests
{
    /// <summary>
    /// Test that Execute compiles sources and collects test outcomes using MockSimulator.
    /// </summary>
    [Fact]
    public void TestResults_Execute_WithMockSimulator_CollectsTestOutcomes()
    {
        // Arrange: set up options with a known test bench name via MockSimulator config
        var config = new ConfigDocument { Files = ["test.vhd"], Tests = ["my_passing_test"] };
        var options = new Options(Path.GetTempPath(), config);
        using var context = Context.Create(["--silent"]);

        // Act: execute via the explicit overload with MockSimulator
        var results = TestResults.Execute(context, "UnitTestRun", "TestCodeBase", options, MockSimulator.Instance);

        // Assert: build results captured and one test outcome collected
        Assert.NotNull(results.BuildResults);
        Assert.Single(results.Tests);
        Assert.Equal("my_passing_test", results.Tests[0].TestName);
    }

    /// <summary>
    /// Test that PrintSummary writes a formatted summary including pass and fail indicators.
    /// </summary>
    [Fact]
    public void TestResults_PrintSummary_WithMixedResults_WritesSummaryToOutput()
    {
        // Arrange: build a mixed result set and a log path for capturing output
        var logPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        var passRunResults = new RunResults(
            RunLineType.Info,
            new DateTime(2024, 8, 10, 0, 0, 0, DateTimeKind.Utc),
            1.0,
            0,
            "Passed",
            new ReadOnlyCollection<RunLine>([new RunLine(RunLineType.Info, "Passed")]));

        var failRunResults = new RunResults(
            RunLineType.Error,
            new DateTime(2024, 8, 10, 0, 0, 1, DateTimeKind.Utc),
            1.0,
            1,
            "Error: assertion failed",
            new ReadOnlyCollection<RunLine>([new RunLine(RunLineType.Error, "Error: assertion failed")]));

        var testResults = new TestResults("TestRun", "TestCodeBase");
        testResults.Tests.Add(new VHDLTestResult("Suite", "PassingTest", passRunResults));
        testResults.Tests.Add(new VHDLTestResult("Suite", "FailingTest", failRunResults));

        try
        {
            string output;
            using (var context = Context.Create(["--log", logPath, "--silent"]))
            {
                // Act: call PrintSummary on the mixed result set
                testResults.PrintSummary(context);
            }

            // Assert: output contains formatted pass and fail count lines
            output = File.ReadAllText(logPath);
            Assert.Contains("Passed 1 of 2 tests", output);
            Assert.Contains("Failed 1 of 2 tests", output);
        }
        finally
        {
            File.Delete(logPath);
        }
    }

    /// <summary>
    /// Test saving test results to a TRX file.
    /// </summary>
    [Fact]
    public void TestResults_SaveResults_WithTrxExtension_CreatesTrxFile()
    {
        // Arrange: create TestResults with a single passing test
        var trxFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".trx");
        var results = new TestResults("TestRun", "TestCodeBase");
        results.Tests.Add(
            new VHDLTestResult(
                "TestClass", "TestName",
                new RunResults(
                    RunLineType.Info,
                    new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
                    5.0,
                    0,
                    "Test\nNo Issues",
                    new ReadOnlyCollection<RunLine>([
                        new RunLine(RunLineType.Text, "Test"),
                        new RunLine(RunLineType.Text, "No Issues")
                    ])
                )
            )
        );

        try
        {
            // Act: save to TRX file
            results.SaveResults(trxFile);

            // Assert: file exists and is valid TRX XML
            Assert.True(File.Exists(trxFile));
            var content = File.ReadAllText(trxFile);
            Assert.Contains("<?xml", content);
            Assert.Contains("TestRun", content);
        }
        finally
        {
            if (File.Exists(trxFile))
            {
                File.Delete(trxFile);
            }
        }
    }

    /// <summary>
    /// Test saving test results to a JUnit XML file.
    /// </summary>
    [Fact]
    public void TestResults_SaveResults_WithXmlExtension_CreatesJUnitFile()
    {
        // Arrange: create TestResults with a single passing test
        var xmlFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".xml");
        var results = new TestResults("TestRun", "TestCodeBase");
        results.Tests.Add(
            new VHDLTestResult(
                "TestClass", "TestName",
                new RunResults(
                    RunLineType.Info,
                    new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
                    5.0,
                    0,
                    "Test\nNo Issues",
                    new ReadOnlyCollection<RunLine>([
                        new RunLine(RunLineType.Text, "Test"),
                        new RunLine(RunLineType.Text, "No Issues")
                    ])
                )
            )
        );

        try
        {
            // Act: save to XML file
            results.SaveResults(xmlFile);

            // Assert: file exists and is valid JUnit XML
            Assert.True(File.Exists(xmlFile));
            var content = File.ReadAllText(xmlFile);
            Assert.Contains("<?xml", content);
            Assert.Contains("testsuites", content);
        }
        finally
        {
            if (File.Exists(xmlFile))
            {
                File.Delete(xmlFile);
            }
        }
    }

    /// <summary>
    /// Test saving failed test results to a JUnit XML file.
    /// </summary>
    [Fact]
    public void TestResults_SaveResults_WithFailedTest_CreatesJUnitFileWithFailure()
    {
        // Arrange: create TestResults with a single failing test
        var xmlFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".xml");
        var results = new TestResults("TestRun", "TestCodeBase");
        results.Tests.Add(
            new VHDLTestResult(
                "TestClass", "FailedTest",
                new RunResults(
                    RunLineType.Error,
                    new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
                    5.0,
                    1,
                    "Test\nError occurred",
                    new ReadOnlyCollection<RunLine>([
                        new RunLine(RunLineType.Text, "Test"),
                        new RunLine(RunLineType.Error, "Error occurred")
                    ])
                )
            )
        );

        try
        {
            // Act: save to XML file
            results.SaveResults(xmlFile);

            // Assert: file contains failure information
            Assert.True(File.Exists(xmlFile));
            var content = File.ReadAllText(xmlFile);
            Assert.Contains("failure", content);
            Assert.Contains("Error occurred", content);
        }
        finally
        {
            if (File.Exists(xmlFile))
            {
                File.Delete(xmlFile);
            }
        }
    }

    /// <summary>
    /// Test backward compatibility with SaveToTrx method.
    /// </summary>
    [Fact]
    public void TestResults_SaveToTrx_WithTestResults_CreatesTrxFile()
    {
        // Arrange: create TestResults with a single passing test
        var trxFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".trx");
        var results = new TestResults("TestRun", "TestCodeBase");
        results.Tests.Add(
            new VHDLTestResult(
                "TestClass", "TestName",
                new RunResults(
                    RunLineType.Info,
                    new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
                    5.0,
                    0,
                    "Test\nNo Issues",
                    new ReadOnlyCollection<RunLine>([
                        new RunLine(RunLineType.Text, "Test"),
                        new RunLine(RunLineType.Text, "No Issues")
                    ])
                )
            )
        );

        try
        {
            // Act: save via backward-compatible SaveToTrx
            results.SaveToTrx(trxFile);

            // Assert: file exists and is valid TRX XML
            Assert.True(File.Exists(trxFile));
            var content = File.ReadAllText(trxFile);
            Assert.Contains("<?xml", content);
            Assert.Contains("TestRun", content);
        }
        finally
        {
            if (File.Exists(trxFile))
            {
                File.Delete(trxFile);
            }
        }
    }

    /// <summary>
    /// Test that SaveResults throws ArgumentException for null/empty file name.
    /// </summary>
    [Fact]
    public void TestResults_SaveResults_WithNullFileName_ThrowsArgumentException()
    {
        // Arrange: create a TestResults instance with no tests
        var results = new TestResults("TestRun", "TestCodeBase");

        // Act + Assert: null, empty, and whitespace paths all throw ArgumentException
        Assert.Throws<ArgumentException>(() => results.SaveResults(null!));
        Assert.Throws<ArgumentException>(() => results.SaveResults(string.Empty));
        Assert.Throws<ArgumentException>(() => results.SaveResults("   "));
    }

    /// <summary>
    /// Test that SaveResults defaults to TRX for unknown extensions.
    /// </summary>
    [Fact]
    public void TestResults_SaveResults_WithUnknownExtension_CreatesTrxFile()
    {
        // Arrange: create TestResults with a single passing test
        var unknownFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".unknown");
        var results = new TestResults("TestRun", "TestCodeBase");
        results.Tests.Add(
            new VHDLTestResult(
                "TestClass", "TestName",
                new RunResults(
                    RunLineType.Info,
                    new DateTime(2024, 08, 10, 0, 0, 0, DateTimeKind.Utc),
                    5.0,
                    0,
                    "Test\nNo Issues",
                    new ReadOnlyCollection<RunLine>([
                        new RunLine(RunLineType.Text, "Test"),
                        new RunLine(RunLineType.Text, "No Issues")
                    ])
                )
            )
        );

        try
        {
            // Act: save with an unrecognized extension
            results.SaveResults(unknownFile);

            // Assert: file exists and defaults to TRX format
            Assert.True(File.Exists(unknownFile));
            var content = File.ReadAllText(unknownFile);
            Assert.Contains("<?xml", content);
            Assert.Contains("TestRun", content);
        }
        finally
        {
            if (File.Exists(unknownFile))
            {
                File.Delete(unknownFile);
            }
        }
    }

    /// <summary>
    ///     Verifies that TestResults.Execute throws InvalidOperationException with the message
    ///     "Build Failed" when the build step produces an Error-level result via MockSimulator.
    /// </summary>
    [Fact]
    public void TestResults_Execute_WithBuildFailure_ThrowsInvalidOperationException()
    {
        // Arrange: configure an options object with a file that triggers a build error in MockSimulator
        var config = new ConfigDocument { Files = ["_error_file.vhd"], Tests = [] };
        var options = new Options(Path.GetTempPath(), config);
        using var context = Context.Create(["--silent"]);

        // Act / Assert: executing with a build-failing config must throw InvalidOperationException
        var ex = Assert.Throws<InvalidOperationException>(
            () => TestResults.Execute(context, "UnitTestRun", "TestCodeBase", options, MockSimulator.Instance));
        Assert.Equal("Build Failed", ex.Message);
    }

    /// <summary>
    ///     Verifies that PrintSummary with an empty Tests collection writes only the separator
    ///     lines and no pass or fail count lines, confirming the zero-count suppression guards.
    /// </summary>
    [Fact]
    public void TestResults_PrintSummary_EmptyTests_WritesOnlySeparators()
    {
        // Arrange: create a TestResults instance with no tests and a log path for capturing output
        var logPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        var results = new TestResults("TestRun", "TestCodeBase");

        try
        {
            string output;
            using (var context = Context.Create(["--log", logPath, "--silent"]))
            {
                // Act: call PrintSummary on an empty result set
                results.PrintSummary(context);
            }

            // Assert: separator lines are written, but no "Passed" or "Failed" count lines appear
            output = File.ReadAllText(logPath);
            Assert.Multiple(() =>
            {
                Assert.Contains("====", output);
                Assert.DoesNotContain("Passed", output);
                Assert.DoesNotContain("Failed", output);
            });
        }
        finally
        {
            File.Delete(logPath);
        }
    }

    /// <summary>
    ///     Verifies that PrintSummary with all-passing tests writes the "Passed N of M" line
    ///     but suppresses the "Failed" line, confirming the zero-count guard for failed count.
    /// </summary>
    [Fact]
    public void TestResults_PrintSummary_AllPassTests_WritesPassedCountOnly()
    {
        // Arrange: create a TestResults instance with only passing tests
        var logPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        var passRunResults = new RunResults(
            RunLineType.Info,
            new DateTime(2024, 8, 10, 0, 0, 0, DateTimeKind.Utc),
            1.0,
            0,
            "Passed",
            new ReadOnlyCollection<RunLine>([new RunLine(RunLineType.Info, "Passed")]));

        var results = new TestResults("TestRun", "TestCodeBase");
        results.Tests.Add(new VHDLTestResult("Suite", "PassingTest", passRunResults));

        try
        {
            string output;
            using (var context = Context.Create(["--log", logPath, "--silent"]))
            {
                // Act: call PrintSummary on an all-pass result set
                results.PrintSummary(context);
            }

            // Assert: "Passed" count line appears but "Failed" count line is suppressed
            output = File.ReadAllText(logPath);
            Assert.Multiple(() =>
            {
                Assert.Contains("Passed 1 of 1 tests", output);
                Assert.DoesNotContain("Failed", output);
            });
        }
        finally
        {
            File.Delete(logPath);
        }
    }

    /// <summary>
    ///     Verifies that PrintSummary with all-failing tests writes the "Failed N of M" line
    ///     but suppresses the "Passed" line, confirming the zero-count guard for passed count.
    /// </summary>
    [Fact]
    public void TestResults_PrintSummary_AllFailTests_WritesFailedCountOnly()
    {
        // Arrange: create a TestResults instance with only failing tests
        var logPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        var failRunResults = new RunResults(
            RunLineType.Error,
            new DateTime(2024, 8, 10, 0, 0, 1, DateTimeKind.Utc),
            1.0,
            1,
            "Error: assertion failed",
            new ReadOnlyCollection<RunLine>([new RunLine(RunLineType.Error, "Error: assertion failed")]));

        var results = new TestResults("TestRun", "TestCodeBase");
        results.Tests.Add(new VHDLTestResult("Suite", "FailingTest", failRunResults));

        try
        {
            string output;
            using (var context = Context.Create(["--log", logPath, "--silent"]))
            {
                // Act: call PrintSummary on an all-fail result set
                results.PrintSummary(context);
            }

            // Assert: "Failed" count line appears but "Passed" count line is suppressed
            output = File.ReadAllText(logPath);
            Assert.Multiple(() =>
            {
                Assert.Contains("Failed 1 of 1 tests", output);
                Assert.DoesNotContain("Passed", output);
            });
        }
        finally
        {
            File.Delete(logPath);
        }
    }

    /// <summary>
    ///     Verifies that passing null as the context to PrintSummary throws ArgumentNullException,
    ///     confirming the null guard is enforced before any output is written.
    /// </summary>
    [Fact]
    public void TestResults_PrintSummary_NullContext_ThrowsArgumentNullException()
    {
        // Arrange: create an empty TestResults instance
        var results = new TestResults("TestRun", "TestCodeBase");

        // Act / Assert: passing null must throw ArgumentNullException
        Assert.Throws<ArgumentNullException>(() => results.PrintSummary(null!));
    }
}
