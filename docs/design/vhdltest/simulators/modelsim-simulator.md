# ModelSimSimulator Unit Design

## Overview

`ModelSimSimulator.cs` implements VHDL simulation using the ModelSim commercial simulator.

## Responsibilities

- Invoke ModelSim vcom (compilation) for each test file
- Invoke ModelSim vsim (simulation) for each test bench
- Parse ModelSim output to determine pass/fail status
