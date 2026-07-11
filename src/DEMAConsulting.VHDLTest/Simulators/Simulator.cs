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
    ///     Gets the name of the simulator.
    /// </summary>
    /// <value>
    ///     The display name of this simulator instance (for example, "GHDL" or "NVC"). This
    ///     value is guaranteed non-null; it is set from the constructor's
    ///     <c>simulatorName</c> parameter at initialization time.
    /// </value>
    public string SimulatorName => simulatorName;

    /// <summary>
    ///     Gets the path to the simulator installation directory.
    /// </summary>
    /// <value>
    ///     The path to the simulator executable or directory, or <c>null</c> if the simulator
    ///     is not installed.
    /// </value>
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
    ///     Compiles all VHDL source files listed in the run options.
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
    ///     Executes a single named VHDL test bench.
    /// </summary>
    /// <param name="context">
    ///     Execution context used for logging output and error reporting. Must not be null.
    /// </param>
    /// <param name="options">
    ///     Parsed command-line options providing working directory, configuration, and verbosity
    ///     settings. Must not be null.
    /// </param>
    /// <param name="test">
    ///     VHDL entity name to simulate. May be a simple name (e.g., <c>"my_tb"</c>) or a
    ///     library-qualified name (e.g., <c>"lib.my_tb"</c>). Must not be null or empty. How the
    ///     name is passed to the underlying simulator varies by implementation: <see cref="ModelSimSimulator"/>,
    ///     <see cref="ActiveHdlSimulator"/>, and <see cref="QuestaSimSimulator"/> quote the name via
    ///     <see cref="TclText.Quote"/> before interpolating it into a TCL script, so it may safely
    ///     contain whitespace or TCL metacharacters; <see cref="GhdlSimulator"/> and
    ///     <see cref="NvcSimulator"/> pass the name as a separate process argument;
    ///     <see cref="VivadoSimulator"/> quotes the name via <see cref="XilinxArgText.Quote"/>
    ///     before interpolating it into a Xilinx argument file (not a TCL script), so it may
    ///     safely contain whitespace or characters requiring escaping.
    /// </param>
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
    /// <remarks>
    ///     Several design decisions are embedded in this method:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <b>Windows current-directory prepend</b>: On Windows, the current directory is
    ///                 inserted at index 0 of the search path list <em>after</em> deduplication, matching
    ///                 the Windows shell's implicit current-directory search semantics. This insertion
    ///                 occurs after <c>Distinct()</c>, so if the current directory also appears in
    ///                 <c>%PATH%</c> it may be searched twice.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 <b>PATHEXT handling</b>: On Windows, <c>%PATHEXT%</c> is split on
    ///                 <c>Path.PathSeparator</c> (<c>;</c>) — which is correct because both <c>PATH</c>
    ///                 and <c>PATHEXT</c> use <c>;</c> as the list separator on Windows — and each
    ///                 extension is appended to the bare application name to build the candidate file
    ///                 list. When <c>%PATHEXT%</c> is not set, the default
    ///                 <c>.COM;.EXE;.BAT;.CMD</c> is used.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 <b>Null-PATH early return</b>: When the <c>PATH</c> environment variable is
    ///                 absent, the method returns null immediately rather than searching an empty list,
    ///                 because no useful search can be performed without a PATH.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 <b>Thread safety</b>: The method reads environment variables and the file system
    ///                 but does not modify any shared state. It is safe to call concurrently from
    ///                 multiple threads, subject to the usual caveats about environment-variable
    ///                 mutation by other threads.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </remarks>
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
    /// <param name="path">
    ///     Path to test. Accepts <c>null</c> — a null or white-space path is treated as illegal.
    ///     The parameter is typed as <c>string?</c> so that this method can be used directly as a
    ///     LINQ <c>Where</c> predicate against nullable sequences without a compiler warning.
    /// </param>
    /// <returns>True if legal</returns>
    private static bool IsPathLegal(string? path)
    {
        // Defensive null check: PATH entries from string.Split are non-null, but the nullable
        // parameter type keeps this method usable against any nullable string sequence without
        // requiring the caller to pre-filter nulls.
        if (string.IsNullOrWhiteSpace(path))
        {
            return false;
        }

        // No additional canonicalization is needed here; Path.GetFullPath in the caller will
        // normalize any valid non-whitespace path.
        return true;
    }
}
