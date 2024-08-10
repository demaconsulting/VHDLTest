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

using System.Reflection;
using DEMAConsulting.VHDLTest.Results;
using DEMAConsulting.VHDLTest.Simulators;

namespace DEMAConsulting.VHDLTest;

/// <summary>
///     VHDLTest program class
/// </summary>
public static class Program
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
        if (args.Length == 0 || args.Contains("-h") || args.Contains("-?") || args.Contains("--help"))
        {
            PrintUsage();
            Environment.Exit(0);
        }

        try
        {
            // Parse the arguments
            var arguments = Arguments.Parse(args);
            if (arguments.Validate)
                Environment.Exit(Validation.Run(arguments));

            // Get the simulator
            var simulator = SimulatorFactory.Get(arguments.Simulator) ??
                            throw new InvalidOperationException("Simulator not found");

            // Execute the build/test and get the results
            var options = Options.Parse(arguments);
            var results = TestResults.Execute(options, simulator);

            // Print the results summary
            results.PrintSummary();

            // Save the test results
            if (arguments.ResultsFile != null)
                results.SaveToTrx(arguments.ResultsFile);

            // If we got failures then exit with an error code
            if (!arguments.ExitZero && results.Fails.Any())
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

    /// <summary>
    ///     Print usage information
    /// </summary>
    private static void PrintUsage()
    {
        Console.WriteLine("Usage: VHDLTest [options] [tests]");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  -h|-?|--help                 Display help");
        Console.WriteLine("  -v|--version                 Display version");
        Console.WriteLine("    |--validate                Validate operational");
        Console.WriteLine("  -c|--config <config.yaml>    Specify configuration");
        Console.WriteLine("    |--verbose                 Verbose output");
        Console.WriteLine("  -r|--results <out.trx>       Specify test results file");
        Console.WriteLine("  -s|--simulator <name>        Specify simulator");
        Console.WriteLine("  -0|--exit-0                  Exit with code 0 if test fail");
        Console.WriteLine("  --                           End of options");
    }
}