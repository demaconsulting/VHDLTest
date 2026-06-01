### SimulatorFactory

#### Purpose

Static factory class that maps simulator name strings to singleton `Simulator` instances and provides
auto-discovery of the first available installed simulator. It is the single point of access for the rest
of the system to obtain a simulator without knowing which simulators are installed.

#### Data Model

**Simulators**: `Simulator[]` (private static readonly) — ordered array of the six production simulator
singletons: GhdlSimulator, ModelSimSimulator, QuestaSimSimulator, VivadoSimulator, ActiveHdlSimulator,
NvcSimulator. MockSimulator is excluded from this array and is returned only by explicit name request.

#### Key Methods

**Get**: Returns the simulator matching the given name, or auto-selects the first available simulator.

- *Parameters*: `string? name` — simulator name (case-insensitive), or null to auto-select.
- *Returns*: `Simulator?` — the matched or auto-selected simulator, or null if no match is found.
- *Preconditions*: None.
- *Postconditions*: Returns `MockSimulator.Instance` when name equals "mock" (case-insensitive). Searches
  the `Simulators` array for a case-insensitive `SimulatorName` match when a non-mock name is given.
  Returns the first `Simulator` where `Available()` is true when name is null. Returns null if no match
  is found.

#### Error Handling

`Get` returns null for unknown simulator names; the caller (typically Program) is responsible for detecting
this and throwing a descriptive exception. MockSimulator is never included in auto-discovery results; it is
only accessible via the explicit name "mock".

#### Dependencies

- **GhdlSimulator** — production simulator instance held in the Simulators array.
- **ModelSimSimulator** — production simulator instance held in the Simulators array.
- **QuestaSimSimulator** — production simulator instance held in the Simulators array.
- **VivadoSimulator** — production simulator instance held in the Simulators array.
- **ActiveHdlSimulator** — production simulator instance held in the Simulators array.
- **NvcSimulator** — production simulator instance held in the Simulators array.
- **MockSimulator** — returned by explicit name request; not included in the array.

#### Callers

- **Program** — calls `SimulatorFactory.Get(name)` to obtain the simulator for each test run.
