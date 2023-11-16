using System.Collections.ObjectModel;

namespace DEMAConsulting.VHDLTest;

/// <summary>
///     Program Options Class
/// </summary>
/// <param name="WorkingDirectory">Working directory</param>
/// <param name="Config">Configuration options</param>
/// <param name="TestResultsFile">Optional test results file</param>
/// <param name="Simulator">Optional simulator name</param>
/// <param name="Verbose">Verbose flag</param>
/// <param name="ExitZero">Exit with zero error code on test fails</param>
/// <param name="CustomTests">Optional custom tests</param>
public record Options(string WorkingDirectory,
    ConfigDocument Config,
    string? TestResultsFile,
    string? Simulator,
    bool Verbose,
    bool ExitZero,
    ReadOnlyCollection<string>? CustomTests)
{
    /// <summary>
    ///     Parse options from command line arguments
    /// </summary>
    /// <param name="args">Command line arguments</param>
    /// <returns>Options</returns>
    public static Options Parse(string[] args)
    {
        // Process the arguments
        string? configFile = null;
        ConfigDocument? config = null;
        string? testResultsFile = null;
        string? simulator = null;
        var verbose = false;
        var exitZero = false;
        var customTests = new List<string>();
        for (var i = 0; i < args.Length;)
        {
            var arg = args[i++];
            switch (arg)
            {
                case "-c":
                case "--config":
                    if (i >= args.Length)
                        throw new InvalidOperationException("Missing configuration file name");
                    configFile = args[i++];
                    config = ConfigDocument.ReadFile(configFile);
                    break;

                case "-r":
                case "--results":
                    if (i >= args.Length)
                        throw new InvalidOperationException("Missing results file name");
                    testResultsFile = args[i++];
                    break;

                case "-s":
                case "--simulator":
                    if (i >= args.Length)
                        throw new InvalidOperationException("Missing results file name");
                    simulator = args[i++];
                    break;

                case "--verbose":
                    verbose = true;
                    break;

                case "-0":
                case "--exit-0":
                    exitZero = true;
                    break;

                case "--":
                    customTests.AddRange(args.Skip(i).ToArray());
                    i = args.Length;
                    break;

                default:
                    if (arg.StartsWith('-'))
                        throw new InvalidOperationException($"Unsupported option {arg}");

                    customTests.Add(arg);
                    break;
            }
        }

        // Ensure we got a config file
        if (configFile == null || config == null)
            throw new InvalidOperationException("Configuration file not specified");

        // Get the working directory
        var absConfigFile = Path.GetFullPath(configFile);
        var workingDir = Path.GetDirectoryName(absConfigFile)
                         ?? throw new InvalidOperationException($"Invalid configuration file {absConfigFile}");

        // Return the new options object
        return new Options(
            workingDir,
            config,
            testResultsFile,
            simulator,
            verbose,
            exitZero,
            customTests.Count > 0 ? customTests.AsReadOnly() : null);
    }

    /// <summary>
    ///     Print usage information
    /// </summary>
    public static void PrintUsage()
    {
        Console.WriteLine("Usage: VHDLTest [options] [tests]");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  -h|-?|--help                 Display help");
        Console.WriteLine("  -v|--version                 Display version");
        Console.WriteLine("  -c|--config <config.yaml>    Specify configuration");
        Console.WriteLine("    |--verbose                 Verbose output");
        Console.WriteLine("  -r|--results <out.trx>       Specify test results file");
        Console.WriteLine("  -s|--simulator <name>        Specify simulator");
        Console.WriteLine("  -0|--exit-0                  Exit with code 0 if test fail");
        Console.WriteLine("  --                           End of options");
    }
}