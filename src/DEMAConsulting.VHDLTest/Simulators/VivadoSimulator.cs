﻿using System.Text;
using System.Text.RegularExpressions;
using DEMAConsulting.VHDLTest.Results;
using DEMAConsulting.VHDLTest.Run;

namespace DEMAConsulting.VHDLTest.Simulators;

/// <summary>
///     Vivado Simulator Class
/// </summary>
public sealed class VivadoSimulator : Simulator
{
    /// <summary>
    ///     Text match rules when compiling
    /// </summary>
    private static readonly RunLineRule[] CompileRules =
    {
        new(RunLineType.Error, new Regex("Error: "))
    };

    /// <summary>
    ///     Text match rules when running a test
    /// </summary>
    private static readonly RunLineRule[] TestRules =
    {
        new(RunLineType.Info, new Regex("Note: ")),
        new(RunLineType.Warning, new Regex("Warning: ")),
        new(RunLineType.Error, new Regex("Error: ")),
        new(RunLineType.Error, new Regex("Failure: "))
    };

    /// <summary>
    ///     Vivado simulator instance
    /// </summary>
    public static readonly VivadoSimulator Instance = new();

    /// <summary>
    ///     Initializes a new instance of the Vivado simulator
    /// </summary>
    private VivadoSimulator() : base("Vivado", FindPath())
    {
    }

    /// <inheritdoc />
    public override RunResults Compile(Options options)
    {
        // Log the start of the compile command
        if (options.Verbose)
            Console.WriteLine("Starting Vivado compile...");

        // Fail if we cannot find the simulator
        var simPath = SimulatorPath ??
                      throw new InvalidOperationException("Vivado Simulator not available");
        if (options.Verbose)
            Console.WriteLine($"  Simulator Path: {simPath}");

        // Create the library directory
        var libDir = Path.Combine(options.WorkingDirectory, "VHDLTest.out/Vivado");
        if (options.Verbose)
            Console.WriteLine($"  Library Directory: {libDir}");
        if (!Directory.Exists(libDir))
            Directory.CreateDirectory(libDir);

        // Build the batch file
        var writer = new StringBuilder();
        writer.AppendLine("-2008");
        writer.AppendLine("-nolog");
        writer.AppendLine("-work work");
        foreach (var file in options.Config.Files)
            writer.AppendLine($"../../{file}");

        // Write the batch file
        var script = Path.Combine(libDir, "compile.do");
        if (options.Verbose)
            Console.WriteLine($"  Script File: {script}");
        File.WriteAllText(script, writer.ToString());

        // Run the ModelSim compiler
        var application = Path.Combine(simPath, "xvhdl");
        if (options.Verbose)
            Console.WriteLine($"  Run Directory: {libDir}");
        if (options.Verbose)
            Console.WriteLine($"  Run Command: cmd /c {application} -file compile.do");
        return RunResults.Execute(
            CompileRules,
            "cmd",
            libDir,
            "/c",
            application,
            "-file",
            "compile.do");
    }

    /// <inheritdoc />
    public override TestResult Test(Options options, string test)
    {
        // Log the start of the compile command
        if (options.Verbose)
            Console.WriteLine($"Starting Vivado test {test}...");

        // Fail if we cannot find the simulator
        var simPath = SimulatorPath ??
                      throw new InvalidOperationException("Vivado Simulator not available");
        if (options.Verbose)
            Console.WriteLine($"  Simulator Path: {simPath}");

        // Get the library directory
        var libDir = Path.Combine(options.WorkingDirectory, "VHDLTest.out/Vivado");
        if (options.Verbose)
            Console.WriteLine($"  Library Directory: {libDir}");

        // Build the batch file
        var writer = new StringBuilder();
        writer.AppendLine("-nolog");
        writer.AppendLine("-standalone");
        writer.AppendLine("-runall");
        writer.AppendLine(test);

        // Write the batch file
        var script = Path.Combine(libDir, "test.do");
        if (options.Verbose)
            Console.WriteLine($"  Script File: {script}");
        File.WriteAllText(script, writer.ToString());

        // Run the test
        var application = Path.Combine(simPath, "xelab");
        if (options.Verbose)
            Console.WriteLine($"  Run Directory: {libDir}");
        if (options.Verbose)
            Console.WriteLine($"  Run Command: cmd /c {application} -file test.do");
        var testRunResults = RunResults.Execute(
            TestRules,
            "cmd",
            libDir,
            "/c",
            application,
            "-file",
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
        var simPathEnv = Environment.GetEnvironmentVariable("VHDLTEST_VIVADO_PATH");
        if (simPathEnv != null)
            return simPathEnv;

        // Find the path to the simulator application
        var simPath = Where("vivado");
        if (simPath == null)
            return null;

        // Return the working directory
        return Path.GetDirectoryName(simPath);
    }
}