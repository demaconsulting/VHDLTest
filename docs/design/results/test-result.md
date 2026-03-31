# TestResult Unit Design

## Overview

`TestResult.cs` represents the outcome of a single VHDL test bench execution.

## Data Model

| Property   | Type       | Description             |
| ---------- | ---------- | ----------------------- |
| `Name`     | `string`   | Test bench name         |
| `Passed`   | `bool`     | True if the test passed |
| `Duration` | `TimeSpan` | Execution duration      |
