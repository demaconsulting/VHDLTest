# VivadoSimulator Unit Design

## Overview

`VivadoSimulator.cs` implements VHDL simulation using the Vivado simulator
from Xilinx/AMD.

## Responsibilities

- Invoke `xvhdl` analysis for each VHDL source file
- Invoke `xelab` elaboration and simulation for each test bench
- Parse Vivado output to determine pass/fail status
