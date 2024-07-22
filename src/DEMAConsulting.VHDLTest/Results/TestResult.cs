using DEMAConsulting.VHDLTest.Run;

namespace DEMAConsulting.VHDLTest.Results;

/// <summary>
/// Test result class
/// </summary>
public sealed class TestResult
{
    /// <summary>
    ///     Initializes a new instance of the TestResult class
    /// </summary>
    /// <param name="className">Class name</param>
    /// <param name="testName">Test name</param>
    /// <param name="runResults">Test run results</param>
    public TestResult(string className, string testName, RunResults runResults)
    {
        ClassName = className;
        TestName = testName;
        RunResults = runResults;
    }

    /// <summary>
    ///     Class Name
    /// </summary>
    public string ClassName { get; init; }

    /// <summary>
    ///     Test Name
    /// </summary>
    public string TestName { get; init; }

    /// <summary>
    ///     Test ID
    /// </summary>
    public Guid TestId { get; init; } = Guid.NewGuid();

    /// <summary>
    ///     Test Execution ID
    /// </summary>
    public Guid ExecutionId { get; init; } = Guid.NewGuid();

    /// <summary>
    ///     Run Results
    /// </summary>
    public RunResults RunResults { get; init; }

    /// <summary>
    ///     Gets a value indicating whether the test passed
    /// </summary>
    public bool Passed => RunResults.Summary < RunLineType.Error;

    /// <summary>
    ///     Gets a value indicating whether the test failed
    /// </summary>
    public bool Failed => RunResults.Summary >= RunLineType.Error;

    /// <summary>
    ///     Print a summary line to the console
    /// </summary>
    public void PrintSummary()
    {
        // Print the colored summary word
        if (Passed)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Passed");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Failed");
        }

        // Restore default color and print test name and duration
        Console.ResetColor();
        Console.WriteLine($" {TestName} ({RunResults.Duration:F1} seconds)");
    }
}