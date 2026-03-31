# RunResults Unit Design

## Overview

`RunResults.cs` holds the results of executing an external simulator program.

## Data Model

| Property   | Type        | Description           |
| ---------- | ----------- | --------------------- |
| `ExitCode` | `int`       | Process exit code     |
| `Lines`    | `RunLine[]` | Captured output lines |
