# NvcSimulator Unit Design

## Overview

`NvcSimulator.cs` implements VHDL simulation using the NVC open-source simulator.

## Responsibilities

- Invoke NVC analysis and elaboration for each test bench
- Invoke NVC run for each test bench
- Parse NVC output to determine pass/fail status
