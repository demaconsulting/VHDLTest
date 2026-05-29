### RunProgram

#### Purpose

`RunProgram` is a static utility class that launches an external process, captures its
combined stdout and stderr output, and returns the exit code. It isolates all
`System.Diagnostics.Process` interactions so that the rest of the Run subsystem depends
only on a simple `Run` method signature.

#### Data Model

N/A — `RunProgram` is a static class with no instance state and no static fields.

#### Key Methods

**`Run(out string output, string application, string workingDirectory, string[] arguments) → int`**

Constructs a `ProcessStartInfo` with `UseShellExecute = false`,
`RedirectStandardOutput = true`, and `RedirectStandardError = true`. Each element of
`arguments` is added to `ArgumentList` individually; `ArgumentList` handles quoting
values that contain spaces, so callers must not pre-quote arguments. Stdout and stderr
are read concurrently using async tasks to prevent deadlock when either output buffer
fills while the process is still running. The method waits for the process to exit,
concatenates the stdout and stderr results into `output`, and returns the exit code.

- *Preconditions*: `application` is a valid path to an executable; no argument in
  `arguments` carries pre-applied shell quoting.
- *Postconditions*: `output` contains the combined stdout and stderr text; the return
  value is the process exit code.

#### Error Handling

No exceptions are caught within `RunProgram`. If the executable is not found or cannot
be started, `Process.Start` throws an exception that propagates to the caller
(`RunProcessor`).

#### Dependencies

- **`System.Diagnostics.Process`** (OTS — .NET Base Class Library) — used for process
  creation, output-stream redirection, and exit-code retrieval.

#### Callers

Called exclusively by `RunProcessor.Execute(string, string, string[])`.
