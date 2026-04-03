# ActiveHdlSimulator Unit Design

## Overview

`ActiveHdlSimulator.cs` implements VHDL simulation using the Active-HDL simulator
from Aldec.

## Responsibilities

- Invoke Active-HDL compilation commands for each test file
- Invoke Active-HDL simulation for each test bench
- Parse Active-HDL output to determine pass/fail status
