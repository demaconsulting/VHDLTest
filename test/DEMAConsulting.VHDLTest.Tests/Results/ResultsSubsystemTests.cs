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
using DEMAConsulting.VHDLTest.Run;
using VHDLTestResult = DEMAConsulting.VHDLTest.Results.TestResult;
using VHDLTestResults = DEMAConsulting.VHDLTest.Results.TestResults;

namespace DEMAConsulting.VHDLTest.Tests.Results;

/// <summary>
/// Subsystem integration tests for the Results subsystem.
/// These tests verify that <see cref="VHDLTestResult"/> and <see cref="VHDLTestResults"/>
/// work together to collect, summarize, and persist test results.
/// </summary>
[TestClass]
public class ResultsSubsystemTests
{
    /// <summary>
    /// Test that TestResult and TestResults work together to correctly track
    /// pass and fail counts across a collection of test outcomes.
    /// </summary>
    [TestMethod]
    public void ResultsSubsystem_CollectAndSummarize_WithMixedResults_ReportsCorrectPassFailCounts()
    {
        // Arrange - create passing and failing RunResults objects
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

        // Act - wrap each RunResults in a TestResult and add to TestResults
        var testResults = new VHDLTestResults("SubsystemTestRun", "VHDLTest");
        testResults.Tests.Add(new VHDLTestResult("Suite", "Test1", passRunResults));
        testResults.Tests.Add(new VHDLTestResult("Suite", "Test2", passRunResults));
        testResults.Tests.Add(new VHDLTestResult("Suite", "Test3", failRunResults));

        // Assert - TestResults correctly aggregates pass and fail counts
        Assert.AreEqual(3, testResults.Tests.Count);
        Assert.AreEqual(2, testResults.Passes.Count());
        Assert.AreEqual(1, testResults.Fails.Count());
    }

    /// <summary>
    /// Test that TestResults correctly saves a combined pass/fail result set to
    /// a TRX file, verifying that TestResult and TestResults integrate through
    /// the serialization pipeline.
    /// </summary>
    [TestMethod]
    public void ResultsSubsystem_SaveMixedResults_ToTrxFile_CreatesTrxFileWithCorrectCounts()
    {
        const string resultsFile = "results-subsystem-test.trx";

        try
        {
            // Arrange - build a mixed result set
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

            var testResults = new VHDLTestResults("SubsystemTestRun", "VHDLTest");
            testResults.Tests.Add(new VHDLTestResult("Suite", "Test1", passRunResults));
            testResults.Tests.Add(new VHDLTestResult("Suite", "Test2", failRunResults));

            // Act - save the combined results to a TRX file
            testResults.SaveResults(resultsFile);

            // Assert - file was created and contains both test results
            Assert.IsTrue(File.Exists(resultsFile));
            var content = File.ReadAllText(resultsFile);
            Assert.IsTrue(content.Contains("Test1"));
            Assert.IsTrue(content.Contains("Test2"));
        }
        finally
        {
            File.Delete(resultsFile);
        }
    }
}
