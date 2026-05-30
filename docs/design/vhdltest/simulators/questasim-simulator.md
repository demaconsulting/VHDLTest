### QuestaSimSimulator

#### Purpose

Concrete Simulator implementation for the QuestaSim commercial VHDL simulator by Mentor/Siemens.
Structurally identical to ModelSimSimulator: drives `vsim` via TCL do-scripts using `vcom` for
VHDL-2008 compilation and `vsim`/`run` for test execution. Distinguished from ModelSimSimulator by its
registered name ("QuestaSim"), its path environment variable, and its working directory path.

#### Data Model

**Instance**: `QuestaSimSimulator` (public static readonly) — singleton instance. `SimulatorName` is
"QuestaSim"; `SimulatorPath` is resolved by `FindPath()` at class initialization.

**CompileProcessor**: `RunProcessor` (public static readonly) — output classifier for QuestaSim compile
output. Classifies lines matching `.*Error:` (trailing space after the colon prevents false positives
on identifiers ending with "Error") as Error. Lines not matching any rule are left unclassified as Text.

**TestProcessor**: `RunProcessor` (public static readonly) — output classifier for QuestaSim simulation
output. Classifies `.*Note:` as Info, `.*Warning:` as Warning, and `.*Error:` or `.*Failure:` as
Error (each pattern includes a trailing space after the colon to prevent false positives on identifiers
ending with the pattern keyword). Lines not matching any rule are left unclassified as Text.

#### Key Methods

**Compile** (public override): Compiles all VHDL source files using QuestaSim's vcom utility via a TCL do-script.

- *Parameters*: `Context context` — verbose logging. `Options options` — file list and working directory.
- *Returns*: `RunResults` — classified compile output.
- *Preconditions*: SimulatorPath must be non-null.
- *Postconditions*: Creates `VHDLTest.out/QuestaSim/` if absent. Writes `compile.do` containing
  `onerror {exit -code 1}`, `vlib work`, `set worklib work`, and `vcom -2008 ../../{file}` for each
  source file, followed by `exit -code 0`. Runs `vsim -c -do compile.do` from the library directory.

**Test** (public override): Simulates a single test bench using QuestaSim's vsim utility via a TCL do-script.

- *Parameters*: `Context context` — verbose logging. `Options options` — working directory.
  `string test` — VHDL entity name or library-qualified entity name (e.g., `my_tb` or `lib.my_tb`);
  must not contain whitespace or TCL metacharacters because the name is interpolated directly
  into the TCL script without escaping.
- *Returns*: `TestResult` — simulation outcome.
- *Preconditions*: SimulatorPath must be non-null; Compile must have completed successfully.
- *Postconditions*: Writes `test.do` containing `onerror {exit -code 1}`, `set worklib work`,
  `vsim -quiet {test}`, `run -all`, `endsim`, and `exit -code 0`. Runs `vsim -c -do test.do` from the
  library directory.

**FindPath** (public static): Resolves the path to the QuestaSim installation directory.

- *Returns*: `string?` — directory path, or null if QuestaSim is not found.
- *Postconditions*: Returns the value of the `VHDLTEST_QUESTASIM_PATH` environment variable when set.
  Otherwise searches PATH for the `vsim` executable and returns its parent directory.

#### Error Handling

`FindPath` returns null when QuestaSim is not installed, causing `Available()` to return false. Calling
`Compile` or `Test` when `SimulatorPath` is null throws `InvalidOperationException` with message
"QuestaSim Simulator not available". The do-script includes `onerror {exit -code 1}` to propagate errors
as non-zero exit codes.

#### Dependencies

- **Simulator** — base class providing `SimulatorName`, `SimulatorPath`, `Available()`, and `Where()`.
- **Context** — verbose logging during compile and test.
- **Options** — VHDL file list and working directory.
- **RunProcessor** — process execution and output classification.
- **RunLineRule** — output classification rules for CompileProcessor and TestProcessor.
- **RunResults** — return type of `RunProcessor.Execute`.
- **TestResult** — wraps `RunResults` for a single test bench result.

#### Callers

- **SimulatorFactory** — holds `QuestaSimSimulator.Instance` in the Simulators array.
