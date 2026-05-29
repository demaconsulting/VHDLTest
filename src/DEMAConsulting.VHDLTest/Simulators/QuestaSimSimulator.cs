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
///     Concrete <see cref="Simulator"/> implementation for the QuestaSim commercial VHDL simulator
///     by Mentor/Siemens.
/// </summary>
/// <remarks>
///     Structurally identical to <see cref="ModelSimSimulator"/>: drives <c>vsim</c> via TCL
///     do-scripts using <c>vcom</c> for VHDL-2008 compilation and <c>vsim</c>/<c>run</c> for
///     test bench simulation. Distinguished from <see cref="ModelSimSimulator"/> by its registered
///     name ("QuestaSim"), its path environment variable (<c>VHDLTEST_QUESTASIM_PATH</c>), and its
///     working directory path. Implemented as a singleton (<see cref="Instance"/>) initialized at
///     class load time; stateless after construction and therefore thread-safe.
/// </remarks>
public sealed class QuestaSimSimulator : Simulator
{
    /// <summary>
    ///     Output classifier for QuestaSim compilation (<c>vcom</c>) output.
    /// </summary>
    /// <remarks>
    ///     Applies one classification rule: lines matching <c>.*Error: </c> (trailing space
    ///     prevents false positives on identifiers ending with "Error") are classified as Error.
    ///     Lines not matching any rule are left unclassified as Text.
    /// </remarks>
    public static RunProcessor CompileProcessor { get; } = new(
        [
            RunLineRule.Create(RunLineType.Error, ".*Error: ")
        ]
    );

    /// <summary>
    ///     Output classifier for QuestaSim simulation (<c>vsim</c>) output.
    /// </summary>
    /// <remarks>
    ///     Applies four classification rules in order (each includes a trailing space to
    ///     prevent false positives on identifiers ending with the keyword):
    ///     <list type="bullet">
    ///         <item><description>Lines matching <c>.*Note: </c> are classified as Info.</description></item>
    ///         <item><description>Lines matching <c>.*Warning: </c> are classified as Warning.</description></item>
    ///         <item><description>Lines matching <c>.*Error: </c> are classified as Error.</description></item>
    ///         <item><description>Lines matching <c>.*Failure: </c> are classified as Error.</description></item>
    ///     </list>
    /// </remarks>
    public static RunProcessor TestProcessor { get; } = new(
        [
            RunLineRule.Create(RunLineType.Info, ".*Note: "),
            RunLineRule.Create(RunLineType.Warning, ".*Warning: "),
            RunLineRule.Create(RunLineType.Error, ".*Error: "),
            RunLineRule.Create(RunLineType.Error, ".*Failure: ")
        ]
    );

    /// <summary>
    ///     The singleton <see cref="QuestaSimSimulator"/> instance, initialized at class load time.
    /// </summary>
    /// <remarks>
    ///     Singleton pattern: only one instance is created per process, held in this static
    ///     property. The instance is stateless after construction (path is resolved once in the
    ///     constructor via <see cref="FindPath"/>); concurrent reads from multiple threads are safe.
    ///     Always access the simulator through this property rather than constructing a new instance.
    /// </remarks>
    public static QuestaSimSimulator Instance { get; } = new();

    /// <summary>
    ///     Initializes a new instance of the QuestaSim simulator
    /// </summary>
    private QuestaSimSimulator() : base("QuestaSim", FindPath())
    {
    }

    /// <inheritdoc />
    public override RunResults Compile(Context context, Options options)
    {
        // Log the start of the compile command
        context.WriteVerboseLine("Starting QuestaSim compile...");

        // Fail if we cannot find the simulator
        var simPath = SimulatorPath ??
                      throw new InvalidOperationException("QuestaSim Simulator not available");
        context.WriteVerboseLine($"  Simulator Path: {simPath}");

        // Create the library directory
        var libDir = Path.Combine(options.WorkingDirectory, "VHDLTest.out/QuestaSim");
        context.WriteVerboseLine($"  Library Directory: {libDir}");
        if (!Directory.Exists(libDir))
        {
            Directory.CreateDirectory(libDir);
        }

        // Build the batch file
        var writer = new StringBuilder();
        writer.AppendLine("onerror {exit -code 1}");
        writer.AppendLine("vlib work");
        writer.AppendLine("set worklib work");
        foreach (var file in options.Config.Files)
        {
            writer.AppendLine($"vcom -2008 ../../{file}");
        }

        writer.AppendLine("exit -code 0");

        // Write the batch file
        var script = Path.Combine(libDir, "compile.do");
        context.WriteVerboseLine($"  Script File: {script}");
        File.WriteAllText(script, writer.ToString());

        // Run the QuestaSim compiler
        var application = Path.Combine(simPath, "vsim");
        return CompileProcessor.Execute(
            context,
            application,
            libDir,
            "-c",
            "-do",
            "compile.do");
    }

    /// <inheritdoc />
    public override TestResult Test(Context context, Options options, string test)
    {
        // Log the start of the test command
        context.WriteVerboseLine($"Starting QuestaSim test {test}...");

        // Fail if we cannot find the simulator
        var simPath = SimulatorPath ??
                      throw new InvalidOperationException("QuestaSim Simulator not available");
        context.WriteVerboseLine($"  Simulator Path: {simPath}");

        // Get the library directory
        var libDir = Path.Combine(options.WorkingDirectory, "VHDLTest.out/QuestaSim");
        context.WriteVerboseLine($"  Library Directory: {libDir}");

        // Build the batch file
        var writer = new StringBuilder();
        writer.AppendLine("onerror {exit -code 1}");
        writer.AppendLine("set worklib work");
        writer.AppendLine($"vsim -quiet {test}");
        writer.AppendLine("run -all");
        writer.AppendLine("endsim");
        writer.AppendLine("exit -code 0");

        // Write the batch file
        var script = Path.Combine(libDir, "test.do");
        context.WriteVerboseLine($"  Script File: {script}");
        File.WriteAllText(script, writer.ToString());

        // Run the test
        var application = Path.Combine(simPath, "vsim");
        var testRunResults = TestProcessor.Execute(
            context,
            application,
            libDir,
            "-c",
            "-do",
            "test.do");

        // Return the test results
        return new TestResult(
            test,
            test,
            testRunResults);
    }

    /// <summary>
    ///     Searches for the QuestaSim installation directory.
    /// </summary>
    /// <returns>Directory path containing the QuestaSim executables, or null if QuestaSim is not found.</returns>
    /// <remarks>
    ///     Resolution order:
    ///     <list type="number">
    ///         <item><description>Returns the <c>VHDLTEST_QUESTASIM_PATH</c> environment variable value when set,
    ///         allowing CI environments and users to override the default installation path.</description></item>
    ///         <item><description>Searches the system PATH for the <c>vsim</c> executable and returns its
    ///         parent directory (the simulator installation directory).</description></item>
    ///     </list>
    ///     Returns null when QuestaSim is not found by either mechanism, causing
    ///     <see cref="Simulator.Available"/> to return false.
    /// </remarks>
    public static string? FindPath()
    {
        // Look for an environment variable
        var simPathEnv = Environment.GetEnvironmentVariable("VHDLTEST_QUESTASIM_PATH");
        if (simPathEnv != null)
        {
            return simPathEnv;
        }

        // Find the path to the simulator application
        var simPath = Where("vsim");
        if (simPath == null)
        {
            return null;
        }

        // Return the directory containing vsim (the simulator installation directory)
        return Path.GetDirectoryName(simPath);
    }
}
