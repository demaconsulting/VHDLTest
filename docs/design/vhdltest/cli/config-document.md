# ConfigDocument Unit Design

## Overview

`ConfigDocument.cs` implements the `ConfigDocument` class, which deserializes the
YAML configuration file that specifies the VHDL test suite to run.

## Responsibilities

- Deserialize YAML configuration using YamlDotNet
- Expose test file lists and test bench names
- Provide a static `ReadFile` method for loading from a file path

## Data Model

| Property | Type       | Description                       |
| -------- | ---------- | --------------------------------- |
| `Files`  | `string[]` | Array of VHDL source file paths   |
| `Tests`  | `string[]` | Array of test bench file patterns |
