using System.Collections.ObjectModel;
using System.Diagnostics;

namespace DEMAConsulting.VHDLTest.Run;

/// <summary>
///     Run Results Class
/// </summary>
public sealed class RunResults
{
    /// <summary>
    ///     Initializes a new instance of the RunResults class
    /// </summary>
    /// <param name="summary">Result summary</param>
    /// <param name="start">Start time</param>
    /// <param name="duration">Duration</param>
    /// <param name="exitCode">Exit code</param>
    /// <param name="output">Output text</param>
    /// <param name="lines">Result lines</param>
    private RunResults(
        RunLineType summary,
        DateTime start,
        double duration,
        int exitCode,
        string output,
        ReadOnlyCollection<RunLine> lines)
    {
        Summary = summary;
        Start = start;
        Duration = duration;
        ExitCode = exitCode;
        Output = output;
        Lines = lines;
    }

    /// <summary>
    ///     Result classification
    /// </summary>
    public RunLineType Summary { get; init; }

    /// <summary>
    ///     Gets the start time
    /// </summary>
    public DateTime Start { get; init; }

    /// <summary>
    ///     Gets the run duration
    /// </summary>
    public double Duration { get; init; }

    /// <summary>
    ///     Gets the exit code
    /// </summary>
    public int ExitCode { get; init; }

    /// <summary>
    ///     Gets the raw output
    /// </summary>
    public string Output { get; init; }

    /// <summary>
    ///     Gets the result lines
    /// </summary>
    public ReadOnlyCollection<RunLine> Lines { get; init; }

    /// <summary>
    ///     Execute a command and return the run results
    /// </summary>
    /// <param name="rules">Result rules</param>
    /// <param name="application">Application to run</param>
    /// <param name="workingDirectory">Working directory</param>
    /// <param name="arguments">Program arguments</param>
    /// <returns>Run results</returns>
    public static RunResults Execute(
        RunLineRule[] rules,
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
        var duration = (end - start).TotalSeconds;

        // Classify the output lines
        var lines = output
            .Replace("\r\n", "\n")
            .Split('\n')
            .Select(line => new RunLine(
                Array.Find(rules, r => r.Pattern.IsMatch(line))?.Type ?? RunLineType.Text,
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
            new ReadOnlyCollection<RunLine>(lines));
    }

    /// <summary>
    ///     Print the results to the console colorized
    /// </summary>
    public void Print(bool verbose)
    {
        // Write all lines
        var currentColor = Console.ForegroundColor;
        foreach (var line in Lines)
        {
            // Skip text lines unless verbose requested
            if (!verbose && line.Type == RunLineType.Text)
                continue;

            // Pick the desired color
            var newColor = line.Type switch
            {
                RunLineType.Info => ConsoleColor.White,
                RunLineType.Warning => ConsoleColor.Yellow,
                RunLineType.Error => ConsoleColor.Red,
                _ => ConsoleColor.Gray
            };

            // Switch colors if necessary
            if (newColor != currentColor)
                Console.ForegroundColor = currentColor = newColor;

            // Write the line
            Console.WriteLine(line.Text);
        }

        // Reset colors
        Console.ResetColor();
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

        foreach (var argument in arguments) startInfo.ArgumentList.Add(argument);

        // Launch the process
        var p = new Process {StartInfo = startInfo};
        p.Start();

        // Collect all output
        output = p.StandardOutput.ReadToEnd();
        p.WaitForExit();

        // Return the output
        return p.ExitCode;
    }
}