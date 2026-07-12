### VivadoSimulator

![Simulators Structure](SimulatorsView.svg)

#### Purpose

Concrete Simulator implementation for the Vivado simulator from Xilinx/AMD. Drives `xvhdl` for VHDL-2008
source analysis and `xelab` for elaboration and simulation, using argument-file (`.do`) scripts to pass
options and file lists. File paths and test entity names are quoted via `XilinxArgText.Quote` before
interpolation into these argument files, which follow Xilinx's UG900 shell-style tokenization
convention rather than TCL syntax.

#### Data Model

**Instance**: `VivadoSimulator` (public static get-only property) — singleton instance. `SimulatorName` is "Vivado";
`SimulatorPath` is resolved by `FindPath()` at class initialization.

**CompileProcessor**: `RunProcessor` (public static get-only property) — output classifier for xvhdl output.
Classifies lines matching `` `Error: ` `` as Error. The trailing space after the colon ensures the
pattern only matches when a space follows the colon, preventing false matches on lines like
`ErrorDetails: ...`. Lines not matching any rule are left unclassified.

**TestProcessor**: `RunProcessor` (public static get-only property) — output classifier for xelab simulation output.
Classifies `` `Note: ` `` as Info, `` `Warning: ` `` as Warning, and `` `Error: ` `` or `` `Failure: ` `` as
Error. The trailing space after the colon ensures the pattern only matches when a space follows the colon,
preventing false matches on lines like `ErrorDetails: ...`.

**_compileProcessor**: `RunProcessor` (private instance field) — per-instance processor backed by the injected invoker.
**_testProcessor**: `RunProcessor` (private instance field) — per-instance processor backed by the injected invoker.

#### Key Methods

**Compile**: Analyzes all VHDL source files using xvhdl via an argument file.

- *Parameters*: `Context context` — verbose logging. `Options options` — file list and working directory.
- *Returns*: `RunResults` — classified compile output.
- *Preconditions*: SimulatorPath must be non-null.
- *Postconditions*: Creates `VHDLTest.out/Vivado/` if absent. Writes `compile.do` containing `-2008`,
  `-nolog`, `-work work`, and the quoted (via `XilinxArgText.Quote`) relative path for each source
  file. Runs `xvhdl -file compile.do` from the library directory; `RunProcessor` handles
  platform-specific execution transparently. Source files are passed to the compiler in the order
  they appear in `options.Config.Files`.

**Test**: Elaborates and simulates a single test bench using xelab via an argument file.

- *Parameters*: `Context context` — verbose logging. `Options options` — working directory.
  `string test` — test bench entity name; quoted via `XilinxArgText.Quote` before interpolation,
  so it may safely contain whitespace or characters requiring escaping.
- *Returns*: `TestResult` — simulation outcome.
- *Preconditions*: SimulatorPath must be non-null; Compile must have completed successfully.
- *Postconditions*: Writes `test.do` containing `-nolog`, `-standalone`, `-runall`, and the quoted
  (via `XilinxArgText.Quote`) test entity name. Runs `xelab -file test.do` from the library
  directory; `RunProcessor` handles platform-specific execution transparently.

**FindPath** (public static): Resolves the path to the Vivado installation directory.

- *Returns*: `string?` — directory path, or null if Vivado is not found.
- *Postconditions*: Returns the value of the `VHDLTEST_VIVADO_PATH` environment variable when set.
  Otherwise searches PATH for the `vivado` executable and returns its parent directory.

**CreateForTesting** (internal static): Creates a non-singleton instance backed by a supplied `IProcessInvoker`.
Used by unit tests to verify simulator invocations without launching real processes.

#### Error Handling

`FindPath` returns null when Vivado is not installed, causing `Available()` to return false. Calling
`Compile` or `Test` when `SimulatorPath` is null throws `InvalidOperationException` with message "Vivado
Simulator not available".

#### Dependencies

- **Simulator** — base class providing `SimulatorName`, `SimulatorPath`, `Available()`, and `Where()`.
- **XilinxArgText** — quotes file paths and test names before interpolation into generated Xilinx
  argument files.
- **Context** — verbose logging during compile and test.
- **Options** — VHDL file list and working directory.
- **RunProcessor** — process execution and output classification.
- **RunLineRule** — output classification rules for CompileProcessor and TestProcessor.
- **RunResults** — return type of `RunProcessor.Execute`.
- **TestResult** — wraps `RunResults` for a single test bench result.
- **IProcessInvoker** — injected invoker used by instance processors.
- **ProcessInvoker** — default invoker used by the singleton instance.

#### Callers

- **SimulatorFactory** — holds `VivadoSimulator.Instance` in the Simulators array.
