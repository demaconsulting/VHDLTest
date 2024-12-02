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
///     Program Options Class
/// </summary>
/// <param name="WorkingDirectory">Working directory</param>
/// <param name="Config">Configuration options</param>
public record Options(string WorkingDirectory,
    ConfigDocument Config)
{
    /// <summary>
    ///     Parse options from command line arguments
    /// </summary>
    /// <param name="args">Command line arguments</param>
    /// <returns>Options</returns>
    public static Options Parse(Context args)
    {
        // Verify a configuration file was specified
        if (args.ConfigFile == null)
            throw new InvalidOperationException("Configuration file not specified");

        // Read the configuration file
        var config = ConfigDocument.ReadFile(args.ConfigFile);

        // Get the working directory
        var absConfigFile = Path.GetFullPath(args.ConfigFile);
        var workingDir = Path.GetDirectoryName(absConfigFile)
                         ?? throw new InvalidOperationException($"Invalid configuration file {absConfigFile}");

        // Return the new options object
        return new Options(
            workingDir,
            config);
    }
}