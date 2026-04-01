# RunProcessor Unit Design

## Overview

`RunProcessor.cs` coordinates the execution of simulator programs and processes
their output to determine test outcomes.

## Responsibilities

- Apply `RunLineRule` patterns to classify output lines
- Determine pass/fail status for each test bench execution
- Aggregate results across multiple test benches
