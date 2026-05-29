### NvcSimulator

#### Purpose

Concrete Simulator implementation for the NVC open-source VHDL simulator. Drives the `nvc` command-line
tool to analyze VHDL source files and elaborate and simulate individual test benches under the VHDL-2008
standard, combining elaboration and simulation in a single invocation.

#### Data Model

**Instance**: `NvcSimulator` (public static readonly) — singleton instance. `SimulatorName` is "NVC";
`SimulatorPath` is resolved by `FindPath()` at class initialization.

**CompileProcessor**: `RunProcessor` (public static readonly) — output classifier for NVC analysis output.
Classifies lines matching `.* Note:` as Info, `.* Warning:` as Warning, and `.* Error:`, `.* Failure:`,
`.* Fatal:` as Error.

**TestProcessor**: `RunProcessor` (public static readonly) — output classifier for NVC simulation output.
Uses the same classification patterns as CompileProcessor.

#### Key Methods

**Compile**: Compiles all VHDL source files using NVC analysis mode.

- *Parameters*: `Context context` — verbose logging. `Options options` — file list and working directory.
- *Returns*: `RunResults` — classified compile output.
- *Preconditions*: SimulatorPath must be non-null.
- *Postconditions*: Creates `VHDLTest.out/NVC/` if absent (path resolved relative to
  `options.WorkingDirectory`). Writes `VHDLTest.out/NVC/compile.rsp` listing
  all source files. Runs
  `nvc --std=2008 --work=work:VHDLTest.out/NVC/lib -a @VHDLTest.out/NVC/compile.rsp`.
  All paths in the `nvc` command arguments are relative to `options.WorkingDirectory`,
  which is also passed as the working directory to `CompileProcessor.Execute`.

**Test**: Elaborates and runs a single test bench in a single NVC invocation.

- *Parameters*: `Context context` — verbose logging. `Options options` — working directory.
  `string test` — test bench entity name.
- *Returns*: `TestResult` — run outcome.
- *Preconditions*: SimulatorPath must be non-null; Compile must have completed successfully.
- *Postconditions*: Runs
  `nvc --std=2008 --work=work:VHDLTest.out/NVC/lib -e {test} -r {test}` in a single process invocation,
  combining elaboration and simulation.

**FindPath** (public static): Resolves the path to the NVC installation directory.

- *Returns*: `string?` — directory path, or null if NVC is not found.
- *Postconditions*: Returns the value of the `VHDLTEST_NVC_PATH` environment variable when set. Otherwise
  searches PATH for the `nvc` executable and returns its parent directory.

#### Error Handling

`FindPath` returns null when NVC is not installed, causing `Available()` to return false. Calling `Compile`
or `Test` when `SimulatorPath` is null throws `InvalidOperationException` with message "NVC Simulator not
available".

#### Dependencies

- **Simulator** — base class providing `SimulatorName`, `SimulatorPath`, `Available()`, and `Where()`.
- **Context** — verbose logging during compile and test.
- **Options** — VHDL file list and working directory.
- **RunProcessor** — process execution and output classification.
- **RunLineRule** — output classification rules for CompileProcessor and TestProcessor.
- **RunResults** — return type of `RunProcessor.Execute`.
- **TestResult** — wraps `RunResults` for a single test bench result.

#### Callers

- **SimulatorFactory** — holds `NvcSimulator.Instance` in the Simulators array.
