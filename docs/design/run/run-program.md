# RunProgram Unit Design

## Overview

`RunProgram.cs` executes an external simulator program and captures its output.

## Responsibilities

- Launch an external process with specified arguments
- Capture stdout and stderr output
- Return a `RunResults` instance with exit code and output lines
