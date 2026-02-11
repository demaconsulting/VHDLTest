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
using DEMAConsulting.VHDLTest.Results;
using DEMAConsulting.VHDLTest.Run;

namespace DEMAConsulting.VHDLTest.Tests;

/// <summary>
/// Tests for <see cref="Results.TestResults"/> class.
/// </summary>
[TestClass]
public class TestResultsTests
{
    /// <summary>
    /// Test saving test results to a TRX file.
    /// </summary>
    [TestMethod]
    public void TestResults_SaveResults_WithTrxExtension_CreatesTrxFile()
    {
        var results = new TestResults("TestRun", "TestCodeBase");
        results.Tests.Add(
            new Results.TestResult(
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
            results.SaveResults("TestResults.trx");
            Assert.IsTrue(File.Exists("TestResults.trx"));

            // Verify it's valid XML
            var content = File.ReadAllText("TestResults.trx");
            Assert.Contains("<?xml", content);
            Assert.Contains("TestRun", content);
        }
        finally
        {
            if (File.Exists("TestResults.trx"))
                File.Delete("TestResults.trx");
        }
    }

    /// <summary>
    /// Test saving test results to a JUnit XML file.
    /// </summary>
    [TestMethod]
    public void TestResults_SaveResults_WithXmlExtension_CreatesJUnitFile()
    {
        var results = new TestResults("TestRun", "TestCodeBase");
        results.Tests.Add(
            new Results.TestResult(
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
            results.SaveResults("TestResults.xml");
            Assert.IsTrue(File.Exists("TestResults.xml"));

            // Verify it's valid JUnit XML
            var content = File.ReadAllText("TestResults.xml");
            Assert.Contains("<?xml", content);
            Assert.Contains("testsuites", content);
        }
        finally
        {
            if (File.Exists("TestResults.xml"))
                File.Delete("TestResults.xml");
        }
    }

    /// <summary>
    /// Test saving failed test results to a JUnit XML file.
    /// </summary>
    [TestMethod]
    public void TestResults_SaveResults_WithFailedTest_CreatesJUnitFileWithFailure()
    {
        var results = new TestResults("TestRun", "TestCodeBase");
        results.Tests.Add(
            new Results.TestResult(
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
            results.SaveResults("TestResults.xml");
            Assert.IsTrue(File.Exists("TestResults.xml"));

            // Verify it contains failure information
            var content = File.ReadAllText("TestResults.xml");
            Assert.Contains("failure", content);
            Assert.Contains("Error occurred", content);
        }
        finally
        {
            if (File.Exists("TestResults.xml"))
                File.Delete("TestResults.xml");
        }
    }

    /// <summary>
    /// Test backward compatibility with SaveToTrx method.
    /// </summary>
    [TestMethod]
    public void TestResults_SaveToTrx_WithTestResults_CreatesTrxFile()
    {
        var results = new TestResults("TestRun", "TestCodeBase");
        results.Tests.Add(
            new Results.TestResult(
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
            results.SaveToTrx("TestResults.trx");
            Assert.IsTrue(File.Exists("TestResults.trx"));
        }
        finally
        {
            if (File.Exists("TestResults.trx"))
                File.Delete("TestResults.trx");
        }
    }

    /// <summary>
    /// Test that SaveResults throws ArgumentException for null/empty file name.
    /// </summary>
    [TestMethod]
    public void TestResults_SaveResults_WithNullFileName_ThrowsArgumentException()
    {
        var results = new TestResults("TestRun", "TestCodeBase");

        Assert.ThrowsExactly<ArgumentException>(() => results.SaveResults(null!));
        Assert.ThrowsExactly<ArgumentException>(() => results.SaveResults(string.Empty));
        Assert.ThrowsExactly<ArgumentException>(() => results.SaveResults("   "));
    }

    /// <summary>
    /// Test that SaveResults defaults to TRX for unknown extensions.
    /// </summary>
    [TestMethod]
    public void TestResults_SaveResults_WithUnknownExtension_CreatesTrxFile()
    {
        var results = new TestResults("TestRun", "TestCodeBase");
        results.Tests.Add(
            new Results.TestResult(
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
            results.SaveResults("TestResults.unknown");
            Assert.IsTrue(File.Exists("TestResults.unknown"));

            // Verify it's TRX format
            var content = File.ReadAllText("TestResults.unknown");
            Assert.Contains("<?xml", content);
            Assert.Contains("TestRun", content);
        }
        finally
        {
            if (File.Exists("TestResults.unknown"))
                File.Delete("TestResults.unknown");
        }
    }
}
