# SimulatorFactory Unit Design

## Overview

`SimulatorFactory.cs` implements the factory method for creating `Simulator` instances
by name.

## Responsibilities

- Map simulator name strings to concrete `Simulator` implementations
- Return the appropriate simulator for a given configuration
- Return `null` for unknown simulator names
- Callers are responsible for handling this case (for example, by throwing a descriptive exception)
