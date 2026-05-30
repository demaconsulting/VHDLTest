### GhdlSimulator

#### Purpose

Concrete Simulator implementation for the GHDL open-source VHDL simulator. Drives the `ghdl` command-line
tool to analyze (compile) VHDL source files and elaborate and run individual test benches under the
VHDL-2008 standard. Requires an explicit elaboration step before execution.

#### Data Model

**Instance**: `GhdlSimulator` (public static readonly) — singleton instance. `SimulatorName` is "GHDL";
`SimulatorPath` is resolved by `FindPath()` at class initialization.

**CompileProcessor**: `RunProcessor` (public static get-only property) — output classifier for GHDL analysis output.
Classifies lines matching `.*:\d+:\d+:warning:` as Warning; lines matching `.*:\d+:\d+:` (trailing
space prevents false matches on warning-prefixed lines that include the colon-digit-digit-colon
prefix), `.*:error:`, or `.*: cannot open` as Error.

**TestProcessor**: `RunProcessor` (public static get-only property) — output classifier for GHDL run output.
Applies nine classification rules in order:

- Lines matching `.*:\(assertion note\):` or `.*:\(report note\):` are classified as Info.
- Lines matching `.*:\(assertion warning\):` or `.*:\(report warning\):` are classified as Warning.
- Lines matching `.*:\(assertion error\):`, `.*:\(report error\):`, `.*:\(assertion failure\):`,
  `.*:\(report failure\):`, or `.*:error:` are classified as Error.

#### Key Methods

**Compile**: Compiles all VHDL source files using GHDL analysis mode.

- *Parameters*: `Context context` — verbose logging. `Options options` — VHDL file list and working
  directory.
- *Returns*: `RunResults` — classified compile output.
- *Preconditions*: SimulatorPath must be non-null. `Options.Config.Files` is guaranteed
  non-null by all callers (the configuration loader validates the file list before invoking
  any simulator method).
- *Postconditions*: Creates `VHDLTest.out/GHDL/` if absent. Writes `VHDLTest.out/GHDL/compile.rsp`
  listing all source files one per line. Runs
  `ghdl -a --std=08 --workdir=VHDLTest.out/GHDL @VHDLTest.out/GHDL/compile.rsp`.

**Test**: Elaborates and runs a single VHDL test bench using GHDL.

- *Parameters*: `Context context` — verbose logging. `Options options` — working directory.
  `string test` — test bench entity name.
- *Returns*: `TestResult` — elaboration and run outcome.
- *Preconditions*: SimulatorPath must be non-null; Compile must have completed successfully beforehand.
- *Postconditions*: Runs `ghdl -e --std=08 --workdir=VHDLTest.out/GHDL {test}` to elaborate. If
  elaboration produces an Error-severity result, returns that result immediately without attempting to
  run. Otherwise runs `ghdl -r --std=08 --workdir=VHDLTest.out/GHDL {test}`.
- *Note*: `CompileProcessor` is reused to classify the output from the elaboration step (`ghdl -e`),
  since elaboration output follows the same format as compilation output. This reuse is intentional:
  both phases produce diagnostics in the same `file:line:col: message` format.

**FindPath** (public static): Resolves the path to the GHDL installation directory.

- *Returns*: `string?` — directory path, or null if GHDL is not found.
- *Postconditions*: Returns the value of the `VHDLTEST_GHDL_PATH` environment variable when set.
  Otherwise searches PATH for the `ghdl` executable and returns its parent directory.

#### Error Handling

`FindPath` returns null when GHDL is not installed, causing `Available()` to return false. Calling
`Compile` or `Test` when `SimulatorPath` is null throws `InvalidOperationException` with message
"GHDL Simulator not available". Elaboration failures in `Test` short-circuit execution: the method
returns the elaboration `RunResults` wrapped in a `TestResult` without attempting the run step, avoiding
misleading empty output from a simulation that cannot start.

#### Dependencies

- **Simulator** — base class providing `SimulatorName`, `SimulatorPath`, `Available()`, and `Where()`.
- **Context** — verbose logging during compile and test.
- **Options** — VHDL file list and working directory.
- **RunProcessor** — process execution and output classification.
- **RunLineRule** — output classification rules for CompileProcessor and TestProcessor.
- **RunResults** — return type of `RunProcessor.Execute`.
- **TestResult** — wraps `RunResults` for a single test bench result.

#### Callers

- **SimulatorFactory** — holds `GhdlSimulator.Instance` in the Simulators array.
