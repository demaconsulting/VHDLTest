### RunProcessor

#### Purpose

`RunProcessor` coordinates the execution of external simulator programs and the
classification of their output. It accepts an ordered set of `RunLineRule` patterns at
construction, applies them to each captured output line, determines the overall run
summary from the highest-severity line type and the process exit code, and returns a
`RunResults` record. It is the primary integration point between simulator
implementations and the output-processing pipeline.

#### Data Model

| Field   | Type            | Description                                                          |
| ------- | --------------- | -------------------------------------------------------------------- |
| `rules` | `RunLineRule[]` | Ordered rules injected at construction; immutable for the instance lifetime. |

#### Key Methods

**`Execute(Context context, string application, string workingDirectory, string[] arguments) → RunResults`**

Logs the run directory and command through `context.WriteVerboseLine`. On Windows the
executable is wrapped in `cmd /c` to support `.bat` and `.cmd` invocation; the
application path is passed directly to `ArgumentList` so paths containing spaces are
quoted automatically. The verbose log uses a display form with quotes for readability
only. Delegates to the core `Execute(string, string, string[])` overload and
returns its result.

- *Preconditions*: `context` is not null; `application` identifies a reachable
  executable or batch file; individual arguments must not contain `cmd.exe` shell
  metacharacters.
- *Postconditions*: Returns a `RunResults` with all fields populated.

**`Execute(string application, string workingDirectory, string[] arguments) → RunResults`**

Records the start time, calls `RunProgram.Run` to launch the process and capture
output, records the end time, then calls `Parse` and returns the result. No logging
is performed.

- *Preconditions*: `application` identifies a reachable executable.
- *Postconditions*: Returns a `RunResults` with all fields populated.

**`Parse(DateTime start, DateTime end, string output, int exitCode) → RunResults`**

Normalizes line endings in `output` (`\r\n` → `\n`), splits into lines, and wraps each
in a `RunLine` by applying the `rules` array in order — assigning the type of the first
matching rule or `RunLineType.Text` if no rule matches. Computes duration from `start`
and `end`. Determines the summary as the maximum `RunLineType` across all lines; a
non-zero `exitCode` forces the summary to at least `RunLineType.Error`.

- *Preconditions*: `output` is not null; `start` is earlier than or equal to `end`.
- *Postconditions*: Returns a fully populated `RunResults`.

#### Error Handling

No exceptions are caught within `RunProcessor`. If `RunProgram.Run` cannot launch the
process (for example, the executable is not found), the exception propagates to the
caller. `RegexMatchTimeoutException` from rule pattern matching also propagates to the
caller unchanged.

#### Dependencies

- **RunProgram** — called to launch the external simulator process.
- **RunLine** — instantiated during output parsing for each classified line.
- **RunLineRule** — applied to each output line to determine its `RunLineType`.
- **RunLineType** — used for the default line type, summary computation, and error threshold.
- **RunResults** — the return type of all public methods.
- **Context** (Cli subsystem) — consumed for verbose logging in the
  `Execute(Context, ...)` overload.

#### Callers

Called by simulator implementations in the Simulators subsystem. Each simulator
constructs a `RunProcessor` with its own rule set and calls an `Execute` overload for
each compilation or simulation step.
