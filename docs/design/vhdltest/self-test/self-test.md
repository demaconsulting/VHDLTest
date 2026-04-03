# SelfTest Subsystem Design

## Overview

The SelfTest subsystem implements VHDLTest's self-validation capability, allowing
users to verify that the tool is correctly installed and functioning in their environment.

## Units

| Unit       | File                     | Responsibility                             |
| ---------- | ------------------------ | ------------------------------------------ |
| Validation | `SelfTest/Validation.cs` | Executes and reports self-validation tests |

## Interactions

- Invoked by `Program.cs` when `--validate` is specified
- Uses embedded VHDL test files from `ValidationFiles/` for test execution
- Produces output via the `Context` I/O channels
- Optionally writes a TRX or JUnit results file when `--results` is specified
