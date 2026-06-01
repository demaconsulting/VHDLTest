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
///     Vivado simulator integration that drives xvhdl for VHDL-2008 source analysis and xelab
///     for elaboration and simulation, classifying output lines by severity using
///     <see cref="RunProcessor"/> rules.
/// </summary>
/// <remarks>
///     Uses argument-file (`.do`) scripts to pass options and file lists to the Vivado tools,
///     avoiding command-line length limits. Implemented as a singleton (<see cref="Instance"/>)
///     initialized at class load time; stateless after construction and therefore thread-safe.
///     When Vivado is not installed, <see cref="Simulator.SimulatorPath"/> is null and
///     <see cref="Simulator.Available"/> returns false.
/// </remarks>
public sealed class VivadoSimulator : Simulator
{
    /// <summary>
    ///     Relative path (from the working directory) to the Vivado library output directory
    ///     where compiled work libraries and do-scripts are stored.
    /// </summary>
    private const string OutputSubDirectory = "VHDLTest.out/Vivado";

    private static readonly RunLineRule[] CompileRules =
    [
        RunLineRule.Create(RunLineType.Error, "Error: ")
    ];

    private static readonly RunLineRule[] TestRules =
    [
        RunLineRule.Create(RunLineType.Info, "Note: "),
        RunLineRule.Create(RunLineType.Warning, "Warning: "),
        RunLineRule.Create(RunLineType.Error, "Error: "),
        RunLineRule.Create(RunLineType.Error, "Failure: ")
    ];

    /// <summary>
    ///     Output classifier for xvhdl compilation output.
    /// </summary>
    /// <remarks>
    ///     Classification rules:
    ///     <list type="bullet">
    ///         <item><description><c>Error: </c> — classified as Error.</description></item>
    ///     </list>
    ///     Unmatched lines are left as Text. The trailing space in each pattern prevents false
    ///     matches on identifiers that share a keyword prefix (for example, <c>ErrorDetails:</c>).
    /// </remarks>
    public static RunProcessor CompileProcessor { get; } = new(CompileRules);

    /// <summary>
    ///     Output classifier for xelab simulation output.
    /// </summary>
    /// <remarks>
    ///     Classification rules:
    ///     <list type="bullet">
    ///         <item><description><c>Note: </c> — classified as Info.</description></item>
    ///         <item><description><c>Warning: </c> — classified as Warning.</description></item>
    ///         <item><description><c>Error: </c> — classified as Error.</description></item>
    ///         <item><description><c>Failure: </c> — classified as Error.</description></item>
    ///     </list>
    ///     Unmatched lines are left as Text. The trailing space in each pattern prevents false
    ///     matches on identifiers that share a keyword prefix (for example, <c>ErrorDetails:</c>).
    /// </remarks>
    public static RunProcessor TestProcessor { get; } = new(TestRules);

    /// <summary>
    ///     Singleton <see cref="VivadoSimulator"/> instance shared across the application.
    /// </summary>
    /// <remarks>
    ///     The singleton is initialized once at class load time. <see cref="FindPath"/> is
    ///     called during construction; if Vivado is not found, <see cref="Simulator.SimulatorPath"/>
    ///     is null and <see cref="Simulator.Available"/> returns false.
    /// </remarks>
    public static VivadoSimulator Instance { get; } = new();

    private readonly RunProcessor _compileProcessor;
    private readonly RunProcessor _testProcessor;

    /// <summary>
    ///     Initializes a new instance of the Vivado simulator.
    /// </summary>
    /// <remarks>
    ///     Private to enforce the singleton pattern — callers must use <see cref="Instance"/>.
    ///     <see cref="FindPath"/> is invoked within the base-constructor call, initializing
    ///     <see cref="Simulator.SimulatorPath"/> to null when Vivado is not found.
    /// </remarks>
    private VivadoSimulator() : this(FindPath(), ProcessInvoker.Instance)
    {
    }

    private VivadoSimulator(string? simulatorPath, IProcessInvoker invoker)
        : base("Vivado", simulatorPath)
    {
        _compileProcessor = new RunProcessor(CompileRules, invoker);
        _testProcessor = new RunProcessor(TestRules, invoker);
    }

    /// <summary>
    ///     Creates a non-singleton instance for testing, using the supplied invoker instead of real process execution.
    /// </summary>
    /// <param name="simulatorPath">Path to use as the simulator installation directory.</param>
    /// <param name="invoker">Process invoker to use for all simulator invocations.</param>
    /// <returns>A new <see cref="VivadoSimulator"/> instance backed by <paramref name="invoker"/>.</returns>
    internal static VivadoSimulator CreateForTesting(string simulatorPath, IProcessInvoker invoker)
        => new(simulatorPath, invoker);

    /// <inheritdoc />
    public override RunResults Compile(Context context, Options options)
    {
        // Log the start of the compile command
        context.WriteVerboseLine("Starting Vivado compile...");

        // Fail if we cannot find the simulator
        var simPath = SimulatorPath ??
                      throw new InvalidOperationException("Vivado Simulator not available");
        context.WriteVerboseLine($"  Simulator Path: {simPath}");

        // Create the library directory
        var libDir = Path.Combine(options.WorkingDirectory, OutputSubDirectory);
        context.WriteVerboseLine($"  Library Directory: {libDir}");
        if (!Directory.Exists(libDir))
        {
            Directory.CreateDirectory(libDir);
        }

        // Build the batch file
        var writer = new StringBuilder();
        writer.AppendLine("-2008");
        writer.AppendLine("-nolog");
        writer.AppendLine("-work work");
        foreach (var file in options.Config.Files)
        {
            writer.AppendLine($"../../{file}");
        }

        // Write the batch file
        var script = Path.Combine(libDir, "compile.do");
        context.WriteVerboseLine($"  Script File: {script}");
        File.WriteAllText(script, writer.ToString());

        // Run the Vivado compiler; RunProcessor handles platform-specific execution transparently
        var application = Path.Combine(simPath, "xvhdl");
        return _compileProcessor.Execute(context, application, libDir, "-file", "compile.do");
    }

    /// <inheritdoc />
    public override TestResult Test(Context context, Options options, string test)
    {
        // Log the start of the test command
        context.WriteVerboseLine($"Starting Vivado test {test}...");

        // Fail if we cannot find the simulator
        var simPath = SimulatorPath ??
                      throw new InvalidOperationException("Vivado Simulator not available");
        context.WriteVerboseLine($"  Simulator Path: {simPath}");

        // Get the library directory
        var libDir = Path.Combine(options.WorkingDirectory, OutputSubDirectory);
        context.WriteVerboseLine($"  Library Directory: {libDir}");

        // Build the batch file
        var writer = new StringBuilder();
        writer.AppendLine("-nolog");
        writer.AppendLine("-standalone");
        writer.AppendLine("-runall");
        writer.AppendLine(test);

        // Write the batch file
        var script = Path.Combine(libDir, "test.do");
        context.WriteVerboseLine($"  Script File: {script}");
        File.WriteAllText(script, writer.ToString());

        // Run the Vivado test; RunProcessor handles platform-specific execution transparently
        var application = Path.Combine(simPath, "xelab");
        var testRunResults = _testProcessor.Execute(context, application, libDir, "-file", "test.do");

        // Return the test results
        return new TestResult(
            test,
            test,
            testRunResults);
    }

    /// <summary>
    ///     Searches for the Vivado installation directory.
    /// </summary>
    /// <returns>Directory path containing the Vivado executables, or null if Vivado is not found.</returns>
    /// <remarks>
    ///     Resolution order:
    ///     <list type="number">
    ///         <item><description>Returns the <c>VHDLTEST_VIVADO_PATH</c> environment variable value when set.</description></item>
    ///         <item><description>Searches the system PATH for the <c>vivado</c> executable and returns its parent directory.</description></item>
    ///     </list>
    /// </remarks>
    public static string? FindPath()
    {
        // Look for an environment variable
        var simPathEnv = Environment.GetEnvironmentVariable("VHDLTEST_VIVADO_PATH");
        if (simPathEnv != null)
        {
            return simPathEnv;
        }

        // Find the path to the simulator application
        var simPath = Where("vivado");
        if (simPath == null)
        {
            return null;
        }

        // Return the Vivado installation directory (parent of the vivado executable)
        return Path.GetDirectoryName(simPath);
    }
}
