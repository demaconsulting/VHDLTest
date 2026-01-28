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
using DEMAConsulting.VHDLTest.Simulators;
using DemaConsulting.TestResults;
using DemaConsulting.TestResults.IO;

namespace DEMAConsulting.VHDLTest.Results;

/// <summary>
///     Test Results class
/// </summary>
/// <param name="runName">Test Run Name</param>
/// <param name="codeBase">Code Base</param>
public sealed class TestResults(string runName, string codeBase)
{
    /// <summary>
    ///     Gets the Test Run ID
    /// </summary>
    public Guid RunId { get; init; } = Guid.NewGuid();

    /// <summary>
    ///     Gets the Test Run Name
    /// </summary>
    public string RunName => runName;

    /// <summary>
    ///     Gets the Code Base
    /// </summary>
    public string CodeBase => codeBase;

    /// <summary>
    ///     Gets or sets the build run information
    /// </summary>
    public RunResults? BuildResults { get; set; }

    /// <summary>
    ///     Gets the tests
    /// </summary>
    public List<TestResult> Tests { get; init; } = [];

    /// <summary>
    ///     Gets the passed tests
    /// </summary>
    public IEnumerable<TestResult> Passes => Tests.Where(r => r.Passed);

    /// <summary>
    ///     Gets the failing tests
    /// </summary>
    public IEnumerable<TestResult> Fails => Tests.Where(r => r.Failed);

    /// <summary>
    ///     Execute the requested tests
    /// </summary>
    /// <param name="context">Program context</param>
    /// <param name="options">Test options</param>
    /// <param name="simulator">Simulator</param>
    /// <returns>Test results</returns>
    public static TestResults Execute(Context context, Options options, Simulator simulator)
    {
        return Execute(
            context,
            $"{Environment.UserName}@{Environment.MachineName} {DateTime.Now}",
            options.WorkingDirectory,
            options,
            simulator);
    }

    /// <summary>
    ///     Execute the requested tests
    /// </summary>
    /// <param name="context">Program context</param>
    /// <param name="runName">Run name</param>
    /// <param name="codeBase">Code base</param>
    /// <param name="options">Test options</param>
    /// <param name="simulator">Simulator</param>
    /// <returns>Test results</returns>
    public static TestResults Execute(Context context, string runName, string codeBase, Options options, Simulator simulator)
    {
        // Construct the results
        var results = new TestResults(runName, codeBase);

        // Run the build
        context.WriteLine($"Building with {simulator.SimulatorName}...");
        results.BuildResults = simulator.Compile(context, options);
        results.BuildResults.Print(context);
        if (results.BuildResults.Summary >= RunLineType.Error)
            throw new InvalidOperationException("Build Failed");

        // Report pass of build
        context.Write(ConsoleColor.Green,
            """
            Build Passed
            
            
            """);

        // Execute the tests
        var tests = context.CustomTests ?? options.Config.Tests.AsReadOnly();
        foreach (var test in tests)
        {
            // Report start of test
            context.WriteLine($"Starting {test}");

            // Run the test
            var testResult = simulator.Test(context, options, test);
            testResult.RunResults.Print(context);
            results.Tests.Add(testResult);

            // Print the result splash
            testResult.PrintSummary(context);
            context.WriteLine("");
        }

        // Return the results
        return results;
    }

    /// <summary>
    ///     Save results to a file (TRX or JUnit based on file extension)
    /// </summary>
    /// <param name="fileName">File name (extension determines format: .trx for TRX, .xml for JUnit, others default to TRX)</param>
    /// <exception cref="ArgumentException">Thrown when fileName is null or empty</exception>
    public void SaveResults(string fileName)
    {
        // Validate parameter
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name cannot be null or empty", nameof(fileName));

        // Create the TestResults from the library
        var testResults = new DemaConsulting.TestResults.TestResults
        {
            Id = RunId,
            Name = RunName
        };

        // Add each test result
        foreach (var test in Tests)
        {
            var result = new DemaConsulting.TestResults.TestResult
            {
                TestId = test.TestId,
                ExecutionId = test.ExecutionId,
                Name = test.TestName,
                ClassName = test.ClassName,
                CodeBase = CodeBase,
                Outcome = test.Passed ? TestOutcome.Passed : TestOutcome.Failed,
                StartTime = test.RunResults.Start,
                Duration = TimeSpan.FromSeconds(test.RunResults.Duration),
                SystemOutput = test.RunResults.Output
            };

            // Add error information if the test failed
            if (test.Failed)
            {
                result.ErrorMessage = string.Join(
                    '\n',
                    test.RunResults.Lines
                        .Where(l => l.Type == RunLineType.Error)
                        .Select(l => l.Text));
            }

            testResults.Results.Add(result);
        }

        // Determine format based on file extension (.xml = JUnit, others = TRX)
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        var content = extension == ".xml"
            ? JUnitSerializer.Serialize(testResults)
            : TrxSerializer.Serialize(testResults);

        // Save the file
        File.WriteAllText(fileName, content);
    }

    /// <summary>
    ///     Save results to TRX file (backward compatibility)
    /// </summary>
    /// <param name="fileName">TRX file name</param>
    public void SaveToTrx(string fileName)
    {
        SaveResults(fileName);
    }

    /// <summary>
    ///     Print test results summary
    /// </summary>
    /// <param name="context">Program context</param>
    public void PrintSummary(Context context)
    {
        // Print the summary table
        context.WriteLine("==== summary ===========================");
        foreach (var r in Tests)
            r.PrintSummary(context);
        context.WriteLine("========================================");

        // Get the totals
        var total = Tests.Count;
        var passed = Passes.Count();
        var failed = Fails.Count();

        // Print pass totals
        if (passed > 0)
        {
            context.Write(ConsoleColor.Green, "Passed");
            context.WriteLine($" {passed} of {total} tests");
        }

        // Print fail totals
        if (failed > 0)
        {
            context.Write(ConsoleColor.Red, "Failed");
            context.WriteLine($" {failed} of {total} tests");
        }
    }
}
