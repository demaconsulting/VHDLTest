// Copyright (c) 2024 DEMA Consulting
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
using DEMAConsulting.VHDLTest.Results;
using DEMAConsulting.VHDLTest.Run;

namespace DEMAConsulting.VHDLTest.Simulators;

/// <summary>
///     Mock Simulator Class
/// </summary>
public sealed class MockSimulator : Simulator
{
    /// <summary>
    /// Compile processor
    /// </summary>
    public static readonly RunProcessor CompileProcessor = new(
        [
            RunLineRule.Create(RunLineType.Info, "Info:"),
            RunLineRule.Create(RunLineType.Warning, "Warning:"),
            RunLineRule.Create(RunLineType.Error, "Error:")
        ]
    );

    /// <summary>
    /// Test processor
    /// </summary>
    public static readonly RunProcessor TestProcessor = new(
        [
            RunLineRule.Create(RunLineType.Info, "Info:"),
            RunLineRule.Create(RunLineType.Warning, "Warning:"),
            RunLineRule.Create(RunLineType.Error, "Failure:"),
            RunLineRule.Create(RunLineType.Error, "Error:")
        ]
    );

    /// <summary>
    ///     Mock simulator instance
    /// </summary>
    public static readonly MockSimulator Instance = new();

    /// <summary>
    ///     Initializes a new instance of the Mock simulator
    /// </summary>
    private MockSimulator() : base("Mock", null)
    {
    }

    /// <inheritdoc />
    public override RunResults Compile(Context context, Options options)
    {
        // Log the start of the compile command
        context.WriteVerboseLine("Starting Mock compile...");

        // Simulate compiling, and produce output
        var output = new StringBuilder();
        var exitCode = 0;
        foreach (var file in options.Config.Files)
        {
            // Use filename to determine type of result
            if (file.Contains("_error_"))
            {
                // File produces error and compile fails
                output.AppendLine($"Error: {file}");
                exitCode = 1;
            }
            else if (file.Contains("_warning_"))
            {
                // File produces warning
                output.AppendLine($"Warning: {file}");
            }
            else if (file.Contains("_info_"))
            {
                // File produces info
                output.AppendLine($"Info: {file}");
            }
            else
            {
                // File produces no special output
                output.AppendLine($"Compiled {file}");
            }
        }

        // Return the compile results
        return CompileProcessor.Parse(
            DateTime.Now,
            DateTime.Now,
            output.ToString(),
            exitCode);
    }

    /// <inheritdoc />
    public override TestResult Test(Context context, Options options, string test)
    {
        // Log the start of the compile command
        context.WriteVerboseLine($"Starting Mock test {test}...");

        // Simulate test, and produce output
        var output = new StringBuilder();
        var exitCode = 0;

        // Add warning if desired
        if (test.Contains("_warning_"))
            output.AppendLine($"Warning: {test}");

        // Add info if desired
        if (test.Contains("_info_"))
            output.AppendLine($"Info: {test}");

        // Use test name to determine type of result
        if (test.Contains("_error_"))
        {
            output.AppendLine($"Error: {test}");
            exitCode = 1;
        }
        else if (test.Contains("_fail_"))
        {
            output.AppendLine($"Failure: {test}");
        }
        else
        {
            output.AppendLine($"Passed: {test}");
        }

        // Return the test results
        return new TestResult(
            test,
            test,
            TestProcessor.Parse(
                DateTime.Now,
                DateTime.Now,
                output.ToString(),
                exitCode));
    }
}