## Run

### Overview

The Run subsystem handles the execution of external VHDL simulation programs and the
processing of their output to determine test outcomes. It is bounded to launching
simulator processes, capturing their combined stdout and stderr output, classifying
each output line against a configurable set of regex rules, and returning a structured
result. It contains the following units: RunProcessor, RunProgram, RunResults, RunLine,
RunLineRule, and RunLineType.

### Interfaces

**RunProcessor**: Coordinates simulator execution and output classification for the
Simulators subsystem.

- *Type*: In-process .NET public API.
- *Role*: Provider.
- *Contract*: `Execute(Context, string, string, string[]) → RunResults` runs a simulator
  program with verbose logging via the supplied `Context`;
  `Execute(string, string, string[]) → RunResults` runs a program directly without
  logging; `Parse(DateTime, DateTime, string, int) → RunResults` classifies
  pre-captured output. All overloads return a fully populated `RunResults` record.
- *Constraints*: On Windows the program is launched via `cmd /c` to support `.bat` and
  `.cmd` files; individual arguments must not contain `cmd.exe` shell metacharacters.

**Cli.Context**: I/O channel consumed for verbose logging and colored output display.

- *Type*: In-process .NET public API.
- *Role*: Consumer.
- *Contract*: Consumes `Context.WriteVerboseLine` to log the working directory and
  command before each run; consumes `Context.Write` and `Context.WriteLine` via
  `RunResults.Print` to write color-coded output lines.
- *Constraints*: Only consumed when the `Execute(Context, ...)` overload or
  `RunResults.Print` is called.

### Design

1. A simulator implementation in the Simulators subsystem constructs a `RunProcessor`
   with its `RunLineRule` patterns and calls
   `Execute(context, application, workingDirectory, arguments)`.
2. `RunProcessor.Execute` logs the run directory and command via `Context`, then
   delegates to the internal `Execute(application, workingDirectory, arguments)` overload.
3. `RunProgram.Run` is called, which launches the process using
   `ProcessStartInfo.ArgumentList`, reads stdout and stderr concurrently to prevent
   buffer deadlock, waits for the process to exit, and returns the combined output
   text and exit code.
4. `RunProcessor.Parse` splits the output into lines, applies each `RunLineRule` in
   order, and wraps each line as a `RunLine` with the assigned `RunLineType`.
5. The summary `RunLineType` is the maximum type across all classified lines; a
   non-zero exit code forces the summary to at least `RunLineType.Error`.
6. The completed `RunResults` record is returned to the caller.
