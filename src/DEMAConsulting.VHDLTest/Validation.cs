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
    /// Run validation tests
    /// </summary>
    /// <param name="arguments">Parsed program arguments</param>
    /// <returns>Validation exit code</returns>
    public static int Run(Arguments arguments)
    {
        // Validation folder
        var validationDir = Path.GetFullPath("VHDLTest.out/Validation");

        try
        {
            // Create the validation directory
            if (!Directory.Exists(validationDir))
                Directory.CreateDirectory(validationDir);

            // Extract the validation resources
            ExtractValidationFiles(validationDir);

            // Run the validation
            var start = DateTime.Now;
            var exitCode = RunValidation(out var output, validationDir, arguments);
            if (exitCode != 0)
                throw new InvalidOperationException($"Validation failed with exit code {exitCode}");

            // Analyze the validation results
            var results = AnalyzeValidation(start, 0.0, output);

            // Print the results summary
            results.PrintSummary();

            // Save the test results
            if (arguments.ResultsFile != null)
                results.SaveToTrx(arguments.ResultsFile);

            // Select the exit code
            return results.Fails.Any() ? 1 : 0;
        }
        finally
        {
            // Clean up the validation directory
            Directory.Delete(validationDir, true);
        }
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
            using var stream = typeof(Validation).Assembly.GetManifestResourceStream(resource);
            if (stream == null)
            {
                throw new InvalidOperationException($"Resource {resource} not found");
            }

            // Copy the resource to the file
            using var file = File.Create(target);
            stream.CopyTo(file);
        }
    }

    /// <summary>
    /// Run the validation tests
    /// </summary>
    /// <param name="output">Output</param>
    /// <param name="validationDir">Validation directory</param>
    /// <param name="arguments">Arguments</param>
    /// <returns>Exit code</returns>
    private static int RunValidation(out string output, string validationDir, Arguments arguments)
    {
        // Construct the parameters
        var parameters = new List<string>
        {
            typeof(Validation).Assembly.Location,
            "-c",
            "validate.yaml",
            "--exit-0"
        };

        // Add simulator if specified
        if (arguments.Simulator != null)
        {
            parameters.Add("-s");
            parameters.Add(arguments.Simulator);
        }

        return RunProgram.Run(out output, "dotnet", validationDir, parameters.ToArray());
    }

    /// <summary>
    /// Analyze the validation output
    /// </summary>
    /// <param name="start">Start time</param>
    /// <param name="duration">Duration</param>
    /// <param name="output">Output</param>
    /// <returns>Results</returns>
    private static TestResults AnalyzeValidation(DateTime start, double duration, string output)
    {
        // Construct the validation results
        var results = new TestResults("validation", "validation");

        // Construct the results of the passes
        RunResults passResults;
        if (output.Contains("Passed full_adder_pass_tb") && output.Contains("Passed half_adder_pass_tb"))
            passResults = new RunResults(RunLineType.Info, start, duration, 0, output, ReadOnlyCollection<RunLine>.Empty);
        else
            passResults = new RunResults(RunLineType.Error, start, duration, 0, output, ReadOnlyCollection<RunLine>.Empty);

        // Construct the results of the passes
        RunResults failResults;
        if (output.Contains("Failed full_adder_fail_tb") && output.Contains("Failed half_adder_fail_tb"))
            failResults = new RunResults(RunLineType.Info, start, duration, 0, output, ReadOnlyCollection<RunLine>.Empty);
        else
            failResults = new RunResults(RunLineType.Error, start, duration, 0, output, ReadOnlyCollection<RunLine>.Empty);

        // Add the results to the test results
        results.Tests.Add(new TestResult("passes_reported", "passes_reported", passResults));
        results.Tests.Add(new TestResult("fails_reported", "fails_reported", failResults));

        // Return the test results
        return results;
    }
}