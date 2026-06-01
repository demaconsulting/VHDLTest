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

namespace DEMAConsulting.VHDLTest.Run;

/// <summary>
///     Coordinates external simulator execution and classifies captured output using an ordered
///     set of <see cref="RunLineRule"/> patterns. It is the primary integration point between
///     simulator implementations in the Simulators subsystem and the output-processing pipeline.
/// </summary>
/// <remarks>
///     Rules are evaluated in the order they were supplied at construction. The first rule
///     whose pattern matches a given output line wins; remaining rules are not evaluated for
///     that line. Lines that match no rule are assigned <see cref="RunLineType.Text"/>.
///     The overall run summary is the maximum severity across all classified lines; a non-zero
///     process exit code forces the summary to at least <see cref="RunLineType.Error"/>.
///     Thread-safety: the <c>_rules</c> array is immutable after construction, so multiple
///     threads may call <see cref="Execute(string,string,string[])"/> or
///     <see cref="Parse"/> concurrently on the same instance without synchronization,
///     provided each call uses its own process and output string.
/// </remarks>
public class RunProcessor
{
    private readonly RunLineRule[] _rules;
    private readonly IProcessInvoker _invoker;

    /// <summary>
    ///     Initializes a new instance of <see cref="RunProcessor"/> with the specified rules and an optional invoker.
    /// </summary>
    /// <param name="rules">
    ///     Ordered classification rules applied to each captured output line. Rules are evaluated
    ///     in array order; the first matching rule determines the line's <see cref="RunLineType"/>.
    /// </param>
    /// <param name="invoker">
    ///     Process invoker used to launch external processes. When null, defaults to
    ///     <see cref="ProcessInvoker.Instance"/> for production use.
    /// </param>
    public RunProcessor(RunLineRule[] rules, IProcessInvoker? invoker = null)
    {
        _rules = rules;
        _invoker = invoker ?? ProcessInvoker.Instance;
    }
    /// <summary>
    ///     Logs the run directory and command through <paramref name="context"/>, then launches
    ///     <paramref name="application"/> and classifies its output.
    /// </summary>
    /// <remarks>
    ///     On Windows the executable is wrapped in <c>cmd /c</c> so that batch files
    ///     (.bat/.cmd) are resolved correctly by the command interpreter; the application path is
    ///     passed directly to <c>ArgumentList</c> rather than being shell-quoted by the caller.
    ///     A display form with quotes is written to the verbose log so the log shows a
    ///     shell-reproducible command. Callers are responsible for ensuring that individual
    ///     arguments do not contain <c>cmd.exe</c> shell metacharacters.
    /// </remarks>
    /// <param name="context">
    ///     Program context used for verbose logging. Must not be null. Verbose lines are written
    ///     only when <c>context.Verbose</c> is true; the value is not otherwise used.
    /// </param>
    /// <param name="application">
    ///     Path or name of the executable or batch file to launch. Must not be pre-quoted; the
    ///     CLR passes the path directly to the OS, which handles spaces natively without shell
    ///     quoting. On Windows the path is forwarded to <c>cmd /c</c> as an argument.
    /// </param>
    /// <param name="workingDirectory">
    ///     Working directory for the launched process. Pass an empty string to inherit the
    ///     current process working directory.
    /// </param>
    /// <param name="arguments">
    ///     Arguments to pass to the process. Each element must be an individual unquoted value;
    ///     <c>ArgumentList</c> handles quoting values that contain spaces.
    /// </param>
    /// <returns>Run results with all fields populated.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="context"/> is null.</exception>
    /// <exception cref="System.ComponentModel.Win32Exception">
    ///     Thrown on Windows when the simulator executable cannot be found or cannot be started.
    ///     Propagated from <see cref="RunProgram.Run"/>.
    /// </exception>
    /// <exception cref="System.IO.FileNotFoundException">
    ///     Thrown on non-Windows platforms when the simulator executable path does not exist.
    ///     Propagated from <see cref="RunProgram.Run"/>.
    /// </exception>
    public RunResults Execute(
        Context context,
        string application,
        string workingDirectory = "",
        params string[] arguments)
    {
        // Validate the context before dereferencing it for verbose logging
        ArgumentNullException.ThrowIfNull(context);

        // Log the run directory and command
        context.WriteVerboseLine($"  Run Directory: {workingDirectory}");
        if (OperatingSystem.IsWindows())
        {
            // On Windows, batch files (.bat/.cmd) cannot be launched directly; use cmd /c.
            // Pass application directly to ArgumentList (no manual quoting); ArgumentList
            // handles quoting paths with spaces automatically. Use a display form with quotes
            // only for the verbose log so the log shows a valid shell-reproducible command.
            var displayApplication = application.Contains(' ') ? $"\"{application}\"" : application;
            context.WriteVerboseLine($"  Run Command: cmd /c {displayApplication} {string.Join(" ", arguments)}");
            var windowsArgs = new[] { "/c", application }.Concat(arguments).ToArray();
            return Execute("cmd", workingDirectory, windowsArgs);
        }

        context.WriteVerboseLine($"  Run Command: {application} {string.Join(" ", arguments)}");
        return Execute(application, workingDirectory, arguments);
    }

    /// <summary>
    ///     Launches <paramref name="application"/> as an external process, captures its output,
    ///     and classifies each output line against the rule set.
    /// </summary>
    /// <remarks>
    ///     Start and end timestamps are recorded from <c>DateTime.Now</c> (local wall-clock time)
    ///     immediately before process launch and immediately after exit. Using local time is
    ///     intentional for the reporting use case — results are displayed to end users who expect
    ///     local time in run logs. No verbose logging is performed; use the
    ///     <see cref="Execute(Context, string, string, string[])"/> overload when logging is needed.
    /// </remarks>
    /// <param name="application">
    ///     Path or name of the executable to launch. Must not be pre-quoted even if it contains
    ///     spaces; <c>ArgumentList</c> handles quoting automatically.
    /// </param>
    /// <param name="workingDirectory">
    ///     Working directory for the launched process. Pass an empty string to inherit the
    ///     current process working directory.
    /// </param>
    /// <param name="arguments">
    ///     Arguments to pass to the process. Each element must be an individual unquoted value;
    ///     do not pre-quote values that contain spaces.
    /// </param>
    /// <returns>Run results with all fields populated.</returns>
    /// <exception cref="System.ComponentModel.Win32Exception">
    ///     Thrown on Windows when the simulator executable cannot be found or cannot be started.
    ///     Propagated from <see cref="IProcessInvoker.Execute"/>.
    /// </exception>
    /// <exception cref="System.IO.FileNotFoundException">
    ///     Thrown on non-Windows platforms when the simulator executable path does not exist.
    ///     Propagated from <see cref="IProcessInvoker.Execute"/>.
    /// </exception>
    public RunResults Execute(
        string application,
        string workingDirectory = "",
        params string[] arguments)
    {
        // Save the start time using local wall-clock time (DateTime.Now) for reporting purposes
        var start = DateTime.Now;

        // Run the application
        var (exitCode, output) = _invoker.Execute(workingDirectory, application, arguments);

        // Save the end time and calculate the duration
        var end = DateTime.Now;

        // Parse the output
        return Parse(start, end, output, exitCode);
    }

    /// <summary>
    ///     Classifies each line of captured simulator output against the rule set, computes run
    ///     timing, determines overall severity, and returns a <see cref="RunResults"/> record.
    /// </summary>
    /// <remarks>
    ///     CRLF normalization: <c>\r\n</c> sequences in <paramref name="output"/> are replaced
    ///     with <c>\n</c> before splitting so that Windows-style line endings do not produce
    ///     trailing carriage-return characters on classified lines.
    ///
    ///     Summary-escalation algorithm: the initial summary is <c>RunLineType.Error</c> when
    ///     <paramref name="exitCode"/> is non-zero, otherwise <c>RunLineType.Text</c>. The maximum
    ///     <see cref="RunLineType"/> value across all classified lines is computed; if it exceeds
    ///     the initial summary it replaces it. This ensures a non-zero exit code always produces at
    ///     least an Error summary even when no output line matches an error pattern.
    /// </remarks>
    /// <param name="start">
    ///     Wall-clock timestamp recorded immediately before the simulator process was launched.
    ///     Must be earlier than or equal to <paramref name="end"/>.
    /// </param>
    /// <param name="end">
    ///     Wall-clock timestamp recorded immediately after the simulator process exited.
    ///     Must be later than or equal to <paramref name="start"/>.
    /// </param>
    /// <param name="output">
    ///     Combined stdout and stderr text captured from the simulator process. Must not be null;
    ///     may be empty. CRLF sequences are normalized to LF before the string is split into lines.
    /// </param>
    /// <param name="exitCode">
    ///     Exit code returned by the simulator process. A value of zero indicates no process-level
    ///     error; any non-zero value forces the summary to at least <see cref="RunLineType.Error"/>
    ///     regardless of matched output patterns.
    /// </param>
    /// <returns>A fully populated <see cref="RunResults"/> record.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="output"/> is null.</exception>
    /// <exception cref="System.Text.RegularExpressions.RegexMatchTimeoutException">
    ///     Thrown when a <see cref="RunLineRule"/> pattern match exceeds its configured timeout
    ///     during line classification. Propagates to the caller without being caught.
    /// </exception>
    public RunResults Parse(
        DateTime start,
        DateTime end,
        string output,
        int exitCode)
    {
        // Validate output before dereferencing it
        ArgumentNullException.ThrowIfNull(output);

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
        // String.Split always returns at least one element, making this guard defensive
        // rather than strictly necessary; it explicitly documents the intent that the
        // summary-elevation block runs only when there are classified lines to inspect
        if (lines.Length > 0)
        {
            var maxLineType = lines.Max(line => line.Type);
            if (maxLineType > summary)
            {
                summary = maxLineType;
            }
        }

        return new RunResults(
            summary,
            start,
            duration,
            exitCode,
            output,
            lines.AsReadOnly());
    }
}
