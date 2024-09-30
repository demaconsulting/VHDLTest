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
public class TestTestResults
{
    /// <summary>
    /// Test saving test results to a TRX file.
    /// </summary>
    [TestMethod]
    public void Test_TestResults_SaveToTrx()
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
            File.Delete("TestResults.trx");
        }
    }
}