# RunResults Unit Design

## Overview

`RunResults.cs` holds the results of executing an external simulator program.

## Data Model

| Property     | Type                          | Description                                 |
| ------------ | ----------------------------- | ------------------------------------------- |
| `Summary`    | `string`                      | High-level summary of the run               |
| `Start`      | `DateTime`                    | Timestamp when the run started              |
| `Duration`   | `double`                      | Duration of the run in seconds              |
| `ExitCode`   | `int`                         | Process exit code                           |
| `Output`     | `string`                      | Full captured stdout/stderr text            |
| `Lines`      | `ReadOnlyCollection<RunLine>` | Captured output lines, split into run lines |
