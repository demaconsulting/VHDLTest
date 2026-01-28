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
using DEMAConsulting.VHDLTest.Results;
using DEMAConsulting.VHDLTest.Run;

namespace DEMAConsulting.VHDLTest.Simulators;

/// <summary>
///     ModelSim Simulator Class
/// </summary>
public sealed class ModelSimSimulator : Simulator
{
    /// <summary>
    /// Compile processor
    /// </summary>
    public static RunProcessor CompileProcessor { get; } = new(
        [
            RunLineRule.Create(RunLineType.Error, ".*Error: ")
        ]
    );

    /// <summary>
    /// Test processor
    /// </summary>
    public static RunProcessor TestProcessor { get; } = new(
        [
            RunLineRule.Create(RunLineType.Info, ".*Note: "),
            RunLineRule.Create(RunLineType.Warning, ".*Warning: "),
            RunLineRule.Create(RunLineType.Error, ".*Error: "),
            RunLineRule.Create(RunLineType.Error, ".*Failure: ")
        ]
    );

    /// <summary>
    ///     ModelSim simulator instance
    /// </summary>
    public static ModelSimSimulator Instance { get; } = new();

    /// <summary>
    ///     Initializes a new instance of the ModelSim simulator
    /// </summary>
    private ModelSimSimulator() : base("ModelSim", FindPath())
    {
    }

    /// <inheritdoc />
    public override RunResults Compile(Context context, Options options)
    {
        // Log the start of the compile command
        context.WriteVerboseLine("Starting ModelSim compile...");

        // Fail if we cannot find the simulator
        var simPath = SimulatorPath ??
                      throw new InvalidOperationException("ModelSim Simulator not available");
        context.WriteVerboseLine($"  Simulator Path: {simPath}");

        // Create the library directory
        var libDir = Path.Combine(options.WorkingDirectory, "VHDLTest.out/ModelSim");
        context.WriteVerboseLine($"  Library Directory: {libDir}");
        if (!Directory.Exists(libDir))
            Directory.CreateDirectory(libDir);

        // Build the batch file
        var writer = new StringBuilder();
        writer.AppendLine("onerror {exit -code 1}");
        writer.AppendLine("vlib work");
        writer.AppendLine("set worklib work");
        foreach (var file in options.Config.Files)
            writer.AppendLine($"vcom -2008 ../../{file}");
        writer.AppendLine("exit -code 0");

        // Write the batch file
        var script = Path.Combine(libDir, "compile.do");
        context.WriteVerboseLine($"  Script File: {script}");
        File.WriteAllText(script, writer.ToString());

        // Run the ModelSim compiler
        var application = Path.Combine(simPath, "vsim");
        context.WriteVerboseLine($"  Run Directory: {libDir}");
        context.WriteVerboseLine($"  Run Command: {application} -c -do compile.do");
        return CompileProcessor.Execute(
            application,
            libDir,
            "-c",
            "-do",
            "compile.do");
    }

    /// <inheritdoc />
    public override TestResult Test(Context context, Options options, string test)
    {
        // Log the start of the compile command
        context.WriteVerboseLine($"Starting ModelSim test {test}...");

        // Fail if we cannot find the simulator
        var simPath = SimulatorPath ??
                      throw new InvalidOperationException("ModelSim Simulator not available");
        context.WriteVerboseLine($"  Simulator Path: {simPath}");

        // Get the library directory
        var libDir = Path.Combine(options.WorkingDirectory, "VHDLTest.out/ModelSim");
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
        context.WriteVerboseLine($"  Run Directory: {libDir}");
        context.WriteVerboseLine($"  Run Command: {application} -c -do test.do");
        var testRunResults = TestProcessor.Execute(
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
    ///     Find the simulator path
    /// </summary>
    /// <returns>Simulator path or null if not found</returns>
    public static string? FindPath()
    {
        // Look for an environment variable
        var simPathEnv = Environment.GetEnvironmentVariable("VHDLTEST_MODELSIM_PATH");
        if (simPathEnv != null)
            return simPathEnv;

        // Find the path to the simulator application
        var simPath = Where("vsim");
        if (simPath == null)
            return null;

        // Return the working directory
        return Path.GetDirectoryName(simPath);
    }
}
