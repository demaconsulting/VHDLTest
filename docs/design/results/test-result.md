# TestResult Unit Design

## Overview

`TestResult.cs` represents the outcome of a single VHDL test bench execution.

## Data Model

| Property     | Type         | Description                                                         |
| ------------ | ------------ | ------------------------------------------------------------------- |
| `ClassName`  | `string`     | Fully qualified test class name                                    |
| `TestName`   | `string`     | Test bench name (logical test identifier)                          |
| `RunResults` | `RunResults` | Execution results, including `Duration` as a `double` (seconds)    |
| `Passed`     | `bool`       | Derived: true if the test passed based on `RunResults`             |
| `Failed`     | `bool`       | Derived: logical negation of `Passed`                              |
