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

namespace DEMAConsulting.VHDLTest;

/// <summary>
/// Validation runner
/// </summary>
public static class Validation
{
    /// <summary>
    ///     Run self-validation
    /// </summary>
    /// <param name="context">Program context</param>
    public static void Run(Context context)
    {
        // Write validation header
        context.WriteLine(
            $"""
             {new string('#', context.Depth)} DEMAConsulting.VHDLTest

             | Information         | Value                                              |
             | :------------------ | :------------------------------------------------- |
             | VHDLTest Version    | {Program.Version,-50} |
             | Machine Name        | {Environment.MachineName,-50} |
             | OS Version          | {Environment.OSVersion.VersionString,-50} |
             | DotNet Runtime      | {Environment.Version,-50} |
             | Time Stamp          | {DateTime.UtcNow,-50:u} |

             Tests:
              
             """);

        // Run validation tests
        var results = new TestResults("Validation", "VHDLTest");
        ValidateTestPasses(context, results);
        ValidateTestFails(context, results);

        // Save results if requested
        if (context.ResultsFile != null)
            results.SaveToTrx(context.ResultsFile);

        // If all validations succeeded (no errors) then report validation passed
        if (context.Errors == 0)
            context.WriteLine("\nValidation Passed");
    }

    /// <summary>
    ///     Validate test passes are reported
    /// </summary>
    /// <param name="context">Program context</param>
    /// <param name="results">Test results</param>
    public static void ValidateTestPasses(Context context, TestResults results)
    {
        // Run the validation files
        var start = DateTime.UtcNow;
        var exitCode = RunValidation(out var output, context.Simulator);
        var duration = (DateTime.UtcNow -  start).TotalSeconds;

        // Determine if the test succeeded
        var succeeded =
            exitCode == 0 &&
            output.Contains("Passed full_adder_pass_tb") &&
            output.Contains("Passed half_adder_pass_tb");

        // Report result
        ReportTestResult(
            context,
            results,
            "TestPasses",
            start,
            duration,
            exitCode,
            output,
            succeeded);
    }

    /// <summary>
    ///     Validate test fails are reported
    /// </summary>
    /// <param name="context">Program context</param>
    /// <param name="results">Test results</param>
    public static void ValidateTestFails(Context context, TestResults results)
    {
        // Run the validation files
        var start = DateTime.UtcNow;
        var exitCode = RunValidation(out var output, context.Simulator);
        var duration = (DateTime.UtcNow - start).TotalSeconds;

        // Determine if the test succeeded
        var succeeded =
            exitCode == 0 &&
            output.Contains("Failed full_adder_fail_tb") &&
            output.Contains("Failed half_adder_fail_tb");

        // Report result
        ReportTestResult(
            context,
            results,
            "TestFails",
            start,
            duration,
            exitCode,
            output,
            succeeded);
    }

    /// <summary>
    ///     Run the simulator
    /// </summary>
    /// <param name="results">Results output</param>
    /// <param name="simulator">Simulator to use</param>
    /// <returns>Exit code</returns>
    public static int RunValidation(out string results, string? simulator)
    {
        try
        {
            // Create the temporary validation folder
            Directory.CreateDirectory("validation.tmp");

            // Extract the validation resources
            ExtractValidationFiles("validation.tmp");

            // Construct the arguments
            var args = new List<string>([
                "--log", "output.log",
                "--silent",
                "--config", "validate.yaml",
                "--exit-0"]);
            if (simulator != null)
                args.AddRange(["--simulator", simulator]);

            // Run VhdlTest on the validation files
            var exitCode = RunVhdlTest("validation.tmp", [..args]);

            // Read the output
            results = File.Exists("validation.tmp/output.log") ? 
                File.ReadAllText("validation.tmp/output.log") : 
                "";

            // Return the exit code
            return exitCode;
        }
        finally
        {
            // Delete the validation directory
            Directory.Delete("validation.tmp", true);
        }
    }

    /// <summary>
    ///     Report validation test results
    /// </summary>
    /// <param name="context">Program context</param>
    /// <param name="results">Test results</param>
    /// <param name="testName">Test name</param>
    /// <param name="start">Test start time-stamp</param>
    /// <param name="duration">Test duration</param>
    /// <param name="exitCode">Program exit-code</param>
    /// <param name="output">Output text</param>
    /// <param name="succeeded">True if test succeeded</param>
    private static void ReportTestResult(
        Context context,
        TestResults results,
        string testName,
        DateTime start,
        double duration,
        int exitCode,
        string output,
        bool succeeded)
    {
        // Report to the context
        if (succeeded)
            context.WriteLine($"- {testName}: Passed");
        else
            context.WriteError($"- {testName}: Failed");

        // Get the line type
        var line = succeeded
            ? new RunLine(RunLineType.Info, $"{testName} Passed")
            : new RunLine(RunLineType.Error, $"{testName} Failed");

        // Report failure
        results.Tests.Add(
            new TestResult(
                "VHDLTest.Validation",
                $"VHDLTest_{testName}",
                new RunResults(
                    line.Type,
                    start,
                    duration,
                    exitCode,
                    output,
                    new ReadOnlyCollection<RunLine>([line])
                )
            )
        );
    }

    /// <summary>
    /// Extract the validation resources to the specified path
    /// </summary>
    /// <param name="path">Extraction path</param>
    /// <exception cref="InvalidOperationException">Thrown on error</exception>
    private static void ExtractValidationFiles(string path)
    {
        const string prefix = "DEMAConsulting.VHDLTest.ValidationFiles.";

        // Get the resources to extract
        var resources = typeof(Validation)
            .Assembly
            .GetManifestResourceNames()
            .Where(n => n.StartsWith(prefix))
            .ToArray();

        // Extract the resource files
        foreach (var resource in resources)
        {
            // Get the file name
            var name = resource[prefix.Length..];
            var target = Path.Combine(path, name);

            // Get the resource stream
            using var stream = typeof(Validation).Assembly.GetManifestResourceStream(resource) ?? 
                throw new InvalidOperationException($"Resource {resource} not found");

            // Copy the resource to the file
            using var file = File.Create(target);
            stream.CopyTo(file);
        }
    }

    /// <summary>
    ///     Run VhdlTest with the specified arguments
    /// </summary>
    /// <param name="args">Arguments</param>
    /// <returns>Exit code</returns>
    internal static int RunVhdlTest(string[] args)
    {
        // Create the context
        using var context = Context.Create(args);

        // Run VhdlTest
        Program.Run(context);

        // Return the exit code
        return context.ExitCode;
    }

    /// <summary>
    ///     Run VhdlTest in the specified folder with the specified arguments
    /// </summary>
    /// <param name="workingFolder">Working folder</param>
    /// <param name="args">Arguments</param>
    /// <returns>Exit code</returns>
    internal static int RunVhdlTest(string workingFolder, string[] args)
    {
        var cwd = Directory.GetCurrentDirectory();
        try
        {
            Directory.SetCurrentDirectory(workingFolder);
            return RunVhdlTest(args);
        }
        finally
        {
            Directory.SetCurrentDirectory(cwd);
        }
    }
}