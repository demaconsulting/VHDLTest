using System.Text;
using System.Text.RegularExpressions;
using DEMAConsulting.VHDLTest.Results;
using DEMAConsulting.VHDLTest.Run;

namespace DEMAConsulting.VHDLTest.Simulators;

/// <summary>
///     GHDL Simulator Class
/// </summary>
public sealed class GhdlSimulator : Simulator
{
    /// <summary>
    ///     Text match rules when compiling
    /// </summary>
    private static readonly RunLineRule[] CompileRules =
    {
        new(RunLineType.Warning, new Regex(@".*:\d+:\d+:warning:")),
        new(RunLineType.Error, new Regex(@".*:\d+:\d+: ")),
        new(RunLineType.Error, new Regex(".*:error:")),
        new(RunLineType.Error, new Regex(".*: cannot open"))
    };

    /// <summary>
    ///     Text match rules when running a test
    /// </summary>
    private static readonly RunLineRule[] TestRules =
    {
        new(RunLineType.Info, new Regex(@".*:\(assertion note\):")),
        new(RunLineType.Info, new Regex(@".*:\(report note\):")),
        new(RunLineType.Warning, new Regex(@".*:\(assertion warning\):")),
        new(RunLineType.Warning, new Regex(@".*:\(report warning\):")),
        new(RunLineType.Error, new Regex(@".*:\(assertion error\):")),
        new(RunLineType.Error, new Regex(@".*:\(report error\):")),
        new(RunLineType.Error, new Regex(@".*:\(assertion failure\):")),
        new(RunLineType.Error, new Regex(@".*:\(report failure\):")),
        new(RunLineType.Error, new Regex(".*:error:"))
    };

    /// <summary>
    ///     GHDL simulator instance
    /// </summary>
    public static readonly GhdlSimulator Instance = new();

    /// <summary>
    ///     Initializes a new instance of the GHDL simulator
    /// </summary>
    private GhdlSimulator() : base("GHDL", FindPath())
    {
    }

    /// <inheritdoc />
    public override RunResults Compile(Options options)
    {
        // Log the start of the compile command
        if (options.Verbose)
            Console.WriteLine("Starting GHDL compile...");

        // Fail if we cannot find the simulator
        var simPath = SimulatorPath ??
                      throw new InvalidOperationException("GHDL Simulator not available");
        if (options.Verbose)
            Console.WriteLine($"  Simulator Path: {simPath}");

        // Create the library directory
        var libDir = Path.Combine(options.WorkingDirectory, "VHDLTest.out/GHDL");
        if (options.Verbose)
            Console.WriteLine($"  Library Directory: {libDir}");
        if (!Directory.Exists(libDir))
            Directory.CreateDirectory(libDir);

        // Build the batch file
        var writer = new StringBuilder();
        foreach (var file in options.Config.Files)
            writer.AppendLine(file);

        // Write the batch file
        var script = Path.Combine(libDir, "compile.rsp");
        if (options.Verbose)
            Console.WriteLine($"  Script File: {script}");
        File.WriteAllText(script, writer.ToString());

        // Run the GHDL compiler
        var application = Path.Combine(simPath, "ghdl");
        if (options.Verbose)
            Console.WriteLine($"  Run Directory: {options.WorkingDirectory}");
        if (options.Verbose)
            Console.WriteLine($"  Run Command: {application} -a --std=08 --workdir=VHDLTest.out/GHDL @VHDLTest.out/GHDL/compile.rsp");
        return RunResults.Execute(
            CompileRules,
            application,
            options.WorkingDirectory,
            "-a",
            "--std=08",
            "--workdir=VHDLTest.out/GHDL",
            "@VHDLTest.out/GHDL/compile.rsp");
    }

    /// <inheritdoc />
    public override TestResult Test(Options options, string test)
    {
        // Log the start of the compile command
        if (options.Verbose)
            Console.WriteLine($"Starting GHDL test {test}...");

        // Fail if we cannot find the simulator
        var simPath = SimulatorPath ??
                      throw new InvalidOperationException("GHDL Simulator not available");
        if (options.Verbose)
            Console.WriteLine($"  Simulator Path: {simPath}");

        // Get the library directory
        var libDir = Path.Combine(options.WorkingDirectory, "VHDLTest.out/ModelSim");
        if (options.Verbose)
            Console.WriteLine($"  Library Directory: {libDir}");

        // Run the test
        var application = Path.Combine(simPath, "ghdl");
        if (options.Verbose)
            Console.WriteLine($"  Run Directory: {options.WorkingDirectory}");
        if (options.Verbose)
            Console.WriteLine($"  Run Command: {application} -r --std=08 --workdir=VHDLTest.out/GHDL {test}");
        var testRunResults = RunResults.Execute(
            TestRules,
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
    ///     Find the simulator path
    /// </summary>
    /// <returns>Simulator path or null if not found</returns>
    public static string? FindPath()
    {
        // Look for an environment variable
        var simPathEnv = Environment.GetEnvironmentVariable("VHDLTEST_GHDL_PATH");
        if (simPathEnv != null)
            return simPathEnv;

        // Find the path to the simulator application
        var simPath = Where("ghdl");
        if (simPath == null)
            return null;

        // Return the working directory
        return Path.GetDirectoryName(simPath);
    }
}