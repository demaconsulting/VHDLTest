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

namespace DEMAConsulting.VHDLTest;

/// <summary>
/// Program Arguments Class
/// </summary>
public sealed class Context : IDisposable
{
    /// <summary>
    ///     Output log-file writer (when logging output to file)
    /// </summary>
    private readonly StreamWriter? _log;

    /// <summary>
    ///     Initializes a new instance of the Context class
    /// </summary>
    /// <param name="log">Optional log-file writer</param>
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
    ///     Gets the depth of the validation report
    /// </summary>
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
    ///     Write colored text
    /// </summary>
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
    ///     Write text to output
    /// </summary>
    /// <param name="text">Text to write</param>
    public void WriteVerboseLine(string text)
    {
        // Skip if not verbose
        if (!Verbose)
            return;

        // Write to the console unless silent
        if (!Silent)
            Console.WriteLine(text);

        // Write to the log if specified
        _log?.WriteLine(text);
    }

    /// <summary>
    ///     Write text to output
    /// </summary>
    /// <param name="text">Text to write</param>
    public void WriteLine(string text)
    {
        // Write to the console unless silent
        if (!Silent)
            Console.WriteLine(text);

        // Write to the log if specified
        _log?.WriteLine(text);
    }

    /// <summary>
    ///     Write an error message to output
    /// </summary>
    /// <param name="message">Error message to write</param>
    public void WriteError(string message)
    {
        // Write to the console unless silent
        if (!Silent)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        // Write to the log if specified
        _log?.WriteLine(message);

        // Increment the number of errors
        Errors++;
    }

    /// <summary>
    ///     Create the context
    /// </summary>
    /// <param name="args">Program arguments</param>
    /// <returns>Program context</returns>
    /// <exception cref="InvalidOperationException">On invalid arguments</exception>
    public static Context Create(string[] args)
    {
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
                    depth = int.Parse(GetArgument(e, "Missing depth argument"));
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
                        customTests.Add(e.Current);
                    break;

                default:
                    if (arg.StartsWith('-'))
                        throw new InvalidOperationException($"Unsupported option {arg}");

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
            throw new InvalidOperationException(message);

        return enumerator.Current;
    }
}