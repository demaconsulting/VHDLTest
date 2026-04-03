# Program Unit Design

## Overview

`Program.cs` is the entry point of VHDLTest. It parses the command-line context,
dispatches to the appropriate handler (version, help, validate, or test execution),
and returns a process exit code.

## Responsibilities

- Initialize the `Context` from command-line arguments
- Display version information when requested
- Display help/usage information when requested
- Invoke self-validation when requested
- Load configuration and execute VHDL tests when a configuration file is specified
- Return appropriate process exit codes

## Interactions

- Depends on `Cli/Context.cs` for command-line argument parsing
- Depends on `Cli/Options.cs` and `Cli/ConfigDocument.cs` for test configuration
- Depends on `Simulators/` for VHDL simulation
- Depends on `SelfTest/Validation.cs` for self-validation
