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
///     Active-HDL simulator integration that drives <c>vsimsa</c> in batch mode via TCL do-scripts,
///     using <c>acom</c> for VHDL-2008 compilation and <c>asim</c>/<c>run</c> for test-bench
///     execution. Classifies output lines by severity and suppresses Lattice Edition license
///     advisory messages.
/// </summary>
/// <remarks>
///     All public members are thread-safe for concurrent read access; the singleton
///     <see cref="Instance"/> is initialized once at class load time.
/// </remarks>
public sealed class ActiveHdlSimulator : Simulator
{
    /// <summary>
    ///     Name of the Active-HDL batch-mode simulator executable used to drive compilation and test execution.
    /// </summary>
    private const string SimApp = "vsimsa";

    /// <summary>
    ///     Relative path (from the working directory) to the Active-HDL library output directory where
    ///     compiled work libraries and do-scripts are stored.
    /// </summary>
    private const string LibDirPath = "VHDLTest.out/ActiveHDL";

    /// <summary>
    ///     Output classifier for <c>vsimsa</c> compilation output.
    /// </summary>
    /// <remarks>
    ///     Classification rules:
    ///     <list type="bullet">
    ///         <item><description><c>KERNEL:\s*Warning:</c> — classified as Warning.</description></item>
    ///         <item><description><c>Error:</c> — classified as Error.</description></item>
    ///         <item><description><c>RUNTIME:\s*Fatal Error</c> — classified as Error.</description></item>
    ///     </list>
    ///     Unmatched lines are left as Text.
    /// </remarks>
    public static RunProcessor CompileProcessor { get; } = new(
        [
            RunLineRule.Create(RunLineType.Warning, @"KERNEL:\s*Warning:"),
            RunLineRule.Create(RunLineType.Error, "Error:"),
            RunLineRule.Create(RunLineType.Error, @"RUNTIME:\s*Fatal Error")
        ]
    );

    /// <summary>
    ///     Output classifier for <c>vsimsa</c> simulation output.
    /// </summary>
    /// <remarks>
    ///     Classification rules (applied in order):
    ///     <list type="bullet">
    ///         <item><description><c>KERNEL:\s*Warning:\s*You are using the Active-HDL Lattice Edition</c> — suppressed (classified as Text).</description></item>
    ///         <item><description><c>KERNEL:\s*Warning:\s*Contact Aldec for available upgrade options</c> — suppressed (classified as Text).</description></item>
    ///         <item><description><c>KERNEL:\s*Warning:</c> — classified as Warning.</description></item>
    ///         <item><description><c>KERNEL:\s*WARNING:</c> — classified as Warning.</description></item>
    ///         <item><description><c>EXECUTION::\s*NOTE</c> — classified as Info.</description></item>
    ///         <item><description><c>EXECUTION::\s*WARNING</c> — classified as Warning.</description></item>
    ///         <item><description><c>EXECUTION::\s*ERROR</c> — classified as Error.</description></item>
    ///         <item><description><c>EXECUTION::\s*FAILURE</c> — classified as Error.</description></item>
    ///         <item><description><c>KERNEL:\s*ERROR</c> — classified as Error.</description></item>
    ///         <item><description><c>RUNTIME:\s*Fatal Error:</c> — classified as Error.</description></item>
    ///         <item><description><c>VSIM:\s*Error:</c> — classified as Error.</description></item>
    ///     </list>
    ///     The two Lattice Edition suppression rules must appear before the general
    ///     <c>KERNEL:\s*Warning:</c> rule so they take priority.
    /// </remarks>
    public static RunProcessor TestProcessor { get; } = new(
        [
            RunLineRule.Create(RunLineType.Text, @"KERNEL:\s*Warning:\s*You are using the Active-HDL Lattice Edition"),
            RunLineRule.Create(RunLineType.Text, @"KERNEL:\s*Warning:\s*Contact Aldec for available upgrade options"),
            RunLineRule.Create(RunLineType.Warning, @"KERNEL:\s*Warning:"),
            RunLineRule.Create(RunLineType.Warning, @"KERNEL:\s*WARNING:"),
            RunLineRule.Create(RunLineType.Info, @"EXECUTION::\s*NOTE"),
            RunLineRule.Create(RunLineType.Warning, @"EXECUTION::\s*WARNING"),
            RunLineRule.Create(RunLineType.Error, @"EXECUTION::\s*ERROR"),
            RunLineRule.Create(RunLineType.Error, @"EXECUTION::\s*FAILURE"),
            RunLineRule.Create(RunLineType.Error, @"KERNEL:\s*ERROR"),
            RunLineRule.Create(RunLineType.Error, @"RUNTIME:\s*Fatal Error:"),
            RunLineRule.Create(RunLineType.Error, @"VSIM:\s*Error:")
        ]
    );

    /// <summary>
    ///     Singleton <see cref="ActiveHdlSimulator"/> instance shared across the application.
    /// </summary>
    /// <remarks>
    ///     Initialized once at class load time. If Active-HDL is not installed,
    ///     <see cref="Simulator.SimulatorPath"/> is null and <see cref="Simulator.Available"/>
    ///     returns false.
    /// </remarks>
    public static ActiveHdlSimulator Instance { get; } = new();

    /// <summary>
    ///     Initializes a new instance of the Active-HDL simulator.
    /// </summary>
    /// <remarks>
    ///     Private to enforce the singleton pattern — callers must use <see cref="Instance"/>.
    ///     Passes null for the path when Active-HDL is not installed, causing
    ///     <see cref="Simulator.Available"/> to return false and preventing accidental use
    ///     in environments without the simulator.
    /// </remarks>
    private ActiveHdlSimulator() : base("ActiveHdl", FindPath())
    {
    }

    /// <summary>
    ///     Compiles all VHDL source files using Active-HDL's <c>acom</c> utility via a TCL do-script.
    /// </summary>
    /// <remarks>
    ///     Builds a TCL do-script containing <c>onerror {exit -code 1}</c>, an <c>alib</c> library
    ///     initialisation command, <c>set worklib work</c>, and one <c>acom -2008 -dbg {file}</c>
    ///     line per source file. The script is written to
    ///     <c>VHDLTest.out/ActiveHDL/compile.do</c> and executed via
    ///     <c>vsimsa -do VHDLTest.out/ActiveHDL/compile.do</c> from the working directory.
    /// </remarks>
    /// <param name="context">Execution context used for verbose logging. Must not be null.</param>
    /// <param name="options">Parsed options providing the VHDL file list and working directory. Must not be null.</param>
    /// <returns>Classified compile output as <see cref="RunResults"/>.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when <see cref="Simulator.SimulatorPath"/> is null (Active-HDL not installed).
    /// </exception>
    public override RunResults Compile(Context context, Options options)
    {
        // Log the start of the compile command
        context.WriteVerboseLine("Starting ActiveHdl compile...");

        // Fail if we cannot find the simulator
        var simPath = SimulatorPath ??
                      throw new InvalidOperationException("ActiveHDL Simulator not available");
        context.WriteVerboseLine($"  Simulator Path: {simPath}");

        // Create the library directory
        var libDir = Path.Combine(options.WorkingDirectory, LibDirPath);
        context.WriteVerboseLine($"  Library Directory: {libDir}");
        if (!Directory.Exists(libDir))
        {
            Directory.CreateDirectory(libDir);
        }

        // Build the batch file
        var writer = new StringBuilder();
        writer.AppendLine("onerror {exit -code 1}");
        writer.AppendLine($"alib work {LibDirPath}");
        writer.AppendLine("set worklib work");
        foreach (var file in options.Config.Files)
        {
            writer.AppendLine($"acom -2008 -dbg {file}");
        }

        // Write the batch file
        var script = Path.Combine(libDir, "compile.do");
        context.WriteVerboseLine($"  Script File: {script}");
        File.WriteAllText(script, writer.ToString());

        // Run the ActiveHDL compiler
        var application = Path.Combine(simPath, SimApp);
        return CompileProcessor.Execute(
            context,
            application,
            options.WorkingDirectory,
            "-do",
            $"{LibDirPath}/compile.do");
    }

    /// <summary>
    ///     Simulates a single test bench using Active-HDL's <c>asim</c> utility via a TCL do-script.
    /// </summary>
    /// <remarks>
    ///     Builds a TCL do-script containing <c>onerror {exit -code 1}</c>, <c>set worklib work</c>,
    ///     <c>asim {test}</c>, <c>run -all</c>, <c>endsim</c>, and <c>exit -code 0</c>. The script
    ///     is written to <c>VHDLTest.out/ActiveHDL/test.do</c> and executed via
    ///     <c>vsimsa -do VHDLTest.out/ActiveHDL/test.do</c> from the working directory. The
    ///     trailing <c>exit -code 0</c> is required because <c>vsimsa</c> does not emit a success
    ///     exit code automatically when simulation completes normally.
    /// </remarks>
    /// <param name="context">Execution context used for verbose logging. Must not be null.</param>
    /// <param name="options">Parsed options providing the working directory. Must not be null.</param>
    /// <param name="test">Test bench entity name to simulate. Must not be null or empty.</param>
    /// <returns>Simulation outcome as a <see cref="TestResult"/>.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when <see cref="Simulator.SimulatorPath"/> is null (Active-HDL not installed).
    /// </exception>
    public override TestResult Test(Context context, Options options, string test)
    {
        // Log the start of the test command
        context.WriteVerboseLine($"Starting ActiveHDL test {test}...");

        // Fail if we cannot find the simulator
        var simPath = SimulatorPath ??
                      throw new InvalidOperationException("ActiveHDL Simulator not available");
        context.WriteVerboseLine($"  Simulator Path: {simPath}");

        // Get the library directory
        var libDir = Path.Combine(options.WorkingDirectory, LibDirPath);
        context.WriteVerboseLine($"  Library Directory: {libDir}");

        // Build the batch file
        var writer = new StringBuilder();
        writer.AppendLine("onerror {exit -code 1}");
        writer.AppendLine("set worklib work");
        writer.AppendLine($"asim {test}");
        writer.AppendLine("run -all");
        writer.AppendLine("endsim");
        writer.AppendLine("exit -code 0");

        // Write the batch file
        var script = Path.Combine(libDir, "test.do");
        context.WriteVerboseLine($"  Script File: {script}");
        File.WriteAllText(script, writer.ToString());

        // Run the test
        var application = Path.Combine(simPath, SimApp);
        var testRunResults = TestProcessor.Execute(
            context,
            application,
            options.WorkingDirectory,
            "-do",
            $"{LibDirPath}/test.do");

        // Return the test results
        return new TestResult(
            test,
            test,
            testRunResults);
    }

    /// <summary>
    ///     Searches for the Active-HDL installation directory.
    /// </summary>
    /// <returns>Directory path containing <c>vsimsa</c>, or null if Active-HDL is not found.</returns>
    /// <remarks>
    ///     Resolution order:
    ///     <list type="number">
    ///         <item><description>Returns the <c>VHDLTEST_ACTIVEHDL_PATH</c> environment variable value when set, allowing CI environments to override the default installation path.</description></item>
    ///         <item><description>Searches the system PATH for <c>vsimsa</c> and returns its parent directory.</description></item>
    ///     </list>
    /// </remarks>
    public static string? FindPath()
    {
        // Look for an environment variable
        var simPathEnv = Environment.GetEnvironmentVariable("VHDLTEST_ACTIVEHDL_PATH");
        if (simPathEnv != null)
        {
            return simPathEnv;
        }

        // Find the path to the simulator application
        var simPath = Where(SimApp);
        if (simPath == null)
        {
            return null;
        }

        // Return the simulator installation directory (parent of the vsimsa executable)
        return Path.GetDirectoryName(simPath);
    }
}
