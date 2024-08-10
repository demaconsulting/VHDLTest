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

using System.Diagnostics;

namespace DEMAConsulting.VHDLTest.Run;

/// <summary>
/// Class to run a program
/// </summary>
public static class RunProgram
{
    /// <summary>
    ///     Run a Program
    /// </summary>
    /// <param name="output">Output text</param>
    /// <param name="application">Program name</param>
    /// <param name="workingDirectory">Working directory</param>
    /// <param name="arguments">Program arguments</param>
    /// <returns>Program exit code</returns>
    public static int Run(
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
            RedirectStandardError = true,
            WorkingDirectory = workingDirectory
        };

        foreach (var argument in arguments)
            startInfo.ArgumentList.Add(argument);

        // Launch the process
        var p = new Process { StartInfo = startInfo };
        p.Start();

        // Collect all output
        output = p.StandardOutput.ReadToEnd() + p.StandardError.ReadToEnd();
        p.WaitForExit();

        // Return the output
        return p.ExitCode;
    }
}
