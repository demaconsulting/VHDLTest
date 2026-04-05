// Test what happens when Program.Run is called with no config file
using DEMAConsulting.VHDLTest.Cli;
using DEMAConsulting.VHDLTest;

// Case 1: No config file (should print usage and return non-zero)
using (var context = Context.Create(["--silent"]))
{
    Console.WriteLine($"Before Run: ExitCode={context.ExitCode}, Errors={context.Errors}");
    Program.Run(context);
    Console.WriteLine($"After Run: ExitCode={context.ExitCode}, Errors={context.Errors}");
}

Console.WriteLine();

// Case 2: Version flag (should return zero)
using (var context = Context.Create(["--version", "--silent"]))
{
    Console.WriteLine($"Before Run (version): ExitCode={context.ExitCode}, Errors={context.Errors}");
    Program.Run(context);
    Console.WriteLine($"After Run (version): ExitCode={context.ExitCode}, Errors={context.Errors}");
}

Console.WriteLine();

// Case 3: Help flag (should return zero)
using (var context = Context.Create(["--help", "--silent"]))
{
    Console.WriteLine($"Before Run (help): ExitCode={context.ExitCode}, Errors={context.Errors}");
    Program.Run(context);
    Console.WriteLine($"After Run (help): ExitCode={context.ExitCode}, Errors={context.Errors}");
}
