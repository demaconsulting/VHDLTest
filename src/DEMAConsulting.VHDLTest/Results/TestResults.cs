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

using System.Text;
using System.Xml.Linq;
using DEMAConsulting.VHDLTest.Run;
using DEMAConsulting.VHDLTest.Simulators;

namespace DEMAConsulting.VHDLTest.Results;

/// <summary>
///     Test Results class
/// </summary>
public sealed class TestResults
{
    /// <summary>
    ///     Namespace for TRX files
    /// </summary>
    private static readonly XNamespace TrxNamespace = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010";

    /// <summary>
    ///     Initializes a new instance of the TestResults class
    /// </summary>
    /// <param name="runName">Test Run Name</param>
    /// <param name="codeBase">Code Base</param>
    public TestResults(string runName, string codeBase)
    {
        RunName = runName;
        CodeBase = codeBase;
    }

    /// <summary>
    ///     Gets the Test Run ID
    /// </summary>
    public Guid RunId { get; init; } = Guid.NewGuid();

    /// <summary>
    ///     Gets the Test Run Name
    /// </summary>
    public string RunName { get; init; }

    /// <summary>
    ///     Gets the Code Base
    /// </summary>
    public string CodeBase { get; init; }

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
    /// <param name="options">Test options</param>
    /// <param name="simulator">Simulator</param>
    /// <returns>Test results</returns>
    public static TestResults Execute(Options options, Simulator simulator)
    {
        return Execute(
            $"{Environment.UserName}@{Environment.MachineName} {DateTime.Now}",
            options.WorkingDirectory,
            options,
            simulator);
    }

    /// <summary>
    ///     Execute the requested tests
    /// </summary>
    /// <param name="runName">Run name</param>
    /// <param name="codeBase">Code base</param>
    /// <param name="options">Test options</param>
    /// <param name="simulator">Simulator</param>
    /// <returns>Test results</returns>
    public static TestResults Execute(string runName, string codeBase, Options options, Simulator simulator)
    {
        // Construct the results
        var results = new TestResults(runName, codeBase);

        // Run the build
        Console.WriteLine($"Building with {simulator.SimulatorName}...");
        results.BuildResults = simulator.Compile(options);
        results.BuildResults.Print(options.Verbose);
        if (results.BuildResults.Summary >= RunLineType.Error)
            throw new InvalidOperationException("Build Failed");

        // Report pass of build
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Build Passed");
        Console.WriteLine();
        Console.ResetColor();

        // Execute the tests
        var tests = options.CustomTests ?? options.Config.Tests.AsReadOnly();
        foreach (var test in tests)
        {
            // Report start of test
            Console.WriteLine($"Starting {test}");

            // Run the test
            var testResult = simulator.Test(options, test);
            testResult.RunResults.Print(options.Verbose);
            results.Tests.Add(testResult);

            // Print the result splash
            testResult.PrintSummary();
            Console.WriteLine();
        }

        // Return the results
        return results;
    }

    /// <summary>
    ///     Save results to TRX file
    /// </summary>
    /// <param name="fileName">TRX file name</param>
    public void SaveToTrx(string fileName)
    {
        // Construct the root element
        var root = new XElement(TrxNamespace + "TestRun",
            new XAttribute("id", RunId),
            new XAttribute("name", RunName));

        // Construct the results
        var results = new XElement(TrxNamespace + "Results");
        root.Add(results);
        foreach (var r in Tests)
        {
            // Construct the unit test results
            var unitTestResult = new XElement(TrxNamespace + "UnitTestResult",
                new XAttribute("executionId", r.ExecutionId),
                new XAttribute("testId", r.TestId),
                new XAttribute("testName", r.TestName),
                new XAttribute("computerName", Environment.MachineName),
                new XAttribute("testType", "13CDC9D9-DDB5-4fa4-A97D-D965CCFC6D4B"),
                new XAttribute("outcome", r.RunResults.Summary == RunLineType.Error ? "Failed" : "Passed"),
                new XAttribute("testListId", "19431567-8539-422a-85D7-44EE4E166BDA"));
            results.Add(unitTestResult);

            // Construct the output
            var output = new XElement(TrxNamespace + "Output");
            unitTestResult.Add(output);

            // Add the standard output
            output.Add(
                new XElement(TrxNamespace + "StdOut",
                    new XCData(r.RunResults.Output)));

            // Add the optional error information
            if (r.Failed)
                output.Add(
                    new XElement(TrxNamespace + "ErrorInfo",
                        new XElement("Message",
                            new XCData(
                                string.Join(
                                    '\n',
                                    r.RunResults.Lines
                                        .Where(l => l.Type == RunLineType.Error)
                                        .Select(l => l.Text))))));
        }

        // Construct the definitions
        var definitions = new XElement(TrxNamespace + "TestDefinitions");
        root.Add(definitions);
        foreach (var r in Tests)
            definitions.Add(
                new XElement(TrxNamespace + "UnitTest",
                    new XAttribute("name", r.TestName),
                    new XAttribute("id", r.TestId),
                    new XElement(TrxNamespace + "Execution",
                        new XAttribute("id", r.ExecutionId)),
                    new XElement(TrxNamespace + "TestMethod",
                        new XAttribute("codeBase", CodeBase),
                        new XAttribute("className", r.ClassName),
                        new XAttribute("name", r.TestName))));

        // Construct the test entries
        var testEntries = new XElement(TrxNamespace + "TestEntries");
        root.Add(testEntries);
        foreach (var r in Tests)
            testEntries.Add(
                new XElement(TrxNamespace + "TestEntry",
                    new XAttribute("testId", r.TestId),
                    new XAttribute("executionId", r.ExecutionId),
                    new XAttribute("testListId", "19431567-8539-422a-85D7-44EE4E166BDA")));

        // Construct the test lists
        root.Add(
            new XElement(TrxNamespace + "TestLists",
                new XElement(TrxNamespace + "TestList",
                    new XAttribute("name", "All Loaded Results"),
                    new XAttribute("id", "19431567-8539-422a-85D7-44EE4E166BDA"))));

        // Construct the complete stdout
        var stdout = new StringBuilder();

        // Add the build results
        if (BuildResults != null)
            foreach (var line in BuildResults.Lines)
                stdout.AppendLine(line.Text);

        // Add all test executions
        foreach (var line in Tests.Select(r => r.RunResults.Output))
            stdout.AppendLine(line);

        // Construct the summary
        root.Add(
            new XElement(
                TrxNamespace + "ResultSummary",
                new XAttribute("outcome", "Completed"),
                new XElement(
                    TrxNamespace + "Counters",
                    new XAttribute("total", Tests.Count),
                    new XAttribute("executed", Tests.Count),
                    new XAttribute("passed", Passes.Count()),
                    new XAttribute("failed", Fails.Count()))),
            new XElement(
                TrxNamespace + "Output",
                new XElement(
                    TrxNamespace + "StdOut",
                    new XCData(stdout.ToString()))));

        // Save the document
        var doc = new XDocument(root);
        doc.Save(fileName);
    }

    /// <summary>
    ///     Print test results summary
    /// </summary>
    public void PrintSummary()
    {
        // Print the summary table
        Console.WriteLine("==== summary ===========================");
        foreach (var r in Tests)
            r.PrintSummary();
        Console.WriteLine("========================================");

        // Get the totals
        var total = Tests.Count;
        var passed = Passes.Count();
        var failed = Fails.Count();

        // Print pass totals
        if (passed > 0)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Passed");
            Console.ResetColor();
            Console.WriteLine($" {passed} of {total} tests");
        }

        // Print fail totals
        if (failed > 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Failed");
            Console.ResetColor();
            Console.WriteLine($" {failed} of {total} tests");
        }
    }
}