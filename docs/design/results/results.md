# Results Subsystem Design

## Overview

The Results subsystem provides the data model for storing and serializing VHDL test
results in standard formats (TRX and JUnit XML).

## Units

| Unit        | File                     | Responsibility                                |
| ----------- | ------------------------ | --------------------------------------------- |
| TestResult  | `Results/TestResult.cs`  | Represents a single test result               |
| TestResults | `Results/TestResults.cs` | Collection of test results with serialization |

## Interactions

- Populated by the Run subsystem with pass/fail outcomes
- Serialized to TRX or JUnit XML output files when `--results` is specified
