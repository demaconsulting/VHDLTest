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
using DEMAConsulting.VHDLTest.Cli;
using DEMAConsulting.VHDLTest.Results;
using DEMAConsulting.VHDLTest.SelfTest;
using DEMAConsulting.VHDLTest.Simulators;

namespace DEMAConsulting.VHDLTest;

/// <summary>
///     CLI entry point and dispatch coordinator for the VHDLTest application.
/// </summary>
/// <remarks>
///     <see cref="Main"/> creates the <see cref="Cli.Context"/> from raw command-line
///     arguments and delegates to <see cref="Run"/> for all dispatch logic.
///     <see cref="Run"/> handles version display, help, self-validation, and the primary
///     test-execution path, communicating results through the context's error channel and
///     exit code. All operational errors are caught and reported without re-throwing;
///     unexpected exceptions are reported and re-thrown so the runtime records them as
///     unhandled.
/// </remarks>
public static class Program
{
    /// <summary>
    ///     Gets the informational version string of this program's assembly, read from
    ///     <see cref="System.Reflection.AssemblyInformationalVersionAttribute"/>.
    /// </summary>
    /// <remarks>
    ///     The value is resolved once at static initialization from the assembly's
    ///     <see cref="System.Reflection.AssemblyInformationalVersionAttribute"/>; if the
    ///     attribute is absent, the value is <c>"Unknown"</c>. The property is read-only
    ///     after static initialization and is therefore safe to read from any thread.
    /// </remarks>
    public static string Version { get; } =
        typeof(Program).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion ?? "Unknown";

    /// <summary>
    ///     Creates the program context from command-line arguments, delegates execution to
    ///     <see cref="Run"/>, and propagates the resulting exit code to the process.
    /// </summary>
    /// <remarks>
    ///     The primary output channel is <see cref="System.Environment.ExitCode"/>, set from
    ///     <see cref="Context.ExitCode"/> after <see cref="Run"/> completes. Two exception
    ///     layers are present: <see cref="InvalidOperationException"/> is caught and reported
    ///     as a formatted error message with exit code 1; all other exceptions are reported
    ///     and re-thrown so that the runtime reports an unhandled exception with a non-zero
    ///     exit code.
    /// </remarks>
    /// <param name="args">Program arguments. Must not be null.</param>
    /// <exception cref="Exception">Thrown when an unexpected error occurs that cannot be attributed to an operational condition; the exception propagates to the runtime unhandled.</exception>
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
    /// <remarks>
    ///     Dispatch order: version display → help display → self-validation → normal test run.
    ///     Each early-exit path returns without setting an error on <paramref name="context"/>,
    ///     so <see cref="Context.ExitCode"/> remains 0 on normal dispatch. Results from test
    ///     execution are communicated through <paramref name="context"/> via
    ///     <see cref="Context.WriteError"/>, which increments <see cref="Context.Errors"/> and
    ///     causes <see cref="Context.ExitCode"/> to return non-zero.
    /// </remarks>
    /// <param name="context">Program context. Must not be null.</param>
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
            {
                try
                {
                    results.SaveResults(context.ResultsFile);
                }
                catch (Exception ex)
                {
                    context.WriteError($"Error: Failed to write results file: {ex.Message}");
                }
            }

            // If we got failures then exit with an error code
            if (!context.ExitZero && results.Fails.Any())
            {
                // Pass null to increment the error counter without printing a duplicate message —
                // the test failure details have already been written by PrintSummary
                context.WriteError(null);
            }
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
    /// <remarks>
    ///     Writes the usage/help text to the context output channel. The method is stateless —
    ///     it only reads from the constant usage string and writes to <paramref name="context"/>;
    ///     it has no side effects beyond writing to the context. It is not responsible for setting
    ///     an error condition; callers must call <see cref="Context.WriteError"/> separately if a
    ///     non-zero exit code is required. Stateless and thread-safe provided the caller does not
    ///     share <paramref name="context"/> across threads.
    /// </remarks>
    /// <param name="context">Output target that receives the usage text. Must not be null.</param>
    private static void PrintUsage(Context context)
    {
        context.WriteLine(
            """
            Usage: VHDLTest [options] [tests]
            
            Options:
              -h, -?, --help               Display help
              -v, --version                Display version
              --silent                     Silence console output
              --verbose                    Verbose output
              --validate                   Perform self-validation
              --depth <n>                  Validation report depth (default: 1)
              -l, --log <log.txt>          Log output to file
              -c, --config <config.yaml>   Specify configuration
              -r, --results <out.trx>      Specify test results file
              -s, --simulator <name>       Specify simulator
              -0, --exit-0                 Exit with code 0 if test fail
              --                           End of options

            Arguments:
              [tests]                      Optional test bench names to filter which tests to run
            """);
    }
}
