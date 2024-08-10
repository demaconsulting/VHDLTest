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
/// Program Arguments Class
/// </summary>
/// <param name="ConfigFile">Configuration file name</param>
/// <param name="ResultsFile">Results file name</param>
/// <param name="Simulator">Simulator name</param>
/// <param name="Verbose">Verbose flag</param>
/// <param name="ExitZero">Exit-zero flag</param>
/// <param name="Validate">Validate flag</param>
/// <param name="CustomTests">Custom tests</param>
public record Arguments(
    string? ConfigFile,
    string? ResultsFile,
    string? Simulator,
    bool Verbose,
    bool ExitZero,
    bool Validate,
    ReadOnlyCollection<string>? CustomTests)
{
    /// <summary>
    /// Parse the program arguments
    /// </summary>
    /// <param name="args">Program arguments</param>
    /// <returns>Parsed arguments</returns>
    /// <exception cref="InvalidOperationException">On invalid arguments</exception>
    public static Arguments Parse(string[] args)
    {
        string? configFile = null;
        string? resultsFile = null;
        string? simulator = null;
        var verbose = false;
        var exitZero = false;
        var validate = false;
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
                    break;

                case "-r":
                case "--results":
                    resultsFile = GetResultsFile(e);
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

                case "--validate":
                    validate = true;
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

        return new Arguments(
            configFile,
            resultsFile,
            simulator,
            verbose,
            exitZero,
            validate,
            customTests.Count > 0 ? customTests.AsReadOnly() : null);
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