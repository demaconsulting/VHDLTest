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

using System.Text;
using DEMAConsulting.VHDLTest.Results;
using DEMAConsulting.VHDLTest.Run;

namespace DEMAConsulting.VHDLTest.Simulators;

/// <summary>
///     GHDL Simulator Class
/// </summary>
public sealed class GhdlSimulator : Simulator
{
    /// <summary>
    /// Compile processor
    /// </summary>
    public static readonly RunProcessor CompileProcessor = new(
        [
            RunLineRule.Create(RunLineType.Warning, @".*:\d+:\d+:warning:"),
            RunLineRule.Create(RunLineType.Error, @".*:\d+:\d+: "),
            RunLineRule.Create(RunLineType.Error, ".*:error:"),
            RunLineRule.Create(RunLineType.Error, ".*: cannot open")
        ]
    );

    /// <summary>
    /// Test processor
    /// </summary>
    public static readonly RunProcessor TestProcessor = new(
        [
            RunLineRule.Create(RunLineType.Info, @".*:\(assertion note\):"),
            RunLineRule.Create(RunLineType.Info, @".*:\(report note\):"),
            RunLineRule.Create(RunLineType.Warning, @".*:\(assertion warning\):"),
            RunLineRule.Create(RunLineType.Warning, @".*:\(report warning\):"),
            RunLineRule.Create(RunLineType.Error, @".*:\(assertion error\):"),
            RunLineRule.Create(RunLineType.Error, @".*:\(report error\):"),
            RunLineRule.Create(RunLineType.Error, @".*:\(assertion failure\):"),
            RunLineRule.Create(RunLineType.Error, @".*:\(report failure\):"),
            RunLineRule.Create(RunLineType.Error, ".*:error:")
        ]
    );

    /// <summary>
    ///     GHDL simulator instance
    /// </summary>
    public static readonly GhdlSimulator Instance = new();

    /// <summary>
    ///     Initializes a new instance of the GHDL simulator
    /// </summary>
    private GhdlSimulator() : base("GHDL", FindPath())
    {
    }

    /// <inheritdoc />
    public override RunResults Compile(Options options)
    {
        // Log the start of the compile command
        if (options.Verbose)
            Console.WriteLine("Starting GHDL compile...");

        // Fail if we cannot find the simulator
        var simPath = SimulatorPath ??
                      throw new InvalidOperationException("GHDL Simulator not available");
        if (options.Verbose)
            Console.WriteLine($"  Simulator Path: {simPath}");

        // Create the library directory
        var libDir = Path.Combine(options.WorkingDirectory, "VHDLTest.out/GHDL");
        if (options.Verbose)
            Console.WriteLine($"  Library Directory: {libDir}");
        if (!Directory.Exists(libDir))
            Directory.CreateDirectory(libDir);

        // Build the batch file
        var writer = new StringBuilder();
        foreach (var file in options.Config.Files)
            writer.AppendLine(file);

        // Write the batch file
        var script = Path.Combine(libDir, "compile.rsp");
        if (options.Verbose)
            Console.WriteLine($"  Script File: {script}");
        File.WriteAllText(script, writer.ToString());

        // Run the GHDL compiler
        var application = Path.Combine(simPath, "ghdl");
        if (options.Verbose)
            Console.WriteLine($"  Run Directory: {options.WorkingDirectory}");
        if (options.Verbose)
            Console.WriteLine($"  Run Command: {application} -a --std=08 --workdir=VHDLTest.out/GHDL @VHDLTest.out/GHDL/compile.rsp");
        return CompileProcessor.Execute(
            application,
            options.WorkingDirectory,
            "-a",
            "--std=08",
            "--workdir=VHDLTest.out/GHDL",
            "@VHDLTest.out/GHDL/compile.rsp");
    }

    /// <inheritdoc />
    public override TestResult Test(Options options, string test)
    {
        // Log the start of the compile command
        if (options.Verbose)
            Console.WriteLine($"Starting GHDL test {test}...");

        // Fail if we cannot find the simulator
        var simPath = SimulatorPath ??
                      throw new InvalidOperationException("GHDL Simulator not available");
        if (options.Verbose)
            Console.WriteLine($"  Simulator Path: {simPath}");

        // Get the library directory
        var libDir = Path.Combine(options.WorkingDirectory, "VHDLTest.out/GHDL");
        if (options.Verbose)
            Console.WriteLine($"  Library Directory: {libDir}");

        // Run the test
        var application = Path.Combine(simPath, "ghdl");
        if (options.Verbose)
            Console.WriteLine($"  Run Directory: {options.WorkingDirectory}");
        if (options.Verbose)
            Console.WriteLine($"  Run Command: {application} -r --std=08 --workdir=VHDLTest.out/GHDL {test}");
        var testRunResults = TestProcessor.Execute(
            application,
            options.WorkingDirectory,
            "-r",
            "--std=08",
            "--workdir=VHDLTest.out/GHDL",
            test);

        // Return the test results
        return new TestResult(
            test,
            test,
            testRunResults);
    }

    /// <summary>
    ///     Find the simulator path
    /// </summary>
    /// <returns>Simulator path or null if not found</returns>
    public static string? FindPath()
    {
        // Look for an environment variable
        var simPathEnv = Environment.GetEnvironmentVariable("VHDLTEST_GHDL_PATH");
        if (simPathEnv != null)
            return simPathEnv;

        // Find the path to the simulator application
        var simPath = Where("ghdl");
        if (simPath == null)
            return null;

        // Return the working directory
        return Path.GetDirectoryName(simPath);
    }
}