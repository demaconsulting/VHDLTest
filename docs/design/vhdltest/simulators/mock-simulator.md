### MockSimulator

#### Purpose

Deterministic in-process VHDL simulator test double for self-validation testing of VHDLTest itself.
Generates synthetic compile and test output by inspecting embedded keywords in file names and test bench
names, with no external process invocation. Selected only when the simulator name is explicitly "mock".

#### Data Model

**Instance**: `MockSimulator` (public static readonly) — singleton instance. `SimulatorName` is "Mock";
`SimulatorPath` is always null, meaning `Available()` always returns false and auto-discovery by
SimulatorFactory never selects MockSimulator.

**CompileProcessor**: `RunProcessor` (public static readonly) — output classifier for synthetic compile
output. Classifies lines matching `Info:` as Info, `Warning:` as Warning, and `Error:` as Error.

**TestProcessor**: `RunProcessor` (public static readonly) — output classifier for synthetic test output.
Classifies `Info:` as Info, `Warning:` as Warning, and `Failure:` or `Error:` as Error.

#### Key Methods

**Compile**: Generates synthetic compile output from source file name patterns.

- *Parameters*: `Context context` — verbose logging. `Options options` — file list.
- *Returns*: `RunResults` — synthetic classified compile output.
- *Preconditions*: None (callable regardless of SimulatorPath being null).
- *Postconditions*: Iterates `options.Config.Files`. For each file: appends `Error: {file}` and sets
  exitCode to 1 when the name contains `_error_`; appends `Warning: {file}` when it contains `_warning_`;
  appends `Info: {file}` when it contains `_info_`; otherwise appends `Compiled {file}`. Passes the
  accumulated output to `CompileProcessor.Parse`.

**Test**: Generates synthetic test output from the test bench name pattern.

- *Parameters*: `Context context` — verbose logging. `Options options` — not used. `string test` — test
  bench name.
- *Returns*: `TestResult` — synthetic test outcome.
- *Preconditions*: None.
- *Postconditions*: Appends `Warning: {test}` when test contains `_warning_`; appends `Info: {test}` when
  it contains `_info_`. Then: appends `Error: {test}` (exitCode 1) when it contains `_error_`; appends
  `Failure: {test}` (exitCode remains 0) when it contains `_fail_`; otherwise appends `Passed: {test}`. Passes the accumulated
  output to `TestProcessor.Parse`.

#### Error Handling

`SimulatorPath` is always null and `Available()` always returns false, so SimulatorFactory never
auto-selects MockSimulator. No external process invocations occur, so there are no I/O or process errors.
Unlike production simulators, `Compile` and `Test` do not check `SimulatorPath` and do not throw
`InvalidOperationException`; the mock is designed to be called explicitly regardless of availability.

#### Dependencies

- **Simulator** — base class providing `SimulatorName`, `SimulatorPath`, and `Available()`.
- **Context** — verbose logging via `WriteVerboseLine` during compile and test.
- **Options** — provides `Config.Files` for `Compile`.
- **RunProcessor** — parses synthetic output via `CompileProcessor.Parse` and `TestProcessor.Parse`.
- **RunResults** — returned by `Compile`.
- **TestResult** — returned by `Test`; wraps `RunResults` from `TestProcessor.Parse`.

#### Callers

- **SimulatorFactory** — returns `MockSimulator.Instance` when the simulator name equals "mock"
  (case-insensitive).
