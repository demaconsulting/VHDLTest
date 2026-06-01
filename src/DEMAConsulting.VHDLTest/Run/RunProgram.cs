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
///     Isolates all <see cref="System.Diagnostics.Process"/> interactions for launching external
///     programs, so that callers depend only on the simple <see cref="Run"/> method signature
///     and are shielded from process-lifecycle and stream-redirection details.
/// </summary>
public static class RunProgram
{
    /// <summary>
    ///     Launches <paramref name="application"/> as an external process with the given
    ///     <paramref name="arguments"/>, captures its combined stdout and stderr, waits for it
    ///     to exit, and returns its exit code.
    /// </summary>
    /// <remarks>
    ///     Stdout and stderr are read concurrently on background tasks before
    ///     <c>WaitForExit()</c> is called. This prevents a
    ///     deadlock that would occur if either output buffer filled while the process was still
    ///     running and no consumer was draining it. Each element of
    ///     <paramref name="arguments"/> is added to <c>ArgumentList</c> individually;
    ///     <c>ArgumentList</c> handles quoting values that contain spaces, so callers must not
    ///     pre-quote arguments.
    ///     This method is not thread-safe with respect to shared process state; callers must
    ///     not invoke it concurrently on the same process instance (each call creates a new
    ///     process, so concurrent calls across different invocations are safe).
    /// </remarks>
    /// <param name="output">
    ///     Receives the combined text from the program's standard output and standard error
    ///     streams, with stdout content followed by stderr content.
    /// </param>
    /// <param name="application">
    ///     Path or name of the executable to launch. Must not be pre-quoted even if it contains
    ///     spaces; the CLR passes the path directly to the OS, which handles spaces natively
    ///     without shell quoting.
    /// </param>
    /// <param name="workingDirectory">
    ///     Working directory for the launched process. Pass an empty string (the default) to
    ///     inherit the caller's current working directory.
    /// </param>
    /// <param name="arguments">
    ///     Arguments to pass to the process. Each element is added individually to
    ///     <c>ArgumentList</c>; do not pre-quote values that contain spaces.
    /// </param>
    /// <returns>The exit code returned by the process.</returns>
    /// <exception cref="System.ComponentModel.Win32Exception">
    ///     Thrown on Windows when the executable is not found or cannot be started.
    /// </exception>
    /// <exception cref="System.IO.FileNotFoundException">
    ///     Thrown on non-Windows platforms when the executable path does not exist.
    /// </exception>
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
        {
            startInfo.ArgumentList.Add(argument);
        }

        // Launch the process and ensure proper disposal
        using var p = new Process { StartInfo = startInfo };
        p.Start();

        // Read both streams asynchronously to prevent deadlock if either buffer fills
        var stdoutTask = p.StandardOutput.ReadToEndAsync();
        var stderrTask = p.StandardError.ReadToEndAsync();
        p.WaitForExit();

        // Collect all output
        output = stdoutTask.GetAwaiter().GetResult() + stderrTask.GetAwaiter().GetResult();

        // Return the exit code
        return p.ExitCode;
    }
}
