using DEMAConsulting.VHDLTest.Run;
using System.Text;
using System.Text.RegularExpressions;
using DEMAConsulting.VHDLTest.Results;

namespace DEMAConsulting.VHDLTest.Simulators;

/// <summary>
///     NVC Simulator Class
/// </summary>
public sealed class NvcSimulator : Simulator
{
    /// <summary>
    ///     Text match rules when compiling
    /// </summary>
    private static readonly RunLineRule[] CompileRules =
    {
        new(RunLineType.Info, new Regex(@".* Note:")),
        new(RunLineType.Warning, new Regex(@".* Warning:")),
        new(RunLineType.Error, new Regex(".* Error:")),
        new(RunLineType.Error, new Regex(".* Failure:")),
        new(RunLineType.Error, new Regex(".* Fatal:"))
    };

    /// <summary>
    ///     Text match rules when running a test
    /// </summary>
    private static readonly RunLineRule[] TestRules =
    {
        new(RunLineType.Info, new Regex(@".* Note:")),
        new(RunLineType.Warning, new Regex(@".* Warning:")),
        new(RunLineType.Error, new Regex(".* Error:")),
        new(RunLineType.Error, new Regex(".* Failure:")),
        new(RunLineType.Error, new Regex(".* Fatal:"))
    };

    /// <summary>
    ///     NVC simulator instance
    /// </summary>
    public static readonly NvcSimulator Instance = new();

    /// <summary>
    ///     Initializes a new instance of the NVC simulator
    /// </summary>
    private NvcSimulator() : base("NVC", FindPath())
    {
    }

    /// <inheritdoc />
    public override RunResults Compile(Options options)
    {
        // Log the start of the compile command
        if (options.Verbose)
            Console.WriteLine("Starting NVC compile...");

        // Fail if we cannot find the simulator
        var simPath = SimulatorPath ??
                      throw new InvalidOperationException("NVC Simulator not available");
        if (options.Verbose)
            Console.WriteLine($"  Simulator Path: {simPath}");

        // Create the library directory
        var libDir = Path.Combine(options.WorkingDirectory, "VHDLTest.out/NVC");
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
        var application = Path.Combine(simPath, "nvc");
        if (options.Verbose)
            Console.WriteLine($"  Run Directory: {options.WorkingDirectory}");
        if (options.Verbose)
            Console.WriteLine($"  Run Command: {application} --std=08 --work=work:VHDLTest.out/NVC/lib -a @VHDLTest.out/NVC/compile.rsp");
        return RunResults.Execute(
            CompileRules,
            application,
            options.WorkingDirectory,
            "--std=08",
            "--work=work:VHDLTest.out/NVC/lib",
            "-a",
            "@VHDLTest.out/NVC/compile.rsp");
    }

    /// <inheritdoc />
    public override TestResult Test(Options options, string test)
    {
        // Log the start of the compile command
        if (options.Verbose)
            Console.WriteLine($"Starting NVC test {test}...");

        // Fail if we cannot find the simulator
        var simPath = SimulatorPath ??
                      throw new InvalidOperationException("NVC Simulator not available");
        if (options.Verbose)
            Console.WriteLine($"  Simulator Path: {simPath}");

        // Get the library directory
        var libDir = Path.Combine(options.WorkingDirectory, "VHDLTest.out/NVC");
        if (options.Verbose)
            Console.WriteLine($"  Library Directory: {libDir}");

        // Run the test
        var application = Path.Combine(simPath, "nvc");
        if (options.Verbose)
            Console.WriteLine($"  Run Directory: {options.WorkingDirectory}");
        if (options.Verbose)
            Console.WriteLine($"  Run Command: {application} --std=2008 --work=work:VHDLTest.out/NVC/lib -e {test} -r {test}");
        var testRunResults = RunResults.Execute(
            TestRules,
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
    ///     Find the simulator path
    /// </summary>
    /// <returns>Simulator path or null if not found</returns>
    public static string? FindPath()
    {
        // Look for an environment variable
        var simPathEnv = Environment.GetEnvironmentVariable("VHDLTEST_NVC_PATH");
        if (simPathEnv != null)
            return simPathEnv;

        // Find the path to the simulator application
        var simPath = Where("nvc");
        if (simPath == null)
            return null;

        // Return the working directory
        return Path.GetDirectoryName(simPath);
    }
}