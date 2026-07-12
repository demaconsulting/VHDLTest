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

Logs the run directory and command through `context.WriteVerboseLine`. On Windows, when this
instance uses the production `ProcessInvoker.Instance` (i.e. a real process will actually be
launched), `application` is first resolved to an existing executable path via a private
`TryResolveWindowsExecutable` helper — a `PATHEXT`-aware search mirroring `CreateProcess`'s and
`cmd.exe`'s own resolution order (the current directory or the supplied directory first, then
the Windows system directory `%SystemRoot%\System32` and the Windows directory `%SystemRoot%`
— always implicitly searched by `CreateProcess`/`cmd.exe` regardless of `PATH` contents — then
each `PATH` entry; in every directory, an application name that already carries its own
extension is matched literally, while a bare, extensionless name is matched only against each
`PATHEXT`-qualified variant — an extensionless file literally named `application` is never
treated as a match, mirroring `cmd.exe`'s own resolution, which only ever launches a
`PATHEXT`-recognized file for an unqualified command). An application name that already
carries its own extension (e.g. `tool.exe`) is only matched against the literal path — no
further `PATHEXT` variants are appended — so it cannot resolve to an unrelated file such as
`tool.exe.cmd`, matching `cmd.exe`'s own resolution semantics for an extension-qualified name.
If resolution fails, a `Win32Exception` is thrown
immediately, with `NativeErrorCode` set to the standard `ERROR_FILE_NOT_FOUND` (2) so callers
relying on the Win32 error code observe a semantically correct value, before any process is
launched. Resolution — and this pre-flight throw — is skipped when a test double
`IProcessInvoker` is supplied, since no real process is spawned in that case and unit tests
are not required to reference an executable that actually exists on disk; missing-program
behavior in that case is delegated entirely to the supplied invoker. When resolution
succeeds (or is skipped), the (possibly resolved) path is wrapped in `cmd /c` to support
`.bat` and `.cmd` invocation; the path is passed directly to `ArgumentList` so
paths containing spaces are quoted automatically. The verbose log uses a display form
with quotes for readability only. Delegates to the core `Execute(string, string, string[])`
overload and returns its result.

- *Preconditions*: `context` is not null; `application` identifies a reachable
  executable or batch file; individual arguments must not contain `cmd.exe` shell
  metacharacters.
- *Postconditions*: Returns a `RunResults` with all fields populated on success. On
  Windows, when this instance uses the production `ProcessInvoker.Instance`, throws
  `Win32Exception` immediately (without launching any process) when `application` cannot
  be resolved to an existing executable — this now matches the non-Windows path's behavior
  for a missing program, closing a prior inconsistency where `cmd /c` silently swallowed a
  missing program into a non-throwing, non-zero-exit `RunResults` instead of throwing. When
  a test double `IProcessInvoker` is supplied, this pre-flight resolution and throw are
  skipped, and missing-program behavior is delegated to the supplied invoker.

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
Windows, when this instance uses the production `ProcessInvoker.Instance`, `Execute(Context,
...)` throws `Win32Exception` immediately when `application` cannot be resolved to an existing
executable by the pre-flight `TryResolveWindowsExecutable` search — this is now consistent with
the non-Windows path and with the `Execute(string, ...)` overload, both of which already throw
for a missing program. When a test double `IProcessInvoker` is supplied, this pre-flight
resolution and throw are skipped, and missing-program behavior is delegated entirely to the
supplied invoker. If `IProcessInvoker.Execute` cannot launch an already-resolved process (for
example, a permissions failure), the exception propagates to the caller. `RegexMatchTimeoutException`
from rule pattern matching also propagates to the caller unchanged.

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
