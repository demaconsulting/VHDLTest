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
///     Active-HDL Simulator Class
/// </summary>
public sealed class ActiveHdlSimulator : Simulator
{
    /// <summary>
    /// Simulation application
    /// </summary>
    private const string SimApp = "vsimsa";

    /// <summary>
    /// Compile processor
    /// </summary>
    public static RunProcessor CompileProcessor { get; } = new(
        [
            RunLineRule.Create(RunLineType.Warning, @"KERNEL:\s*Warning:"),
            RunLineRule.Create(RunLineType.Error, "Error:"),
            RunLineRule.Create(RunLineType.Error, @"RUNTIME:\s*Fatal Error")
        ]
    );

    /// <summary>
    /// Test processor
    /// </summary>
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
    ///     Active-HDL simulator instance
    /// </summary>
    public static ActiveHdlSimulator Instance { get; } = new();

    /// <summary>
    ///     Initializes a new instance of the ActiveHdl simulator
    /// </summary>
    private ActiveHdlSimulator() : base("ActiveHdl", FindPath())
    {
    }

    /// <inheritdoc />
    public override RunResults Compile(Context context, Options options)
    {
        // Log the start of the compile command
        context.WriteVerboseLine("Starting ActiveHdl compile...");

        // Fail if we cannot find the simulator
        var simPath = SimulatorPath ??
                      throw new InvalidOperationException("ActiveHdl Simulator not available");
        context.WriteVerboseLine($"  Simulator Path: {simPath}");

        // Create the library directory
        var libDir = Path.Combine(options.WorkingDirectory, "VHDLTest.out/ActiveHdl");
        context.WriteVerboseLine($"  Library Directory: {libDir}");
        if (!Directory.Exists(libDir))
            Directory.CreateDirectory(libDir);

        // Build the batch file
        var writer = new StringBuilder();
        writer.AppendLine("onerror {exit -code 1}");
        writer.AppendLine("alib work VHDLTest.out/ActiveHDL");
        writer.AppendLine("set worklib work");
        foreach (var file in options.Config.Files)
            writer.AppendLine($"acom -2008 -dbg {file}");

        // Write the batch file
        var script = Path.Combine(libDir, "compile.do");
        context.WriteVerboseLine($"  Script File: {script}");
        File.WriteAllText(script, writer.ToString());

        // Run the ActiveHDL compiler
        var application = Path.Combine(simPath, SimApp);
        context.WriteVerboseLine($"  Run Directory: {options.WorkingDirectory}");
        context.WriteVerboseLine($"  Run Command: {application} -do VHDLTest.out/ActiveHDL/compile.do");
        return CompileProcessor.Execute(
            application,
            options.WorkingDirectory,
            "-do",
            "VHDLTest.out/ActiveHDL/compile.do");
    }

    /// <inheritdoc />
    public override TestResult Test(Context context, Options options, string test)
    {
        // Log the start of the compile command
        context.WriteVerboseLine($"Starting ActiveHDL test {test}...");

        // Fail if we cannot find the simulator
        var simPath = SimulatorPath ??
                      throw new InvalidOperationException("ActiveHDL Simulator not available");
        context.WriteVerboseLine($"  Simulator Path: {simPath}");

        // Get the library directory
        var libDir = Path.Combine(options.WorkingDirectory, "VHDLTest.out/ActiveHDL");
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
        context.WriteVerboseLine($"  Run Directory: {options.WorkingDirectory}");
        context.WriteVerboseLine($"  Run Command: {application} -do VHDLTest.out/ActiveHDL/test.do");
        var testRunResults = TestProcessor.Execute(
            Path.Combine(simPath, SimApp),
            options.WorkingDirectory,
            "-do",
            "VHDLTest.out/ActiveHDL/test.do");

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
        var simPathEnv = Environment.GetEnvironmentVariable("VHDLTEST_ACTIVEHDL_PATH");
        if (simPathEnv != null)
            return simPathEnv;

        // Find the path to the simulator application
        var simPath = Where(SimApp);
        if (simPath == null)
            return null;

        // Return the working directory
        return Path.GetDirectoryName(simPath);
    }
}