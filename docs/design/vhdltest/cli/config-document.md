# ConfigDocument Unit Design

## Overview

`ConfigDocument.cs` implements the `ConfigDocument` class, which deserializes the
YAML configuration file that specifies the VHDL test suite to run.

## Responsibilities

- Deserialize YAML configuration using YamlDotNet
- Expose test file lists and simulator name
- Provide a static `Parse` method for loading from a file path

## Data Model

| Property    | Type       | Description                       |
| ----------- | ---------- | --------------------------------- |
| `Simulator` | `string?`  | Name of simulator to use          |
| `Results`   | `string?`  | Default results file path         |
| `Tests`     | `string[]` | Array of test bench file patterns |
