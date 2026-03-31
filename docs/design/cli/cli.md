# Cli Subsystem Design

## Overview

The Cli subsystem implements command-line interface handling for VHDLTest. It encompasses
all logic for parsing command-line arguments, reading configuration files, and managing
output channels.

## Units

| Unit           | File                    | Responsibility                                      |
| -------------- | ----------------------- | --------------------------------------------------- |
| Context        | `Cli/Context.cs`        | Parses command-line arguments and owns I/O channels |
| ConfigDocument | `Cli/ConfigDocument.cs` | Deserializes the YAML test configuration            |
| Options        | `Cli/Options.cs`        | Holds parsed configuration options                  |

## Interactions

The Cli subsystem is consumed primarily by `Program.cs`, which creates a `Context`
from the raw argument array, reads the config file path from it, then constructs
`Options` from a parsed `ConfigDocument`.
