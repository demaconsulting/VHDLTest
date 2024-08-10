﻿// Copyright (c) 2023 DEMA Consulting
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

using System.Diagnostics;

namespace DEMAConsulting.VHDLTest.Run;

/// <summary>
/// Run Processor class
/// </summary>
public class RunProcessor
{
    /// <summary>
    /// Run processing rules
    /// </summary>
    private readonly RunLineRule[] _rules;

    /// <summary>
    /// Initialize a new instance of the RunProcessor class
    /// </summary>
    /// <param name="rules">Processing rules</param>
    public RunProcessor(RunLineRule[] rules)
    {
        _rules = rules;
    }

    /// <summary>
    /// Run a program and process the results
    /// </summary>
    /// <param name="application">Program to run</param>
    /// <param name="workingDirectory">Working directory</param>
    /// <param name="arguments">Program arguments</param>
    /// <returns>Run results</returns>
    public RunResults Execute(
        string application,
        string workingDirectory = "",
        params string[] arguments)
    {
        // Save the start time
        var start = DateTime.Now;

        // Run the application
        var exitCode = Run(
            out var output,
            application,
            workingDirectory,
            arguments);

        // Save the end time and calculate the duration
        var end = DateTime.Now;

        // Parse the output
        return Parse(start, end, output, exitCode);
    }

    /// <summary>
    /// Parse the output of a program
    /// </summary>
    /// <param name="start">Start time</param>
    /// <param name="end">End time</param>
    /// <param name="output">Program output</param>
    /// <param name="exitCode">Program exit code</param>
    /// <returns>Run results</returns>
    public RunResults Parse(
        DateTime start,
        DateTime end,
        string output,
        int exitCode)
    {
        // Calculate the duration
        var duration = (end - start).TotalSeconds;

        // Classify the output lines
        var lines = output
            .Replace("\r\n", "\n")
            .Split('\n')
            .Select(line => new RunLine(
                Array.Find(_rules, r => r.Pattern.IsMatch(line))?.Type ?? RunLineType.Text,
                line
            ))
            .ToArray();

        // Calculate the summary type
        var summary = exitCode != 0 ? RunLineType.Error : RunLineType.Text;
        foreach (var type in lines.Select(line => line.Type))
            if (type > summary)
                summary = type;

        return new RunResults(
            summary,
            start,
            duration,
            exitCode,
            output,
            lines);
    }

    /// <summary>
    ///     Run a Program
    /// </summary>
    /// <param name="output">Output text</param>
    /// <param name="application">Program name</param>
    /// <param name="workingDirectory">Working directory</param>
    /// <param name="arguments">Program arguments</param>
    /// <returns>Program exit code</returns>
    private static int Run(
        out string output,
        string application,
        string workingDirectory = "",
        params string[] arguments)
    {
        // Construct the process start information
        var startInfo = new ProcessStartInfo(application)
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            WorkingDirectory = workingDirectory
        };

        foreach (var argument in arguments)
            startInfo.ArgumentList.Add(argument);

        // Launch the process
        var p = new Process { StartInfo = startInfo };
        p.Start();

        // Collect all output
        output = p.StandardOutput.ReadToEnd();
        p.WaitForExit();

        // Return the output
        return p.ExitCode;
    }
}