# Context Unit Design

## Overview

`Context.cs` implements the `Context` class, which is responsible for parsing the
raw command-line argument array and exposing the parsed flags and output channels
to the rest of the application.

## Responsibilities

- Parse `-v`/`--version`, `-?`/`-h`/`--help`, `--silent`, `--verbose`,
  `--validate`, `--depth`, `--results`, `--log`, and positional arguments
- Open an optional log-file writer when `--log` is specified
- Expose typed properties for each flag
- Write output to both console and log-file through `WriteLine`/`WriteError`
- Set an error exit code when `WriteError` is called

## Data Model

| Property      | Type                        | Description                                     |
| ------------- | --------------------------- | ----------------------------------------------- |
| `Version`     | `bool`                      | True when `--version` or `-v` was passed        |
| `Help`        | `bool`                      | True when `--help`, `-h`, or `-?` was passed    |
| `Silent`      | `bool`                      | True when `--silent` was passed                 |
| `Verbose`     | `bool`                      | True when `--verbose` was passed                |
| `ExitZero`    | `bool`                      | True when `--exit-zero` was passed              |
| `Validate`    | `bool`                      | True when `--validate` was passed               |
| `Depth`       | `int`                       | Heading depth for validation reports            |
| `ConfigFile`  | `string?`                   | Path to the YAML configuration file             |
| `ResultsFile` | `string?`                   | Path to results output file                     |
| `Simulator`   | `string?`                   | Name of simulator to use                        |
| `CustomTests` | `IReadOnlyList<string>?`    | Custom test names to run                        |
| `Errors`      | `int`                       | Count of errors written via WriteError          |
| `ExitCode`    | `int`                       | Process exit code (0 = success, 1 = failure)    |
