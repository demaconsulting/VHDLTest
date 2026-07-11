### Context

![Cli Structure](CliView.svg)

#### Purpose

`Context` is responsible for parsing the raw command-line argument array, exposing typed
properties for each recognized flag and path, providing I/O output methods that write to both
the console and an optional log file, and accumulating an error count that determines the
process exit code.

#### Data Model

**_log**: `StreamWriter?` — Private backing field holding the optional log-file writer opened
when `--log` is specified. Set to `null` after the writer is closed by `Dispose()`.

**Version**: `bool` — True when `--version` or `-v` was passed on the command line.

**Help**: `bool` — True when `--help`, `-h`, or `-?` was passed on the command line.

**Silent**: `bool` — True when `--silent` was passed. Suppresses all console output.

**Verbose**: `bool` — True when `--verbose` was passed. Enables verbose output via
`WriteVerboseLine`.

**ExitZero**: `bool` — True when `--exit-0` or `-0` was passed. Instructs the caller to
suppress non-zero exit codes on test failures.

**Validate**: `bool` — True when `--validate` was passed. Instructs the caller to perform
self-validation.

**Depth**: `int` — Heading depth for validation reports. Defaults to `1`. Set via `--depth`.

**ConfigFile**: `string?` — Path to the YAML configuration file, set via `-c` or `--config`.
Null if not specified.

**ResultsFile**: `string?` — Path to the results output file, set via `-r`, `--result`, or
`--results`. Null if not specified.

**Simulator**: `string?` — Name of the simulator to use, set via `-s` or `--simulator`.
Null if not specified.

**CustomTests**: `IReadOnlyList<string>?` — Optional list of specific test names to run,
collected from positional arguments or after `--`. Null if no tests were specified.

**Errors**: `int` — Running count of errors reported via `WriteError`. Incremented each time
`WriteError` is called.

**ExitCode**: `int` — Computed property: returns `1` if `Errors > 0`, otherwise `0`.

#### Key Methods

**Create**: Parses raw command-line arguments and returns an initialized Context instance.

- *Parameters*: `string[] args` — the process argument array.
- *Returns*: `Context` — a fully initialized context; caller is responsible for disposal.
- *Preconditions*: `args` must not be null.
- *Postconditions*: All recognized flags are parsed into the corresponding properties; any
  unrecognized flags or missing argument values throw `InvalidOperationException`.

Iterates through `args` using an enumerator and switches on each token to set boolean flags or
read the following token as a value argument. The following tokens are accepted:

| Property set    | Accepted tokens                           | Notes                                    |
|-----------------|-------------------------------------------|------------------------------------------|
| `Version`       | `-v`, `--version`                         | Sets `Version = true`                    |
| `Help`          | `-h`, `-?`, `--help`                      | Sets `Help = true`                       |
| `Silent`        | `--silent`                                | Sets `Silent = true`                     |
| `Verbose`       | `--verbose`                               | Sets `Verbose = true`                    |
| `ExitZero`      | `-0`, `--exit-0`                          | Sets `ExitZero = true`                   |
| `Validate`      | `--validate`                              | Sets `Validate = true`                   |
| `Depth`         | `--depth <n>`                             | Integer ≥ 1; throws on invalid value     |
| `_log`          | `-l`, `--log <file>`                      | Opens a `StreamWriter` for the log file  |
| `ConfigFile`    | `-c`, `--config <file>`                   | Sets `ConfigFile` to the next token      |
| `ResultsFile`   | `-r`, `--result`, `--results <file>`      | Sets `ResultsFile` to the next token     |
| `Simulator`     | `-s`, `--simulator <name>`                | Sets `Simulator` to the next token       |
| `CustomTests`   | `--` followed by remaining args           | Collects all remaining tokens            |
| `CustomTests`   | Any positional arg not starting with `-`  | Appended to the custom tests list        |

Unrecognized tokens starting with `-` throw `InvalidOperationException`.

**Note on value-argument token consumption**: `GetArgument()` simply advances the enumerator
to the next token and returns it verbatim. If a value-argument flag such as `--depth` is
immediately followed by another flag token (for example, `--depth --verbose`), `GetArgument()`
returns `"--verbose"` as the depth value, causing the subsequent integer-parse validation to
throw `InvalidOperationException`. This behavior is intentional: no lookahead heuristic is
applied because any such input is unambiguously malformed.

**Dispose**: Releases the log-file writer.

- *Parameters*: none.
- *Returns*: void.
- *Preconditions*: none.
- *Postconditions*: `_log` is flushed, closed, and set to `null`; subsequent I/O methods
  silently skip log output because the null-conditional operator (`_log?.Write`,
  `_log?.WriteLine`) becomes a no-op.

**Write**: Writes colored text to the console (unless silent) and to the log file.

- *Parameters*: `ConsoleColor color` — foreground color; `string text` — text to write without
  a trailing newline.
- *Returns*: void.
- *Preconditions*: none.
- *Postconditions*: Text is written to `Console` with the specified foreground color (reset
  afterward) if `Silent` is false; text is also appended to `_log` if a log file is open.

**WriteVerboseLine**: Writes a line of text only when `Verbose` is true.

- *Parameters*: `string text` — text to write.
- *Returns*: void.
- *Preconditions*: none.
- *Postconditions*: If `Verbose` is false, no output is produced; otherwise behaves as
  `WriteLine`.

**WriteLine**: Writes a line of text to the console (unless silent) and to the log file.

- *Parameters*: `string text` — text to write.
- *Returns*: void.
- *Preconditions*: none.
- *Postconditions*: Text line is written to `Console` if `Silent` is false; line is also
  appended to `_log` if a log file is open.

**WriteError**: Increments `Errors` and writes an error message in red to the console and log.

- *Parameters*: `string? message` — error message; if null, `Errors` is incremented but no
  text is written.
- *Returns*: void.
- *Preconditions*: none.
- *Postconditions*: `Errors` is incremented by one; if `message` is non-null it is written in
  `ConsoleColor.Red` to `Console` as a line (with newline, via `Console.WriteLine`) unless
  `Silent` is true, and appended as a line (with newline, via `_log?.WriteLine`) to `_log`
  if open. This distinguishes `WriteError` from `Write`, which writes without a newline.

#### Error Handling

`Create` throws `ArgumentNullException` if `args` is null (via `ArgumentNullException.ThrowIfNull`).
It throws `InvalidOperationException` for unrecognized option flags (tokens beginning with `-`)
or when a value argument is required but the argument array is exhausted. An invalid value for
`--depth` (non-integer or less than 1) also throws `InvalidOperationException`. All exceptions
from `Create` propagate to `Program.Main`, which catches and reports them.

`WriteError` never throws — a null `message` is silently accepted. The error count accumulated
in `Errors` is used by `Program.Main` to set `Environment.ExitCode` via `Context.ExitCode`.

#### Dependencies

- **System.IO** — `StreamWriter` for optional log-file output.
- **System.Console** — console output and foreground color control.

#### Callers

- **Program** — calls `Context.Create(args)` in `Main` and consumes all Context properties and
  I/O methods in `Run`.
