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

using DEMAConsulting.VHDLTest.Cli;
using DEMAConsulting.VHDLTest.Results;
using DEMAConsulting.VHDLTest.Run;

namespace DEMAConsulting.VHDLTest.Simulators;

/// <summary>
///     Abstract base class defining the uniform compile-and-test contract that all
///     VHDL simulator integrations must implement.
/// </summary>
/// <remarks>
///     The abstract base class pattern ensures that every concrete simulator exposes
///     the same interface — <see cref="SimulatorName"/>, <see cref="SimulatorPath"/>,
///     <see cref="Available"/>, <see cref="Compile"/>, and <see cref="Test"/> — so
///     the core run logic in <c>TestResults</c> can invoke any supported simulator
///     without coupling to simulator-specific details. Concrete implementations are
///     obligated to throw <see cref="InvalidOperationException"/> from
///     <see cref="Compile"/> and <see cref="Test"/> when <see cref="SimulatorPath"/>
///     is null, because the base class cannot enforce this precondition itself.
/// </remarks>
/// <param name="simulatorName">Display name of the simulator (e.g., "GHDL"). Must not be null.</param>
/// <param name="simulatorPath">
///     Absolute path to the directory containing the simulator executable, or null when
///     the simulator is not installed.
/// </param>
public abstract class Simulator(string simulatorName, string? simulatorPath)
{
    /// <summary>
    ///     Gets the name of the simulator
    /// </summary>
    /// <returns>
    ///     The display name of this simulator instance (for example, "GHDL" or "NVC"). This
    ///     value is guaranteed non-null; it is set from the constructor's
    ///     <c>simulatorName</c> parameter at initialization time.
    /// </returns>
    public string SimulatorName => simulatorName;

    /// <summary>
    ///     Gets the path to the simulator installation directory.
    /// </summary>
    /// <returns>
    ///     The path to the simulator executable or directory, or <c>null</c> if the simulator
    ///     is not installed.
    /// </returns>
    public string? SimulatorPath => simulatorPath;

    /// <summary>
    ///     Tests whether the simulator is available for use.
    /// </summary>
    /// <returns>
    ///     <c>true</c> if <see cref="SimulatorPath"/> is non-null; <c>false</c> otherwise.
    /// </returns>
    public bool Available()
    {
        return SimulatorPath != null;
    }

    /// <summary>
    ///     Compile the simulator library
    /// </summary>
    /// <param name="context">
    ///     Execution context used for logging output and error reporting. Must not be null.
    /// </param>
    /// <param name="options">
    ///     Parsed command-line options providing working directory, configuration, and verbosity
    ///     settings. Must not be null.
    /// </param>
    /// <returns>
    ///     The ordered list of parsed output lines produced by the compilation, each classified
    ///     by severity type (Text, Info, Warning, or Error) according to the simulator's compile
    ///     output patterns.
    /// </returns>
    /// <exception cref="InvalidOperationException">Thrown by implementations when SimulatorPath is null.</exception>
    public abstract RunResults Compile(Context context, Options options);

    /// <summary>
    ///     Execute a test
    /// </summary>
    /// <param name="context">
    ///     Execution context used for logging output and error reporting. Must not be null.
    /// </param>
    /// <param name="options">
    ///     Parsed command-line options providing working directory, configuration, and verbosity
    ///     settings. Must not be null.
    /// </param>
    /// <param name="test">Test name</param>
    /// <returns>
    ///     The ordered list of parsed output lines produced by the test execution, each classified
    ///     by severity type (Text, Info, Warning, or Error) according to the simulator's test
    ///     output patterns, wrapped in a <see cref="TestResult"/> with pass/fail status.
    /// </returns>
    /// <exception cref="InvalidOperationException">Thrown by implementations when SimulatorPath is null.</exception>
    public abstract TestResult Test(Context context, Options options, string test);

    /// <summary>
    ///     Find the path of a potential application
    /// </summary>
    /// <param name="application">
    ///     Bare executable name without any path prefix or file extension (for example,
    ///     <c>"ghdl"</c> or <c>"nvc"</c>). On Windows, PATHEXT extensions (such as
    ///     <c>.COM</c>, <c>.EXE</c>, <c>.BAT</c>, <c>.CMD</c>) are appended automatically
    ///     when building the candidate file list.
    /// </param>
    /// <returns>Application path or null if not found</returns>
    protected static string? Where(string application)
    {
        // Get the path environment
        var searchPath = Environment.GetEnvironmentVariable("PATH");
        if (searchPath == null)
        {
            return null;
        }

        // Get all the paths and files to search
        var searchPaths = searchPath
            .Split(Path.PathSeparator)
            .Where(IsPathLegal)
            .Select(Path.GetFullPath)
            .Distinct()
            .ToList();
        var searchFiles = new List<string> { application };

        // Handle windows specifics
        if (OperatingSystem.IsWindows())
        {
            // Prepend current directory
            searchPaths.Insert(0, Directory.GetCurrentDirectory());

            // Update the files list considering the executable extensions
            var pathExt = Environment.GetEnvironmentVariable("PATHEXT") ?? ".COM;.EXE;.BAT;.CMD";
            // Path.PathSeparator (';') is valid here: both PATH and PATHEXT use ';' as the
            // list separator on Windows, so reusing Path.PathSeparator is semantically correct.
            var extensions = pathExt.Split(Path.PathSeparator);
            searchFiles = [.. extensions.Select(e => $"{application}{e}")];
        }

        // Search every path and file using SelectMany to combine paths and files
        var result = searchPaths
            .SelectMany(p => searchFiles.Select(f => Path.Combine(p, f)))
            .FirstOrDefault(File.Exists);

        // Return result (null if not found)
        return result;
    }

    /// <summary>
    ///     Test if a path is legal
    /// </summary>
    /// <param name="path">Path to test</param>
    /// <returns>True if legal</returns>
    private static bool IsPathLegal(string path)
    {
        // First check for null or white-space
        if (string.IsNullOrWhiteSpace(path))
        {
            return false;
        }

        // No additional canonicalization is needed here; Path.GetFullPath in the caller will
        // normalize any valid non-whitespace path.
        return true;
    }
}
