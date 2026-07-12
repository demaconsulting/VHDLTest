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
    /// <summary>
    ///     Win32 error code for <c>ERROR_FILE_NOT_FOUND</c>, used as the <c>NativeErrorCode</c>
    ///     of the <see cref="System.ComponentModel.Win32Exception"/> thrown when the Windows
    ///     executable resolution pre-flight cannot find the requested application.
    /// </summary>
    private const int NativeErrorFileNotFound = 2;

    private readonly RunLineRule[] _rules;
    private readonly IProcessInvoker _invoker;

    /// <summary>
    ///     Windows system directories that <c>CreateProcess</c>/<c>cmd.exe</c> always implicitly
    ///     search when resolving an unqualified executable name, regardless of what the
    ///     <c>PATH</c> environment variable contains: the system directory
    ///     (<c>%SystemRoot%\System32</c>, from <see cref="Environment.SystemDirectory"/>) and the
    ///     Windows directory (<c>%SystemRoot%</c>, from
    ///     <see cref="Environment.SpecialFolder.Windows"/>). Evaluated lazily (not at type-load
    ///     time) so tests running on non-Windows platforms never touch these Windows-only APIs.
    /// </summary>
    private static IEnumerable<string> WindowsSystemDirectories
    {
        get
        {
            yield return Environment.SystemDirectory;
            yield return Environment.GetFolderPath(Environment.SpecialFolder.Windows);
        }
    }

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
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="rules"/> is null.</exception>
    public RunProcessor(RunLineRule[] rules, IProcessInvoker? invoker = null)
    {
        ArgumentNullException.ThrowIfNull(rules);

        // Defensively copy the rules array so the documented "immutable after construction"
        // thread-safety invariant holds regardless of whether the caller later mutates the
        // array it passed in.
        _rules = [.. rules];
        _invoker = invoker ?? ProcessInvoker.Instance;
    }
    /// <summary>
    ///     Logs the run directory and command through <paramref name="context"/>, then launches
    ///     <paramref name="application"/> and classifies its output.
    /// </summary>
    /// <remarks>
    ///     On Windows, when this instance uses the production <see cref="ProcessInvoker.Instance"/>
    ///     (i.e. a real process will actually be launched), the executable is first resolved via
    ///     <see cref="TryResolveWindowsExecutable"/> (a <c>PATHEXT</c>-aware search mirroring
    ///     <c>cmd.exe</c>'s own resolution order); if resolution fails, a
    ///     <see cref="System.ComponentModel.Win32Exception"/> is thrown immediately, before any
    ///     process is launched. Resolution is skipped when a test double <see cref="IProcessInvoker"/>
    ///     is supplied, since no real process is spawned in that case and unit tests are not
    ///     required to reference an executable that actually exists on disk. When resolution
    ///     succeeds (or is skipped), the application is wrapped in <c>cmd /c</c> so that batch
    ///     files (.bat/.cmd) are resolved correctly by the command interpreter; the (possibly
    ///     resolved) path is passed directly to <c>ArgumentList</c> rather than being
    ///     shell-quoted by the caller. A display form with quotes is written to the verbose log
    ///     so the log shows a shell-reproducible command. Callers are responsible for ensuring
    ///     that individual arguments do not contain <c>cmd.exe</c> shell metacharacters.
    /// </remarks>
    /// <param name="context">
    ///     Program context used for verbose logging. Must not be null. Verbose lines are written
    ///     only when <c>context.Verbose</c> is true; the value is not otherwise used.
    /// </param>
    /// <param name="application">
    ///     Path or name of the executable or batch file to launch. Must not be pre-quoted; the
    ///     CLR passes the path directly to the OS, which handles spaces natively without shell
    ///     quoting. On Windows the path is resolved (see remarks) then forwarded to <c>cmd /c</c>
    ///     as an argument.
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
    ///     Thrown on Windows when <paramref name="application"/> cannot be resolved to an
    ///     existing executable (see <see cref="TryResolveWindowsExecutable"/>), or when the
    ///     simulator executable cannot be started. On non-Windows platforms, propagated from
    ///     <see cref="IProcessInvoker.Execute"/> instead.
    /// </exception>
    /// <exception cref="System.IO.FileNotFoundException">
    ///     Thrown on non-Windows platforms when the simulator executable path does not exist.
    ///     Propagated from <see cref="IProcessInvoker.Execute"/>.
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
            // Resolve the executable up front only when a real OS process will actually be
            // launched (the production ProcessInvoker). cmd /c does not throw when the wrapped
            // program cannot be found — it merely reports the failure via a non-zero exit code
            // and stderr text — so a missing/misconfigured simulator executable would otherwise
            // be silently swallowed into an ordinary non-zero-exit RunResults instead of
            // throwing, unlike the non-Windows path. Resolving up front and throwing here keeps
            // both platforms consistent. Test doubles (see IProcessInvoker) never spawn a real
            // process, so skipping resolution for them preserves existing unit-test behavior
            // that exercises argument construction without requiring a real executable on disk.
            var resolvedApplication = application;
            if (ReferenceEquals(_invoker, ProcessInvoker.Instance) &&
                !TryResolveWindowsExecutable(application, workingDirectory, out resolvedApplication))
            {
                throw new System.ComponentModel.Win32Exception(
                    NativeErrorFileNotFound,
                    $"The system cannot find the file specified: '{application}'");
            }

            // Batch files (.bat/.cmd) cannot be launched directly; use cmd /c. Pass the resolved
            // application directly to ArgumentList (no manual quoting) — ArgumentList handles
            // quoting paths with spaces automatically. Use a display form with quotes only for
            // the verbose log so the log shows a valid shell-reproducible command.
            var displayApplication = resolvedApplication.Contains(' ') ? $"\"{resolvedApplication}\"" : resolvedApplication;
            context.WriteVerboseLine($"  Run Command: cmd /c {displayApplication} {string.Join(" ", arguments)}");
            var windowsArgs = new[] { "/c", resolvedApplication }.Concat(arguments).ToArray();
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
    ///     Resolves <paramref name="application"/> to an existing executable path using the same
    ///     search order <c>CreateProcess</c>/<c>cmd.exe</c> uses: when <paramref name="application"/>
    ///     contains a directory component, only that directory is searched; otherwise the process's
    ///     actual starting directory (<paramref name="workingDirectory"/>, or the current directory
    ///     when empty) is searched first, then the Windows system directory
    ///     (<c>%SystemRoot%\System32</c>) and the Windows directory (<c>%SystemRoot%</c>) — these
    ///     are always implicitly searched by <c>CreateProcess</c>/<c>cmd.exe</c> regardless of
    ///     <c>PATH</c> contents — then each entry in the <c>PATH</c> environment variable. Within
    ///     each candidate directory, the bare name and each <c>PATHEXT</c>-qualified variant
    ///     (default <c>.COM;.EXE;.BAT;.CMD</c> when <c>PATHEXT</c> is not set) are tried.
    /// </summary>
    /// <param name="application">Application name or path to resolve. Must not be null.</param>
    /// <param name="workingDirectory">
    ///     Working directory the process will actually be started from. Searched in place of the
    ///     current directory so resolution matches the child process's real search context. Pass
    ///     an empty string to search the current process working directory instead.
    /// </param>
    /// <param name="resolved">
    ///     When this method returns true, the resolved, extension-qualified path to the
    ///     executable. When this method returns false, set to <paramref name="application"/>.
    /// </param>
    /// <returns>True when an existing executable was found; otherwise false.</returns>
    private static bool TryResolveWindowsExecutable(string application, string workingDirectory, out string resolved)
    {
        var extensions = (Environment.GetEnvironmentVariable("PATHEXT") ?? ".COM;.EXE;.BAT;.CMD")
            .Split(';', StringSplitOptions.RemoveEmptyEntries);
        var startDirectory = string.IsNullOrEmpty(workingDirectory)
            ? Directory.GetCurrentDirectory()
            : workingDirectory;

        // When application already contains a directory component, search only that directory —
        // resolved against workingDirectory when the path is relative, since that is the
        // directory the child process will actually be started from.
        var directory = Path.GetDirectoryName(application);
        if (!string.IsNullOrEmpty(directory))
        {
            var candidatePath = Path.IsPathRooted(application)
                ? application
                : Path.Combine(startDirectory, application);
            return TryResolveCandidates(candidatePath, extensions, out resolved);
        }

        // No directory component: search the process's actual starting directory first (matching
        // CreateProcess's documented search order, where the current directory is checked before
        // the system directories), then the Windows system directories — these are always
        // implicitly searched by CreateProcess/cmd.exe regardless of what PATH contains, so a
        // tool like "cmd" or "notepad" resolves even in an environment whose PATH omits System32
        // — then each PATH entry.
        if (TryResolveCandidates(Path.Combine(startDirectory, application), extensions, out resolved))
        {
            return true;
        }

        foreach (var systemDirectory in WindowsSystemDirectories)
        {
            if (TryResolveCandidates(Path.Combine(systemDirectory, application), extensions, out resolved))
            {
                return true;
            }
        }

        var pathEntries = (Environment.GetEnvironmentVariable("PATH") ?? string.Empty)
            .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries);
        foreach (var dir in pathEntries)
        {
            if (TryResolveCandidates(Path.Combine(dir, application), extensions, out resolved))
            {
                return true;
            }
        }

        resolved = application;
        return false;
    }

    /// <summary>
    ///     Checks whether <paramref name="basePath"/> (when it already carries an extension) or a
    ///     <paramref name="extensions"/>-qualified variant of it (when it does not) exists as a
    ///     file.
    /// </summary>
    /// <param name="basePath">Candidate path without an extension qualifier applied.</param>
    /// <param name="extensions">Ordered <c>PATHEXT</c>-style extensions (each including the leading dot) to try.</param>
    /// <param name="resolved">The matching path when found; otherwise <paramref name="basePath"/>.</param>
    /// <returns>True when a matching file was found; otherwise false.</returns>
    private static bool TryResolveCandidates(string basePath, string[] extensions, out string resolved)
    {
        // Mirror cmd.exe: when basePath already carries its own extension (e.g. "tool.exe"), it
        // is checked literally and PATHEXT variants are never appended — an already
        // extension-qualified name must not be further qualified into an unintended file such as
        // "tool.exe.cmd".
        if (!string.IsNullOrEmpty(Path.GetExtension(basePath)))
        {
            resolved = basePath;
            return File.Exists(basePath);
        }

        // basePath has no extension of its own: cmd.exe only resolves such bare names via a
        // PATHEXT-qualified variant (default .COM;.EXE;.BAT;.CMD when PATHEXT is not set) — an
        // extensionless file literally named basePath is never treated as an executable match by
        // cmd.exe, even if one happens to exist on disk, so it must not be accepted here either.
        foreach (var ext in extensions)
        {
            var candidate = basePath + ext;
            if (File.Exists(candidate))
            {
                resolved = candidate;
                return true;
            }
        }

        resolved = basePath;
        return false;
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
