# Simulators Subsystem Design

## Overview

The Simulators subsystem provides integration with multiple VHDL simulators. It implements
a factory pattern for creating simulator instances and an abstract base class defining
the interface all simulators must implement.

## Units

| Unit               | File                               | Responsibility                                   |
| ------------------ | ---------------------------------- | ------------------------------------------------ |
| Simulator          | `Simulators/Simulator.cs`          | Abstract base class defining simulator interface |
| SimulatorFactory   | `Simulators/SimulatorFactory.cs`   | Creates simulator instances by name              |
| GhdlSimulator      | `Simulators/GhdlSimulator.cs`      | GHDL open-source simulator integration           |
| NvcSimulator       | `Simulators/NvcSimulator.cs`       | NVC open-source simulator integration            |
| ModelSimSimulator  | `Simulators/ModelSimSimulator.cs`  | ModelSim commercial simulator integration        |
| QuestaSimSimulator | `Simulators/QuestaSimSimulator.cs` | QuestaSim commercial simulator integration       |
| VivadoSimulator    | `Simulators/VivadoSimulator.cs`    | Vivado FPGA simulator integration                |
| ActiveHdlSimulator | `Simulators/ActiveHdlSimulator.cs` | Active-HDL simulator integration                 |
| MockSimulator      | `Simulators/MockSimulator.cs`      | Test double for simulator testing                |

## Design Pattern

The subsystem uses the Factory pattern. `SimulatorFactory.Get(name)` returns the
appropriate `Simulator`-derived instance based on the simulator name string found
in the configuration file.

## Interactions

- Consumed by `Program.cs` via `SimulatorFactory` to obtain the active simulator
- Each simulator receives `RunProgram` callbacks to execute compilation and simulation steps
