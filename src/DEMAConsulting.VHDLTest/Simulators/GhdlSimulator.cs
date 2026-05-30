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
using DEMAConsulting.VHDLTest.Cli;
using DEMAConsulting.VHDLTest.Results;
using DEMAConsulting.VHDLTest.Run;

namespace DEMAConsulting.VHDLTest.Simulators;

/// <summary>
///     Concrete <see cref="Simulator"/> implementation for the GHDL open-source VHDL simulator.
/// </summary>
/// <remarks>
///     Drives the <c>ghdl</c> command-line tool in a two-phase workflow: the <c>Compile</c>
///     method runs <c>ghdl -a</c> (analysis) for all source files, and the <c>Test</c> method
///     runs <c>ghdl -e</c> (elaboration) followed by <c>ghdl -r</c> (simulation) for each
///     individual test bench. The elaboration step is explicit and mandatory for some GHDL
///     backends. Implemented as a singleton (<see cref="Instance"/>) initialized at class
///     load time; stateless after construction and therefore thread-safe.
/// </remarks>
public sealed class GhdlSimulator : Simulator
{
    /// <summary>
    ///     Output classifier for GHDL analysis (<c>ghdl -a</c>) and elaboration (<c>ghdl -e</c>) output.
    /// </summary>
    /// <remarks>
    ///     Applies four classification rules in order:
    ///     <list type="bullet">
    ///         <item><description>
    ///             Lines matching <c>.*:\d+:\d+:warning:</c> are classified as Warning.
    ///         </description></item>
    ///         <item><description>
    ///             Lines matching <c>.*:\d+:\d+: </c> (trailing space prevents false matches on
    ///             warning-prefixed lines) are classified as Error.
    ///         </description></item>
    ///         <item><description>
    ///             Lines matching <c>.*:error:</c> are classified as Error.
    ///         </description></item>
    ///         <item><description>
    ///             Lines matching <c>.*: cannot open</c> are classified as Error.
    ///         </description></item>
    ///     </list>
    ///     This processor is also reused for the elaboration step in <c>Test</c> because elaboration
    ///     output follows the same diagnostic format as analysis output.
    /// </remarks>
    public static RunProcessor CompileProcessor { get; } = new(
        [
            RunLineRule.Create(RunLineType.Warning, @".*:\d+:\d+:warning:"),
            RunLineRule.Create(RunLineType.Error, @".*:\d+:\d+: "),
            RunLineRule.Create(RunLineType.Error, ".*:error:"),
            RunLineRule.Create(RunLineType.Error, ".*: cannot open")
        ]
    );

    /// <summary>
    ///     Output classifier for GHDL simulation (<c>ghdl -r</c>) output.
    /// </summary>
    /// <remarks>
    ///     Applies classification rules matching GHDL's VHDL assertion and report message format:
    ///     <list type="bullet">
    ///         <item><description>
    ///             Lines matching <c>.*:\(assertion note\):</c> or <c>.*:\(report note\):</c>
    ///             are classified as Info.
    ///         </description></item>
    ///         <item><description>
    ///             Lines matching <c>.*:\(assertion warning\):</c> or <c>.*:\(report warning\):</c>
    ///             are classified as Warning.
    ///         </description></item>
    ///         <item><description>
    ///             Lines matching <c>.*:\(assertion error\):</c>, <c>.*:\(report error\):</c>,
    ///             <c>.*:\(assertion failure\):</c>, <c>.*:\(report failure\):</c>, or
    ///             <c>.*:error:</c> are classified as Error.
    ///         </description></item>
    ///     </list>
    /// </remarks>
    public static RunProcessor TestProcessor { get; } = new(
        [
            RunLineRule.Create(RunLineType.Info, @".*:\(assertion note\):"),
            RunLineRule.Create(RunLineType.Info, @".*:\(report note\):"),
            RunLineRule.Create(RunLineType.Warning, @".*:\(assertion warning\):"),
            RunLineRule.Create(RunLineType.Warning, @".*:\(report warning\):"),
            RunLineRule.Create(RunLineType.Error, @".*:\(assertion error\):"),
            RunLineRule.Create(RunLineType.Error, @".*:\(report error\):"),
            RunLineRule.Create(RunLineType.Error, @".*:\(assertion failure\):"),
            RunLineRule.Create(RunLineType.Error, @".*:\(report failure\):"),
            RunLineRule.Create(RunLineType.Error, ".*:error:")
        ]
    );

    /// <summary>
    ///     Gets the singleton <see cref="GhdlSimulator"/> instance shared across the application.
    /// </summary>
    /// <remarks>
    ///     Initialized once at class-load time by calling <see cref="FindPath()"/> to resolve the
    ///     GHDL installation directory. Stateless after construction and therefore thread-safe.
    ///     Always access the simulator through this property rather than constructing a new instance.
    /// </remarks>
    public static GhdlSimulator Instance { get; } = new();

    /// <summary>
    ///     Private constructor that prevents external instantiation and enforces use of the
    ///     singleton <see cref="Instance"/>. Passes the fixed name <c>"GHDL"</c> and the path
    ///     resolved by <see cref="FindPath()"/> to the base class.
    /// </summary>
    private GhdlSimulator() : base("GHDL", FindPath())
    {
    }

    /// <inheritdoc />
    public override RunResults Compile(Context context, Options options)
    {
        // Log the start of the compile command
        context.WriteVerboseLine("Starting GHDL compile...");

        // Fail if we cannot find the simulator
        var simPath = SimulatorPath ??
                      throw new InvalidOperationException("GHDL Simulator not available");
        context.WriteVerboseLine($"  Simulator Path: {simPath}");

        // Create the library directory
        var libDir = Path.Combine(options.WorkingDirectory, "VHDLTest.out", "GHDL");
        context.WriteVerboseLine($"  Library Directory: {libDir}");
        if (!Directory.Exists(libDir))
        {
            Directory.CreateDirectory(libDir);
        }

        // Build the batch file
        var writer = new StringBuilder();
        foreach (var file in options.Config.Files)
        {
            writer.AppendLine(file);
        }

        // Write the batch file
        var script = Path.Combine(libDir, "compile.rsp");
        context.WriteVerboseLine($"  Script File: {script}");
        File.WriteAllText(script, writer.ToString());

        // Run the GHDL compiler
        var application = Path.Combine(simPath, "ghdl");
        return CompileProcessor.Execute(
            context,
            application,
            options.WorkingDirectory,
            "-a",
            "--std=08",
            "--workdir=VHDLTest.out/GHDL",
            "@VHDLTest.out/GHDL/compile.rsp");
    }

    /// <inheritdoc />
    public override TestResult Test(Context context, Options options, string test)
    {
        // Log the start of the test command
        context.WriteVerboseLine($"Starting GHDL test {test}...");

        // Fail if we cannot find the simulator
        var simPath = SimulatorPath ??
                      throw new InvalidOperationException("GHDL Simulator not available");
        context.WriteVerboseLine($"  Simulator Path: {simPath}");

        // Get the library directory
        var libDir = Path.Combine(options.WorkingDirectory, "VHDLTest.out", "GHDL");
        context.WriteVerboseLine($"  Library Directory: {libDir}");

        // Elaborate the test before running it; some GHDL backends require an explicit
        // elaboration step prior to execution.
        var application = Path.Combine(simPath, "ghdl");
        var elaborateResults = CompileProcessor.Execute(
            context,
            application,
            options.WorkingDirectory,
            "-e",
            "--std=08",
            "--workdir=VHDLTest.out/GHDL",
            test);

        // Return elaboration failure immediately without attempting to run
        if (elaborateResults.Summary >= RunLineType.Error)
        {
            return new TestResult(test, test, elaborateResults);
        }

        // Run the test
        var testRunResults = TestProcessor.Execute(
            context,
            application,
            options.WorkingDirectory,
            "-r",
            "--std=08",
            "--workdir=VHDLTest.out/GHDL",
            test);

        // Return the test results
        return new TestResult(
            test,
            test,
            testRunResults);
    }

    /// <summary>
    ///     Searches for the GHDL installation directory by locating the <c>ghdl</c> executable
    ///     on the system PATH, returning null when GHDL is not installed.
    /// </summary>
    /// <remarks>
    ///     Called at class-load time to initialize the <see cref="Instance"/> singleton; the
    ///     result is stored as <see cref="Simulator.SimulatorPath"/> and never re-evaluated.
    ///     The <c>VHDLTEST_GHDL_PATH</c> environment variable takes priority over PATH search:
    ///     when set, its value is returned immediately without invoking <see cref="Simulator.Where"/>.
    ///     When the environment variable is not set, <see cref="Simulator.Where"/> performs the
    ///     PATH search and returns the full path to the <c>ghdl</c> executable, from which the
    ///     containing directory is derived.
    /// </remarks>
    /// <returns>
    ///     The directory containing the <c>ghdl</c> executable, or null if GHDL is not found.
    ///     The <c>VHDLTEST_GHDL_PATH</c> environment variable, when set, overrides PATH search.
    /// </returns>
    public static string? FindPath()
    {
        // Look for an environment variable
        var simPathEnv = Environment.GetEnvironmentVariable("VHDLTEST_GHDL_PATH");
        if (simPathEnv != null)
        {
            return simPathEnv;
        }

        // Find the path to the simulator application
        var simPath = Where("ghdl");
        if (simPath == null)
        {
            return null;
        }

        // Return the directory containing the GHDL executable
        return Path.GetDirectoryName(simPath);
    }
}
