# SimulatorFactory Unit Design

## Overview

`SimulatorFactory.cs` implements the factory method for creating `Simulator` instances
by name.

## Responsibilities

- Map simulator name strings to concrete `Simulator` implementations
- Return the appropriate simulator for a given configuration
- Throw a descriptive exception for unknown simulator names
