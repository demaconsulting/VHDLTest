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

using DEMAConsulting.VHDLTest.Results;
using DEMAConsulting.VHDLTest.Run;

namespace DEMAConsulting.VHDLTest.Simulators;

/// <summary>
///     Simulator Interface
/// </summary>
/// <param name="simulatorName">Simulator name</param>
/// <param name="simulatorPath">Simulator path</param>
public abstract class Simulator(string simulatorName, string? simulatorPath)
{
    /// <summary>
    ///     Gets the name of the simulator
    /// </summary>
    public string SimulatorName => simulatorName;

    /// <summary>
    ///     Gets the path to the simulator
    /// </summary>
    public string? SimulatorPath => simulatorPath;

    /// <summary>
    ///     Test if the simulator is available
    /// </summary>
    /// <returns>True if available</returns>
    public bool Available()
    {
        return SimulatorPath != null;
    }

    /// <summary>
    ///     Compile the simulator library
    /// </summary>
    /// <param name="context">Program context</param>
    /// <param name="options">Options</param>
    /// <returns>Compile Results</returns>
    public abstract RunResults Compile(Context context, Options options);

    /// <summary>
    ///     Execute a test
    /// </summary>
    /// <param name="context">Program context</param>
    /// <param name="options">Options</param>
    /// <param name="test">Test name</param>
    /// <returns>Test Results</returns>
    public abstract TestResult Test(Context context, Options options, string test);

    /// <summary>
    ///     Find the path of a potential application
    /// </summary>
    /// <param name="application">Application</param>
    /// <returns>Application path or null if not found</returns>
    protected static string? Where(string application)
    {
        // Get the path environment
        var searchPath = Environment.GetEnvironmentVariable("PATH");
        if (searchPath == null)
            return null;

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
    /// Test if a path is legal
    /// </summary>
    /// <param name="path">Path to test</param>
    /// <returns>True if legal</returns>
    private static bool IsPathLegal(string path)
    {
        // First check for null or white-space
        if (string.IsNullOrWhiteSpace(path))
            return false;

        // Consider other sanity-checks
        return true;
    }
}
