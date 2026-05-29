### TestResult

#### Purpose

`TestResult.cs` is an immutable record that captures the outcome of a single VHDL test bench
execution. It aggregates the raw `RunResults` from the simulator, derives a pass/fail flag, and
exposes a human-readable summary line.

#### Data Model

**ClassName**: `string` — Fully qualified test class name used as the TRX class identifier.

**TestName**: `string` — Test bench name used as the logical test identifier in reports.

**RunResults**: `RunResults` — The raw execution results from the simulator, including exit code,
captured output lines, duration, and the highest-severity line type.

**TestId**: `Guid` — Unique identifier assigned to this test definition; initialized to
`Guid.NewGuid()` at construction.

**ExecutionId**: `Guid` — Unique identifier assigned to this test execution instance; initialized to
`Guid.NewGuid()` at construction.

**Passed**: `bool` — Derived; true when `RunResults.Summary` is less than `RunLineType.Error`.

**Failed**: `bool` — Derived; true when `RunResults.Summary` is greater than or equal to
`RunLineType.Error`; logical negation of `Passed`.

#### Key Methods

**PrintSummary**: Writes a single colored summary line for this test to the context output.

- *Parameters*: `Context context` — the output channel to write to.
- *Returns*: void.
- *Preconditions*: `context` is not null.
- *Postconditions*: a line containing the pass/fail word (green or red), the test name, and the
  duration in seconds has been written to the context.

Writes the word "Passed" in green or "Failed" in red, followed by the test name and the duration
formatted to one decimal place in parentheses.

#### Error Handling

`TestResult` is a passive record; it performs no I/O and throws no exceptions. Error conditions are
captured in `RunResults.Summary` and `RunResults.Lines` by the Run subsystem before construction.

#### Dependencies

- **RunResults** — holds the raw execution output and severity summary used to derive `Passed` and
  `Failed`.
- **Context** — receives the formatted summary line in `PrintSummary`.

#### Callers

- **TestResults** — constructs `TestResult` instances and appends them to its `Tests` collection.
- **Validation** — constructs `TestResult` instances for self-validation scenarios.
