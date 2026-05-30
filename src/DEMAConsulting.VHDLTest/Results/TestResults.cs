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

using DemaConsulting.TestResults;
using DemaConsulting.TestResults.IO;
using DEMAConsulting.VHDLTest.Cli;
using DEMAConsulting.VHDLTest.Run;
using DEMAConsulting.VHDLTest.Simulators;

namespace DEMAConsulting.VHDLTest.Results;

/// <summary>
///     Aggregate container for a complete VHDLTest run, driving build and test execution
///     through a simulator and serializing outcomes to standard report formats.
/// </summary>
/// <remarks>
///     TestResults owns the compile-then-test workflow: <c>Execute</c> invokes the
///     simulator for the build step and for each test bench in sequence, collecting
///     <see cref="TestResult"/> instances in <see cref="Tests"/>. After execution callers
///     can print a human-readable summary via <see cref="PrintSummary"/> and persist the
///     collection as TRX or JUnit XML via <see cref="SaveResults"/>.
///     Instances are not thread-safe; concurrent access to a single instance requires
///     external synchronization. The static <c>Execute</c> methods are safe to call
///     concurrently provided each call uses independent arguments.
/// </remarks>
/// <param name="runName">
///     Human-readable label for this test run used in generated reports. Must not be null.
/// </param>
/// <param name="codeBase">
///     Path or label identifying the source tree under test, stored in TRX output for
///     traceability. Must not be null.
/// </param>
public sealed class TestResults(string runName, string codeBase)
{
    /// <summary>
    ///     Gets the Test Run ID
    /// </summary>
    /// <remarks>
    ///     Unique identifier for this test run instance; correlates TRX output back to a
    ///     specific VHDLTest invocation.
    /// </remarks>
    public Guid RunId { get; init; } = Guid.NewGuid();

    /// <summary>
    ///     Gets the Test Run Name
    /// </summary>
    /// <remarks>
    ///     Human-readable label set at construction; used in generated reports to identify
    ///     who ran the tests and when.
    /// </remarks>
    public string RunName => runName;

    /// <summary>
    ///     Gets the Code Base
    /// </summary>
    /// <remarks>
    ///     Path or label identifying the source tree under test; stored in TRX output so
    ///     report consumers can trace results back to the code revision.
    /// </remarks>
    public string CodeBase => codeBase;

    /// <summary>
    ///     Gets or sets the build run information
    /// </summary>
    /// <remarks>
    ///     Null before <c>Execute</c> completes; set once compilation finishes
    ///     regardless of whether the build passed or failed.
    /// </remarks>
    public RunResults? BuildResults { get; set; }

    /// <summary>
    ///     Gets the tests
    /// </summary>
    /// <remarks>
    ///     Ordered list of individual test bench outcomes, one per configured test bench;
    ///     populated in order by the <c>Execute</c> method.
    /// </remarks>
    public List<TestResult> Tests { get; init; } = [];

    /// <summary>
    ///     Gets the passed tests
    /// </summary>
    /// <remarks>
    ///     Lazily enumerated view over <see cref="Tests"/> containing only outcomes where
    ///     the test bench ran without error; used by <see cref="PrintSummary"/> to compute
    ///     pass counts.
    /// </remarks>
    public IEnumerable<TestResult> Passes => Tests.Where(r => r.Passed);

    /// <summary>
    ///     Gets the failing tests
    /// </summary>
    /// <remarks>
    ///     Lazily enumerated view over <see cref="Tests"/> containing only outcomes where
    ///     the test bench encountered an error; used by <see cref="PrintSummary"/> to
    ///     compute fail counts.
    /// </remarks>
    public IEnumerable<TestResult> Fails => Tests.Where(r => r.Failed);

    /// <summary>
    ///     Execute the requested tests
    /// </summary>
    /// <remarks>
    ///     Convenience overload that derives the run name from the current user, machine
    ///     name, and local timestamp, and the code base from the options working directory.
    ///     Delegates to the explicit overload; stateless and safe to call concurrently for
    ///     independent contexts.
    /// </remarks>
    /// <param name="context">Program context providing output channels. Must not be null.</param>
    /// <param name="options">Test options supplying working directory and test list. Must not be null.</param>
    /// <param name="simulator">Simulator to use for compilation and test execution. Must not be null.</param>
    /// <returns>Populated <see cref="TestResults"/> instance with build and test outcomes.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the build step reports errors.</exception>
    public static TestResults Execute(Context context, Options options, Simulator simulator)
    {
        // Delegate to the explicit overload, deriving the run name from the current user and
        // machine context and the code base from the options working directory
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
    /// <remarks>
    ///     Primary execution method. Invokes the simulator's compile step; throws
    ///     <see cref="InvalidOperationException"/> with the message "Build Failed" if the
    ///     build reports errors. On success iterates <see cref="Context.CustomTests"/> when
    ///     set, or <c>options.Config.Tests</c>, calling the simulator's test step for each
    ///     and appending a <see cref="TestResult"/> to <see cref="Tests"/>. Stateless with
    ///     respect to <c>this</c>; each call constructs a fresh <see cref="TestResults"/>
    ///     instance.
    /// </remarks>
    /// <param name="context">Program context providing output channels. Must not be null.</param>
    /// <param name="runName">Human-readable run label used in generated reports. Must not be null.</param>
    /// <param name="codeBase">Path or label identifying the code under test. Must not be null.</param>
    /// <param name="options">Test options supplying working directory and test list. Must not be null.</param>
    /// <param name="simulator">Simulator to use for compilation and test execution. Must not be null.</param>
    /// <returns>Populated <see cref="TestResults"/> instance with build and test outcomes.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the build step reports errors.</exception>
    public static TestResults Execute(Context context, string runName, string codeBase, Options options, Simulator simulator)
    {
        // Construct the results
        var results = new TestResults(runName, codeBase);

        // Run the build
        context.WriteLine($"Building with {simulator.SimulatorName}...");
        results.BuildResults = simulator.Compile(context, options);
        results.BuildResults.Print(context);
        if (results.BuildResults.Summary >= RunLineType.Error)
        {
            throw new InvalidOperationException("Build Failed");
        }

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
    /// <remarks>
    ///     Selects TRX format for <c>.trx</c> extensions and JUnit XML for <c>.xml</c> extensions;
    ///     all other extensions default to TRX. Writes the serialized content to disk via
    ///     <see cref="File.WriteAllText(string, string)"/>; the file is created or overwritten. Throws
    ///     <see cref="ArgumentException"/> before any I/O if <paramref name="fileName"/> is
    ///     null, empty, or whitespace-only.
    /// </remarks>
    /// <param name="fileName">File name (extension determines format: .trx for TRX, .xml for JUnit, others default to TRX)</param>
    /// <exception cref="ArgumentException">Thrown when fileName is null or empty</exception>
    public void SaveResults(string fileName)
    {
        // Validate parameter
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("File name cannot be null or empty", nameof(fileName));
        }

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
    /// <remarks>
    ///     Backward-compatibility wrapper that delegates unconditionally to
    ///     <see cref="SaveResults"/>. Retained so callers that previously used
    ///     <c>SaveToTrx</c> continue to work without modification.
    /// </remarks>
    /// <param name="fileName">TRX file name</param>
    public void SaveToTrx(string fileName)
    {
        SaveResults(fileName);
    }

    /// <summary>
    ///     Print test results summary
    /// </summary>
    /// <remarks>
    ///     Writes a separator line, then one summary line per test in <see cref="Tests"/> via
    ///     <see cref="TestResult.PrintSummary"/>. After the per-test lines, writes a closing
    ///     separator, then aggregate "Passed N of M" (green) and/or "Failed N of M" (red)
    ///     totals. All output goes through the injected <paramref name="context"/>; the method
    ///     has no other side effects. <paramref name="context"/> must not be null.
    /// </remarks>
    /// <param name="context">Program context</param>
    public void PrintSummary(Context context)
    {
        // Print the summary table
        context.WriteLine("==== summary ===========================");
        foreach (var r in Tests)
        {
            r.PrintSummary(context);
        }

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
