using DEMAConsulting.VHDLTest.Results;
using DEMAConsulting.VHDLTest.Run;

namespace DEMAConsulting.VHDLTest.Simulators;

/// <summary>
///     Simulator Interface
/// </summary>
public abstract class Simulator
{
    /// <summary>
    ///     Initializes a new instance of the Simulator class
    /// </summary>
    /// <param name="name">Simulator name</param>
    /// <param name="path">Simulator path</param>
    protected Simulator(string name, string? path)
    {
        SimulatorName = name;
        SimulatorPath = path;
    }

    /// <summary>
    ///     Gets the name of the simulator
    /// </summary>
    public string SimulatorName { get; init; }

    /// <summary>
    ///     Gets the path to the simulator
    /// </summary>
    public string? SimulatorPath { get; init; }

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
    /// <param name="options">Options</param>
    /// <returns>Compile Results</returns>
    public abstract RunResults Compile(Options options);

    /// <summary>
    ///     Execute a test
    /// </summary>
    /// <param name="options">Options</param>
    /// <param name="test">Test name</param>
    /// <returns>Test Results</returns>
    public abstract TestResult Test(Options options, string test);

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
        var searchFiles = new List<string> {application};

        // Handle windows specifics
        if (OperatingSystem.IsWindows())
        {
            // Prepend current directory
            searchPaths.Insert(0, Directory.GetCurrentDirectory());

            // Update the files list considering the executable extensions
            var pathExt = Environment.GetEnvironmentVariable("PATHEXT") ?? ".COM;.EXE;.BAT;.CMD";
            var extensions = pathExt.Split(Path.PathSeparator);
            searchFiles = extensions.Select(e => $"{application}{e}").ToList();
        }

        // Search every path and file
        foreach (var p in searchPaths)
        foreach (var f in searchFiles)
        {
            var fullName = Path.Combine(p, f);
            if (File.Exists(fullName))
                return fullName;
        }

        // Not found
        return null;
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