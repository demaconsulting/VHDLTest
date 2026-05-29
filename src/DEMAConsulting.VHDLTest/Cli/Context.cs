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

namespace DEMAConsulting.VHDLTest.Cli;

/// <summary>
///     Parses the raw command-line argument array and aggregates all typed CLI options,
///     output channels, and runtime state for the lifetime of a single VHDLTest invocation.
/// </summary>
/// <remarks>
///     Context is the single point of truth for all command-line–derived state. It aggregates
///     parsed flags and path options, owns the optional log-file <see cref="StreamWriter"/>,
///     and exposes unified output methods so callers do not need to manage multiple output
///     targets. Implements <see cref="IDisposable"/> to ensure the log-file writer is closed
///     deterministically; always construct via <see cref="Create"/> and wrap in a
///     <c>using</c> statement.
/// </remarks>
public sealed class Context : IDisposable
{
    /// <summary>
    ///     Output log-file writer (when logging output to file)
    /// </summary>
    private readonly StreamWriter? _log;

    /// <summary>
    ///     Initializes a new instance of the Context class.
    /// </summary>
    /// <remarks>
    ///     Private to enforce construction via the <see cref="Create"/> factory method.
    ///     Ownership of <paramref name="log"/> is transferred to this instance; the writer
    ///     will be disposed when this instance is disposed via <see cref="Dispose"/>.
    /// </remarks>
    /// <param name="log">Optional log-file writer; may be null when logging is not configured.</param>
    private Context(StreamWriter? log)
    {
        _log = log;
    }

    /// <summary>
    ///     Gets a value indicating the version has been requested
    /// </summary>
    public bool Version { get; private init; }

    /// <summary>
    ///     Gets a value indicating help has been requested
    /// </summary>
    public bool Help { get; private init; }

    /// <summary>
    ///     Gets a value indicating silent-output has been requested
    /// </summary>
    public bool Silent { get; private init; }

    /// <summary>
    ///     Gets a value indicating whether verbose output should be written
    /// </summary>
    public bool Verbose { get; private init; }

    /// <summary>
    ///     Gets a flag indicating whether to force a zero exit-code on test failures
    /// </summary>
    public bool ExitZero { get; private init; }

    /// <summary>
    ///     Gets a flag indicating whether to perform validation
    /// </summary>
    public bool Validate { get; private init; }

    /// <summary>
    ///     Gets the depth of the validation report.
    /// </summary>
    /// <value>
    ///     Number of heading levels to include in the validation report output. Defaults to
    ///     <c>1</c> when <c>--depth</c> is not specified. Must be greater than or equal to
    ///     <c>1</c>; passing a value less than <c>1</c> causes <see cref="Create"/> to throw
    ///     <see cref="InvalidOperationException"/>.
    /// </value>
    public int Depth { get; private init; }

    /// <summary>
    ///     Gets the config file name
    /// </summary>
    public string? ConfigFile { get; private init; }

    /// <summary>
    ///     Gets the results file name
    /// </summary>
    public string? ResultsFile { get; private init; }

    /// <summary>
    ///     Gets the simulator name
    /// </summary>
    public string? Simulator { get; private init; }

    /// <summary>
    ///     Gets the custom tests
    /// </summary>
    public IReadOnlyList<string>? CustomTests { get; private init; }

    /// <summary>
    ///     Gets the errors count
    /// </summary>
    public int Errors { get; private set; }

    /// <summary>
    ///     Gets the proposed exit code
    /// </summary>
    public int ExitCode => Errors > 0 ? 1 : 0;

    /// <summary>
    ///     Dispose of this context
    /// </summary>
    public void Dispose()
    {
        _log?.Dispose();
    }

    /// <summary>
    ///     Writes colored text without a line terminator to the console (unless silent mode is
    ///     active) and to the log file.
    /// </summary>
    /// <remarks>
    ///     The log-file write is unconditional with respect to the Silent flag; only the console
    ///     write is suppressed.
    /// </remarks>
    /// <param name="color">Text color</param>
    /// <param name="text">Text to write</param>
    public void Write(ConsoleColor color, string text)
    {
        // Write to the console if not silent
        if (!Silent)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ResetColor();
        }

        // Write to the optional log
        _log?.Write(text);
    }

    /// <summary>
    ///     Writes a line of text to all configured outputs only when verbose mode is active.
    /// </summary>
    /// <remarks>
    ///     If <see cref="Verbose"/> is false the call is a no-op. When Verbose is true the line
    ///     is written to the console (unless <see cref="Silent"/> is true) and to the log file.
    /// </remarks>
    /// <param name="text">Text to write</param>
    public void WriteVerboseLine(string text)
    {
        // Skip if not verbose
        if (!Verbose)
        {
            return;
        }

        // Write to the console unless silent
        if (!Silent)
        {
            Console.WriteLine(text);
        }

        // Write to the log if specified
        _log?.WriteLine(text);
    }

    /// <summary>
    ///     Writes a line of text to all configured outputs unless silent mode is active.
    /// </summary>
    /// <remarks>
    ///     Always writes to the log file when one is open, regardless of the Silent flag.
    ///     Console output is suppressed when <see cref="Silent"/> is true.
    /// </remarks>
    /// <param name="text">Text to write</param>
    public void WriteLine(string text)
    {
        // Write to the console unless silent
        if (!Silent)
        {
            Console.WriteLine(text);
        }

        // Write to the log if specified
        _log?.WriteLine(text);
    }

    /// <summary>
    ///     Increments the error counter and writes an error message to the console (unless
    ///     silent mode is active) and unconditionally to all configured log outputs.
    /// </summary>
    /// <remarks>
    ///     <see cref="Errors"/> is always incremented, even when <paramref name="message"/> is
    ///     null. Console output is suppressed when <see cref="Silent"/> is true; log-file output
    ///     is unconditional and is always written when a log file is configured. The message,
    ///     when non-null, is written to the console in <see cref="ConsoleColor.Red"/> (unless
    ///     silent mode is active) and to the log file.
    /// </remarks>
    /// <param name="message">Error message to write</param>
    public void WriteError(string? message)
    {
        // Increment the number of errors
        Errors++;

        // Skip writing if no message provided
        if (message == null)
        {
            return;
        }

        // Write to the console unless silent
        if (!Silent)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        // Write to the log if specified
        _log?.WriteLine(message);
    }

    /// <summary>
    ///     Creates a new <see cref="Context"/> by parsing the supplied argument array.
    /// </summary>
    /// <remarks>
    ///     This static factory is the only supported construction path for <see cref="Context"/>.
    ///     The caller is responsible for disposing the returned instance (e.g., with a
    ///     <c>using</c> statement) to ensure any open log-file writer is flushed and closed
    ///     deterministically. Arguments are parsed left-to-right; unrecognised flags (any token
    ///     starting with <c>-</c> that does not match a known option) cause an
    ///     <see cref="InvalidOperationException"/> to be thrown. Bare tokens (tokens not starting
    ///     with <c>-</c>) are accumulated as custom test names. The separator <c>--</c> switches
    ///     all subsequent tokens to custom test names regardless of their prefix.
    /// </remarks>
    /// <param name="args">
    ///     The command-line argument array to parse. Must not be null; pass an empty array to
    ///     obtain a default-valued <see cref="Context"/>.
    /// </param>
    /// <returns>A fully initialised <see cref="Context"/> ready for use.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="args"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when an unrecognised argument flag is provided or a required option value
    ///     is missing (e.g., <c>-c</c> with no following filename).
    /// </exception>
    public static Context Create(string[] args)
    {
        // Validate input
        ArgumentNullException.ThrowIfNull(args);

        // Process the arguments
        var help = false;
        var version = false;
        var silent = false;
        var verbose = false;
        var exitZero = false;
        var validate = false;
        var depth = 1;
        string? logFile = null;
        string? configFile = null;
        string? resultsFile = null;
        string? simulator = null;
        var customTests = new List<string>();
        var e = args.AsEnumerable().GetEnumerator();
        while (e.MoveNext())
        {
            var arg = e.Current;
            switch (arg)
            {
                case "-v":
                case "--version":
                    // Save version query
                    version = true;
                    break;

                case "-h":
                case "-?":
                case "--help":
                    // Save help query
                    help = true;
                    break;

                case "--silent":
                    // Handle silent flag
                    silent = true;
                    break;

                case "--verbose":
                    // Handle verbose flag
                    verbose = true;
                    break;

                case "-0":
                case "--exit-0":
                    // Handle exit-zero flag
                    exitZero = true;
                    break;

                case "--validate":
                    // Handle validate flag
                    validate = true;
                    break;

                case "--depth":
                    // Handle validation markdown depth
                    var depthArgument = GetArgument(e, "Missing depth argument");
                    if (!int.TryParse(depthArgument, out depth) || depth < 1)
                    {
                        throw new InvalidOperationException($"Invalid value '{depthArgument}' for option '--depth'; expected an integer greater than or equal to 1.");
                    }

                    break;

                case "-l":
                case "--log":
                    // Handle logging output
                    logFile = GetArgument(e, "Missing log output file name");
                    break;

                case "-c":
                case "--config":
                    // Handle configuration file
                    configFile = GetArgument(e, "Missing configuration file name");
                    break;

                case "-r":
                case "--result":
                case "--results":
                    // Handle results file
                    resultsFile = GetArgument(e, "Missing results file name");
                    break;

                case "-s":
                case "--simulator":
                    // Handle simulator name
                    simulator = GetArgument(e, "Missing simulator name");
                    break;

                case "--":
                    // Handle custom tests
                    while (e.MoveNext())
                    {
                        customTests.Add(e.Current);
                    }

                    break;

                default:
                    if (arg.StartsWith('-'))
                    {
                        throw new InvalidOperationException($"Unsupported option {arg}");
                    }

                    // Handle custom tests
                    customTests.Add(arg);
                    break;
            }
        }

        return new Context(logFile != null ? new StreamWriter(logFile) : null)
        {
            Version = version,
            Help = help,
            Silent = silent,
            Verbose = verbose,
            ExitZero = exitZero,
            Validate = validate,
            Depth = depth,
            ConfigFile = configFile,
            ResultsFile = resultsFile,
            Simulator = simulator,
            CustomTests = customTests.Count > 0 ? customTests.AsReadOnly() : null
        };
    }

    /// <summary>
    ///     Get the next argument
    /// </summary>
    /// <param name="enumerator">Argument enumerator</param>
    /// <param name="message">Error message if missing</param>
    /// <returns>Next argument</returns>
    /// <exception cref="InvalidOperationException">Thrown if missing</exception>
    private static string GetArgument(IEnumerator<string> enumerator, string message)
    {
        if (!enumerator.MoveNext())
        {
            throw new InvalidOperationException(message);
        }

        return enumerator.Current;
    }
}
