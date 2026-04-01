# QuestaSimSimulator Unit Design

## Overview

`QuestaSimSimulator.cs` implements VHDL simulation using the QuestaSim commercial simulator.

## Responsibilities

- Invoke QuestaSim vcom (compilation) for each test file
- Invoke QuestaSim vsim (simulation) for each test bench
- Parse QuestaSim output to determine pass/fail status
