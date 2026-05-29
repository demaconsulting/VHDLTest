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
///     Concrete <see cref="Simulator"/> implementation for the NVC open-source VHDL simulator.
/// </summary>
/// <remarks>
///     Drives the <c>nvc</c> command-line tool using a combined elaboration-and-simulation
///     design: the <c>Test</c> method passes both <c>-e {test}</c> (elaboration) and
///     <c>-r {test}</c> (simulation) as arguments in a single <c>nvc</c> invocation,
///     so NVC handles elaboration and execution in one step. Implemented as a singleton
///     (<see cref="Instance"/>) initialized at class load time; stateless after construction
///     and therefore thread-safe.
/// </remarks>
public sealed class NvcSimulator : Simulator
{
    /// <summary>
    ///     Output classifier for NVC compilation (<c>nvc -a</c>) output.
    /// </summary>
    /// <remarks>
    ///     Applies five classification rules in order:
    ///     <list type="bullet">
    ///         <item><description>Lines matching <c>.* Note:</c> are classified as Info.</description></item>
    ///         <item><description>Lines matching <c>.* Warning:</c> are classified as Warning.</description></item>
    ///         <item><description>Lines matching <c>.* Error:</c> are classified as Error.</description></item>
    ///         <item><description>Lines matching <c>.* Failure:</c> are classified as Error.</description></item>
    ///         <item><description>Lines matching <c>.* Fatal:</c> are classified as Error.</description></item>
    ///     </list>
    /// </remarks>
    public static RunProcessor CompileProcessor { get; } = new(
        [
            RunLineRule.Create(RunLineType.Info, ".* Note:"),
            RunLineRule.Create(RunLineType.Warning, ".* Warning:"),
            RunLineRule.Create(RunLineType.Error, ".* Error:"),
            RunLineRule.Create(RunLineType.Error, ".* Failure:"),
            RunLineRule.Create(RunLineType.Error, ".* Fatal:")
        ]
    );

    /// <summary>
    ///     Output classifier for NVC simulation (<c>nvc -r</c>) output.
    /// </summary>
    /// <remarks>
    ///     Applies the same five classification rules as <see cref="CompileProcessor"/>:
    ///     <list type="bullet">
    ///         <item><description>Lines matching <c>.* Note:</c> are classified as Info.</description></item>
    ///         <item><description>Lines matching <c>.* Warning:</c> are classified as Warning.</description></item>
    ///         <item><description>Lines matching <c>.* Error:</c> are classified as Error.</description></item>
    ///         <item><description>Lines matching <c>.* Failure:</c> are classified as Error.</description></item>
    ///         <item><description>Lines matching <c>.* Fatal:</c> are classified as Error.</description></item>
    ///     </list>
    /// </remarks>
    public static RunProcessor TestProcessor { get; } = new(
        [
            RunLineRule.Create(RunLineType.Info, ".* Note:"),
            RunLineRule.Create(RunLineType.Warning, ".* Warning:"),
            RunLineRule.Create(RunLineType.Error, ".* Error:"),
            RunLineRule.Create(RunLineType.Error, ".* Failure:"),
            RunLineRule.Create(RunLineType.Error, ".* Fatal:")
        ]
    );

    /// <summary>
    ///     Gets the singleton <see cref="NvcSimulator"/> instance shared across the application.
    /// </summary>
    /// <remarks>
    ///     Initialized once at class-load time by calling <see cref="FindPath()"/> to resolve the
    ///     NVC installation directory. Stateless after construction and therefore thread-safe.
    ///     Always access the simulator through this property rather than constructing a new instance.
    /// </remarks>
    public static NvcSimulator Instance { get; } = new();

    /// <summary>
    ///     Private constructor that prevents external instantiation and enforces use of the
    ///     singleton <see cref="Instance"/>. Passes the fixed name <c>"NVC"</c> and the path
    ///     resolved by <see cref="FindPath()"/> to the base class.
    /// </summary>
    private NvcSimulator() : base("NVC", FindPath())
    {
    }

    /// <inheritdoc />
    public override RunResults Compile(Context context, Options options)
    {
        // Log the start of the compile command
        context.WriteVerboseLine("Starting NVC compile...");

        // Fail if we cannot find the simulator
        var simPath = SimulatorPath ??
                      throw new InvalidOperationException("NVC Simulator not available");
        context.WriteVerboseLine($"  Simulator Path: {simPath}");

        // Create the library directory
        var libDir = Path.Combine(options.WorkingDirectory, "VHDLTest.out/NVC");
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

        // Run the NVC compiler
        var application = Path.Combine(simPath, "nvc");
        return CompileProcessor.Execute(
            context,
            application,
            options.WorkingDirectory,
            "--std=2008",
            "--work=work:VHDLTest.out/NVC/lib",
            "-a",
            "@VHDLTest.out/NVC/compile.rsp");
    }

    /// <inheritdoc />
    public override TestResult Test(Context context, Options options, string test)
    {
        // Log the start of the test command
        context.WriteVerboseLine($"Starting NVC test {test}...");

        // Fail if we cannot find the simulator
        var simPath = SimulatorPath ??
                      throw new InvalidOperationException("NVC Simulator not available");
        context.WriteVerboseLine($"  Simulator Path: {simPath}");

        // Get the library directory
        var libDir = Path.Combine(options.WorkingDirectory, "VHDLTest.out/NVC");
        context.WriteVerboseLine($"  Library Directory: {libDir}");

        // Run the test
        var application = Path.Combine(simPath, "nvc");
        var testRunResults = TestProcessor.Execute(
            context,
            application,
            options.WorkingDirectory,
            "--std=2008",
            "--work=work:VHDLTest.out/NVC/lib",
            "-e",
            test,
            "-r",
            test);

        // Return the test results
        return new TestResult(
            test,
            test,
            testRunResults);
    }

    /// <summary>
    ///     Searches for the NVC installation directory by locating the <c>nvc</c> executable
    ///     on the system PATH, returning null when NVC is not installed.
    /// </summary>
    /// <returns>
    ///     The directory containing the <c>nvc</c> executable, or null if NVC is not found.
    ///     The <c>VHDLTEST_NVC_PATH</c> environment variable, when set, overrides PATH search
    ///     and returns its value directly as the installation directory path.
    /// </returns>
    public static string? FindPath()
    {
        // Look for an environment variable
        var simPathEnv = Environment.GetEnvironmentVariable("VHDLTEST_NVC_PATH");
        if (simPathEnv != null)
        {
            return simPathEnv;
        }

        // Find the path to the simulator application
        var simPath = Where("nvc");
        if (simPath == null)
        {
            return null;
        }

        // Return the working directory
        return Path.GetDirectoryName(simPath);
    }
}
