### ModelSimSimulator

![Simulators Structure](SimulatorsView.svg)

#### Purpose

Concrete Simulator implementation for the ModelSim commercial VHDL simulator by Mentor/Siemens. Drives the
`vsim` command-line tool using TCL do-scripts: `vcom` for VHDL-2008 compilation and `vsim`/`run` for test
bench simulation.

#### Data Model

**Instance**: `ModelSimSimulator` (public static get-only property) — singleton instance. `SimulatorName` is
"ModelSim"; `SimulatorPath` is resolved by `FindPath()` at class initialization.

**CompileProcessor**: `RunProcessor` (public static get-only property) — output classifier for ModelSim compile
output. Classifies lines matching `` `.*Error: ` `` (trailing space after the colon requires a space character
to follow the colon, preventing false matches on identifiers ending with "Error") as Error. Lines not
matching any rule are left unclassified as Text.

**TestProcessor**: `RunProcessor` (public static get-only property) — output classifier for ModelSim simulation
output. Classifies lines matching `.*Note:` (with trailing space) as Info, `.*Warning:` as Warning, and
`.*Error:` or `.*Failure:` as Error (each pattern requires a trailing space to prevent false matches on
identifiers ending with the keyword).

**_compileProcessor**: `RunProcessor` (private instance field) — per-instance processor backed by the injected invoker.
**_testProcessor**: `RunProcessor` (private instance field) — per-instance processor backed by the injected invoker.

#### Key Methods

**Compile**: Compiles all VHDL source files using ModelSim's vcom utility via a TCL do-script.

- *Parameters*: `Context context` — verbose logging. `Options options` — file list and working directory.
- *Returns*: `RunResults` — classified compile output.
- *Preconditions*: SimulatorPath must be non-null.
- *Postconditions*: Creates `VHDLTest.out/ModelSim/` if absent. Writes `compile.do` containing
  `onerror {exit -code 1}`, `vlib work`, `set worklib work`, and `vcom -2008` followed by the
  TCL-quoted (via `TclText.Quote`) relative path for each source file, followed by `exit -code 0`.
  Runs `vsim -c -do compile.do` from the library directory. Source files are passed to the
  compiler in the order they appear in `options.Config.Files`.

**Test**: Simulates a single test bench using ModelSim's vsim utility via a TCL do-script.

- *Parameters*: `Context context` — verbose logging. `Options options` — working directory.
  `string test` — VHDL entity name or library-qualified entity name (e.g., `my_tb` or `lib.my_tb`);
  TCL-quoted via `TclText.Quote` before interpolation, so it may safely contain whitespace or
  TCL metacharacters.
- *Returns*: `TestResult` — simulation outcome.
- *Preconditions*: SimulatorPath must be non-null; Compile must have completed successfully.
- *Postconditions*: Writes `test.do` containing `onerror {exit -code 1}`, `set worklib work`,
  `vsim -quiet` followed by the TCL-quoted (via `TclText.Quote`) test name, `run -all`, `endsim`,
  and `exit -code 0`. Runs `vsim -c -do test.do` from the library directory.

**FindPath** (public static): Resolves the path to the ModelSim installation directory.

- *Returns*: `string?` — directory path, or null if ModelSim is not found.
- *Postconditions*: Returns the value of the `VHDLTEST_MODELSIM_PATH` environment variable when set.
  Otherwise searches PATH for the `vsim` executable and returns its parent directory.

**CreateForTesting** (internal static): Creates a non-singleton instance backed by a supplied `IProcessInvoker`.
Used by unit tests to verify simulator invocations without launching real processes.

#### Error Handling

`FindPath` returns null when ModelSim is not installed, causing `Available()` to return false. Calling
`Compile` or `Test` when `SimulatorPath` is null throws `InvalidOperationException` with message "ModelSim
Simulator not available". The do-script includes `onerror {exit -code 1}` so compile and simulation errors
produce a non-zero exit code, which CompileProcessor and TestProcessor detect and record as Error output.

#### Dependencies

- **Simulator** — base class providing `SimulatorName`, `SimulatorPath`, `Available()`, and `Where()`.
- **TclText** — quotes file paths and test names before interpolation into generated TCL do-scripts.
- **Context** — verbose logging during compile and test.
- **Options** — VHDL file list and working directory.
- **RunProcessor** — process execution and output classification.
- **RunLineRule** — output classification rules for CompileProcessor and TestProcessor.
- **RunResults** — return type of `RunProcessor.Execute`.
- **TestResult** — wraps `RunResults` for a single test bench result.
- **IProcessInvoker** — injected invoker used by instance processors.
- **ProcessInvoker** — default invoker used by the singleton instance.

#### Callers

- **SimulatorFactory** — holds `ModelSimSimulator.Instance` in the Simulators array.
