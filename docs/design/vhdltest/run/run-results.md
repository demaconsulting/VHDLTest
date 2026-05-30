### RunResults

#### Purpose

`RunResults` is an immutable record that holds the complete outcome of a single
simulator execution. It carries the exit code, raw output text, classified output
lines, timing information, and a high-level summary classification. It is the primary
return value from `RunProcessor` and the data source for simulator pass/fail decisions
and result serialization.

#### Data Model

| Property   | Type                          | Description                                                |
| ---------- | ----------------------------- | ---------------------------------------------------------- |
| `Summary`  | `RunLineType`                 | Highest-severity line type; at least `Error` if non-zero. The SummaryElevation invariant (Summary ≥ Error when ExitCode ≠ 0) is enforced by `RunProcessor.Parse` at construction time, not by `RunResults` itself. |
| `Start`    | `DateTime`                    | Timestamp recorded before `RunProgram.Run` is called.      |
| `Duration` | `double`                      | Elapsed time in seconds between `Start` and process exit.  |
| `ExitCode` | `int`                         | Raw process exit code returned by the simulator.           |
| `Output`   | `string`                      | Full combined stdout and stderr text, unmodified.          |
| `Lines`    | `ReadOnlyCollection<RunLine>` | Classified output lines from `RunProcessor.Parse`.         |

#### Key Methods

**`Print(Context context)`**

Iterates over `Lines` and writes each line to the console using a color determined by
its `RunLineType`: `Info` → white, `Warning` → yellow, `Error` → red, `Text` → gray.
Any unrecognized `RunLineType` value also maps to gray.
When `context.Verbose` is false, lines with type `Text` are suppressed.
Each line is written as two separate calls: `context.Write(color, line.Text)` for the
colored text followed by `context.WriteLine("")` without a color argument; the newline is
emitted without color to prevent console color from bleeding into the line separator.

- *Preconditions*: `context` is not null.
- *Postconditions*: Relevant lines are written to the `Context` output channels.

#### Error Handling

N/A — `RunResults` is an immutable record with no fallible operations in its own code.
Any exceptions from `Context.Write` or `Context.WriteLine` inside `Print` propagate to
the caller.

#### Dependencies

- **RunLine** — elements of the `Lines` collection.
- **RunLineType** — type of each line and of `Summary`; used in color selection within
  `Print`.
- **Context** (Cli subsystem) — consumed by `Print` for colored console output.

#### Callers

- Created by `RunProcessor.Parse`; returned from all `RunProcessor.Execute` overloads.
- Read by simulator implementations in the Simulators subsystem to determine pass/fail
  status.
- `Print` is called by simulator implementations to display output after a run.
- Read by `TestResult` (Results subsystem) to populate test result records.
