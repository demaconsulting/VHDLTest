### ActiveHdlSimulator

#### Purpose

Concrete Simulator implementation for the Active-HDL simulator from Aldec. Drives the `vsimsa`
batch-mode command-line tool using TCL do-scripts: `acom` for VHDL-2008 compilation and `asim`/`run`
for test execution. Suppresses nuisance license-edition warnings produced by the Active-HDL Lattice
Edition to prevent false positive diagnostics.

#### Data Model

**Instance**: `ActiveHdlSimulator` (public static readonly) — singleton instance. `SimulatorName` is
"ActiveHdl"; `SimulatorPath` is resolved by `FindPath()` at class initialization.

**CompileProcessor**: `RunProcessor` (public static readonly) — output classifier for `vsimsa` compile
output. Classifies `KERNEL:\s*Warning:` as Warning; `Error:` and `RUNTIME:\s*Fatal Error` as Error.
Active-HDL compile output uses `RUNTIME: Fatal Error` without a trailing colon, while simulation output
uses `RUNTIME: Fatal Error:` with a trailing colon, because the compile tool (`acom`) and the simulation
tool (`asim`) produce slightly different format strings for this message.

**TestProcessor**: `RunProcessor` (public static readonly) — output classifier for `vsimsa` simulation
output. Explicitly suppresses the Aldec Lattice Edition license advisory messages by classifying them as
Text (not Warning). Classifies `KERNEL:\s*Warning:` and `KERNEL:\s*WARNING:` as Warning;
`EXECUTION::\s*NOTE` as Info; `EXECUTION::\s*WARNING` as Warning; `EXECUTION::\s*ERROR`,
`EXECUTION::\s*FAILURE`, `KERNEL:\s*ERROR`, `RUNTIME:\s*Fatal Error:`, and `VSIM:\s*Error:` as Error.

#### Key Methods

**Compile**: Compiles all VHDL source files using Active-HDL's acom utility via a TCL do-script.

- *Parameters*: `Context context` — verbose logging. `Options options` — file list and working directory.
- *Returns*: `RunResults` — classified compile output.
- *Preconditions*: SimulatorPath must be non-null.
- *Postconditions*: Creates `VHDLTest.out/ActiveHDL/` if absent. Writes `compile.do` containing
  `onerror {exit -code 1}`, `alib work VHDLTest.out/ActiveHDL`, `set worklib work`, and
  `acom -2008 -dbg {file}` for each source file. Runs
  `vsimsa -do VHDLTest.out/ActiveHDL/compile.do` from the working directory.

**Test**: Simulates a single test bench using Active-HDL's asim utility via a TCL do-script.

- *Parameters*: `Context context` — verbose logging. `Options options` — working directory.
  `string test` — VHDL entity name or library-qualified entity name (e.g., `my_tb` or `lib.my_tb`);
  must not contain whitespace or TCL metacharacters because the name is interpolated directly
  into the TCL script without escaping.
- *Returns*: `TestResult` — simulation outcome.
- *Preconditions*: SimulatorPath must be non-null; Compile must have completed successfully.
- *Postconditions*: Writes `test.do` containing `onerror {exit -code 1}`, `set worklib work`,
  `asim {test}`, `run -all`, `endsim`, and `exit -code 0`. Runs `vsimsa -do VHDLTest.out/ActiveHDL/test.do` from the
  working directory.

**FindPath** (public static): Resolves the path to the Active-HDL installation directory.

- *Returns*: `string?` — directory path, or null if Active-HDL is not found.
- *Postconditions*: Returns the value of the `VHDLTEST_ACTIVEHDL_PATH` environment variable when set.
  Otherwise searches PATH for the `vsimsa` executable and returns its parent directory.

#### Error Handling

`FindPath` returns null when Active-HDL is not installed, causing `Available()` to return false. Calling
`Compile` or `Test` when `SimulatorPath` is null throws `InvalidOperationException` with message "ActiveHDL
Simulator not available". If `Test` is called before a successful `Compile` run, `File.WriteAllText`
propagates a `DirectoryNotFoundException` because the library output directory (`VHDLTest.out/ActiveHDL/`)
has not been created. The TCL script includes `onerror {exit -code 1}` to propagate errors as non-zero
exit codes. The Lattice Edition license advisory lines are explicitly classified as Text in TestProcessor
to prevent them from being promoted to Warning severity and causing false positive test failures.

#### Dependencies

- **Simulator** — base class providing `SimulatorName`, `SimulatorPath`, `Available()`, and `Where()`.
- **Context** — verbose logging during compile and test.
- **Options** — VHDL file list and working directory.
- **RunProcessor** — process execution and output classification.
- **RunLineRule** — output classification rules for CompileProcessor and TestProcessor.
- **RunResults** — return type of `RunProcessor.Execute`.
- **TestResult** — wraps `RunResults` for a single test bench result.

#### Callers

- **SimulatorFactory** — holds `ActiveHdlSimulator.Instance` in the Simulators array.
