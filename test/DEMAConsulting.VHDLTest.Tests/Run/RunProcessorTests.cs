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
using DEMAConsulting.VHDLTest.Run;

namespace DEMAConsulting.VHDLTest.Tests.Run;

/// <summary>
///     Tests for <see cref="RunProcessor"/> class.
/// </summary>
public class RunProcessorTests
{
    /// <summary>
    ///     Verifies that attempting to execute a non-existent program raises an exception.
    /// </summary>
    [Fact]
    public void RunProcessor_Execute_MissingProgram_ThrowsException()
    {
        // Arrange: create a processor with a single error-matching rule
        var processor = new RunProcessor(
        [
            RunLineRule.Create(RunLineType.Error, "Error")
        ]);

        // Act / Assert: executing an unknown program must throw
        Assert.ThrowsAny<Exception>(() => processor.Execute("unknown-program"));
    }

    /// <summary>
    ///     Verifies that <c>Execute(Context, ...)</c> on Windows throws a <see cref="System.ComponentModel.Win32Exception"/>
    ///     for a missing program, matching the already-verified behavior of the direct
    ///     <see cref="RunProcessor.Execute(string, string, string[])"/> overload
    ///     (<see cref="RunProcessor_Execute_MissingProgram_ThrowsException"/>). Before this fix,
    ///     the Windows <c>cmd /c</c> wrapping silently swallowed a missing program into a
    ///     non-throwing, non-zero-exit <see cref="RunResults"/> instead of throwing.
    /// </summary>
    [Fact]
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public void RunProcessor_Execute_WithContext_MissingProgram_ThrowsWin32ExceptionConsistently()
    {
        // Skip this test on non-Windows platforms — the non-Windows path already throws
        // directly and is already covered by RunProcessor_Execute_MissingProgram_ThrowsException
        if (!OperatingSystem.IsWindows())
        {
            Assert.Skip("This test only applies to Windows");
        }

        // Arrange
        var processor = new RunProcessor(
        [
            RunLineRule.Create(RunLineType.Error, "Error")
        ]);
        using var context = Context.Create(["--silent"]);

        // Act / Assert: a missing program must throw Win32Exception, not swallow the failure
        // into a non-zero-exit RunResults. NativeErrorCode must be set to the standard
        // ERROR_FILE_NOT_FOUND (2) rather than left at its default 0, so callers/logging that
        // rely on the error code see a semantically correct value.
        var exception = Assert.Throws<System.ComponentModel.Win32Exception>(
            () => processor.Execute(context, "definitely-not-a-real-program-xyz"));
        Assert.Equal(2, exception.NativeErrorCode);
    }

    /// <summary>
    ///     Verifies that the Windows pre-flight executable resolution step does not append
    ///     further <c>PATHEXT</c> extensions to an application name that already has an
    ///     extension of its own — regression guard for a bug where an explicitly
    ///     extension-qualified name (e.g. <c>tool.exe</c>) could incorrectly resolve to an
    ///     unrelated file such as <c>tool.exe.cmd</c>, which does not match <c>cmd.exe</c>
    ///     resolution semantics for an extension-qualified name.
    /// </summary>
    [Fact]
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public void RunProcessor_Execute_WithContext_ExtensionQualifiedNameNotFound_DoesNotMatchDoubleExtensionFile()
    {
        // Skip this test on non-Windows platforms — the resolution step only runs on Windows
        if (!OperatingSystem.IsWindows())
        {
            Assert.Skip("This test only applies to Windows");
        }

        // Arrange: create a scratch directory containing only a "<program>.exe.cmd" file (never
        // a "<program>.exe" file), then request execution of the extension-qualified
        // "<program>.exe" name. Real cmd.exe resolution would never match "<program>.exe.cmd"
        // for a request of "<program>.exe", so resolution must fail and throw Win32Exception.
        var scratchDir = Path.Combine(Path.GetTempPath(), $"rp_ext_test_{Guid.NewGuid():N}");
        var programName = $"rp-test-{Guid.NewGuid():N}";
        Directory.CreateDirectory(scratchDir);
        var decoyPath = Path.Combine(scratchDir, $"{programName}.exe.cmd");
        File.WriteAllText(decoyPath, "@echo Usage: test-ok" + Environment.NewLine + "@exit /b 0" + Environment.NewLine);
        try
        {
            var processor = new RunProcessor(
            [
                RunLineRule.Create(RunLineType.Info, "Usage")
            ]);
            using var context = Context.Create(["--silent"]);

            // Act / Assert: requesting "<program>.exe" must not resolve to the decoy
            // "<program>.exe.cmd" file, so a Win32Exception is thrown
            var exception = Assert.Throws<System.ComponentModel.Win32Exception>(
                () => processor.Execute(context, $"{programName}.exe", scratchDir));
            Assert.Equal(2, exception.NativeErrorCode);
        }
        finally
        {
            Directory.Delete(scratchDir, true);
        }
    }

    /// <summary>
    ///     Verifies that a valid Windows application (e.g. <c>dotnet</c>) is unaffected by the
    ///     new pre-flight executable resolution step added to <c>Execute(Context, ...)</c> —
    ///     regression guard proving the resolution step does not break the existing working path.
    /// </summary>
    [Fact]
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public void RunProcessor_Execute_WithContext_ValidProgram_StillInvokesSuccessfully()
    {
        // Skip this test on non-Windows platforms — the resolution step only runs on Windows
        if (!OperatingSystem.IsWindows())
        {
            Assert.Skip("This test only applies to Windows");
        }

        // Arrange: use a unique per-test temp log file to capture verbose context output
        var logFile = Path.Combine(Path.GetTempPath(), $"rp_valid_test_{Guid.NewGuid():N}.log");
        try
        {
            var processor = new RunProcessor(
            [
                RunLineRule.Create(RunLineType.Info, "Usage")
            ]);
            RunResults results;
            using (var context = Context.Create(["--verbose", "--log", logFile, "--silent"]))
            {
                // Act
                results = processor.Execute(context, "dotnet", "", "help");
            }

            // Assert: the valid program still runs successfully and is still wrapped with cmd /c
            Assert.Equal(0, results.ExitCode);
            var logContent = File.ReadAllText(logFile);
            Assert.Contains("cmd /c", logContent);
        }
        finally
        {
            if (File.Exists(logFile))
            {
                File.Delete(logFile);
            }
        }
    }

    /// <summary>
    ///     Verifies that the Windows pre-flight executable resolution step searches the
    ///     <c>workingDirectory</c> supplied to <c>Execute(Context, ...)</c> rather
    ///     than the current process's working directory — regression guard for a bug where
    ///     resolution used <c>Directory.GetCurrentDirectory()</c> even when a different
    ///     <c>workingDirectory</c> was supplied, causing a valid bare-named executable that only
    ///     exists in that directory to be misreported as missing.
    /// </summary>
    [Fact]
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public void RunProcessor_Execute_WithContext_BareNameOnlyInWorkingDirectory_ResolvesSuccessfully()
    {
        // Skip this test on non-Windows platforms — the resolution step only runs on Windows
        if (!OperatingSystem.IsWindows())
        {
            Assert.Skip("This test only applies to Windows");
        }

        // Arrange: create a scratch directory (distinct from the current process directory)
        // containing a batch file with a unique bare name, so resolution only succeeds when the
        // supplied workingDirectory (not the current directory) is searched
        var scratchDir = Path.Combine(Path.GetTempPath(), $"rp_workdir_test_{Guid.NewGuid():N}");
        var programName = $"rp-test-{Guid.NewGuid():N}";
        Directory.CreateDirectory(scratchDir);
        var scriptPath = Path.Combine(scratchDir, $"{programName}.cmd");
        File.WriteAllText(scriptPath, "@echo Usage: test-ok" + Environment.NewLine + "@exit /b 0" + Environment.NewLine);
        try
        {
            var processor = new RunProcessor(
            [
                RunLineRule.Create(RunLineType.Info, "Usage")
            ]);
            using var context = Context.Create(["--silent"]);

            // Act: execute the bare program name with the scratch directory as workingDirectory
            var results = processor.Execute(context, programName, scratchDir);

            // Assert: resolution succeeded against workingDirectory (no Win32Exception thrown)
            // and the process actually ran
            Assert.Equal(0, results.ExitCode);
        }
        finally
        {
            Directory.Delete(scratchDir, true);
        }
    }

    /// <summary>
    ///     Verifies that the Windows pre-flight executable resolution step resolves a relative
    ///     path containing a directory component (e.g. <c>subdir\tool.cmd</c>) against the
    ///     supplied <c>workingDirectory</c> rather than the current process's working directory
    ///     — regression guard for a bug where the directory-component branch of
    ///     <c>TryResolveWindowsExecutable</c> always tested the relative path against the current
    ///     process directory, causing a valid relative path (resolvable when <c>cmd /c</c> later
    ///     launches from <c>workingDirectory</c>) to be misreported as missing.
    /// </summary>
    [Fact]
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public void RunProcessor_Execute_WithContext_RelativeSubdirectoryPathInWorkingDirectory_ResolvesSuccessfully()
    {
        // Skip this test on non-Windows platforms — the resolution step only runs on Windows
        if (!OperatingSystem.IsWindows())
        {
            Assert.Skip("This test only applies to Windows");
        }

        // Arrange: create a scratch directory (distinct from the current process directory)
        // with a subdirectory containing a batch file, referenced by a relative path with a
        // directory component, so resolution only succeeds when the supplied workingDirectory
        // (not the current directory) is used as the base for the relative path
        var scratchDir = Path.Combine(Path.GetTempPath(), $"rp_workdir_subdir_test_{Guid.NewGuid():N}");
        var subDir = Path.Combine(scratchDir, "sub");
        var programName = $"rp-test-{Guid.NewGuid():N}";
        Directory.CreateDirectory(subDir);
        var scriptPath = Path.Combine(subDir, $"{programName}.cmd");
        File.WriteAllText(scriptPath, "@echo Usage: test-ok" + Environment.NewLine + "@exit /b 0" + Environment.NewLine);
        try
        {
            var processor = new RunProcessor(
            [
                RunLineRule.Create(RunLineType.Info, "Usage")
            ]);
            using var context = Context.Create(["--silent"]);
            var relativeApplication = Path.Combine("sub", $"{programName}.cmd");

            // Act: execute the relative directory-qualified path with the scratch directory as
            // workingDirectory
            var results = processor.Execute(context, relativeApplication, scratchDir);

            // Assert: resolution succeeded against workingDirectory (no Win32Exception thrown)
            // and the process actually ran
            Assert.Equal(0, results.ExitCode);
        }
        finally
        {
            Directory.Delete(scratchDir, true);
        }
    }

    /// <summary>
    ///     Verifies that mutating the caller's original rules array after constructing a
    ///     <see cref="RunProcessor"/> does not affect the instance's classification behavior,
    ///     proving <see cref="RunProcessor"/> holds a defensive copy rather than the caller's
    ///     array reference.
    /// </summary>
    [Fact]
    public void RunProcessor_Constructor_MutatingOriginalRulesArrayAfterConstruction_DoesNotAffectClassification()
    {
        // Arrange: build a rules array with one rule matching "Error", construct the processor,
        // then mutate the original array element to a rule that would never match "Error"
        var rules = new[] { RunLineRule.Create(RunLineType.Error, "Error") };
        var processor = new RunProcessor(rules);
        rules[0] = RunLineRule.Create(RunLineType.Warning, "NeverMatches");

        // Act: parse output that would only match the original ("Error") rule
        var result = processor.Parse(DateTime.Now, DateTime.Now, "** Error: something went wrong", 0);

        // Assert: the original classification still applies — the instance is unaffected by
        // post-construction mutation of the caller's array
        Assert.Multiple(
            () => Assert.Equal(RunLineType.Error, result.Summary),
            () => Assert.Contains(result.Lines, l => l.Type == RunLineType.Error));
    }

    /// <summary>
    ///     Verifies that constructing a <see cref="RunProcessor"/> with a null rules array
    ///     throws <see cref="ArgumentNullException"/> rather than a less-clear exception from
    ///     the defensive-copy collection expression.
    /// </summary>
    [Fact]
    public void RunProcessor_Constructor_NullRules_ThrowsArgumentNullException()
    {
        // Act / Assert
        Assert.Throws<ArgumentNullException>(() => new RunProcessor(null!));
    }

    /// <summary>
    ///     Verifies that an output line matching an error rule is classified as Error
    ///     when the exit code is zero, isolating pattern-classification from exit-code escalation.
    /// </summary>
    [Fact]
    public void RunProcessor_Parse_OutputWithErrorPattern_ReturnsErrorClassification()
    {
        // Arrange: create a processor with an error rule and synthetic output with zero exit code
        var processor = new RunProcessor(
        [
            RunLineRule.Create(RunLineType.Error, "Error:")
        ]);

        // Act: parse output containing the error pattern with exit code 0 to isolate pattern classification
        var result = processor.Parse(DateTime.Now, DateTime.Now, "** Error: something went wrong", 0);

        // Assert: the matched line drives the summary to Error even with a zero exit code
        Assert.Multiple(
            () => Assert.Equal(RunLineType.Error, result.Summary),
            () => Assert.Contains(result.Lines, l => l.Type == RunLineType.Error));
    }

    /// <summary>
    ///     Verifies that output containing an error pattern produces an Error summary.
    /// </summary>
    [Fact]
    public void RunProcessor_Execute_ProgramWithError_ReturnsErrorResult()
    {
        // Arrange: create a processor with an error rule matching "Error"
        var processor = new RunProcessor(
        [
            RunLineRule.Create(RunLineType.Error, "Error")
        ]);

        // Act: run dotnet with an unknown command, which produces error output
        var result = processor.Execute("dotnet", "", "unknown-command");

        // Assert: the summary is Error; both pattern matching and exit-code escalation
        // can contribute — this test verifies the combined outcome
        Assert.Equal(RunLineType.Error, result.Summary);
    }

    /// <summary>
    ///     Verifies that output containing an info pattern produces an Info summary.
    /// </summary>
    [Fact]
    public void RunProcessor_Execute_ProgramWithSuccess_ReturnsInfoResult()
    {
        // Arrange: create a processor with an info rule matching "Usage"
        var processor = new RunProcessor(
        [
            RunLineRule.Create(RunLineType.Info, "Usage")
        ]);

        // Act: run dotnet help, which outputs usage information
        var result = processor.Execute("dotnet", "", "help");

        // Assert: individual lines are classified as Info and the summary reflects that
        Assert.Multiple(
            () => Assert.Equal(RunLineType.Info, result.Summary),
            () => Assert.Contains(result.Lines, l => l.Type == RunLineType.Info));
    }

    /// <summary>
    ///     Verifies that a non-zero exit code forces the summary to at least Error even when
    ///     no error pattern is present in the output.
    /// </summary>
    [Fact]
    public void RunProcessor_Execute_ProgramWithNonZeroExitCode_ReturnsErrorResult()
    {
        // Arrange: create a processor with no rules so no line will match an error pattern
        var processor = new RunProcessor([]);

        // Act: run dotnet with an unknown command, which exits non-zero
        var result = processor.Execute("dotnet", "", "unknown-command");

        // Assert: the non-zero exit code elevates the summary to Error regardless of patterns
        Assert.True(result.Summary >= RunLineType.Error);
    }

    /// <summary>
    ///     Verifies that an output line matching a Warning rule is classified as Warning
    ///     and that the summary reflects the highest severity.
    /// </summary>
    [Fact]
    public void RunProcessor_Parse_OutputWithWarningPattern_ReturnsWarningSummary()
    {
        // Arrange: create a processor with a Warning rule and fixed output
        var processor = new RunProcessor(
        [
            RunLineRule.Create(RunLineType.Warning, "warning")
        ]);

        // Act: parse output containing the warning keyword
        var result = processor.Parse(DateTime.Now, DateTime.Now, "this is a warning line", 0);

        // Assert: the matched line is Warning and the summary reflects that
        Assert.Multiple(
            () => Assert.Equal(RunLineType.Warning, result.Summary),
            () => Assert.Contains(result.Lines, l => l.Type == RunLineType.Warning));
    }

    /// <summary>
    ///     Verifies that an output line matching no rule is classified as Text.
    /// </summary>
    [Fact]
    public void RunProcessor_Parse_OutputWithNoMatchingPattern_ReturnsTextClassification()
    {
        // Arrange: create a processor with an Info rule that will not match the output
        var processor = new RunProcessor(
        [
            RunLineRule.Create(RunLineType.Info, "info")
        ]);

        // Act: parse output that contains no matching keyword
        var result = processor.Parse(DateTime.Now, DateTime.Now, "this line does not match", 0);

        // Assert: the unmatched line is classified as Text and the summary is Text
        Assert.Multiple(
            () => Assert.Equal(RunLineType.Text, result.Summary),
            () => Assert.Contains(result.Lines, l => l.Type == RunLineType.Text));
    }

    /// <summary>
    ///     Verifies that <c>Execute(Context, ...)</c> writes verbose log entries for the working
    ///     directory and command to the context before executing the program.
    /// </summary>
    [Fact]
    public void RunProcessor_Execute_WithContext_LogsRunInfo()
    {
        // Arrange: use a unique per-test temp log file to capture verbose context output
        var logFile = Path.Combine(Path.GetTempPath(), $"rp_test_{Guid.NewGuid():N}.log");
        try
        {
            var processor = new RunProcessor(
            [
                RunLineRule.Create(RunLineType.Info, "Usage")
            ]);
            RunResults results;
            using (var context = Context.Create(["--verbose", "--log", logFile, "--silent"]))
            {
                // Act: invoke the Execute overload that accepts a Context
                results = processor.Execute(context, "dotnet", "", "help");
            }

            // Assert: results are valid
            Assert.NotNull(results);
            Assert.Equal(0, results.ExitCode);

            // Assert: context captured the verbose log lines written before execution
            var logContent = File.ReadAllText(logFile);
            Assert.Multiple(
                () => Assert.Contains("Run Directory", logContent),
                () => Assert.Contains("Run Command", logContent));
        }
        finally
        {
            if (File.Exists(logFile))
            {
                File.Delete(logFile);
            }
        }
    }

    /// <summary>
    ///     Verifies that <c>Execute(Context, ...)</c> writes a verbose log entry for the run
    ///     command (including the application name) independently of the working-directory entry.
    /// </summary>
    [Fact]
    public void RunProcessor_Execute_WithVerboseContext_LogsCommand()
    {
        // Arrange: use a unique per-test temp log file to capture verbose context output
        var logFile = Path.Combine(Path.GetTempPath(), $"rp_cmd_test_{Guid.NewGuid():N}.log");
        try
        {
            var processor = new RunProcessor(
            [
                RunLineRule.Create(RunLineType.Info, "Usage")
            ]);
            using (var context = Context.Create(["--verbose", "--log", logFile, "--silent"]))
            {
                // Act: invoke Execute(Context, ...) with a known application
                processor.Execute(context, "dotnet", "", "help");
            }

            // Assert: the verbose log contains the "Run Command" entry with the application name
            var logContent = File.ReadAllText(logFile);
            Assert.Multiple(
                () => Assert.Contains("Run Command", logContent),
                () => Assert.Contains("dotnet", logContent));
        }
        finally
        {
            if (File.Exists(logFile))
            {
                File.Delete(logFile);
            }
        }
    }

    /// <summary>
    ///     Verifies that RunProcessor.Parse elevates the RunResults Summary to at least
    ///     RunLineType.Error when the exit code is non-zero and no rule matches the output,
    ///     confirming the SummaryElevation invariant is enforced independently of pattern matching.
    /// </summary>
    [Fact]
    public void RunProcessor_Parse_WithNonZeroExitCode_ElevatesSummaryToAtLeastError()
    {
        // Arrange: create a processor with no rules so no line elevates the summary via pattern
        // matching; use fixed timestamps so the duration calculation is deterministic
        var processor = new RunProcessor([]);
        var start = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Local);
        var end = new DateTime(2024, 1, 1, 0, 0, 1, DateTimeKind.Local);

        // Act: parse output with exit code 1 — the non-zero exit code must escalate the summary
        // even though no error pattern exists in the rule set
        var results = processor.Parse(start, end, "no error pattern present", exitCode: 1);

        // Assert: summary is at least Error solely due to the non-zero exit code
        Assert.True(results.Summary >= RunLineType.Error,
            $"Expected Summary >= RunLineType.Error for non-zero ExitCode, but got {results.Summary}");
    }

    /// <summary>
    ///     Verifies that <c>Execute(Context, ...)</c> wraps the command with <c>cmd /c</c> on
    ///     Windows so that batch files and other shell-dispatched programs can be launched
    ///     without requiring the caller to know about the Windows shell requirement.
    /// </summary>
    [Fact]
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public void RunProcessor_Execute_WithContext_OnWindows_WrapsCommandWithCmdSlashC()
    {
        // Skip this test on non-Windows platforms — the cmd /c wrapping only occurs on Windows
        if (!OperatingSystem.IsWindows())
        {
            Assert.Skip("This test only applies to Windows");
        }

        // Arrange: use a unique per-test temp log file to capture the verbose command log
        var logFile = Path.Combine(Path.GetTempPath(), $"rp_win_test_{Guid.NewGuid():N}.log");
        try
        {
            var processor = new RunProcessor(
            [
                RunLineRule.Create(RunLineType.Info, "Usage")
            ]);
            using (var context = Context.Create(["--verbose", "--log", logFile, "--silent"]))
            {
                // Act: invoke Execute(Context, ...) with a known-good application; on Windows
                // the implementation wraps the command with cmd /c before launching
                processor.Execute(context, "dotnet", "", "help");
            }

            // Assert: the verbose log contains the cmd /c wrapper added on Windows
            var logContent = File.ReadAllText(logFile);
            Assert.Contains("cmd /c", logContent);
        }
        finally
        {
            if (File.Exists(logFile))
            {
                File.Delete(logFile);
            }
        }
    }
}
