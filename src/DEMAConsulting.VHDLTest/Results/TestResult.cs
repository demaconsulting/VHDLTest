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

using DEMAConsulting.VHDLTest.Run;

namespace DEMAConsulting.VHDLTest.Results;

/// <summary>
/// Test result class
/// </summary>
public sealed class TestResult
{
    /// <summary>
    ///     Initializes a new instance of the TestResult class
    /// </summary>
    /// <param name="className">Class name</param>
    /// <param name="testName">Test name</param>
    /// <param name="runResults">Test run results</param>
    public TestResult(string className, string testName, RunResults runResults)
    {
        ClassName = className;
        TestName = testName;
        RunResults = runResults;
    }

    /// <summary>
    ///     Class Name
    /// </summary>
    public string ClassName { get; init; }

    /// <summary>
    ///     Test Name
    /// </summary>
    public string TestName { get; init; }

    /// <summary>
    ///     Test ID
    /// </summary>
    public Guid TestId { get; init; } = Guid.NewGuid();

    /// <summary>
    ///     Test Execution ID
    /// </summary>
    public Guid ExecutionId { get; init; } = Guid.NewGuid();

    /// <summary>
    ///     Run Results
    /// </summary>
    public RunResults RunResults { get; init; }

    /// <summary>
    ///     Gets a value indicating whether the test passed
    /// </summary>
    public bool Passed => RunResults.Summary < RunLineType.Error;

    /// <summary>
    ///     Gets a value indicating whether the test failed
    /// </summary>
    public bool Failed => RunResults.Summary >= RunLineType.Error;

    /// <summary>
    ///     Print a summary line to the console
    /// </summary>
    public void PrintSummary()
    {
        // Print the colored summary word
        if (Passed)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Passed");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Failed");
        }

        // Restore default color and print test name and duration
        Console.ResetColor();
        Console.WriteLine($" {TestName} ({RunResults.Duration:F1} seconds)");
    }
}