# Options Unit Design

## Overview

`Options.cs` implements the `Options` record, which holds the resolved options
for a test run after combining command-line arguments with the configuration file.

## Responsibilities

- Hold the working directory and parsed `ConfigDocument`
- Provide a static `Parse` method that combines `Context` and file system state

## Data Model

| Property           | Type             | Description                                   |
| ------------------ | ---------------- | --------------------------------------------- |
| `WorkingDirectory` | `string`         | Resolved working directory for test execution |
| `Config`           | `ConfigDocument` | Deserialized configuration document           |
