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

using DEMAConsulting.VHDLTest.Run;
using System.Text;
using DEMAConsulting.VHDLTest.Results;

namespace DEMAConsulting.VHDLTest.Simulators;

/// <summary>
///     NVC Simulator Class
/// </summary>
public sealed class NvcSimulator : Simulator
{
    /// <summary>
    /// Compile processor
    /// </summary>
    public static readonly RunProcessor CompileProcessor = new(
        [
            RunLineRule.Create(RunLineType.Info, ".* Note:"),
            RunLineRule.Create(RunLineType.Warning, ".* Warning:"),
            RunLineRule.Create(RunLineType.Error, ".* Error:"),
            RunLineRule.Create(RunLineType.Error, ".* Failure:"),
            RunLineRule.Create(RunLineType.Error, ".* Fatal:")
        ]
    );

    /// <summary>
    /// Test processor
    /// </summary>
    public static readonly RunProcessor TestProcessor = new(
        [
            RunLineRule.Create(RunLineType.Info, ".* Note:"),
            RunLineRule.Create(RunLineType.Warning, ".* Warning:"),
            RunLineRule.Create(RunLineType.Error, ".* Error:"),
            RunLineRule.Create(RunLineType.Error, ".* Failure:"),
            RunLineRule.Create(RunLineType.Error, ".* Fatal:")
        ]
    );

    /// <summary>
    ///     NVC simulator instance
    /// </summary>
    public static readonly NvcSimulator Instance = new();

    /// <summary>
    ///     Initializes a new instance of the NVC simulator
    /// </summary>
    private NvcSimulator() : base("NVC", FindPath())
    {
    }

    /// <inheritdoc />
    public override RunResults Compile(Options options)
    {
        // Log the start of the compile command
        if (options.Verbose)
            Console.WriteLine("Starting NVC compile...");

        // Fail if we cannot find the simulator
        var simPath = SimulatorPath ??
                      throw new InvalidOperationException("NVC Simulator not available");
        if (options.Verbose)
            Console.WriteLine($"  Simulator Path: {simPath}");

        // Create the library directory
        var libDir = Path.Combine(options.WorkingDirectory, "VHDLTest.out/NVC");
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
        var application = Path.Combine(simPath, "nvc");
        if (options.Verbose)
            Console.WriteLine($"  Run Directory: {options.WorkingDirectory}");
        if (options.Verbose)
            Console.WriteLine($"  Run Command: {application} --std=08 --work=work:VHDLTest.out/NVC/lib -a @VHDLTest.out/NVC/compile.rsp");
        return CompileProcessor.Execute(
            application,
            options.WorkingDirectory,
            "--std=08",
            "--work=work:VHDLTest.out/NVC/lib",
            "-a",
            "@VHDLTest.out/NVC/compile.rsp");
    }

    /// <inheritdoc />
    public override TestResult Test(Options options, string test)
    {
        // Log the start of the compile command
        if (options.Verbose)
            Console.WriteLine($"Starting NVC test {test}...");

        // Fail if we cannot find the simulator
        var simPath = SimulatorPath ??
                      throw new InvalidOperationException("NVC Simulator not available");
        if (options.Verbose)
            Console.WriteLine($"  Simulator Path: {simPath}");

        // Get the library directory
        var libDir = Path.Combine(options.WorkingDirectory, "VHDLTest.out/NVC");
        if (options.Verbose)
            Console.WriteLine($"  Library Directory: {libDir}");

        // Run the test
        var application = Path.Combine(simPath, "nvc");
        if (options.Verbose)
            Console.WriteLine($"  Run Directory: {options.WorkingDirectory}");
        if (options.Verbose)
            Console.WriteLine($"  Run Command: {application} --std=2008 --work=work:VHDLTest.out/NVC/lib -e {test} -r {test}");
        var testRunResults = TestProcessor.Execute(
            application,
            options.WorkingDirectory,
            "--std=2008",
            "--work=work:VHDLTest.out/NVC/lib",
            "-e",
            test,
            "-r",
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
        var simPathEnv = Environment.GetEnvironmentVariable("VHDLTEST_NVC_PATH");
        if (simPathEnv != null)
            return simPathEnv;

        // Find the path to the simulator application
        var simPath = Where("nvc");
        if (simPath == null)
            return null;

        // Return the working directory
        return Path.GetDirectoryName(simPath);
    }
}