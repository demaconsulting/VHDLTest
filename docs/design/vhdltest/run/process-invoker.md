### IProcessInvoker / ProcessInvoker

#### Purpose

`IProcessInvoker` abstracts low-level process execution to decouple `RunProcessor`
from `RunProgram`, allowing unit tests to supply a test double instead of launching
real operating-system processes. `ProcessInvoker` is the production implementation that
delegates directly to `RunProgram.Run`.

#### Data Model

**IProcessInvoker**: Interface with a single method `Execute`.

**ProcessInvoker**: Sealed class implementing `IProcessInvoker`.

| Field      | Type             | Description                                               |
| ---------- | ---------------- | --------------------------------------------------------- |
| `Instance` | `ProcessInvoker` | Public static readonly singleton for production use.      |

#### Key Methods

**`IProcessInvoker.Execute(workingDirectory, application, arguments) → (int ExitCode, string Output)`**

Signature: `Execute(string workingDirectory, string application, IReadOnlyList<string> arguments)`

Executes an application and returns its exit code and combined stdout+stderr output.

- *Preconditions*: `application` identifies a reachable executable; arguments do not require
  pre-quoting (each element is passed individually).
- *Result*: Returns the process exit code and combined stdout+stderr output.

**`ProcessInvoker.Execute` (implementation)**

Delegates to `RunProgram.Run` with the supplied parameters. Converts the `IReadOnlyList<string>`
arguments to an array using a spread expression before passing to `RunProgram.Run`.

#### Error Handling

Exceptions from `RunProgram.Run` (for example, `Win32Exception` when the executable is not
found) propagate to the caller unchanged.

#### Dependencies

- **RunProgram** — called by `ProcessInvoker.Execute` to launch the external process.

#### Callers

- **RunProcessor** — holds an `IProcessInvoker` instance injected at construction.
- **Simulator constructors** — pass `ProcessInvoker.Instance` as the default invoker.
- **Test code** — supplies `FakeProcessInvoker` to avoid launching real processes.
