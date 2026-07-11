### RunProcessor

![Run Structure](RunView.svg)

#### Purpose

`RunProcessor` coordinates the execution of external simulator programs and the
classification of their output. It accepts an ordered set of `RunLineRule` patterns at
construction, applies them to each captured output line, determines the overall run
summary from the highest-severity line type and the process exit code, and returns a
`RunResults` record. It is the primary integration point between simulator
implementations and the output-processing pipeline.

#### Data Model

| Field      | Type              | Description                                                                                                                                                                                                 |
| ---------- | ----------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `_rules`   | `RunLineRule[]`   | Defensive copy (`[.. rules]`) of the constructor's array, taken at construction time (after a null check); immutable for the instance lifetime regardless of later mutation of the caller's original array. |
| `_invoker` | `IProcessInvoker` | Injected invoker; defaults to `ProcessInvoker.Instance` when null.                                                                                                                                          |

#### Key Methods

**`Execute(Context context, string application, string workingDirectory, string[] arguments) → RunResults`**

Logs the run directory and command through `context.WriteVerboseLine`. On Windows,
`application` is first resolved to an existing executable path via a private
`TryResolveWindowsExecutable` helper — a `PATHEXT`-aware search mirroring `cmd.exe`'s own
resolution order (current directory or the supplied directory, then each `PATH` entry;
the bare name and each `PATHEXT`-qualified variant are tried). An application name that
already carries its own extension (e.g. `tool.exe`) is only matched against the literal
path — no further `PATHEXT` variants are appended — so it cannot resolve to an unrelated
file such as `tool.exe.cmd`, matching `cmd.exe`'s own resolution semantics for an
extension-qualified name. If resolution fails, a `Win32Exception` is thrown immediately,
with `NativeErrorCode` set to the standard `ERROR_FILE_NOT_FOUND` (2) so callers relying
on the Win32 error code observe a semantically correct value, before any process is
launched. When resolution
succeeds, the *resolved* (extension-qualified) path is wrapped in `cmd /c` to support
`.bat` and `.cmd` invocation; the resolved path is passed directly to `ArgumentList` so
paths containing spaces are quoted automatically. The verbose log uses a display form
with quotes for readability only. Delegates to the core `Execute(string, string, string[])`
overload and returns its result.

- *Preconditions*: `context` is not null; `application` identifies a reachable
  executable or batch file; individual arguments must not contain `cmd.exe` shell
  metacharacters.
- *Postconditions*: Returns a `RunResults` with all fields populated on success. On
  Windows, throws `Win32Exception` immediately (without launching any process) when
  `application` cannot be resolved to an existing executable — this now matches the
  non-Windows path's behavior for a missing program, closing a prior inconsistency where
  `cmd /c` silently swallowed a missing program into a non-throwing, non-zero-exit
  `RunResults` instead of throwing.

**`Execute(string application, string workingDirectory, string[] arguments) → RunResults`**

Records the start time using `DateTime.Now` (local wall-clock time) immediately before process
launch, calls `_invoker.Execute` to launch the process and capture output, then records the end
time with `DateTime.Now` immediately after exit. Local time is used intentionally — results are
displayed to end users who expect local timestamps in run logs. Calls `Parse` and returns
the result. No logging is performed.

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

`RunProcessor`'s constructor throws `ArgumentNullException` when `rules` is null, before
attempting the defensive copy. No exceptions are otherwise caught within `RunProcessor`. On
Windows, `Execute(Context, ...)` throws `Win32Exception` immediately when `application` cannot
be resolved to an existing executable by the pre-flight `TryResolveWindowsExecutable` search —
this is now consistent with the non-Windows path and with the `Execute(string, ...)` overload,
both of which already throw for a missing program. If `IProcessInvoker.Execute` cannot launch
an already-resolved process (for example, a permissions failure), the exception propagates to
the caller. `RegexMatchTimeoutException` from rule pattern matching also propagates to the
caller unchanged.

#### Dependencies

- **IProcessInvoker** — called to launch the external simulator process via Execute.
- **ProcessInvoker** — default IProcessInvoker used when none is supplied at construction; delegates to RunProgram.
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
