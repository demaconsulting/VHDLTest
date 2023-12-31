﻿using System.Text;
using System.Text.RegularExpressions;
using DEMAConsulting.VHDLTest.Results;
using DEMAConsulting.VHDLTest.Run;

namespace DEMAConsulting.VHDLTest.Simulators;

/// <summary>
///     Active-HDL Simulator Class
/// </summary>
public sealed class ActiveHdlSimulator : Simulator
{
    /// <summary>
    ///     Text match rules when compiling
    /// </summary>
    private static readonly RunLineRule[] CompileRules =
    {
        new(RunLineType.Warning, new Regex(@"KERNEL:\s*Warning:")),
        new(RunLineType.Error, new Regex("Error:")),
        new(RunLineType.Error, new Regex(@"RUNTIME:\s*Fatal Error"))
    };

    /// <summary>
    ///     Text match rules when running a test
    /// </summary>
    private static readonly RunLineRule[] TestRules =
    {
        new(RunLineType.Text, new Regex(@"KERNEL:\s*Warning:\s*You are using the Active-HDL Lattice Edition")),
        new(RunLineType.Text, new Regex(@"KERNEL:\s*Warning:\s*Contact Aldec for available upgrade options")),
        new(RunLineType.Warning, new Regex(@"KERNEL:\s*Warning:")),
        new(RunLineType.Warning, new Regex(@"KERNEL:\s*WARNING:")),
        new(RunLineType.Info, new Regex(@"EXECUTION::\s*NOTE")),
        new(RunLineType.Warning, new Regex(@"EXECUTION::\s*WARNING")),
        new(RunLineType.Error, new Regex(@"EXECUTION::\s*ERROR")),
        new(RunLineType.Error, new Regex(@"EXECUTION::\s*FAILURE")),
        new(RunLineType.Error, new Regex(@"KERNEL:\s*ERROR")),
        new(RunLineType.Error, new Regex(@"RUNTIME:\s*Fatal Error:")),
        new(RunLineType.Error, new Regex(@"VSIM:\s*Error:"))
    };

    /// <summary>
    ///     Active-HDL simulator instance
    /// </summary>
    public static readonly ActiveHdlSimulator Instance = new();

    /// <summary>
    ///     Initializes a new instance of the ActiveHdl simulator
    /// </summary>
    private ActiveHdlSimulator() : base("ActiveHdl", FindPath())
    {
    }

    /// <inheritdoc />
    public override RunResults Compile(Options options)
    {
        // Log the start of the compile command
        if (options.Verbose)
            Console.WriteLine("Starting ActiveHdl compile...");

        // Fail if we cannot find the simulator
        var simPath = SimulatorPath ??
                      throw new InvalidOperationException("ActiveHdl Simulator not available");
        if (options.Verbose)
            Console.WriteLine($"  Simulator Path: {simPath}");

        // Create the library directory
        var libDir = Path.Combine(options.WorkingDirectory, "VHDLTest.out/ActiveHdl");
        if (options.Verbose)
            Console.WriteLine($"  Library Directory: {libDir}");
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
        if (options.Verbose)
            Console.WriteLine($"  Script File: {script}");
        File.WriteAllText(script, writer.ToString());

        // Run the ActiveHDL compiler
        var application = Path.Combine(simPath, "vsimsa");
        if (options.Verbose)
            Console.WriteLine($"  Run Directory: {options.WorkingDirectory}");
        if (options.Verbose)
            Console.WriteLine($"  Run Command: {application} -do VHDLTest.out/ActiveHDL/compile.do");
        return RunResults.Execute(
            CompileRules,
            application,
            options.WorkingDirectory,
            "-do",
            "VHDLTest.out/ActiveHDL/compile.do");
    }

    /// <inheritdoc />
    public override TestResult Test(Options options, string test)
    {
        // Log the start of the compile command
        if (options.Verbose)
            Console.WriteLine($"Starting ActiveHDL test {test}...");

        // Fail if we cannot find the simulator
        var simPath = SimulatorPath ??
                      throw new InvalidOperationException("ActiveHDL Simulator not available");
        if (options.Verbose)
            Console.WriteLine($"  Simulator Path: {simPath}");

        // Get the library directory
        var libDir = Path.Combine(options.WorkingDirectory, "VHDLTest.out/ActiveHDL");
        if (options.Verbose)
            Console.WriteLine($"  Library Directory: {libDir}");

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
        if (options.Verbose)
            Console.WriteLine($"  Script File: {script}");
        File.WriteAllText(script, writer.ToString());

        // Run the test
        var application = Path.Combine(simPath, "vsimsa");
        if (options.Verbose)
            Console.WriteLine($"  Run Directory: {options.WorkingDirectory}");
        if (options.Verbose)
            Console.WriteLine($"  Run Command: {application} -do VHDLTest.out/ActiveHDL/test.do");
        var testRunResults = RunResults.Execute(
            TestRules,
            Path.Combine(simPath, "vsimsa"),
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
        var simPath = Where("vsimsa");
        if (simPath == null)
            return null;

        // Return the working directory
        return Path.GetDirectoryName(simPath);
    }
}