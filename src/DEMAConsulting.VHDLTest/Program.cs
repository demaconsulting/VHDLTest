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
    public static string Version { get; } =
        typeof(Program).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion ?? "Unknown";

    /// <summary>
    ///     Application entry point
    /// </summary>
    /// <param name="args">Program arguments</param>
    public static void Main(string[] args)
    {
        try
        {
            using var context = Context.Create(args);
            Run(context);
            Environment.ExitCode = context.ExitCode;
        }
        catch (InvalidOperationException e)
        {
            // Report standard failure
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {e.Message}");
            Console.ResetColor();
            Environment.Exit(1);
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {e}");
            Console.ResetColor();
            throw;
        }
    }

    /// <summary>
    ///     Run the program context
    /// </summary>
    /// <param name="context">Program context</param>
    public static void Run(Context context)
    {
        // Handle version query
        if (context.Version)
        {
            context.WriteLine(Version);
            return;
        }

        // Print version banner
        context.WriteLine($"VHDL Test Bench Runner (VHDLTest) {Version}\n");

        // Handle help query
        if (context.Help)
        {
            PrintUsage(context);
            return;
        }

        // Handle self-validation
        if (context.Validate)
        {
            Validation.Run(context);
            return;
        }

        // Handle missing arguments
        if (context.ConfigFile == null)
        {
            context.WriteError("Error: Missing arguments");
            PrintUsage(context);
            return;
        }

        try
        {
            // Get the simulator
            var simulator = SimulatorFactory.Get(context.Simulator) ??
                            throw new InvalidOperationException("Simulator not found");

            // Execute the build/test and get the results
            var options = Options.Parse(context);
            var results = TestResults.Execute(context, options, simulator);

            // Print the results summary
            results.PrintSummary(context);

            // Save the test results
            if (context.ResultsFile != null)
                results.SaveToTrx(context.ResultsFile);

            // If we got failures then exit with an error code
            if (!context.ExitZero && results.Fails.Any())
                context.WriteError(null);
        }
        catch (InvalidOperationException ex)
        {
            // Report operation error
            context.WriteError($"Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            // Report unknown exception
            context.WriteError(ex.ToString());
        }
    }

    /// <summary>
    ///     Print usage information
    /// </summary>
    /// <param name="context">Program context</param>
    private static void PrintUsage(Context context)
    {
        context.WriteLine(
            """
            Usage: VHDLTest [options] [tests]
            
            Options:
              -h, --help                   Display help
              -v, --version                Display version
              --silent                     Silence console output
              --verbose                    Verbose output
              --validate                   Perform self-validation
              -c, --config <config.yaml>   Specify configuration
              -r, --results <out.trx>      Specify test results file
              -s, --simulator <name>       Specify simulator
              -0, --exit-0                 Exit with code 0 if test fail
              --                           End of options
            """);
    }
}