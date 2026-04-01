# Simulator Unit Design

## Overview

`Simulator.cs` defines the abstract `Simulator` base class that all VHDL simulator
integrations must inherit from.

## Responsibilities

- Define the `SimulatorName` property contract
- Define the `Compile` and `Simulate` method contracts
- Provide shared utilities used by concrete simulator implementations
