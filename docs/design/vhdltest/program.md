## Program

### Purpose

`Program.cs` is the application entry point for VHDLTest. It creates a `Context` from the raw
command-line argument array, dispatches to the appropriate handler based on the parsed flags, and
sets the process exit code on return.

### Data Model

**Version**: `string` — The assembly informational version string, read once at startup from
`AssemblyInformationalVersionAttribute` and cached as a static property.

### Key Methods

**Main**: Application entry point invoked by the .NET runtime.

- *Parameters*: `string[] args` — raw command-line argument array from the host environment.
- *Returns*: void; sets `Environment.ExitCode` on normal exit.
- *Preconditions*: none.
- *Postconditions*: `Environment.ExitCode` is 0 on success or non-zero on failure.

Creates a `Context` from `args`, delegates to `Run`, then assigns `context.ExitCode` to
`Environment.ExitCode`. Catches `InvalidOperationException` to print a red error message and exit
with code 1. Catches all other exceptions to print the full exception text in red and re-throw so
the runtime records the unhandled exception.

**Run**: Executes the dispatched operation for a given context.

- *Parameters*: `Context context` — parsed command-line context.
- *Returns*: void; result is communicated through `context.ExitCode` and `context.Errors`.
- *Preconditions*: `context` is not null.
- *Postconditions*: appropriate output has been written to the context; `context.ExitCode` reflects
  success or failure.

Dispatches in this order:

1. If `context.Version` — writes the version string and returns.
2. Writes the version banner.
3. If `context.Help` — calls `PrintUsage` and returns.
4. If `context.Validate` — calls `Validation.Run` and returns.
5. If `context.ConfigFile` is null — writes an error, calls `PrintUsage`, and returns.
6. Otherwise — calls `SimulatorFactory.Get`, `Options.Parse`, `TestResults.Execute`,
   `results.PrintSummary`, and optionally `results.SaveResults`. If `context.ExitZero` is
   false and `results.Fails` is non-empty, `context.WriteError(null)` is called to record a
   failure and cause `context.ExitCode` to return non-zero. Catches
   `InvalidOperationException` and generic exceptions, writing them as errors.

The `[tests]` positional argument — any bare command-line tokens not preceded by a flag, or
any tokens after `--` — is accumulated by `Context.Create` into `Context.CustomTests`. When
`CustomTests` is non-null, `TestResults.Execute` uses it to filter the test benches that are
executed to only those whose names appear in the list; tests present in the configuration file
but absent from `CustomTests` are skipped. When `CustomTests` is null all configured test
benches are executed.

**PrintUsage** (private): Writes the usage/help text to the context output channel.

- *Parameters*: `Context context` — output target.
- *Returns*: void.
- *Preconditions*: `context` is not null.
- *Postconditions*: usage text has been written to the context.

### Error Handling

`Main` catches `InvalidOperationException` (expected operational errors such as missing simulator)
and writes a red error line before exiting with code 1. It catches all other exceptions, writes the
full exception text in red, and re-throws so the .NET runtime records the unhandled exception.

`Run` catches `InvalidOperationException` from the test execution path and reports the message via
`context.WriteError`. It also catches generic `Exception` and writes the full exception text as an
error. Within the test execution path, `results.SaveResults(context.ResultsFile)` is wrapped in its
own inner `try/catch`: if saving results throws an `Exception`, the error message is written to the
context via `context.WriteError` and execution continues — the results-save failure does not
propagate to the outer exception handlers.

### Dependencies

- **Context** — provides parsed command-line flags and I/O channels.
- **Options** — holds the parsed test configuration loaded from the YAML file.
- **SimulatorFactory** — resolves the named simulator instance.
- **TestResults** — executes the VHDL tests and serializes results.
- **Validation** — runs the self-validation tests when `--validate` is specified.

### Callers

N/A - entry point, called by the host environment.
