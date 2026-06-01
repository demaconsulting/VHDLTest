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
using System.Runtime.InteropServices;
using DEMAConsulting.VHDLTest.Cli;
using DEMAConsulting.VHDLTest.Results;
using DEMAConsulting.VHDLTest.Run;

namespace DEMAConsulting.VHDLTest.SelfTest;

/// <summary>
///     Executes VHDLTest's self-validation sequence, running embedded VHDL reference test benches
///     through the configured simulator and reporting pass/fail outcomes.
/// </summary>
internal static class Validation
{
    /// <summary>
    ///     Fixed name of the temporary folder created during self-validation.
    /// </summary>
    /// <remarks>
    ///     The name is intentionally fixed (not randomized) so that any partial folder left by
    ///     an earlier crash is deterministically located and cleaned up at the start of the next
    ///     run. As a consequence, concurrent calls to <see cref="RunValidation"/> from the same
    ///     working directory are unsafe: both callers would race to create, populate, and delete
    ///     the same folder, producing unpredictable results.
    /// </remarks>
    private const string ValidationFolder = "validation.tmp";

    /// <summary>
    ///     Executes the full self-validation sequence and reports results.
    /// </summary>
    /// <param name="context">Program context; must not be null.</param>
    /// <remarks>
    ///     Writes a Markdown-formatted system information table (VHDLTest version, machine name,
    ///     OS description, .NET runtime, UTC timestamp) then calls
    ///     <see cref="ValidateTestPasses"/> and <see cref="ValidateTestFails"/>.
    ///     If <c>context.ResultsFile</c> is non-null, saves results to that path.
    ///     Writes a final summary with total/passed/failed counts and, when no errors occurred,
    ///     a "Validation Passed" line.
    /// </remarks>
    public static void Run(Context context)
    {
        // Validate input
        ArgumentNullException.ThrowIfNull(context);

        // Write validation header
        context.WriteLine($"{new string('#', context.Depth)} DEMAConsulting.VHDLTest");
        context.WriteLine("");
        context.WriteLine("| Information         | Value                                              |");
        context.WriteLine("| :------------------ | :------------------------------------------------- |");
        context.WriteLine($"| VHDLTest Version    | {Program.Version,-50} |");
        context.WriteLine($"| Machine Name        | {Environment.MachineName,-50} |");
        context.WriteLine($"| OS Version          | {RuntimeInformation.OSDescription,-50} |");
        context.WriteLine($"| DotNet Runtime      | {RuntimeInformation.FrameworkDescription,-50} |");
        context.WriteLine($"| Time Stamp          | {$"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC",-50} |");
        context.WriteLine("");
        context.WriteLine("Tests:");
        context.WriteLine("");

        // Run validation tests
        var results = new TestResults("Validation", "VHDLTest");
        ValidateTestPasses(context, results);
        ValidateTestFails(context, results);

        // Save results if requested
        if (context.ResultsFile != null)
        {
            results.SaveResults(context.ResultsFile);
        }

        // Print summary
        var totalTests = results.Tests.Count;
        var passedTests = results.Tests.Count(t => t.Passed);
        var failedTests = results.Tests.Count(t => !t.Passed);
        context.WriteLine("");
        context.WriteLine($"Total Tests: {totalTests}");
        context.WriteLine($"Passed: {passedTests}");
        if (failedTests > 0)
        {
            context.WriteError($"Failed: {failedTests}");
        }
        else
        {
            context.WriteLine($"Failed: {failedTests}");
        }

        // If all validations succeeded (no errors) then report validation passed
        if (context.Errors == 0)
        {
            context.WriteLine("\nValidation Passed");
        }
    }

    /// <summary>
    ///     Checks that VHDLTest correctly reports passing tests as passed.
    /// </summary>
    /// <param name="context">Program context; must not be null.</param>
    /// <param name="results">Test results collection to append to; must not be null.</param>
    /// <remarks>
    ///     Calls <see cref="RunValidation"/> and verifies that the exit code is 0 and the log
    ///     contains <c>"Passed full_adder_pass_tb"</c> and <c>"Passed half_adder_pass_tb"</c>.
    /// </remarks>
    public static void ValidateTestPasses(Context context, TestResults results)
    {
        // Run the validation files
        var start = DateTime.UtcNow;
        var exitCode = RunValidation(out var output, context.Simulator);
        var duration = (DateTime.UtcNow - start).TotalSeconds;

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
    ///     Checks that VHDLTest correctly reports failing tests as failed.
    /// </summary>
    /// <param name="context">Program context; must not be null.</param>
    /// <param name="results">Test results collection to append to; must not be null.</param>
    /// <remarks>
    ///     Calls <see cref="RunValidation"/> and verifies that the exit code is 0 and the log
    ///     contains <c>"Failed full_adder_fail_tb"</c> and <c>"Failed half_adder_fail_tb"</c>.
    /// </remarks>
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
    ///     Runs VHDLTest in-process on the embedded VHDL reference files and returns the captured log.
    /// </summary>
    /// <param name="results">Populated with the captured log content on return; empty string if the log file was not written.</param>
    /// <param name="simulator">Name of the simulator to pass to VHDLTest; may be null, in which case the default simulator is used.</param>
    /// <returns>Exit code returned by the inner VHDLTest invocation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when a manifest resource stream cannot be opened.</exception>
    /// <remarks>
    ///     Creates a temporary <c>validation.tmp</c> directory, extracts embedded validation
    ///     resources into it, invokes <see cref="RunVhdlTest(string, string[])"/>, reads the
    ///     captured log file, and deletes the temporary directory in a <c>finally</c> block
    ///     (best-effort; delete failures are swallowed).
    ///     Not safe to call concurrently from the same working directory; both callers would
    ///     attempt to create and delete the same <c>validation.tmp</c> temporary folder.
    /// </remarks>
    public static int RunValidation(out string results, string? simulator)
    {
        try
        {
            // Create the temporary validation folder
            Directory.CreateDirectory(ValidationFolder);

            // Extract the validation resources
            ExtractValidationFiles(ValidationFolder);

            // Construct the arguments
            var args = new List<string>([
                "--log", "output.log",
                "--silent",
                "--config", "validate.yaml",
                "--exit-0"]);
            if (simulator != null)
            {
                args.AddRange(["--simulator", simulator]);
            }

            // Run VhdlTest on the validation files
            var exitCode = RunVhdlTest(ValidationFolder, [.. args]);

            // Read the output
            results = File.Exists($"{ValidationFolder}/output.log") ?
                File.ReadAllText($"{ValidationFolder}/output.log") :
                "";

            // Return the exit code
            return exitCode;
        }
        finally
        {
            // Best-effort cleanup: do not let delete failures mask an earlier exception
            try
            {
                Directory.Delete(ValidationFolder, true);
            }
            catch
            {
                // Swallow delete exception — cleanup failure must not mask an earlier exception
            }
        }
    }

    /// <summary>
    ///     Records and reports a single validation test outcome.
    /// </summary>
    /// <param name="context">Program context; must not be null.</param>
    /// <param name="results">Test results collection to append to; must not be null.</param>
    /// <param name="testName">Short name identifying the validation test; must not be null.</param>
    /// <param name="start">Test start time-stamp</param>
    /// <param name="duration">Test duration</param>
    /// <param name="exitCode">Program exit-code</param>
    /// <param name="output">Raw captured log content from the inner VHDLTest run. May be empty
    ///     but is never null. Line endings may be CR, LF, or CRLF depending on the platform and
    ///     the log writer.</param>
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
        {
            context.WriteLine($"✓ VHDLTest_{testName} - Passed");
        }
        else
        {
            // Write the primary failure line
            context.WriteError($"✗ VHDLTest_{testName} - Failed");

            // Write the exit code so the caller knows what the simulator returned
            context.WriteError($"  Exit code: {exitCode}");

            // Write each line of the captured output indented so it is visually grouped
            // under the failure — trim first to avoid spurious blank trailing lines.
            // Split on both CR and LF to handle Windows (CRLF) and Unix (LF) line endings.
            var trimmedOutput = output.Trim();
            if (trimmedOutput.Length > 0)
            {
                foreach (var outputLine in trimmedOutput.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries))
                {
                    context.WriteError($"  {outputLine}");
                }
            }
        }

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
    ///     Extracts embedded validation resource files to a destination directory.
    /// </summary>
    /// <param name="path">Destination directory path where files will be written; must not be null and must already exist.</param>
    /// <exception cref="InvalidOperationException">Thrown when a manifest resource stream cannot be opened (resource returned null from <see cref="System.Reflection.Assembly.GetManifestResourceStream(string)"/>).</exception>
    /// <remarks>
    ///     Embedded resources are named under the <c>DEMAConsulting.VHDLTest.ValidationFiles.</c>
    ///     prefix; the portion of each resource name after the prefix becomes the output filename
    ///     in <paramref name="path"/>. This design makes the validation suite self-contained:
    ///     all required VHDL source files are bundled inside the assembly so that self-validation
    ///     can run without any on-disk VHDL files in the user's environment.
    /// </remarks>
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
    ///     Runs VHDLTest in-process with the specified arguments.
    /// </summary>
    /// <param name="args">Command-line arguments to pass to VHDLTest; must not be null.</param>
    /// <returns>Exit code returned by the VHDLTest invocation.</returns>
    /// <remarks>
    ///     This overload runs VHDLTest in-process by creating a new <see cref="Context"/> and
    ///     calling <see cref="Program.Run"/>, avoiding subprocess overhead and allowing the
    ///     validation suite to exercise the full application pipeline without spawning a child
    ///     process. This overload is thread-safe in itself; however,
    ///     <see cref="RunVhdlTest(string, string[])"/> (the working-directory overload) is not
    ///     thread-safe because it calls <see cref="Directory.SetCurrentDirectory"/>, which is a
    ///     process-global operation.
    /// </remarks>
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
    ///     Runs VHDLTest in-process inside a specific working directory, restoring the original
    ///     directory after the run completes.
    /// </summary>
    /// <param name="workingFolder">Working directory for the VHDLTest run; must not be null.</param>
    /// <param name="args">Command-line arguments to pass to VHDLTest; must not be null.</param>
    /// <returns>Exit code returned by the VHDLTest invocation.</returns>
    /// <remarks>
    ///     This overload calls <see cref="Directory.SetCurrentDirectory"/> which is a process-global
    ///     operation; it is not safe to call concurrently from multiple threads.
    /// </remarks>
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
