# GhdlSimulator Unit Design

## Overview

`GhdlSimulator.cs` implements VHDL simulation using the GHDL open-source simulator.

## Responsibilities

- Invoke GHDL analysis (compilation) for each test file
- Invoke GHDL elaboration and run for each test bench
- Parse GHDL output to determine pass/fail status
