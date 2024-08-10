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

        var e = args.AsEnumerable().GetEnumerator();
        while (e.MoveNext())
        {
            var arg = e.Current;
            switch (arg)
            {
                case "-c":
                case "--config":
                    configFile = GetConfigurationFile(e);
                    config = ConfigDocument.ReadFile(configFile);
                    break;

                case "-r":
                case "--results":
                    testResultsFile = GetResultsFile(e);
                    break;

                case "-s":
                case "--simulator":
                    simulator = GetSimulator(e);
                    break;

                case "--verbose":
                    verbose = true;
                    break;

                case "-0":
                case "--exit-0":
                    exitZero = true;
                    break;

                case "--":
                    while (e.MoveNext())
                        customTests.Add(e.Current);
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

    /// <summary>
    /// Get the configuration file argument
    /// </summary>
    /// <param name="enumerator">Argument enumerator</param>
    /// <returns>Configuration file</returns>
    /// <exception cref="InvalidOperationException">Thrown on missing argument</exception>
    private static string GetConfigurationFile(IEnumerator<string> enumerator)
    {
        if (!enumerator.MoveNext())
            throw new InvalidOperationException("Missing configuration file name");

        return enumerator.Current;
    }

    /// <summary>
    /// Get the results file argument
    /// </summary>
    /// <param name="enumerator">Argument enumerator</param>
    /// <returns>Results file</returns>
    /// <exception cref="InvalidOperationException">Thrown on missing argument</exception>
    private static string GetResultsFile(IEnumerator<string> enumerator)
    {
        if (!enumerator.MoveNext())
            throw new InvalidOperationException("Missing results file name");

        return enumerator.Current;
    }

    /// <summary>
    /// Get the simulator argument
    /// </summary>
    /// <param name="enumerator">Argument enumerator</param>
    /// <returns>Simulator</returns>
    /// <exception cref="InvalidOperationException">Thrown on missing argument</exception>
    private static string GetSimulator(IEnumerator<string> enumerator)
    {
        if (!enumerator.MoveNext())
            throw new InvalidOperationException("Missing simulator name");

        return enumerator.Current;
    }
}