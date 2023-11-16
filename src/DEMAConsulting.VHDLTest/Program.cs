using System.Reflection;
using DEMAConsulting.VHDLTest.Results;
using DEMAConsulting.VHDLTest.Simulators;

namespace DEMAConsulting.VHDLTest;

/// <summary>
///     VHDLTest program class
/// </summary>
public class Program
{
    /// <summary>
    ///     Gets the version of this programs assembly
    /// </summary>
    public static string Version =>
        typeof(Program).Assembly
            .GetCustomAttributes()
            .OfType<AssemblyInformationalVersionAttribute>()
            .Select(x => x.InformationalVersion)
            .FirstOrDefault() ?? "Unknown";

    /// <summary>
    ///     Application entry point
    /// </summary>
    /// <param name="args">Program arguments</param>
    public static void Main(string[] args)
    {
        // Read the version
        var version = Version;

        // Handle version query
        if (args.Contains("-v") || args.Contains("--version"))
        {
            Console.WriteLine(version);
            Environment.Exit(0);
        }

        // Print Banner
        Console.WriteLine($"VHDL Test Bench Runner (VHDLTest) {version}");
        Console.WriteLine();

        // Handle help request
        if (args.Contains("-h") || args.Contains("-?") || args.Contains("--help"))
        {
            Options.PrintUsage();
            Environment.Exit(0);
        }

        try
        {
            // Parse the options
            var options = Options.Parse(args);

            // Get the simulator
            var simulator = SimulatorFactory.Get(options.Simulator) ??
                            throw new InvalidOperationException("Simulator not found");

            // Execute the build/test and get the results
            var results = TestResults.Execute(options, simulator);
            
            // Save the test results
            if (options.TestResultsFile != null)
                results.SaveToTrx(options.TestResultsFile);

            // If we got failures then exit with an error code
            if (!options.ExitZero && results.Fails.Any())
                Environment.Exit(1);
        }
        catch (InvalidOperationException e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(e.Message);
            Console.ResetColor();
            Environment.Exit(1);
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(e);
            Console.ResetColor();
            throw;
        }
    }
}