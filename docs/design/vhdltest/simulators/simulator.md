### Simulator

![Simulators Structure](SimulatorsView.svg)

#### Purpose

Abstract base class that defines the compile and test contract all VHDL simulator integrations must
implement. Provides `SimulatorName` and `SimulatorPath` properties set at construction, an `Available()`
availability check, and a protected `Where(application)` utility for resolving executable paths from PATH.

#### Data Model

**simulatorName**: `string` — the display name of the simulator (e.g., "GHDL", "ModelSim"). Set at
construction via the primary constructor parameter and exposed read-only via `SimulatorName`.

**simulatorPath**: `string?` — absolute path to the directory containing the simulator executable. Null
when the simulator is not installed. Set at construction via the primary constructor parameter and exposed
read-only via `SimulatorPath`.

#### Key Methods

**SimulatorName** (property): Returns the simulator's registered name string.

- *Returns*: `string` — the name supplied at construction.
- *Preconditions*: None.
- *Postconditions*: Returns a non-null string.

**SimulatorPath** (property): Returns the path to the simulator installation directory, or null.

- *Returns*: `string?` — directory path, or null when the simulator was not found during construction.
- *Preconditions*: None.
- *Postconditions*: A null value indicates the simulator is unavailable.

**Available**: Tests whether the simulator has been located.

- *Returns*: `bool` — true when `SimulatorPath` is non-null.
- *Preconditions*: None.
- *Postconditions*: Returns false when SimulatorPath is null; callers must check before invoking Compile
  or Test.

**Compile** (abstract): Compiles all VHDL source files listed in the run options.

- *Parameters*: `Context context` — provides verbose logging. `Options options` — carries the file list
  and working directory.
- *Returns*: `RunResults` — classified compiler output including severity summary.
- *Preconditions*: SimulatorPath must be non-null for the production simulators.
- *Postconditions*: Returns structured results; concrete implementations must throw
  `InvalidOperationException` if `SimulatorPath` is null, with one documented exception:
  `MockSimulator` is always constructed with a null `SimulatorPath` and intentionally does not
  perform this check, since it must remain callable regardless of availability for
  self-validation use (see `MockSimulator`). Source files are written to the
  compiler in the order they appear in `options.Config.Files`.

**Test** (abstract): Executes a single named VHDL test bench.

- *Parameters*: `Context context` — provides verbose logging. `Options options` — carries configuration.
  `string test` — name of the test bench entity. How the name is safely carried into the underlying
  simulator invocation varies by implementation: `ModelSimSimulator`, `QuestaSimSimulator`, and
  `ActiveHdlSimulator` quote the name via `TclText.Quote` before interpolating it into a TCL script;
  `VivadoSimulator` quotes the name via `XilinxArgText.Quote` before interpolating it into a Xilinx
  argument file (not a TCL script); `GhdlSimulator` and `NvcSimulator` pass the name as a separate
  process argument. In every case the concrete implementation is responsible for ensuring the value
  is safely carried through to the underlying tool; the base class imposes no quoting or sanitization
  itself.
- *Returns*: `TestResult` — pass/fail status and diagnostic output for the test.
- *Preconditions*: SimulatorPath must be non-null for the production simulators.
- *Postconditions*: Returns structured results; concrete implementations must throw
  `InvalidOperationException` if `SimulatorPath` is null, with one documented exception:
  `MockSimulator` is always constructed with a null `SimulatorPath` and intentionally does not
  perform this check (see `MockSimulator`).

**Where** (protected static): Searches the system PATH for an executable and returns its full path.

- *Parameters*: `string application` — the base executable name (without extension on Windows).
- *Returns*: `string?` — full path to the executable, or null if not found.
- *Preconditions*: None.
- *Postconditions*: On Windows, prepends the current directory and appends PATHEXT extensions to the search.
  Returns null if the PATH environment variable is absent or the executable is not found in any directory.

The algorithm splits the PATH environment variable into directories, filters out null, empty, or
whitespace-only entries using the private `IsPathLegal` helper, deduplicates valid entries,
combines them with candidate file names (adding PATHEXT extensions on Windows), and returns
the first path that resolves to an existing file. On Windows, the current directory is inserted
at index 0 of the deduplicated path list after deduplication, so if the current directory also
appears in `%PATH%` it will be searched twice.

**Executable path construction**: Each concrete simulator subclass constructs the full path to the
simulator executable using `Path.Combine(SimulatorPath, "<executable-name>")` — for example
`Path.Combine(SimulatorPath, "ghdl")` for GhdlSimulator. This pattern is consistent across all
simulator implementations; `SimulatorPath` is set at construction to the directory returned by the
simulator's `FindPath()` method.

#### Error Handling

`Available()` returns false when the simulator is not installed; the factory and callers use this to avoid
calling `Compile` or `Test` on an unavailable simulator. Concrete production-simulator subclasses throw
`InvalidOperationException` with a descriptive message if `Compile` or `Test` is called when `SimulatorPath`
is null; `MockSimulator` is a documented exception to this rule — it is always constructed with a null
`SimulatorPath` and its `Compile`/`Test` overrides intentionally omit the null check so it remains callable
regardless of availability (see `MockSimulator`). `Where()` returns
null on failure; concrete subclasses' `FindPath()` methods interpret null as "simulator not found" and supply
null for `SimulatorPath`.

#### Dependencies

- **Context** — consumed by concrete subclasses for verbose logging during compilation and test.
- **Options** — provides the VHDL file list and working directory to `Compile` and `Test`.
- **RunResults** — return type of `Compile`; carries classified output and severity summary.
- **TestResult** — return type of `Test`; wraps `RunResults` for a single test bench.

#### Callers

- **TestResults** — calls `Compile(context, options)` once per run and `Test(context, options, test)` once
  per configured test bench during test execution.
- **SimulatorFactory** — holds references to concrete Simulator instances and exposes them via `Get`.
- **GhdlSimulator** — inherits from Simulator.
- **NvcSimulator** — inherits from Simulator.
- **ModelSimSimulator** — inherits from Simulator.
- **QuestaSimSimulator** — inherits from Simulator.
- **VivadoSimulator** — inherits from Simulator.
- **ActiveHdlSimulator** — inherits from Simulator.
- **MockSimulator** — inherits from Simulator.
