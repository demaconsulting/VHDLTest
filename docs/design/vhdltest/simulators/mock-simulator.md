# MockSimulator Unit Design

## Overview

`MockSimulator.cs` implements a deterministic in-process VHDL simulator used for
self-validation testing. It ships as part of the tool and is selected when the
`--simulator mock` flag is specified.

## Responsibilities

- Simulate compilation by inspecting each filename for `_error_`, `_warning_`,
  or `_info_` substrings to produce the corresponding output and exit code
- Simulate test execution by inspecting the test name for `_error_`, `_fail_`,
  `_warning_`, or `_info_` substrings to produce the corresponding output and exit code
- Classify compile output using `Info:`, `Warning:`, and `Error:` line prefixes
- Classify test output using `Info:`, `Warning:`, `Failure:`, and `Error:` line prefixes
