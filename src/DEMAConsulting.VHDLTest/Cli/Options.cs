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
///     Immutable value type carrying the fully resolved configuration for a VHDLTest run, derived from the parsed command-line context and the loaded YAML configuration document.
/// </summary>
/// <remarks>
///     Options is constructed exclusively via <see cref="Parse"/>, which validates that a
///     configuration file was specified, loads the YAML document, and resolves the working
///     directory to an absolute path. Callers treat Options as a read-only record after
///     construction.
/// </remarks>
/// <param name="WorkingDirectory">Absolute path to the directory containing the configuration file. Equals <c>Path.GetDirectoryName(Path.GetFullPath(configFile))</c> and is guaranteed to be non-null.</param>
/// <param name="Config">Deserialized YAML configuration document. Guaranteed non-null; populated by <see cref="ConfigDocument.ReadFile"/>.</param>
public record Options(string WorkingDirectory,
    ConfigDocument Config)
{
    /// <summary>
    ///     Parse options from command line arguments
    /// </summary>
    /// <remarks>
    ///     Combines the configuration file path stored in <paramref name="args"/> with the parsed
    ///     YAML content to produce a fully resolved <see cref="Options"/> value. The absolute path
    ///     resolution via <c>Path.GetFullPath</c> ensures that <see cref="WorkingDirectory"/> is
    ///     always an absolute path regardless of the current working directory at call time, so
    ///     downstream units can resolve relative VHDL file paths correctly without depending on
    ///     ambient CWD state. This method is stateless and thread-safe; it does not modify any
    ///     shared state and may be called concurrently on different <paramref name="args"/>
    ///     instances.
    /// </remarks>
    /// <param name="args">
    ///     A fully initialised <see cref="Context"/> from which <see cref="Context.ConfigFile"/>
    ///     is read. Must not be null.
    /// </param>
    /// <returns>A non-null <see cref="Options"/> record with <see cref="WorkingDirectory"/> set to the absolute directory of the configuration file and <see cref="Config"/> populated from the YAML content.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no configuration file is specified in <paramref name="args"/>, or when the configuration file path cannot be resolved to a containing directory.</exception>
    /// <exception cref="FileNotFoundException">Thrown when the specified configuration file does not exist on disk.</exception>
    public static Options Parse(Context args)
    {
        // Verify a configuration file was specified
        if (args.ConfigFile == null)
        {
            throw new InvalidOperationException("Configuration file not specified");
        }

        // Read the configuration file
        var config = ConfigDocument.ReadFile(args.ConfigFile);

        // Get the working directory
        var workingDir = ResolveWorkingDirectory(args.ConfigFile);

        // Return the new options object
        return new Options(
            workingDir,
            config);
    }

    /// <summary>
    ///     Resolves the absolute path of the directory containing the specified configuration file.
    /// </summary>
    /// <remarks>
    ///     This internal helper is extracted from <see cref="Parse"/> to allow direct unit testing
    ///     of the defensive null guard. <c>Path.GetDirectoryName</c> returns null for root paths
    ///     such as <c>/</c> or <c>C:\</c>; throwing <see cref="InvalidOperationException"/> in that
    ///     case ensures <see cref="WorkingDirectory"/> is always a valid, absolute path.
    /// </remarks>
    /// <param name="configFile">Path to the configuration file. Must not be null.</param>
    /// <returns>The absolute directory path containing <paramref name="configFile"/>.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the fully-resolved path of <paramref name="configFile"/> has no parent directory
    ///     (i.e., it is a file-system root such as <c>/</c> or <c>C:\</c>).
    /// </exception>
    internal static string ResolveWorkingDirectory(string configFile)
    {
        var absConfigFile = Path.GetFullPath(configFile);
        return Path.GetDirectoryName(absConfigFile)
               ?? throw new InvalidOperationException($"Invalid configuration file {absConfigFile}");
    }
}
