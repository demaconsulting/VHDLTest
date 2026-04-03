# Run Subsystem Design

## Overview

The Run subsystem handles the execution of VHDL simulation programs and the processing
of their output to determine test outcomes.

## Units

| Unit         | File                  | Responsibility                                    |
| ------------ | --------------------- | ------------------------------------------------- |
| RunProcessor | `Run/RunProcessor.cs` | Coordinates execution and output processing       |
| RunProgram   | `Run/RunProgram.cs`   | Executes an external program and captures output  |
| RunResults   | `Run/RunResults.cs`   | Holds execution results (exit code, output lines) |
| RunLine      | `Run/RunLine.cs`      | Represents a single line of program output        |
| RunLineRule  | `Run/RunLineRule.cs`  | Rule for classifying output lines                 |
| RunLineType  | `Run/RunLineType.cs`  | Enumeration of output line type classifications   |

## Design Flow

1. `RunProgram` executes the simulator executable and captures stdout/stderr
2. `RunResults` holds the captured output as a list of `RunLine` instances
3. `RunLineRule` patterns classify each `RunLine` into a `RunLineType`
4. `RunProcessor` aggregates classified lines to determine overall pass/fail
