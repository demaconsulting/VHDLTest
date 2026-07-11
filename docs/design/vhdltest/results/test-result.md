### TestResult

![Results Structure](ResultsView.svg)

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

*Invariant*: Because `TestId` and `ExecutionId` are both initialized to `Guid.NewGuid()` in
their property declarations, every constructed `TestResult` instance carries globally unique
identifiers. Under C# record value-equality semantics, two `TestResult` instances with identical
`ClassName`, `TestName`, and `RunResults` values will still be considered unequal by identity
because their `TestId` and `ExecutionId` GUIDs will differ. Callers that require value-equality
must override the default record equality or compare specific fields explicitly.

**Passed**: `bool` — Derived; true when `RunResults.Summary` is less than `RunLineType.Error`.

**Failed**: `bool` — Derived; true when `RunResults.Summary` is greater than or equal to
`RunLineType.Error`; logical negation of `Passed`.

#### Key Methods

**PrintSummary**: Writes a single colored summary line for this test to the context output.

- *Parameters*: `Context context` — the output channel to write to.
- *Returns*: void.
- *Preconditions*: `context` is not null; passing null throws `ArgumentNullException`.
- *Postconditions*: a line containing the pass/fail word (green or red), the test name, and the
  duration in seconds has been written to the context.

Writes the word "Passed" in green or "Failed" in red, followed by the test name and the duration
formatted to one decimal place in parentheses.

#### Error Handling

`PrintSummary` throws `ArgumentNullException` when `context` is null. All other I/O in
`PrintSummary` is fully delegated to the injected `Context` and is not performed by `TestResult`
directly. Error conditions are captured in `RunResults.Summary` and `RunResults.Lines` by the Run
subsystem before construction.

#### Dependencies

- **RunResults** — holds the raw execution output and severity summary used to derive `Passed` and
  `Failed`.
- **Context** — receives the formatted summary line in `PrintSummary`.

#### Callers

- **Concrete Simulator implementations** (`ActiveHdlSimulator`, `GhdlSimulator`, `MockSimulator`,
  `ModelSimSimulator`, `NvcSimulator`, `QuestaSimSimulator`, `VivadoSimulator`) — construct
  `TestResult` instances from `RunResults` returned by the simulator test step and return them
  to `TestResults.Execute`.
- **Validation** — constructs `TestResult` instances for self-validation scenarios.
