### Simulator

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
- *Preconditions*: SimulatorPath must be non-null.
- *Postconditions*: Returns structured results; concrete implementations must throw
  `InvalidOperationException` if `SimulatorPath` is null.

**Test** (abstract): Executes a single named VHDL test bench.

- *Parameters*: `Context context` — provides verbose logging. `Options options` — carries configuration.
  `string test` — name of the test bench entity.
- *Returns*: `TestResult` — pass/fail status and diagnostic output for the test.
- *Preconditions*: SimulatorPath must be non-null. The `test` parameter is interpolated directly into
  a TCL script by some simulator implementations without sanitization; callers must ensure the value
  contains no TCL metacharacters (e.g., `[`, `]`, `$`, `;`, `\`). The base class does not sanitize the
  input; preventing injection is the responsibility of the caller.
- *Postconditions*: Returns structured results; concrete implementations must throw
  `InvalidOperationException` if `SimulatorPath` is null.

**Where** (protected static): Searches the system PATH for an executable and returns its full path.

- *Parameters*: `string application` — the base executable name (without extension on Windows).
- *Returns*: `string?` — full path to the executable, or null if not found.
- *Preconditions*: None.
- *Postconditions*: On Windows, prepends the current directory and appends PATHEXT extensions to the search.
  Returns null if the PATH environment variable is absent or the executable is not found in any directory.

The algorithm splits the PATH environment variable into directories, filters out null, empty, or
whitespace-only entries using the private `IsPathLegal` helper, deduplicates valid entries,
combines them with candidate file names (adding PATHEXT extensions on Windows), and returns
the first path that resolves to an existing file.

#### Error Handling

`Available()` returns false when the simulator is not installed; the factory and callers use this to avoid
calling `Compile` or `Test` on an unavailable simulator. Concrete subclasses throw `InvalidOperationException`
with a descriptive message if `Compile` or `Test` is called when `SimulatorPath` is null. `Where()` returns
null on failure; concrete subclasses' `FindPath()` methods interpret null as "simulator not found" and supply
null for `SimulatorPath`.

#### Dependencies

- **Context** — consumed by concrete subclasses for verbose logging during compilation and test.
- **Options** — provides the VHDL file list and working directory to `Compile` and `Test`.
- **RunResults** — return type of `Compile`; carries classified output and severity summary.
- **TestResult** — return type of `Test`; wraps `RunResults` for a single test bench.

#### Callers

- **SimulatorFactory** — holds references to concrete Simulator instances and exposes them via `Get`.
- **GhdlSimulator** — inherits from Simulator.
- **NvcSimulator** — inherits from Simulator.
- **ModelSimSimulator** — inherits from Simulator.
- **QuestaSimSimulator** — inherits from Simulator.
- **VivadoSimulator** — inherits from Simulator.
- **ActiveHdlSimulator** — inherits from Simulator.
- **MockSimulator** — inherits from Simulator.
